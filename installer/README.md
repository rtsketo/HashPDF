# Installer Notes

The Windows installer is defined in `installer/HashPDF.iss` for `Inno Setup`.

Expected packaging inputs:

- `src/HashPDF.WinForms/bin/Release/HashPDF.WinForms.exe`
- `src/HashPDF.WinForms/Assets/HashPDF.ico`
- `installer/redist/dotNetFx40_Full_x86_x64.exe`

Installer behavior:

- installs `HashPDF`
- checks whether `.NET Framework 4.0 Full` is already installed
- runs the bundled `.NET Framework 4.0` installer silently when needed
- starts the app after setup finishes

This keeps the end user from having to search for prerequisites manually.
