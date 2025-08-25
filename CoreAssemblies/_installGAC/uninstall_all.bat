@echo off

SET THIS_DIR=%~dp0

echo === ODINSTALACIA SPOLOCNYCH KNIZNIC Z GAC, NATeC === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /ul "%THIS_DIR%fulllist_cora.txt"
echo.
echo === ODINSTALACIA SPOLOCNYCH KNIZNIC Z GAC, tretie strany === CORA GEO, s.r.o. ===
"%THIS_DIR%gacutil" /nologo /ul "%THIS_DIR%fulllist_3rdparty.txt"

pause