#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia_v2.Saves
{
    public static class GameStateMapper
    {
        public static GameSaveState Capture(GameState gameState)
        {
            ArgumentNullException.ThrowIfNull(gameState);

            return new GameSaveState
            {
                Version = 2,
                Player = CapturePlayer(gameState.MainPlayer),
                Rooms = gameState.GameMap.Rooms.Values
                    .Select(room => new RoomSaveState
                    {
                        Name = room.Name,
                        EncounterPokemon = CaptureAnimalList(room.EncounterAnimals)
                    })
                    .ToList(),
                Trainers = GetTrainers(gameState)
                    .Select(CaptureTrainer)
                    .ToList()
            };
        }

        public static void Apply(GameState gameState, GameSaveState state)
        {
            ArgumentNullException.ThrowIfNull(gameState);
            ArgumentNullException.ThrowIfNull(state);

            Dictionary<int, Animal> animalsById = GameData.CreateAnimals()
                .ToDictionary(animal => animal.Id);

            ApplyPlayer(gameState, state.Player, animalsById);
            ApplyRooms(gameState, state.Rooms, animalsById);
            ApplyTrainers(gameState, state.Trainers, animalsById);
        }

        private static PlayerSaveState CapturePlayer(Player player)
        {
            return new PlayerSaveState
            {
                Name = player.Name,
                CurrentRoomName = player.CurrentRoom.Name,
                Badges = player.Badges.ToList(),
                PokemonInventory = CaptureAnimalList(player.AnimalInventory)
            };
        }

        private static TrainerSaveState CaptureTrainer(CompPlayer trainer)
        {
            return new TrainerSaveState
            {
                Name = trainer.Name,
                CurrentRoomName = trainer.CurrentRoom.Name,
                Defeated = trainer.Defeated,
                Badges = trainer.Badges.ToList(),
                BattleTeamTemplate = CaptureAnimalList(trainer.BattleTeamTemplate)
            };
        }

        private static List<PokemonSaveState> CaptureAnimalList(IReadOnlyList<Animal> animalList)
        {
            return animalList
                .Select(animal => new PokemonSaveState
                {
                    Id = animal.Id,
                    Name = animal.Name,
                    Level = animal.Level,
                    Health = animal.Health,
                    BaseHealth = animal.BaseHealth,
                    Speed = animal.Speed,
                    Moves = animal.Moves
                        .Select(move => new MoveSaveState
                        {
                            Name = move.Name,
                            Type = move.Type,
                            Power = move.Power
                        })
                        .ToList()
                })
                .ToList();
        }

        private static void ApplyPlayer(
            GameState gameState,
            PlayerSaveState playerState,
            IReadOnlyDictionary<int, Animal> animalsById)
        {
            gameState.MainPlayer.RestoreName(playerState.Name);
            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom(playerState.CurrentRoomName));
            gameState.MainPlayer.RestoreBadges(playerState.Badges);
            gameState.MainPlayer.RestoreAnimalInventory(CreateAnimals(playerState.PokemonInventory, animalsById));
        }

        private static void ApplyRooms(
            GameState gameState,
            IReadOnlyList<RoomSaveState> rooms,
            IReadOnlyDictionary<int, Animal> animalsById)
        {
            foreach (RoomSaveState roomState in rooms)
            {
                gameState.GameMap.GetRoom(roomState.Name).RestoreEncounterAnimals(CreateAnimals(roomState.EncounterPokemon, animalsById));
            }
        }

        private static void ApplyTrainers(
            GameState gameState,
            IReadOnlyList<TrainerSaveState> trainers,
            IReadOnlyDictionary<int, Animal> animalsById)
        {
            Dictionary<string, CompPlayer> trainersByName = GetTrainers(gameState)
                .ToDictionary(trainer => trainer.Name, StringComparer.Ordinal);

            foreach (TrainerSaveState trainerState in trainers)
            {
                if (!trainersByName.TryGetValue(trainerState.Name, out CompPlayer? trainer))
                {
                    throw new InvalidOperationException($"Unknown trainer in save data: {trainerState.Name}");
                }

                trainer.RestoreName(trainerState.Name);
                trainer.MoveTo(gameState.GameMap.GetRoom(trainerState.CurrentRoomName));
                trainer.Defeated = trainerState.Defeated;
                trainer.RestoreBadges(trainerState.Badges);
                trainer.SetBattleTeam(CreateAnimals(trainerState.BattleTeamTemplate, animalsById));
            }
        }

        private static List<Animal> CreateAnimals(
            IEnumerable<PokemonSaveState> savedPokemon,
            IReadOnlyDictionary<int, Animal> animalsById)
        {
            List<Animal> restoredAnimals = new();

            foreach (PokemonSaveState pokemonState in savedPokemon)
            {
                if (!animalsById.TryGetValue(pokemonState.Id, out Animal? template))
                {
                    throw new InvalidOperationException($"Unknown Pokemon id in save data: {pokemonState.Id}");
                }

                Animal animal = CreateAnimalFromSaveState(pokemonState, template);
                restoredAnimals.Add(animal);
            }

            return restoredAnimals;
        }

        private static Animal CreateAnimalFromSaveState(PokemonSaveState pokemonState, Animal template)
        {
            string name = string.IsNullOrWhiteSpace(pokemonState.Name)
                ? template.Name
                : pokemonState.Name;
            int level = pokemonState.Level > 0
                ? pokemonState.Level
                : template.Level;
            int baseHealth = pokemonState.BaseHealth > 0
                ? pokemonState.BaseHealth
                : template.BaseHealth;
            int speed = pokemonState.Speed > 0
                ? pokemonState.Speed
                : template.Speed;
            int health = ValidateHealth(pokemonState.Health, baseHealth, name);
            IEnumerable<Move> moves = pokemonState.Moves is { Count: > 0 }
                ? pokemonState.Moves.Select(moveState => new Move(moveState.Name, moveState.Type, moveState.Power))
                : template.Moves;

            return new Animal(
                id: pokemonState.Id,
                name: name,
                element: template.Element,
                speed: speed,
                baseHealth: baseHealth,
                health: health,
                level: level,
                moves: moves);
        }

        private static int ValidateHealth(int health, int baseHealth, string pokemonName)
        {
            if (health < 0 || health > baseHealth)
            {
                throw new InvalidOperationException(
                    $"Invalid health for {pokemonName} in save data: {health}. Expected a value from 0 to {baseHealth}.");
            }

            return health;
        }

        private static IReadOnlyList<CompPlayer> GetTrainers(GameState gameState)
        {
            return new[]
            {
                gameState.GymLeader1,
                gameState.GymLeader2,
                gameState.GymLeader3,
                gameState.GymLeader4,
                gameState.ArcadiaChampion
            };
        }
    }
}
