#!/usr/bin/env pwsh
# Render todos los diagramas Mermaid (.mmd) en docs/diagrams a PNG.
# Requiere Node.js (descarga @mermaid-js/mermaid-cli vía npx la primera vez).

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$diagramsDir = Join-Path $root "docs\diagrams"

if (-not (Test-Path $diagramsDir)) {
    Write-Error "No existe el directorio $diagramsDir"
    exit 1
}

$files = Get-ChildItem -Path $diagramsDir -Filter "*.mmd"
Write-Host "Renderizando $($files.Count) diagramas Mermaid a PNG..." -ForegroundColor Cyan
Write-Host ""

# Configuración del tema (light, ancho fijo)
$cfg = @{
    theme = "default"
    themeVariables = @{
        primaryColor      = "#2563EB"
        primaryTextColor  = "#FFFFFF"
        primaryBorderColor= "#1E40AF"
        lineColor         = "#374151"
        secondaryColor    = "#F3F4F6"
        tertiaryColor     = "#FBBF24"
        fontFamily        = "Segoe UI, Arial, sans-serif"
    }
} | ConvertTo-Json -Depth 5

$cfgPath = Join-Path $diagramsDir ".mermaid-config.json"
$cfg | Set-Content -Path $cfgPath -Encoding utf8

$ok = 0
$fail = 0
foreach ($file in $files) {
    $output = [System.IO.Path]::ChangeExtension($file.FullName, ".png")
    Write-Host "  $($file.Name)" -NoNewline

    $args = @(
        "@mermaid-js/mermaid-cli@11.4.2",
        "-i", $file.FullName,
        "-o", $output,
        "-c", $cfgPath,
        "-w", "1600",
        "-b", "white",
        "--quiet"
    )

    try {
        & npx --yes @args 2>$null
        if ($LASTEXITCODE -eq 0 -and (Test-Path $output)) {
            Write-Host "  OK" -ForegroundColor Green
            $ok++
        } else {
            Write-Host "  FAIL (exit $LASTEXITCODE)" -ForegroundColor Red
            $fail++
        }
    } catch {
        Write-Host "  FAIL ($_)" -ForegroundColor Red
        $fail++
    }
}

Write-Host ""
Write-Host "=== Resumen ===" -ForegroundColor Cyan
Write-Host "OK:   $ok / $($files.Count)" -ForegroundColor Green
Write-Host "Fail: $fail / $($files.Count)" -ForegroundColor $(if ($fail -gt 0) { "Red" } else { "Gray" })
