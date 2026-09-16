# Development Rules

1. Read the relevant `.agents` files before modifying code.
2. Never invent CorelDRAW APIs, registration keys, or packaging behavior.
3. Keep CorelDRAW references isolated to `CorelMate.Host`.
4. Keep document operations explicit, undo-aware, and off pure logic.
5. Do not store secrets or log API keys/conversations.
6. Build and run focused tests before claiming completion.
7. Label host behavior as unverified unless tested in CorelDRAW.
8. Update `CURRENT_STATE.md`, `TASKS.md`, `CHANGELOG.md`, and append `SESSION_HISTORY.md` for meaningful changes.
9. Preserve prior ADRs and session history.
10. Avoid new dependencies without a documented reason.