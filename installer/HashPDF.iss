#define MyAppName "HashPDF"
#define MyAppVersion "0.1.5"
#define MyAppPublisher "HashPDF"
#define MyAppURL "https://github.com/rtsketo/HashPDF"
#define MyAppExeName "HashPDF.exe"

[Setup]
AppId={{7FA91994-864D-4B41-9A64-EDECFD8C6E85}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
Compression=lzma
SolidCompression=yes
WizardStyle=modern
OutputDir=output
OutputBaseFilename=HashPDF-Setup
SetupIconFile=..\src\HashPDF.WinForms\Assets\HashPDF.ico
PrivilegesRequired=admin
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "..\src\HashPDF.WinForms\bin\Release\HashPDF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\HashPDF.WinForms\bin\Release\HashPDF.pdb"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\src\HashPDF.WinForms\Assets\HashPDF.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "redist\dotNetFx40_Full_x86_x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall skipifsourcedoesntexist

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\dotNetFx40_Full_x86_x64.exe"; Parameters: "/passive /norestart"; StatusMsg: "Installing Microsoft .NET Framework 4.0..."; Check: NeedDotNet40
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function NeedDotNet40(): Boolean;
var
  InstallValue: Cardinal;
begin
  Result := True;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Install', InstallValue) then
  begin
    Result := InstallValue <> 1;
  end;
end;
