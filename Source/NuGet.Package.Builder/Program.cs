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
	}
}
