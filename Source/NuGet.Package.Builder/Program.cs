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
            if (!ShouldBuildPackage(arguments, options))
            {
                return 0;
            }

			BuildPackage(arguments, options);
			if (options.Publish.PublishOnBuild)
			{
				PushPackages(arguments, options);
			}

			if (arguments.GeneratePublishCommand)
			{
				CreatePublishCommand(arguments, options);
			}

			return 0;
		}

        private static bool ShouldBuildPackage(ArgumentOptions arguments, PackageOptions options)
        {
            return string.IsNullOrEmpty(options.Configuration)
                || string.Compare(options.Configuration, arguments.Configuration, true) == 0;
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
				process.WaitForExit();
			}
		}

		private static void PushPackages(ArgumentOptions args, PackageOptions options)
		{
            PushPackage(args, options, false);
            if (options.Symbols)
                PushPackage(args, options, true);
        }

        private static void PushPackage(ArgumentOptions args, PackageOptions options, bool pushSymbolPackage)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = options.GetPushCommandArgs(args, pushSymbolPackage),
                FileName = Path.Combine(args.PathToNuGet, "NuGet.exe"),
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = args.WorkingDirectory
            };

            Console.WriteLine("{0} {1}", startInfo.FileName, startInfo.Arguments);
            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }

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
				string.Format("\"{0}\" {1}", Path.Combine(args.PathToNuGet, "NuGet.exe"), options.GetPushCommandArgs(args, false))
			);
		}
	}
}
