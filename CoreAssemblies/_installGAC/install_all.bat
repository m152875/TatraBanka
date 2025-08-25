@echo off

SET THIS_DIR=%~dp0

echo === INSTALACIA SPOLOCNYCH KNIZNIC DO GAC, NATeC Core === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /il "%THIS_DIR%list_core.txt" /f
echo.
echo === INSTALACIA SPOLOCNYCH KNIZNIC DO GAC, NATeC Libs === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /il "%THIS_DIR%list_libs.txt" /f
echo.
echo === INSTALACIA SPOLOCNYCH KNIZNIC DO GAC, NATeC Base === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /il "%THIS_DIR%list_base.txt" /f
echo.
echo === INSTALACIA SPOLOCNYCH KNIZNIC DO GAC, NATeC Web === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /il "%THIS_DIR%list_web.txt" /f
echo.
echo === INSTALACIA SPOLOCNYCH KNIZNIC DO GAC, NATeC Desktop === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /il "%THIS_DIR%list_desktop.txt" /f
