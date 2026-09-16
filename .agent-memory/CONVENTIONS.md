# Conventions

- File-scoped C# namespaces and explicit `using` directives are used.
- Nullable reference types and warnings-as-errors are enabled globally.
- Project names use `CorelMate.<Module>` and source folders mirror project boundaries.
- Host/UI projects target .NET Framework 4.8; pure projects target .NET Standard 2.0.
- CorelDRAW/VGCore duplicate types are referenced through aliases.
- PowerShell is used for host bootstrap scripts.
- Existing project style includes compact XML project files in some modules; preserve local style rather than broad reformatting.
- The only current automated test is an executable with explicit failure exceptions and console success output.
- No project-wide logging implementation, API framework, database, frontend state library, or backend exists yet.
- Existing `.agents` files are detailed project documentation; update them only under an explicit task and through the mandatory PR workflow.