using Arcadia_v2;
using Arcadia_v2.Creatures;

namespace UnitTest;

public class WildBattleFlowTests
{
    // Checks that catching a defeated wild animal adds it to the party and removes it from the room encounter list.
    [Fact]
    public void HandleWildBattle_CatchAnimal_AddsAnimalToPartyAndRemovesEncounter()
    {
        GameState gameState = CreateRoadOneWildBattle();
        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 1;
        FakeGameIO io = new("4", "yes");

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.Contains(wildAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.Contains($"You caught {wildAnimal.Name}!", io.OutputText);
    }

    // Checks that a full party plus an invalid release prompt answer causes a re-prompt before letting the wild creature run away.
    [Fact]
    public void HandleWildBattle_FullPartyInvalidReleaseAnswer_RePromptsAndLetsWildAnimalRunAway()
    {
        GameState gameState = CreateRoadOneWildBattle();
        AddAnimalsUntilPartyIsFull(gameState.MainPlayer);

        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 1;

        FakeGameIO io = new("4", "yes", "maybe", "no");

        WildBattleFlow.HandleWildBattle(io, gameState);

        int releasePromptCount = io.OutputText.Split("Would you like to release an animal? -- (yes or no)").Length - 1;
        Assert.Equal(2, releasePromptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Contains($"{wildAnimal.Name} ran away!", io.OutputText);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
    }

    // Checks that releasing a party creature while full catches the wild creature and returns the released one to the room.
    [Fact]
    public void HandleWildBattle_FullPartyReleaseAnimal_CatchesWildAnimalAndReturnsReleasedAnimalToRoom()
    {
        GameState gameState = CreateRoadOneWildBattle();
        AddAnimalsUntilPartyIsFull(gameState.MainPlayer);
        Animal releasedAnimal = gameState.MainPlayer.AnimalInventory[0];
        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 1;
        FakeGameIO io = new("4", "yes", "yes", releasedAnimal.Name);

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(releasedAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.Contains(wildAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.Contains(releasedAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.Equal(releasedAnimal.BaseHealth, releasedAnimal.Health);
        Assert.Contains($"You caught {wildAnimal.Name}!", io.OutputText);
    }

    // Checks that declining to catch a defeated wild animal removes it from the room encounter list.
    [Fact]
    public void HandleWildBattle_NoCatch_RemovesEncounterAnimal()
    {
        GameState gameState = CreateRoadOneWildBattle();
        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 1;
        FakeGameIO io = new("4", "no");

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.Contains($"{wildAnimal.Name} ran away!", io.OutputText);
    }

    // Checks that wild battles do not start when every animal in the player's party has been defeated.
    [Fact]
    public void HandleWildBattle_WhenAllPlayerAnimalsAreDefeated_PrintsPartyDefeatedMessage()
    {
        GameState gameState = CreateRoadOneWildBattle();
        foreach (Animal animal in gameState.MainPlayer.AnimalInventory)
        {
            animal.Health = 0;
        }

        FakeGameIO io = new();

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.Contains("All animals in your party are defeated.", io.OutputText);
        Assert.DoesNotContain("A wild", io.OutputText);
    }

    // Checks that wild battles print the player's defeated message once when no switch happens.
    [Fact]
    public void HandleWildBattle_WhenPlayerAnimalIsDefeatedWithoutSwitch_PrintsDefeatedMessageOnce()
    {
        GameState gameState = CreateRoadOneWildBattle();
        Animal playerAnimal = new(
            id: 99,
            name: "N_CAT",
            element: AnimalElement.Nature,
            speed: 5,
            baseHealth: 5,
            health: 5,
            level: 1,
            moves: new[] { new Move("WEAKHIT", ElementType.Base, 1) });
        gameState.MainPlayer.RestoreAnimalInventory(new[] { playerAnimal });
        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 20;
        FakeGameIO io = new("1", "no");

        WildBattleFlow.HandleWildBattle(io, gameState, new FixedMoveSelector(new Move("STRONG", ElementType.Base, 5)));

        Assert.Equal(0, playerAnimal.Health);
        Assert.Equal(1, io.OutputText.Split("N_CAT defeated.").Length - 1);
        Assert.Equal(1, io.OutputText.Split("Battle Lost, all animals in your party are defeated").Length - 1);
    }

    private static GameState CreateRoadOneWildBattle()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 1"));
        return gameState;
    }

    private static void AddAnimalsUntilPartyIsFull(Player player)
    {
        IReadOnlyList<Animal> animals = GameData.CreateAnimals();

        for (int i = player.AnimalInventory.Count; i < 6; ++i)
        {
            player.AddAnimal(animals[i + 1]);
        }
    }

    private sealed class FixedMoveSelector : IBattleMoveSelector
    {
        private readonly Move mMove;

        public FixedMoveSelector(Move move)
        {
            mMove = move;
        }

        public Move SelectMove(Animal animal)
        {
            return mMove;
        }
    }
}
