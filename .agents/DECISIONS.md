# Decisions

## ADR-001: Isolate the host boundary

Status: Accepted. The CorelDRAW COM interop reference stays in `CorelMate.Host`; pure logic does not depend on CorelDRAW or UI.

## ADR-002: Use the installed interop assembly

Status: Accepted for the local prototype. CorelDRAW 2026 exposes `Corel.Interop.CorelDRAW.dll`, version 27.1.0.129. The project must re-verify this path per target installation.

## ADR-003: Do not guess Docker registration

Status: Accepted. VSTA/Docker packaging is not verified locally, so the shell documents the boundary without fake registration code.

## ADR-004: Use WPF for the v27 Docker surface

Status: Accepted for the verified Docker surface; automatic addon startup remains unverified.

Context: CorelDRAW 2026 exposes `FrameWork.AddDocker` for a .NET assembly, but the hosted control type had to be verified.

Decision: Use a public parameterless WPF `UserControl` in a .NET Framework 4.8 UI assembly. Register it with the stable Docker GUID through `FrameWork.AddDocker`.

Alternatives considered: WinForms `UserControl`, WinForms `Form`, HTML/JavaScript, and native addon packaging.

Reason: WPF successfully rendered inside CorelDRAW v27.1. WinForms controls registered but failed during `ShowDocker` with `RPC_E_SERVERFAULT` and rendered blank.

Verification: CorelDRAW 2026 displayed the panel; show, hide, and reopen were exercised through the real COM API.