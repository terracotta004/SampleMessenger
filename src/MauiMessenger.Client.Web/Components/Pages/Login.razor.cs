using Microsoft.AspNetCore.Components;
using MauiMessenger.Client.Web.Services;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Web.Components.Pages;

public partial class Login
{
    [Inject] private ApiClient ApiClient { get; set; } = default!;
    [Inject] private CurrentUserState CurrentUserState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private LoginForm login = new();
    private bool isSubmitting;
    private string? errorMessage;

    private async Task LoginAsync()
    {
        errorMessage = null;
        isSubmitting = true;

        try
        {
            var request = new LoginRequest(login.Username, login.Password);
            var user = await ApiClient.LoginAsync(request);
            CurrentUserState.SetUser(user);
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private sealed class LoginForm
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
