param(
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'CorelMate.sln'
$uiAssembly = Join-Path $root "src\CorelMate.UI\bin\$Configuration\net48\CorelMate.UI.dll"
$dockerGuid = '{A63F9C4C-64A3-4E91-9AC9-3A15C783D5D7}'
$dockerClass = 'CorelMate.UI.CorelMatePanel'

dotnet build $solution -c $Configuration
if ($LASTEXITCODE -ne 0) { throw "CorelMate build failed." }

$app = New-Object -ComObject CorelDRAW.Application
$app.Visible = $true
$app.FrameWork.AddDocker($dockerGuid, $dockerClass, $uiAssembly)
$app.FrameWork.ShowDocker($dockerGuid)

Write-Output "CorelMate Docker shown in CorelDRAW $($app.Version)."
Write-Output 'Close or reopen it from Window > Dockers after registration.'