using Arcadia_v2.Commands;
using System.IO;
using System.Text;

namespace UnitTest
{
    public class CommandTest
    {
        // Checks that reading the main command from console input returns Go and prints the expected prompt.
        [Fact]
        public void ReadMainCommandInput_GoInput_ReturnsGo()
        {
            MainCommandInput result = RunWithConsoleIo("GO", Commands.ReadMainCommandInput, out string output);

            Assert.Equal(MainCommandType.Go, result.MainCommand);
            Assert.Equal(string.Empty, result.Remainder);
            Assert.Contains("Move or action?", output);
        }

        // Checks that reading a combined main command line captures the main command and the remaining subcommand text.
        [Fact]
        public void ReadMainCommandInput_GoNorthInput_ReturnsGoWithRemainder()
        {
            MainCommandInput result = RunWithConsoleIo("go north", Commands.ReadMainCommandInput, out string output);

            Assert.Equal(MainCommandType.Go, result.MainCommand);
            Assert.Equal("NORTH", result.Remainder);
            Assert.Contains("Move or action?", output);
        }

        // Checks that reading the direction command from console input returns North and prints the direction prompt.
        [Fact]
        public void ReadDirectionCommand_NorthInput_ReturnsNorth()
        {
            DirectionCommandType result = RunWithConsoleIo("north", Commands.ReadDirectionCommand, out string output);

            Assert.Equal(DirectionCommandType.North, result);
            Assert.Contains("What direction would you like to move?", output);
        }

        // Checks that reading the action command from console input returns Menu and prints the action prompt.
        [Fact]
        public void ReadActionCommand_MenuInput_ReturnsMenu()
        {
            ActionCommandType result = RunWithConsoleIo("menu", Commands.ReadActionCommand, out string output);

            Assert.Equal(ActionCommandType.Menu, result);
            Assert.Contains("What would you like to do?", output);
        }

        // Checks that reading the menu command from console input returns Swap and prints the menu text.
        [Fact]
        public void ReadMenuCommand_SwapInput_ReturnsSwap()
        {
            MenuCommandType result = RunWithConsoleIo("swap", Commands.ReadMenuCommand, out string output);

            Assert.Equal(MenuCommandType.Swap, result);
            Assert.Contains("Menu", output);
            Assert.Contains("Swap Pokemon", output);
            Assert.Contains("Save Game", output);
            Assert.DoesNotContain("Load Game", output);
            Assert.DoesNotContain("Delete Save", output);
        }

        // Checks that direction enum values convert to the same legacy numeric choices used by the command flow.
        [Theory]
        [InlineData(DirectionCommandType.North, 1)]
        [InlineData(DirectionCommandType.Quit, 5)]
        [InlineData(DirectionCommandType.Invalid, 0)]
        public void GetDirectionChoice_ReturnsEnumValue(DirectionCommandType command, int expected)
        {
            int result = Commands.GetDirectionChoice(command);

            Assert.Equal(expected, result);
        }

        // Checks that action enum values convert to the same legacy numeric choices used by the command flow.
        [Theory]
        [InlineData(ActionCommandType.Battle, 1)]
        [InlineData(ActionCommandType.Menu, 3)]
        [InlineData(ActionCommandType.Invalid, 0)]
        public void GetActionChoice_ReturnsEnumValue(ActionCommandType command, int expected)
        {
            int result = Commands.GetActionChoice(command);

            Assert.Equal(expected, result);
        }

        // Checks that menu enum values convert to the same legacy numeric choices used by the menu flow.
        [Theory]
        [InlineData(MenuCommandType.Heal, 1)]
        [InlineData(MenuCommandType.Gym, 4)]
        [InlineData(MenuCommandType.Invalid, 0)]
        public void GetMenuChoice_ReturnsEnumValue(MenuCommandType command, int expected)
        {
            int result = Commands.GetMenuChoice(command);

            Assert.Equal(expected, result);
        }

        private static T RunWithConsoleIo<T>(string input, Func<T> action, out string output)
        {
            TextReader originalIn = Console.In;
            TextWriter originalOut = Console.Out;
            StringReader reader = new StringReader(input + Environment.NewLine);
            StringWriter writer = new StringWriter(new StringBuilder());

            try
            {
                Console.SetIn(reader);
                Console.SetOut(writer);
                T result = action();
                output = writer.ToString();
                return result;
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
                reader.Dispose();
                writer.Dispose();
            }
        }
    }
}
