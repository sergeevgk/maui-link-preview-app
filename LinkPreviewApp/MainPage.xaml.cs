using CommunityToolkit.Mvvm.ComponentModel;

namespace LinkPreviewApp;

public partial class MainPage : ContentPage
{
	public MainPage(LinkPreviewCollectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
