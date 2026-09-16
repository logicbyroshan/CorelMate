param(
    [string]$Configuration = 'Release',
    [string]$Version = '0.1.0'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'CorelMate.sln'
$outputRoot = Join-Path $root 'artifacts'
$staging = Join-Path $outputRoot "CorelMate-$Version"
$package = Join-Path $outputRoot "CorelMate-$Version-docker.zip"
$uiAssembly = Join-Path $root "src\CorelMate.UI\bin\$Configuration\net48\CorelMate.UI.dll"
$hostOutput = Join-Path $root "src\CorelMate.Host\bin\$Configuration\net48"
$managedAssemblies = @(
  'CorelMate.UI.dll',
  'CorelMate.Host.dll',
  'CorelMate.Badges.dll',
  'CorelMate.Infrastructure.dll',
  'CorelMate.Core.dll'
)

if (Test-Path $outputRoot) { Remove-Item $outputRoot -Recurse -Force }
New-Item $staging -ItemType Directory -Force | Out-Null

Get-ChildItem $root -Directory -Recurse -Force | Where-Object { $_.Name -in @('bin', 'obj') } | Remove-Item -Recurse -Force

dotnet restore $solution
if ($LASTEXITCODE -ne 0) { throw 'CorelMate restore failed.' }

dotnet build $solution -c $Configuration --no-restore -p:DebugType=None -p:DebugSymbols=false
if ($LASTEXITCODE -ne 0) { throw 'CorelMate build failed.' }

dotnet run --project (Join-Path $root 'tests\CorelMate.Tests\CorelMate.Tests.csproj') -c $Configuration --no-build
if ($LASTEXITCODE -ne 0) { throw 'CorelMate tests failed.' }

if (-not (Test-Path $uiAssembly -PathType Leaf)) { throw "Required Docker assembly was not built: $uiAssembly" }
foreach ($assemblyName in $managedAssemblies)
{
  $assemblyPath = if ($assemblyName -eq 'CorelMate.UI.dll') { $uiAssembly } else { Join-Path $hostOutput $assemblyName }
  if (-not (Test-Path $assemblyPath -PathType Leaf)) { throw "Required package assembly was not built: $assemblyPath" }
  Copy-Item $assemblyPath (Join-Path $staging $assemblyName)
}
Copy-Item (Join-Path $root 'packaging\Install-CorelMate.ps1') $staging
Copy-Item (Join-Path $root 'packaging\Uninstall-CorelMate.ps1') $staging

@"
CorelMate 0.1.0 Docker package

Target: CorelDRAW Graphics Suite 2026 / v27
Docker GUID: {A63F9C4C-64A3-4E91-9AC9-3A15C783D5D7}
Docker class: CorelMate.UI.CorelMatePanel

Install for the current CorelDRAW session:
  powershell -ExecutionPolicy Bypass -File .\Install-CorelMate.ps1

Remove from the current CorelDRAW session:
  powershell -ExecutionPolicy Bypass -File .\Uninstall-CorelMate.ps1

This package intentionally does not contain Corel-owned interop assemblies.
CorelDRAW provides those assemblies. It also does not claim automatic startup
through a .addon file because that .NET startup mechanism is not verified for
CorelDRAW v27.1.
"@ | Set-Content (Join-Path $staging 'README.txt') -Encoding ASCII

$files = @(Get-ChildItem $staging -File)
if ($files.Name -contains 'Corel.Interop.CorelDRAW.dll' -or $files.Name -contains 'Corel.Interop.VGCore.dll') { throw 'Corel-owned interop assemblies must not be packaged.' }
if ($files.Name | Where-Object { $_ -match '\.pdb$|\.user$|\.secret|appsettings|local' }) { throw 'Development-only or local files detected in package.' }
$content = $files | ForEach-Object { Get-Content $_.FullName -Raw -ErrorAction Stop }
if ($content | Where-Object { $_ -match 'C:\\Users\\|[A-Z]:\\|Program Files' }) { throw 'Absolute developer path detected in package contents.' }
foreach ($assemblyName in $managedAssemblies) { if (-not ($files.Name -contains $assemblyName)) { throw "$assemblyName is missing from package." } }

Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $package -CompressionLevel Optimal
Remove-Item $staging -Recurse -Force
Write-Output "Release package created: $package"