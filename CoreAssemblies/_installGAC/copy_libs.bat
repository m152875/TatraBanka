@echo off

SET THIS_DIR=%~dp0

call "%THIS_DIR%copy_dlls.bat" %1 Core
call "%THIS_DIR%copy_dlls.bat" %1 Libs

if [pause]==[%2] pause