# Tiny Version Updater

A small WPF client for auto-updating applications. The system includes the following functions:
1. Checking for new versions based on the current version of the executable file.
2. Download files from FTP and replace files. 

## Usage

You can download last available version. (Windows only)

![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/h3xb0y/TinyVersionUpdater)

Update `updater.config` before use.

```
{
	"ExecutablePath":"YOUR_EXECUTABLE_PATH",
	"RootDirectory":"ROOT_DIR",
	"FtpConfig":
	{
		"Path":"FTP_PATH",
		"Login":"FTP_LOGIN",
		"Password":"FTP_PASS"
	}
}
```

## License

[WTFPL](LICENSE)
