using LinkPreviewApp.ApiService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LinkPreviewApp.ApiService.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LinkPreviewController : ControllerBase
	{
		private readonly ILogger<LinkPreviewController> _logger;
		private readonly IUrlDataService _service;

		public LinkPreviewController(ILogger<LinkPreviewController> logger, IUrlDataService service)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_service = service ?? throw new ArgumentNullException(nameof(service));
		}

		[HttpGet]
		public async Task<ActionResult<LinkPreviewResponse>> GetLinkPreview([FromQuery]string url)
		{
			_logger.LogInformation($"Process {nameof(GetLinkPreview)}.");

			var isValidUri = Uri.TryCreate(url, UriKind.Absolute, out var validatedUrl);
			if (!isValidUri)
			{
				_logger.LogError($"Error during url validaition {url}");
				return BadRequest($"Url {url} is not a valid URL.");
			}

			UrlData result = null;
			try
			{
				result = await _service.GetUrlDataAsync(url);

				var serializedResult = JsonSerializer.Serialize(result);
				_logger.LogInformation($"Finish {nameof(GetLinkPreview)} with result {serializedResult}.");

			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Error processing the request for url {url}. {ex.Message}");
				return Problem($"{url}, {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
			}

			return Ok(result);
		}
	}
}
