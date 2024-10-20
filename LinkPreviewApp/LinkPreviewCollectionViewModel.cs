using CommunityToolkit.Mvvm.Input;
using LinkPreviewApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LinkPreviewApp;

public partial class LinkPreviewCollectionViewModel : INotifyPropertyChanged
{
	private readonly IUrlDataService _urlDataService;

	public event PropertyChangedEventHandler? PropertyChanged;

	public ICommand FetchPreviewCommand { get; }
	public IList<LinkPreviewModel> LinkPreviews { get; set; } = new ObservableCollection<LinkPreviewModel>();

	private string? _enteredUrlText;
	public string? EnteredUrlText
	{
		get => _enteredUrlText;
		set => SetProperty(ref _enteredUrlText, value);
	}

	private int _collectionViewHeight;
	public int CollectionViewHeight
	{
		get => _collectionViewHeight;
		set => SetProperty(ref _collectionViewHeight, value);
	}

	public LinkPreviewCollectionViewModel(IUrlDataService urlDataService)
	{
		_urlDataService = urlDataService ?? throw new ArgumentNullException(nameof(urlDataService));
		FetchPreviewCommand = new Command(async () => await FetchPreviewAsync(EnteredUrlText));
		CollectionViewHeight = 430;
	}

	private async Task FetchPreviewAsync(string urlText)
	{
		try
		{
			var urls = urlText.Split(['\n', '\r']);
			var asyncLoadTasks = urls.Select((url) => FetchAndSaveLinkPreview(url));

			await Task.WhenAll(asyncLoadTasks);
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

	private async Task FetchAndSaveLinkPreview(string url)
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
			await Shell.Current.DisplayAlert("Alert", "The provided link is invalid", "OK");
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
