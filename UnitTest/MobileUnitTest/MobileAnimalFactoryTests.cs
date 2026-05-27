using Arcadia_Mobile.Creatures;

namespace UnitTest.MobileUnitTest;

public class MobileAnimalFactoryTests
{
    // Checks that the mobile animal factory creates the same roster size as the reference animal catalog.
    [Fact]
    public void CreateAnimals_ReturnsFullMobileAnimalRoster()
    {
        IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

        Assert.Equal(97, animals.Count);
        Assert.Equal("NULL0", animals[0].Name);
        Assert.Equal("N_CAT", animals[1].Name);
        Assert.Equal("NU_DRAGON", animals[^1].Name);
    }

    // Checks that each generated mobile animal has a valid four-move battle set.
    [Fact]
    public void CreateAnimals_EachAnimalHasFourMoves()
    {
        IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

        Assert.All(animals, animal => Assert.Equal(4, animal.Moves.Count));
    }

    // Checks that the mobile adult-animal catalog contains the stage-two species used by the intro flow.
    [Fact]
    public void CreateNatureAdultAnimals_ReturnsIntroChoiceSpecies()
    {
        IReadOnlyList<Animal> animals = AnimalFactory.CreateNatureAdultAnimals();

        Assert.Equal(
            new[] { "N_LION", "N_WOLF", "N_STALLION", "N_TORTOISE", "N_EAGLE", "N_BEE", "N_BEAR", "N_DRAGON" },
            animals.Select(animal => animal.Name).ToArray());
    }

    // Checks that cloning a mobile animal produces a separate mutable health instance.
    [Fact]
    public void Clone_CreatesSeparateAnimalWithSameData()
    {
        Animal original = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_LION");

        Animal clone = original.Clone();
        clone.Health -= 10;

        Assert.NotSame(original, clone);
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Name, clone.Name);
        Assert.NotEqual(original.Health, clone.Health);
    }

    // Checks that mobile animals reject invalid health values.
    [Fact]
    public void Health_SetOutsideBaseHealth_ThrowsArgumentOutOfRangeException()
    {
        Animal animal = AnimalFactory.CreateAnimals().Single(animal => animal.Name == "N_LION");

        Assert.Throws<ArgumentOutOfRangeException>(() => animal.Health = animal.BaseHealth + 1);
    }
}
