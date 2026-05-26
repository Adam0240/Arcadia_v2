using Arcadia_v2;
using Arcadia_v2.Creatures;
using Arcadia_v2.Map;

namespace UnitTest
{
    public class PlayerTest
    {
        // Checks that creating a player with an empty name throws an argument exception.
        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            Room startRoom = new("Professor's Lab", "Starting room");

            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Player(" ", startRoom));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a player without a starting room throws an argument null exception.
        [Fact]
        public void Constructor_NullStartingRoom_ThrowsArgumentNullException()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Player("Red", null!));
            Assert.Equal("startingRoom", exception.ParamName);
        }

        // Checks that moving a player to a null room throws an argument null exception.
        [Fact]
        public void MoveTo_NullRoom_ThrowsArgumentNullException()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => player.MoveTo(null!));
            Assert.Equal("room", exception.ParamName);
        }

        // Checks that adding the same star fragment twice only stores one copy.
        [Fact]
        public void AddStarFragment_AddsUniqueStarFragmentOnlyOnce()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            player.AddStarFragment("Stone Star Fragment");
            player.AddStarFragment("Stone Star Fragment");

            Assert.Single(player.StarFragments);
            Assert.Equal("Stone Star Fragment", player.StarFragments[0]);
        }

        // Checks that adding an empty star fragment name throws an argument exception.
        [Fact]
        public void AddStarFragment_EmptyStarFragment_ThrowsArgumentException()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            ArgumentException exception = Assert.Throws<ArgumentException>(() => player.AddStarFragment(""));
            Assert.Equal("starFragment", exception.ParamName);
        }

        // Checks that the star fragment display returns all earned star fragments in the expected format.
        [Fact]
        public void GetStarFragmentDisplay_WithStarFragments_ReturnsFormattedStarFragmentList()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            player.AddStarFragment("Stone Star Fragment");
            player.AddStarFragment("Cascade Star Fragment");

            Assert.Equal("Star Fragments:\nStone Star Fragment\nCascade Star Fragment", player.GetStarFragmentDisplay());
        }

        // Checks that new players start with zero bond for every element.
        [Fact]
        public void BondByElement_NewPlayer_StartsAtZeroForEveryElement()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            Assert.All(Enum.GetValues<AnimalElement>(), element => Assert.Equal(0, player.GetBond(element)));
        }

        // Checks that adding bond clamps the meter at one hundred percent.
        [Fact]
        public void AddBond_AboveLimit_ClampsAtOneHundred()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            player.AddBond(AnimalElement.Nature, 50);
            player.AddBond(AnimalElement.Nature, 75);

            Assert.Equal(100, player.GetBond(AnimalElement.Nature));
        }

        // Checks that the bond display lists each element and its current meter.
        [Fact]
        public void GetBondDisplay_ReturnsAllElementMeters()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            player.AddBond(AnimalElement.Nature, 50);

            Assert.Equal(
                "Bond:\nNature 50%/100%\nMystic 0%/100%\nThunder 0%/100%\nDraconic 0%/100%\nCosmic 0%/100%\nNuclear 0%/100%",
                player.GetBondDisplay());
        }

        // Checks that replacing an animal keeps party ownership inside the player model.
        [Fact]
        public void ReplaceAnimalAt_ReplacesSelectedPartyAnimal()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            Animal cat = GameData.CreateAnimals().Single(animal => animal.Name == "N_CAT");
            Animal lion = GameData.CreateAnimals().Single(animal => animal.Name == "N_LION");
            player.AddAnimal(cat);

            player.ReplaceAnimalAt(0, lion);

            Assert.Equal("N_LION", player.AnimalInventory[0].Name);
        }

        // Checks that an empty animal inventory returns the expected empty-state display text.
        [Fact]
        public void GetAnimalInventoryDisplay_EmptyInventory_ReturnsEmptyMessage()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            Assert.Equal("Inventory is Empty! :'( ", player.GetAnimalInventoryDisplay());
        }

        // Checks that the animal inventory display includes each stored creature and its health.
        [Fact]
        public void GetAnimalInventoryDisplay_WithAnimals_ReturnsFormattedInventory()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            Animal thunderCat = new(
                id: 25,
                name: "T_CAT",
                element: AnimalElement.Nature,
                speed: 12,
                baseHealth: 35,
                health: 35,
                level: 5,
                moves: new[] { MoveData.VOLT_JAB });
            Animal mysticTurtle = new(
                id: 7,
                name: "M_TURTLE",
                element: AnimalElement.Mystic,
                speed: 10,
                baseHealth: 40,
                health: 30,
                level: 5,
                moves: new[] { MoveData.CURRENT_RUSH });
            player.AddAnimal(thunderCat);
            player.AddAnimal(mysticTurtle);

            Assert.Equal("Inventory List:\nT_CAT Health: 35\nM_TURTLE Health: 30", player.GetAnimalInventoryDisplay());
        }

        // Checks that setting a computer player's battle team clones the template animals instead of reusing the same instances.
        [Fact]
        public void SetBattleTeam_ClonesTemplateAnimals()
        {
            CompPlayer compPlayer = new("Blue", new Room("Nature Sanctuary", "Battle room"));
            Animal templateAnimal = new(
                id: 25,
                name: "T_CAT",
                element: AnimalElement.Nature,
                speed: 12,
                baseHealth: 35,
                health: 35,
                level: 5,
                moves: new[] { MoveData.VOLT_JAB });

            compPlayer.SetBattleTeam(new[] { templateAnimal });

            Assert.Single(compPlayer.BattleTeamTemplate);
            Assert.Single(compPlayer.AnimalInventory);
            Assert.NotSame(templateAnimal, compPlayer.BattleTeamTemplate[0]);
            Assert.NotSame(compPlayer.BattleTeamTemplate[0], compPlayer.AnimalInventory[0]);
            Assert.Equal(templateAnimal.Name, compPlayer.BattleTeamTemplate[0].Name);
        }

        // Checks that preparing a computer player for battle rebuilds the active animal inventory from the stored team template.
        [Fact]
        public void PrepareForBattle_RebuildsInventoryFromTemplate()
        {
            CompPlayer compPlayer = new("Blue", new Room("Nature Sanctuary", "Battle room"));
            Animal templateAnimal = new(
                id: 25,
                name: "T_CAT",
                element: AnimalElement.Nature,
                speed: 12,
                baseHealth: 35,
                health: 35,
                level: 5,
                moves: new[] { MoveData.VOLT_JAB });
            compPlayer.SetBattleTeam(new[] { templateAnimal });
            compPlayer.AnimalInventory[0].Health = 5;

            compPlayer.PrepareForBattle();

            Assert.Single(compPlayer.AnimalInventory);
            Assert.NotSame(compPlayer.BattleTeamTemplate[0], compPlayer.AnimalInventory[0]);
            Assert.Equal(compPlayer.BattleTeamTemplate[0].Health, compPlayer.AnimalInventory[0].Health);
        }
    }
}
