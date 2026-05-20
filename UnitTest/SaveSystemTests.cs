using Arcadia_v2;
using Arcadia_v2.Saves;

namespace UnitTest
{
    public class SaveSystemTests
    {
        // Checks that initializing the save repository creates the backing database file.
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

        // Checks that saving twice and loading returns the most recent saved JSON payload.
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

        // Checks that saving the live game state writes the current animal-based runtime data into the SQLite save slot.
        [Fact]
        public void Repository_SaveGameState_PersistsCurrentAnimalRuntimeData()
        {
            string databasePath = CreateTemporaryDatabasePath();

            try
            {
                SqliteGameSaveRepository repository = new(databasePath);
                repository.Initialize();

                GameState gameState = CreateGameState("Red");
                Animal leadAnimal = gameState.MainPlayer.AnimalInventory[0];
                List<Animal> roadAnimals = GameData.CreateAnimals();
                Animal roadOneAnimal = GameData.FindAnimal(roadAnimals, AnimalElement.Nature, "Horse");
                leadAnimal.CurrentHealth = 7;
                gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
                gameState.MainPlayer.AddBadge("Grass Badge");
                gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(new[] { roadOneAnimal });
                gameState.GymLeader1.Defeated = true;

                GameSaveService saveService = new(repository);

                saveService.Save(gameState);

                string? persistedJson = repository.LoadJson();
                Assert.NotNull(persistedJson);
                GameSaveState persistedState = GameSaveSerializer.Deserialize(persistedJson);

                Assert.Equal(2, persistedState.Version);
                Assert.Equal("Red", persistedState.Player.Name);
                Assert.Equal("Road 2", persistedState.Player.CurrentRoomName);
                Assert.Contains("Grass Badge", persistedState.Player.Badges);
                Assert.Equal(leadAnimal.Id, persistedState.Player.PokemonInventory[0].Id);
                Assert.Equal(7, persistedState.Player.PokemonInventory[0].CurrentHealth);
                Assert.Equal(leadAnimal.BaseHealth, persistedState.Player.PokemonInventory[0].BaseHealth);
                Assert.Equal(leadAnimal.Speed, persistedState.Player.PokemonInventory[0].Speed);
                Assert.Equal(leadAnimal.Moves[0].Name, persistedState.Player.PokemonInventory[0].Moves[0].Name);
                Assert.Equal(roadOneAnimal.Id, persistedState.Rooms.Single(room => room.Name == "Road 1").EncounterPokemon[0].Id);
                Assert.True(persistedState.Trainers.Single(trainer => trainer.Name == "Mrs. Mcmann").Defeated);
            }
            finally
            {
                DeleteTemporaryDatabase(databasePath);
            }
        }

        // Checks that deleting a save removes the stored slot and reports false on a second delete attempt.
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

        // Checks that save-state serialization and deserialization preserve the key persisted fields.
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
                            CurrentHealth = 30,
                            BaseHealth = 40,
                            Speed = 7,
                            Moves = new List<MoveSaveState>
                            {
                                new() { Name = "TACKLE", Type = MoveType.Neutral, Power = 5, Effect = MoveEffect.Damage }
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
                            new() { Id = 3, Name = "PIDGEY", CurrentHealth = 12 }
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
                            new() { Id = 4, Name = "CHARMANDER", CurrentHealth = 20 }
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

        // Checks that capturing and reapplying save data restores the main runtime state of the game.
        [Fact]
        public void Mapper_CaptureAndApply_RestoresRuntimeState()
        {
            GameState gameState = CreateGameState("Red");
            Animal playerAnimal = gameState.MainPlayer.AnimalInventory[0];
            playerAnimal.CurrentHealth = 7;

            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
            gameState.MainPlayer.AddBadge("Grass Badge");
            gameState.GymLeader1.Defeated = true;
            List<Animal> roadAnimals = GameData.CreateAnimals();
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(new[] { GameData.FindAnimal(roadAnimals, AnimalElement.Nature, "Horse") });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestoreName("Changed");
            gameState.MainPlayer.MoveTo(gameState.GameMap.StartRoom);
            gameState.MainPlayer.RestoreBadges(Array.Empty<string>());
            List<Animal> replacementAnimals = GameData.CreateAnimals();
            gameState.MainPlayer.RestoreAnimalInventory(new[] { GameData.FindAnimal(replacementAnimals, AnimalElement.Nature, "Stallion") });
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(Array.Empty<Animal>());
            gameState.GymLeader1.Defeated = false;

            GameStateMapper.Apply(gameState, captured);

            Assert.Equal("Red", gameState.MainPlayer.Name);
            Assert.Equal("Road 2", gameState.MainPlayer.CurrentRoom.Name);
            Assert.Equal("Grass Badge", gameState.MainPlayer.Badges[0]);
            Assert.Equal(playerAnimal.Id, gameState.MainPlayer.AnimalInventory[0].Id);
            Assert.Equal(7, gameState.MainPlayer.AnimalInventory[0].CurrentHealth);
            Assert.Single(gameState.GameMap.GetRoom("Road 1").EncounterAnimals);
            Assert.True(gameState.GymLeader1.Defeated);
        }

        // Checks that saving through the service sends the current animal-based runtime data to the repository JSON payload.
        [Fact]
        public void Service_Save_SendsCurrentAnimalRuntimeDataToRepository()
        {
            GameState gameState = CreateGameState("Red");
            Animal changedAnimal = new(
                id: 1,
                name: "UMBREON",
                element: AnimalElement.Nature,
                speed: 21,
                baseHealth: 101,
                currentHealth: 88,
                level: 7,
                moves: new[]
                {
                    new Move("CUSTOMBITE", MoveType.Nuclear, 13),
                    new Move("CUSTOMSPARK", MoveType.Thunder, 11, MoveEffect.Healing)
                });
            gameState.MainPlayer.RestoreAnimalInventory(new[] { changedAnimal });
            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
            FakeGameSaveRepository repository = new();
            GameSaveService saveService = new(repository);

            SaveCommandResult result = saveService.Save(gameState);

            string? savedJson = repository.SaveJsonValue;
            Assert.NotNull(savedJson);
            GameSaveState savedState = GameSaveSerializer.Deserialize(savedJson);
            Assert.True(result.Succeeded);
            Assert.Equal("Game saved.", result.Message);
            Assert.Equal("Road 2", savedState.Player.CurrentRoomName);
            Assert.Equal(1, savedState.Player.PokemonInventory[0].Id);
            Assert.Equal("UMBREON", savedState.Player.PokemonInventory[0].Name);
            Assert.Equal(21, savedState.Player.PokemonInventory[0].Speed);
            Assert.Equal(101, savedState.Player.PokemonInventory[0].BaseHealth);
            Assert.Equal(88, savedState.Player.PokemonInventory[0].CurrentHealth);
            Assert.Equal(7, savedState.Player.PokemonInventory[0].Level);
            Assert.Equal("CUSTOMBITE", savedState.Player.PokemonInventory[0].Moves[0].Name);
            Assert.Equal(MoveType.Nuclear, savedState.Player.PokemonInventory[0].Moves[0].Type);
            Assert.Equal(13, savedState.Player.PokemonInventory[0].Moves[0].Power);
            Assert.Equal("CUSTOMSPARK", savedState.Player.PokemonInventory[0].Moves[1].Name);
            Assert.Equal(MoveEffect.Healing, savedState.Player.PokemonInventory[0].Moves[1].Effect);
        }

        // Checks that capturing and reapplying save data restores modified runtime stats and custom moves.
        [Fact]
        public void Mapper_CaptureAndApply_RestoresAnimalRuntimeStatsAndMoves()
        {
            GameState gameState = CreateGameState("Red");
            Animal changedAnimal = new(
                id: 1,
                name: "UMBREON",
                element: AnimalElement.Nature,
                speed: 21,
                baseHealth: 101,
                currentHealth: 88,
                level: 7,
                moves: new[]
                {
                    new Move("CUSTOMBITE", MoveType.Nuclear, 13),
                    new Move("CUSTOMSPARK", MoveType.Thunder, 11, MoveEffect.Healing)
                });
            gameState.MainPlayer.RestoreAnimalInventory(new[] { changedAnimal });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            List<Animal> replacementAnimals = GameData.CreateAnimals();
            gameState.MainPlayer.RestoreAnimalInventory(new[] { GameData.FindAnimal(replacementAnimals, AnimalElement.Nature, "Dog") });
            GameStateMapper.Apply(gameState, captured);

            Animal restoredAnimal = gameState.MainPlayer.AnimalInventory[0];
            Assert.Equal(2, captured.Version);
            Assert.Equal(1, restoredAnimal.Id);
            Assert.Equal("UMBREON", restoredAnimal.Name);
            Assert.Equal(21, restoredAnimal.Speed);
            Assert.Equal(101, restoredAnimal.BaseHealth);
            Assert.Equal(88, restoredAnimal.CurrentHealth);
            Assert.Equal(7, restoredAnimal.Level);
            Assert.Equal("CUSTOMBITE", restoredAnimal.Moves[0].Name);
            Assert.Equal(MoveType.Nuclear, restoredAnimal.Moves[0].Type);
            Assert.Equal(13, restoredAnimal.Moves[0].Power);
            Assert.Equal("CUSTOMSPARK", restoredAnimal.Moves[1].Name);
            Assert.Equal(MoveEffect.Healing, restoredAnimal.Moves[1].Effect);
        }

        // Checks that version 1 save data falls back to factory defaults for fields that were not present yet.
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
                        new() { Id = 1, Name = "UMBREON", CurrentHealth = 12 }
                    }
                }
            };

            GameStateMapper.Apply(gameState, oldSaveState);

            Animal template = GameData.CreateAnimals().Single(animal => animal.Id == 1);
            Animal restoredAnimal = gameState.MainPlayer.AnimalInventory[0];
            Assert.Equal("Blue", gameState.MainPlayer.Name);
            Assert.Equal(12, restoredAnimal.CurrentHealth);
            Assert.Equal(template.Level, restoredAnimal.Level);
            Assert.Equal(template.BaseHealth, restoredAnimal.BaseHealth);
            Assert.Equal(template.Speed, restoredAnimal.Speed);
            Assert.Equal(template.Moves.Select(move => move.Name), restoredAnimal.Moves.Select(move => move.Name));
        }

        // Checks that applying save data with out-of-range currentHealth values throws a validation error.
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
                        new() { Id = 1, Name = "UMBREON", CurrentHealth = savedHealth, BaseHealth = 75 }
                    }
                }
            };

            Assert.Throws<InvalidOperationException>(() => GameStateMapper.Apply(gameState, saveState));
        }

        // Checks that the load service reports failure when persisted creature currentHealth exceeds the allowed range.
        [Fact]
        public void Service_LoadPokemonSaveWithInvalidHealth_ReturnsFailure()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = GameStateMapper.Capture(gameState);
            saveState.Player.PokemonInventory[0].CurrentHealth = saveState.Player.PokemonInventory[0].BaseHealth + 1;
            string saveJson = GameSaveSerializer.Serialize(saveState);
            GameSaveService saveService = new(new FakeGameSaveRepository(saveJson));

            SaveCommandResult result = saveService.Load(gameState);

            Assert.False(result.Succeeded);
            Assert.Equal("Save data could not be loaded.", result.Message);
        }

        // Checks that startup loads an existing save without showing the new-game-only menu layout.
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

        // Checks that deleting an existing save returns startup to the new-game-only menu flow.
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

        // Checks that invalid delete confirmation input re-displays the confirmation prompt instead of proceeding.
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

        // Checks that invalid startup input without a save re-displays only the new game option.
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
            List<Animal> mainAnimals = GameData.CreateAnimals();
            List<Animal> gymAnimals = GameData.CreateAnimals();
            Arcadia_v2.Map.Map gameMap = new();

            Player mainPlayer = new(playerName, gameMap.StartRoom);
            mainPlayer.AddAnimal(GameData.FindAnimal(mainAnimals, AnimalElement.Nature, "Lion"));
            mainPlayer.AddAnimal(GameData.FindAnimal(mainAnimals, AnimalElement.Nature, "Dog"));

            CompPlayer gymLeader1 = new("Mrs. Mcmann", gameMap.GymLeader1Room);
            gymLeader1.SetBattleTeam(new[]
            {
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Wolf"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Serpent")
            });
            gymLeader1.AddBadge("Grass Badge");

            CompPlayer gymLeader2 = new("Minofo", gameMap.GymLeader2Room);
            gymLeader2.SetBattleTeam(new[]
            {
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Stallion"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Eagle")
            });
            gymLeader2.AddBadge("Water Badge");

            CompPlayer gymLeader3 = new("Golden", gameMap.GymLeader3Room);
            gymLeader3.SetBattleTeam(new[]
            {
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Ant"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Mystic, "Cat")
            });
            gymLeader3.AddBadge("Rock Badge");

            CompPlayer gymLeader4 = new("Wiggins", gameMap.GymLeader4Room);
            gymLeader4.SetBattleTeam(new[]
            {
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Bear"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Mystic, "Lion")
            });
            gymLeader4.AddBadge("Dragon Badge");

            CompPlayer arcadiaChampion = new("Adam", gameMap.ChampionRoom);
            arcadiaChampion.SetBattleTeam(new[]
            {
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Horse"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Bird"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Nature, "Turtle"),
                GameData.FindAnimal(gymAnimals, AnimalElement.Mystic, "Dog")
            });
            arcadiaChampion.AddBadge("Champion Badge");

            return new GameState(
                gameMap,
                mainAnimals,
                gymAnimals,
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
