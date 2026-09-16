# Security Memory

## Current Model

There is no user authentication, authorization service, database, backend, or network API in the current code. The Docker runs locally inside CorelDRAW. The AI interface is only a future abstraction and currently sends nothing.

## Rules

- Never commit API keys, tokens, credentials, private keys, or secrets.
- Never place secrets in `.agent-memory`, source, logs, scripts, or project files.
- Future AI credentials must use an appropriate Windows secure credential mechanism rather than plaintext JSON.
- Do not log complete AI conversations, artwork contents, passwords, or keys.
- Do not add telemetry, analytics, remote code execution, automatic downloads, or network calls without an explicit reviewed task.
- Treat CorelDRAW documents and selected artwork as sensitive user data.
- Validate any future file import/export paths and avoid uncontrolled filesystem writes.
- Review third-party dependencies before adding them; the current solution has none.

## Known Security-Sensitive Areas

The PowerShell scripts create a local COM automation object and load a local assembly into CorelDRAW. Any future installer, macro/bootstrap mechanism, AI provider, or document mutation code requires an explicit security review.