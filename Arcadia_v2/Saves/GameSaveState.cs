#nullable enable

using Arcadia_v2.Creatures;
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
        public List<string> StarFragments { get; set; } = new();
        public List<BondSaveState> Bond { get; set; } = new();
        public List<AnimalSaveState> AnimalInventory { get; set; } = new();
    }

    public sealed class RoomSaveState
    {
        public string Name { get; set; } = string.Empty;
        public List<AnimalSaveState> EncounterAnimals { get; set; } = new();
    }

    public sealed class TrainerSaveState
    {
        public string Name { get; set; } = string.Empty;
        public string CurrentRoomName { get; set; } = string.Empty;
        public bool Defeated { get; set; }
        public List<string> StarFragments { get; set; } = new();
        public List<AnimalSaveState> BattleTeamTemplate { get; set; } = new();
    }

    public sealed class AnimalSaveState
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Health { get; set; }
        public int BaseHealth { get; set; }
        public int Speed { get; set; }
        public List<MoveSaveState> Moves { get; set; } = new();
    }

    public sealed class BondSaveState
    {
        public AnimalElement Element { get; set; }
        public int Percent { get; set; }
    }

    public sealed class MoveSaveState
    {
        public string Name { get; set; } = string.Empty;
        public ElementType Type { get; set; }
        public int Power { get; set; }
        public MoveEffect Effect { get; set; }
    }
}
