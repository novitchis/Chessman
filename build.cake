#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./src/Chessman.sln";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Chessman.AppView/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(solution, settings =>
        settings.SetConfiguration(configuration)
		.SetMSBuildPlatform(MSBuildPlatform.x86)
		.SetPlatformTarget(PlatformTarget.x86)
		.WithProperty("DefineConstants", "PORTABLE NETFX_CORE")
		);
    }
    else
    {
      // Use XBuild
      XBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
});

//Task("Run-Unit-Tests")
//    .IsDependentOn("Build")
//    .Does(() =>
//{
//    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
//        NoResults = true
//        });
//});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
