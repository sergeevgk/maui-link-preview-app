using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LinkPreviewApp.Services;

namespace LinkPreviewApp;

public partial class LinkPreviewCollectionViewModel : INotifyPropertyChanged
{
	private readonly IUrlDataService _urlDataService;

	public event PropertyChangedEventHandler? PropertyChanged;

	public ICommand FetchPreviewCommand { get; }
	public IList<LinkPreviewModel> LinkPreviews { get; set; } = new ObservableCollection<LinkPreviewModel>();

	private string _enteredUrl;
	public string EnteredUrl
	{
		get => _enteredUrl;
		set => SetProperty(ref _enteredUrl, value);
	}

	public LinkPreviewCollectionViewModel(IUrlDataService urlDataService)
	{
		_urlDataService = urlDataService ?? throw new ArgumentNullException(nameof(urlDataService));
		FetchPreviewCommand = new Command(async () => await FetchPreviewAsync(EnteredUrl));
	}

	private async Task FetchPreviewAsync(string url)
	{
		try
		{
			var urlData = await _urlDataService.GetUrlDataAsync(url);

			var linkPreview = new LinkPreviewModel
			{
				Title = urlData.Title,
				Description = urlData.Description,
				UrlText = urlData.Url,
				Image = urlData.Image,
				Source = urlData.Source
			};

			LinkPreviews.Add(linkPreview);
		}
		catch (Exception ex)
		{
			var errorMessage = $"Failed to fetch link preview. {ex.Message}";
			var linkPreview = new LinkPreviewModel
			{
				HasError = true,
				ErrorMessage = errorMessage
			};

			LinkPreviews.Add(linkPreview);
		}
	}

	[RelayCommand]
	private async Task TapLinkPreview(LinkPreviewModel item)
	{
		try
		{
			Uri uri = new Uri(item.Url);
			await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
		}
		catch (Exception ex)
		{
			// An unexpected error occurred. No browser may be installed on the device.
		}
	}

	[RelayCommand]
	private async Task DeleteLinkPreview(LinkPreviewModel item)
	{
		var previewRemoved = LinkPreviews.Remove(item);
	}

	private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
	{
		if (Equals(storage, value))
			return false;

		storage = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
