// build4all.csx created by NonExistPlayer for EclairPlayer (https://github.com/NonExistPlayer/EclairPlayer)

#load "buildcore.csx" // buildcore.csx must be stored in the same folder with this script.

WriteLine("Build started.");

if (Run("dotnet --version", false) == int.MinValue) {
    ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(".NET not found.");
    ResetColor();
    return;
}

Run("dotnet restore ../Eclair");
Run("dotnet restore ../Eclair.Android");
Run("dotnet restore ../Eclair.Desktop");

BuildForPlatform("win", "Release", "x64");
BuildForPlatform("win", "Release", "x86");
BuildForPlatform("linux", "Release", "x64");
BuildForPlatform("linux", "Release", "x86");
BuildForPlatform("android", "Release", "x64");
BuildForPlatform("android", "Release", "x86");
BuildForPlatform("android", "Release", "arm64");

WriteLine("Build ended.");