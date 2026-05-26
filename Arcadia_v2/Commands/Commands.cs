namespace Arcadia_v2.Commands
{
    public static class Commands
    {
        public static MainCommandInput ReadMainCommandInput(IGameIO io)
        {
            io.WriteLine($"\nMove or action? ({BuildInlinePrompt(CommandDefinitions.MainCommands)})");
            return Parser.ParseMainCommandInput(io.ReadLine());
        }

        public static DirectionCommandType ReadDirectionCommand(IGameIO io)
        {
            io.WriteLine($"\nWhat direction would you like to move? ({BuildInlinePrompt(CommandDefinitions.DirectionCommands)})");
            return Parser.ParseDirectionCommand(io.ReadLine());
        }

        public static ActionCommandType ReadActionCommand(IGameIO io)
        {
            io.WriteLine("\nWhat would you like to do?");
            WriteCommandOptions(io, CommandDefinitions.ActionCommands);
            return Parser.ParseActionCommand(io.ReadLine());
        }

        public static MenuCommandType ReadMenuCommand(IGameIO io)
        {
            return ReadMenuCommand(io, includeGrowthCommand: false);
        }

        public static MenuCommandType ReadMenuCommand(IGameIO io, bool includeGrowthCommand)
        {
            io.WriteLine("\nMenu");

            if (includeGrowthCommand)
            {
                io.WriteLine("An animal is growing up!");
            }

            WriteCommandOptions(io, CommandDefinitions.GetMenuCommands(includeGrowthCommand));
            io.WriteLine("\nWhat would you like to do?");
            return Parser.ParseMenuCommand(io.ReadLine());
        }

        private static string BuildInlinePrompt<TCommand>(IEnumerable<CommandOption<TCommand>> options)
        {
            return string.Join(" || ", options.Select(option => string.Join(" || ", option.Aliases)));
        }

        private static void WriteCommandOptions<TCommand>(IGameIO io, IEnumerable<CommandOption<TCommand>> options)
        {
            foreach (CommandOption<TCommand> option in options)
            {
                io.WriteLine($"{option.DisplayName} ({string.Join("/", option.Aliases)})");
            }
        }
    }
}
