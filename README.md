# CorelMate

CorelMate is a Windows-first CorelDRAW productivity addon foundation. Version 0.1.0 establishes modular code boundaries, a CorelDRAW 2026 (internal 27.x) interop host adapter, and a minimal status panel. Badge generation, text-to-curves, and AI are intentionally not implemented yet.

## Build

This machine has .NET SDK 10.0.103. Build the solution with:

```powershell
dotnet build .\CorelMate.sln -c Release
dotnet run --project .\tests\CorelMate.Tests\CorelMate.Tests.csproj -c Release
```

The host project references the installed CorelDRAW interop assembly at `C:\Program Files\Corel\CorelDRAW Graphics Suite\27\Programs64\Assemblies\Corel.Interop.CorelDRAW.dll`. On another machine, update that path in `src/CorelMate.Host/CorelMate.Host.csproj` after verifying the matching CorelDRAW installation.

## Run the verified Docker shell

After building, use the developer bootstrap:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Install-Dev.ps1
```

This invokes CorelDRAW's `FrameWork.AddDocker` through COM automation and shows the WPF Docker. Remove it from the current session with:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Uninstall-Dev.ps1
```

## Build a release package

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Build-Release.ps1
```

This cleans generated output, restores, builds without debug symbols, runs the smoke test, validates package contents, and creates `artifacts\CorelMate-0.1.0-docker.zip`. The ZIP contains the WPF UI plus its CorelMate managed dependencies and package-local session install/remove scripts. It excludes Corel-owned interop assemblies, source, tests, PDBs, and developer configuration.

Extract the ZIP and run `Install-CorelMate.ps1` to register the Docker for the current CorelDRAW session. Run `Uninstall-CorelMate.ps1` to remove it. Automatic startup through a CorelDRAW `.addon` package is not claimed because the .NET startup mechanism is not verified for v27.1.

## Current host status

The WPF Docker and generated ZIP session-install workflow have been verified inside the installed CorelDRAW 2026 / v27.1 host. Automatic startup from a `.addon` package remains unverified. See [CORELDRAW_INTEGRATION.md](.agents/CORELDRAW_INTEGRATION.md) for evidence and limitations.

Read [.agents/README.md](.agents/README.md) before making architectural changes.