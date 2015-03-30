# NuGet.Package.Builder

NuGet.Package.Builder imports a MSBuild target file into the project along with a [.nuspec] (http://docs.nuget.org/create/nuspec-reference) file and a builder configuration file (`package.builder.json`).
The rules and conventions for configuring a [.nuspec] (http://docs.nuget.org/create/nuspec-reference) file can be found in the [official NuGet documentation] (http://docs.nuget.org/create/creating-and-publishing-a-package)

##Getting started
- In Visual Studio, create a new Class library.
- Open Package Manager Console view and type ```Install-Package -Id NuGet.Package.Builder```
- You now have two new files added `{your project name}.nuspec` and `package.builder.json`
- Compile and you now have have your first NuGet package

##Package output
Generated packages are location in the same folder as output binaries produced by the build, respecting build configuration, platform and output redirect as configured by VSO build template. 

##Publish Packages
Automatic publishing packages can be configured in `package.builder.json` by setting Publish.PublishOnBuild to `true`. 
This will publish the package every time the project is compiled enabling a convenient way of publishing a package directly from Visual Studio.
However publishing every time you compile is often not an ideal workflow so NuGet.Package.Builder offers a number of override possibilities.

Parsing MSBuild properties you can override; When to publish `PublishNuGetPackage=true` , Provide a new API key `PublishApiKey={key}` or Change the source `PublishSource=https://myget/F/MyFeed/`

```
MSBuild.exe myproject.csproj /p:PublishNuGetPackage=true;PublishApiKey={key};PublishSource=https://myget/F/MyFeed/
```

```
MSBuild.exe mysolution.sln /p:PublishNuGetPackage=true;PublishApiKey={key};PublishSource=https://myget/F/MyFeed/
```

## Build Server
NuGet.Package.Builder is designed to run on a build server working well with Package Restore without any dependency to installed software.
It uses `nuget.exe` under the cover to package and publish its packages.

Override publish options from VSO or TFS build template


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

    "Publish": {
        // Publish nuget package on build.
        // Note: This will publish the package everytime you compile the project.
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

        // Specifies the timeout for pushing to a server in seconds. Defaults to 300 seconds (5 minutes).
        "Timeout": 300
    }
}
```
