var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));

Task("Build")
.Does(() =>
{
    foreach(var project in GetFiles("./WorkTime/**/*.csproj"))
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
    foreach(var project in GetFiles("./WorkTimeTests/**/*.csproj"))
    {
        DotNetCoreTest(
            project.GetDirectory().FullPath,
            new DotNetCoreTestSettings()
            {
                Configuration = configuration
            });
    }
});

Task("Create-Nuget-Package")
.IsDependentOn("Test")
.Does(() =>
{
    foreach (var project in GetFiles("./WorkTime/**/*.csproj"))
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

Task("Push-Nuget-Package")
.IsDependentOn("Create-Nuget-Package")
.Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API");
	Information($"Utilizando a chave de API: {apiKey}");

	var settings = new DotNetCoreNuGetPushSettings
    {
		Source = "https://api.nuget.org/v3/index.json",
		ApiKey = apiKey
    };
	DotNetCoreNuGetPush($"{artifactsDirectory}/WorkTime*.nupkg", settings);
});

Task("Only-Push-Nuget-Package")
.Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API");
	Information($"Utilizando a chave de API: {apiKey}");
    
	var settings = new DotNetCoreNuGetPushSettings
    {
		Source = "https://api.nuget.org/v3/index.json",
		ApiKey = apiKey
    };
	
    foreach (var pack in GetFiles($"{artifactsDirectory}/*.nupkg"))
    {
		DotNetCoreNuGetPush($"{pack.FullPath}/WorkTime.1.1.0.nupkg", settings);
	}
});

Task("Default").IsDependentOn("Create-Nuget-Package");

RunTarget(target);