#!/usr/bin/env pwsh
# Script de utilidad: aplica migraciones y arranca la API en modo dev.
# Uso: ./scripts/seed-and-run.ps1
$ErrorActionPreference = "Stop"
Write-Host "=== Full Internet Services - Bootstrap ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "[1/3] Aplicando migraciones EF Core..." -ForegroundColor Yellow
dotnet ef database update `
    --project src/FIS.Infrastructure `
    --startup-project src/FIS.Api
Write-Host ""
Write-Host "[2/3] Compilando solucion..." -ForegroundColor Yellow
dotnet build FullInternetServices.sln --configuration Debug --nologo
Write-Host ""
Write-Host "[3/3] Iniciando FIS.Api en https://localhost:7001" -ForegroundColor Green
Write-Host "   - Swagger UI:     https://localhost:7001/swagger" -ForegroundColor Gray
Write-Host "   - Login demo:     admin / Admin123*" -ForegroundColor Gray
Write-Host ""
dotnet run --project src/FIS.Api --launch-profile https
