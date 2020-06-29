var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));

Task("Build")
.Does(() =>
{
    foreach(var project in GetFiles("./src/**/*.csproj"))
    {
        DotNetCoreBuild(
            project.GetDirectory().FullPath, 
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration
            });
    }
});

Task("Test")
.IsDependentOn("Build")
.Does(() =>
{
    foreach(var project in GetFiles("./test/**/*.csproj"))
    {
        DotNetCoreTest(
            project.GetDirectory().FullPath,
            new DotNetCoreTestSettings()
            {
                Configuration = configuration
            });
    }
});

Task ("PublishConsoleCliWindows")
	.Does(() => {
		Information ("Publicando Cli Windows.");
		 var publishDir = $"{(MakeAbsolute (artifactsDirectory).FullPath)}/totp.cli/windows-x64";
		 if (!DirectoryExists (publishDir)) {
			CreateDirectory (publishDir);
		 }
		 
		 var apiProject = "./src/totp.cli/totpcli.csproj";
		 
		 var settings = new DotNetCorePublishSettings
		 {
			 Framework = "net5",
			 Configuration = "Release",
			 Runtime = "win-x64",
			 SelfContained = true,
			 OutputDirectory = publishDir
		 };
		 DotNetCorePublish(apiProject, settings);

        Zip (publishDir, artifactsDirectory + "/" + File ($"cli-windows.zip"));
});

Task ("PublishGtkWindows")
	.Does(() => {
		Information ("Publicando UI-GTK# Windows.");
		 var publishDir = $"{(MakeAbsolute (artifactsDirectory).FullPath)}/totp.gtk/windows-x64";
		 if (!DirectoryExists (publishDir)) {
			CreateDirectory (publishDir);
		 }
		 
		 var apiProject = "./src/totp.gtksharp/totp.csproj";
		 
		 var settings = new DotNetCorePublishSettings
		 {
			 Framework = "net5",
			 Configuration = "Release",
			 Runtime = "win-x64",
			 SelfContained = true,
			 OutputDirectory = publishDir
		 };
		 DotNetCorePublish(apiProject, settings);

        Zip (publishDir, artifactsDirectory + "/" + File ($"gtk-windows.zip"));
});

Task ("PublishUiTerminalWindows")
	.Does(() => {
		Information ("Publicando GUI-Console Windows.");
		 var publishDir = $"{(MakeAbsolute (artifactsDirectory).FullPath)}/totp.uiterm/windows-x64";
		 if (!DirectoryExists (publishDir)) {
			CreateDirectory (publishDir);
		 }
		 
		 var apiProject = "./src/totp.uiterm/totp.csproj";
		 
		 var settings = new DotNetCorePublishSettings
		 {
			 Framework = "net5",
			 Configuration = "Release",
			 Runtime = "win-x64",
			 SelfContained = true,
			 OutputDirectory = publishDir
		 };
		 DotNetCorePublish(apiProject, settings);

        Zip (publishDir, artifactsDirectory + "/" + File ($"gtk-windows.zip"));
});

Task("Publish")
.IsDependentOn("Test")
.IsDependentOn("PublishConsoleCliWindows")
.IsDependentOn("PublishGtkWindows")
.IsDependentOn("PublishUiTerminalWindows")
.Does(() =>
{
});

Task("Create-Lib-Nuget-Package")
.IsDependentOn("Test")
.Does(() =>
{
    foreach (var project in GetFiles("./src/enki.token/*.csproj"))
    {
        DotNetCorePack(
            project.GetDirectory().FullPath,
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory
            });
    }
});

Task("Push-Lib-Nuget-Package")
.IsDependentOn("Create-Lib-Nuget-Package")
.Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API");
    
    foreach (var package in GetFiles($"{artifactsDirectory}/*.nupkg"))
    {
        NuGetPush(package, 
            new NuGetPushSettings {
                Source = "https://www.nuget.org/api/v2/package",
                ApiKey = apiKey
            });
    }
});

Task("Default").IsDependentOn("Create-Lib-Nuget-Package");

RunTarget(target);