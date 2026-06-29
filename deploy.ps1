# LostPeople RD - Script de despliegue para producción
# Requisitos: PowerShell 7+, Docker, SQL Server (local o remoto)
# Uso: ./deploy.ps1 [-Environment production] [-DbPassword "tu_password"]

param(
    [string]$Environment = "production",
    [string]$DbPassword = ""
)

$ErrorActionPreference = "Stop"
$ROOT = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "=== LostPeople RD - Despliegue ===" -ForegroundColor Cyan
Write-Host "Entorno: $Environment" -ForegroundColor Yellow

# 1. Validar requisitos
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK no encontrado. Instalar desde https://dotnet.microsoft.com/download"
}

if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Docker no encontrado. Usando dotnet run directo." -ForegroundColor Yellow
    $USE_DOCKER = $false
} else {
    $USE_DOCKER = $true
}

# 2. Pedir password si no se proporciono
if ([string]::IsNullOrEmpty($DbPassword)) {
    $DbPassword = Read-Host -Prompt "Password de SQL Server (sa)" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($DbPassword)
    $DbPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# 3. Construir
Write-Host "`n=== Construyendo solucion ===" -ForegroundColor Cyan
Set-Location $ROOT
dotnet restore
dotnet build -c Release --no-restore

if ($LASTEXITCODE -ne 0) {
    throw "Error de compilacion"
}

Write-Host "Compilacion exitosa!" -ForegroundColor Green

# 4. Publicar
Write-Host "`n=== Publicando ===" -ForegroundColor Cyan
$publishDir = Join-Path $ROOT "publish"
if (Test-Path $publishDir) { Remove-Item -Path $publishDir -Recurse -Force }
dotnet publish src/Web/LostPeople.Web.csproj -c Release -o $publishDir --no-build

# 5. Variables de entorno
$env:LOSTPEOPLE_DB_SERVER = if ($USE_DOCKER) { "sqlserver" } else { "localhost" }
$env:LOSTPEOPLE_DB_NAME = "LostPeople"
$env:LOSTPEOPLE_DB_USER = "sa"
$env:LOSTPEOPLE_DB_PASSWORD = $DbPassword
$env:ASPNETCORE_ENVIRONMENT = $Environment
$env:ASPNETCORE_URLS = "http://+:5000"

# 6. Iniciar
if ($USE_DOCKER) {
    Write-Host "`n=== Iniciando con Docker Compose ===" -ForegroundColor Cyan
    $env:LOSTPEOPLE_DB_PASSWORD = $DbPassword
    docker-compose up -d --build
    Write-Host "App iniciada en http://localhost:5000" -ForegroundColor Green
} else {
    Write-Host "`n=== Iniciando con dotnet run ===" -ForegroundColor Cyan
    Write-Host "Variables de entorno configuradas:"
    Write-Host "  LOSTPEOPLE_DB_SERVER = $env:LOSTPEOPLE_DB_SERVER"
    Write-Host "  LOSTPEOPLE_DB_NAME = $env:LOSTPEOPLE_DB_NAME"
    Write-Host "  ASPNETCORE_ENVIRONMENT = $env:ASPNETCORE_ENVIRONMENT"
    Write-Host "  ASPNETCORE_URLS = $env:ASPNETCORE_URLS"

    Set-Location $publishDir
    dotnet LostPeople.Web.dll
}

Write-Host "`n=== Despliegue completado ===" -ForegroundColor Cyan
