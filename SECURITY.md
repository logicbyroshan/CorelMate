# Security Policy

## Supported Versions

CorelMate is pre-1.0. Security fixes are handled on the current default branch unless a release-specific exception is documented.

## Reporting a Vulnerability

Please do not open a public issue for a suspected vulnerability. Use GitHub's private security advisory mechanism for the repository when available. Include reproduction steps, affected commit/version, environment details, and impact. Do not include API keys, private artwork, credentials, or other sensitive data.

## Security Expectations

- Never commit secrets, credentials, tokens, private keys, or local machine configuration.
- Treat CorelDRAW documents, selected artwork, and future AI conversations as sensitive.
- Do not add telemetry, background polling, automatic downloads, remote code execution, or unreviewed network calls.
- Future AI credentials must use a Windows secure credential mechanism rather than plaintext files.
- Review new dependencies and their licenses before adding them.
- Do not redistribute Corel-owned interop assemblies in release artifacts.