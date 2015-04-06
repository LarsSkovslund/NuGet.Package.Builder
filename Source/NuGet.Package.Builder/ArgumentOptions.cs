using CommandLine;
using CommandLine.Text;

namespace NuGet.Package.Builder
{
	public class ArgumentOptions
	{
		[Option('a', "path", Required = true, HelpText = "Path to nuget.exe")]
		public string PathToNuGet { get; set; }

		[Option('t', "targetName", Required = true, HelpText = "Target name of project")]
		public string TargetName { get; set; }

		[Option('e', "projectExt", Required = true, HelpText = "Project extension (.csproj, .vbproj, .fsproj)")]
		public string ProjectExt { get; set; }

		[Option('p', "properties", Required = true, HelpText = "Default properties to set on nuget.exe pack -Properties")]
		public string Properties { get; set; }

		[Option('o', "outdir", Required = true, HelpText = "Output directory")]
		public string OutDir { get; set; }

		[Option('w', "workingDirectory", Required = true, HelpText = "Working directory")]
		public string WorkingDirectory { get; set; }

		[Option('f', "forcePublish", Required = false, DefaultValue = false, HelpText = "Force publishing of packages")]
		public bool ForcePublishing { get; set; }

		[Option("apiKey", DefaultValue = null, HelpText = "Override api key used to publish packages")]
		public string OverrideApiKey { get; set; }

		[Option("source", DefaultValue = null, HelpText = "Override publish source")]
		public string OverrideSource { get; set; }

		[Option("createPublishCommand", DefaultValue = false, HelpText = "Generate publish command")]
		public bool GeneratePublishCommand { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
