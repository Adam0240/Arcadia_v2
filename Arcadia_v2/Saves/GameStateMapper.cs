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
                Version = 1,
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

            ApplyPlayer(gameState, state.Player);
            ApplyRooms(gameState, state.Rooms);
            ApplyTrainers(gameState, state.Trainers);
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
                    Health = pokemon.Health
                })
                .ToList();
        }

        private static void ApplyPlayer(GameState gameState, PlayerSaveState playerState)
        {
            gameState.MainPlayer.RestoreName(playerState.Name);
            gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom(playerState.CurrentRoomName));
            gameState.MainPlayer.RestoreBadges(playerState.Badges);
            gameState.MainPlayer.RestorePokemonInventory(CreatePokemon(playerState.PokemonInventory));
        }

        private static void ApplyRooms(GameState gameState, IReadOnlyList<RoomSaveState> rooms)
        {
            foreach (RoomSaveState roomState in rooms)
            {
                gameState.GameMap.GetRoom(roomState.Name).RestoreEncounterPokemon(CreatePokemon(roomState.EncounterPokemon));
            }
        }

        private static void ApplyTrainers(GameState gameState, IReadOnlyList<TrainerSaveState> trainers)
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
                trainer.SetBattleTeam(CreatePokemon(trainerState.BattleTeamTemplate));
            }
        }

        private static List<Pokemon> CreatePokemon(IEnumerable<PokemonSaveState> savedPokemon)
        {
            Dictionary<int, Pokemon> pokemonById = GameData.CreatePokemon()
                .ToDictionary(pokemon => pokemon.Id);

            List<Pokemon> restoredPokemon = new();

            foreach (PokemonSaveState pokemonState in savedPokemon)
            {
                if (!pokemonById.TryGetValue(pokemonState.Id, out Pokemon? template))
                {
                    throw new InvalidOperationException($"Unknown Pokemon id in save data: {pokemonState.Id}");
                }

                Pokemon pokemon = template.Clone();
                pokemon.Health = pokemonState.Health;
                restoredPokemon.Add(pokemon);
            }

            return restoredPokemon;
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
