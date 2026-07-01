#Requires -Version 5.1
<#
.SYNOPSIS
  Helpers GraphQL/gh para GitHub Project v2 (bootstrap, views, status).
#>

function Test-GhProjectScope {
    $status = gh auth status 2>&1 | Out-String
    return ($status -match 'project' -or $status -match 'read:project')
}

function Invoke-GhGraphql {
    param(
        [Parameter(Mandatory)][string]$Query,
        [hashtable]$Variables = @{}
    )
    $payload = @{ query = $Query; variables = $Variables }
    $json = $payload | ConvertTo-Json -Compress -Depth 20
    $raw = $json | gh api graphql --input - 2>&1
    if ($LASTEXITCODE -ne 0) { throw "GraphQL failed: $raw" }
    $result = $raw | ConvertFrom-Json
    if ($result.errors) {
        throw "GraphQL errors: $($result.errors | ConvertTo-Json -Compress)"
    }
    return $result
}

function Get-ProjectOwnerType {
    param([string]$Owner)
    $user = gh api "users/$Owner" --jq .type 2>$null
    if ($user -eq 'Organization') { return 'organization' }
    return 'user'
}

function Get-ProjectV2Node {
    param(
        [string]$Owner,
        [int]$ProjectNumber
    )
    $ownerType = Get-ProjectOwnerType -Owner $Owner
    if ($ownerType -eq 'organization') {
        $query = @'
query($owner: String!, $number: Int!) {
  organization(login: $owner) {
    projectV2(number: $number) {
      id
      number
      title
      url
      fields(first: 40) {
        nodes {
          ... on ProjectV2SingleSelectField {
            id
            name
            options { id name }
          }
          ... on ProjectV2Field { id name }
        }
      }
      items(first: 100) {
        nodes {
          id
          content {
            ... on Issue { number url title state }
            ... on DraftIssue { title body }
          }
        }
      }
      views(first: 20) {
        nodes { id name layout }
      }
    }
  }
}
'@
    } else {
        $query = @'
query($owner: String!, $number: Int!) {
  user(login: $owner) {
    projectV2(number: $number) {
      id
      number
      title
      url
      fields(first: 40) {
        nodes {
          ... on ProjectV2SingleSelectField {
            id
            name
            options { id name }
          }
          ... on ProjectV2Field { id name }
        }
      }
      items(first: 100) {
        nodes {
          id
          content {
            ... on Issue { number url title state }
            ... on DraftIssue { title body }
          }
        }
      }
      views(first: 20) {
        nodes { id name layout }
      }
    }
  }
}
'@
    }
    $result = Invoke-GhGraphql -Query $query -Variables @{ owner = $Owner; number = $ProjectNumber }
    $node = if ($ownerType -eq 'organization') { $result.data.organization.projectV2 } else { $result.data.user.projectV2 }
    return $node
}

function Set-ProjectNumberInConfig {
    param(
        [string]$Root,
        [int]$ProjectNumber
    )
    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'
    if (-not (Test-Path $path)) { return $false }
    $raw = Get-Content $path -Raw
    if ($raw -match '(?m)^project_number:\s*\d+') {
        $raw = $raw -replace '(?m)^project_number:\s*\d+', "project_number: $ProjectNumber"
    } else {
        $raw = $raw -replace '(?m)^project_number:\s*null', "project_number: $ProjectNumber"
    }
    Set-Content -Path $path -Value ($raw.TrimEnd() + "`n") -Encoding UTF8 -NoNewline
    return $true
}

function Get-StatusFieldFromProject {
    param($ProjectNode)
    foreach ($field in $ProjectNode.fields.nodes) {
        if ($field.name -eq 'Status' -and $field.options) {
            return $field
        }
    }
    return $null
}

function Update-ProjectStatusOptions {
    param(
        [string]$FieldId,
        [string[]]$OptionNames
    )
    $mutation = @'
mutation($fieldId: ID!, $options: [ProjectV2SingleSelectFieldOptionInput!]!) {
  updateProjectV2Field(input: { fieldId: $fieldId, singleSelectOptions: $options }) {
    projectV2Field {
      ... on ProjectV2SingleSelectField { options { id name } }
    }
  }
}
'@
    $options = @($OptionNames | ForEach-Object {
        @{ name = $_; color = 'GRAY'; description = '' }
    })
    return Invoke-GhGraphql -Query $mutation -Variables @{
        fieldId = $FieldId
        options = $options
    }
}

function Ensure-ProjectViews {
    param(
        [string]$ProjectId,
        [array]$ViewNames,
        [array]$ExistingViews
    )
    $existing = @($ExistingViews | ForEach-Object { $_.name })
    $created = @()
    foreach ($name in $ViewNames) {
        if ($existing -contains $name) { continue }
        $layout = if ($name -match 'Kanban|Board') { 'BOARD' }
                  elseif ($name -match 'Roadmap') { 'ROADMAP' }
                  else { 'TABLE' }
        $mutation = @'
mutation($projectId: ID!, $name: String!, $layout: ProjectV2ViewsLayout!) {
  createProjectV2View(input: { projectId: $projectId, name: $name, layout: $layout }) {
    projectView { id name layout }
  }
}
'@
        try {
            Invoke-GhGraphql -Query $mutation -Variables @{
                projectId = $ProjectId
                name      = $name
                layout    = $layout
            } | Out-Null
            $created += $name
        } catch {
            Write-Warning "View '$name' not created: $_"
        }
    }
    return $created
}

function Set-ProjectItemStatus {
    param(
        [string]$ProjectId,
        [string]$ItemId,
        [string]$StatusFieldId,
        [string]$OptionId
    )
    gh project item-edit `
        --project-id $ProjectId `
        --id $ItemId `
        --field-id $StatusFieldId `
        --single-select-option-id $OptionId 2>$null | Out-Null
    return ($LASTEXITCODE -eq 0)
}

function Get-OpenPullRequestsForRepo {
    param([string]$Repo)
    $prs = gh pr list --repo $Repo --state open --limit 100 --json number,title,body,url,reviewDecision,statusCheckRollup 2>$null | ConvertFrom-Json
    if (-not $prs) { return @() }
    return @($prs)
}

function Resolve-PhaseStatusColumn {
    param(
        [object]$Issue,
        [hashtable]$PhaseItem,
        [string]$Root,
        [array]$OpenPrs = @()
    )

    if ($Issue -and $Issue.state -eq 'CLOSED') { return 'Done' }

    $issueNum = if ($Issue) { $Issue.number } else { $null }
    $linkedPr = @()
    if ($issueNum) {
        $linkedPr = @($OpenPrs | Where-Object {
            $_.body -match "(?i)(close[sd]?|fix(?:e[sd])?)\s+#$issueNum\b"
        })
    }

    if ($linkedPr.Count -gt 0) {
        $needsReview = @($linkedPr | Where-Object {
            $_.reviewDecision -eq 'REVIEW_REQUIRED' -or $_.reviewDecision -eq 'CHANGES_REQUESTED'
        })
        if ($needsReview.Count -gt 0) { return 'Review' }
        return 'In Progress'
    }

    if ($PhaseItem) {
        foreach ($dep in $PhaseItem.blocked_by) {
            if ($dep -match '^FASE' -and -not (Test-PhaseCompleteFromGitHub -Root $Root -PhaseId $dep)) {
                return 'Backlog'
            }
        }
        # Fase desbloqueada: 'In Progress' se há issue aberta, senão 'Ready'
        if ($Issue -and $Issue.state -eq 'OPEN') { return 'In Progress' }
        return 'Ready'
    }

    if ($Issue -and $Issue.state -eq 'OPEN') { return 'In Progress' }
    return 'Backlog'
}

function Get-GhProjectToken {
    foreach ($name in @('GH_PROJECT_TOKEN', 'GH_AW_PROJECT_GITHUB_TOKEN')) {
        $val = [Environment]::GetEnvironmentVariable($name)
        if ($val) { return $val }
    }
    return $env:GH_TOKEN
}

function Test-GhProjectTokenAvailable {
    $token = Get-GhProjectToken
    if (-not $token) { return $false }
    try {
        $q = 'query { viewer { login } }'
        Invoke-GhGraphql -Query $q | Out-Null
        return $true
    } catch {
        return $false
    }
}

function Get-RepositoryNodeId {
    param([string]$Owner, [string]$Name)
    $q = 'query($o: String!, $n: String!) { repository(owner: $o, name: $n) { id } }'
    $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner; n = $Name }
    return $r.data.repository.id
}

function Get-OwnerNodeId {
    param([string]$Owner)
    $ownerType = Get-ProjectOwnerType -Owner $Owner
    if ($ownerType -eq 'organization') {
        $q = 'query($o: String!) { organization(login: $o) { id } }'
        $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner }
        return $r.data.organization.id
    }
    $q = 'query($o: String!) { user(login: $o) { id } }'
    $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner }
    return $r.data.user.id
}

function Find-ProjectV2ByTitle {
    param([string]$Owner, [string]$Title)
    $ownerType = Get-ProjectOwnerType -Owner $Owner
    if ($ownerType -eq 'organization') {
        $q = @'
query($o: String!) {
  organization(login: $o) {
    projectsV2(first: 30) {
      nodes { id number title url }
    }
  }
}
'@
        $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner }
        $nodes = $r.data.organization.projectsV2.nodes
    } else {
        $q = @'
query($o: String!) {
  user(login: $o) {
    projectsV2(first: 30) {
      nodes { id number title url }
    }
  }
}
'@
        $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner }
        $nodes = $r.data.user.projectsV2.nodes
    }
    return $nodes | Where-Object { $_.title -eq $Title } | Select-Object -First 1
}

function New-ProjectV2 {
    param([string]$OwnerId, [string]$Title, [string]$RepositoryId = $null)
    $q = if ($RepositoryId) {
        @'
mutation($ownerId: ID!, $title: String!, $repoId: ID!) {
  createProjectV2(input: { ownerId: $ownerId, title: $title, repositoryId: $repoId }) {
    projectV2 { id number title url }
  }
}
'@
    } else {
        @'
mutation($ownerId: ID!, $title: String!) {
  createProjectV2(input: { ownerId: $ownerId, title: $title }) {
    projectV2 { id number title url }
  }
}
'@
    }
    $vars = @{ ownerId = $OwnerId; title = $Title }
    if ($RepositoryId) { $vars.repositoryId = $RepositoryId }
    $r = Invoke-GhGraphql -Query $q -Variables $vars
    return $r.data.createProjectV2.projectV2
}

function Link-ProjectV2ToRepository {
    param([string]$ProjectId, [string]$RepositoryId)
    $q = @'
mutation($projectId: ID!, $repositoryId: ID!) {
  linkProjectV2ToRepository(input: { projectId: $projectId, repositoryId: $repositoryId }) {
    repository { nameWithOwner }
  }
}
'@
    Invoke-GhGraphql -Query $q -Variables @{ projectId = $ProjectId; repositoryId = $RepositoryId } | Out-Null
}

function Get-IssueNodeId {
    param([string]$Owner, [string]$RepoName, [int]$IssueNumber)
    $q = 'query($o: String!, $n: String!, $num: Int!) { repository(owner: $o, name: $n) { issue(number: $num) { id } } }'
    $r = Invoke-GhGraphql -Query $q -Variables @{ o = $Owner; n = $RepoName; num = $IssueNumber }
    return $r.data.repository.issue.id
}

function Add-IssueToProjectViaGraphql {
    param(
        [string]$ProjectId,
        [string]$IssueNodeId
    )
    $q = @'
mutation($projectId: ID!, $contentId: ID!) {
  addProjectV2ItemById(input: { projectId: $projectId, contentId: $contentId }) {
    item { id }
  }
}
'@
    $r = Invoke-GhGraphql -Query $q -Variables @{ projectId = $ProjectId; contentId = $IssueNodeId }
    return $r.data.addProjectV2ItemById.item.id
}

function Set-ProjectConfigInYaml {
    param(
        [string]$Root,
        [int]$ProjectNumber,
        [string]$ProjectUrl = ''
    )
    $path = Join-Path $Root '.github/project/arah-sustentacao.yml'
    if (-not (Test-Path $path)) { return $false }
    $raw = Get-Content $path -Raw
    if ($raw -match '(?m)^project_number:\s*\d+') {
        $raw = $raw -replace '(?m)^project_number:\s*\d+', "project_number: $ProjectNumber"
    } else {
        $raw = $raw -replace '(?m)^project_number:\s*null', "project_number: $ProjectNumber"
    }
    if ($ProjectUrl) {
        if ($raw -match '(?m)^project_url:\s*.+$') {
            $raw = $raw -replace '(?m)^project_url:\s*.+$', "project_url: `"$ProjectUrl`""
        } else {
            $raw = $raw -replace '(?m)^project_number:\s*(\d+)', "project_number: `$1`nproject_url: `"$ProjectUrl`""
        }
    }
    Set-Content -Path $path -Value ($raw.TrimEnd() + "`n") -Encoding UTF8 -NoNewline
    return $true
}

function Ensure-ProjectV2Bootstrap {
    param(
        [string]$Root,
        [string]$Title,
        [switch]$DryRun,
        [switch]$Json
    )

    if (-not (Test-GhProjectTokenAvailable)) {
        $msg = @'
Token sem scope project. GITHUB_TOKEN do Actions NÃO acessa Projects v2.

Uma vez:
  1) GitHub → Settings → Developer settings → PAT (classic)
     scopes: project + repo
  2) Repo → Settings → Secrets → Actions → GH_PROJECT_TOKEN

Local:
  gh auth refresh -h github.com -s project,read:project

CI:
  Actions → "Bootstrap GitHub Project" → Run workflow
'@
        Write-Warning $msg
        if ($Json) {
            @{ warning = 'project_token_missing'; title = $Title; hint = 'GH_PROJECT_TOKEN' } | ConvertTo-Json
        }
        return @{ project_number = 0; warning = 'project_token_missing' }
    }

    $repoMeta = gh repo view --json nameWithOwner -q .nameWithOwner
    $owner, $repoName = $repoMeta -split '/'

    $cfg = Get-ProjectConfig -Root $Root
    $projectNumber = if ($cfg -and $cfg.project_number -gt 0) { $cfg.project_number } else { 0 }
    $projectUrl = $null
    $projectId = $null

    $existing = Find-ProjectV2ByTitle -Owner $owner -Title $Title
    if ($existing) {
        $projectNumber = [int]$existing.number
        $projectUrl = $existing.url
        $projectId = $existing.id
    } elseif (-not $DryRun) {
        $ownerId = Get-OwnerNodeId -Owner $owner
        $repoId = Get-RepositoryNodeId -Owner $owner -Name $repoName
        $created = New-ProjectV2 -OwnerId $ownerId -Title $Title -RepositoryId $repoId
        $projectNumber = [int]$created.number
        $projectUrl = $created.url
        $projectId = $created.id
    }

    if ($projectNumber -gt 0 -and $projectId -and -not $DryRun) {
        try {
            $repoId = Get-RepositoryNodeId -Owner $owner -Name $repoName
            Link-ProjectV2ToRepository -ProjectId $projectId -RepositoryId $repoId
        } catch {
            Write-Warning "link repo (pode já estar ligado): $_"
        }
        Set-ProjectConfigInYaml -Root $Root -ProjectNumber $projectNumber -ProjectUrl $projectUrl | Out-Null
    }

    $result = [ordered]@{
        project_number = $projectNumber
        project_url    = $projectUrl
        title          = $Title
        dry_run        = [bool]$DryRun
    }
    # Retorna o objeto ao chamador (Invoke-ProjectEnsure faz a serialização);
    # não emitir no pipeline aqui para não poluir o valor capturado.
    return $result
}

function Find-ProjectItemForPhase {
    param(
        [object]$ProjectNode,
        [string]$PhaseId,
        [array]$AllIssues = @()
    )

    foreach ($item in $ProjectNode.items.nodes) {
        $content = $item.content
        if ($content -and $content.number) {
            $issue = $AllIssues | Where-Object { $_.number -eq $content.number } | Select-Object -First 1
            if ($issue -and ($issue.body -match "arah-next-phase id=$([regex]::Escape($PhaseId))" -or $issue.title -match [regex]::Escape($PhaseId))) {
                return @{ item_id = $item.id; issue = $issue; kind = 'issue' }
            }
        }
        if ($content -and $content.title -and $content.title -match [regex]::Escape($PhaseId)) {
            return @{ item_id = $item.id; issue = $null; kind = 'draft' }
        }
    }
    return $null
}
