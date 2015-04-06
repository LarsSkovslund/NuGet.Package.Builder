@echo Off
SETLOCAL
set config=%1

if "%config%" == "" (
   set config=Release
)

REM Dev10 and Dev11 msbuild path
set nugetmsbuildpath="%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild"

REM Dev12 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\12.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\12.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

REM Dev14 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

set EnableNuGetPackageRestore=true
tools\nuget.exe restore
%nugetmsbuildpath% NuGet.Package.Builder.sln /p:Configuration="%config%" /p:Platform="Any CPU" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Detailed /nr:false 
ENDLOCAL

