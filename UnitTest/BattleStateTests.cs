using Arcadia_v2;
using Arcadia_v2.Creatures;
using Arcadia_v2.Map;

namespace UnitTest;

public class BattleStateTests
{
    // Verifies that battle damage stops at zero HP instead of allowing negative health values.
    [Fact]
    public void ApplyDamage_ReducesHealthWithoutGoingNegative()
    {
        Animal target = new Animal(id: 99, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 5, level: 1, moves: new[] { MoveData.POUNCE });

        Program.ApplyDamage(target, 10);

        Assert.Equal(0, target.Health);
    }

    // Verifies that negative damage is rejected so the damage helper cannot be used as accidental healing.
    [Fact]
    public void ApplyDamage_NegativeDamage_ThrowsArgumentOutOfRangeException()
    {
        Animal target = new Animal(id: 99, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 5, level: 1, moves: new[] { MoveData.POUNCE });

        Assert.Throws<ArgumentOutOfRangeException>(() => Program.ApplyDamage(target, -1));
    }

    // Verifies that attack moves report damage correctly and clamp the defender's health at zero.
    [Fact]
    public void BattleEngine_UseAttackMove_AppliesDamageAndClampsHealthAtZero()
    {
        Animal attacker = new Animal(id: 98, name: "N_DOG", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        Animal defender = new Animal(id: 99, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 3, level: 1, moves: new[] { MoveData.POUNCE });

        BattleMoveResult result = BattleEngine.UseMove(attacker, defender, new Move("STRONGHIT", ElementType.Base, 10));

        Assert.Equal(BattleMoveResultType.Damage, result.ResultType);
        Assert.Equal("STRONGHIT", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(0, result.TargetHealth);
        Assert.Equal(0, defender.Health);
    }

    // Verifies that healing moves use the selected move's power instead of always using the first move slot.
    [Fact]
    public void HealingMoves_UseSelectedMovePower()
    {
        Animal animal = new Animal(
            id: 99,
            name: "M_CAT",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 75,
            health: 50,
            level: 1,
            moves: new[] { MoveData.POUNCE, MoveData.BLOOM });

        Move selectedMove = animal.Moves[1];

        BattleMoveResult result = BattleEngine.UseMove(animal, animal, selectedMove);

        Assert.Equal(BattleMoveResultType.Healing, result.ResultType);
        Assert.Equal("Bloom", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(60, animal.Health);
    }

    // Verifies that healing moves at full health return the no-effect result without changing health.
    [Fact]
    public void BattleEngine_UseHealingMoveAtFullHealth_ReturnsNoEffect()
    {
        Animal animal = new Animal(id: 99, name: "M_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 30, health: 30, level: 1, moves: new[] { MoveData.BLOOM });

        BattleMoveResult result = BattleEngine.UseMove(animal, animal, MoveData.BLOOM);

        Assert.Equal(BattleMoveResultType.NoEffect, result.ResultType);
        Assert.Equal("Bloom", result.MoveName);
        Assert.Equal(0, result.Amount);
        Assert.Equal(30, result.TargetHealth);
        Assert.Equal(30, animal.Health);
    }

    // Verifies that healing by the exact missing amount reaches full health and reports restoration text.
    [Fact]
    public void HealingMoves_ExactAmountToFull_RestoresHealth()
    {
        Animal animal = new Animal(id: 99, name: "M_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 30, health: 25, level: 1, moves: new[] { MoveData.BLOOM });
        FakeGameIO io = new();

        BattleHelpers.UseHealingMove(io, animal, MoveData.BLOOM.Power);

        Assert.Equal(30, animal.Health);
        Assert.Contains("Health Restored", io.OutputText);
        Assert.DoesNotContain("Nothing happened", io.OutputText);
    }

    // Verifies that player healing move output includes the move used.
    [Fact]
    public void HandlePlayerTurn_HealingMove_PrintsMoveUsed()
    {
        Animal player = new Animal(id: 1, name: "N_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 10, level: 1, moves: new[] { MoveData.BLOOM });
        Animal opponent = new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        FakeGameIO io = new("1");

        BattleHelpers.HandlePlayerTurn(io, player, opponent, string.Empty);

        Assert.Equal(20, player.Health);
        Assert.Contains("You used Bloom", io.OutputText);
        Assert.Contains("N_CAT used Bloom", io.OutputText);
        Assert.Contains("Health Restored", io.OutputText);
    }

    // Verifies that player battle input chooses the fourth move from a 1-based numbered move list.
    [Fact]
    public void HandlePlayerTurn_SelectsMoveByNumber()
    {
        Animal player = new Animal(
            id: 1,
            name: "N_CAT",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 20,
            health: 20,
            level: 1,
            moves: new[]
            {
                new Move("FIRST", ElementType.Base, 1),
                new Move("SECOND", ElementType.Base, 2),
                new Move("THIRD", ElementType.Base, 3),
                new Move("FOURTH", ElementType.Base, 9)
            });
        Animal opponent = new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        FakeGameIO io = new("4");

        BattleHelpers.HandlePlayerTurn(io, player, opponent, string.Empty);

        Assert.Equal(11, opponent.Health);
        Assert.Contains("4. FOURTH", io.OutputText);
        Assert.Contains("You used FOURTH", io.OutputText);
    }

    // Verifies that an out-of-range move number uses the existing invalid move message and re-prompts.
    [Fact]
    public void HandlePlayerTurn_InvalidNumber_RePromptsForMoveNumber()
    {
        Animal player = new Animal(
            id: 1,
            name: "N_CAT",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 20,
            health: 20,
            level: 1,
            moves: new[]
            {
                new Move("FIRST", ElementType.Base, 1),
                new Move("SECOND", ElementType.Base, 7)
            });
        Animal opponent = new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        FakeGameIO io = new("5", "2");

        BattleHelpers.HandlePlayerTurn(io, player, opponent, string.Empty);

        int promptCount = io.OutputText.Split("Enter your move number.").Length - 1;
        Assert.Equal(2, promptCount);
        Assert.Contains("5 is an invalid move.", io.OutputText);
        Assert.Equal(13, opponent.Health);
        Assert.Contains("You used SECOND", io.OutputText);
    }

    // Verifies that an invalid then no response re-prompts and leaves the party order unchanged.
    [Fact]
    public void HandlePlayerDefeatedAnimal_InvalidThenNo_RePromptsWithoutSwapping()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        FakeGameIO io = new("maybe", "no");

        PlayerDefeatedAnimalResult result = BattleHelpers.HandlePlayerDefeatedAnimal(io, player, "Would you like to switch animals?");

        int promptCount = io.OutputText.Split("Would you like to switch animals?").Length - 1;
        Assert.Equal(PlayerDefeatedAnimalResult.DefeatedNoSwitch, result);
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.DoesNotContain("defeated.", io.OutputText);
        Assert.Equal("N_CAT", player.AnimalInventory[0].Name);
        Assert.Equal("N_LION", player.AnimalInventory[1].Name);
    }

    // Verifies that an invalid then yes response re-prompts and swaps the selected party creatures.
    [Fact]
    public void HandlePlayerDefeatedAnimal_InvalidThenYes_RePromptsAndSwapsAnimals()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        FakeGameIO io = new("maybe", "yes", "n_cat", "n_lion");

        PlayerDefeatedAnimalResult result = BattleHelpers.HandlePlayerDefeatedAnimal(io, player, "Would you like to switch animals?");

        int promptCount = io.OutputText.Split("Would you like to switch animals?").Length - 1;
        Assert.Equal(PlayerDefeatedAnimalResult.Switched, result);
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.DoesNotContain("defeated.", io.OutputText);
        Assert.Equal("N_LION", player.AnimalInventory[0].Name);
        Assert.Equal("N_CAT", player.AnimalInventory[1].Name);
    }

    // Verifies that a two-creature party auto-switches to the healthy backup without prompting.
    [Fact]
    public void HandlePlayerDefeatedAnimal_WithTwoAnimals_AutoSwitchesToHealthyAnimalWithoutPrompting()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        FakeGameIO io = new();

        PlayerDefeatedAnimalResult result = BattleHelpers.HandlePlayerDefeatedAnimal(io, player, "Would you like to switch animals?");

        Assert.Equal(PlayerDefeatedAnimalResult.Switched, result);
        Assert.Equal("N_LION", player.AnimalInventory[0].Name);
        Assert.Equal("N_CAT", player.AnimalInventory[1].Name);
        Assert.DoesNotContain("defeated.", io.OutputText);
        Assert.DoesNotContain("Would you like to switch animals?", io.OutputText);
    }

    // Verifies that a two-creature party does not auto-switch when both creatures have been defeated.
    [Fact]
    public void HandlePlayerDefeatedAnimal_WithTwoDefeatedAnimals_DoesNotAutoSwitch()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;
        FakeGameIO io = new();

        PlayerDefeatedAnimalResult result = BattleHelpers.HandlePlayerDefeatedAnimal(io, player, "Would you like to switch animals?");

        Assert.Equal(PlayerDefeatedAnimalResult.DefeatedNoSwitch, result);
        Assert.Equal("N_CAT", player.AnimalInventory[0].Name);
        Assert.Equal("N_LION", player.AnimalInventory[1].Name);
        Assert.DoesNotContain("defeated.", io.OutputText);
        Assert.DoesNotContain("Would you like to switch animals?", io.OutputText);
    }

    // Verifies that the defeat handler reports no action when the active creature is still healthy.
    [Fact]
    public void HandlePlayerDefeatedAnimal_WhenAnimalIsHealthy_ReturnsNotDefeated()
    {
        Player player = CreateTwoAnimalPlayer();
        FakeGameIO io = new();

        PlayerDefeatedAnimalResult result = BattleHelpers.HandlePlayerDefeatedAnimal(io, player, "Would you like to switch animals?");

        Assert.Equal(PlayerDefeatedAnimalResult.NotDefeated, result);
        Assert.Equal(string.Empty, io.OutputText);
    }

    // Verifies that defeat detection returns true for zero health.
    [Fact]
    public void BattleEngine_IsDefeated_ReturnsTrueForZeroHealth()
    {
        Animal animal = new Animal(id: 99, name: "N_CAT", element: AnimalElement.Nature, speed: 5, baseHealth: 20, health: 0, level: 1, moves: new[] { MoveData.POUNCE });

        Assert.True(BattleEngine.IsDefeated(animal));
    }

    // Verifies that the engine reports no usable party creatures when all health values are zero or below.
    [Fact]
    public void BattleEngine_HasUsableAnimals_ReturnsFalseWhenNoPartyAnimalsHaveHealth()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;

        Assert.False(BattleEngine.HasUsableAnimals(player));
        Assert.Equal(-1, BattleEngine.GetNextHealthyAnimalIndex(player));
    }

    // Verifies that the next-healthy lookup returns the first healthy creature at or after the requested start index.
    [Fact]
    public void BattleEngine_GetNextHealthyAnimalIndex_ReturnsFirstHealthyAnimalAtOrAfterStartIndex()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "N_DOG", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.THORNWRAP }));
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;

        Assert.True(BattleEngine.HasUsableAnimals(player));
        Assert.Equal(2, BattleEngine.GetNextHealthyAnimalIndex(player));
        Assert.Equal(2, BattleEngine.GetNextHealthyAnimalIndex(player, startIndex: 1));
    }

    // Verifies that auto-swap eligibility is limited to parties with exactly two creatures.
    [Fact]
    public void BattleEngine_CanAutoSwapTwoAnimalParty_ReturnsTrueOnlyForTwoAnimalParty()
    {
        Assert.True(BattleEngine.CanAutoSwapTwoAnimalParty(CreateTwoAnimalPlayer()));
        Assert.False(BattleEngine.CanAutoSwapTwoAnimalParty(CreateThreeAnimalPlayer()));
    }

    // Verifies that the helper returns the opposite party slot for a two-creature party.
    [Fact]
    public void BattleEngine_GetOnlyOtherAnimalIndex_ReturnsOtherIndexForTwoAnimalParty()
    {
        Player player = CreateTwoAnimalPlayer();

        Assert.Equal(1, BattleEngine.GetOnlyOtherAnimalIndex(player, player.AnimalInventory[0]));
        Assert.Equal(0, BattleEngine.GetOnlyOtherAnimalIndex(player, player.AnimalInventory[1]));
    }

    // Verifies that auto-switching fails when the only replacement creature has also been defeated.
    [Fact]
    public void BattleEngine_TryAutoSwitchTwoAnimalParty_ReturnsFalseWhenReplacementIsDefeated()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;

        bool switched = BattleEngine.TryAutoSwitchTwoAnimalParty(player, player.AnimalInventory[0]);

        Assert.False(switched);
        Assert.Equal("N_CAT", player.AnimalInventory[0].Name);
        Assert.Equal("N_LION", player.AnimalInventory[1].Name);
    }

    // Verifies that catching a wild creature adds it to the party and removes it from the room encounter list.
    [Fact]
    public void BattleEngine_TryCatchWildAnimal_AddsAnimalToPartyAndRemovesEncounter()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 0, level: 1, moves: new[] { MoveData.RAD_BURST });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        bool caught = BattleEngine.TryCatchWildAnimal(player, wildAnimal);

        Assert.True(caught);
        Assert.Contains(wildAnimal, player.AnimalInventory);
        Assert.DoesNotContain(wildAnimal, player.CurrentRoom.EncounterAnimals);
    }

    // Verifies that catching fails when the player's party is already full.
    [Fact]
    public void BattleEngine_TryCatchWildAnimal_ReturnsFalseWhenPartyIsFull()
    {
        Player player = CreateFullAnimalPartyPlayer();
        Animal wildAnimal = new Animal(id: 7, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 0, level: 1, moves: new[] { MoveData.RAD_BURST });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        bool caught = BattleEngine.TryCatchWildAnimal(player, wildAnimal);

        Assert.False(caught);
        Assert.DoesNotContain(wildAnimal, player.AnimalInventory);
        Assert.Contains(wildAnimal, player.CurrentRoom.EncounterAnimals);
    }

    // Verifies that releasing one party creature while catching a wild one swaps the room and party ownership correctly.
    [Fact]
    public void BattleEngine_ReleaseAnimalAndCatchWildAnimal_SwapsPartyAndEncounterAnimals()
    {
        Player player = CreateFullAnimalPartyPlayer();
        Animal animalToRelease = player.AnimalInventory[0];
        Animal wildAnimal = new Animal(id: 7, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 0, level: 1, moves: new[] { MoveData.RAD_BURST });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        BattleEngine.ReleaseAnimalAndCatchWildAnimal(player, animalToRelease, wildAnimal);

        Assert.DoesNotContain(animalToRelease, player.AnimalInventory);
        Assert.Contains(wildAnimal, player.AnimalInventory);
        Assert.Contains(animalToRelease, player.CurrentRoom.EncounterAnimals);
        Assert.DoesNotContain(wildAnimal, player.CurrentRoom.EncounterAnimals);
        Assert.Equal(animalToRelease.BaseHealth, animalToRelease.Health);
    }

    // Verifies that opponent move choice can be controlled in tests without relying on Random.Shared.
    [Fact]
    public void HandleOpponentTurn_UsesInjectedMoveSelector()
    {
        Animal opponent = new Animal(
            id: 1,
            name: "N_LION",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 20,
            health: 20,
            level: 1,
            moves: new[] { new Move("WEAK", ElementType.Base, 1), new Move("STRONG", ElementType.Base, 7) });
        Animal player = new Animal(id: 2, name: "N_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        FakeGameIO io = new();

        BattleHelpers.HandleOpponentTurn(io, opponent, player, "Opponent Move", string.Empty, new FixedMoveSelector(opponent.Moves[1]));

        Assert.Equal(13, player.Health);
        Assert.Contains("N_LION used STRONG", io.OutputText);
    }

    // Verifies that opponent healing move output includes the move used.
    [Fact]
    public void HandleOpponentTurn_HealingMove_PrintsMoveUsed()
    {
        Animal opponent = new Animal(
            id: 1,
            name: "N_LION",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 20,
            health: 10,
            level: 1,
            moves: new[] { MoveData.BLOOM });
        Animal player = new Animal(id: 2, name: "N_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 20, health: 20, level: 1, moves: new[] { MoveData.POUNCE });
        FakeGameIO io = new();

        BattleHelpers.HandleOpponentTurn(io, opponent, player, "Opponent Move", string.Empty, new FixedMoveSelector(MoveData.BLOOM));

        Assert.Equal(20, opponent.Health);
        Assert.Contains("N_LION used Bloom", io.OutputText);
        Assert.Contains("Health Restored", io.OutputText);
    }

    // Verifies that letting a wild creature run away removes it from the room encounter list.
    [Fact]
    public void BattleEngine_LetWildAnimalRunAway_RemovesEncounterAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 0, level: 1, moves: new[] { MoveData.RAD_BURST });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        BattleEngine.LetWildAnimalRunAway(player, wildAnimal);

        Assert.DoesNotContain(wildAnimal, player.CurrentRoom.EncounterAnimals);
    }

    // Verifies that creating a wild battle uses the player's current lead and the supplied wild opponent.
    [Fact]
    public void BattleState_CreateWildBattle_UsesCurrentPlayerLeadAndWildAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.RAD_BURST });

        BattleState battleState = BattleState.CreateWildBattle(player, wildAnimal);

        Assert.Equal("N_CAT", battleState.PlayerAnimal.Name);
        Assert.Equal("N_BIRD", battleState.OpponentAnimal.Name);
        Assert.False(battleState.IsOver);
    }

    // Verifies that creating a wild battle skips defeated lead creatures and starts with the next healthy one.
    [Fact]
    public void BattleState_CreateWildBattle_SkipsDefeatedPlayerLeadAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "N_DOG", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.THORNWRAP }));
        player.AnimalInventory[0].Health = 0;
        player.AnimalInventory[1].Health = 0;
        Animal wildAnimal = new Animal(id: 4, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.RAD_BURST });

        BattleState battleState = BattleState.CreateWildBattle(player, wildAnimal);

        Assert.Equal(2, battleState.PlayerActiveIndex);
        Assert.Equal("N_DOG", battleState.PlayerAnimal.Name);
        Assert.False(battleState.IsOver);
    }

    // Verifies that trainer battles can switch the opponent to the next healthy creature on demand.
    [Fact]
    public void BattleState_TrySwitchOpponentToNextHealthyAnimal_UpdatesActiveOpponent()
    {
        Player player = CreateTwoAnimalPlayer();
        CompPlayer opponent = new("Opponent", new Map().StartRoom);
        opponent.SetBattleTeam(new[]
        {
            new Animal(id: 3, name: "N_BIRD", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 0, level: 1, moves: new[] { MoveData.RAD_BURST }),
            new Animal(id: 4, name: "T_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.STATIC_CLAW })
        });
        BattleState battleState = BattleState.CreateTrainerBattle(player, opponent);

        bool switched = battleState.TrySwitchOpponentToNextHealthyAnimal(startIndex: 1);

        Assert.True(switched);
        Assert.Equal(1, battleState.OpponentActiveIndex);
        Assert.Equal("T_CAT", battleState.OpponentAnimal.Name);
    }

    // Verifies that cloning creates a separate animal and move list so later mutations do not leak back to the original.
    [Fact]
    public void AnimalClone_CreatesIndependentMoveObjects()
    {
        Animal original = new Animal(
            id: 99,
            name: "N_CAT",
            element: AnimalElement.Nature,
            speed: 8,
            baseHealth: 30,
            health: 30,
            level: 4,
            moves: new[] { new Move("TACKLE", ElementType.Base, 5) });

        Animal clone = original.Clone();
        clone.Health = 1;

        Assert.Equal("N_CAT", original.Name);
        Assert.Equal("TACKLE", original.Moves[0].MoveName);
        Assert.Equal(5, original.Moves[0].MovePower);
        Assert.Equal(30, original.Health);
    }

    // Verifies that a trainer can rebuild a fresh battle roster from templates after a previous battle changed live HP values.
    [Fact]
    public void PrepareForBattle_RebuildsFreshTrainerRosterFromTemplate()
    {
        List<Animal> animals = GameData.CreateAnimals();
        Room startingRoom = new Map().StartRoom;
        CompPlayer gymLeader = new CompPlayer("Trainer", startingRoom);
        gymLeader.SetBattleTeam(new[] { animals[3], animals[14] });

        gymLeader.AnimalInventory[0].Health = 1;
        gymLeader.AnimalInventory[1].Health = 2;
        Animal firstBattleLead = gymLeader.AnimalInventory[0];

        gymLeader.PrepareForBattle();

        Assert.Equal(gymLeader.BattleTeamTemplate[0].BaseHealth, gymLeader.AnimalInventory[0].Health);
        Assert.Equal(gymLeader.BattleTeamTemplate[1].BaseHealth, gymLeader.AnimalInventory[1].Health);
        Assert.NotSame(firstBattleLead, gymLeader.AnimalInventory[0]);
    }

    private static Player CreateTwoAnimalPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "N_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.VENOM_FANG }));
        player.AddAnimal(new Animal(id: 2, name: "N_LION", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.DEEPSEA_RUPTURE }));
        return player;
    }

    private static Player CreateThreeAnimalPlayer()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "N_DOG", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.THORNWRAP }));
        return player;
    }

    private static Player CreateFullAnimalPartyPlayer()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AddAnimal(new Animal(id: 4, name: "M_TURTLE", element: AnimalElement.Mystic, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.CURRENT_RUSH }));
        player.AddAnimal(new Animal(id: 5, name: "N_CUB", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.THORNWRAP }));
        player.AddAnimal(new Animal(id: 6, name: "T_CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, health: 10, level: 1, moves: new[] { MoveData.STATIC_CLAW }));
        return player;
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
