
@echo off
echo Processing Transformation exchange table to tags configuration files

%1Agileo.MessageDataBus.ExcelConverter.exe -in=%2 -serverDriverType=%3

if %errorlevel% == 0 echo Transformation done
if %errorlevel% == 1 echo Transformation error : wrong number of arguments
if %errorlevel% == 2 echo Transformation error : invalid arguments
if %errorlevel% == 3 echo Transformation : unexpected error
