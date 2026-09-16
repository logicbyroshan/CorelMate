# Known Limitations

- The Docker surface is verified through `FrameWork.AddDocker`; automatic startup from a Corel `.addon` package is not verified.
- The developer bootstrap uses COM automation and must be run after building; it is not a production installer.
- The release ZIP also requires a manual session install; automatic CorelDRAW startup is not verified.
- The host reference uses a machine-specific installed SDK path.
- Badge generation, curves, AI, secure credentials, installer, and licensing are not implemented.
- CorelDRAW-specific integration was run manually through COM on the installed host; repeatable automated GUI assertions are not yet present.
- The direct CorelDRAW adapter has been exercised with a temporary grouped master, but the WPF button-driven workflow has not yet been tested end to end.
- Multi-page creation and rollback code exists but has not been validated with overflow artwork in the real host.
- Desktop automation could not foreground CorelDRAW reliably for click-by-click WPF testing; do not claim the full UI workflow until manually verified.
- 100 badges passed in about 9 seconds; 500 and 1000 badge performance remains unverified.
- Convert Text to Curves special containers, symbols, text-on-path, and full button-driven confirmation remain unverified.