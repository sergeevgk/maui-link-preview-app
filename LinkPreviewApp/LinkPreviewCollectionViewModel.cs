﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LinkPreviewApp.Services;

namespace LinkPreviewApp;

public class LinkPreviewCollectionViewModel : INotifyPropertyChanged
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
			var preview = await _urlDataService.GetUrlDataAsync(url);

			var linkPreview = new LinkPreviewModel
			{
				Title = preview.Title,
				Description = preview.Description,
				UrlText = preview.Url,
				Image = preview.Image,
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