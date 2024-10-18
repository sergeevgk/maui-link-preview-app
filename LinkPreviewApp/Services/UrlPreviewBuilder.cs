namespace LinkPreviewApp.Services;

public class UrlPreviewBuilder : IUrlPreviewBuilder
{
	public string BuildUrlPreview(string url)
	{
		return $"{url} with preview info";
	}
}
