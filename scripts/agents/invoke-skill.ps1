#Requires -Version 5.1

param(

    [Parameter(Mandatory)]

    [string]$Skill,

    [string]$Area = 'backend',

    [string[]]$ChangedFiles = @(),

    [string]$BaseRef = 'origin/main',

    [string]$Agent = 'backend',

    [string]$Title = '',

    [int]$Issue = 0

)



$Root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent

$SkillFile = Join-Path $Root ".skills/$Skill.skill.yaml"

$ScriptDir = $PSScriptRoot



if (-not (Test-Path $SkillFile)) {

    Write-Error "Skill not found: $SkillFile"

    exit 1

}



Write-Host "Skill: $Skill (area=$Area)"



Push-Location $Root

try {

    switch ($Skill) {

        'run-tests' {

            switch ($Area) {

                'backend' {

                    dotnet test backend/Tests/Arah.Tests/Arah.Tests.csproj --configuration Release

                }

                'bff' {

                    dotnet test backend/Tests/Arah.Tests.Bff/Arah.Tests.Bff.csproj --configuration Release

                }

                'flutter' {

                    Push-Location frontend/arah.app

                    flutter test

                    Pop-Location

                }

                'web' {

                    Push-Location frontend/devportal

                    if (Test-Path package.json) { npm test }

                    Pop-Location

                }

                default {

                    Write-Error "Unknown area: $Area"

                    exit 1

                }

            }

        }

        'gen-l10n' {

            & (Join-Path $ScriptDir 'gen-l10n.ps1')

        }

        'sync-docs' {

            & (Join-Path $ScriptDir 'sync-docs-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef

        }

        'doc-taxonomy' {

            & (Join-Path $ScriptDir 'generate-docs-index.ps1')

        }

        'register-bff-journey' {

            & (Join-Path $ScriptDir 'register-bff-journey-check.ps1') -ChangedFiles $ChangedFiles -BaseRef $BaseRef

        }

        'open-pr' {

            if (-not $Title) {

                Write-Error 'open-pr requires -Title'

                exit 1

            }

            & (Join-Path $ScriptDir 'open-pr.ps1') -Agent $Agent -Title $Title -Issue $Issue

        }

        'code-review' {

            Write-Host 'Checklist: docs/21_CODE_REVIEW.md, docs/22_COHESION_AND_TESTS.md'

            Get-Content (Join-Path $Root '.agents/templates/qa-checklist.md')

            exit 0

        }

        'dep-audit' {

            $sln = Get-ChildItem -Path $Root -Filter 'Arah.sln' | Select-Object -First 1

            if ($sln) {

                dotnet list $sln.FullName package --vulnerable --include-transitive

            }

            exit 0

        }

        'release-cut' {

            Write-Host 'release-cut: declarative — tag + CHANGELOG manual; ver .skills/release-cut.skill.yaml'

            exit 0

        }

        'address-bot-review' {

            if ($Issue -le 0) {

                Write-Error 'address-bot-review requires -Issue (PR number)'

                exit 1

            }

            & (Join-Path $ScriptDir 'address-bot-review.ps1') -PrNumber $Issue

        }

        'next-phase' {

            & (Join-Path $ScriptDir 'next-phase.ps1')

        }

        'spec-validate' {

            & (Join-Path $Root 'scripts/harness/validate-specs.ps1')

        }

        'harness-run' {

            $hParams = @{}
            if ($env:ARAH_SPEC_ID) { $hParams['SpecId'] = $env:ARAH_SPEC_ID }
            & (Join-Path $Root 'scripts/harness/run-harness.ps1') @hParams

        }

        'spec-author' {

            Write-Host 'spec-author: copie docs/specs/_template.spec.yaml → docs/specs/phases/<id>.spec.yaml'
            Write-Host 'Preencha acceptance + harness; rode spec-validate. Ver .skills/spec-author.skill.yaml'

            exit 0

        }

        default {

            Write-Host (Get-Content $SkillFile -Raw)

            Write-Host "Skill '$Skill' — ver $SkillFile e AGENTS.md"

            exit 0

        }

    }

} finally {

    Pop-Location

}


