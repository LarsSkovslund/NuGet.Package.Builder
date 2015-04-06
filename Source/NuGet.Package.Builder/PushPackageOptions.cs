namespace NuGet.Package.Builder
{
	public class PushPackageOptions
	{
		public bool PublishOnBuild { get; set; }
		public string ApiKey { get; set; }
		public string Source { get; set; }
		public int Timeout { get; set; }

		public PushPackageOptions()
		{
			PublishOnBuild = false;
			Timeout = 300;
		}
	}
}
