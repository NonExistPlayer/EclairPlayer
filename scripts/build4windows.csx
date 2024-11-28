// build4windows.csx created by NonExistPlayer for EclairPlayer (https://github.com/NonExistPlayer/EclairPlayer)

#load "buildcore.csx" // buildcore.csx must be stored in the same folder with this script.

WriteLine("build4windows.csx started.");

bool success = BuildForPlatform("windows", "Release", 
    Architecture,
    false);

if (!success) {
    ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine("Something went wrong...");
    ResetColor();
    return;
}

WriteLine("end");