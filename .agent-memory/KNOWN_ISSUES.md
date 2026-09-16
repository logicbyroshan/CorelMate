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

These findings are recorded only; bootstrap does not fix them.