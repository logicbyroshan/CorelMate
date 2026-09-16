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

## 2026-09-16 - Task 4 Badge Generator

- Goal: implement the generic badge engine and initial CorelDRAW integration without adding AI, curves, or import features.
- Changed: added placeholder parsing/replacement, generic data rows, millimeter layout and multi-page planning, recursive Corel shape traversal, native duplication/movement, text replacement, page creation, command grouping, and a manual WPF workflow.
- Verified: pure parser/layout/validation smoke tests, Release compilation, and a real CorelDRAW 27.1 temporary-document test with grouped artistic text placeholders and three generated copies.
- Not verified: full interactive WPF generation, formatting preservation across varied text, undo/redo behavior, overflow pages, large quantities, and release-package rerun after the new managed dependency set.

## 2026-09-16 - Task 5 Badge Workflow Hardening

- Goal: make the existing Badge Generator usable and safer through the WPF Docker without starting other product features.
- Changed: added dynamic variable-derived row editors, row validation, preview recalculation, reset, friendly errors, busy-state/double-submit protection, and captured-document validation.
- Verified: Release build and pure tests; real v27.1 generation of 100 badges across 5 pages in about 9 seconds; missing-value preflight rejection; grouped generation undo/redo via shape counts.
- Verified: final clean release ZIP with all managed dependencies; extracted package install and uninstall passed.
- Unverified: click-by-click WPF interaction, visual foreground screenshot, formatting variants, 500/1000 quantities, and full realistic user workflow.

## 2026-09-16 - Task 6 Convert Text to Curves

- Goal: add selection-scoped native text-to-curves conversion without affecting Badge Generator behavior.
- Changed: added shared materialized shape traversal, Curves summary model, Host preflight/converter, WPF confirmation/result controls, and Curves regression coverage.
- Verified: v27.1 artistic/grouped conversion, paragraph conversion, rectangle preservation, locked/hidden classification, and command-group undo/redo.
- Unverified: special containers, symbols, text-on-path, full WPF confirmation clicks, and large text-selection performance.

## 2026-09-16 - Task 7 Excel / CSV Import

- Goal: add local CSV/XLSX import without rewriting Badge Generator or using Excel COM.
- Changed: added dependency-free CSV and Open XML XLSX sources, header normalization/mapping, duplicate/missing/extra-column handling, row/quantity validation, Unicode support, WPF import action, and synthetic regression tests.
- Verified: quoted/escaped/multiline CSV, Unicode, duplicate/missing headers, extra columns, invalid quantities, blank rows, XLSX first worksheet, build, and existing regressions.
- Verified: real v27.1 CSV-to-generator bridge imported three rows and generated 11 badges through the existing engine.
- Unverified: foreground WPF file-picker/import interaction and real XLSX-to-CorelDRAW generation.

## 2026-09-16 - Task 8 Badge Generator QA

- Goal: audit and harden the existing Badge Generator without redesigning its architecture or adding features.
- Finding: failed master capture/import could leave stale row editors visible; Reset did not clear Curves result feedback.
- Changed: clear row/layout/result state on failed capture/import/reset while preserving CorelDRAW artwork.
- Verified: build/tests/release package; real v27.1 manual adapter generation of 11 badges, invalid preflight rejection, master preservation, Undo, and Redo.
- Unverified: foreground WPF click workflow, file picker, real XLSX generation, document switching, and 500/1000 performance.