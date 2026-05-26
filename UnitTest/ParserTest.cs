using Arcadia_v2.Commands;

namespace UnitTest
{
    public class ParserTest
    {
        // Checks that main command text is normalized into the expected top-level command.
        [Theory]
        [InlineData("GO", MainCommandType.Go)]
        [InlineData("g", MainCommandType.Go)]
        [InlineData(" action ", MainCommandType.Action)]
        [InlineData("save", MainCommandType.Invalid)]
        [InlineData("bad", MainCommandType.Invalid)]
        public void ParseMainCommand_ReturnsExpectedCommand(string input, MainCommandType expected)
        {
            MainCommandType result = Parser.ParseMainCommand(input);

            Assert.Equal(expected, result);
        }

        // Checks that a combined main command line is split into the top-level command and the remaining subcommand text.
        [Fact]
        public void ParseMainCommandInput_GoNorth_ReturnsGoAndNorthRemainder()
        {
            MainCommandInput result = Parser.ParseMainCommandInput("go north");

            Assert.Equal(MainCommandType.Go, result.MainCommand);
            Assert.Equal("NORTH", result.Remainder);
        }

        // Checks that direction text maps to the matching direction enum or invalid when unsupported.
        [Theory]
        [InlineData("north", DirectionCommandType.North)]
        [InlineData("EAST", DirectionCommandType.East)]
        [InlineData(" South ", DirectionCommandType.South)]
        [InlineData("west", DirectionCommandType.West)]
        [InlineData("QUIT", DirectionCommandType.Quit)]
        [InlineData("up", DirectionCommandType.Invalid)]
        public void ParseDirectionCommand_ReturnsExpectedDirection(string input, DirectionCommandType expected)
        {
            DirectionCommandType result = Parser.ParseDirectionCommand(input);

            Assert.Equal(expected, result);
        }

        // Checks that action command text maps to the expected action enum.
        [Theory]
        [InlineData("battle", ActionCommandType.Battle)]
        [InlineData("B", ActionCommandType.Battle)]
        [InlineData("animalinventory", ActionCommandType.AnimalInventory)]
        [InlineData("animals", ActionCommandType.AnimalInventory)]
        [InlineData("ai", ActionCommandType.AnimalInventory)]
        [InlineData("menu", ActionCommandType.Menu)]
        [InlineData("m", ActionCommandType.Menu)]
        [InlineData("heal", ActionCommandType.Invalid)]
        public void ParseActionCommand_ReturnsExpectedAction(string input, ActionCommandType expected)
        {
            ActionCommandType result = Parser.ParseActionCommand(input);

            Assert.Equal(expected, result);
        }

        // Checks that menu command text maps to the expected menu option enum.
        [Theory]
        [InlineData("heal", MenuCommandType.Heal)]
        [InlineData("H", MenuCommandType.Heal)]
        [InlineData("bag", MenuCommandType.Bag)]
        [InlineData("b", MenuCommandType.Bag)]
        [InlineData("swap", MenuCommandType.Swap)]
        [InlineData("s", MenuCommandType.Swap)]
        [InlineData("sanctuary", MenuCommandType.Sanctuary)]
        [InlineData("sanctuaries", MenuCommandType.Sanctuary)]
        [InlineData("bond", MenuCommandType.Bond)]
        [InlineData("grow", MenuCommandType.Grow)]
        [InlineData("save", MenuCommandType.Save)]
        [InlineData("load", MenuCommandType.Invalid)]
        [InlineData("delete", MenuCommandType.Invalid)]
        [InlineData("menu", MenuCommandType.Invalid)]
        public void ParseMenuCommand_ReturnsExpectedMenuChoice(string input, MenuCommandType expected)
        {
            MenuCommandType result = Parser.ParseMenuCommand(input);

            Assert.Equal(expected, result);
        }

        // Checks that null input is converted into an empty uppercase-safe string.
        [Fact]
        public void ToUpperCase_NullInput_ReturnsEmptyString()
        {
            string result = Parser.ToUpperCase(null);

            Assert.Equal(string.Empty, result);
        }
    }
}
