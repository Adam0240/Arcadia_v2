namespace Arcadia_Mobile;

public partial class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Title = "Explore",
            Content = mainPage,
            Route = "MainPage"
        });
    }
}
