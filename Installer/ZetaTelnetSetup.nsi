;Include Modern UI

  !include "MUI2.nsh"
  !include "FileAssociation.nsh"

Name "Zeta Telnet 3.01"
OutFile "ZetaTelnet3.01Setup.exe"
InstallDir "$PROGRAMFILES\Zeta Centauri\Zeta Telnet"
InstallDirRegKey HKLM "Software\Zeta Telnet" "Install_Dir"
RequestExecutionLevel admin
!define MUI_ICON "svr.ico"
!define MUI_UNICON "svr.ico"

;Version Information

  VIProductVersion "3.0.1.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Zeta Telnet"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Zeta Centauri"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Copyright 2007-2012 Zeta Centauri"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Zeta Telnet Installer"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "3.0.1.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" "3.0.1.0"

;Interface Settings

  !define MUI_ABORTWARNING

;Pages

  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_INSTFILES
      !define MUI_FINISHPAGE_NOAUTOCLOSE
      !define MUI_FINISHPAGE_RUN
      !define MUI_FINISHPAGE_RUN_CHECKED
      !define MUI_FINISHPAGE_RUN_TEXT "Launch Zeta Telnet"
      !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchProgram"
      !define MUI_FINISHPAGE_SHOWREADME ""
      !define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
      !define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
      !define MUI_FINISHPAGE_SHOWREADME_FUNCTION finishpageaction
  !insertmacro MUI_PAGE_FINISH
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES

;Languages
 
  !insertmacro MUI_LANGUAGE "English"


; The stuff to install
Section "ZetaTelnet"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "ZetaTelnet.exe"
  File "License.txt"
  File "svr.ico"

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\ZetaTelnet "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "DisplayName" "Zeta Telnet"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "DisplayVersion" "3.01"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "Publisher" "Zeta Centauri"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "DisplayIcon" "$INSTDIR\svr.ico"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet" "NoRepair" 1
  WriteUninstaller "uninstall.exe"

SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Zeta Centauri\Zeta Telnet"
  CreateShortCut "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Zeta Telnet.lnk" "$INSTDIR\ZetaTelnet.exe" "" "" 0
  ;CreateShortCut "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  WriteINIStr "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Zeta Telnet Website.url" "InternetShortcut" "URL" "http://zetacentauri.com/software_zetatelnet.htm"
  
SectionEnd

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ZetaTelnet"
  DeleteRegKey HKLM SOFTWARE\ZetaTelnet

  ; Remove files and uninstaller
  Delete $INSTDIR\ZetaTelnet.exe
  Delete $INSTDIR\License.txt
  Delete $INSTDIR\svr.ico
  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\*.*"
  Delete "$DESKTOP\Zeta Telnet.lnk"
  Delete "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Zeta Telnet Website.url"
  ;DeleteINISec "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Zeta Telnet Website.url" "InternetShortcut"

  ; Remove directories used
  RMDir "$SMPROGRAMS\Zeta Centauri\Zeta Telnet"
  RMDir "$SMPROGRAMS\Zeta Centauri"
  RMDir "$INSTDIR"


SectionEnd

Function LaunchProgram
  ExecShell "" "$SMPROGRAMS\Zeta Centauri\Zeta Telnet\Zeta Telnet.lnk"
FunctionEnd

Function finishpageaction
  ;CreateShortCut "$DESKTOP\Zeta Telnet.lnk" "$INSTDIR\ZetaTelnet.exe" "" "" 0
  CreateShortcut "$DESKTOP\Zeta Telnet.lnk" "$INSTDIR\ZetaTelnet.exe"
FunctionEnd