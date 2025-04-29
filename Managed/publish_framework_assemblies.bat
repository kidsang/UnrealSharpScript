echo off
echo publishing sharp script framework assemblies ...
set DOTNET_PATH=%~dp0..\dotnet\sdk\dotnet.exe
set PROJECT_PATH=%~dp0SharpScript
set ASSEMBLIES_PATH=%~dp0Assemblies

"%DOTNET_PATH%" build "%PROJECT_PATH%" --output "%ASSEMBLIES_PATH%" -c Release