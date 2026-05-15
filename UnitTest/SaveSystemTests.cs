using Arcadia_v2;
using Arcadia_v2.Saves;
using System.Text;

namespace UnitTest
{
    public class SaveSystemTests
    {
        [Fact]
        public void Repository_Initialize_CreatesDatabaseFile()
        {
            string databasePath = CreateTemporaryDatabasePath();

            try
            {
                SqliteGameSaveRepository repository = new(databasePath);

                repository.Initialize();

                Assert.True(File.Exists(databasePath));
            }
            finally
            {
                DeleteTemporaryDatabase(databasePath);
            }
        }

        [Fact]
        public void Repository_SaveThenLoad_ReturnsLatestJson()
        {
            string databasePath = CreateTemporaryDatabasePath();

            try
            {
                SqliteGameSaveRepository repository = new(databasePath);
                repository.Initialize();

                repository.SaveJson("""{"value":1}""");
                repository.SaveJson("""{"value":2}""");

                Assert.Equal("""{"value":2}""", repository.LoadJson());
            }
            finally
            {
                DeleteTemporaryDatabase(databasePath);
            }
        }

        [Fact]
        public void Repository_DeleteSave_RemovesSlot()
        {
            string databasePath = CreateTemporaryDatabasePath();

            try
            {
                SqliteGameSaveRepository repository = new(databasePath);
                repository.Initialize();
                repository.SaveJson("""{"value":1}""");

                bool firstDelete = repository.DeleteSave();
                bool secondDelete = repository.DeleteSave();

                Assert.True(firstDelete);
                Assert.False(secondDelete);
                Assert.Null(repository.LoadJson());
            }
            finally
            {
                DeleteTemporaryDatabase(databasePath);
            }
        }

        [Fact]
        public void Serializer_RoundTripsGameSaveState()
        {
            GameSaveState state = new()
            {
                Player = new PlayerSaveState
                {
                    Name = "Red",
                    CurrentRoomName = "Ikena",
                    Badges = new List<string> { "Grass Badge" },
                    PokemonInventory = new List<PokemonSaveState>
                    {
                        new() { Id = 1, Name = "BULBASAUR", Health = 30 }
                    }
                },
                Rooms = new List<RoomSaveState>
                {
                    new()
                    {
                        Name = "Route 1",
                        EncounterPokemon = new List<PokemonSaveState>
                        {
                            new() { Id = 3, Name = "PIDGEY", Health = 12 }
                        }
                    }
                },
                Trainers = new List<TrainerSaveState>
                {
                    new()
                    {
                        Name = "Mrs. Mcmann",
                        CurrentRoomName = "Oak Pass",
                        Defeated = true,
                        Badges = new List<string> { "Grass Badge" },
                        BattleTeamTemplate = new List<PokemonSaveState>
                        {
                            new() { Id = 4, Name = "CHARMANDER", Health = 20 }
                        }
                    }
                }
            };

            string json = GameSaveSerializer.Serialize(state);
            GameSaveState restored = GameSaveSerializer.Deserialize(json);

            Assert.Equal("Red", restored.Player.Name);
            Assert.Equal("Ikena", restored.Player.CurrentRoomName);
            Assert.Single(restored.Player.Badges);
            Assert.Single(restored.Rooms);
            Assert.True(restored.Trainers[0].Defeated);
        }

        [Fact]
        public void Mapper_CaptureAndApply_RestoresRuntimeState()
        {
            GameState gameState = CreateGameState("Red");
            Pokemon playerPokemon = gameState.MainPlayer.PokemonInventory[0];
            playerPokemon.Health = 7;

            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Route 2"));
            gameState.MainPlayer.AddBadge("Grass Badge");
            gameState.GymLeader1.Defeated = true;
            gameState.GameMap.GetRoom("Route 1").RestoreEncounterPokemon(new[] { GameData.CreatePokemon()[4] });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestoreName("Changed");
            gameState.MainPlayer.MoveTo(gameState.GameMap.StartRoom);
            gameState.MainPlayer.RestoreBadges(Array.Empty<string>());
            gameState.MainPlayer.RestorePokemonInventory(new[] { GameData.CreatePokemon()[5] });
            gameState.GameMap.GetRoom("Route 1").RestoreEncounterPokemon(Array.Empty<Pokemon>());
            gameState.GymLeader1.Defeated = false;

            GameStateMapper.Apply(gameState, captured);

            Assert.Equal("Red", gameState.MainPlayer.Name);
            Assert.Equal("Route 2", gameState.MainPlayer.CurrentRoom.Name);
            Assert.Equal("Grass Badge", gameState.MainPlayer.Badges[0]);
            Assert.Equal(playerPokemon.Id, gameState.MainPlayer.PokemonInventory[0].Id);
            Assert.Equal(7, gameState.MainPlayer.PokemonInventory[0].Health);
            Assert.Single(gameState.GameMap.GetRoom("Route 1").EncounterPokemon);
            Assert.True(gameState.GymLeader1.Defeated);
        }

        [Fact]
        public void StartupFlow_WithSave_LoadsSavedGameWithoutShowingNewGameOption()
        {
            GameState savedGameState = CreateGameState("Red");
            savedGameState.MainPlayer.MoveTo(savedGameState.GameMap.GetRoom("Route 2"));
            string saveJson = GameSaveSerializer.Serialize(GameStateMapper.Capture(savedGameState));
            GameSaveService saveService = new(new FakeGameSaveRepository(saveJson));

            GameState loadedGameState = RunWithConsoleIo("2", () => StartupFlow.Run(saveService), out string output);

            Assert.Equal("Red", loadedGameState.MainPlayer.Name);
            Assert.Equal("Route 2", loadedGameState.MainPlayer.CurrentRoom.Name);
            Assert.DoesNotContain("1. New Game", output);
            Assert.Contains("2. Load Game", output);
            Assert.Contains("3. Delete Game", output);
            Assert.Contains("Game loaded.", output);
        }

        [Fact]
        public void StartupFlow_DeleteExistingSaveThenNewGame_ShowsNewGameOptionAgain()
        {
            GameState savedGameState = CreateGameState("Red");
            string saveJson = GameSaveSerializer.Serialize(GameStateMapper.Capture(savedGameState));
            FakeGameSaveRepository repository = new(saveJson);
            GameSaveService saveService = new(repository);

            GameState newGameState = RunWithConsoleIo(
                "3" + Environment.NewLine + "yes" + Environment.NewLine + "1" + Environment.NewLine + "Blue",
                () => StartupFlow.Run(saveService),
                out string output);

            Assert.Equal("Blue", newGameState.MainPlayer.Name);
            Assert.False(repository.HasSave);
            Assert.Contains("Are you sure you want to delete?", output);
            Assert.Contains("Save data deleted.", output);
            Assert.Contains("1. New Game", output);
        }

        [Fact]
        public void StartupFlow_LoadWithoutSave_PrintsNoSaveDataFoundAndReturnsToPrompt()
        {
            GameSaveService saveService = new(new FakeGameSaveRepository(null));

            GameState newGameState = RunWithConsoleIo(
                "2" + Environment.NewLine + "1" + Environment.NewLine + "Blue",
                () => StartupFlow.Run(saveService),
                out string output);

            Assert.Equal("Blue", newGameState.MainPlayer.Name);
            Assert.Contains("No save data found.", output);
        }

        private static GameState CreateGameState(string playerName)
        {
            List<Pokemon> mainPokemon = GameData.CreatePokemon();
            List<Pokemon> gymPokemon = GameData.CreatePokemon();
            Arcadia_v2.Map.Map gameMap = new();

            Player mainPlayer = new(playerName, gameMap.StartRoom);
            mainPlayer.AddPokemon(mainPokemon[1]);
            mainPlayer.AddPokemon(mainPokemon[2]);

            CompPlayer gymLeader1 = new("Mrs. Mcmann", gameMap.GymLeader1Room);
            gymLeader1.SetBattleTeam(new[] { gymPokemon[3], gymPokemon[14] });
            gymLeader1.AddBadge("Grass Badge");

            CompPlayer gymLeader2 = new("Minofo", gameMap.GymLeader2Room);
            gymLeader2.SetBattleTeam(new[] { gymPokemon[5], gymPokemon[9] });
            gymLeader2.AddBadge("Water Badge");

            CompPlayer gymLeader3 = new("Golden", gameMap.GymLeader3Room);
            gymLeader3.SetBattleTeam(new[] { gymPokemon[10], gymPokemon[16] });
            gymLeader3.AddBadge("Rock Badge");

            CompPlayer gymLeader4 = new("Wiggins", gameMap.GymLeader4Room);
            gymLeader4.SetBattleTeam(new[] { gymPokemon[13], gymPokemon[17] });
            gymLeader4.AddBadge("Dragon Badge");

            CompPlayer arcadiaChampion = new("Adam", gameMap.ChampionRoom);
            arcadiaChampion.SetBattleTeam(new[] { gymPokemon[4], gymPokemon[8], gymPokemon[6], gymPokemon[18] });
            arcadiaChampion.AddBadge("Champion Badge");

            return new GameState(
                gameMap,
                mainPokemon,
                gymPokemon,
                mainPlayer,
                gymLeader1,
                gymLeader2,
                gymLeader3,
                gymLeader4,
                arcadiaChampion);
        }

        private static string CreateTemporaryDatabasePath()
        {
            string directory = Path.Combine(Path.GetTempPath(), "ArcadiaSaveTests", Guid.NewGuid().ToString("N"));
            return Path.Combine(directory, "savegame.db");
        }

        private static void DeleteTemporaryDatabase(string databasePath)
        {
            string? directory = Path.GetDirectoryName(databasePath);

            if (directory != null && Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }

        private static T RunWithConsoleIo<T>(string input, Func<T> action, out string output)
        {
            TextReader originalIn = Console.In;
            TextWriter originalOut = Console.Out;
            StringReader reader = new(input + Environment.NewLine);
            StringWriter writer = new(new StringBuilder());

            try
            {
                Console.SetIn(reader);
                Console.SetOut(writer);
                T result = action();
                output = writer.ToString();
                return result;
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
                reader.Dispose();
                writer.Dispose();
            }
        }

        private sealed class FakeGameSaveRepository : IGameSaveRepository
        {
            private string? mSaveJson;

            public FakeGameSaveRepository(string? saveJson)
            {
                mSaveJson = saveJson;
            }

            public bool HasSave => mSaveJson != null;

            public void Initialize()
            {
            }

            public void SaveJson(string saveJson)
            {
                mSaveJson = saveJson;
            }

            public string? LoadJson()
            {
                return mSaveJson;
            }

            public bool DeleteSave()
            {
                bool hadSave = mSaveJson != null;
                mSaveJson = null;
                return hadSave;
            }
        }
    }
}
