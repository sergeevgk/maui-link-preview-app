namespace LinkPreviewApp;

public partial class LinkPreviewItemTemplate
{
	public LinkPreviewItemTemplate()
	{
		InitializeComponent();
	}

	private async void Image_Loaded(object sender, EventArgs e)
	{
		try
		{
			if (LinkPreviewImage.Source.IsEmpty)
			{
				LinkPreviewImage.Source = ImageSource.FromFile("fallback_100.png");
			}
		}

		catch (Exception ex)
		{
			LinkPreviewImage.Source = ImageSource.FromFile("fallback_100.png");
		}
	}
}