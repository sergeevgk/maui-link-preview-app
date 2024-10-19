using Microsoft.Extensions.Options;
using Polly;
using RestSharp;

namespace LinkPreviewApp.ApiService.Services;

public class ExternalUrlDataService : IUrlDataService
{
	private readonly RestClient _httpClient;

	// Retry policy options for connection failures
	private static readonly int MaxRetryAttempts = 2;
	private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);

	public ExternalUrlDataService(IOptions<LinkPreviewServiceSettings> settings)
	{
		ArgumentNullException.ThrowIfNull(settings);

		var restClientOptions = new RestClientOptions
		{
			FollowRedirects = true,
			Timeout = TimeSpan.FromSeconds(10),
			BaseUrl = new Uri(settings.Value.BaseUri)
		};
		_httpClient = new RestClient(restClientOptions);
		_httpClient.AddDefaultHeader("X-Linkpreview-Api-Key", settings.Value.ApiKey);
	}

	public async Task<UrlData> GetUrlDataAsync(string url)
	{
		try
		{
			var request = new RestRequest("", Method.Post);
			request.AddJsonBody(new { q = url });

			var retryPolicy = Policy
				.HandleResult<RestResponse<LinkPreviewResponse>>(r => !r.IsSuccessful)
				.WaitAndRetryAsync(MaxRetryAttempts, retryAttempt => PauseBetweenFailures, (response, timespan, retryCount, context) =>
					{
						Console.WriteLine($"Retry {retryCount} due to: {response.Result.ErrorMessage}");
					}
				);

			var response = await retryPolicy.ExecuteAsync(() => _httpClient.ExecuteAsync<LinkPreviewResponse>(request));

			if (!response.IsSuccessful || response.Data == null)
			{
				return new UrlData(url, "Error", response.ErrorMessage ?? "Unknown error", "Unknown", string.Empty);
			}

			if (response.Data.Error.HasValue)
			{
				return new UrlData(url, "Error", response.Data.Description, "Unknown", string.Empty);
			}

			var result = new UrlData(
				response.Data.Url,
				response.Data.Title,
				response.Data.Description,
				new Uri(response.Data.Url).Host,
				response.Data.Image ?? string.Empty
			);

			return result;
		}
		catch (Exception ex)
		{
			return new UrlData(url, "Error", ex.Message, "Unknown", string.Empty);
		}
	}
}
