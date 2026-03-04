# Releasing HashPDF

## Version source of truth

The release version is stored in `VERSION`.

Current release flow:

1. Update `VERSION`
2. Run `pwsh -File build/Sync-Version.ps1`
3. Commit the version change
4. Push to `main`
5. Create and push a tag in the form `vX.Y.Z`
6. GitHub Actions builds:
   - `HashPDF.exe`
   - `HashPDF-<version>-portable.zip`
   - `HashPDF-Setup-<version>.exe`
7. The tag workflow creates or updates the GitHub Release and uploads both assets

Notes:

- The installer bundles the official `.NET Framework 4.0` offline installer during CI packaging
- The repository should keep `AssemblyInfo`, installer metadata, and `VERSION` aligned via `build/Sync-Version.ps1`
