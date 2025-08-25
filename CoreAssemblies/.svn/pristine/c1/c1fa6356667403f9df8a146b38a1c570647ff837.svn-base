@echo on
@echo.
@echo.
@echo rem  Vynutenie SVN update pred kompilaciou v mode Release+Install
@echo rem  %1 = configuration
@echo rem  %2 = working copy root

if %COMPUTERNAME% == SRV19 goto END

goto %1

:Release+Install
:Release+InstallGAC

@echo on
@echo \\coradata\rzv\vymena\svn\bin\svn.exe update %2

:Debug
:Debug+Install
:Debug+InstallGAC
:END

