namespace LinkPreviewApp.ApiService.Services;

public interface IUrlDataService
{
	public Task<UrlData> GetUrlDataAsync(string url);
}
