# maui-link-preview-app
Test assignment: a .NET MAUI Mobile app with functionality of previewing a link in a pretty and informative way.

## Description
LinkPreviewApp is a .NET MAUI application built for mobile devices. Composes simple previews for provided URLs (links to websites), supports opening the links in a default device's browser.

Supports 2 modes: a simple "standalone" application with a link preview composition logic included in the app, and a "backend-integrated" application working with a LinkPreviewApp.ApiService - a Backend .NET Web API. The second option requires hosting the Backend service somewhere, otherwise available only for a local development.

The reason for this architecture decision is: LinkPreviewApp.ApiService communicates with an external Link Preview service to get the link previews. It uses an API Key which is a developer's secret. LinkPreviewApp is a client-side application, in which it is strictly recommended to not put any sensitive developer's secrets. See https://blog.gitguardian.com/how-to-handle-mobile-app-secrets/ for a more detailed information.

## Instructions:

### Standalone mode:
1. Build the application as you would normally. 
2. Use an IDE with emulator or publish the application and install on a real device.

### Backend-integrated mode:
Register on https://www.linkpreview.net/, obtain an API Key and provide it in the project LinkPreviewApp.ApiService in appsettings.json / appsettings.Development.json in:
```json
 "LinkPreviewService": {
     "BaseUri": "https://api.linkpreview.net",
     "ApiKey": "YourApiKey"
 }
```

#### To install on a device:
1. Build and publish the Backend service ('LinkPreviewApp.ApiService' project). One of the possible options is to publish it as Azure App Service appication. (requires hosting the application somewhere).
2. Update BackendLinkPreviewService.BaseUri in the appsettings.json of LinkPreviewApp project to use your Backend service URL.
3. Build and publish the MAUI application LinkPreviewApp with a flag '/p:UseBackendService=true' (requires signing the application).
4. Install the application on the device.

#### To launch in debug mode on an emulator:
1. Run the LinkPreviewApp.ApiService
2. Update the links in the LinkPreviewApp project appsettings.Development.json to use the correct port.
3. Run the LinkPreviewApp on some emulator or as a Windows Machine app (the app layout is not adjusted to different devices yet).

## Libraries used:
- [HTML Agility Pack](https://github.com/zzzprojects/html-agility-pack/)
- [Polly](https://github.com/App-vNext/Polly)
- [RestSharp](https://github.com/RestSharp/RestSharp)
- [.NET Community Toolkit](https://github.com/CommunityToolkit/dotnet), [MVVM Toolkit Samples](https://github.com/CommunityToolkit/MVVM-Samples)