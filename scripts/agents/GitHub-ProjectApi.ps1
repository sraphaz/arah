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
    Set-Content -Path $path -Value $raw.TrimEnd() + "`n" -Encoding UTF8 -NoNewline
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
        $layout = if ($name -match 'Kanban|Board') { 'BOARD' } else { 'TABLE' }
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
    }

    if ($Issue -and $Issue.state -eq 'OPEN') { return 'In Progress' }
    return 'Backlog'
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
