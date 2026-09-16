# Contributing to CorelMate

Thank you for helping improve CorelMate. The project targets CorelDRAW 2026 on Windows, so reproducible host evidence matters as much as code quality.

## Before You Start

Read [AGENTS.md](AGENTS.md), [.agents/README.md](.agents/README.md), [.agents/CURRENT_STATE.md](.agents/CURRENT_STATE.md), [.agents/DEVELOPMENT_RULES.md](.agents/DEVELOPMENT_RULES.md), and [.agents/KNOWN_LIMITATIONS.md](.agents/KNOWN_LIMITATIONS.md).

## Required Workflow

Never work directly on `main`.

```powershell
git switch main
git pull --ff-only origin main
git switch -c feature/short-description
```

Make a focused change, run the narrowest useful tests, then run the solution build when the change crosses projects. Commit with a conventional prefix such as `feat:`, `fix:`, `docs:`, `test:`, or `chore:`. Push and open a PR:

```powershell
git push -u origin feature/short-description
gh pr create --base main --head feature/short-description
```

Merge through the PR only after checks and review pass. Update local `main` after merge and delete the completed branch.

## CorelDRAW Rules

- Keep CorelDRAW COM access in `CorelMate.Host`.
- Use the installed v27 interop assemblies; never invent APIs.
- Keep the Docker on WPF unless a new real-host test and ADR justify a change.
- Keep document mutation on the CorelDRAW/UI context.
- Use CorelDRAW command grouping for destructive operations where supported.
- Do not use clipboard automation for artwork operations.
- Mark host behavior unverified unless tested against CorelDRAW 27.1.0.129.

## Testing And Documentation

```powershell
dotnet build .\CorelMate.sln -c Release
dotnet run --project .\tests\CorelMate.Tests\CorelMate.Tests.csproj -c Release
```

For host changes, record the CorelDRAW version, document setup, operation, result, and anything not tested. Update relevant `.agents/` files, `.agent-memory/`, and changelog entries in the same PR.

## Pull Requests

Explain user-visible behavior, architecture impact, tests, CorelDRAW tests, and remaining limitations. Do not include secrets, generated build output, PDB files, or Corel-owned redistributables.