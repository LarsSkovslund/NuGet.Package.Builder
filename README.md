# NuGet.Package.Builder

NuGet.Package.Builder imports a MSBuild target file into the project along with a [.nuspec](http://docs.nuget.org/create/nuspec-reference) file and a builder configuration file (`package.builder.json`).
The rules and conventions for configuring a [.nuspec](http://docs.nuget.org/create/nuspec-reference) file can be found in the [official NuGet documentation](http://docs.nuget.org/create/creating-and-publishing-a-package)

Change history can be found [here](Changelog.md).

## Getting started
- In Visual Studio, create a new Class library.
- Open Package Manager Console view and type ```Install-Package -Id NuGet.Package.Builder```.
- You now have two new files added `{your project name}.nuspec` and `package.builder.json`.
- Compile and you now have your first NuGet package.

## Package output
Generated packages are location in the same folder as output binaries produced by the build, respecting build configuration, platform and output redirect as configured by VSO build template.
To limit package generation to a specific configuration set the ```Configuration``` property in the ```package.builder.json``` file to the desired configuration name, e.g. setting it to ```Release``` will only generate a package when compiling a release configuration.

## Publish Packages
Automatic publishing packages can be configured in `package.builder.json` by setting Publish.PublishOnBuild to `true`. 
This will publish the package every time the project is compiled enabling a convenient way of publishing a package directly from Visual Studio.
However publishing every time you compile is often not an ideal workflow so NuGet.Package.Builder offers a number of override possibilities.

You can override publishing values by parsing MSBuild properties to a solution or project.
- To publish nuget package on build set `PublishNuGetPackage=true`
- To provide a new API key set `PublishApiKey={key}`
- To publish to another source set `PublishSource=https://myget/F/MyFeed/`
- To publish symbols to another source set `PublishSymbolSource=https://nuget.smbsrc.net/`
- To create a cmd file for publishing the package set `GeneratePublishCommand=true`

```
MSBuild.exe myproject.csproj /p:PublishNuGetPackage=true;PublishApiKey={key};PublishSource=https://myget/F/MyFeed/
```

```
MSBuild.exe mysolution.sln /p:PublishNuGetPackage=true;PublishApiKey={key};PublishSource=https://myget/F/MyFeed/
```

### Generate Publish Cmd
To generate a publishing cmd file set the MSBuild parameter `/p:GeneratePublishCommand=true`. This is useful when 
publishing is required to occur after all tests are passed. The cmd file is located in the same folder as the solution file with the following naming convention `publish_{project}.cmd`.

If no source is specified, that is `package.builder.json -> Publish.Source` or `PublishSource` is not specified, then this value is required to be passed to the `cmd` file as the second parameter.

If no ApiKey is specified, that is `package.builder.json -> Publish.ApiKey` or `PublishApiKey` is not specified, then this value is required to be passed to the `cmd` file as the first parameter.

## Build Server
NuGet.Package.Builder is designed to run on a build server working well with Package Restore without any dependency to installed software.
It uses `nuget.exe` under the cover to package and publish its packages.

Override publish options from VSO or TFS build template
![VSO Build Process Template](docs/BuildProcessTemplate.PNG)

## package.builder.json
```
{
    // Enable this if you need to build package from the nuspec file instead of the project file.
    "UseNuspecFileOnly": false,

    // Determines if a package containing sources and symbols should be created.
    // When specified with a nuspec, creates a regular NuGet package file and the corresponding symbols package
    "Symbols": false,

    // Include referenced projects either as dependencies or as part of the package.
    // If a referenced project has a corresponding nuspec file that has the same name as the project,
    // then that referenced project is added as a dependency.
    // Otherwise, the referenced project is added as part of the package.
    "IncludeReferencedProjects": true,

    // Prevent default exclusion of NuGet package files and files and folders starting with a dot e.g. .svn
    "NoDefaultExcludes": true,

    // Display this amount of details in the output: normal, quiet, detailed.
    "Verbosity": "Detailed",

    // Provides the ability to specify a semicolon ";" delimited list of properties when creating a package.
    // Included by default are Configuration and Platform
    "AdditionalProperties": "",

    // Limit the package generation to a specific build configuration.
    // Leaving this value empty will always trigger package generation
    // Note: config parameter was introduced in release 1.0.7
    "Configuration": "",

	// Specifies one or more wildcard patterns to exclude when creating a package.
	"Exclude": "",

    "Publish": {
        // Publish nuget package on build.
        // Note: This will publish the package every time you compile the project.
        // You can override this behavior by parsing in a MSBuild property named PublishNuGetPackage setting
        // the value to true to control when a package is published.
        // e.g. To publish package when building nightly build on VSO, add the following to the build definitions process template
        // 2. Build -> 5. Advanced -> MSBuild Arguments -> "/p:PublishNuGetPackage=true"
        "PublishOnBuild": false,

        // The API key for the server.
        // You can override this behavior by parsing in a MSBuild property named PublishApiKey.
        "ApiKey": "",

        // Specifies the server URL. If not specified, nuget.org is used unless DefaultPushSource config value
        // is set in the NuGet config file. If NuGet.exe identifies a UNC/folder source, it will perform the file copy to the source
        // You can override this behavior by parsing in a MSBuild property named PublishSource.
        "Source": "",

        // Specifies the symbol server URL.
        // If the presence of a .symbols.nupkg package is detected e.g.Symbols = "true" it will be automatically pushed to SymbolSource.org unless
        // an alternative symbol source is specified.
        "SymbolSource": "",

        // Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes).
        "Timeout": 300
    }
}
```
