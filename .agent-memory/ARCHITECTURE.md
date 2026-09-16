# Architecture Memory

## Modules

`CorelMate.Core`, `Badges`, `Curves`, `AI`, and `Infrastructure` are host-independent library projects. `CorelMate.UI` is the WPF Docker surface. `CorelMate.Host` is the only project that references `Corel.Interop.CorelDRAW.dll` and `Corel.Interop.VGCore.dll`. `CorelMate.Tests` is a small executable smoke test.

## Runtime Flow

`scripts/Install-Dev.ps1` builds the solution, creates a CorelDRAW COM automation object, calls `Application.FrameWork.AddDocker(DockerGuid, DockerClassName, AssemblyPath)`, and calls `ShowDocker`. The WPF `CorelMatePanel` is then rendered by CorelDRAW. `Uninstall-Dev.ps1` hides and removes the Docker from the current session.

## Boundaries

`ICorelDrawHost` exposes the application and active document/page/layer/selection through a narrow host adapter, but feature code must not spread raw COM objects. No current feature mutates CorelDRAW artwork. Settings and logging are abstractions only.

## Packaging Boundary

The installed suite contains native addon folders with DLLs and zero-byte `.addon` markers. CorelMate has not verified automatic startup registration for a .NET assembly; the current reproducible path is developer COM bootstrap, not a production addon package.