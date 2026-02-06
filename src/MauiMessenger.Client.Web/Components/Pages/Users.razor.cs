using Microsoft.AspNetCore.Components;
using MauiMessenger.Client.Web.Services;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Web.Components.Pages;

public partial class Users
{
    [Inject] private ApiClient ApiClient { get; set; } = default!;

    private List<UserDto>? users;
    private CreateUserForm newUser = new();
    private bool isSaving;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        errorMessage = null;
        users = (await ApiClient.GetUsersAsync()).ToList();
    }

    private async Task CreateUserAsync()
    {
        errorMessage = null;
        isSaving = true;

        try
        {
            var request = new CreateUserRequest(newUser.Username, newUser.DisplayName, newUser.Email, newUser.Password);
            await ApiClient.CreateUserAsync(request);
            newUser = new CreateUserForm();
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSaving = false;
        }
    }

    private sealed class CreateUserForm
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
