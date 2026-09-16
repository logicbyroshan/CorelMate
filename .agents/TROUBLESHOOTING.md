# Troubleshooting

- If the host project cannot resolve `Corel.Interop.CorelDRAW`, verify the installed CorelDRAW version and update its `HintPath`.
- If .NET Framework 4.8 reference assemblies are missing, install the Visual Studio .NET Framework 4.8 Developer Pack.
- Do not copy random registration snippets from blogs; confirm them against the target CorelDRAW SDK.
- Close CorelDRAW before rebuilding the WPF Docker assembly if MSBuild reports that `CorelMate.UI.dll` is locked; the host loads the assembly into its process.
- The release ZIP is a session installer, not an automatic `.addon` package. Do not add a guessed marker/XML file to change that status.