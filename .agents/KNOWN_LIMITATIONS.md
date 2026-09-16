# Known Limitations

- The Docker surface is verified through `FrameWork.AddDocker`; automatic startup from a Corel `.addon` package is not verified.
- The developer bootstrap uses COM automation and must be run after building; it is not a production installer.
- The release ZIP also requires a manual session install; automatic CorelDRAW startup is not verified.
- The host reference uses a machine-specific installed SDK path.
- Badge generation, curves, AI, secure credentials, installer, and licensing are not implemented.
- CorelDRAW-specific integration was run manually through COM on the installed host; repeatable automated GUI assertions are not yet present.
- The direct CorelDRAW adapter has been exercised with a temporary grouped master, but the WPF button-driven workflow has not yet been tested end to end.
- Multi-page creation and rollback code exists but has not been validated with overflow artwork in the real host.