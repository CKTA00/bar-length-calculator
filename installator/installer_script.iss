; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Kalkulator Sztabek"
#define MyAppVersion "2.1"
#define MyAppPublisher "Przemys�aw Ko�uch"
#define MyAppExeName "dzielenie_sztang.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{79ACC1D5-9364-47AA-AF36-A2F36FAA5FEC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=bin
OutputBaseFilename=Instaluj Kalkulator Sztabek 2.1
SetupIconFile=C:\Users\chick\source\repos\dzielenie_sztang\dzielenie_sztang\bin\Debug\img\new ico.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\chick\source\repos\dzielenie_sztang\dzielenie_sztang\bin\Debug\dzielenie_sztang.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\chick\source\repos\dzielenie_sztang\dzielenie_sztang\bin\Debug\dzielenie_sztang.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\chick\source\repos\dzielenie_sztang\dzielenie_sztang\bin\Debug\dzielenie_sztang.pdb"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
