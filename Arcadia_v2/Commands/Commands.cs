namespace Arcadia_v2.Commands
{
    public static class Commands
    {
        public static MainCommandInput ReadMainCommandInput(IGameIO io)
        {
            io.WriteLine("\nMove or action? (GO || G || Action || A)");
            return Parser.ParseMainCommandInput(io.ReadLine());
        }

        public static DirectionCommandType ReadDirectionCommand(IGameIO io)
        {
            io.WriteLine("\nWhat direction would you like to move? (North || East || South || West || Quit)");
            return Parser.ParseDirectionCommand(io.ReadLine());
        }

        public static ActionCommandType ReadActionCommand(IGameIO io)
        {
            io.WriteLine("\nWhat would you like to do?");
            io.WriteLine("(Battle (battle/b) || PokeInventory (pokeinventory/pi) || Menu (menu/m)\n");
            return Parser.ParseActionCommand(io.ReadLine());
        }

        public static MenuCommandType ReadMenuCommand(IGameIO io)
        {
            io.WriteLine("\nMenu");
            io.WriteLine("Heal Pokemon (heal/h)");
            io.WriteLine("Check Bag (bag/b)");
            io.WriteLine("Swap Pokemon (swap/b)");
            io.WriteLine("Check Gyms (gym/g)");
            io.WriteLine("Save Game (save)");
            io.WriteLine("\nWhat would you like to do?");
            return Parser.ParseMenuCommand(io.ReadLine());
        }
    }
}
