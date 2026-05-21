using Arcadia_v2;
using Arcadia_v2.Commands;
using Arcadia_v2.Creatures;
using Arcadia_v2.Map;

namespace UnitTest;

public class ProgramFlowTests
{
    // Verifies that the uppercase helper uses invariant-uppercase conversion.
    [Fact]
    public void ToUpperCase_ConvertsInputToUppercase()
    {
        string result = Parser.ToUpperCase("n_Cat");

        Assert.Equal("N_CAT", result);
    }

    // Verifies that the name prompt helper returns the entered value without changing it.
    [Fact]
    public void GetName_ReturnsConsoleInput()
    {
        FakeGameIO io = new("Riley");

        string result = Program.GetName(io);

        Assert.Equal("Riley", result);
        Assert.Contains("Enter your name", io.OutputText);
    }

    // Verifies that blank names are rejected at the input boundary instead of crashing player setup.
    [Fact]
    public void GetName_BlankInput_RePromptsUntilNameIsEntered()
    {
        FakeGameIO io = new("   ", "Riley");

        string result = Program.GetName(io);

        Assert.Equal("Riley", result);
        Assert.Contains("Name cannot be empty.", io.OutputText);
        Assert.Equal(2, io.OutputText.Split("Enter your name").Length - 1);
    }

    // Verifies that a two-animal party swaps immediately without asking for creature names.
    [Fact]
    public void SwapAnimals_WithTwoAnimals_AutoSwapsPartyPositions()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "C_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.VENOM_FANG }));
        player.AddAnimal(new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.DEEPSEA_RUPTURE }));
        FakeGameIO io = new();

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("N_LION", player.AnimalInventory[0].Name);
        Assert.Equal("C_CAT", player.AnimalInventory[1].Name);
        Assert.Contains("You are swapping: C_CAT and N_LION .", io.OutputText);
        Assert.DoesNotContain("Who would you like to trade positions with?", io.OutputText);
    }

    // Verifies that an invalid first swap name causes the party swap flow to re-prompt until a valid name is entered.
    [Fact]
    public void SwapAnimals_InvalidFirstName_RePromptsUntilValidName()
    {
        Player player = CreateFourAnimalPlayer();

        FakeGameIO io = new("missing", "c_cat", "n_lion");

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("N_LION", player.AnimalInventory[0].Name);
        Assert.Equal("C_CAT", player.AnimalInventory[1].Name);
        Assert.Contains("Invalid animal name MISSING .", io.OutputText);
        Assert.Equal(2, io.OutputText.Split("Here are your animals. Who would you like to trade positions with?").Length - 1);
    }

    // Verifies that the swap flow uses exact matched party indexes when swapping within a four-creature party.
    [Fact]
    public void SwapAnimals_WithFourAnimals_UsesExactMatchedIndexes()
    {
        Player player = CreateFourAnimalPlayer();
        FakeGameIO io = new("t_cat", "n_dog");

        PartyFlow.SwapAnimals(player, io);

        Assert.Equal("C_CAT", player.AnimalInventory[0].Name);
        Assert.Equal("N_LION", player.AnimalInventory[1].Name);
        Assert.Equal("N_DOG", player.AnimalInventory[2].Name);
        Assert.Equal("T_CAT", player.AnimalInventory[3].Name);
    }

    private static Player CreateFourAnimalPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "C_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.VENOM_FANG }));
        player.AddAnimal(new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.DEEPSEA_RUPTURE }));
        player.AddAnimal(new Animal(id: 3, name: "T_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.VOLT_JAB }));
        player.AddAnimal(new Animal(id: 4, name: "N_DOG", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.THORNWRAP }));
        return player;
    }

}
