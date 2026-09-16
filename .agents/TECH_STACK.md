# Technology Stack

- C# with .NET Framework 4.8 for the in-process host/UI boundary.
- .NET Standard 2.0 for pure modules intended to remain host-independent.
- Installed CorelDRAW 2026, internal 27.x; local interop assembly `Corel.Interop.CorelDRAW.dll` version 27.1.0.129.
- WPF is used for the minimal panel because CorelDRAW v27.1 successfully hosts a WPF `UserControl`; WinForms `UserControl` and `Form` activation failed in the same API experiment.
- MSBuild via .NET SDK 10.0.103.
- No third-party packages in the foundation milestone.
- Tests are a dependency-free .NET 10 executable until the host-independent test suite grows enough to justify a framework.
- `scripts/Build-Release.ps1` creates a validated ZIP package without debug symbols or Corel-owned interop redistribution.
- Public documentation uses Markdown and the canonical PNG brand asset at `assets/brand/corelmate-logo.png`.
- CSV uses an internal standards-aware parser; XLSX uses `System.IO.Compression` and LINQ to XML against the Open XML package. No Excel installation, Office interop, or new third-party dependency is required.