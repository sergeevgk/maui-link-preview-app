using HtmlAgilityPack;

namespace LinkPreviewApp.Services;

public class UrlDataService : IUrlDataService
{
	private readonly HttpClient _httpClient;
	public UrlDataService()
	{
		_httpClient = new HttpClient();
	}

	public async Task<UrlData> GetUrlDataAsync(string url)
	{
		string placeholderImageValue = string.Empty;
		try
		{
			// Make an HTTP request to get the page content
			var response = await _httpClient.GetAsync(url);

			// Get the actual final URL in case of redirects
			var finalUrl = response.RequestMessage.RequestUri.ToString();

			// Ensure a successful response
			response.EnsureSuccessStatusCode();

			var htmlContent = await response.Content.ReadAsStringAsync();

			// Use HtmlAgilityPack to parse the HTML content
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(htmlContent);

			// Extract title
			var title = htmlDocument.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim() ?? "No Title Available";

			// Extract meta description
			var descriptionNode = htmlDocument.DocumentNode.SelectNodes("//meta")
				?.FirstOrDefault(node => node.Attributes["name"]?.Value.ToLower() == "description" || node.Attributes["property"]?.Value.ToLower() == "og:description");
			var description = descriptionNode?.Attributes["content"]?.Value?.Trim() ?? "No Description Available";

			// Extract preview image (og:image or similar)
			var imageNode = htmlDocument.DocumentNode.SelectNodes("//meta")
				?.FirstOrDefault(node => node.Attributes["property"]?.Value.ToLower() == "og:image" || node.Attributes["name"]?.Value.ToLower() == "image");
			var image = imageNode?.Attributes["content"]?.Value ?? placeholderImageValue; // Placeholder image is in Resources, see fallback in template

			// Extract source (domain)
			var uri = new Uri(finalUrl);
			var source = uri.Host;

			// Return the extracted data
			return new UrlData(finalUrl, title, description, source, image);
		}
		catch (Exception ex)
		{
			// Return a failure result (log or handle the exception if needed)
			return new UrlData(url, "Error", ex.Message, "Unknown", placeholderImageValue); // Placeholder image is in Resources, see fallback in template
		}
	}
}
