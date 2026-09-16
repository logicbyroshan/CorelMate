# Project Memory

CorelMate is a Windows-first CorelDRAW productivity addon foundation. The intended product will eventually automate repeated artwork/badges, convert text to curves, and provide an abstracted AI assistant. The current product is version 0.1.0 and intentionally stops at a host integration shell.

## Stack

- C#.
- .NET Framework 4.8 for CorelDRAW host and WPF Docker UI.
- .NET Standard 2.0 for pure modules.
- .NET 10 executable smoke tests because that runtime is installed locally.
- CorelDRAW Graphics Suite 2026/v27.1.0.129 interop assemblies.
- MSBuild via .NET SDK 10.0.103.
- No third-party packages.

## Users And Constraints

The target user is a CorelDRAW professional. CorelDRAW remains the host. Idle CPU/memory must stay low, no polling or unnecessary workers are allowed, document operations must respect the host thread and undo model, and secrets must never enter source or logs.

## Current Maturity

The WPF Docker shell has been displayed inside the installed CorelDRAW host and can be shown, hidden, and reopened through `FrameWork.AddDocker`. Feature modules are placeholders. Automatic `.addon` startup loading and production packaging are not verified.