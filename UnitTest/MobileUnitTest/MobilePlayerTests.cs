using Arcadia_Mobile.Creatures;
using Arcadia_Mobile.Map;
using Arcadia_Mobile.Player;

namespace UnitTest.MobileUnitTest;

public class MobilePlayerTests
{
    // Checks that mobile player construction stores name and starting room.
    [Fact]
    public void Constructor_WithNameAndStartingRoom_SetsInitialState()
    {
        GameMap map = new();

        Player player = new("Nova", map.StartRoom);

        Assert.Equal("Nova", player.Name);
        Assert.Same(map.StartRoom, player.CurrentRoom);
    }

    // Checks that mobile player construction rejects empty names.
    [Fact]
    public void Constructor_WithEmptyName_ThrowsArgumentException()
    {
        GameMap map = new();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Player(" ", map.StartRoom));

        Assert.Equal("name", exception.ParamName);
    }

    // Checks that moving the player updates the current room.
    [Fact]
    public void MoveTo_WithRoom_UpdatesCurrentRoom()
    {
        GameMap map = new();
        Player player = new("Nova", map.StartRoom);
        Room destination = map.GetRoom(RoomId.Ikena);

        player.MoveTo(destination);

        Assert.Same(destination, player.CurrentRoom);
    }

    // Checks that star fragments remain unique and display correctly.
    [Fact]
    public void AddStarFragment_DuplicateFragment_AddsOnlyOnce()
    {
        Player player = new("Nova", new GameMap().StartRoom);

        player.AddStarFragment("Nature Star Fragment");
        player.AddStarFragment("Nature Star Fragment");

        Assert.Single(player.StarFragments);
        Assert.Contains("Nature Star Fragment", player.GetStarFragmentDisplay());
    }

    // Checks that mobile bond values increase and cap at one hundred percent.
    [Fact]
    public void AddBond_AboveCap_ClampsToOneHundred()
    {
        Player player = new("Nova", new GameMap().StartRoom);

        player.AddBond(AnimalElement.Nature, 60);
        player.AddBond(AnimalElement.Nature, 60);

        Assert.Equal(100, player.GetBond(AnimalElement.Nature));
    }

    // Checks that restoring bond values clamps invalid persisted data into the accepted range.
    [Fact]
    public void RestoreBond_WithOutOfRangeValues_ClampsValues()
    {
        Player player = new("Nova", new GameMap().StartRoom);

        player.RestoreBond(new Dictionary<AnimalElement, int>
        {
            [AnimalElement.Nature] = 150,
            [AnimalElement.Mystic] = -10
        });

        Assert.Equal(100, player.GetBond(AnimalElement.Nature));
        Assert.Equal(0, player.GetBond(AnimalElement.Mystic));
    }

    // Checks that mobile animal inventory supports add, swap, replace, and remove operations.
    [Fact]
    public void AnimalInventory_PartyOperations_UpdateInventoryOrder()
    {
        Player player = new("Nova", new GameMap().StartRoom);
        Animal lion = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_LION");
        Animal wolf = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_WOLF");
        Animal dragon = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_DRAGON");

        player.AddAnimal(lion);
        player.AddAnimal(wolf);
        player.SwapAnimalPositions(0, 1);
        player.ReplaceAnimalAt(1, dragon);

        Assert.Same(wolf, player.GetAnimalAt(0));
        Assert.Same(dragon, player.GetAnimalAt(1));
        Assert.True(player.RemoveAnimal(wolf));
        Assert.Single(player.AnimalInventory);
    }

    // Checks that restoring animal inventory replaces the existing party.
    [Fact]
    public void RestoreAnimalInventory_ReplacesExistingAnimals()
    {
        Player player = new("Nova", new GameMap().StartRoom);
        Animal lion = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_LION");
        Animal wolf = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_WOLF");

        player.AddAnimal(lion);
        player.RestoreAnimalInventory(new[] { wolf });

        Animal restoredAnimal = Assert.Single(player.AnimalInventory);
        Assert.Same(wolf, restoredAnimal);
    }

    // Checks that computer player battle teams are cloned and active battle damage can be reset.
    [Fact]
    public void CompPlayer_PrepareForBattle_RebuildsInventoryFromTemplateClones()
    {
        CompPlayer trainer = new("Guardian", new GameMap().StartRoom);
        Animal lion = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_LION");

        trainer.SetBattleTeam(new[] { lion });
        Animal activeAnimal = trainer.GetAnimalAt(0);
        activeAnimal.Health -= 10;

        trainer.PrepareForBattle();

        Animal refreshedAnimal = trainer.GetAnimalAt(0);
        Assert.NotSame(lion, refreshedAnimal);
        Assert.Equal(lion.Health, refreshedAnimal.Health);
        Assert.Single(trainer.BattleTeamTemplate);
    }
}
