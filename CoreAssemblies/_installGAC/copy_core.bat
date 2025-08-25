@echo off

SET THIS_DIR=%~dp0

call "%THIS_DIR%copy_dlls.bat" %1 core

if [pause]==[%2] pause