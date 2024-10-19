namespace LinkPreviewApp.Services;

public class UrlDataService : IUrlDataService
{
	public async Task<UrlData> GetUrlDataAsync(string url)
	{
		return new UrlData(url, "Title", "Description", "Source", "https://aka.ms/campus.jpg");
	}
}
