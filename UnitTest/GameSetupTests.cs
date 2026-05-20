using Arcadia_v2;

namespace UnitTest
{
    public class GameSetupTests
    {
        // Checks that the initial player party is assigned by intended species and element.
        [Fact]
        public void CreateForLoad_AssignsNamedStarterAnimals()
        {
            GameState gameState = GameSetup.CreateForLoad();

            Assert.Equal("Nature Cat", gameState.MainPlayer.AnimalInventory[0].Name);
            Assert.Equal("Nature Lion", gameState.MainPlayer.AnimalInventory[1].Name);
        }

        // Checks that trainer rosters are assigned by intended species and element.
        [Fact]
        public void CreateForLoad_AssignsNamedTrainerTeams()
        {
            GameState gameState = GameSetup.CreateForLoad();

            AssertTeam(gameState.GymLeader1, "Nature Dog", "Nature Bear");
            AssertTeam(gameState.GymLeader2, "Nature Horse", "Nature Bird");
            AssertTeam(gameState.GymLeader3, "Nature Eagle", "Nature Dragon");
            AssertTeam(gameState.GymLeader4, "Nature Cub", "Mystic Cat");
            AssertTeam(gameState.ArcadiaChampion, "Nature Wolf", "Nature Tortoise", "Nature Stallion", "Mystic Lion");
        }

        private static void AssertTeam(CompPlayer trainer, params string[] expectedAnimalNames)
        {
            Assert.Equal(expectedAnimalNames, trainer.BattleTeamTemplate.Select(animal => animal.Name));
        }
    }
}
