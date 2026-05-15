namespace Arcadia_v2.Commands
{
    public readonly record struct MainCommandInput(MainCommandType MainCommand, string Remainder);

    public enum MainCommandType
    {
        Invalid = 0,
        Go = 1,
        Action = 2
    }

    public enum DirectionCommandType
    {
        Invalid = 0,
        North = 1,
        East = 2,
        South = 3,
        West = 4,
        Quit = 5
    }

    public enum ActionCommandType
    {
        Invalid = 0,
        Battle = 1,
        PokeInventory = 2,
        Menu = 3
    }

    public enum MenuCommandType
    {
        Invalid = 0,
        Heal = 1,
        Bag = 2,
        Swap = 3,
        Gym = 4,
        Save = 5
    }

    public static class Parser
    {
        public static string ToUpperCase(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        public static MainCommandInput ParseMainCommandInput(string? input)
        {
            string commandLine = ToUpperCase(input);

            if (string.IsNullOrEmpty(commandLine))
            {
                return new MainCommandInput(MainCommandType.Invalid, string.Empty);
            }

            string[] parts = commandLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string mainToken = parts[0];
            string remainder = parts.Length > 1 ? parts[1] : string.Empty;

            return new MainCommandInput(ParseMainCommand(mainToken), remainder);
        }

        public static MainCommandType ParseMainCommand(string? input)
        {
            string command = ToUpperCase(input);

            if (command == "GO" || command == "G")
            {
                return MainCommandType.Go;
            }

            if (command == "ACTION" || command == "A")
            {
                return MainCommandType.Action;
            }

            return MainCommandType.Invalid;
        }

        public static DirectionCommandType ParseDirectionCommand(string? input)
        {
            string direction = ToUpperCase(input);

            if (direction == "NORTH")
            {
                return DirectionCommandType.North;
            }

            if (direction == "EAST")
            {
                return DirectionCommandType.East;
            }

            if (direction == "SOUTH")
            {
                return DirectionCommandType.South;
            }

            if (direction == "WEST")
            {
                return DirectionCommandType.West;
            }

            if (direction == "QUIT")
            {
                return DirectionCommandType.Quit;
            }

            return DirectionCommandType.Invalid;
        }

        public static ActionCommandType ParseActionCommand(string? input)
        {
            string action = ToUpperCase(input);

            if (action == "BATTLE" || action == "B")
            {
                return ActionCommandType.Battle;
            }

            if (action == "POKEINVENTORY" || action == "PI")
            {
                return ActionCommandType.PokeInventory;
            }

            if (action == "MENU" || action == "M")
            {
                return ActionCommandType.Menu;
            }

            return ActionCommandType.Invalid;
        }

        public static MenuCommandType ParseMenuCommand(string? input)
        {
            string menuChoice = ToUpperCase(input);

            if (menuChoice == "HEAL" || menuChoice == "H")
            {
                return MenuCommandType.Heal;
            }

            if (menuChoice == "BAG" || menuChoice == "B")
            {
                return MenuCommandType.Bag;
            }

            if (menuChoice == "SWAP" || menuChoice == "S")
            {
                return MenuCommandType.Swap;
            }

            if (menuChoice == "GYM" || menuChoice == "G")
            {
                return MenuCommandType.Gym;
            }

            if (menuChoice == "SAVE")
            {
                return MenuCommandType.Save;
            }

            return MenuCommandType.Invalid;
        }
    }
}
