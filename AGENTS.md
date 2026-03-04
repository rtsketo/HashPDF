# Repository Guidelines

## Project Structure & Module Organization
`HashPDF.sln` contains a single Windows Forms app at `src/HashPDF.WinForms`. UI code lives in `MainForm.cs` and `Controls/`, hashing and PDF generation are in `Services/`, shared data models are in `Models/`, and bilingual strings are in `Localization/`. Versioned installer assets live in `installer/`, release helpers in `build/`, and small utility scripts in `tools/`. The canonical release version is stored in `VERSION`.

## Build, Test, and Development Commands
Use Windows with .NET Framework 4.0 reference assemblies installed.

`msbuild HashPDF.sln /t:Rebuild /p:Configuration=Debug /p:Platform="Any CPU"` builds a local debug executable.

`msbuild HashPDF.sln /t:Rebuild /p:Configuration=Release /p:Platform="Any CPU"` creates the release binary used by packaging.

`pwsh -File build/Sync-Version.ps1` syncs `VERSION`, `AssemblyInfo.cs`, and `installer/HashPDF.iss`.

`pwsh -File build/Sync-Version.ps1 -Check` fails if version metadata is out of sync; this matches CI.

`& "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" "installer\HashPDF.iss"` builds the Windows installer after a release build.

## Coding Style & Naming Conventions
Follow the existing C# style: 4-space indentation, braces on new lines, PascalCase for types and methods, camelCase for private fields, and explicit types where they improve readability. Keep WinForms layout code grouped in the form class, and place reusable logic in `Services/` or `Controls/` instead of expanding `MainForm.cs`. Preserve the current .NET Framework 4.0 compatibility target and avoid adding dependencies unless necessary.

## Testing Guidelines
There is no automated test project yet. Validate changes with a manual smoke test on Windows: open the app, process a sample file, confirm the SHA-512 hash appears, confirm `<name>.hash.pdf` is written beside the source file, and verify both "Open PDF" and "Open Folder" actions. For localization or UI changes, test both Greek and English.

## Commit & Pull Request Guidelines
Keep commit messages short, imperative, and capitalized, matching recent history such as `Add installer and app metadata` or `Make README links clickable`. Pull requests should include a concise summary, manual test notes, linked issues when applicable, and screenshots for any visible UI change. If a PR changes packaging or release behavior, mention updates to `VERSION`, installer metadata, or GitHub Actions explicitly.
