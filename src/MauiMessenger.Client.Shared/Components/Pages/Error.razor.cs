using System.Diagnostics;

namespace MauiMessenger.Client.Shared.Components.Pages;

public partial class Error
{
    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id;
}
