using Arcadia_Mobile.Map;
using Arcadia_Mobile.Saves;
using Arcadia_Mobile.Services;
using Arcadia_Mobile.ViewModels;
using Microsoft.Extensions.Logging;

namespace Arcadia_Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<GameMap>();
        builder.Services.AddSingleton<MobileGameSession>();
        builder.Services.AddSingleton<IMobileGameSaveRepository>(_ =>
            new FileMobileGameSaveRepository(Path.Combine(FileSystem.AppDataDirectory, "savegame.json")));
        builder.Services.AddSingleton<MobileGameSaveService>();
        builder.Services.AddTransient<StartMenuViewModel>();
        builder.Services.AddTransient<StartMenuPage>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
