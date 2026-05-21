# Code Review

Review target: current working tree in `Arcadia_v2`.

Validation run:

```powershell
dotnet build .\Arcadia_v2\Arcadia_v2.csproj /nologo /clp:ErrorsOnly
dotnet test .\UnitTest\UnitTest.csproj /nologo /clp:ErrorsOnly
```

Result: build passed with `0` warnings/errors, and all `162` unit tests passed.

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

### Resolved: Healing move behavior is hard-coded to old uppercase names

Original finding: `BattleEngine.IsHealingMove` only recognized exact strings:

- `Arcadia_v2/Battles/BattleEngine.cs:75`
- `Arcadia_v2/Battles/BattleEngine.cs:77`

Current `MoveData` no longer defines `MOONLIGHT` or `SUNLIGHT`, and the new predefined move names use display casing such as `Pounce`, `Current Rush`, and `Deepsea Rupture`.

Impact: healing behavior was disconnected from the actual move catalog. Any future healing move with display casing like `Moonlight`, or a renamed healing move, would be treated as a damage move unless this string check was manually updated.

Resolution: `Move` now has an explicit `MoveEffect`, `Bloom` is marked as `MoveEffect.Heal`, and `BattleEngine.UseMove` branches on `move.Effect` instead of move-name strings. Save capture/restore also persists move effects, with a factory-template fallback for older saves that do not contain the new field.

Status: fixed.

### Resolved: Player defeat messages can be printed twice

Original finding: `BattleHelpers.HandlePlayerDefeatedAnimal` printed the defeat message immediately:

- `Arcadia_v2/Battles/BattleHelpers.cs:106`

The battle finalizers could print the same message again after the loop exited:

- `Arcadia_v2/Battles/TrainerBattleFlow.cs:114`
- `Arcadia_v2/Gameplay/WildBattleFlow.cs:86`

Impact: when the player's active animal was defeated and the player could not or did not switch, the UI could report the same defeat event twice.

Resolution: `HandlePlayerDefeatedAnimal` now returns `PlayerDefeatedAnimalResult` (`NotDefeated`, `Switched`, or `DefeatedNoSwitch`) and does not print defeat text. Wild and trainer battle flows print the defeat message once after the opponent turn, and their finalizers no longer repeat it.

Status: fixed.

### Resolved: Final encounter text does not match the actual encounter

Original finding: the endgame text described an Arceus-style final challenge:

- `Arcadia_v2/Gameplay/GameLoop.cs:141`
- `Arcadia_v2/Gameplay/GameLoop.cs:143`

But the final room encounter was populated with `mapAnimals[19]`, which was `M_DOG` in the current factory:

- `Arcadia_v2/Map/Map.cs:201`

Impact: the player was told they were facing a final god/champion encounter, but the actual battle target was a normal roster creature.

Resolution: `TheEnd` now resolves the final encounter by the `NU_DRAGON` creature name instead of the old index.

Status: fixed.

### Resolved: Old Pokemon naming still leaks through production code

Original finding: several production types, properties, comments, command labels, and story strings still used Pokemon terminology.

Examples:

- `Arcadia_v2/Saves/GameSaveState.cs:21`
- `Arcadia_v2/Saves/GameSaveState.cs:27`
- `Arcadia_v2/Saves/GameSaveState.cs:39`
- `Arcadia_v2/Saves/GameStateMapper.cs:147`
- `Arcadia_v2/Commands/CommandDefinitions.cs:24`
- `Arcadia_v2/Gameplay/GameSetup.cs:91`
- `Arcadia_v2/Gameplay/GameSetup.cs:92`
- `Arcadia_v2/Gameplay/GameSetup.cs:118`

Impact: this created a split domain model where code talked about animals/creatures in most places but still serialized and displayed Pokemon language elsewhere. It made future changes more error-prone because contributors had to remember which old names were still intentional.

Resolution: save compatibility was not required, so both the C# save model and JSON shape were renamed to animal terminology. `PokemonSaveState`, `PokemonInventory`, and `EncounterPokemon` are now `AnimalSaveState`, `AnimalInventory`, and `EncounterAnimals`. The action command enum/display name now uses `AnimalInventory`, the command aliases are `animalinventory`, `animals`, and `ai`, and the remaining production story/map text now refers to creatures or Arcadia-specific names.

Status: fixed.

### Resolved: Move catalog has mismatched names and likely typos

Original finding: current move constants included display names that did not match the constant or appeared misspelled:

- `Arcadia_v2/Creatures/Move.cs:74` has `COLONY_RUSH = new Move("LEER", ...)`
- `Arcadia_v2/Creatures/Move.cs:93` has `OCEON_PULSE`
- `Arcadia_v2/Creatures/Move.cs:95` has `"Tital Break"`

Impact: UI output and saved move data could show names that looked accidental or left over from old move data.

Resolution: `COLONY_RUSH` now displays `Colony Rush`, `OCEON_PULSE` was renamed to `OCEAN_PULSE` with display name `Ocean Pulse`, and `Tital Break` was corrected to `Tidal Break`. A focused unit test now verifies every predefined `MoveData` display name against an explicit expected list.

Status: fixed.

### Resolved: Movement requirements are split between unused room properties and map rules

Original finding: `Room` exposed requirement-like properties:

- `Arcadia_v2/Map/Room.cs:39`
- `Arcadia_v2/Map/Room.cs:40`

But movement actually used the `Map` requirement dictionary:

- `Arcadia_v2/Map/Map.cs:150`
- `Arcadia_v2/Gameplay/MovementFlow.cs:66`

`TheEnd` set `RequiresChampionDefeatToEnter = true`, but that property was not what gated movement. The real gate was separately added in `AddMovementRequirements`.

Impact: future changes could update the room property and assume movement was gated, while the game still relied on the dictionary entry.

Resolution: the unused room-level requirement properties were removed. Directional movement requirements now live only in `Map`, where `MovementFlow` already reads them.

Status: fixed.

### Low: Roster construction is hard to audit and easy to break

`AnimalFactory.CreateAnimals` is a large hand-written list with repeated structure for every element/species combination.

Relevant file:

- `Arcadia_v2/Creatures/AnimalFactory.cs`

Impact: this makes simple changes risky. The current mixed-case Mystic names and move-name typos are examples of issues that are easy to introduce in this format.

Recommended fix: represent species and element move pairs as small data records, then generate the roster from those records. Add tests for contiguous IDs, final ID `96`, unique IDs, normalized names, four moves per creature, and expected element/move pairings.

## Test Gaps

The current test suite is passing, but it does not yet cover several real user flows:

- The uppercase roster-name invariant beyond the new factory-name test in `AnimalTest`.
- Player defeat without switching and ensuring the defeat message is printed once.
- The identity of the final `The End` room encounter.
- The predefined move catalog's display names and spelling.
- Null or partially missing save JSON lists, which could still escape the current save-load failure handling.

## Overall Result

The codebase is in a working state and the current tests pass. The biggest risk is not compilation; it is inconsistent domain data and string-based matching. Normalizing creature names, replacing move-name effect checks with explicit move metadata, and tightening save/domain naming would reduce the most likely bugs going forward.
