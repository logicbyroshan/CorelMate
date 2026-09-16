# Decisions

## 2026-09-16: Preserve The Existing Modular Boundary

Decision: Keep pure modules separate from `CorelMate.Host` and `CorelMate.UI`.

Context: CorelDRAW COM types must not leak through badge, AI, curves, or infrastructure logic.

Reason: It keeps pure logic testable and isolates host/runtime compatibility.

Alternatives: One project or feature projects referencing COM directly.

Consequences: Host adapters may require explicit mapping code, but feature modules remain portable.

## 2026-09-16: WPF Docker Surface

Decision: Use a public parameterless WPF `UserControl` targeting .NET Framework 4.8 for the v27 Docker.

Context: CorelDRAW v27.1 accepted and rendered WPF through `FrameWork.AddDocker`; WinForms controls failed at `ShowDocker`.

Reason: Verified real-host compatibility.

Alternatives: WinForms, HTML/JavaScript, native addon packaging.

Consequences: WPF/Corel host compatibility is a required validation boundary.

## 2026-09-16: Mandatory PR-Only Changes

Decision: Never change `main` directly. Every change must use a pushed branch and a GitHub PR created and merged with `gh` or the GitHub UI.

Context: The repository is now published at `https://github.com/logicbyroshan/CorelMate.git` and the owner requested a reviewable history.

Reason: Preserve reviewability, checks, and a clean protected-main workflow.

Alternatives: Direct main commits or local-only changes.

Consequences: Every future task must include branch creation, push, PR, checks/review, merge, and cleanup.