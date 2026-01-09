#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

cp "${repo_root}/README.md" "${repo_root}/docs/README.md"
cp "${repo_root}/CHANGELOG.md" "${repo_root}/docs/CHANGELOG.md"
cp "${repo_root}/CONTRIBUTING.md" "${repo_root}/docs/CONTRIBUTING.md"

cp "${repo_root}/docs/index.html" "${repo_root}/backend/Araponga.Api/wwwroot/index.html"
cp "${repo_root}/docs/styles.css" "${repo_root}/backend/Araponga.Api/wwwroot/styles.css"

echo "GitHub Pages sincronizado com docs/ e wwwroot." 
