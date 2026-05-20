#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2.Saves
{
    public sealed class GameSaveState
    {
        public int Version { get; set; } = 2;
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
        public int Level { get; set; }
        public int CurrentHealth { get; set; }
        public int BaseHealth { get; set; }
        public int Speed { get; set; }
        public List<MoveSaveState> Moves { get; set; } = new();
    }

    public sealed class MoveSaveState
    {
        public string Name { get; set; } = string.Empty;
        public MoveType Type { get; set; }
        public int Power { get; set; }
        public MoveEffect Effect { get; set; }
    }
}
