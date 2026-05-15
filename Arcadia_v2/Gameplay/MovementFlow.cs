#nullable enable

using System;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Handles player movement, including direction validation, room changes, and badge/champion gates.
    public static class MovementFlow
    {
        public static void HandleMovement(Player mainPlayer, CompPlayer arcadiaChampion, int choice, string direction)
        {
            Room? destination = GetDestinationRoom(mainPlayer, choice);

            if (choice == 5)
            {
                Console.WriteLine("Goodbye!");
                return;
            }

            if (choice == 6)
            {
                Console.WriteLine($"\nSorry, {direction} is invalid.");
                return;
            }

            if (destination == null)
            {
                Console.WriteLine("\nError, you cant go this way.\n");
                return;
            }

            if (!CanEnterRoom(mainPlayer, arcadiaChampion, destination))
            {
                return;
            }

            MovePlayerToRoom(mainPlayer, destination);
        }

        // Returns the room in the selected direction, or null if that direction has no room.
        private static Room? GetDestinationRoom(Player mainPlayer, int choice)
        {
            return choice switch
            {
                1 => mainPlayer.CurrentRoom.North,
                2 => mainPlayer.CurrentRoom.East,
                3 => mainPlayer.CurrentRoom.South,
                4 => mainPlayer.CurrentRoom.West,
                _ => null
            };
        }

        // Checks whether the player is allowed to enter the destination room.
        private static bool CanEnterRoom(Player mainPlayer, CompPlayer arcadiaChampion, Room destination)
        {
            if (IsBadgeLocked(mainPlayer, destination))
            {
                return false;
            }

            if (destination.RequiresChampionDefeatToEnter && !arcadiaChampion.Defeated)
            {
                Console.WriteLine("Your not ready to go here yet. You must become the Champion of the region to proceed.");
                return false;
            }

            return true;
        }

        // Checks whether a destination room is locked behind a badge requirement.
        private static bool IsBadgeLocked(Player mainPlayer, Room destination)
        {
            if (destination.RequiredBadgesToEnter <= 0)
            {
                return false;
            }

            if (mainPlayer.Badges.Count >= destination.RequiredBadgesToEnter)
            {
                return false;
            }

            Console.WriteLine($"You need to obtain {destination.RequiredBadgesToEnter} badge(s) before this way unlocks!");
            Console.WriteLine($"You currently have {mainPlayer.Badges.Count} total badges.");

            return true;
        }

        // Moves the player and displays the new room.
        private static void MovePlayerToRoom(Player mainPlayer, Room destination)
        {
            mainPlayer.MoveTo(destination);
            Console.WriteLine();
            RoomDisplay.Print(mainPlayer.CurrentRoom);
        }
    }
}
