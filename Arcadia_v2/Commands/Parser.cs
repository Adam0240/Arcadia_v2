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
        AnimalInventory = 2,
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
            return ParseCommand(input, CommandDefinitions.MainCommands, MainCommandType.Invalid);
        }

        public static DirectionCommandType ParseDirectionCommand(string? input)
        {
            return ParseCommand(input, CommandDefinitions.DirectionCommands, DirectionCommandType.Invalid);
        }

        public static ActionCommandType ParseActionCommand(string? input)
        {
            return ParseCommand(input, CommandDefinitions.ActionCommands, ActionCommandType.Invalid);
        }

        public static MenuCommandType ParseMenuCommand(string? input)
        {
            return ParseCommand(input, CommandDefinitions.MenuCommands, MenuCommandType.Invalid);
        }

        private static TCommand ParseCommand<TCommand>(
            string? input,
            IEnumerable<CommandOption<TCommand>> options,
            TCommand invalidCommand)
        {
            string command = ToUpperCase(input);

            foreach (CommandOption<TCommand> option in options)
            {
                if (option.Aliases.Any(alias => ToUpperCase(alias) == command))
                {
                    return option.Command;
                }
            }

            return invalidCommand;
        }
    }
}
