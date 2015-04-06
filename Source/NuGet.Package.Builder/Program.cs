using CommandLine;
using System.IO;
using System;
using System.Diagnostics;

namespace NuGet.Package.Builder
{
	class Program
	{
		static int Main(string[] args)
		{
			var arguments = new ArgumentOptions();
			if (!Parser.Default.ParseArguments(args, arguments))
			{
				arguments.GetUsage();
				return 1;
			}

			var options = PackageOptions.LoadOrDefault(arguments);
			BuildPackage(arguments, options);
			if (options.Publish.PublishOnBuild)
			{
				PushPackage(arguments, options);
			}

			if (arguments.GeneratePublishCommand)
			{
				CreatePublishCommand(arguments, options);
			}

			return 0;
		}

		private static void BuildPackage(ArgumentOptions args, PackageOptions options)
		{
			var startInfo = new ProcessStartInfo
			{
				Arguments = options.GetBuildCommandArgs(args),
				FileName = Path.Combine(args.PathToNuGet, "NuGet.exe"),
				CreateNoWindow = false,
				UseShellExecute = false,
				WorkingDirectory = args.WorkingDirectory
			};

			Console.WriteLine("{0} {1}", startInfo.FileName, startInfo.Arguments);
			using (var process = Process.Start(startInfo))
			{
				process.WaitForExit(5000);
			}
		}

		private static void PushPackage(ArgumentOptions args, PackageOptions options)
		{
			var startInfo = new ProcessStartInfo
			{
				Arguments = options.GetPushCommandArgs(args),
				FileName = Path.Combine(args.PathToNuGet, "NuGet.exe"),
				CreateNoWindow = false,
				UseShellExecute = false,
				WorkingDirectory = args.WorkingDirectory
			};

			Console.WriteLine("{0} {1}", startInfo.FileName, startInfo.Arguments);
			using (var process = Process.Start(startInfo))
			{
				process.WaitForExit(5000);
			}
		}

		//Publish Get-ChildItem -Path "C:\Users\Lars\Documents\visual studio 2013\Projects\ClassLibrary2" -File publish_*.cmd | Foreach { & $_.FullName 'apikey', 'https://mysource' }
		private static void CreatePublishCommand(ArgumentOptions args, PackageOptions options)
		{
			var solutionDir = Directory.GetParent(args.PathToNuGet + @"\..\..\..\").FullName;
			var outputFile = Path.Combine(solutionDir, string.Format("publish_{0}.cmd", args.TargetName));
			Console.WriteLine(outputFile);
			if (File.Exists(outputFile))
				File.Delete(outputFile);

			if (string.IsNullOrWhiteSpace(args.OverrideApiKey) && string.IsNullOrWhiteSpace(options.Publish.ApiKey))
				args.OverrideApiKey = "%1";

			if (string.IsNullOrWhiteSpace(args.OverrideSource) && string.IsNullOrWhiteSpace(options.Publish.Source))
				args.OverrideSource = "%2";

			File.WriteAllText(
				outputFile, 
				string.Format("\"{0}\" {1}", Path.Combine(args.PathToNuGet, "NuGet.exe"), options.GetPushCommandArgs(args))
			);
		}
	}
}
