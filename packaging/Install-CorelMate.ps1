param(
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$dockerGuid = '{A63F9C4C-64A3-4E91-9AC9-3A15C783D5D7}'
$dockerClass = 'CorelMate.UI.CorelMatePanel'
$assemblyPath = Join-Path $PSScriptRoot 'CorelMate.UI.dll'

if (-not (Test-Path $assemblyPath -PathType Leaf)) {
    throw "CorelMate.UI.dll was not found beside this script. Extract the complete release package first."
}

$app = New-Object -ComObject CorelDRAW.Application
if ($app.Version -notlike 'Version 27.*') {
    throw "CorelMate 0.1.0 targets CorelDRAW 2026 / v27; detected $($app.Version)."
}

try {
    if ($app.FrameWork.IsDockerVisible($dockerGuid)) {
        $app.FrameWork.ShowDocker($dockerGuid)
    }
    else {
        $app.FrameWork.AddDocker($dockerGuid, $dockerClass, $assemblyPath)
        $app.FrameWork.ShowDocker($dockerGuid)
    }
}
catch {
    throw "CorelMate could not be registered in CorelDRAW $($app.Version): $($_.Exception.Message)"
}

Write-Output "CorelMate Docker registered for the current CorelDRAW session ($($app.Version))."
Write-Output 'Automatic startup through a CorelDRAW .addon package is not included because it is not verified for .NET assemblies on v27.1.'