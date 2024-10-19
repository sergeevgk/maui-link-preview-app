namespace LinkPreviewApp.ApiService;

public record LinkPreviewResponse(string Title, string Description, string Image, string Url, int? Error = null);
