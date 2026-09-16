## Summary

<!-- What changed and why? -->

## Scope

- [ ] This change is focused and does not include unrelated cleanup.
- [ ] No AI, licensing, telemetry, installer, or automatic-startup work was added without explicit scope.
- [ ] CorelDRAW interop remains isolated to `CorelMate.Host`.

## Validation

- [ ] `dotnet build .\CorelMate.sln -c Release`
- [ ] `dotnet run --project .\tests\CorelMate.Tests\CorelMate.Tests.csproj -c Release`
- [ ] Real CorelDRAW validation performed, or limitations are listed below.

## CorelDRAW Evidence

<!-- Include version, document setup, operation, observed result, and unverified cases. -->

## Documentation

- [ ] Relevant `.agents/` files updated.
- [ ] Relevant `.agent-memory/` files updated.
- [ ] `CHANGELOG.md` updated when the change is meaningful.

## Limitations / Risks

<!-- Be explicit. Do not claim host behavior that was not tested. -->