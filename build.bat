@echo off
echo Building Wabbajack-Unlocked...

REM Set version number
SET VERSION=4.0.4.1
echo Building version: %VERSION%

REM Create temporary directory for build output
SET OUTPUT_DIR=c:\tmp\publish-wj
echo Output directory: %OUTPUT_DIR%

REM Clean up previous build if it exists
if exist "%OUTPUT_DIR%" rmdir /q/s "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%\app"
mkdir "%OUTPUT_DIR%\launcher"

REM Clean and build the solution
echo Cleaning solution...
dotnet clean

echo Building main application...
dotnet publish Wabbajack.App.Wpf\Wabbajack.App.Wpf.csproj --framework "net9.0-windows" --runtime win-x64 --configuration Release /p:Platform=x64 -o "%OUTPUT_DIR%\app" /p:IncludeNativeLibrariesForSelfExtract=true --self-contained /p:DebugType=none /p:DebugSymbols=false /p:VERSION=%VERSION%

echo Building launcher...
dotnet publish Wabbajack.Launcher\Wabbajack.Launcher.csproj --framework "net9.0-windows" --runtime win-x64 --configuration Release /p:Platform=x64 -o "%OUTPUT_DIR%\launcher" /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained /p:DebugType=none /p:DebugSymbols=false /p:VERSION=%VERSION%

echo Building CLI...
dotnet publish Wabbajack.CLI\Wabbajack.CLI.csproj --framework "net9.0-windows" --runtime win-x64 --configuration Release /p:Platform=x64 -o "%OUTPUT_DIR%\app\cli" /p:IncludeNativeLibrariesForSelfExtract=true --self-contained /p:DebugType=none /p:DebugSymbols=false /p:VERSION=%VERSION%

echo Copying launcher to main directory...
copy "%OUTPUT_DIR%\launcher\Wabbajack.exe" "%OUTPUT_DIR%\Wabbajack.exe"

echo Build complete!
echo Exe file located in: %OUTPUT_DIR%\app\Wabbajack.exe