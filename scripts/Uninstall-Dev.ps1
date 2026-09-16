$ErrorActionPreference = 'Stop'
$dockerGuid = '{A63F9C4C-64A3-4E91-9AC9-3A15C783D5D7}'
$app = New-Object -ComObject CorelDRAW.Application
$app.FrameWork.HideDocker($dockerGuid)
try { $app.FrameWork.RemoveDocker($dockerGuid) } catch { }
Write-Output 'CorelMate Docker removed from the current CorelDRAW session.'