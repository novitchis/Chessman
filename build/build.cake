//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "../src/Chessman.sln";
var platform = PlatformTarget.x86;

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("../src/Chessman.AppView/AppPackages");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);

     // Clean the solution, uwp supports multiple platforms
    MSBuild(solution, settings =>
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Quiet)
            .SetMSBuildPlatform(MSBuildPlatform.x86)
            .SetPlatformTarget(platform)
            .WithTarget("Clean"));
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
    // Use MSBuild
    MSBuild(solution, settings =>
        settings.SetConfiguration(configuration)
		.WithTarget("Build")
		.SetMSBuildPlatform(MSBuildPlatform.x86)
		.SetPlatformTarget(platform)
		.WithProperty("DefineConstants", "PORTABLE NETFX_CORE") // TODO: move to project
		.WithProperty("AppxBundle", "Always")
        .WithProperty("AppxBundlePlatforms","x86")
		);
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
