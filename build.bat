@echo off
rem This bat file is needed to compile for supported platforms for release in GitHub

dotnet clean

cls

call :build Eclair.Desktop win x86
call :build Eclair.Desktop win x64
call :build Eclair.Desktop linux x64
call :build Eclair.Desktop linux arm64

call :build Eclair.Android android x64

exit /b 0
:build
rem %1 = project
rem %2 = platform
rem %3 = arch
if exist "publish\%2-%3.zip" (
    echo "%2-%3 already exists."
    pause >nul
    exit /b 1
    rem skip
)
echo Starting building %1 for %2-%3...
echo MSBuild:
if "%1"=="Eclair.Desktop" (
    echo dotnet build %1 -c Release --os %2 -a %3 -v m -o publish\%2-%3\ --sc
    echo.
    dotnet build %1 -c Release --os %2 -a %3 -v m -o publish\%2-%3\ --sc
) else (
    echo dotnet build %1 -c Release -o publish/%2-%3/
    echo.
    dotnet build %1 -c Release -o publish/%2-%3/
)
if errorlevel 1 (
    echo.
    echo dotnet exited with code: 1
    pause >nul
    exit /b 1
)
echo MSBuild completed.
echo.
if "%1"=="Eclair.Desktop" (
    echo Packing into an zip archive by 7z:
    echo 7z a -tzip publish\%2-%3.zip publish\%2-%3
    echo.
    7z a -tzip publish\%2-%3.zip publish\%2-%3 >nul
    if errorlevel 1 (
        echo.
        echo 7z exited with code: 1
        pause >nul
        exit /b 1
    )
    echo Packing completed.
) else (
    echo Renaming...
    move publish\android-%3\net.nonexistplayer.eclair-Signed.apk publish\android-%3.apk
)
echo.
echo Cleaning...
rmdir /s /q publish\%2-%3
echo.
echo %1 %2-%3 done!
cls