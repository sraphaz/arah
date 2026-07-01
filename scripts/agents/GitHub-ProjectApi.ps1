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
        [hashtable]$Variables = @{},
        [switch]$AllowPartialErrors
    )
    $payload = @{ query = $Query; variables = $Variables }
    $json = $payload | ConvertTo-Json -Compress -Depth 20
    $raw = $json | gh api graphql --input - 2>&1 | Out-String
    $result = $null
    try {
        $result = $raw | ConvertFrom-Json
    } catch {
        throw "GraphQL failed (invalid JSON): $raw"
    }
    if ($result.errors -and -not $AllowPartialErrors) {
        if (-not $result.data) {
            throw "GraphQL errors: $($result.errors | ConvertTo-Json -Compress)"
        }
    }
    if ($LASTEXITCODE -ne 0 -and -not $result.data) {
        throw "GraphQL failed: $raw"
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
    if ($node) {
        $node = Merge-ProjectV2AllItems -Owner $Owner -ProjectNumber $ProjectNumber -BaseNode $node
    }
    return $node
}

function Merge-ProjectV2AllItems {
    param(
        [string]$Owner,
        [int]$ProjectNumber,
        [object]$BaseNode
    )
    $ownerType = Get-ProjectOwnerType -Owner $Owner
    $itemsQuery = if ($ownerType -eq 'organization') {
        @'
query($owner: String!, $number: Int!, $after: String) {
  organization(login: $owner) {
    projectV2(number: $number) {
      items(first: 100, after: $after) {
        pageInfo { hasNextPage endCursor }
        nodes {
          id
          content {
            ... on Issue { number url title state }
            ... on DraftIssue { title body }
          }
        }
      }
    }
  }
}
'@
    } else {
        @'
query($owner: String!, $number: Int!, $after: String) {
  user(login: $owner) {
    projectV2(number: $number) {
      items(first: 100, after: $after) {
        pageInfo { hasNextPage endCursor }
        nodes {
          id
          content {
            ... on Issue { number url title state }
            ... on DraftIssue { title body }
          }
        }
      }
    }
  }
}
'@
    }

    $allNodes = @()
    $cursor = $null
    do {
        $vars = @{ owner = $Owner; number = $ProjectNumber }
        if ($cursor) { $vars.after = $cursor }
        $result = Invoke-GhGraphql -Query $itemsQuery -Variables $vars
        $project = if ($ownerType -eq 'organization') { $result.data.organization.projectV2 } else { $result.data.user.projectV2 }
        $page = $project.items
        if ($page.nodes) { $allNodes += @($page.nodes) }
        if ($page.pageInfo.hasNextPage) { $cursor = $page.pageInfo.endCursor } else { $cursor = $null }
    } while ($cursor)

    $BaseNode | Add-Member -NotePropertyName 'items' -NotePropertyValue @{ nodes = $allNodes } -Force
    return $BaseNode
}

function Add-ProjectV2DraftIssue {
    param(
        [string]$ProjectId,
        [string]$Title,
        [string]$Body
    )
    $mutation = @'
mutation($projectId: ID!, $title: String!, $body: String!) {
  addProjectV2DraftIssue(input: { projectId: $projectId, title: $title, body: $body }) {
    projectItem { id }
  }
}
'@
    $r = Invoke-GhGraphql -Query $mutation -Variables @{
        projectId = $ProjectId
        title     = $Title
        body      = $Body
    }
    return $r.data.addProjectV2DraftIssue.projectItem.id
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

function Find-StatusOptionForTarget {
    param(
        [object]$StatusField,
        [string]$TargetStatus
    )
    if (-not $StatusField -or -not $StatusField.options) { return $null }

    $exact = $StatusField.options | Where-Object { $_.name -eq $TargetStatus } | Select-Object -First 1
    if ($exact) { return $exact }

    $aliases = @{
        'Backlog'       = @('Backlog', 'Todo', 'No Status', 'New')
        'Ready'         = @('Ready', 'Todo', 'Backlog')
        'In Progress'   = @('In Progress', 'In progress', 'Doing')
        'Review'        = @('Review', 'In review', 'In Review', 'In Progress')
        'Done'          = @('Done', 'Complete', 'Completed')
    }
    $candidates = if ($aliases.ContainsKey($TargetStatus)) { $aliases[$TargetStatus] } else { @($TargetStatus) }
    foreach ($name in $candidates) {
        $hit = $StatusField.options | Where-Object { $_.name -ieq $name } | Select-Object -First 1
        if ($hit) { return $hit }
    }
    return $null
}

function Set-ProjectItemStatusViaGraphql {
    param(
        [string]$ProjectId,
        [string]$ItemId,
        [string]$StatusFieldId,
        [string]$OptionId
    )
    $mutation = @'
mutation($projectId: ID!, $itemId: ID!, $fieldId: ID!, $optionId: String!) {
  updateProjectV2ItemFieldValue(input: {
    projectId: $projectId
    itemId: $itemId
    fieldId: $fieldId
    value: { singleSelectOptionId: $optionId }
  }) {
    projectV2Item { id }
  }
}
'@
    Invoke-GhGraphql -Query $mutation -Variables @{
        projectId = $ProjectId
        itemId    = $ItemId
        fieldId   = $StatusFieldId
        optionId  = $OptionId
    } | Out-Null
    return $true
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
    if ($LASTEXITCODE -eq 0) { return $true }
    try {
        Set-ProjectItemStatusViaGraphql -ProjectId $ProjectId -ItemId $ItemId -StatusFieldId $StatusFieldId -OptionId $OptionId | Out-Null
        return $true
    } catch {
        return $false
    }
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

    if ($PhaseItem -and $PhaseItem.status -eq 'completed') { return 'Done' }
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

function Get-GraphqlRateLimitRemaining {
    try {
        $rl = gh api rate_limit --jq '.resources.graphql' 2>$null | ConvertFrom-Json
        if ($rl) { return [int]$rl.remaining }
    } catch { }
    return -1
}

function Test-GraphqlRateLimitAvailable {
    param([int]$Minimum = 50)
    $remaining = Get-GraphqlRateLimitRemaining
    if ($remaining -lt 0) { return $true }
    return ($remaining -ge $Minimum)
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
    param(
        [string]$Owner,
        [string]$Title,
        [int]$ProjectNumber = 0
    )

    if ($ProjectNumber -gt 0) {
        try {
            $one = Get-ProjectV2Node -Owner $Owner -ProjectNumber $ProjectNumber
            if ($one) {
                if ($Title -and $one.title -ne $Title) {
                    Write-Warning "Project #$ProjectNumber título '$($one.title)' difere do config '$Title' — usando mesmo assim."
                }
                return $one
            }
        } catch {
            Write-Warning "project by number #$ProjectNumber : $_"
        }
    }

  # gh CLI — evita list GraphQL quando há projects inacessíveis no PAT
    try {
        $listed = gh project list --owner $Owner --limit 50 --format json 2>$null | ConvertFrom-Json
        if ($listed -and $listed.projects) {
            $hit = $listed.projects | Where-Object { $_.title -eq $Title } | Select-Object -First 1
            if ($hit) {
                return @{ id = $hit.id; number = [int]$hit.number; title = $hit.title; url = $hit.url }
            }
        }
    } catch {
        Write-Warning "gh project list: $_"
    }

    $q = @'
query {
  viewer {
    login
    projectsV2(first: 50) {
      nodes { id number title url }
    }
  }
}
'@
    try {
        $r = Invoke-GhGraphql -Query $q -AllowPartialErrors
        $nodes = @($r.data.viewer.projectsV2.nodes | Where-Object { $_ -and $_.title })
        $match = $nodes | Where-Object { $_.title -eq $Title } | Select-Object -First 1
        if ($match) { return $match }
    } catch {
        Write-Warning "viewer projectsV2: $_"
    }

    return $null
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
    if ($RepositoryId) { $vars.repoId = $RepositoryId }
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
  1) GitHub → Settings → Developer settings → PAT **Tokens (classic)**
     scopes: **project** + **repo** (fine-grained NÃO basta só repo)
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

    $existing = Find-ProjectV2ByTitle -Owner $owner -Title $Title -ProjectNumber $projectNumber
    if ($existing) {
        $projectNumber = [int]$existing.number
        $projectUrl = $existing.url
        $projectId = $existing.id
    } elseif (-not $DryRun) {
        try {
            $ownerId = Get-OwnerNodeId -Owner $owner
            $repoId = Get-RepositoryNodeId -Owner $owner -Name $repoName
            $created = New-ProjectV2 -OwnerId $ownerId -Title $Title -RepositoryId $repoId
            $projectNumber = [int]$created.number
            $projectUrl = $created.url
            $projectId = $created.id
        } catch {
            $hint = @'
Falha ao criar Project. Verifique o PAT (GH_PROJECT_TOKEN):

  PAT CLASSIC (recomendado): scopes project + repo
  PAT fine-grained: Account permissions → Projects → Read and write
  Se org com SAML: github.com/settings/tokens → Configure SSO → Authorize

NÃO use fine-grained só com permissões de repositório — Projects é permissão de CONTA.
'@
            Write-Error "$hint`n`nErro: $_"
        }
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
    $all = Find-AllProjectItemsForPhase -ProjectNode $ProjectNode -PhaseId $PhaseId -AllIssues $AllIssues
    if ($all.Count -gt 0) { return $all[0] }
    return $null
}

function Find-AllProjectItemsForPhase {
    param(
        [object]$ProjectNode,
        [string]$PhaseId,
        [array]$AllIssues = @()
    )
    $found = @()
    foreach ($item in $ProjectNode.items.nodes) {
        $content = $item.content
        if ($content -and $content.number) {
            $issue = $AllIssues | Where-Object { $_.number -eq $content.number } | Select-Object -First 1
            if ($issue -and (Test-PhaseIssueMarker -Issue $issue -PhaseId $PhaseId)) {
                $found += @{ item_id = $item.id; issue = $issue; kind = 'issue' }
                continue
            }
        }
        if ($content -and $content.body -and $content.body -match "arah-phase-draft id=$([regex]::Escape($PhaseId))(\s|-->)") {
            $found += @{ item_id = $item.id; issue = $null; kind = 'draft' }
            continue
        }
        if ($content -and $content.title -and $content.title -match "\b$([regex]::Escape($PhaseId))\b") {
            $found += @{ item_id = $item.id; issue = $null; kind = 'draft' }
        }
    }
    return $found
}
