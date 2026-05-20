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

    // Verifies that blank names are rejected at the input boundary instead of crashing player setup.
    [Fact]
    public void GetName_BlankInput_RePromptsUntilNameIsEntered()
    {
        FakeGameIO io = new("   ", "Ash");

        string result = Program.GetName(io);

        Assert.Equal("Ash", result);
        Assert.Contains("Name cannot be empty.", io.OutputText);
        Assert.Equal(2, io.OutputText.Split("Enter your name").Length - 1);
    }

    // Verifies that a two-animal party swaps immediately without asking for creature names.
    [Fact]
    public void SwapAnimals_WithTwoAnimals_AutoSwapsPartyPositions()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "UMBREON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Bite }));
        player.AddAnimal(new Animal(id: 2, name: "ESPEON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Psychic }));
        FakeGameIO io = new();

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("ESPEON", player.AnimalInventory[0].Name);
        Assert.Equal("UMBREON", player.AnimalInventory[1].Name);
        Assert.Contains("You are swapping: UMBREON and ESPEON .", io.OutputText);
        Assert.DoesNotContain("Who would you like to trade positions with?", io.OutputText);
    }

    // Verifies that an invalid first swap name causes the party swap flow to re-prompt until a valid name is entered.
    [Fact]
    public void SwapAnimals_InvalidFirstName_RePromptsUntilValidName()
    {
        Player player = CreateFourPokemonPlayer();

        FakeGameIO io = new("missing", "umbreon", "espeon");

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("ESPEON", player.AnimalInventory[0].Name);
        Assert.Equal("UMBREON", player.AnimalInventory[1].Name);
        Assert.Contains("Invalid animal name MISSING .", io.OutputText);
        Assert.Equal(2, io.OutputText.Split("Here are your animals. Who would you like to trade positions with?").Length - 1);
    }

    // Verifies that the swap flow uses exact matched party indexes when swapping within a four-creature party.
    [Fact]
    public void SwapAnimals_WithFourAnimals_UsesExactMatchedIndexes()
    {
        Player player = CreateFourPokemonPlayer();
        FakeGameIO io = new("jolteon", "flareon");

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("UMBREON", player.AnimalInventory[0].Name);
        Assert.Equal("ESPEON", player.AnimalInventory[1].Name);
        Assert.Equal("FLAREON", player.AnimalInventory[2].Name);
        Assert.Equal("JOLTEON", player.AnimalInventory[3].Name);
    }

    private static Player CreateFourPokemonPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "UMBREON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Bite }));
        player.AddAnimal(new Animal(id: 2, name: "ESPEON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Psychic }));
        player.AddAnimal(new Animal(id: 3, name: "JOLTEON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Thunderbolt }));
        player.AddAnimal(new Animal(id: 4, name: "FLAREON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Ember }));
        return player;
    }

}
