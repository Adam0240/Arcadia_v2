using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest
{
    public class PlayerTest
    {
        // Checks that a player stores the provided name and starting room.
        [Fact]
        public void Constructor_SetsNameAndStartingRoom()
        {
            Room startRoom = new("Professor's Lab", "Starting room");
            Player player = new("Red", startRoom);

            Assert.Equal("Red", player.Name);
            Assert.Same(startRoom, player.CurrentRoom);
        }

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

        // Checks that moving a player updates the current room to the destination room.
        [Fact]
        public void MoveTo_ValidRoom_UpdatesCurrentRoom()
        {
            Room startRoom = new("Professor's Lab", "Starting room");
            Room nextRoom = new("Ikena", "Next room");
            Player player = new("Red", startRoom);

            player.MoveTo(nextRoom);

            Assert.Same(nextRoom, player.CurrentRoom);
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

        // Checks that the badge display returns the empty-state message when no badges exist.
        [Fact]
        public void GetBadgeDisplay_NoBadges_ReturnsEmptyMessage()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            Assert.Equal("You have no badges!", player.GetBadgeDisplay());
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

        // Checks that adding a Pokemon stores it in the player's inventory.
        [Fact]
        public void AddPokemon_AddsPokemonToInventory()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            Pokemon pikachu = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, new[] { MoveData.Thunderbolt });

            player.AddPokemon(pikachu);

            Assert.Single(player.PokemonInventory);
            Assert.Same(pikachu, player.PokemonInventory[0]);
        }

        // Checks that removing a stored Pokemon returns true and removes it from the inventory.
        [Fact]
        public void RemovePokemon_StoredPokemon_RemovesPokemon()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            Pokemon pikachu = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, new[] { MoveData.Thunderbolt });
            player.AddPokemon(pikachu);

            bool removed = player.RemovePokemon(pikachu);

            Assert.True(removed);
            Assert.Empty(player.PokemonInventory);
        }

        // Checks that an empty Pokemon inventory returns the expected empty-state display text.
        [Fact]
        public void GetPokemonInventoryDisplay_EmptyInventory_ReturnsEmptyMessage()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));

            Assert.Equal("Inventory is Empty! :'( ", player.GetPokemonInventoryDisplay());
        }

        // Checks that the Pokemon inventory display includes each stored Pokemon and its health.
        [Fact]
        public void GetPokemonInventoryDisplay_WithPokemon_ReturnsFormattedInventory()
        {
            Player player = new("Red", new Room("Professor's Lab", "Starting room"));
            Pokemon pikachu = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, new[] { MoveData.Thunderbolt });
            Pokemon squirtle = new(7, "SQUIRTLE", PokemonType.Water, 10, 40, 30, 5, new[] { MoveData.WaterGun });
            player.AddPokemon(pikachu);
            player.AddPokemon(squirtle);

            Assert.Equal("Inventory List:\nPIKACHU Health: 35\nSQUIRTLE Health: 30", player.GetPokemonInventoryDisplay());
        }

        // Checks that setting a computer player's battle team clones the template Pokemon instead of reusing the same instances.
        [Fact]
        public void SetBattleTeam_ClonesTemplatePokemon()
        {
            CompPlayer compPlayer = new("Blue", new Room("Oak Pass", "Battle room"));
            Pokemon templatePokemon = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, new[] { MoveData.Thunderbolt });

            compPlayer.SetBattleTeam(new[] { templatePokemon });

            Assert.Single(compPlayer.BattleTeamTemplate);
            Assert.Single(compPlayer.PokemonInventory);
            Assert.NotSame(templatePokemon, compPlayer.BattleTeamTemplate[0]);
            Assert.NotSame(compPlayer.BattleTeamTemplate[0], compPlayer.PokemonInventory[0]);
            Assert.Equal(templatePokemon.Name, compPlayer.BattleTeamTemplate[0].Name);
        }

        // Checks that preparing a computer player for battle rebuilds the active inventory from the stored team template.
        [Fact]
        public void PrepareForBattle_RebuildsInventoryFromTemplate()
        {
            CompPlayer compPlayer = new("Blue", new Room("Oak Pass", "Battle room"));
            Pokemon templatePokemon = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, new[] { MoveData.Thunderbolt });
            compPlayer.SetBattleTeam(new[] { templatePokemon });
            compPlayer.PokemonInventory[0].Health = 5;

            compPlayer.PrepareForBattle();

            Assert.Single(compPlayer.PokemonInventory);
            Assert.NotSame(compPlayer.BattleTeamTemplate[0], compPlayer.PokemonInventory[0]);
            Assert.Equal(compPlayer.BattleTeamTemplate[0].Health, compPlayer.PokemonInventory[0].Health);
        }
    }
}
