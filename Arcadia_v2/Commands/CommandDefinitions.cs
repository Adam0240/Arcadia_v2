namespace Arcadia_v2.Commands
{
    public sealed record CommandOption<TCommand>(
        TCommand Command,
        string DisplayName,
        params string[] Aliases);

    public static class CommandDefinitions
    {
        public static readonly CommandOption<MainCommandType>[] MainCommands =
        {
            new(MainCommandType.Go, "Move", "go", "g"),
            new(MainCommandType.Action, "Action", "action", "a")
        };

        public static readonly CommandOption<DirectionCommandType>[] DirectionCommands =
        {
            new(DirectionCommandType.North, "North", "north"),
            new(DirectionCommandType.East, "East", "east"),
            new(DirectionCommandType.South, "South", "south"),
            new(DirectionCommandType.West, "West", "west"),
            new(DirectionCommandType.Quit, "Quit", "quit")
        };

        public static readonly CommandOption<ActionCommandType>[] ActionCommands =
        {
            new(ActionCommandType.Battle, "Battle", "battle", "b"),
            new(ActionCommandType.AnimalInventory, "AnimalInventory", "animalinventory", "animals", "ai"),
            new(ActionCommandType.Menu, "Menu", "menu", "m")
        };

        private static readonly CommandOption<MenuCommandType> GrowthMenuCommand =
            new(MenuCommandType.Grow, "Grow", "grow");

        private static readonly CommandOption<MenuCommandType>[] BaseMenuCommands =
        {
            new(MenuCommandType.Heal, "Heal Animals", "heal", "h"),
            new(MenuCommandType.Bag, "Check Bag", "bag", "b"),
            new(MenuCommandType.Swap, "Swap Animals", "swap", "s"),
            new(MenuCommandType.Sanctuary, "Check Sanctuaries", "sanctuary", "sanctuaries"),
            new(MenuCommandType.Bond, "Check Bond", "bond"),
            new(MenuCommandType.Save, "Save Game", "save")
        };

        public static readonly CommandOption<MenuCommandType>[] MenuCommands =
            BaseMenuCommands
                .Concat(new[] { GrowthMenuCommand })
                .ToArray();

        public static IEnumerable<CommandOption<MenuCommandType>> GetMenuCommands(bool includeGrowthCommand)
        {
            if (!includeGrowthCommand)
            {
                return BaseMenuCommands;
            }

            return BaseMenuCommands
                .Take(BaseMenuCommands.Length - 1)
                .Concat(new[] { GrowthMenuCommand })
                .Concat(BaseMenuCommands.TakeLast(1));
        }
    }
}
