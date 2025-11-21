namespace FW.Common.WebUtilties.Helpers
{
	public static class NextJSBFFHelper
	{
		public static bool DoesNextJsRelativePathExist(this string relativePath)
		{
			relativePath = relativePath.Trim ('/');
			var segments = relativePath.Split ('/', StringSplitOptions.RemoveEmptyEntries);

			var root = Path.Combine (Directory.GetCurrentDirectory (), "client-app", "out");
			var targetPagePath = Path.Combine (root, Path.Combine (segments), "index.html");

			return File.Exists (targetPagePath);
		}	
	}
}