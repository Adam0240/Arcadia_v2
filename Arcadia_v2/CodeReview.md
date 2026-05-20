# Arcadia v2 Code Review

## Scope

Reviewed the production code under `Arcadia_v2/` plus the current unit test coverage under `UnitTest/`. The project is in a mostly working state, but the recent creature, move, and save-model refactors introduced several areas where future changes can silently break runtime behavior or saved games.

Current known test status: `161` tests passing.

## Completed / Reclassified Findings

### 1. Save files are fragile after model renames

Files:

- `Arcadia_v2/Saves/GameSaveState.cs`
- `Arcadia_v2/Saves/GameStateMapper.cs`
- `Arcadia_v2/Saves/GameSaveSerializer.cs`

Status: Complete / reclassified.

This is not a current bug while the game is still in active data-model development and old saves are intentionally discarded after major animal, move, or save-shape changes.

Keep this as a future release concern instead of an active issue. Once saves are expected to survive updates, add versioned migration support for renamed fields such as `Health` to `CurrentHealth`, renamed move/animal data, and changed enum values.

Current recommendation:

- No action needed right now if old saves are reset after major data changes.
- Before sharing durable builds, either add save migration or document a clear save-reset policy.
- Focus current save work on making new saves stable going forward.

### 2. Animal IDs are generated from list order but saves depend on them

Files:

- `Arcadia_v2/Creatures/Animal.cs`
- `Arcadia_v2/Creatures/AnimalFactory.cs`
- `UnitTest/AnimalTest.cs`

Status: Complete.

Animal IDs are now generated from stable species and element identities instead of from the animal list position. `AnimalElement` values are explicitly assigned, `AnimalFactory` has explicit `AnimalSpeciesId` values, and `CreateAnimalId(...)` combines them into stable IDs.

Examples:

- `Nature Cat` = `1`
- `Nature Lion` = `2`
- `Mystic Cat` = `101`
- `Nuclear Dragon` = `516`

The factory now has test coverage proving these IDs remain tied to species and element instead of roster loop order.

### 3. Map, starter, and trainer rosters rely on magic indexes

Files:

- `Arcadia_v2/Creatures/GameData.cs`
- `Arcadia_v2/Gameplay/GameSetup.cs`
- `Arcadia_v2/Map/Map.cs`
- `UnitTest/GameSetupTests.cs`
- `UnitTest/MapTest.cs`

Status: Complete.

Starter animals, trainer teams, and map wild encounters now use `GameData.FindAnimal(...)` with explicit element/species choices instead of list indexes.

Examples:

- `GameData.FindAnimal(mainAnimals, AnimalElement.Nature, "Cat")`
- `GameData.FindAnimal(gymAnimals, AnimalElement.Mystic, "Lion")`
- `AddAnimalToRoom(RoomId.FinalTrials, mapAnimals, AnimalElement.Mystic, "Cat")`

The test suite now includes direct coverage for named starter assignments, trainer teams, and map encounter assignments.

### 4. Healing behavior is hard-coded by move name

Files:

- `Arcadia_v2/Creatures/Move.cs`
- `Arcadia_v2/Battles/BattleEngine.cs`
- `Arcadia_v2/Saves/GameSaveState.cs`
- `Arcadia_v2/Saves/GameStateMapper.cs`
- `UnitTest/BattleStateTests.cs`
- `UnitTest/MoveTest.cs`
- `UnitTest/SaveSystemTests.cs`

Status: Complete.

Move behavior now lives on the move data instead of being inferred from move names. `Move` has a `MoveEffect` value, `MoveTemplate` carries the same effect for generated moves, and `BattleEngine.UseMove(...)` switches on `move.Effect`.

Saved moves now persist `Effect` as part of `MoveSaveState`, so custom healing moves stay healing after save/load.

Examples:

- `MoveEffect.Damage`
- `MoveEffect.Healing`
- `new Move("CUSTOM HEAL", MoveType.Nature, 6, MoveEffect.Healing)`

### 5. `AnimalFactory` is partly migrated and now carries two move-definition styles

Files:

- `Arcadia_v2/Creatures/AnimalFactory.cs`
- `UnitTest/AnimalTest.cs`

Status: Complete.

The factory still supports both generated template moves and legacy fixed moves, but the transitional model is now explicit and type-safe. `MoveSlot` is an abstract private record with concrete slot types instead of nullable dual-mode state.

Current slot types:

- `ElementalMoveSlot`
- `NeutralMoveSlot`
- `FixedMoveSlot`

`AnimalFactory.CreateAnimals()` also validates every species-element move set before generation. Each species must define every element in `ElementOrder`, and each generated animal must have between one and four moves.

## High Priority Findings

No active high priority findings remain from this review.

## Medium Priority Findings

### 1. Move data still mixes legacy Pokemon names with Arcadia elements

File:

- `Arcadia_v2/Creatures/Move.cs`

`MoveType` has been updated to Arcadia elements, but many static moves are still legacy names such as `EMBER`, `HYDROPUMP`, `PSYCHIC`, and `DARKPULSE`. The comments also say things like `// Lion Moves`, even when the moves are generic legacy move data.

Recommended fix:

- Separate new Arcadia move templates from legacy compatibility moves.
- Rename or regroup the static move data so comments describe the actual purpose.
- Decide whether uppercase legacy move names and title-case generated move names should coexist long term.

### 2. Element language is inconsistent

Files:

- `Arcadia_v2/Creatures/Animal.cs`
- `Arcadia_v2/Creatures/Move.cs`
- `Arcadia_v2/Map/Map.cs`
- `Arcadia_v2/Map/Room.cs`

The element enum uses `Nuclear`, while the map has `RadioactiveWay`. The user-facing design has also mentioned Nuclear/Radioactive as a possible naming choice. This is not a bug yet, but inconsistent domain language tends to spread through saves, tests, UI text, and content data.

Recommended fix:

- Pick one term for the element and use it consistently.
- If renaming a serialized enum value, handle save migration at the same time.

### 3. Save data uses display names as stable lookup keys

File:

- `Arcadia_v2/Saves/GameStateMapper.cs`

Rooms and trainers are restored by names such as `CurrentRoomName` and trainer `Name`. Display names are likely to change as the Pokemon-to-Arcadia rename continues, and those changes would break old saves.

Recommended fix:

- Save `RoomId` instead of room display name.
- Add stable trainer IDs.
- Keep display names free to change without becoming persistence keys.

## Lower Priority Findings

### 4. Player input still requires typing move and creature names

Files:

- `Arcadia_v2/Battles/BattleHelpers.cs`
- `Arcadia_v2/Battles/PartyFlow.cs`

Move and party selection are case-insensitive now, which is good, but the user still has to type names exactly enough to match. Generated move names like `Nature's Fury` make this more awkward.

Recommended fix:

- Use numbered move selection in battle.
- Use numbered party selection when swapping or releasing animals.

### 5. Some comments and text were mechanically renamed

Files:

- `Arcadia_v2/Battles/TrainerBattleFlow.cs`
- `Arcadia_v2/Gameplay/WildBattleFlow.cs`
- `Arcadia_v2/Gameplay/GameSetup.cs`
- `Arcadia_v2/Map/Map.cs`
- `Arcadia_v2/Creatures/Move.cs`

Examples include comments like `current currentHealth`, remaining Pokemon/Pokeball story text, and spelling issues such as `dissapates`, `Professsor`, and `unconsious`.

Recommended fix:

- Do a focused text/content pass after the core model stabilizes.
- Keep code comments about mechanics, not old domain names.

### 6. `BattleHelpers.HandlePlayerFaintedAnimal` assumes at least one party animal

File:

- `Arcadia_v2/Battles/BattleHelpers.cs`

The overload that accepts only `Player` indexes `mainPlayer.AnimalInventory[0]`. Current game flow likely ensures the player has animals, but the helper itself does not guard against an empty party.

Recommended fix:

- Either assert/throw with a clear message when the inventory is empty, or return `false`.
- Add a focused test if empty parties are possible in future flows.

### 7. Some public names still expose Pokemon terminology

Files:

- `Arcadia_v2/Saves/GameSaveState.cs`
- `Arcadia_v2/Saves/GameStateMapper.cs`
- Unit tests under `UnitTest/`

Types and properties like `PokemonSaveState`, `PokemonInventory`, and `EncounterPokemon` remain. This is not currently breaking behavior, but it conflicts with the Animal/Creature vocabulary and makes future refactors harder to track.

Recommended fix:

- Rename internal code to Animal/Creature naming separately from serialized JSON compatibility.
- If JSON property names must remain for old saves, use serialization attributes or migration DTOs instead of keeping old names in the domain model.

## Design Direction

The species-element move-set design is the right direction for the current goals. It allows:

- Shared species stats.
- Per-element variants.
- Reusable elemental move templates.
- Shared moves across different species of the same element.

The biggest architectural improvement now is not another abstraction layer. It is continuing to stabilize identity and persistence:

Make save migration explicit before saves are treated as durable player data.

Those changes will make the AnimalFactory work safer to continue.
