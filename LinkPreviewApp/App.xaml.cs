namespace LinkPreviewApp;

public partial class App : Application
{
	public App(IServiceProvider services)
	{
		InitializeComponent();
		var appShell = services.GetRequiredService<AppShell>();
		MainPage = appShell;
	}
}
