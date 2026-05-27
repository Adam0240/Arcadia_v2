using Arcadia_Mobile.Services;

namespace Arcadia_Mobile.Dialogs;

public sealed class MauiPlayerNamePromptService : IPlayerNamePromptService
{
    public Task<string?> PromptForNameAsync()
    {
        return Shell.Current.DisplayPromptAsync(
            "New Game",
            "What is your name?",
            accept: "Continue",
            cancel: "Cancel",
            placeholder: "Name",
            maxLength: 24,
            keyboard: Keyboard.Text);
    }

    public Task ShowEmptyNameMessageAsync()
    {
        return Shell.Current.DisplayAlertAsync("Name Required", "Please enter a name before continuing.", "OK");
    }

    public Task<bool> ConfirmNameAsync(string playerName)
    {
        return Shell.Current.DisplayAlertAsync(
            "Confirm Name",
            $"Is your name {playerName}?",
            accept: "Yes",
            cancel: "No");
    }
}
