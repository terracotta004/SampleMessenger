using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MauiMessenger.Client.Mobile;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnBlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
	{
#if ANDROID
		var webView = e.WebView;
		webView.ClearCache(true);
		webView.ClearHistory();
		webView.Settings.CacheMode = Android.Webkit.CacheModes.NoCache;
#endif
	}
}

