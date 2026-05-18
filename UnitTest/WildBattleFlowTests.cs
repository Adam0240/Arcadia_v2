using Arcadia_v2;

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
        FakeGameIO io = new("tackle", "yes");

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

        FakeGameIO io = new("tackle", "yes", "maybe", "no");

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
        FakeGameIO io = new("tackle", "yes", "yes", releasedAnimal.Name);

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(releasedAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.Contains(wildAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.Contains(releasedAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.Equal(20, releasedAnimal.Health);
        Assert.Contains($"You caught {wildAnimal.Name}!", io.OutputText);
    }

    // Checks that declining to catch a defeated wild animal removes it from the room encounter list.
    [Fact]
    public void HandleWildBattle_NoCatch_RemovesEncounterAnimal()
    {
        GameState gameState = CreateRoadOneWildBattle();
        Animal wildAnimal = gameState.MainPlayer.CurrentRoom.EncounterAnimals[0];
        wildAnimal.Health = 1;
        FakeGameIO io = new("tackle", "no");

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.AnimalInventory);
        Assert.DoesNotContain(wildAnimal, gameState.MainPlayer.CurrentRoom.EncounterAnimals);
        Assert.Contains($"{wildAnimal.Name} ran away!", io.OutputText);
    }

    // Checks that wild battles do not start when every animal in the player's party has fainted.
    [Fact]
    public void HandleWildBattle_WhenAllPlayerAnimalsAreFainted_PrintsPartyFaintedMessage()
    {
        GameState gameState = CreateRoadOneWildBattle();
        foreach (Animal animal in gameState.MainPlayer.AnimalInventory)
        {
            animal.Health = 0;
        }

        FakeGameIO io = new();

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.Contains("All animals in your party are fainted.", io.OutputText);
        Assert.DoesNotContain("A wild", io.OutputText);
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
}
