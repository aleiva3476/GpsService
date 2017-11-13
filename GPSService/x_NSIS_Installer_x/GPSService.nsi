!include "MUI2.nsh"

!define PATH_EXE "..\bin\Release"
;--------------------------------
;Nombre del instalador
Name "Servicio lectura GPS"
OutFile "Instalador GPSService.exe"

ShowInstDetails show
ShowUninstDetails show

SetCompressor /SOLID lzma

InstallDir "$PROGRAMFILES64\GPSService"

RequestExecutionLevel admin


;--------------------------------
;Pages

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

;--------------------------------
;Languages
!insertmacro MUI_LANGUAGE "Spanish"

;--------------------------------
;Secciones

Section
  SetShellVarContext all
  SetOutPath $INSTDIR
  WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

Section "GPSService" SeccionGOLD
  SetShellVarContext all
  SetOutPath $INSTDIR

  ExecWait 'net stop "GPSService"'
  ExecWait '$WINDIR\Microsoft.Net\Framework\v4.0.30319\installutil /u /LogFile="" /LogToConsole=true "$INSTDIR\GPSService.exe"'

  File "${PATH_EXE}\GPSService.exe"
  File "${PATH_EXE}\Npgsql.dll"
  File "${PATH_EXE}\System.Threading.Tasks.Extensions.dll"
  
  ExecWait '$WINDIR\Microsoft.Net\Framework\v4.0.30319\installutil /LogFile="" /LogToConsole=true "$INSTDIR\GPSService.exe"'
  ExecWait 'net start "GPSService"'
SectionEnd

Section "uninstall"
  SetShellVarContext all
  
  ExecWait 'net stop "GPSService"'
  ExecWait '$WINDIR\Microsoft.Net\Framework\v4.0.30319\installutil /u /LogFile="" /LogToConsole=true "$INSTDIR\GPSService.exe"'

  # Borramos los ficheros
  RMDir /r $INSTDIR
SectionEnd
