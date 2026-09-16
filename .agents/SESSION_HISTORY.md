# Session History

## 2026-09-16

- Goal: establish the CorelMate architecture and host integration foundation.
- Changed: created solution, modular projects, minimal panel, real local interop reference, documentation system, and smoke test.
- Verified: CorelDRAW Graphics Suite 2026 / internal 27.x; interop assembly 27.1.0.129; .NET SDK 10.0.103.
- Verified: Release solution build and dependency-free badge-grid smoke test.
- Verified: CorelDRAW 27.1.0.129 displayed the WPF Docker; show, hide, and reopen passed.
- Found: WinForms controls fail at ShowDocker; WPF UserControl succeeds.
- Not tested: automatic `.addon` startup loading or production packaging.
- Next: verify the official Docker/VSTA packaging path and prove the shell in-host.

## 2026-09-16 - Task 3 Packaging

- Goal: determine and package the supported CorelDRAW 2026 Docker deployment path without fabricating automatic startup metadata.
- Changed: added `scripts/Build-Release.ps1`, package-local install/remove scripts, release validation, and a generated ZIP workflow.
- Verified: clean release build, smoke test, package validation, extracted package installation, Docker visibility, and package uninstall against CorelDRAW 27.1.0.129.
- Finding: shipped v27 `.addon` files are zero-byte markers beside native DLLs; no verified .NET startup schema or VSTA tooling is available locally.
- Limitation: automatic startup after a fresh CorelDRAW launch remains unverified; no guessed `.addon` or registry workaround was added.