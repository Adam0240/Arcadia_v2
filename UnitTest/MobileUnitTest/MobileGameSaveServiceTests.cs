using Arcadia_Mobile.Map;
using Arcadia_Mobile.Saves;
using Arcadia_Mobile.Services;

namespace UnitTest.MobileUnitTest;

public class MobileGameSaveServiceTests
{
    // Checks that saving writes mobile session state and reports that a save exists.
    [Fact]
    public async Task SaveAsync_WritesCurrentRoomAndReportsSaveExists()
    {
        string saveDirectory = CreateSaveDirectory();

        try
        {
            MobileGameSession session = new(new GameMap());
            session.Move(RoomDirection.North);
            MobileGameSaveService saveService = CreateSaveService(saveDirectory);

            MobileSaveCommandResult result = await saveService.SaveAsync(session);

            Assert.True(result.Succeeded);
            Assert.Equal("Game saved.", result.Message);
            Assert.True(await saveService.HasSaveAsync());
        }
        finally
        {
            DeleteSaveDirectory(saveDirectory);
        }
    }

    // Checks that loading applies the saved current room to a fresh mobile session.
    [Fact]
    public async Task LoadAsync_WithSavedRoom_RestoresCurrentRoom()
    {
        string saveDirectory = CreateSaveDirectory();

        try
        {
            MobileGameSaveService saveService = CreateSaveService(saveDirectory);
            MobileGameSession savedSession = new(new GameMap());
            savedSession.Move(RoomDirection.North);
            savedSession.Move(RoomDirection.West);
            await saveService.SaveAsync(savedSession);
            MobileGameSession loadedSession = new(new GameMap());

            MobileSaveCommandResult result = await saveService.LoadAsync(loadedSession);

            Assert.True(result.Succeeded);
            Assert.Equal("Game loaded.", result.Message);
            Assert.Equal(RoomId.Road1, loadedSession.CurrentRoom.Id);
            Assert.Contains(RoomId.Ikena, loadedSession.VisitedRoomIds);
        }
        finally
        {
            DeleteSaveDirectory(saveDirectory);
        }
    }

    // Checks that deleting removes the mobile save slot.
    [Fact]
    public async Task DeleteAsync_WithExistingSave_RemovesSave()
    {
        string saveDirectory = CreateSaveDirectory();

        try
        {
            MobileGameSaveService saveService = CreateSaveService(saveDirectory);
            await saveService.SaveAsync(new MobileGameSession(new GameMap()));

            MobileSaveCommandResult result = await saveService.DeleteAsync();

            Assert.True(result.Succeeded);
            Assert.Equal("Save data deleted.", result.Message);
            Assert.False(await saveService.HasSaveAsync());
        }
        finally
        {
            DeleteSaveDirectory(saveDirectory);
        }
    }

    // Checks that invalid mobile save data returns a failure without changing the active room.
    [Fact]
    public async Task LoadAsync_WithInvalidRoomId_ReturnsFailure()
    {
        string saveDirectory = CreateSaveDirectory();

        try
        {
            FileMobileGameSaveRepository repository = CreateRepository(saveDirectory);
            await repository.SaveJsonAsync(
                """
                {
                  "Version": 1,
                  "Player": {
                    "CurrentRoomId": "MissingRoom",
                    "VisitedRoomIds": []
                  }
                }
                """);
            MobileGameSaveService saveService = new(repository);
            MobileGameSession session = new(new GameMap());

            MobileSaveCommandResult result = await saveService.LoadAsync(session);

            Assert.False(result.Succeeded);
            Assert.Equal("Save data could not be loaded.", result.Message);
            Assert.Equal(RoomId.MaiaStable, session.CurrentRoom.Id);
        }
        finally
        {
            DeleteSaveDirectory(saveDirectory);
        }
    }

    private static MobileGameSaveService CreateSaveService(string saveDirectory)
    {
        return new MobileGameSaveService(CreateRepository(saveDirectory));
    }

    private static FileMobileGameSaveRepository CreateRepository(string saveDirectory)
    {
        return new FileMobileGameSaveRepository(Path.Combine(saveDirectory, "savegame.json"));
    }

    private static string CreateSaveDirectory()
    {
        return Path.Combine(AppContext.BaseDirectory, "MobileSaveTests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteSaveDirectory(string saveDirectory)
    {
        if (Directory.Exists(saveDirectory))
        {
            Directory.Delete(saveDirectory, recursive: true);
        }
    }
}
