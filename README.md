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

## Current host status

The WPF Docker has been verified inside the installed CorelDRAW 2026 / v27.1 host. Automatic startup from a `.addon` package and production packaging are still unverified. See [CORELDRAW_INTEGRATION.md](.agents/CORELDRAW_INTEGRATION.md) for evidence and limitations.

Read [.agents/README.md](.agents/README.md) before making architectural changes.