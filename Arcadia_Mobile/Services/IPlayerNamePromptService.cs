namespace Arcadia_Mobile.Services;

public interface IPlayerNamePromptService
{
    Task<string?> PromptForNameAsync();
    Task ShowEmptyNameMessageAsync();
    Task<bool> ConfirmNameAsync(string playerName);
}
