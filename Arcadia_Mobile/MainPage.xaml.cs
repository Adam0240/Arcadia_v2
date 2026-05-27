using Arcadia_Mobile.ViewModels;

namespace Arcadia_Mobile;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
