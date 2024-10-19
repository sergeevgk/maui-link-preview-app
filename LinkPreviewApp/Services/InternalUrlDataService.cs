﻿using Polly;
using RestSharp;

namespace LinkPreviewApp.Services;

public class InternalUrlDataService : IUrlDataService
{
	private readonly RestClient _httpClient;
	// TODO: provide a link in settings for Development and Release.
	private const string ApiUrl = "https://localhost:1234";

	// Retry policy options for connection failures
	private static readonly int MaxRetryAttempts = 2;
	private static readonly TimeSpan PauseBetweenFailures = TimeSpan.FromSeconds(2);

	public InternalUrlDataService()
	{
		var restClientOptions = new RestClientOptions
		{
			FollowRedirects = true,
			Timeout = TimeSpan.FromSeconds(10),
			BaseUrl = new Uri(ApiUrl)
		};
		_httpClient = new RestClient(restClientOptions);
	}

	public async Task<UrlData> GetUrlDataAsync(string url)
	{
		try
		{
			var request = new RestRequest("/api/LinkPreview", Method.Get)
				.AddParameter("url", url);

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

			var result =  new UrlData(
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
