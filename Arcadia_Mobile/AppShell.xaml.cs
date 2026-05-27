namespace Arcadia_Mobile;

public partial class AppShell : Shell
{
    public AppShell(StartMenuPage startMenuPage, MainPage mainPage)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Title = "Start",
            Content = startMenuPage,
            Route = "StartMenu"
        });

        Items.Add(new ShellContent
        {
            Title = "Explore",
            Content = mainPage,
            Route = "Explore"
        });
    }
}
