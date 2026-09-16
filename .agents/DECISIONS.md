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

## ADR-005: Ship A Session-Install ZIP, Not An Unverified Startup Addon

Status: Accepted for Task 3; automatic startup remains unverified.

Context: v27.1 exposes `FrameWork.AddDocker`, but local shipped `.addon` files are zero-byte markers beside native DLLs. No verified .NET `.addon` schema, startup callback, or VSTA runtime is available on this machine, and official web pages are blocked from machine-readable extraction.

Decision: Ship a reproducible ZIP containing `CorelMate.UI.dll` and package-local PowerShell install/remove scripts. The scripts use the verified COM API for the current CorelDRAW session. Do not generate a guessed `.addon` file.

Alternatives: Invent an XML/.addon package, modify CorelDRAW installation files, use registry hacks, or run a background startup process.

Reason: The ZIP uses an observed API and tested artifact path without unsupported startup behavior or host modification.

Consequences: Users must run the package installer for a session; automatic startup is a documented limitation and remains a future research task.

Verification: Clean release build/package passed; extracted package install showed the Docker in CorelDRAW 27.1 and package uninstall removed it.

## ADR-006: Generic Badge Generation Through A Pure Plan And Corel Adapter

Status: Accepted for the Task 4 implementation; complete UI workflow remains only partially host-verified.

Context: Badge generation must support arbitrary placeholder names, grouped artwork, quantities, millimeter layout, and multiple pages without coupling pure calculations to CorelDRAW.

Decision: Keep placeholder parsing, row validation, layout fitting, and multi-page planning in `CorelMate.Badges`. Keep selection capture, recursive Corel shape traversal, native `ShapeRange.Duplicate`, text replacement, page creation, unit conversion, and command grouping in `CorelMate.Host`. Keep manual row entry and layout controls in the WPF Docker.

Alternatives: Hard-code school badge fields, use clipboard copy/paste, put COM calls in WPF handlers, or rebuild text objects.

Reason: The pure engine is testable and generic; the adapter uses verified v27 object-model APIs and preserves the master by duplicating the selected range.

Consequences: Formatting preservation depends on Corel's native `Text.Replace`; Excel/CSV import and richer table editing remain future work.

Verification: Pure tests pass. A real v27.1 temporary-document test captured a grouped master with `{{TITLE}}` and `{{STANDARD}}`, generated three copies, and retained the master.