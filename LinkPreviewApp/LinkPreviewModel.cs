using CommunityToolkit.Mvvm.ComponentModel;

namespace LinkPreviewApp;

public class LinkPreviewModel : ObservableObject
{
	private string _urlText;
	private string _title;
	private string _description;
	private string _image;
	private bool _hasError;
	private string _errorMessage;

	public string UrlText { get => _urlText; set => SetProperty(ref _urlText, value); }
	public string Url => UrlText; // TODO: validate the UrlText before processing, save sanitized link in Url?
	public string Title { get => _title; set => SetProperty(ref _title, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public string Image { get => _image; set => SetProperty(ref _image, value); }
	public bool IsPreviewAvailable => !string.IsNullOrEmpty(Title);
	public bool HasError { get => _hasError; set => SetProperty(ref _hasError, value); }
	public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
}
