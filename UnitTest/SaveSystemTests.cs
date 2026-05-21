using Arcadia_v2;
using Arcadia_v2.Creatures;
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
                leadAnimal.Health = 7;
                gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
                gameState.MainPlayer.AddBadge("Grass Badge");
                gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(new[] { GameData.CreateAnimals()[4] });
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
                Assert.Equal(leadAnimal.Id, persistedState.Player.AnimalInventory[0].Id);
                Assert.Equal(7, persistedState.Player.AnimalInventory[0].Health);
                Assert.Equal(leadAnimal.BaseHealth, persistedState.Player.AnimalInventory[0].BaseHealth);
                Assert.Equal(leadAnimal.Speed, persistedState.Player.AnimalInventory[0].Speed);
                Assert.Equal(leadAnimal.Moves[0].Name, persistedState.Player.AnimalInventory[0].Moves[0].Name);
                Assert.Equal(leadAnimal.Moves[0].Effect, persistedState.Player.AnimalInventory[0].Moves[0].Effect);
                Assert.Equal(4, persistedState.Rooms.Single(room => room.Name == "Road 1").EncounterAnimals[0].Id);
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
                    AnimalInventory = new List<AnimalSaveState>
                    {
                        new()
                        {
                            Id = 1,
                            Name = "N_CUB",
                            Level = 5,
                            Health = 30,
                            BaseHealth = 40,
                            Speed = 7,
                            Moves = new List<MoveSaveState>
                            {
                                new() { Name = "TACKLE", Type = ElementType.Base, Power = 5, Effect = MoveEffect.Damage }
                            }
                        }
                    }
                },
                Rooms = new List<RoomSaveState>
                {
                    new()
                    {
                        Name = "Road 1",
                        EncounterAnimals = new List<AnimalSaveState>
                        {
                            new() { Id = 9, Name = "N_BIRD", Health = 12 }
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
                        BattleTeamTemplate = new List<AnimalSaveState>
                        {
                            new() { Id = 3, Name = "N_DOG", Health = 20 }
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
            Assert.Equal(5, restored.Player.AnimalInventory[0].Level);
            Assert.Equal(40, restored.Player.AnimalInventory[0].BaseHealth);
            Assert.Equal(7, restored.Player.AnimalInventory[0].Speed);
            Assert.Equal("TACKLE", restored.Player.AnimalInventory[0].Moves[0].Name);
            Assert.Equal(MoveEffect.Damage, restored.Player.AnimalInventory[0].Moves[0].Effect);
        }

        // Checks that capturing and reapplying save data restores the main runtime state of the game.
        [Fact]
        public void Mapper_CaptureAndApply_RestoresRuntimeState()
        {
            GameState gameState = CreateGameState("Red");
            Animal playerAnimal = gameState.MainPlayer.AnimalInventory[0];
            playerAnimal.Health = 7;

            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 2"));
            gameState.MainPlayer.AddBadge("Grass Badge");
            gameState.GymLeader1.Defeated = true;
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(new[] { GameData.CreateAnimals()[4] });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestoreName("Changed");
            gameState.MainPlayer.MoveTo(gameState.GameMap.StartRoom);
            gameState.MainPlayer.RestoreBadges(Array.Empty<string>());
            gameState.MainPlayer.RestoreAnimalInventory(new[] { GameData.CreateAnimals()[5] });
            gameState.GameMap.GetRoom("Road 1").RestoreEncounterAnimals(Array.Empty<Animal>());
            gameState.GymLeader1.Defeated = false;

            GameStateMapper.Apply(gameState, captured);

            Assert.Equal("Red", gameState.MainPlayer.Name);
            Assert.Equal("Road 2", gameState.MainPlayer.CurrentRoom.Name);
            Assert.Equal("Grass Badge", gameState.MainPlayer.Badges[0]);
            Assert.Equal(playerAnimal.Id, gameState.MainPlayer.AnimalInventory[0].Id);
            Assert.Equal(7, gameState.MainPlayer.AnimalInventory[0].Health);
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
                name: "N_CAT",
                element: AnimalElement.Nature,
                speed: 21,
                baseHealth: 101,
                health: 88,
                level: 7,
                moves: new[]
                {
                    new Move("CUSTOMBITE", ElementType.Nature, 13),
                    new Move("CUSTOMSPARK", ElementType.Thunder, 11)
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
            Assert.Equal(1, savedState.Player.AnimalInventory[0].Id);
            Assert.Equal("N_CAT", savedState.Player.AnimalInventory[0].Name);
            Assert.Equal(21, savedState.Player.AnimalInventory[0].Speed);
            Assert.Equal(101, savedState.Player.AnimalInventory[0].BaseHealth);
            Assert.Equal(88, savedState.Player.AnimalInventory[0].Health);
            Assert.Equal(7, savedState.Player.AnimalInventory[0].Level);
            Assert.Equal("CUSTOMBITE", savedState.Player.AnimalInventory[0].Moves[0].Name);
            Assert.Equal(ElementType.Nature, savedState.Player.AnimalInventory[0].Moves[0].Type);
            Assert.Equal(13, savedState.Player.AnimalInventory[0].Moves[0].Power);
            Assert.Equal(MoveEffect.Damage, savedState.Player.AnimalInventory[0].Moves[0].Effect);
            Assert.Equal("CUSTOMSPARK", savedState.Player.AnimalInventory[0].Moves[1].Name);
        }

        // Checks that capturing and reapplying save data restores modified runtime stats and custom moves.
        [Fact]
        public void Mapper_CaptureAndApply_RestoresAnimalRuntimeStatsAndMoves()
        {
            GameState gameState = CreateGameState("Red");
            Animal changedAnimal = new(
                id: 1,
                name: "N_CAT",
                element: AnimalElement.Nature,
                speed: 21,
                baseHealth: 101,
                health: 88,
                level: 7,
                moves: new[]
                {
                    new Move("CUSTOMHEAL", ElementType.Nuclear, 13, MoveEffect.Heal),
                    new Move("CUSTOMSPARK", ElementType.Thunder, 11)
                });
            gameState.MainPlayer.RestoreAnimalInventory(new[] { changedAnimal });

            GameSaveState captured = GameStateMapper.Capture(gameState);

            gameState.MainPlayer.RestoreAnimalInventory(new[] { GameData.CreateAnimals()[2] });
            GameStateMapper.Apply(gameState, captured);

            Animal restoredAnimal = gameState.MainPlayer.AnimalInventory[0];
            Assert.Equal(2, captured.Version);
            Assert.Equal(1, restoredAnimal.Id);
            Assert.Equal("N_CAT", restoredAnimal.Name);
            Assert.Equal(21, restoredAnimal.Speed);
            Assert.Equal(101, restoredAnimal.BaseHealth);
            Assert.Equal(88, restoredAnimal.Health);
            Assert.Equal(7, restoredAnimal.Level);
            Assert.Equal("CUSTOMHEAL", restoredAnimal.Moves[0].Name);
            Assert.Equal(ElementType.Nuclear, restoredAnimal.Moves[0].Type);
            Assert.Equal(13, restoredAnimal.Moves[0].Power);
            Assert.Equal(MoveEffect.Heal, restoredAnimal.Moves[0].Effect);
            Assert.Equal("CUSTOMSPARK", restoredAnimal.Moves[1].Name);
        }

        // Checks that version 1 save data falls back to factory defaults for fields that were not present yet.
        [Fact]
        public void Mapper_ApplyVersion1AnimalSave_FallsBackToFactoryDefaultsForMissingRuntimeFields()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState oldSaveState = new()
            {
                Version = 1,
                Player = new PlayerSaveState
                {
                    Name = "Blue",
                    CurrentRoomName = gameState.GameMap.StartRoom.Name,
                    AnimalInventory = new List<AnimalSaveState>
                    {
                        new() { Id = 1, Name = "N_CAT", Health = 12 }
                    }
                }
            };

            GameStateMapper.Apply(gameState, oldSaveState);

            Animal template = GameData.CreateAnimals().Single(animal => animal.Id == 1);
            Animal restoredAnimal = gameState.MainPlayer.AnimalInventory[0];
            Assert.Equal("Blue", gameState.MainPlayer.Name);
            Assert.Equal(12, restoredAnimal.Health);
            Assert.Equal(template.Level, restoredAnimal.Level);
            Assert.Equal(template.BaseHealth, restoredAnimal.BaseHealth);
            Assert.Equal(template.Speed, restoredAnimal.Speed);
            Assert.Equal(template.Moves.Select(move => move.Name), restoredAnimal.Moves.Select(move => move.Name));
        }

        // Checks that save data written before move effects existed can still restore Bloom as a healing move.
        [Fact]
        public void Mapper_ApplySaveWithoutMoveEffect_UsesFactoryMoveEffect()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = new()
            {
                Player = new PlayerSaveState
                {
                    Name = "Blue",
                    CurrentRoomName = gameState.GameMap.StartRoom.Name,
                    AnimalInventory = new List<AnimalSaveState>
                    {
                        new()
                        {
                            Id = 2,
                            Name = "N_LION",
                            Health = 40,
                            BaseHealth = 75,
                            Speed = 7,
                            Level = 1,
                            Moves = new List<MoveSaveState>
                            {
                                new() { Name = "Bloom", Type = ElementType.Nature, Power = 10 }
                            }
                        }
                    }
                }
            };

            GameStateMapper.Apply(gameState, saveState);

            Move restoredMove = gameState.MainPlayer.AnimalInventory[0].Moves[0];
            Assert.Equal("Bloom", restoredMove.Name);
            Assert.Equal(MoveEffect.Heal, restoredMove.Effect);
        }

        // Checks that applying save data with out-of-range health values throws a validation error.
        [Theory]
        [InlineData(-1)]
        [InlineData(76)]
        public void Mapper_ApplyAnimalSaveWithInvalidHealth_ThrowsInvalidOperationException(int savedHealth)
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = new()
            {
                Player = new PlayerSaveState
                {
                    Name = "Blue",
                    CurrentRoomName = gameState.GameMap.StartRoom.Name,
                    AnimalInventory = new List<AnimalSaveState>
                    {
                        new() { Id = 1, Name = "N_CAT", Health = savedHealth, BaseHealth = 75 }
                    }
                }
            };

            Assert.Throws<InvalidOperationException>(() => GameStateMapper.Apply(gameState, saveState));
        }

        // Checks that the load service reports failure when persisted creature health exceeds the allowed range.
        [Fact]
        public void Service_LoadAnimalSaveWithInvalidHealth_ReturnsFailure()
        {
            GameState gameState = CreateGameState("Red");
            GameSaveState saveState = GameStateMapper.Capture(gameState);
            saveState.Player.AnimalInventory[0].Health = saveState.Player.AnimalInventory[0].BaseHealth + 1;
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
            mainPlayer.AddAnimal(mainAnimals[1]);
            mainPlayer.AddAnimal(mainAnimals[2]);

            CompPlayer gymLeader1 = new("Mrs. Mcmann", gameMap.GymLeader1Room);
            gymLeader1.SetBattleTeam(new[] { gymAnimals[3], gymAnimals[14] });
            gymLeader1.AddBadge("Grass Badge");

            CompPlayer gymLeader2 = new("Minofo", gameMap.GymLeader2Room);
            gymLeader2.SetBattleTeam(new[] { gymAnimals[5], gymAnimals[9] });
            gymLeader2.AddBadge("Water Badge");

            CompPlayer gymLeader3 = new("Golden", gameMap.GymLeader3Room);
            gymLeader3.SetBattleTeam(new[] { gymAnimals[10], gymAnimals[16] });
            gymLeader3.AddBadge("Rock Badge");

            CompPlayer gymLeader4 = new("Wiggins", gameMap.GymLeader4Room);
            gymLeader4.SetBattleTeam(new[] { gymAnimals[13], gymAnimals[17] });
            gymLeader4.AddBadge("Dragon Badge");

            CompPlayer arcadiaChampion = new("Adam", gameMap.ChampionRoom);
            arcadiaChampion.SetBattleTeam(new[] { gymAnimals[4], gymAnimals[8], gymAnimals[6], gymAnimals[18] });
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

