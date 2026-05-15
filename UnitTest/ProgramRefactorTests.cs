using Arcadia_v2;
using Arcadia_v2.Commands;
using Arcadia_v2.Map;

namespace UnitTest;

public class ProgramFlowTests
{
    // Verifies that the uppercase helper uses invariant-uppercase conversion.
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
        FakeGameIO io = new("Ash");

        string result = Program.GetName(io);

        Assert.Equal("Ash", result);
        Assert.Contains("Enter your name", io.OutputText);
    }

    // Verifies that swapping two valid Pokemon names updates the active party order.
    [Fact]
    public void SwapPokemon_WithTwoValidNames_SwapsPartyPositions()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddPokemon(new Pokemon(1, "UMBREON", PokemonType.Dark, 7, 10, 10, 1, new[] { MoveData.Bite }));
        player.AddPokemon(new Pokemon(2, "ESPEON", PokemonType.Psychic, 7, 10, 10, 1, new[] { MoveData.Psychic }));
        FakeGameIO io = new("umbreon", "espeon");

        PartyFlow.SwapPokemon(player, io);

        Assert.Equal("ESPEON", player.PokemonInventory[0].Name);
        Assert.Equal("UMBREON", player.PokemonInventory[1].Name);
        Assert.DoesNotContain("its working.", io.OutputText);
    }

    [Fact]
    public void SwapPokemon_InvalidFirstName_RePromptsUntilValidName()
    {
        Player player = CreateFourPokemonPlayer();

        FakeGameIO io = new("missing", "umbreon", "espeon");

        PartyFlow.SwapPokemon(player, io);

        Assert.Equal("ESPEON", player.PokemonInventory[0].Name);
        Assert.Equal("UMBREON", player.PokemonInventory[1].Name);
        Assert.Contains("Invalid Pokemon name MISSING .", io.OutputText);
        Assert.Equal(2, io.OutputText.Split("Heres your Pokemon. Who would you like to trade positions with?").Length - 1);
    }

    [Fact]
    public void SwapPokemon_WithFourPokemon_UsesExactMatchedIndexes()
    {
        Player player = CreateFourPokemonPlayer();
        FakeGameIO io = new("jolteon", "flareon");

        PartyFlow.SwapPokemon(player, io);

        Assert.Equal("UMBREON", player.PokemonInventory[0].Name);
        Assert.Equal("ESPEON", player.PokemonInventory[1].Name);
        Assert.Equal("FLAREON", player.PokemonInventory[2].Name);
        Assert.Equal("JOLTEON", player.PokemonInventory[3].Name);
    }

    private static Player CreateFourPokemonPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddPokemon(new Pokemon(1, "UMBREON", PokemonType.Dark, 7, 10, 10, 1, new[] { MoveData.Bite }));
        player.AddPokemon(new Pokemon(2, "ESPEON", PokemonType.Psychic, 7, 10, 10, 1, new[] { MoveData.Psychic }));
        player.AddPokemon(new Pokemon(3, "JOLTEON", PokemonType.Electric, 7, 10, 10, 1, new[] { MoveData.Thunderbolt }));
        player.AddPokemon(new Pokemon(4, "FLAREON", PokemonType.Fire, 7, 10, 10, 1, new[] { MoveData.Ember }));
        return player;
    }

}
