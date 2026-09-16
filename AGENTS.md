# CorelMate Agent Instructions

## Project

CorelMate is a Windows-first CorelDRAW productivity addon foundation. It targets CorelDRAW Graphics Suite 2026/v27 and currently provides a verified WPF Docker shell plus host-independent scaffolding for future badge generation, text-to-curves, and AI features. Feature implementation is intentionally incomplete.

## Source Of Truth

Use this order when information conflicts: current implementation, explicit user request, this file, current configuration, `.agent-memory/`, existing project documentation, then assumptions. The existing `.agents/` directory contains detailed CorelDRAW research and project history; consult it for host-specific context. The `.agent-memory/` directory is the persistent agent operating system created by this bootstrap.

Before meaningful work, read this file and the relevant `.agent-memory` files, then inspect the actual code path. Do not reread the entire repository when targeted inspection is sufficient.

## Repository Map

- `src/CorelMate.Core`: settings and host-independent core concepts.
- `src/CorelMate.Badges`: pure placeholder, row, and badge layout logic; no CorelDRAW dependency.
- `src/CorelMate.Curves`: future text-to-curves contract; implementation is intentionally unsupported.
- `src/CorelMate.AI`: provider abstraction only.
- `src/CorelMate.Infrastructure`: logging abstractions and infrastructure seams.
- `src/CorelMate.UI`: WPF Docker control targeting .NET Framework 4.8.
- `src/CorelMate.Host`: the only CorelDRAW/VGCore interop boundary and host adapter.
- `tests/CorelMate.Tests`: dependency-free .NET 10 executable smoke test.
- `scripts`: developer COM bootstrap and current-session Docker removal scripts.
- `.agents`: existing CorelDRAW/project documentation.
- `.agent-memory`: persistent agent audit, decisions, risks, and task history.

Important files include `CorelMate.sln`, `Directory.Build.props`, `README.md`, `src/CorelMate.Host/CorelMateAddIn.cs`, `src/CorelMate.Host/ICorelDrawHost.cs`, and `src/CorelMate.UI/CorelMatePanel.xaml`.

## How It Works

The developer script builds the solution, creates a CorelDRAW COM automation object, invokes `Application.FrameWork.AddDocker` with the permanent Docker GUID and WPF control class name, then shows the Docker. `CorelMate.Host` isolates CorelDRAW COM types; pure modules do not reference CorelDRAW or UI. The Docker currently reads no document data and performs no document mutation.

CorelDRAW 2026/v27.1 was verified to host a public parameterless WPF `UserControl`. WinForms controls failed during `ShowDocker`, so do not switch UI technology without a new real-host test and an ADR update.

## Commands

```powershell
dotnet build .\CorelMate.sln -c Release
dotnet run --project .\tests\CorelMate.Tests\CorelMate.Tests.csproj -c Release
powershell -ExecutionPolicy Bypass -File .\scripts\Install-Dev.ps1
powershell -ExecutionPolicy Bypass -File .\scripts\Uninstall-Dev.ps1
```

The host project contains a machine-specific CorelDRAW SDK `HintPath`; verify the installed v27 path before building on another machine. Host integration requires CorelDRAW and cannot be replaced by a unit test.

## Conventions

- C# uses file-scoped namespaces, explicit framework imports, nullable reference types, and warnings as errors from `Directory.Build.props`.
- Pure modules target `netstandard2.0`; host/UI target `net48` and x64 where required by the host.
- Corel interop aliases are used when CorelDRAW and VGCore export duplicate type names.
- PowerShell scripts are used for developer host automation.
- Tests are currently executable smoke tests with explicit assertions; do not claim CorelDRAW behavior from them.
- Keep CorelDRAW object-model access in `CorelMate.Host`; expose only narrow abstractions to feature code.
- Do not add dependencies or invent formatting conventions without recording the reason.

## Safety And Security

Never commit API keys, credentials, tokens, generated secrets, or machine-specific secret files. Do not add telemetry, network calls, automatic downloads, or background polling to the Docker. Keep CorelDRAW COM access on the host/UI context and do not move document operations to arbitrary background threads. Treat user artwork and future AI conversations as sensitive.

## Change Workflow: Mandatory GitHub PR Rule

This repository is hosted at `https://github.com/logicbyroshan/CorelMate.git`. **Never make a change directly on `main`.** Every change, including documentation or configuration changes, must follow this workflow:

1. Start from an up-to-date `main`.
2. Create a descriptive `feature/...`, `fix/...`, `bug/...`, `refactor/...`, `docs/...`, or `chore/...` branch.
3. Make the smallest focused change and update relevant memory/changelog files.
4. Build and run focused tests.
5. Commit with a meaningful conventional prefix such as `feat:`, `fix:`, `docs:`, `test:`, or `chore:`.
6. Push the branch to `origin` with upstream tracking.
7. Use `gh pr create` to open a PR; include summary and verification details.
8. Wait for required checks/review, then use `gh pr merge` or the GitHub PR UI to merge the PR. Do not push to `main` as a substitute.
9. Update local `main` only after the PR is merged, then delete the merged branch when appropriate.

Before pushing, inspect `git status`, the diff, secrets, branch name, and remote. Never force-push shared branches, rewrite another contributor's work, or merge a PR with failing required checks. If authentication, repository permissions, required checks, or merge conflicts block the workflow, report the blocker instead of bypassing the rule.

## Implementation Rules

- Read relevant memory and existing code before edits.
- Make the smallest safe change; do not refactor unrelated code.
- Never invent CorelDRAW APIs, registration keys, or packaging behavior.
- Preserve the modular host boundary and document significant architectural decisions.
- Do not implement badge generation, curves, AI, installer, licensing, or automatic addon startup without an explicit task.
- For bugs discovered during audits, record them in `.agent-memory/KNOWN_ISSUES.md`; do not opportunistically fix them.
- Test first at the narrowest useful level, then build the solution when the change is cross-project.
- Mark CorelDRAW-specific behavior unverified unless it was exercised against the installed host.

## Documentation Rules

For meaningful work, update the relevant `.agent-memory` files, append concise task history, and update the existing root `CHANGELOG.md` through the mandatory PR workflow. Do not rewrite historical entries. Keep `CURRENT_STATE.md`, architecture, decisions, known issues, and changelog consistent. Never store secrets in memory.