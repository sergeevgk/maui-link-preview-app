using LinkPreviewApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
#if USE_BACKEND_SERVICE
			builder.Services.AddSingleton<IUrlDataService, BackendUrlDataService>();
#else
			builder.Services.AddSingleton<IUrlDataService, CustomUrlDataService>();
#endif
			builder.Services.AddTransient<AppShell>();
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<LinkPreviewModel>();
			builder.Services.AddTransient<LinkPreviewCollectionViewModel>();

			var asm = Assembly.GetExecutingAssembly();// for appsettings.json included in MAUI project as __Embedded Resource__ (important!)

			string appsettingsFileName = $"{asm.GetName().Name}.appsettings.json";
#if DEBUG
			builder.Logging.AddDebug();
			appsettingsFileName = $"{asm.GetName().Name}.appsettings.Development.json";
#endif
			using var stream = asm.GetManifestResourceStream(appsettingsFileName);
			builder.Configuration.AddJsonStream(stream!);
			var isAndroid = DeviceInfo.Platform == DevicePlatform.Android;
			builder.Services.AddOptions<InternalLinkPreviewServiceSettings>()
				.BindConfiguration(isAndroid ? "Android:InternalLinkPreviewService" : "Default:InternalLinkPreviewService");


			return builder.Build();
		}
	}
}
