@echo off
set upid=%1
set mpid=%2
taskkill /F /PID %mpid%
taskkill /F /PID %upid%
move /y .\Updater\Contents\*.* .\
start .\DGEBotExampleProgram.exe
exit