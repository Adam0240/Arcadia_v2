using Arcadia_v2;

namespace UnitTest
{
    public class AnimalTest
    {
        // Checks that an animal copies the provided move sequence into its runtime move list.
        [Fact]
        public void Constructor_CopiesMovesIntoMoveList()
        {
            Move[] moves = { MoveData.Ember, MoveData.FireFang, MoveData.QuickAttack };
            Animal animal = new(
                id: 7,
                name: "CHARMANDER",
                element: AnimalElement.Nature,
                speed: 7,
                baseHealth: 40,
                health: 40,
                level: 0,
                moves: moves);

            Assert.Equal(3, animal.Moves.Count);
            Assert.Same(MoveData.Ember, animal.Moves[0]);
            Assert.Same(MoveData.FireFang, animal.Moves[1]);
            Assert.Same(MoveData.QuickAttack, animal.Moves[2]);
        }

        // Checks that creating an animal without any moves throws an argument exception.
        [Fact]
        public void Constructor_EmptyMoveCollection_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Animal(
                    id: 0,
                    name: "ZERO",
                    element: AnimalElement.Nature,
                    speed: 0,
                    baseHealth: 0,
                    health: 0,
                    level: 0,
                    moves: Array.Empty<Move>()));
        }

        // Checks that creating an animal with more than four moves throws an argument exception.
        [Fact]
        public void Constructor_MoreThanFourMoves_ThrowsArgumentException()
        {
            Move[] moves = { MoveData.Tackle, MoveData.QuickAttack, MoveData.Bite, MoveData.Ember, MoveData.WaterGun };

            Assert.Throws<ArgumentException>(() =>
                new Animal(
                    id: 0,
                    name: "OVERFLOW",
                    element: AnimalElement.Nature,
                    speed: 0,
                    baseHealth: 10,
                    health: 10,
                    level: 1,
                    moves: moves));
        }

        // Checks that creating an animal with a null move is rejected at construction.
        [Fact]
        public void Constructor_NullMove_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Animal(
                    id: 0,
                    name: "TESTMON",
                    element: AnimalElement.Nature,
                    speed: 1,
                    baseHealth: 10,
                    health: 10,
                    level: 1,
                    moves: new[] { MoveData.Tackle, null! }));
        }

        // Checks that an animal cannot be created with an empty name.
        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Animal(
                    id: 0,
                    name: "",
                    element: AnimalElement.Nature,
                    speed: 1,
                    baseHealth: 10,
                    health: 10,
                    level: 1,
                    moves: new[] { MoveData.Tackle }));
        }

        // Checks that constructor health is constrained to the animal's valid health range.
        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void Constructor_InvalidHealth_ThrowsArgumentOutOfRangeException(int health)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Animal(
                    id: 0,
                    name: "TESTMON",
                    element: AnimalElement.Nature,
                    speed: 1,
                    baseHealth: 10,
                    health: health,
                    level: 1,
                    moves: new[] { MoveData.Tackle }));
        }

        // Checks that runtime health assignments are constrained to the animal's valid health range.
        [Fact]
        public void Health_InvalidAssignment_ThrowsArgumentOutOfRangeException()
        {
            Animal animal = new(
                id: 0,
                name: "TESTMON",
                element: AnimalElement.Nature,
                speed: 1,
                baseHealth: 10,
                health: 10,
                level: 1,
                moves: new[] { MoveData.Tackle });

            Assert.Throws<ArgumentOutOfRangeException>(() => animal.Health = 11);
        }

        // Checks that cloning creates a separate animal instance so later health changes do not affect the original.
        [Fact]
        public void Clone_CreatesIndependentAnimalCopy()
        {
            Animal original = new(
                id: 1,
                name: "CAT",
                element: AnimalElement.Nature,
                speed: 9,
                baseHealth: 75,
                health: 75,
                level: 4,
                moves: new[] { MoveData.Bite, MoveData.Moonlight });

            Animal clone = original.Clone();
            clone.Health = 10;

            Assert.NotSame(original, clone);
            Assert.Equal(75, original.Health);
            Assert.Equal(10, clone.Health);
            Assert.Equal(original.Name, clone.Name);
            Assert.Equal(original.Element, clone.Element);
            Assert.Equal(original.Moves.Select(move => move.Name), clone.Moves.Select(move => move.Name));
        }

        // Checks that the animal factory still creates the full expected roster size.
        [Fact]
        public void AnimalFactory_CreateAnimals_ReturnsExpectedCount()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(20, animals.Count);
        }

        // Checks that all creatures that used to be water-based now map to the Mystic element.
        [Fact]
        public void AnimalFactory_CreateAnimals_MapsFormerWaterPokemonToMystic()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(AnimalElement.Mystic, animals.Single(animal => animal.Name == "TURTLE").Element);
        }

        // Checks that non-water roster entries currently map to the Nature element during this migration stage.
        [Fact]
        public void AnimalFactory_CreateAnimals_MapsNonWaterPokemonToNature()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(AnimalElement.Nature, animals.Single(animal => animal.Name == "CAT").Element);
        }

        // Checks that the migrated animal roster still preserves key move assignments for important entries.
        [Fact]
        public void AnimalFactory_CreateAnimals_PreservesKeyRosterMoves()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();
            Animal cat = animals.Single(animal => animal.Name == "CAT");
            Animal lion = animals.Single(animal => animal.Name == "LION");

            Assert.Contains(MoveData.Moonlight, cat.Moves);
            Assert.Contains(MoveData.Sunlight, lion.Moves);
        }
    }
}
