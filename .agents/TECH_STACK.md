# Technology Stack

- C# with .NET Framework 4.8 for the in-process host/UI boundary.
- .NET Standard 2.0 for pure modules intended to remain host-independent.
- Installed CorelDRAW 2026, internal 27.x; local interop assembly `Corel.Interop.CorelDRAW.dll` version 27.1.0.129.
- WPF is used for the minimal panel because CorelDRAW v27.1 successfully hosts a WPF `UserControl`; WinForms `UserControl` and `Form` activation failed in the same API experiment.
- MSBuild via .NET SDK 10.0.103.
- No third-party packages in the foundation milestone.
- Tests are a dependency-free .NET 10 executable until the host-independent test suite grows enough to justify a framework.