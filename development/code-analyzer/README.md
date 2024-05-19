To use the analyzer in your project, add a NuGet reference to the analyzer package.

The nuget package is not published in any public nuget feed. You can build the package from the source code and reference the built package in your project.

# Build and publish nuget package on local machine

1. Open a command prompt (or powershell) and navigate to the directory where the source code (containing .nuspec file) is located.

2. Run the following command to build the nuget package:

```cmd / pwsh
dotnet pack --configuration Release
```

3. The nuget package (.nupkg) will be created in the `bin\Release` directory.

4. Run the following command to publish the package to a local nuget feed:

```cmd / pwsh
dotnet nuget push <path-to-nupkg> --source <path-to-local-nuget-feed>
```

# Add the analyzer to your project

1. Navigate to the project file (.csproj) where you want to add the analyzer.

2. Run the following command to add the analyzer package reference to the project file:

``` cmd / pwsh
dotnet add package YamlPrompt.Development.CodeAnalyzer
```

3. If analyzer not working shortly after, then build the project to apply the analyzer to the project.

4. Since code analyzers are only used during development, it makes no sense to include them in the release build. Also, the analyzer package is not published in any public nuget feed, therefore, it
may break the build if a project references the analyzer package but it cannot be restored.
To avoid breaking the build you need to set the condition for the analyzer reference in the project file. You can set the condition as follows:

```xml
<ItemGroup Condition="'$(Configuration)' != 'Release'">
  <PackageReference Include="YamlPrompt.Development.CodeAnalyzer" Version="1.0.0" />
</ItemGroup>


# Troubleshooting

1. If `dotnet add package` command didn't work, then it could be that the Nuget package source is not added to the project. You can add the source by running the following command:

```cmd / pwsh
dotnet nuget add source <path-to-local-nuget-feed>
```

- Then run the `dotnet add package` command again. 