# Code Review

Review target: current working tree in `Arcadia_v2`.

Validation run:

```powershell
dotnet build .\Arcadia_v2\Arcadia_v2.csproj /nologo /clp:ErrorsOnly
dotnet test .\UnitTest\UnitTest.csproj /nologo /clp:ErrorsOnly
```

Result: build passed with `0` warnings/errors, and all `152` unit tests passed.

## Findings

### Resolved: Mixed-case creature names cannot be selected by player input

Original finding: `AnimalFactory` defined the first Mystic creatures as `M_Cat`, `M_Lion`, and `M_Dog` while most other roster names were uppercase, for example `M_TURTLE`, `N_CAT`, and `T_CAT`.

Relevant code:

- `Arcadia_v2/Creatures/AnimalFactory.cs:202`
- `Arcadia_v2/Creatures/AnimalFactory.cs:212`
- `Arcadia_v2/Creatures/AnimalFactory.cs:222`
- `Arcadia_v2/Battles/PartyFlow.cs:51`
- `Arcadia_v2/Battles/PartyFlow.cs:64`
- `Arcadia_v2/Gameplay/WildBattleFlow.cs:174`
- `Arcadia_v2/Gameplay/WildBattleFlow.cs:188`

The player input path uppercases names with `Program.ReadUpperTrimmedInput`, then compares the result to `animal.Name` using exact equality. That meant a player typing `m_cat` or `M_Cat` became `M_CAT`, which did not match the stored `M_Cat`.

Resolution: factory names were normalized to uppercase, including `NULL0`, `M_CAT`, `M_LION`, and `M_DOG`, and a regression test now asserts that all factory creature names are uppercase.

Status: fixed.

### Medium: Healing move behavior is hard-coded to old uppercase names

`BattleEngine.IsHealingMove` only recognizes exact strings:

- `Arcadia_v2/Battles/BattleEngine.cs:75`
- `Arcadia_v2/Battles/BattleEngine.cs:77`

Current `MoveData` no longer defines `MOONLIGHT` or `SUNLIGHT`, and the new predefined move names use display casing such as `Pounce`, `Current Rush`, and `Deepsea Rupture`.

Impact: healing behavior is disconnected from the actual move catalog. Any future healing move with display casing like `Moonlight`, or a renamed healing move, will be treated as a damage move unless this string check is manually updated.

Recommended fix: add an explicit move category/effect, such as `MoveEffect.Damage` and `MoveEffect.Heal`, to `Move`. Then `BattleEngine.UseMove` can branch on data instead of move-name strings.

### Medium: Player faint messages can be printed twice

`BattleHelpers.HandlePlayerFaintedAnimal` prints the faint message immediately:

- `Arcadia_v2/Battles/BattleHelpers.cs:106`

The battle finalizers can print the same message again after the loop exits:

- `Arcadia_v2/Battles/TrainerBattleFlow.cs:114`
- `Arcadia_v2/Gameplay/WildBattleFlow.cs:86`

Impact: when the player's active animal faints and the player cannot or does not switch, the UI can report the same faint event twice.

Recommended fix: return a richer result from `HandlePlayerFaintedAnimal` such as `NotFainted`, `Switched`, `FaintedNoSwitch`, and let one place own the final faint message.

### Medium: Final encounter text does not match the actual encounter

The endgame text describes an Arceus-style final challenge:

- `Arcadia_v2/Gameplay/GameLoop.cs:141`
- `Arcadia_v2/Gameplay/GameLoop.cs:143`

But the final room encounter is populated with `mapAnimals[19]`, which is `M_DOG` in the current factory:

- `Arcadia_v2/Map/Map.cs:201`

Impact: the player is told they are facing a final god/champion encounter, but the actual battle target is a normal roster creature.

Recommended fix: either add a real final boss creature to the roster and place it in `TheEnd`, or change the story text to match the creature that is actually there.

### Medium: Old Pokemon naming still leaks through production code

Several production types, properties, comments, command labels, and story strings still use Pokemon terminology.

Examples:

- `Arcadia_v2/Saves/GameSaveState.cs:21`
- `Arcadia_v2/Saves/GameSaveState.cs:27`
- `Arcadia_v2/Saves/GameSaveState.cs:39`
- `Arcadia_v2/Saves/GameStateMapper.cs:147`
- `Arcadia_v2/Commands/CommandDefinitions.cs:24`
- `Arcadia_v2/Gameplay/GameSetup.cs:91`
- `Arcadia_v2/Gameplay/GameSetup.cs:92`
- `Arcadia_v2/Gameplay/GameSetup.cs:118`

Impact: this creates a split domain model where code now talks about animals/creatures in most places but still serializes and displays Pokemon language elsewhere. It makes future changes more error-prone because contributors have to remember which old names are still intentional.

Recommended fix: decide whether save compatibility requires the JSON property names to remain `Pokemon*`. If compatibility matters, keep the serialized names with attributes but rename the C# types/properties to `AnimalSaveState`, `AnimalInventory`, and `EncounterAnimals`. If compatibility does not matter, rename both the C# model and JSON shape.

### Low: Move catalog has mismatched names and likely typos

Current move constants include display names that do not match the constant or appear misspelled:

- `Arcadia_v2/Creatures/Move.cs:74` has `COLONY_RUSH = new Move("LEER", ...)`
- `Arcadia_v2/Creatures/Move.cs:93` has `OCEON_PULSE`
- `Arcadia_v2/Creatures/Move.cs:95` has `"Tital Break"`

Impact: UI output and saved move data can show names that look accidental or left over from old move data.

Recommended fix: correct the display names and add a focused unit test that asserts all predefined move display names are intentional.

### Low: Movement requirements are split between unused room properties and map rules

`Room` exposes requirement-like properties:

- `Arcadia_v2/Map/Room.cs:39`
- `Arcadia_v2/Map/Room.cs:40`

But movement actually uses the `Map` requirement dictionary:

- `Arcadia_v2/Map/Map.cs:150`
- `Arcadia_v2/Gameplay/MovementFlow.cs:66`

`TheEnd` sets `RequiresChampionDefeatToEnter = true`, but that property is not what gates movement. The real gate is separately added in `AddMovementRequirements`.

Impact: future changes may update the room property and assume movement is gated, while the game still relies on the dictionary entry.

Recommended fix: remove the unused requirement properties from `Room`, or make `MovementFlow` read requirements from room data instead of duplicating them in `Map`.

### Low: Roster construction is hard to audit and easy to break

`AnimalFactory.CreateAnimals` is a large hand-written list with repeated structure for every element/species combination.

Relevant file:

- `Arcadia_v2/Creatures/AnimalFactory.cs`

Impact: this makes simple changes risky. The current mixed-case Mystic names and move-name typos are examples of issues that are easy to introduce in this format.

Recommended fix: represent species and element move pairs as small data records, then generate the roster from those records. Add tests for contiguous IDs, final ID `96`, unique IDs, normalized names, four moves per creature, and expected element/move pairings.

## Test Gaps

The current test suite is passing, but it does not yet cover several real user flows:

- The uppercase roster-name invariant beyond the new factory-name test in `AnimalTest`.
- Player fainting without switching and ensuring the faint message is printed once.
- The identity of the final `The End` room encounter.
- The predefined move catalog's display names and spelling.
- Null or partially missing save JSON lists, which could still escape the current save-load failure handling.

## Overall Result

The codebase is in a working state and the current tests pass. The biggest risk is not compilation; it is inconsistent domain data and string-based matching. Normalizing creature names, replacing move-name effect checks with explicit move metadata, and tightening save/domain naming would reduce the most likely bugs going forward.
