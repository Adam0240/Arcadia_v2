using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Arcadia_Mobile.Map;
using Arcadia_Mobile.Services;

namespace Arcadia_Mobile.ViewModels;

public sealed class MainPageViewModel : INotifyPropertyChanged
{
    private readonly MobileGameSession gameSession;
    private string statusMessage = "The journey begins.";

    public MainPageViewModel(MobileGameSession gameSession)
    {
        this.gameSession = gameSession;

        MoveNorthCommand = new Command(() => Move(RoomDirection.North));
        MoveEastCommand = new Command(() => Move(RoomDirection.East));
        MoveSouthCommand = new Command(() => Move(RoomDirection.South));
        MoveWestCommand = new Command(() => Move(RoomDirection.West));
        InteractCommand = new Command(Interact);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string RoomName => gameSession.CurrentRoom.Name;
    public string RoomDescription => gameSession.CurrentRoom.Description;
    public string RoomImageSource => gameSession.CurrentRoom.ImageName;
    public bool CanMoveNorth => gameSession.CanMove(RoomDirection.North);
    public bool CanMoveEast => gameSession.CanMove(RoomDirection.East);
    public bool CanMoveSouth => gameSession.CanMove(RoomDirection.South);
    public bool CanMoveWest => gameSession.CanMove(RoomDirection.West);

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

    public ICommand MoveNorthCommand { get; }
    public ICommand MoveEastCommand { get; }
    public ICommand MoveSouthCommand { get; }
    public ICommand MoveWestCommand { get; }
    public ICommand InteractCommand { get; }

    private void Move(RoomDirection direction)
    {
        MoveResult result = gameSession.Move(direction);
        StatusMessage = result.Message;
        RefreshRoomState();
    }

    private void Interact()
    {
        StatusMessage = gameSession.Interact();
    }

    private void RefreshRoomState()
    {
        OnPropertyChanged(nameof(RoomName));
        OnPropertyChanged(nameof(RoomDescription));
        OnPropertyChanged(nameof(RoomImageSource));
        OnPropertyChanged(nameof(CanMoveNorth));
        OnPropertyChanged(nameof(CanMoveEast));
        OnPropertyChanged(nameof(CanMoveSouth));
        OnPropertyChanged(nameof(CanMoveWest));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
