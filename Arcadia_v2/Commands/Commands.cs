namespace Arcadia_v2.Commands
{
    public static class Commands
    {
        public static MainCommandInput ReadMainCommandInput()
        {
            Console.WriteLine("\nMove or action? (GO || G || Action || A)");
            return Parser.ParseMainCommandInput(Console.ReadLine());
        }

        public static DirectionCommandType ReadDirectionCommand()
        {
            Console.WriteLine("\nWhat direction would you like to move? (North || East || South || West || Quit)");
            return Parser.ParseDirectionCommand(Console.ReadLine());
        }

        public static ActionCommandType ReadActionCommand()
        {
            Console.WriteLine("\nWhat would you like to do?");
            Console.WriteLine("(Battle (battle/b) || PokeInventory (pokeinventory/pi) || Menu (menu/m)\n");
            return Parser.ParseActionCommand(Console.ReadLine());
        }

        public static MenuCommandType ReadMenuCommand()
        {
            Console.WriteLine("\nMenu");
            Console.WriteLine("Heal Pokemon (heal/h)");
            Console.WriteLine("Check Bag (bag/b)");
            Console.WriteLine("Swap Pokemon (swap/b)");
            Console.WriteLine("Check Gyms (gym/g)");
            Console.WriteLine("Save Game (save)");
            Console.WriteLine("\nWhat would you like to do?");
            return Parser.ParseMenuCommand(Console.ReadLine());
        }

        public static int GetDirectionChoice(DirectionCommandType directionCommand)
        {
            return (int)directionCommand;
        }

        public static int GetActionChoice(ActionCommandType actionCommand)
        {
            return (int)actionCommand;
        }

        public static int GetMenuChoice(MenuCommandType menuCommand)
        {
            return (int)menuCommand;
        }
    }
}
