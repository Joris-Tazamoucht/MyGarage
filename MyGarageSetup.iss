#define AppName "MyGarage"
#define AppVersion "1.0.0"
#define AppPublisher "Joris Tazamoucht"
#define AppExeName "MyGarage.exe"
#define AppSourceDir "MyGarage\bin\Release\net10.0-windows\publish\win-x64"
[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=Setup\Output
OutputBaseFilename=MyGarageSetup_v{#AppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#AppExeName}
PrivilegesRequired=admin

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "Créer un raccourci sur le Bureau"; GroupDescription: "Icônes supplémentaires :"; Flags: checkedonce

[Files]
; Exécutable principal
Source: "{#AppSourceDir}\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion

; Fichier de configuration — NE PAS écraser si déjà présent
Source: "{#AppSourceDir}\appsettings.json"; DestDir: "{app}"; Flags: onlyifdoesntexist

[Icons]
; Raccourci menu Démarrer
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\Désinstaller {#AppName}"; Filename: "{uninstallexe}"

; Raccourci Bureau (si coché)
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
; Lancer l'app après installation
Filename: "{app}\{#AppExeName}"; Description: "Lancer {#AppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Nettoyer les fichiers générés à l'utilisation mais pas l'appsettings
Type: filesandordirs; Name: "{app}\*.db"