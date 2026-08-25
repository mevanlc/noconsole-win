# noconsole-win

Launch console programs without displaying console windows (on Windows).
This is done by calling win32's CreateProcess function with the `CREATE_NO_WINDOW` flag.

## noconsolec
Usage: `noconsolec.exe <commandline>`

Executes: 
```
%windir%\system32\cmd.exe /c <commandline>
```

## noconsolew
Usage: `noconsolew.exe <commandline>`

Executes: 
```
<commandline>
```

## Common behavior
* Uses win32's MessageBox() to display detected errors.
* Retrieves `<commandline>` via `GetCommandLineW()` to minimize commandline quoting/escaping quirks.
* If the `NOCONSOLE_DEBUG` environment variable is set to `1`, pauses before launch to display the final command line and copy it to the clipboard.
