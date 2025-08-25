@echo off

rem short name adresara (bez medzier apod.) kvoli pouzitiu vo for
SET THIS_DIR=%~dps0

echo === INSTALACIA KNIZNIC DO GAC === CORA GEO, s.r.o. ===
for %%i in (%THIS_DIR%*.dll) do "%THIS_DIR%gacutil.exe" /nologo /i "%%i" /f
echo.