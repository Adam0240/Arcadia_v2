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
    private bool hasSave;
    private string statusMessage = string.Empty;

    public StartMenuViewModel(MobileGameSession gameSession, MobileGameSaveService saveService)
    {
        this.gameSession = gameSession;
        this.saveService = saveService;

        NewGameCommand = new Command(async () => await StartNewGameAsync());
        LoadGameCommand = new Command(async () => await LoadGameAsync());
        DeleteGameCommand = new Command(async () => await DeleteGameAsync());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool HasSave
    {
        get => hasSave;
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

    public bool HasNoSave => !hasSave;

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
        await saveService.InitializeAsync();
        HasSave = await saveService.HasSaveAsync();
    }

    private async Task StartNewGameAsync()
    {
        gameSession.StartNewGame();
        await Shell.Current.GoToAsync("//Explore");
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
