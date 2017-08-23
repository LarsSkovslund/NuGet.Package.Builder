using Newtonsoft.Json;
using System;
using System.IO;

namespace NuGet.Package.Builder
{
	public class PackageOptions
	{
		public bool UseNuspecFileOnly { get; set; }
		public bool Symbols { get; set; }
		public bool IncludeReferencedProjects { get; set; }
		public bool NoDefaultExcludes { get; set; }
		public string Verbosity { get; set; }
		public string AdditionalProperties { get; set; }
        public string Configuration { get; set; }
        public string Exclude { get; set; }

        public PushPackageOptions Publish { get; set; }

		public PackageOptions()
		{
			UseNuspecFileOnly = false;
			Symbols = true;
			IncludeReferencedProjects = true;
			NoDefaultExcludes = true;
			Verbosity = "Detailed";
			Publish = new PushPackageOptions();
		}

		public static PackageOptions LoadOrDefault(ArgumentOptions arguments)
		{
			var file = Path.Combine(arguments.WorkingDirectory, "package.builder.json");
			var options = (File.Exists(file))
				? JsonConvert.DeserializeObject<PackageOptions>(File.ReadAllText(file))
				: new PackageOptions();

			if (arguments.ForcePublishing)
				options.Publish.PublishOnBuild = true;

			return options;
		}

		private string GetProjectOrNuspecFile(ArgumentOptions arguments)
		{
			return Path.Combine(arguments.WorkingDirectory, arguments.ProjectName + ((UseNuspecFileOnly) ? ".nuspec" : arguments.ProjectExt));
		}

		private string GetVerbosity()
		{
			return string.IsNullOrWhiteSpace(Verbosity)
				? ""
				: string.Format("-Verbosity {0}", Verbosity);
		}

		private string GetProperties(ArgumentOptions arguments)
		{
			return string.Format("-Properties \"OutDir={0};{1}{2}\"",
				arguments.OutDir,
				arguments.Properties,
				string.IsNullOrWhiteSpace(AdditionalProperties) ? "" : ";" + AdditionalProperties
			);
		}

		private string GetOutputDirectory(ArgumentOptions arguments)
		{
			return string.Format("-OutputDirectory \"{0}\"", arguments.OutDir);
		}

		private string GetBasePath(ArgumentOptions arguments)
		{
			return string.Format("-basepath \"{0}\"", arguments.OutDir);
		}

		public string GetBuildCommandArgs(ArgumentOptions arguments)
		{
			return string.Format(@"pack ""{0}"" {1} {2} {3} {4} -NonInteractive {5} {6} {7} {8}",
				GetProjectOrNuspecFile(arguments),
				Symbols ? "-Symbols" : "",
				NoDefaultExcludes ? "-NoDefaultExcludes" : "",
				GetVerbosity(),
				GetProperties(arguments),
				GetOutputDirectory(arguments),
				GetBasePath(arguments),
				IncludeReferencedProjects ? "-IncludeReferencedProjects" : "",
                GetExclude()                
			);
		}

	    private string GetExclude()
	    {
            return string.IsNullOrWhiteSpace(Exclude) ? "" : string.Format("-Exclude \"{0}\"", Exclude);
	    }

	    private string GetPackagesToPush(ArgumentOptions arguments, bool pushSymbolsPackage)
		{
            var packages = Directory.GetFiles(arguments.OutDir, $"{arguments.TargetName}.*.nupkg");
            foreach (var package in packages)
            {
                var isSymbolPackage = package.IndexOf("symbols", StringComparison.InvariantCultureIgnoreCase) != -1;
                if (pushSymbolsPackage && isSymbolPackage)
                    return package;

                if (!pushSymbolsPackage && !isSymbolPackage)
                    return package;
            }

            return string.Format("{0}\\{1}.*.nupkg", arguments.OutDir, arguments.TargetName);
		}

		private string GetPackageSource(ArgumentOptions arguments)
		{
			var source = arguments.OverrideSource == null 
				? Publish.Source 
				: arguments.OverrideSource;

			return !string.IsNullOrWhiteSpace(source)
				? string.Format("-Source {0}", source)
				: string.Empty;
		}

        private string GetSymbolPackageSource(ArgumentOptions arguments)
        {
            var source = arguments.OverrideSymbolSource == null
                ? Publish.SymbolSource
                : arguments.OverrideSymbolSource;

            return !string.IsNullOrWhiteSpace(source)
                ? string.Format("-SymbolSource {0}", source)
                : string.Empty;
        }

        private string GetApiKey(ArgumentOptions arguments)
		{
			return arguments.OverrideApiKey == null 
				? Publish.ApiKey 
				: arguments.OverrideApiKey;
		}

		public string GetTimeout()
		{
			return string.Format("-Timeout {0}", Publish.Timeout);
		}

		public string GetPushCommandArgs(ArgumentOptions arguments, bool pushSymbolsPackage)
		{
			return string.Format(@"push ""{0}"" {1} {2} {3} {4} -NonInteractive",
				GetPackagesToPush(arguments, pushSymbolsPackage),
				GetApiKey(arguments),
                pushSymbolsPackage 
                  ? GetSymbolPackageSource(arguments)
                  : GetPackageSource(arguments),
				GetTimeout(),
				GetVerbosity()
			);
		}
	}

}
