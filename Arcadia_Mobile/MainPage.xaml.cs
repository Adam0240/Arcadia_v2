using Arcadia_Mobile.ViewModels;

namespace Arcadia_Mobile;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        this.viewModel = viewModel;
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.Refresh();
    }
}
