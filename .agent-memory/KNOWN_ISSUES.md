# Known Issues And Risks

- CorelDRAW automatic startup loading through an official `.addon` package is unverified.
- The host project uses a machine-specific absolute SDK `HintPath`.
- The developer bootstrap relies on COM automation and is not a production installer.
- CorelDRAW integration lacks repeatable automated GUI assertions.
- The current test executable covers only one badge-grid calculation.
- `ConvertTextToCurvesService` intentionally throws `NotSupportedException`; it is not an implemented feature.
- The logger is a null implementation; structured logging is not implemented.
- AI provider and settings abstractions do not persist secure credentials.
- The empty initial Git history means this bootstrap must establish the first reviewable PR carefully.
- CorelDRAW reported the Docker GUID visible after a fresh launch following removal, but the content could not be visually confirmed; treat this as ambiguous persisted workspace state, not verified automatic addon startup.
- Release packaging currently needs to be rerun and inspected after adding Host/Badges/Infrastructure dependencies to the Docker.
- The WPF UI catches and displays exception messages but does not yet route technical exceptions through a real structured logger.

These findings are recorded only; bootstrap does not fix them.