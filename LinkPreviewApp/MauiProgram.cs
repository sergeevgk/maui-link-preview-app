using LinkPreviewApp.Services;
using Microsoft.Extensions.Logging;

namespace LinkPreviewApp
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			builder.Services.AddSingleton<HttpClient>();
			builder.Services.AddSingleton<IUrlDataService, UrlDataService>();
			builder.Services.AddTransient<AppShell>();
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<LinkPreviewModel>();
			builder.Services.AddTransient<LinkPreviewCollectionViewModel>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
