using Arcadia_v2;
using Arcadia_v2.Creatures;
using Arcadia_v2.Map;

namespace UnitTest;

public class TrainerBattleFlowTests
{
    // Checks that trainer battles send out the next healthy opponent creature after the active one is defeated.
    [Fact]
    public void Run_WhenOpponentAnimalIsDefeated_SendsOutNextHealthyAnimal()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("1", "1");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("Rival sent out N_WOLF", io.OutputText);
    }

    // Checks that defeating an entire opponent team marks the trainer defeated and awards the star fragment.
    [Fact]
    public void Run_WhenOpponentTeamIsDefeated_CompletesBattleAndAwardsStarFragment()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("1", "1");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.True(opponent.Defeated);
        Assert.Contains("Test Star Fragment", player.StarFragments);
        Assert.Contains("Rival defeated.", io.OutputText);
    }

    // Checks that defeating a guardian adds full bond for the awarded fragment's element.
    [Fact]
    public void Run_WhenGuardianIsDefeated_AddsFullBondForRewardElement()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        opponent.RestoreStarFragments(new[] { "Nature Star Fragment" });
        FakeGameIO io = new("1", "1");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Equal(100, player.GetBond(AnimalElement.Nature));
    }

    // Checks that trainer battles rebuild a fresh opponent battle team before the fight begins.
    [Fact]
    public void Run_PreparesFreshTrainerTeamBeforeBattle()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        opponent.AnimalInventory[0].Health = 1;
        FakeGameIO io = new("1", "1");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("The opponents N_DOG's health is at: 5", io.OutputText);
    }

    // Checks that trainer battles start with the next healthy player creature when the lead has been defeated.
    [Fact]
    public void Run_WhenPlayerLeadIsDefeated_StartsWithNextHealthyAnimal()
    {
        Player player = CreatePlayer();
        player.AnimalInventory[0].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("1", "1");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("You sent out N_LION", io.OutputText);
    }

    // Checks that trainer battles stop immediately with the party-defeated message when no usable player creatures remain.
    [Fact]
    public void Run_WhenAllPlayerAnimalsAreDefeated_PrintsPartyDefeatedMessage()
    {
        Player player = CreatePlayer();
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new();

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("All animals in your party are defeated.", io.OutputText);
        Assert.DoesNotContain("You sent out", io.OutputText);
    }

    // Checks that trainer battle flow can use an injected selector for deterministic opponent move choice.
    [Fact]
    public void Run_UsesInjectedMoveSelectorForOpponentTurns()
    {
        Player player = new("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { new Move("WEAKHIT", ElementType.Base, 1) }));
        CompPlayer opponent = new("Rival", new Map().StartRoom);
        opponent.AddStarFragment("Test Star Fragment");
        opponent.SetBattleTeam(new[]
        {
            new Animal(id: 3, name: "N_DOG", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 20, level: 1, moves: new[] { new Move("WEAK", ElementType.Base, 1), new Move("STRONG", ElementType.Base, 5) })
        });
        FakeGameIO io = new("1", "no");

        TrainerBattleFlow.Run(io, player, opponent, new FixedMoveSelector(new Move("STRONG", ElementType.Base, 5)));

        Assert.Equal(0, player.AnimalInventory[0].Health);
        Assert.Contains("N_DOG used STRONG", io.OutputText);
        Assert.Equal(1, io.OutputText.Split("N_CAT defeated.").Length - 1);
        Assert.Equal(1, io.OutputText.Split("Battle Lost, all animals in your party are defeated").Length - 1);
    }

    private static Player CreatePlayer()
    {
        Player player = new("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 50, health: 50, level: 1, moves: new[] { new Move("STRONGHIT", ElementType.Base, 10) }));
        player.AddAnimal(new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 5, baseHealth: 50, health: 50, level: 1, moves: new[] { new Move("STRONGHIT", ElementType.Base, 10) }));
        return player;
    }

    private static CompPlayer CreateOpponent()
    {
        CompPlayer opponent = new("Rival", new Map().StartRoom);
        opponent.AddStarFragment("Test Star Fragment");
        opponent.SetBattleTeam(new[]
        {
            new Animal(id: 3, name: "N_DOG", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { MoveData.HEAD_BASH }),
            new Animal(id: 4, name: "N_WOLF", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { MoveData.HEAD_BASH })
        });
        return opponent;
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
