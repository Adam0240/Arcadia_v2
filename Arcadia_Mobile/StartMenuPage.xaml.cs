using Arcadia_Mobile.ViewModels;

namespace Arcadia_Mobile;

public partial class StartMenuPage : ContentPage
{
    private readonly StartMenuViewModel viewModel;

    public StartMenuPage(StartMenuViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.RefreshSaveStateAsync();
    }
}
