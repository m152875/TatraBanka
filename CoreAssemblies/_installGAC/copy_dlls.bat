@echo off

SET THIS_DIR=%~dp0

if []==[%1] goto NODESTINATION
if not exist %1 goto NOTEXISTS

set LIST=%THIS_DIR%list_%2.txt
if not exist "%LIST%" goto LISTNOTEXISTS

echo === KOPIROVANIE SPOLOCNYCH KNIZNIC DO ADRESARA %1, NATeC %2 === CORA GEO, s.r.o. ===
for /f "usebackq delims=" %%i in ("%LIST%") do xcopy /f /r /y "%THIS_DIR%%%i" %1
echo.

goto END

:NODESTINATION
echo Chyba: cielovy adresar (prvy argument) nezadany.
goto END

:NOTEXISTS
echo Chyba: cielovy adresar %1 neexistuje.
goto END

:LISTNOTEXISTS
echo Chyba: subor so zoznamom kniznic (druhy argument) "%LIST%" neexistuje.

:END