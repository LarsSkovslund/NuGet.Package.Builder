 ##Release 1.0.6
 --------------------------------------------------
 + To support generation of packages through nuget.exe pack myproject.csproj a "special" version of 
   NuGet.exe 2.8.5 is included that fixes the issue. see http://nuget.codeplex.com/workitem/4013
   A pull request fixing the issue can be found here https://nuget.codeplex.com/SourceControl/network/forks/StephenCleary/Issue4013/contribution/6798
   It has been merged and will be available in 3.0 so until then the special version will be used.

 ##Release 1.0.5
 --------------------------------------------------
 + Now using NuGet.exe 2.8.5
 + {project}.nuspec files are no longer removed when uninstallling. This fixes the issue with running a Update-Package from Package Manager Console where all .nuspec files will be removed and added again.


 ##Release 1.0.3
 --------------------------------------------------
 + Fixed bug when parsing PublishApiKey or PublishSource from MSBuild to NuGet.Package.Builder.
 
 
