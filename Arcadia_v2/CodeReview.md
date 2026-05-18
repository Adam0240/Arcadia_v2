# Code Review

Reviewed the main game project and the unit test project. Unit tests were run with:

```powershell
dotnet test "C:\Users\STEAM Room\Desktop\Arcadia_v2\UnitTest\UnitTest.csproj" /nologo /clp:ErrorsOnly
```

Result: 151 passed, 0 failed, 0 skipped.

Existing note addressed: "Several map issues" is expanded below under movement gating and map transition design.

## Findings

1. Medium - The menu prompt advertises the wrong shortcut for swapping animals. `Commands.ReadMenuCommand` prints `Swap Animals (swap/b)` in `Arcadia_v2/Commands/Commands.cs:29`, but `Parser.ParseMenuCommand` maps `b` to Bag at `Arcadia_v2/Commands/Parser.cs:143` and maps Swap to `s` at `Arcadia_v2/Commands/Parser.cs:148`. A player following the menu text will open the bag instead of swapping. Fix either the prompt to `swap/s` or the parser to use a non-conflicting shortcut, and add a command test that enters `b` from the menu prompt if `b` is intended to swap.

   Completed - Updated the menu prompt to advertise `swap/s`, matching the existing parser behavior where `b` means Bag and `s` means Swap. Added command tests that verify the displayed prompt and the `s` shortcut.

2. Medium - New-game startup can crash on blank player names instead of re-prompting. `Program.GetName` returns the trimmed input directly at `Arcadia_v2/Input/ProgramInput.cs:22-26`, and `GameSetup.Initialize` passes that value into `CreateInitialState` at `Arcadia_v2/Gameplay/GameSetup.cs:19`. `Player` rejects blank names, so pressing Enter at the name prompt throws an `ArgumentException` and exits the game. Handle this at the input boundary by looping until a non-empty name is entered. Add a startup/setup test for blank name followed by a valid name.

   Completed - Changed `Program.GetName` to loop until the user enters a non-empty trimmed name and print `Name cannot be empty.` for invalid input. Added a test that supplies a blank name first and verifies the re-prompt.

3. Medium - Movement gating is hard-coded against room names instead of room metadata, making map changes fragile. `MovementFlow.GetRequiredBadgesForMovement` checks `"Ikena" -> "Road 6"` and `"Road 5" -> "Nucleon"` directly at `Arcadia_v2/Gameplay/MovementFlow.cs:100-112`; `RequiresMysticForMovement` has similar name checks at `Arcadia_v2/Gameplay/MovementFlow.cs:131-134`; champion gating also special-cases `"Road 8" -> "Guardian's Tower"` at `Arcadia_v2/Gameplay/MovementFlow.cs:139-140`. Since `Room` already has gate-related properties at `Arcadia_v2/Map/Room.cs:14-15`, move these rules into explicit room/edge metadata or a map transition table so tests and gameplay do not silently break when room names or connections change.

   Completed - Moved movement gate rules into `Map` as explicit transition requirements keyed by source and destination rooms. `MovementFlow` now asks `GameMap` for the requirement instead of hard-coding room-name checks. Added a positive champion-gate movement test.

4. Low - `Animal` allows invalid runtime state that other layers assume cannot exist. The constructor assigns `Name`, `BaseHealth`, `Health`, `Speed`, and `Level` directly at `Arcadia_v2/Creatures/Animal.cs:41-45`, only validating move count at `Arcadia_v2/Creatures/Animal.cs:50`. Save loading validates health, but normal runtime/test code can still construct animals with blank names, negative health, health greater than base health, or invalid stats. Add constructor validation for name and numeric ranges so model invariants live in the model, not only in save restoration.

   Completed - Added `Animal` validation for blank names, negative speed/base health/level, and health outside `0..BaseHealth`. Runtime health assignment now uses the same bounds. Updated release logic to restore released animals to their own `BaseHealth` instead of a hard-coded value.

5. Low - Random move selection is embedded in static helpers, which makes battle behavior difficult to test deterministically. `BattleHelpers.GetRandomMove` uses `Random.Shared` at `Arcadia_v2/Battles/BattleHelpers.cs:37-39`, and `Animal.RanNum` does the same at `Arcadia_v2/Creatures/Animal.cs:71-73`. The current tests cover battle outcomes by choosing player moves, but opponent move choice remains nondeterministic. Consider injecting an RNG abstraction or move selector into battle flows, and remove `RanNum` if it is only a legacy compatibility method.

   Completed - Added `IBattleMoveSelector` and `RandomBattleMoveSelector`, updated opponent-turn handling to accept an injected selector, and removed unused `Animal.RanNum`. Added a deterministic opponent-turn test using a fixed move selector.

6. Low - The test suite covers many core paths but has gaps around user-facing prompts and integration-level loops. `CommandTest.ReadMenuCommand_SwapInput_ReturnsSwap` only verifies entering `swap`, not the advertised shortcut from the prompt at `UnitTest/CommandTest.cs:59`. `MenuFlowTests` covers heal, save, and invalid commands, but not bag, swap, or gym dispatch at `UnitTest/MenuFlowTests.cs:10-47`. `MovementFlowTests` covers several gates but only one side of the champion/Guardian's Tower behavior at `UnitTest/MovementFlowTests.cs:89`. Add tests for each displayed shortcut and for positive/negative gate paths so prompt regressions and map-lock regressions are caught.

   Completed - Added tests for the displayed swap shortcut, bag menu output, swap menu dispatch, gym menu dispatch, deterministic opponent move selection, validation failures, blank-name re-prompting, and the positive Guardian's Tower champion-gate path.

## Design Improvements

- Completed - Consolidated command text and parser aliases into one source of truth. Added `CommandDefinitions` with shared `CommandOption<TCommand>` entries for main, direction, action, and menu commands. Prompt rendering and parser methods now both use those same definitions, so aliases cannot drift from displayed command text.
- Completed - Replaced string-based room gate checks with named transition rules. Each room now has a stable `RoomId`, and movement requirements are keyed by `(RoomId From, RoomId To)` instead of display-name strings. Name-based room lookup remains available for save data and tests, but gate logic no longer depends on room display text.
- Completed - Kept domain validation in constructors or factory methods. `Animal` now validates names, health bounds, speed, base health, level, move count, and null moves during construction. Runtime health assignment is bounded, and `Moves` is exposed as `IReadOnlyList<Move>` backed by a private list so callers cannot mutate an animal into an invalid move state after construction.
- Completed - Separated deterministic battle logic from random selection. Added `IBattleMoveSelector` and `RandomBattleMoveSelector`; battle helpers and trainer/wild battle flows now support injected move selectors. Tests use fixed selectors to verify opponent-turn behavior without relying on `Random.Shared`.

## Test Notes

- Addressed - Added focused prompt and alias tests around command definitions, menu rendering, and the displayed swap shortcut so prompt/parser regressions are caught directly.
- Confirmed - SQLite repository tests continue to use real temporary databases for persistence coverage, while fake repository tests keep startup and menu-flow tests fast.
- Addressed - Added focused movement requirement tests after replacing procedural gate checks with `RoomId` transition rules, including positive and negative champion-gate coverage.
- Addressed - Added validation and deterministic battle tests for the final design improvements. The suite now passes with 151 tests.
