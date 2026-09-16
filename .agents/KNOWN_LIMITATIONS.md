# Known Limitations

- The Docker surface is verified through `FrameWork.AddDocker`; automatic startup from a Corel `.addon` package is not verified.
- The developer bootstrap uses COM automation and must be run after building; it is not a production installer.
- The host reference uses a machine-specific installed SDK path.
- Badge generation, curves, AI, secure credentials, installer, and licensing are not implemented.
- CorelDRAW-specific integration was run manually through COM on the installed host; repeatable automated GUI assertions are not yet present.