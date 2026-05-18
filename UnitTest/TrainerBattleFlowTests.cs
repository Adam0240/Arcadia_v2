using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest;

public class TrainerBattleFlowTests
{
    // Checks that trainer battles send out the next healthy opponent creature after the active one faints.
    [Fact]
    public void Run_WhenOpponentAnimalFaints_SendsOutNextHealthyAnimal()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("Rival sent out SECOND", io.OutputText);
    }

    // Checks that defeating an entire opponent team marks the trainer defeated and awards the badge.
    [Fact]
    public void Run_WhenOpponentTeamFaints_CompletesBattleAndAwardsBadge()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.True(opponent.Defeated);
        Assert.Contains("Test Badge", player.Badges);
        Assert.Contains("Rival defeated.", io.OutputText);
    }

    // Checks that trainer battles rebuild a fresh opponent battle team before the fight begins.
    [Fact]
    public void Run_PreparesFreshTrainerTeamBeforeBattle()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        opponent.AnimalInventory[0].Health = 1;
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("The opponents FIRST's health is at: 5", io.OutputText);
    }

    // Checks that trainer battles start with the next healthy player creature when the lead has fainted.
    [Fact]
    public void Run_WhenPlayerLeadIsFainted_StartsWithNextHealthyPokemon()
    {
        Player player = CreatePlayer();
        player.AnimalInventory[0].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("You sent out BACKUP", io.OutputText);
    }

    // Checks that trainer battles stop immediately with the party-fainted message when no usable player creatures remain.
    [Fact]
    public void Run_WhenAllPlayerAnimalsAreFainted_PrintsPartyFaintedMessage()
    {
        Player player = CreatePlayer();
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new();

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("All animals in your party are fainted.", io.OutputText);
        Assert.DoesNotContain("You sent out", io.OutputText);
    }

    // Checks that trainer battle flow can use an injected selector for deterministic opponent move choice.
    [Fact]
    public void Run_UsesInjectedMoveSelectorForOpponentTurns()
    {
        Player player = new("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "LEAD", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { new Move("WEAKHIT", MoveType.Normal, 1) }));
        CompPlayer opponent = new("Rival", new Map().StartRoom);
        opponent.AddBadge("Test Badge");
        opponent.SetBattleTeam(new[]
        {
            new Animal(id: 3, name: "FIRST", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 20, level: 1, moves: new[] { new Move("WEAK", MoveType.Normal, 1), new Move("STRONG", MoveType.Normal, 5) })
        });
        FakeGameIO io = new("weakhit", "no");

        TrainerBattleFlow.Run(io, player, opponent, new FixedMoveSelector(new Move("STRONG", MoveType.Normal, 5)));

        Assert.Equal(0, player.AnimalInventory[0].Health);
        Assert.Contains("FIRST used STRONG", io.OutputText);
    }

    private static Player CreatePlayer()
    {
        Player player = new("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "LEAD", element: AnimalElement.Nature, speed: 5, baseHealth: 50, health: 50, level: 1, moves: new[] { new Move("STRONGHIT", MoveType.Normal, 10) }));
        player.AddAnimal(new Animal(id: 2, name: "BACKUP", element: AnimalElement.Nature, speed: 5, baseHealth: 50, health: 50, level: 1, moves: new[] { new Move("STRONGHIT", MoveType.Normal, 10) }));
        return player;
    }

    private static CompPlayer CreateOpponent()
    {
        CompPlayer opponent = new("Rival", new Map().StartRoom);
        opponent.AddBadge("Test Badge");
        opponent.SetBattleTeam(new[]
        {
            new Animal(id: 3, name: "FIRST", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { MoveData.Splash }),
            new Animal(id: 4, name: "SECOND", element: AnimalElement.Nature, speed: 5, baseHealth: 5, health: 5, level: 1, moves: new[] { MoveData.Splash })
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
