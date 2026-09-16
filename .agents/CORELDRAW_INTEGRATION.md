# CorelDRAW Integration

## Verified locally

The machine has CorelDRAW Graphics Suite 2026 under `C:\Program Files\Corel\CorelDRAW Graphics Suite\27`. Registry key `HKLM\SOFTWARE\Corel\CorelDRAW\27.0` reports `ProgramsDir` and `PluginDir`. The installation contains `Programs64\Assemblies\Corel.Interop.CorelDRAW.dll` (assembly version 27.1.0.129), its required `Corel.Interop.VGCore.dll`, `Programs64\TypeLibs\CorelDRAW.tlb`, and an exported `Corel.Interop.CorelDRAW.Application` type. The interop assembly targets CLR v4.0.

## Current approach

`CorelMate.Host` targets .NET Framework 4.8, x64, and references the installed CorelDRAW and VGCore interop DLLs with `EmbedInteropTypes=false`. This uses the real CorelDRAW Object Model reference and keeps it out of pure business modules. `CorelMate.UI` is a WPF assembly targeting .NET Framework 4.8. Its public parameterless `CorelMatePanel` is registered with `Application.FrameWork.AddDocker`.

## Verified v27.1 API behavior

Reflection over the installed `Corel.Interop.CorelDRAW.dll` and `Corel.Interop.VGCore.dll` confirmed:

- `Corel.Interop.CorelDRAW.Application.FrameWork` returns `Corel.Interop.VGCore.FrameWork`.
- `Corel.Interop.VGCore.ICUIFrameWork.AddDocker` exists as `AddDocker(string Guid, string ClassName, string AssemblyPath)`.
- The installed assembly exposes `ICUIFrameWork.ShowDocker`, `HideDocker`, `IsDockerVisible`, and `RemoveDocker`.
- CorelDRAW 2026 successfully hosted a public parameterless WPF `System.Windows.Controls.UserControl` from CorelMate.UI.
- The same test with WinForms `UserControl` and `Form` returned `RPC_E_SERVERFAULT` on `ShowDocker` and produced a blank view.

## Official sources and verification status

| Source | URL | Version | Finding | Status |
|---|---|---|---|---|
| CorelDRAW SDK landing page | https://www.coreldraw.com/en/pages/sdk/ | Current site | Official SDK entry point | URL reachable; content extraction blocked by anti-bot protection |
| CorelDRAW SDK Guide | https://community.coreldraw.com/sdk/w/guide/214/overview | Historical/current guide entry point | Official developer guide entry point | URL returned an anti-bot page; local API reflection used for the installed v27 contract |
| CorelDRAW API reference: AddDocker | https://community.coreldraw.com/sdk/api/draw/27.1/ICUIFrameWork/AddDocker | 27.1 | Official URL matches the installed `AddDocker` signature | Direct page blocked by anti-bot protection; signature verified locally |
| Installed Corel interop assemblies | `Programs64\Assemblies` and `TypeLibs` | 27.1.0.129 / v27 | Exposes FrameWork and the actual COM method surface | Verified locally |

The official Corel pages were not usable as machine-readable content because the current site returned an Imperva/Incapsula challenge. The project therefore records the URLs and labels the online prose as unverified, while treating the installed interop metadata and real CorelDRAW execution as the v27 evidence.

## Addon/package boundary

The installed CorelDRAW suite contains native addon directories under `Programs64\Addons\<Name>` with a native DLL and zero-byte `.addon` marker files. No official .NET `.addon` package template, VSTA tool, or automatic startup registration artifact was found locally. CorelMate currently uses the supported-looking `FrameWork.AddDocker` .NET assembly path through `scripts\Install-Dev.ps1`; this is a reproducible developer bootstrap, not a production installer or automatic startup addon.

## Release package

`scripts\Build-Release.ps1` produces `artifacts\CorelMate-0.1.0-docker.zip`. The package contains `CorelMate.UI.dll`, the verified .NET Framework 4.8 WPF Docker assembly, package-local session install/remove scripts, and a README. It deliberately excludes `Corel.Interop.CorelDRAW.dll` and `Corel.Interop.VGCore.dll`; CorelDRAW supplies those Corel-owned host assemblies. Source, tests, PDBs, local configuration, and developer scripts are also excluded.

Release validation checks required files, prohibited assemblies, debug/development artifacts, secrets, and absolute developer paths. The package-local installer was tested from a temporary extracted directory against CorelDRAW 27.1.0.129: the Docker became visible, then the package-local uninstaller removed it. This verifies session installation only, not startup after a fresh CorelDRAW launch.

## Developer installation

From the repository root, run `powershell -ExecutionPolicy Bypass -File .\scripts\Install-Dev.ps1`. The script builds the solution, creates a CorelDRAW COM automation object, calls `AddDocker`, and calls `ShowDocker`. The Docker can then be hidden/reopened through CorelDRAW's Docker UI. Run `scripts\Uninstall-Dev.ps1` to remove the Docker from the current session. These scripts do not copy files into Corel's installation directory.

## Remaining unverified behavior

No VSTA executable or VSTA runtime was found in the searched Visual Studio locations. Automatic CorelDRAW startup loading from a `.addon` package has not been verified. The current proven path requires the developer bootstrap script or an equivalent Corel macro/command to invoke `FrameWork.AddDocker`.

No registry hack, DLL injection, polling process, executable modification, or guessed XML/.addon schema was added.

Official starting points: [CorelDRAW SDK](https://www.coreldraw.com/en/pages/sdk/) and [CorelDRAW SDK Guide](https://community.coreldraw.com/sdk/w/guide/214/overview).

## Concepts

GMS is CorelDRAW's macro storage format. VSTA is Microsoft's Visual Studio Tools for Applications integration used by some host applications. A Docker is CorelDRAW's dockable tool-window surface. The Object Model exposes the application, documents, pages, layers, and shapes through COM interop. These concepts must be confirmed against the target release before implementation.