#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2.Saves
{
    public sealed class GameSaveState
    {
        public int Version { get; set; } = 1;
        public PlayerSaveState Player { get; set; } = new();
        public List<RoomSaveState> Rooms { get; set; } = new();
        public List<TrainerSaveState> Trainers { get; set; } = new();
    }

    public sealed class PlayerSaveState
    {
        public string Name { get; set; } = string.Empty;
        public string CurrentRoomName { get; set; } = string.Empty;
        public List<string> Badges { get; set; } = new();
        public List<PokemonSaveState> PokemonInventory { get; set; } = new();
    }

    public sealed class RoomSaveState
    {
        public string Name { get; set; } = string.Empty;
        public List<PokemonSaveState> EncounterPokemon { get; set; } = new();
    }

    public sealed class TrainerSaveState
    {
        public string Name { get; set; } = string.Empty;
        public string CurrentRoomName { get; set; } = string.Empty;
        public bool Defeated { get; set; }
        public List<string> Badges { get; set; } = new();
        public List<PokemonSaveState> BattleTeamTemplate { get; set; } = new();
    }

    public sealed class PokemonSaveState
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Health { get; set; }
    }
}
