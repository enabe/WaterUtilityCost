#define MyAppName "水道光熱費アプリ"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "rolan"
#define MyAppExeName "WaterUtilityCost.exe"
#define MyAppBuildDir "bin\\Release\\net48"
#define DotNet48InstallerFile "ndp48-x86-x64-allos-jpn.exe"
#define DotNet48InstallerPath "prerequisites\\" + DotNet48InstallerFile
#ifnexist DotNet48InstallerPath
  #error DotNet48InstallerPath + " が見つかりません。prerequisites フォルダに .NET Framework 4.8 オフライン インストーラーを配置してください。"
#endif

[Setup]
; NOTE: AppId should be unique. Replace with your own GUID when needed.
AppId={{D41AB786-A34B-4E59-8DD5-2DA18D8B6210}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName=C:\Program Files\rolan
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
OutputDir=installer\output
OutputBaseFilename=WaterUtilityCost_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Files]
Source: "{#MyAppBuildDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb,*.xml"
Source: "{#DotNet48InstallerPath}"; DestDir: "{tmp}"; Flags: dontcopy;

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet48OrLaterInstalled: Boolean;
var
  Release: Cardinal;
begin
  Result :=
    RegQueryDWordValue(
      HKLM,
      'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
      'Release',
      Release
    ) and (Release >= 528040);
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';

  if IsDotNet48OrLaterInstalled then
    exit;

  WizardForm.StatusLabel.Caption :=
    '.NET Framework 4.8 をインストールしています。しばらくお待ちください。';
  WizardForm.Update;

  ExtractTemporaryFile('{#DotNet48InstallerFile}');

  if not Exec(
    ExpandConstant('{tmp}\{#DotNet48InstallerFile}'),
    '/q /norestart',
    '',
    SW_SHOW,
    ewWaitUntilTerminated,
    ResultCode
  ) then
  begin
    Result :=
      '.NET Framework 4.8 のインストーラーを起動できませんでした。' + #13#10 +
      '管理者権限で再実行してください。';
    exit;
  end;

  if (ResultCode = 0) then
    exit;

  if (ResultCode = 3010) or (ResultCode = 1641) then
  begin
    NeedsRestart := True;
    exit;
  end;

  Result :=
    '.NET Framework 4.8 のインストールに失敗しました。(終了コード: ' +
    IntToStr(ResultCode) + ')' + #13#10 +
    '先に .NET Framework 4.8 を導入後、再度セットアップを実行してください。';
end;
