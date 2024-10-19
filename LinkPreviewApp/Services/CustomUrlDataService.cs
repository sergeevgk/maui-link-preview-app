using HtmlAgilityPack;
using Polly;
using RestSharp;
using System.Net;

namespace LinkPreviewApp.Services;

public class CustomUrlDataService : IUrlDataService
{
	private readonly RestClient _httpClient;
	// retry policy options for connections
	private static int _maxRetryAttempts = 2;
	private static TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);

	public CustomUrlDataService()
	{
		var restClientOptions = new RestClientOptions
		{
			FollowRedirects = true,
			Timeout = TimeSpan.FromSeconds(10),
			UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
		};
		_httpClient = new RestClient();
		_httpClient.AddDefaultHeader("Connection", "keep-alive");
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
	}

	public async Task<UrlData> GetUrlDataAsync(string url)
	{
		string placeholderImageValue = string.Empty;
		try
		{
			// Make an HTTP request to get the page content
			var request = new RestRequest(url);
			var retryPolicy = Policy
				.HandleResult<RestResponse>(x => !x.IsSuccessful)
				.WaitAndRetryAsync(_maxRetryAttempts, x => _pauseBetweenFailures, (iRestResponse, timeSpan, retryCount, context) =>
				{
					// replace with some real logging
					Console.WriteLine($"The request failed. HttpStatusCode={iRestResponse.Result.StatusCode}. Waiting {timeSpan} seconds before retry. Number attempt {retryCount}. Uri={iRestResponse.Result.ResponseUri}; RequestResponse={iRestResponse.Result.Content}");
				});
			var response = await _httpClient.ExecuteGetAsync(request);
			var finalUrl = response.ResponseUri?.ToString();

			// Ensure a successful response
			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(message: $"{response.StatusCode} {response.ErrorMessage} {response.ErrorException}");
			}

			var htmlContent = response.Content;

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
			var image = imageNode?.Attributes["content"]?.Value ?? placeholderImageValue; // Placeholder image is in Resources, see LinkPreviewItemTemplate code-behind

			// Extract source (domain)
			var uri = new Uri(finalUrl);
			var source = uri.Host;

			// Return the extracted data
			var result = new UrlData(finalUrl, title, description, source, image);

			return result;
		}
		catch (Exception ex)
		{
			// Return a failure result (log or handle the exception if needed)
			return new UrlData(url, "Error", ex.Message, "Unknown", placeholderImageValue); // Placeholder image is in Resources, see LinkPreviewItemTemplate code-behind
		}
	}
}
