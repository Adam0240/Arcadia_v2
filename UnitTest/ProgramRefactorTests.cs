using Arcadia_v2;
using Arcadia_v2.Commands;
using Arcadia_v2.Map;

namespace UnitTest;

[Collection("Console Input")]
public class ProgramRefactorTests
{
    // Verifies that the uppercase helper preserves the legacy invariant-uppercase conversion.
    [Fact]
    public void ToUpperCase_ConvertsInputToUppercase()
    {
        string result = Parser.ToUpperCase("PiKaChu");

        Assert.Equal("PIKACHU", result);
    }

    // Verifies that the name prompt helper returns the entered value without changing it.
    [Fact]
    public void GetName_ReturnsConsoleInput()
    {
        TextReader originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("Ash" + Environment.NewLine));

            string result = Program.GetName();

            Assert.Equal("Ash", result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    // Verifies that swapping two valid Pokemon names updates the active party order.
    [Fact]
    public void SwapPokemon_WithTwoValidNames_SwapsPartyPositions()
    {
        TextReader originalIn = Console.In;
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddPokemon(new Pokemon(1, "UMBREON", PokemonType.Dark, 7, 10, 10, 1, new[] { MoveData.Bite }));
        player.AddPokemon(new Pokemon(2, "ESPEON", PokemonType.Psychic, 7, 10, 10, 1, new[] { MoveData.Psychic }));

        try
        {
            Console.SetIn(new StringReader("umbreon" + Environment.NewLine + "espeon" + Environment.NewLine));

            PartyFlow.SwapPokemon(player);

            Assert.Equal("ESPEON", player.PokemonInventory[0].Name);
            Assert.Equal("UMBREON", player.PokemonInventory[1].Name);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }
}
