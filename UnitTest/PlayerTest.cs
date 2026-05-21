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

        // Checks that adding the same badge twice only stores one copy.
        [Fact]
        public void AddBadge_AddsUniqueBadgeOnlyOnce()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            player.AddBadge("Boulder Badge");
            player.AddBadge("Boulder Badge");

            Assert.Single(player.Badges);
            Assert.Equal("Boulder Badge", player.Badges[0]);
        }

        // Checks that adding an empty badge name throws an argument exception.
        [Fact]
        public void AddBadge_EmptyBadge_ThrowsArgumentException()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            ArgumentException exception = Assert.Throws<ArgumentException>(() => player.AddBadge(""));
            Assert.Equal("badge", exception.ParamName);
        }

        // Checks that the badge display returns all earned badges in the expected format.
        [Fact]
        public void GetBadgeDisplay_WithBadges_ReturnsFormattedBadgeList()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            player.AddBadge("Boulder Badge");
            player.AddBadge("Cascade Badge");

            Assert.Equal("Badges:\nBoulder Badge\nCascade Badge", player.GetBadgeDisplay());
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
            CompPlayer compPlayer = new("Blue", new Room("Oak Pass", "Battle room"));
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
            CompPlayer compPlayer = new("Blue", new Room("Oak Pass", "Battle room"));
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
