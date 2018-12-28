#tool "nuget:?package=GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "../src/Chessman.sln";
var supportedPlatforms = new PlatformTarget[]
    {
        //PlatformTarget.ARM,
        //PlatformTarget.x64,
        PlatformTarget.x86,
    };

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
    foreach(var platform in supportedPlatforms)
    {
        MSBuild(solution, settings =>
            settings.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Quiet)
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetPlatformTarget(platform)
                .WithTarget("Clean"));
    }
});

Task("Versioning")
    .IsDependentOn("Clean")
    .Does(() =>
{
    GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
    });
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Versioning")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    foreach(var platform in supportedPlatforms)
    {
        MSBuild(solution, settings =>
            settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Quiet)
            .SetMSBuildPlatform(MSBuildPlatform.x86)
            .SetPlatformTarget(platform)
            .WithTarget("Build")
			.WithProperty("AppxBundlePlatforms","x86")
			.WithProperty("UseDotNetNativeToolchain","false")
			.WithProperty("BuildAppxUploadPackageForUap","true"));
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
