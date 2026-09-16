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