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
                currentHealth: 40,
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
                    currentHealth: 0,
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
                    currentHealth: 10,
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
                    currentHealth: 10,
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
                    currentHealth: 10,
                    level: 1,
                    moves: new[] { MoveData.Tackle }));
        }

        // Checks that constructor currentHealth is constrained to the animal's valid currentHealth range.
        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void Constructor_InvalidHealth_ThrowsArgumentOutOfRangeException(int currentHealth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Animal(
                    id: 0,
                    name: "TESTMON",
                    element: AnimalElement.Nature,
                    speed: 1,
                    baseHealth: 10,
                    currentHealth: currentHealth,
                    level: 1,
                    moves: new[] { MoveData.Tackle }));
        }

        // Checks that runtime currentHealth assignments are constrained to the animal's valid currentHealth range.
        [Fact]
        public void Health_InvalidAssignment_ThrowsArgumentOutOfRangeException()
        {
            Animal animal = new(
                id: 0,
                name: "TESTMON",
                element: AnimalElement.Nature,
                speed: 1,
                baseHealth: 10,
                currentHealth: 10,
                level: 1,
                moves: new[] { MoveData.Tackle });

            Assert.Throws<ArgumentOutOfRangeException>(() => animal.CurrentHealth = 11);
        }

        // Checks that cloning creates a separate animal instance so later currentHealth changes do not affect the original.
        [Fact]
        public void Clone_CreatesIndependentAnimalCopy()
        {
            Animal original = new(
                id: 1,
                name: "CAT",
                element: AnimalElement.Nature,
                speed: 9,
                baseHealth: 75,
                currentHealth: 75,
                level: 4,
                moves: new[] { MoveData.Bite, MoveData.Moonlight });

            Animal clone = original.Clone();
            clone.CurrentHealth = 10;

            Assert.NotSame(original, clone);
            Assert.Equal(75, original.CurrentHealth);
            Assert.Equal(10, clone.CurrentHealth);
            Assert.Equal(original.Name, clone.Name);
            Assert.Equal(original.Element, clone.Element);
            Assert.Equal(original.Moves.Select(move => move.Name), clone.Moves.Select(move => move.Name));
        }

        // Checks that the animal factory creates every species for every element.
        [Fact]
        public void AnimalFactory_CreateAnimals_ReturnsExpectedCount()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(96, animals.Count);
        }

        // Checks that factory validation keeps generated animals inside the legal battle move range.
        [Fact]
        public void AnimalFactory_CreateAnimals_CreatesOnlyValidMoveCounts()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.All(animals, animal => Assert.InRange(animal.Moves.Count, 1, 4));
        }

        // Checks that generated animal ids are based on stable species and element identities, not list order.
        [Fact]
        public void AnimalFactory_CreateAnimals_UsesStableSpeciesElementIds()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(1, animals.Single(animal => animal.Name == "Nature Cat").Id);
            Assert.Equal(2, animals.Single(animal => animal.Name == "Nature Lion").Id);
            Assert.Equal(101, animals.Single(animal => animal.Name == "Mystic Cat").Id);
            Assert.Equal(516, animals.Single(animal => animal.Name == "Nuclear Dragon").Id);
        }

        // Checks that generated Mystic variants map to the Mystic element.
        [Fact]
        public void AnimalFactory_CreateAnimals_CreatesMysticVariants()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(AnimalElement.Mystic, animals.Single(animal => animal.Name == "Mystic Turtle").Element);
        }

        // Checks that generated Nature variants map to the Nature element.
        [Fact]
        public void AnimalFactory_CreateAnimals_CreatesNatureVariants()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Equal(AnimalElement.Nature, animals.Single(animal => animal.Name == "Nature Cat").Element);
        }

        // Checks that themed Cat moves use element-specific names with shared damage values.
        [Fact]
        public void AnimalFactory_CreateAnimals_CreatesThemedCatMoves()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();
            Animal natureCat = animals.Single(animal => animal.Name == "Nature Cat");
            Animal mysticCat = animals.Single(animal => animal.Name == "Mystic Cat");

            AssertMove(natureCat.Moves[0], "Nature's Fury", MoveType.Nature, 10);
            AssertMove(natureCat.Moves[1], "Speed Attack", MoveType.Neutral, 5);
            AssertMove(natureCat.Moves[2], "Defensive Move", MoveType.Neutral, 1);
            AssertMove(natureCat.Moves[3], "Nature's Bomb", MoveType.Nature, 12);

            AssertMove(mysticCat.Moves[0], "Mystic Fury", MoveType.Mystic, 10);
            AssertMove(mysticCat.Moves[1], "Speed Attack", MoveType.Neutral, 5);
            AssertMove(mysticCat.Moves[2], "Defensive Move", MoveType.Neutral, 1);
            AssertMove(mysticCat.Moves[3], "Mystic Bomb", MoveType.Mystic, 12);
        }

        // Checks that variants of the same species share the same base stats.
        [Fact]
        public void AnimalFactory_CreateAnimals_VariantsShareSpeciesStats()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();
            Animal natureCat = animals.Single(animal => animal.Name == "Nature Cat");
            Animal mysticCat = animals.Single(animal => animal.Name == "Mystic Cat");

            Assert.Equal(natureCat.Speed, mysticCat.Speed);
            Assert.Equal(natureCat.BaseHealth, mysticCat.BaseHealth);
        }

        // Checks that two different species can reuse the same elemental move template.
        [Fact]
        public void AnimalFactory_CreateAnimals_AllowsSpeciesToShareElementMoveTemplates()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();
            Animal natureLion = animals.Single(animal => animal.Name == "Nature Lion");
            Animal natureWolf = animals.Single(animal => animal.Name == "Nature Wolf");

            AssertMove(natureLion.Moves[0], "Nature's Storm", MoveType.Nature, 10);
            AssertMove(natureWolf.Moves[0], "Nature's Storm", MoveType.Nature, 10);
            AssertMove(natureLion.Moves[3], "Nature's Roar", MoveType.Nature, 8);
            AssertMove(natureWolf.Moves[3], "Nature's Howl", MoveType.Nature, 8);
        }

        // Checks that one species gets a variant for every defined non-placeholder element.
        [Fact]
        public void AnimalFactory_CreateAnimals_CreatesEveryElementForSpecies()
        {
            IReadOnlyList<Animal> animals = AnimalFactory.CreateAnimals();

            Assert.Contains(animals, animal => animal.Name == "Nature Dragon");
            Assert.Contains(animals, animal => animal.Name == "Mystic Dragon");
            Assert.Contains(animals, animal => animal.Name == "Thunder Dragon");
            Assert.Contains(animals, animal => animal.Name == "Draconic Dragon");
            Assert.Contains(animals, animal => animal.Name == "Cosmic Dragon");
            Assert.Contains(animals, animal => animal.Name == "Nuclear Dragon");
        }

        private static void AssertMove(Move move, string expectedName, MoveType expectedType, int expectedPower)
        {
            Assert.Equal(expectedName, move.Name);
            Assert.Equal(expectedType, move.Type);
            Assert.Equal(expectedPower, move.Power);
        }
    }
}
