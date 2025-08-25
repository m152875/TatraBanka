@echo off

SET THIS_DIR=%~dp0

echo === VYPIS KNIZNIC ZAREGISTROVANYCH DO GAC, NATeC Core === CORA GEO, s.r.o. ===
echo === Cora.*, PublicKeyToken=80ab63ad4cd8c4bf ===
"%THIS_DIR%gacutil" /nologo /l | find /i "Cora." | find /i "PublicKeyToken=80ab63ad4cd8c4bf"
echo.
echo === *.DynamicPDF.* ===
"%THIS_DIR%gacutil" /nologo /l | find /i "DynamicPDF."
echo.
echo === ICSharpCode.SharpZipLib, Version=0.8* ===
"%THIS_DIR%gacutil" /nologo /l | find /i "ICSharpCode.SharpZipLib, Version=0.8"
echo.
echo === zlib.net, Version=1.0.3.0 ===
"%THIS_DIR%gacutil" /nologo /l | find /i "zlib.net, Version=1.0.3.0"
echo.
echo === Oracle.DataAccess (instalovane samotatne) ===
"%THIS_DIR%gacutil" /nologo /l | find /i "Oracle.DataAccess"
echo.

echo === VYPIS KNIZNIC ZAREGISTROVANYCH DO GAC, NATeC Web === CORA GEO, s.r.o. ===
echo === AjaxControlToolkit, Version=3.0.20229.20843 ===
"%THIS_DIR%gacutil" /nologo /l | find /i "AjaxControlToolkit, Version=3.0.20229.20843"
echo.
echo === Coolite.Ext.Web ===
"%THIS_DIR%gacutil" /nologo /l | find /i "Coolite.Ext.Web, Version=0.7.0.9936"
echo.
echo === ZedGraph.*, Version=4.5.8.* ===
"%THIS_DIR%gacutil" /nologo /l | find /i "ZedGraph" | find /i "Version=4.5.8."
echo.

echo === VYPIS KNIZNIC ZAREGISTROVANYCH DO GAC, NATeC Desktop === CORA GEO, s.r.o. ===
echo === System.Windows.Forms.DataVisualization, Version=3.5.0.0 ===
"%THIS_DIR%gacutil" /nologo /l | find /i "System.Windows.Forms.DataVisualization" | find /i "Version=3.5.0.0"

pause