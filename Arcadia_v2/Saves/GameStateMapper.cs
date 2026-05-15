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
                        EncounterPokemon = CapturePokemonList(room.EncounterPokemon)
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

            Dictionary<int, Pokemon> pokemonById = GameData.CreatePokemon()
                .ToDictionary(pokemon => pokemon.Id);

            ApplyPlayer(gameState, state.Player, pokemonById);
            ApplyRooms(gameState, state.Rooms, pokemonById);
            ApplyTrainers(gameState, state.Trainers, pokemonById);
        }

        private static PlayerSaveState CapturePlayer(Player player)
        {
            return new PlayerSaveState
            {
                Name = player.Name,
                CurrentRoomName = player.CurrentRoom.Name,
                Badges = player.Badges.ToList(),
                PokemonInventory = CapturePokemonList(player.PokemonInventory)
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
                BattleTeamTemplate = CapturePokemonList(trainer.BattleTeamTemplate)
            };
        }

        private static List<PokemonSaveState> CapturePokemonList(IReadOnlyList<Pokemon> pokemonList)
        {
            return pokemonList
                .Select(pokemon => new PokemonSaveState
                {
                    Id = pokemon.Id,
                    Name = pokemon.Name,
                    Level = pokemon.Level,
                    Health = pokemon.Health,
                    BaseHealth = pokemon.BaseHealth,
                    Speed = pokemon.Speed,
                    Moves = pokemon.Moves
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
            IReadOnlyDictionary<int, Pokemon> pokemonById)
        {
            gameState.MainPlayer.RestoreName(playerState.Name);
            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom(playerState.CurrentRoomName));
            gameState.MainPlayer.RestoreBadges(playerState.Badges);
            gameState.MainPlayer.RestorePokemonInventory(CreatePokemon(playerState.PokemonInventory, pokemonById));
        }

        private static void ApplyRooms(
            GameState gameState,
            IReadOnlyList<RoomSaveState> rooms,
            IReadOnlyDictionary<int, Pokemon> pokemonById)
        {
            foreach (RoomSaveState roomState in rooms)
            {
                gameState.GameMap.GetRoom(roomState.Name).RestoreEncounterPokemon(CreatePokemon(roomState.EncounterPokemon, pokemonById));
            }
        }

        private static void ApplyTrainers(
            GameState gameState,
            IReadOnlyList<TrainerSaveState> trainers,
            IReadOnlyDictionary<int, Pokemon> pokemonById)
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
                trainer.SetBattleTeam(CreatePokemon(trainerState.BattleTeamTemplate, pokemonById));
            }
        }

        private static List<Pokemon> CreatePokemon(
            IEnumerable<PokemonSaveState> savedPokemon,
            IReadOnlyDictionary<int, Pokemon> pokemonById)
        {
            List<Pokemon> restoredPokemon = new();

            foreach (PokemonSaveState pokemonState in savedPokemon)
            {
                if (!pokemonById.TryGetValue(pokemonState.Id, out Pokemon? template))
                {
                    throw new InvalidOperationException($"Unknown Pokemon id in save data: {pokemonState.Id}");
                }

                Pokemon pokemon = CreatePokemonFromSaveState(pokemonState, template);
                restoredPokemon.Add(pokemon);
            }

            return restoredPokemon;
        }

        private static Pokemon CreatePokemonFromSaveState(PokemonSaveState pokemonState, Pokemon template)
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

            return new Pokemon(
                pokemonState.Id,
                name,
                template.Type,
                speed,
                baseHealth,
                health,
                level,
                moves);
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
