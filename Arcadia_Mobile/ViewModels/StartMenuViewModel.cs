using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Arcadia_Mobile.Saves;
using Arcadia_Mobile.Services;

namespace Arcadia_Mobile.ViewModels;

public sealed class StartMenuViewModel : INotifyPropertyChanged
{
    private readonly MobileGameSession gameSession;
    private readonly MobileGameSaveService saveService;
    private readonly IPlayerNamePromptService playerNamePromptService;
    private bool hasSave;
    private bool isCheckingSaveState = true;
    private string statusMessage = string.Empty;

    public StartMenuViewModel(
        MobileGameSession gameSession,
        MobileGameSaveService saveService,
        IPlayerNamePromptService playerNamePromptService)
    {
        this.gameSession = gameSession;
        this.saveService = saveService;
        this.playerNamePromptService = playerNamePromptService;

        NewGameCommand = new Command(async () => await StartNewGameAsync());
        LoadGameCommand = new Command(async () => await LoadGameAsync());
        DeleteGameCommand = new Command(async () => await DeleteGameAsync());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool HasSave
    {
        get => hasSave && CanShowMenuActions;
        private set
        {
            if (hasSave == value)
            {
                return;
            }

            hasSave = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasNoSave));
        }
    }

    public bool IsCheckingSaveState
    {
        get => isCheckingSaveState;
        private set
        {
            if (isCheckingSaveState == value)
            {
                return;
            }

            isCheckingSaveState = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanShowMenuActions));
            OnPropertyChanged(nameof(HasSave));
            OnPropertyChanged(nameof(HasNoSave));
        }
    }

    public bool CanShowMenuActions => !isCheckingSaveState;
    public bool HasNoSave => !hasSave && CanShowMenuActions;

    public string StatusMessage
    {
        get => statusMessage;
        private set
        {
            if (statusMessage == value)
            {
                return;
            }

            statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand NewGameCommand { get; }
    public ICommand LoadGameCommand { get; }
    public ICommand DeleteGameCommand { get; }

    public async Task RefreshSaveStateAsync()
    {
        IsCheckingSaveState = true;

        try
        {
            await saveService.InitializeAsync();
            HasSave = await saveService.HasSaveAsync();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            HasSave = false;
            StatusMessage = "Save data could not be checked.";
        }
        finally
        {
            IsCheckingSaveState = false;
        }
    }

    private async Task StartNewGameAsync()
    {
        while (true)
        {
            string? enteredName = await playerNamePromptService.PromptForNameAsync();

            if (enteredName == null)
            {
                StatusMessage = string.Empty;
                return;
            }

            string playerName = enteredName.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                await playerNamePromptService.ShowEmptyNameMessageAsync();
                continue;
            }

            bool confirmed = await playerNamePromptService.ConfirmNameAsync(playerName);

            if (!confirmed)
            {
                continue;
            }

            gameSession.StartNewGame(playerName);
            await Shell.Current.GoToAsync("//Explore");
            return;
        }
    }

    private async Task LoadGameAsync()
    {
        MobileSaveCommandResult result = await saveService.LoadAsync(gameSession);
        StatusMessage = result.Message;

        if (result.Succeeded)
        {
            await Shell.Current.GoToAsync("//Explore");
        }
        else
        {
            await RefreshSaveStateAsync();
        }
    }

    private async Task DeleteGameAsync()
    {
        MobileSaveCommandResult result = await saveService.DeleteAsync();
        StatusMessage = result.Message;
        await RefreshSaveStateAsync();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
