# ============================================
# Criar e gerenciar clientes OAuth via gcloud (IAM OAuth clients)
# ============================================
# Estes comandos criam um *cliente OAuth do IAM* (Workload Identity / cenários
# server-side com client secret). Nao substituem o "ID do cliente OAuth" criado
# no Console (APIs e servicos > Credenciais) usado pelo "Sign in with Google"
# no Flutter - esse continua sendo criado manualmente no Console.
#
# Uso (substitua PROJECT_ID e nomes):
#   .\scripts\gcloud-oauth-client.ps1 -ProjectId MEU_PROJETO -Create
#   .\scripts\gcloud-oauth-client.ps1 -ProjectId MEU_PROJETO -List
#   .\scripts\gcloud-oauth-client.ps1 -ProjectId MEU_PROJETO -CreateCredential -OAuthClientId araponga-web
#
# Requer: gcloud instalado e gcloud auth login

param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectId,
    [switch]$Create,
    [switch]$List,
    [switch]$CreateCredential,
    [string]$OAuthClientId = "araponga-web",
    [string]$DisplayName = "Araponga OAuth app",
    [string]$RedirectUri = "http://localhost:5001/",
    [switch]$Help
)

$ErrorActionPreference = "Stop"
$Location = "global"

function Show-GcloudOAuthHelp {
    Write-Host ""
    Write-Host "Uso:" -ForegroundColor Cyan
    Write-Host "  .\scripts\gcloud-oauth-client.ps1 -ProjectId PROJECT_ID -Create"
    Write-Host "  .\scripts\gcloud-oauth-client.ps1 -ProjectId PROJECT_ID -List"
    Write-Host "  .\scripts\gcloud-oauth-client.ps1 -ProjectId PROJECT_ID -CreateCredential -OAuthClientId araponga-web"
    Write-Host ""
    Write-Host "Criar cliente OAuth (confidential client, authorization code):"
    Write-Host "  -Create          Cria o cliente OAuth com o nome em -OAuthClientId"
    Write-Host "  -OAuthClientId   ID do cliente (default: araponga-web)"
    Write-Host "  -DisplayName     Nome de exibicao"
    Write-Host "  -RedirectUri     URI de redirecionamento (default: http://localhost:5001/)"
    Write-Host ""
    Write-Host "Criar credencial (obter client secret):"
    Write-Host "  -CreateCredential  Cria uma credencial para o cliente; a saida inclui clientSecret."
    Write-Host ""
    Write-Host "Sign in with Google no Flutter: o Client ID usado pelo google_sign_in"
    Write-Host " (tipo xxx.apps.googleusercontent.com) e criado no Console:"
    Write-Host "  https://console.cloud.google.com/apis/credentials?project=$ProjectId"
    Write-Host "  Criar credenciais > ID do cliente OAuth > Aplicativo da Web (ou Android/iOS)"
    Write-Host ""
}

if ($Help) { Show-GcloudOAuthHelp; exit 0 }

if (-not (Get-Command gcloud -ErrorAction SilentlyContinue)) {
    Write-Error "gcloud nao encontrado. Instale o Google Cloud SDK e execute gcloud auth login."
    exit 1
}

if ($Create) {
    Write-Host "Criando cliente OAuth: $OAuthClientId (project: $ProjectId) ..." -ForegroundColor Cyan
    gcloud iam oauth-clients create $OAuthClientId `
        --project=$ProjectId `
        --location=$Location `
        --client-type="confidential-client" `
        --display-name=$DisplayName `
        --description="OAuth client for Araponga" `
        --allowed-scopes="openid,email,https://www.googleapis.com/auth/userinfo.email,https://www.googleapis.com/auth/userinfo.profile" `
        --allowed-redirect-uris=$RedirectUri `
        --allowed-grant-types="authorization-code-grant,refresh-token-grant"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Cliente OAuth criado. Para obter uma credencial (client secret), use -CreateCredential." -ForegroundColor Green
    }
    exit $LASTEXITCODE
}

if ($List) {
    Write-Host "Listando clientes OAuth (project: $ProjectId) ..." -ForegroundColor Cyan
    gcloud iam oauth-clients list --project=$ProjectId --location=$Location
    exit $LASTEXITCODE
}

if ($CreateCredential) {
    $credId = "${OAuthClientId}-cred-1"
    Write-Host "Criando credencial '$credId' para o cliente '$OAuthClientId' ..." -ForegroundColor Cyan
    gcloud iam oauth-clients credentials create $credId `
        --oauth-client=$OAuthClientId `
        --project=$ProjectId `
        --location=$Location `
        --display-name="Araponga OAuth client credential"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Para ver o client secret:" -ForegroundColor Green
        Write-Host "  gcloud iam oauth-clients credentials describe $credId --oauth-client=$OAuthClientId --project=$ProjectId --location=$Location"
    }
    exit $LASTEXITCODE
}

Show-GcloudOAuthHelp
Write-Host "Especifique -Create, -List ou -CreateCredential." -ForegroundColor Yellow
