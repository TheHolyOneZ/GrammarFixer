@echo off
echo ================================
echo Grammar Fixer - Build Script
echo ================================
echo.

set BUILD_NUM=%RANDOM%

echo [1/3] Cleaning previous builds...
if exist "publish" rmdir /s /q "publish" 2>nul
if exist "GrammarFixer\bin" rmdir /s /q "GrammarFixer\bin" 2>nul
if exist "GrammarFixer\obj" rmdir /s /q "GrammarFixer\obj" 2>nul
timeout /t 1 /nobreak >nul

echo [2/3] Restoring packages...
dotnet restore GrammarFixer\GrammarFixer.csproj

echo [3/3] Publishing standalone executable...
dotnet publish GrammarFixer\GrammarFixer.csproj ^
  --configuration Release ^
  --runtime win-x64 ^
  --self-contained true ^
  --output publish ^
  /p:PublishSingleFile=true ^
  /p:IncludeNativeLibrariesForSelfExtract=true ^
  /p:EnableCompressionInSingleFile=true ^
  /p:PublishTrimmed=false ^
  /p:Version=1.0.%BUILD_NUM%

echo.
if exist "publish\GrammarFixer.exe" (
    echo ================================
    echo BUILD SUCCESSFUL!
    echo ================================
    echo.
    ren "publish\GrammarFixer.exe" "GrammarFixer_v%BUILD_NUM%.exe"
    echo Location: publish\GrammarFixer_v%BUILD_NUM%.exe
    cd publish
    dir GrammarFixer_*.exe
    cd ..
) else (
    echo ================================
    echo BUILD FAILED - EXE NOT FOUND!
    echo ================================
)

echo.
pause