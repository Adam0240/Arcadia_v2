using Arcadia_v2.Commands;

namespace UnitTest
{
    public class CommandTest
    {
        // Checks that reading the main command from console input returns Go and prints the expected prompt.
        [Fact]
        public void ReadMainCommandInput_GoInput_ReturnsGo()
        {
            FakeGameIO io = new("GO");

            MainCommandInput result = Commands.ReadMainCommandInput(io);

            Assert.Equal(MainCommandType.Go, result.MainCommand);
            Assert.Equal(string.Empty, result.Remainder);
            Assert.Contains("Move or action?", io.OutputText);
        }

        // Checks that reading a combined main command line captures the main command and the remaining subcommand text.
        [Fact]
        public void ReadMainCommandInput_GoNorthInput_ReturnsGoWithRemainder()
        {
            FakeGameIO io = new("go north");

            MainCommandInput result = Commands.ReadMainCommandInput(io);

            Assert.Equal(MainCommandType.Go, result.MainCommand);
            Assert.Equal("NORTH", result.Remainder);
            Assert.Contains("Move or action?", io.OutputText);
        }

        // Checks that reading the direction command from console input returns North and prints the direction prompt.
        [Fact]
        public void ReadDirectionCommand_NorthInput_ReturnsNorth()
        {
            FakeGameIO io = new("north");

            DirectionCommandType result = Commands.ReadDirectionCommand(io);

            Assert.Equal(DirectionCommandType.North, result);
            Assert.Contains("What direction would you like to move?", io.OutputText);
        }

        // Checks that reading the action command from console input returns Menu and prints the action prompt.
        [Fact]
        public void ReadActionCommand_MenuInput_ReturnsMenu()
        {
            FakeGameIO io = new("menu");

            ActionCommandType result = Commands.ReadActionCommand(io);

            Assert.Equal(ActionCommandType.Menu, result);
            Assert.Contains("What would you like to do?", io.OutputText);
        }

        // Checks that reading the menu command from console input returns Swap and prints the menu text.
        [Fact]
        public void ReadMenuCommand_SwapInput_ReturnsSwap()
        {
            FakeGameIO io = new("swap");

            MenuCommandType result = Commands.ReadMenuCommand(io);

            Assert.Equal(MenuCommandType.Swap, result);
            Assert.Contains("Menu", io.OutputText);
            Assert.Contains("Swap Animals (swap/s)", io.OutputText);
            Assert.DoesNotContain("Swap Animals (swap/b)", io.OutputText);
            Assert.Contains("Check Bond (bond)", io.OutputText);
            Assert.DoesNotContain("Grow (grow)", io.OutputText);
            Assert.Contains("Save Game", io.OutputText);
            Assert.DoesNotContain("Load Game", io.OutputText);
            Assert.DoesNotContain("Delete Save", io.OutputText);
        }

        // Checks that the displayed swap shortcut maps to the swap command.
        [Fact]
        public void ReadMenuCommand_SShortcut_ReturnsSwap()
        {
            FakeGameIO io = new("s");

            MenuCommandType result = Commands.ReadMenuCommand(io);

            Assert.Equal(MenuCommandType.Swap, result);
            Assert.Contains("Swap Animals (swap/s)", io.OutputText);
        }

        // Checks that the grow command is displayed only when the caller includes growth options.
        [Fact]
        public void ReadMenuCommand_WithGrowthOption_PrintsGrowthPromptAndReturnsGrow()
        {
            FakeGameIO io = new("grow");

            MenuCommandType result = Commands.ReadMenuCommand(io, includeGrowthCommand: true);

            Assert.Equal(MenuCommandType.Grow, result);
            Assert.Contains("An animal is growing up!", io.OutputText);
            Assert.Contains("Grow (grow)", io.OutputText);
        }
    }
}
