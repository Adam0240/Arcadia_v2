using Microsoft.Extensions.DependencyInjection;

namespace Arcadia_Mobile;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Title = "Start",
            ContentTemplate = new DataTemplate(() => serviceProvider.GetRequiredService<StartMenuPage>()),
            Route = "StartMenu"
        });

        Items.Add(new ShellContent
        {
            Title = "Explore",
            ContentTemplate = new DataTemplate(() => serviceProvider.GetRequiredService<MainPage>()),
            Route = "Explore"
        });
    }
}
