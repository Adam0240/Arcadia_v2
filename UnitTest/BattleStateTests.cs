using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest;

public class BattleStateTests
{
    // Verifies that battle damage stops at zero HP instead of allowing negative currentHealth values.
    [Fact]
    public void ApplyDamage_ReducesHealthWithoutGoingNegative()
    {
        Animal target = new Animal(id: 99, name: "TESTMON", element: AnimalElement.Nature, speed: 5, baseHealth: 20, currentHealth: 5, level: 1, moves: new[] { MoveData.Tackle });

        Program.ApplyDamage(target, 10);

        Assert.Equal(0, target.CurrentHealth);
    }

    // Verifies that negative damage is rejected so the damage helper cannot be used as accidental healing.
    [Fact]
    public void ApplyDamage_NegativeDamage_ThrowsArgumentOutOfRangeException()
    {
        Animal target = new Animal(id: 99, name: "TESTMON", element: AnimalElement.Nature, speed: 5, baseHealth: 20, currentHealth: 5, level: 1, moves: new[] { MoveData.Tackle });

        Assert.Throws<ArgumentOutOfRangeException>(() => Program.ApplyDamage(target, -1));
    }

    // Verifies that attack moves report damage correctly and clamp the defender's currentHealth at zero.
    [Fact]
    public void BattleEngine_UseAttackMove_AppliesDamageAndClampsHealthAtZero()
    {
        Animal attacker = new Animal(id: 98, name: "ATTACKMON", element: AnimalElement.Nature, speed: 5, baseHealth: 20, currentHealth: 20, level: 1, moves: new[] { MoveData.Tackle });
        Animal defender = new Animal(id: 99, name: "DEFENDMON", element: AnimalElement.Nature, speed: 5, baseHealth: 20, currentHealth: 3, level: 1, moves: new[] { MoveData.Tackle });

        BattleMoveResult result = BattleEngine.UseMove(attacker, defender, new Move("STRONGHIT", MoveType.Neutral, 10));

        Assert.Equal(BattleMoveResultType.Damage, result.ResultType);
        Assert.Equal("STRONGHIT", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(0, result.TargetHealth);
        Assert.Equal(0, defender.CurrentHealth);
    }

    // Verifies that healing moves use the selected move's power instead of always using the first move slot.
    [Fact]
    public void HealingMoves_UseSelectedMovePower()
    {
        Animal pokemon = new Animal(
            id: 99,
            name: "HEALMON",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 75,
            currentHealth: 50,
            level: 1,
            moves: new[] { MoveData.Tackle, MoveData.Moonlight });

        Move selectedMove = pokemon.Moves[1];

        BattleMoveResult result = BattleEngine.UseMove(pokemon, pokemon, selectedMove);

        Assert.Equal(BattleMoveResultType.Healing, result.ResultType);
        Assert.Equal("MOONLIGHT", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(60, pokemon.CurrentHealth);
    }

    // Verifies that healing behavior comes from move metadata, not hard-coded move names.
    [Fact]
    public void BattleEngine_UseHealingMove_UsesMoveEffectInsteadOfMoveName()
    {
        Move healingMove = new Move("CUSTOM HEAL", MoveType.Nature, 6, MoveEffect.Healing);
        Animal pokemon = new Animal(
            id: 99,
            name: "HEALMON",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 30,
            currentHealth: 20,
            level: 1,
            moves: new[] { healingMove });

        BattleMoveResult result = BattleEngine.UseMove(pokemon, pokemon, healingMove);

        Assert.True(BattleEngine.IsHealingMove(healingMove));
        Assert.Equal(BattleMoveResultType.Healing, result.ResultType);
        Assert.Equal("CUSTOM HEAL", result.MoveName);
        Assert.Equal(6, result.Amount);
        Assert.Equal(26, pokemon.CurrentHealth);
    }

    // Verifies that healing moves at full currentHealth return the no-effect result without changing currentHealth.
    [Fact]
    public void BattleEngine_UseHealingMoveAtFullHealth_ReturnsNoEffect()
    {
        Animal pokemon = new Animal(id: 99, name: "HEALMON", element: AnimalElement.Nature, speed: 7, baseHealth: 30, currentHealth: 30, level: 1, moves: new[] { MoveData.Moonlight });

        BattleMoveResult result = BattleEngine.UseMove(pokemon, pokemon, MoveData.Moonlight);

        Assert.Equal(BattleMoveResultType.NoEffect, result.ResultType);
        Assert.Equal("MOONLIGHT", result.MoveName);
        Assert.Equal(0, result.Amount);
        Assert.Equal(30, result.TargetHealth);
        Assert.Equal(30, pokemon.CurrentHealth);
    }

    // Verifies that healing by the exact missing amount reaches full currentHealth and reports restoration text.
    [Fact]
    public void HealingMoves_ExactAmountToFull_RestoresHealth()
    {
        Animal pokemon = new Animal(id: 99, name: "HEALMON", element: AnimalElement.Nature, speed: 7, baseHealth: 30, currentHealth: 25, level: 1, moves: new[] { MoveData.Moonlight });
        FakeGameIO io = new();

        BattleHelpers.UseHealingMove(io, pokemon, 5);

        Assert.Equal(30, pokemon.CurrentHealth);
        Assert.Contains("Health Restored", io.OutputText);
        Assert.DoesNotContain("Nothing happened", io.OutputText);
    }

    // Verifies that an invalid then no response re-prompts and leaves the party order unchanged.
    [Fact]
    public void HandlePlayerFaintedAnimal_InvalidThenNo_RePromptsWithoutSwapping()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        FakeGameIO io = new("maybe", "no");

        BattleHelpers.HandlePlayerFaintedAnimal(io, player, "Would you like to switch animals?");

        int promptCount = io.OutputText.Split("Would you like to switch animals?").Length - 1;
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Equal("CAT", player.AnimalInventory[0].Name);
        Assert.Equal("LION", player.AnimalInventory[1].Name);
    }

    // Verifies that an invalid then yes response re-prompts and swaps the selected party creatures.
    [Fact]
    public void HandlePlayerFaintedAnimal_InvalidThenYes_RePromptsAndSwapsAnimals()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        FakeGameIO io = new("maybe", "yes", "cat", "lion");

        BattleHelpers.HandlePlayerFaintedAnimal(io, player, "Would you like to switch animals?");

        int promptCount = io.OutputText.Split("Would you like to switch animals?").Length - 1;
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Equal("LION", player.AnimalInventory[0].Name);
        Assert.Equal("CAT", player.AnimalInventory[1].Name);
    }

    // Verifies that a two-creature party auto-switches to the healthy backup without prompting.
    [Fact]
    public void HandlePlayerFaintedAnimal_WithTwoAnimals_AutoSwitchesToHealthyAnimalWithoutPrompting()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        FakeGameIO io = new();

        bool switched = BattleHelpers.HandlePlayerFaintedAnimal(io, player, "Would you like to switch animals?");

        Assert.True(switched);
        Assert.Equal("LION", player.AnimalInventory[0].Name);
        Assert.Equal("CAT", player.AnimalInventory[1].Name);
        Assert.DoesNotContain("Would you like to switch animals?", io.OutputText);
    }

    // Verifies that a two-creature party does not auto-switch when both creatures have fainted.
    [Fact]
    public void HandlePlayerFaintedAnimal_WithTwoFaintedAnimals_DoesNotAutoSwitch()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        player.AnimalInventory[1].CurrentHealth = 0;
        FakeGameIO io = new();

        bool switched = BattleHelpers.HandlePlayerFaintedAnimal(io, player, "Would you like to switch animals?");

        Assert.False(switched);
        Assert.Equal("CAT", player.AnimalInventory[0].Name);
        Assert.Equal("LION", player.AnimalInventory[1].Name);
        Assert.DoesNotContain("Would you like to switch animals?", io.OutputText);
    }

    // Verifies that faint detection returns true for zero currentHealth.
    [Fact]
    public void BattleEngine_IsFainted_ReturnsTrueForZeroHealth()
    {
        Animal pokemon = new Animal(id: 99, name: "TESTMON", element: AnimalElement.Nature, speed: 5, baseHealth: 20, currentHealth: 0, level: 1, moves: new[] { MoveData.Tackle });

        Assert.True(BattleEngine.IsFainted(pokemon));
    }

    // Verifies that the engine reports no usable party creatures when all currentHealth values are zero or below.
    [Fact]
    public void BattleEngine_HasUsableAnimals_ReturnsFalseWhenNoPartyAnimalsHaveHealth()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        player.AnimalInventory[1].CurrentHealth = 0;

        Assert.False(BattleEngine.HasUsableAnimals(player));
        Assert.Equal(-1, BattleEngine.GetNextHealthyAnimalIndex(player));
    }

    // Verifies that the next-healthy lookup returns the first healthy creature at or after the requested start index.
    [Fact]
    public void BattleEngine_GetNextHealthyAnimalIndex_ReturnsFirstHealthyAnimalAtOrAfterStartIndex()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "DOG", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Ember }));
        player.AnimalInventory[0].CurrentHealth = 0;
        player.AnimalInventory[1].CurrentHealth = 0;

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

    // Verifies that auto-switching fails when the only replacement creature has also fainted.
    [Fact]
    public void BattleEngine_TryAutoSwitchTwoAnimalParty_ReturnsFalseWhenReplacementIsFainted()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AnimalInventory[0].CurrentHealth = 0;
        player.AnimalInventory[1].CurrentHealth = 0;

        bool switched = BattleEngine.TryAutoSwitchTwoAnimalParty(player, player.AnimalInventory[0]);

        Assert.False(switched);
        Assert.Equal("CAT", player.AnimalInventory[0].Name);
        Assert.Equal("LION", player.AnimalInventory[1].Name);
    }

    // Verifies that catching a wild creature adds it to the party and removes it from the room encounter list.
    [Fact]
    public void BattleEngine_TryCatchWildAnimal_AddsAnimalToPartyAndRemovesEncounter()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 0, level: 1, moves: new[] { MoveData.Peck });
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
        Animal wildAnimal = new Animal(id: 7, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 0, level: 1, moves: new[] { MoveData.Peck });
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
        Animal wildAnimal = new Animal(id: 7, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 0, level: 1, moves: new[] { MoveData.Peck });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        BattleEngine.ReleaseAnimalAndCatchWildAnimal(player, animalToRelease, wildAnimal);

        Assert.DoesNotContain(animalToRelease, player.AnimalInventory);
        Assert.Contains(wildAnimal, player.AnimalInventory);
        Assert.Contains(animalToRelease, player.CurrentRoom.EncounterAnimals);
        Assert.DoesNotContain(wildAnimal, player.CurrentRoom.EncounterAnimals);
        Assert.Equal(animalToRelease.BaseHealth, animalToRelease.CurrentHealth);
    }

    // Verifies that opponent move choice can be controlled in tests without relying on Random.Shared.
    [Fact]
    public void HandleOpponentTurn_UsesInjectedMoveSelector()
    {
        Animal opponent = new Animal(
            id: 1,
            name: "OPPONENT",
            element: AnimalElement.Nature,
            speed: 7,
            baseHealth: 20,
            currentHealth: 20,
            level: 1,
            moves: new[] { new Move("WEAK", MoveType.Neutral, 1), new Move("STRONG", MoveType.Neutral, 7) });
        Animal player = new Animal(id: 2, name: "PLAYER", element: AnimalElement.Nature, speed: 7, baseHealth: 20, currentHealth: 20, level: 1, moves: new[] { MoveData.Tackle });
        FakeGameIO io = new();

        BattleHelpers.HandleOpponentTurn(io, opponent, player, "Opponent Move", string.Empty, new FixedMoveSelector(opponent.Moves[1]));

        Assert.Equal(13, player.CurrentHealth);
        Assert.Contains("OPPONENT used STRONG", io.OutputText);
    }

    // Verifies that letting a wild creature run away removes it from the room encounter list.
    [Fact]
    public void BattleEngine_LetWildAnimalRunAway_RemovesEncounterAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 0, level: 1, moves: new[] { MoveData.Peck });
        player.CurrentRoom.AddEncounterAnimal(wildAnimal);

        BattleEngine.LetWildAnimalRunAway(player, wildAnimal);

        Assert.DoesNotContain(wildAnimal, player.CurrentRoom.EncounterAnimals);
    }

    // Verifies that creating a wild battle uses the player's current lead and the supplied wild opponent.
    [Fact]
    public void BattleState_CreateWildBattle_UsesCurrentPlayerLeadAndWildAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        Animal wildAnimal = new Animal(id: 3, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Peck });

        BattleState battleState = BattleState.CreateWildBattle(player, wildAnimal);

        Assert.Equal("CAT", battleState.PlayerAnimal.Name);
        Assert.Equal("PIDGEY", battleState.OpponentAnimal.Name);
        Assert.False(battleState.IsOver);
    }

    // Verifies that creating a wild battle skips fainted lead creatures and starts with the next healthy one.
    [Fact]
    public void BattleState_CreateWildBattle_SkipsFaintedPlayerLeadAnimal()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "FLAREON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Ember }));
        player.AnimalInventory[0].CurrentHealth = 0;
        player.AnimalInventory[1].CurrentHealth = 0;
        Animal wildAnimal = new Animal(id: 4, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Peck });

        BattleState battleState = BattleState.CreateWildBattle(player, wildAnimal);

        Assert.Equal(2, battleState.PlayerActiveIndex);
        Assert.Equal("FLAREON", battleState.PlayerAnimal.Name);
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
            new Animal(id: 3, name: "PIDGEY", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 0, level: 1, moves: new[] { MoveData.Peck }),
            new Animal(id: 4, name: "PIKACHU", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Spark })
        });
        BattleState battleState = BattleState.CreateTrainerBattle(player, opponent);

        bool switched = battleState.TrySwitchOpponentToNextHealthyAnimal(startIndex: 1);

        Assert.True(switched);
        Assert.Equal(1, battleState.OpponentActiveIndex);
        Assert.Equal("PIKACHU", battleState.OpponentAnimal.Name);
    }

    // Verifies that cloning creates a separate animal and move list so later mutations do not leak back to the original.
    [Fact]
    public void AnimalClone_CreatesIndependentMoveObjects()
    {
        Animal original = new Animal(
            id: 99,
            name: "CLONEMON",
            element: AnimalElement.Nature,
            speed: 8,
            baseHealth: 30,
            currentHealth: 30,
            level: 4,
            moves: new[] { new Move("TACKLE", MoveType.Neutral, 5) });

        Animal clone = original.Clone();
        clone.CurrentHealth = 1;

        Assert.Equal("CLONEMON", original.Name);
        Assert.Equal("TACKLE", original.Moves[0].MoveName);
        Assert.Equal(5, original.Moves[0].MovePower);
        Assert.Equal(30, original.CurrentHealth);
    }

    // Verifies that a trainer can rebuild a fresh battle roster from templates after a previous battle changed live HP values.
    [Fact]
    public void PrepareForBattle_RebuildsFreshTrainerRosterFromTemplate()
    {
        List<Animal> animals = GameData.CreateAnimals();
        Room startingRoom = new Map().StartRoom;
        CompPlayer gymLeader = new CompPlayer("Trainer", startingRoom);
        gymLeader.SetBattleTeam(new[] { animals[3], animals[14] });

        gymLeader.AnimalInventory[0].CurrentHealth = 1;
        gymLeader.AnimalInventory[1].CurrentHealth = 2;
        Animal firstBattleLead = gymLeader.AnimalInventory[0];

        gymLeader.PrepareForBattle();

        Assert.Equal(gymLeader.BattleTeamTemplate[0].BaseHealth, gymLeader.AnimalInventory[0].CurrentHealth);
        Assert.Equal(gymLeader.BattleTeamTemplate[1].BaseHealth, gymLeader.AnimalInventory[1].CurrentHealth);
        Assert.NotSame(firstBattleLead, gymLeader.AnimalInventory[0]);
    }

    private static Player CreateTwoAnimalPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddAnimal(new Animal(id: 1, name: "CAT", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Bite }));
        player.AddAnimal(new Animal(id: 2, name: "LION", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Psychic }));
        return player;
    }

    private static Player CreateThreeAnimalPlayer()
    {
        Player player = CreateTwoAnimalPlayer();
        player.AddAnimal(new Animal(id: 3, name: "FLAREON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Ember }));
        return player;
    }

    private static Player CreateFullAnimalPartyPlayer()
    {
        Player player = CreateThreeAnimalPlayer();
        player.AddAnimal(new Animal(id: 4, name: "VAPOREON", element: AnimalElement.Mystic, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.WaterGun }));
        player.AddAnimal(new Animal(id: 5, name: "LEAFEON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.VineWhip }));
        player.AddAnimal(new Animal(id: 6, name: "JOLTEON", element: AnimalElement.Nature, speed: 7, baseHealth: 10, currentHealth: 10, level: 1, moves: new[] { MoveData.Spark }));
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
