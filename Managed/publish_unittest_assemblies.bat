echo off
echo publishing unit test cases assemblies ...
set DOTNET_PATH=%~dp0..\dotnet\sdk\dotnet.exe
set PROJECT_PATH=%~dp0SharpScriptUnitTest
set ASSEMBLIES_PATH=%~dp0Assemblies

"%DOTNET_PATH%" build "%PROJECT_PATH%" --output "%ASSEMBLIES_PATH%" -c Release