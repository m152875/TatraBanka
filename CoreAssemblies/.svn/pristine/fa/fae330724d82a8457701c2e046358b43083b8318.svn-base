@echo off 
rem *** Odkopirovanie zadaneho vystupu kompilacie (dll, pdb a xml) do Assemblies, Symbols a zaregistrovanie do GAC-u ***
rem *** Argumenty: nazov kompilacie, adresar kompilacie, nazov vystupneho suboru kompilacie

set ASSEMBLIES_DIR=.
set SYMBOLS_DIR=\\coradata\rzv\Vymena\CoraTech_Symbols

if %1 == "Release+Install" goto NONGAC
if %1 == "Debug+Install" goto NONGAC
if %1 == "Release+InstallGAC" goto GAC
if %1 == "Debug+InstallGAC" goto GAC

echo No job for _copy_gacInstall (%1), thank you.
goto END

:NONGAC
xcopy %2%3.??? %ASSEMBLIES_DIR%\ /F /R /Y /D

if %1 == "Release+Install" goto SYMB else goto END

:GAC
xcopy %2%3.??? %ASSEMBLIES_DIR%\%3\ /F /R /Y /D | find "0 File"
if errorlevel 1 (
	set errorlevel=0
@rem kniznice do GAC uz neregistrujeme (ak na tom trvas, odpoznamkuj si nasl. riadok)
@rem	%ASSEMBLIES_DIR%\_installGAC\gacutil.exe /i %ASSEMBLIES_DIR%\%3\%3.dll /f /nologo
	if %1 == "Release+InstallGAC" goto SYMB else goto END
)

echo Vysledne subory sa nezmenili
goto END

:SYMB
if not exist %SYMBOLS_DIR%\bin\add_to_store.bat goto END
%SYMBOLS_DIR%\bin\add_to_store.bat %2 %3

:END
