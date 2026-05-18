#nullable enable

using System.Linq;
using Arcadia_v2.Map;
using Arcadia_v2.Commands;

namespace Arcadia_v2
{
    // Handles player movement, including direction validation, room changes, and badge/champion gates.
    public static class MovementFlow
    {
        public static void HandleMovement(IGameIO io, GameState gameState, DirectionCommandType directionCommand, string direction)
        {
            Player mainPlayer = gameState.MainPlayer;
            CompPlayer arcadiaChampion = gameState.ArcadiaChampion;
            Room? destination = GetDestinationRoom(mainPlayer, directionCommand);

            if (directionCommand == DirectionCommandType.Quit)
            {
                io.WriteLine("Goodbye!");
                return;
            }

            if (directionCommand == DirectionCommandType.Invalid)
            {
                io.WriteLine($"\nSorry, {direction} is invalid.");
                return;
            }

            if (destination == null)
            {
                io.WriteLine("\nError, you cant go this way.\n");
                return;
            }

            if (!CanEnterRoom(io, gameState.GameMap, mainPlayer, arcadiaChampion, mainPlayer.CurrentRoom, destination))
            {
                return;
            }

            MovePlayerToRoom(io, mainPlayer, destination);
        }

        // Returns the room in the selected direction, or null if that direction has no room.
        private static Room? GetDestinationRoom(Player mainPlayer, DirectionCommandType directionCommand)
        {
            return directionCommand switch
            {
                DirectionCommandType.North => mainPlayer.CurrentRoom.North,
                DirectionCommandType.East => mainPlayer.CurrentRoom.East,
                DirectionCommandType.South => mainPlayer.CurrentRoom.South,
                DirectionCommandType.West => mainPlayer.CurrentRoom.West,
                _ => null
            };
        }

        // Checks whether the player is allowed to enter the destination room.
        private static bool CanEnterRoom(
            IGameIO io,
            Map.Map gameMap,
            Player mainPlayer,
            CompPlayer arcadiaChampion,
            Room currentRoom,
            Room destination)
        {
            MovementRequirement requirement = gameMap.GetMovementRequirement(currentRoom, destination);

            if (IsBadgeLocked(io, mainPlayer, requirement))
            {
                return false;
            }

            if (IsAnimalElementLocked(io, mainPlayer, requirement))
            {
                return false;
            }

            if (requirement.RequiresChampionDefeat && !arcadiaChampion.Defeated)
            {
                io.WriteLine("Your not ready to go here yet. You must become the Champion of the region to proceed.");
                return false;
            }

            return true;
        }

        // Checks whether a destination room is locked behind a badge requirement.
        private static bool IsBadgeLocked(IGameIO io, Player mainPlayer, MovementRequirement requirement)
        {
            int requiredBadges = requirement.RequiredBadges;

            if (requiredBadges <= 0)
            {
                return false;
            }

            if (mainPlayer.Badges.Count >= requiredBadges)
            {
                return false;
            }

            io.WriteLine($"You need to obtain {requiredBadges} badge(s) before this way unlocks!");
            io.WriteLine($"You currently have {mainPlayer.Badges.Count} total badges.");

            return true;
        }

        private static bool IsAnimalElementLocked(IGameIO io, Player mainPlayer, MovementRequirement requirement)
        {
            if (requirement.RequiredAnimalElement == null)
            {
                return false;
            }

            if (mainPlayer.AnimalInventory.Any(animal => animal.Element == requirement.RequiredAnimalElement))
            {
                return false;
            }

            io.WriteLine($"You need a {requirement.RequiredAnimalElement} animal on your team before this way unlocks!");
            return true;
        }

        // Moves the player and displays the new room.
        private static void MovePlayerToRoom(IGameIO io, Player mainPlayer, Room destination)
        {
            mainPlayer.MoveTo(destination);
            io.WriteLine();
            RoomDisplay.Print(io, mainPlayer.CurrentRoom);
        }
    }
}
