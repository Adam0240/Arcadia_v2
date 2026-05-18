using Arcadia_v2;
using Arcadia_v2.Saves;

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
                        new()
                        {
                            Id = 1,
                            Name = "BULBASAUR",
                            Level = 5,
                            Health = 30,
                            BaseHealth = 40,
                            Speed = 7,
                            Moves = new List<MoveSaveState>
                            {
                                new() { Name = "TACKLE", Type = MoveType.Normal, Power = 5 }
                            }
                        }
                    }
                },
                Rooms = new List<RoomSaveState>
                {
                    new()
                    {
                        Name = "Road 1",
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
            Assert.Equal(5, restored.Player.PokemonInventory[0].Level);
            Assert.Equal(40, restored.Player.PokemonInventory[0].BaseHealth);
            Assert.Equal(7, restored.Player.PokemonInventory[0].Speed);
            Assert.Equal("TACKLE", restored.Player.PokemonInventory[0].Moves[0].Name);
        }

        [Fact]
        public void Mapper_CaptureAndApply_RestoresRuntimeState()
        {
            GameState gameState = CreateGameState("Red");
            Pokemon playerPokemon = gameState.MainPlayer.PokemonInventory[0];
            playerPokemon.Health = 7;

            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
            gameState.MainPlayer.AddBadge("Grass Badge");
            gameState.GymLeader1.Defeated = true;
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterPokemon(new[] { GameData.CreatePokemon()[4] });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestoreName("Changed");
            gameState.MainPlayer.MoveTo(gameState.GameMap.StartRoom);
            gameState.MainPlayer.RestoreBadges(Array.Empty<string>());
            gameState.MainPlayer.RestorePokemonInventory(new[] { GameData.CreatePokemon()[5] });
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterPokemon(Array.Empty<Pokemon>());
            gameState.GymLeader1.Defeated = false;

            GameStateMapper.Apply(gameState, captured);

            Assert.Equal("Red", gameState.MainPlayer.Name);
            Assert.Equal("Road 2", gameState.MainPlayer.CurrentRoom.Name);
            Assert.Equal("Grass Badge", gameState.MainPlayer.Badges[0]);
            Assert.Equal(playerPokemon.Id, gameState.MainPlayer.PokemonInventory[0].Id);
            Assert.Equal(7, gameState.MainPlayer.PokemonInventory[0].Health);
            Assert.Single(gameState.GameMap.GetRoom("Road 1").EncounterPokemon);
            Assert.True(gameState.GymLeader1.Defeated);
        }

        [Fact]
        public void Mapper_CaptureAndApply_RestoresPokemonRuntimeStatsAndMoves()
        {
            GameState gameState = CreateGameState("Red");
            Pokemon changedPokemon = new(
                1,
                "UMBREON",
                PokemonType.Dark,
                21,
                101,
                88,
                7,
                new[]
                {
                    new Move("CUSTOMBITE", MoveType.Dark, 13),
                    new Move("CUSTOMSPARK", MoveType.Electric, 11)
                });
            gameState.MainPlayer.RestorePokemonInventory(new[] { changedPokemon });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestorePokemonInventory(new[] { GameData.CreatePokemon()[2] });
            GameStateMapper.Apply(gameState, captured);

            Pokemon restoredPokemon = gameState.MainPlayer.PokemonInventory[0];
            Assert.Equal(2, captured.Version);
            Assert.Equal(1, restoredPokemon.Id);
            Assert.Equal("UMBREON", restoredPokemon.Name);
            Assert.Equal(21, restoredPokemon.Speed);
            Assert.Equal(101, restoredPokemon.BaseHealth);
            Assert.Equal(88, restoredPokemon.Health);
            Assert.Equal(7, restoredPokemon.Level);
            Assert.Equal("CUSTOMBITE", restoredPokemon.Moves[0].Name);
            Assert.Equal(MoveType.Dark, restoredPokemon.Moves[0].Type);
            Assert.Equal(13, restoredPokemon.Moves[0].Power);
            Assert.Equal("CUSTOMSPARK", restoredPokemon.Moves[1].Name);
        }

        [Fact]
        public void Mapper_ApplyVersion1PokemonSave_FallsBackToFactoryDefaultsForMissingRuntimeFields()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState oldSaveState = new()
            {
                Version = 1,
                Player = new PlayerSaveState
                {
                    Name = "Blue",
                    CurrentRoomName = gameState.GameMap.StartRoom.Name,
                    PokemonInventory = new List<PokemonSaveState>
                    {
                        new() { Id = 1, Name = "UMBREON", Health = 12 }
                    }
                }
            };

            GameStateMapper.Apply(gameState, oldSaveState);

            Pokemon template = GameData.CreatePokemon().Single(pokemon => pokemon.Id == 1);
            Pokemon restoredPokemon = gameState.MainPlayer.PokemonInventory[0];
            Assert.Equal("Blue", gameState.MainPlayer.Name);
            Assert.Equal(12, restoredPokemon.Health);
            Assert.Equal(template.Level, restoredPokemon.Level);
            Assert.Equal(template.BaseHealth, restoredPokemon.BaseHealth);
            Assert.Equal(template.Speed, restoredPokemon.Speed);
            Assert.Equal(template.Moves.Select(move => move.Name), restoredPokemon.Moves.Select(move => move.Name));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(76)]
        public void Mapper_ApplyPokemonSaveWithInvalidHealth_ThrowsInvalidOperationException(int savedHealth)
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = new()
            {
                Player = new PlayerSaveState
                {
                    Name = "Blue",
                    CurrentRoomName = gameState.GameMap.StartRoom.Name,
                    PokemonInventory = new List<PokemonSaveState>
                    {
                        new() { Id = 1, Name = "UMBREON", Health = savedHealth, BaseHealth = 75 }
                    }
                }
            };

            Assert.Throws<InvalidOperationException>(() => GameStateMapper.Apply(gameState, saveState));
        }

        [Fact]
        public void Service_LoadPokemonSaveWithInvalidHealth_ReturnsFailure()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = GameStateMapper.Capture(gameState);
            saveState.Player.PokemonInventory[0].Health = saveState.Player.PokemonInventory[0].BaseHealth + 1;
            string saveJson = GameSaveSerializer.Serialize(saveState);
            GameSaveService saveService = new(new FakeGameSaveRepository(saveJson));

            SaveCommandResult result = saveService.Load(gameState);

            Assert.False(result.Succeeded);
            Assert.Equal("Save data could not be loaded.", result.Message);
        }

        [Fact]
        public void StartupFlow_WithSave_LoadsSavedGameWithoutShowingNewGameOption()
        {
            GameState savedGameState = CreateGameState("Red");
            savedGameState.MainPlayer.MoveTo(savedGameState.GameMap.GetRoom("Road 2"));
            string saveJson = GameSaveSerializer.Serialize(GameStateMapper.Capture(savedGameState));
            GameSaveService saveService = new(new FakeGameSaveRepository(saveJson));
            FakeGameIO io = new("1");

            GameState loadedGameState = StartupFlow.Run(io, saveService);

            Assert.Equal("Red", loadedGameState.MainPlayer.Name);
            Assert.Equal("Road 2", loadedGameState.MainPlayer.CurrentRoom.Name);
            Assert.DoesNotContain("1. New Game", io.OutputText);
            Assert.Contains("1. Load Game", io.OutputText);
            Assert.Contains("2. Delete Game", io.OutputText);
            Assert.DoesNotContain("3. Delete Game", io.OutputText);
            Assert.Contains("Game loaded.", io.OutputText);
        }

        [Fact]
        public void StartupFlow_DeleteExistingSaveThenNewGame_ShowsNewGameOptionAgain()
        {
            GameState savedGameState = CreateGameState("Red");
            string saveJson = GameSaveSerializer.Serialize(GameStateMapper.Capture(savedGameState));
            FakeGameSaveRepository repository = new(saveJson);
            GameSaveService saveService = new(repository);
            FakeGameIO io = new("2", "yes", "1", "Blue");

            GameState newGameState = StartupFlow.Run(io, saveService);

            Assert.Equal("Blue", newGameState.MainPlayer.Name);
            Assert.False(repository.HasSave);
            Assert.Contains("Are you sure you want to delete?", io.OutputText);
            Assert.Contains("Save data deleted.", io.OutputText);
            Assert.Contains("1. New Game", io.OutputText);
            Assert.DoesNotContain("2. Load Game", io.OutputText);
            Assert.DoesNotContain("3. Delete Game", io.OutputText);
        }

        [Fact]
        public void StartupFlow_DeleteInvalidConfirmation_ReDisplaysConfirmationPrompt()
        {
            GameState savedGameState = CreateGameState("Red");
            string saveJson = GameSaveSerializer.Serialize(GameStateMapper.Capture(savedGameState));
            FakeGameSaveRepository repository = new(saveJson);
            GameSaveService saveService = new(repository);
            FakeGameIO io = new("2", "maybe", "no", "1");

            GameState loadedGameState = StartupFlow.Run(io, saveService);

            int confirmationPromptCount = io.OutputText.Split("Are you sure you want to delete?").Length - 1;

            Assert.Equal("Red", loadedGameState.MainPlayer.Name);
            Assert.True(repository.HasSave);
            Assert.Equal(2, confirmationPromptCount);
            Assert.Contains("Invalid input", io.OutputText);
            Assert.Contains("Game loaded.", io.OutputText);
        }

        [Fact]
        public void StartupFlow_InvalidChoiceWithoutSave_ReDisplaysOnlyNewGameOption()
        {
            GameSaveService saveService = new(new FakeGameSaveRepository(null));
            FakeGameIO io = new("2", "1", "Blue");

            GameState newGameState = StartupFlow.Run(io, saveService);

            Assert.Equal("Blue", newGameState.MainPlayer.Name);
            Assert.Contains("Invalid input", io.OutputText);
            Assert.DoesNotContain("No save data found.", io.OutputText);
            Assert.Contains("1. New Game", io.OutputText);
            Assert.DoesNotContain("2. Load Game", io.OutputText);
            Assert.DoesNotContain("3. Delete Game", io.OutputText);
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
    }
}
