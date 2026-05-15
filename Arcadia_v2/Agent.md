## Current Request

Review the project's code overall.
Looking for feedback on the following:
- Project's architecture
- Any poor design or smells
- Any bugs or errors

This should be a full code review. 

When completed put your response on this document

## Response goes below this line

## Code Review Response

### Findings

1. `StartupFlow.HandleDelete` does not re-display the delete confirmation prompt after invalid yes/no input.
   - File: `Gameplay/StartupFlow.cs`, lines 62-80.
   - The current behavior prints `Invalid input` once and returns to the startup menu. The request says invalid input should display the previous prompt again. For delete confirmation, the previous prompt is `Are you sure you want to delete?`, so this should loop until the player enters `yes` or `no`.
   - Risk: low to medium. The save is not deleted accidentally, but the UX does not match the requested behavior.

2. Healing moves fail at exactly the amount needed for a full heal.
   - File: `Battles/BattleHelpers.cs`, lines 110-118.
   - Current condition: `pokemon.Health >= pokemon.BaseHealth - healingPower`.
   - Example: if base health is `30`, current health is `25`, and healing power is `5`, the move prints `Nothing happened` instead of restoring to `30`.
   - The no-op check should be based on whether health is already full, not whether health is within one healing move of full.

3. The in-game menu method has unused parameters and unused local state.
   - File: `Gameplay/MenuFlow.cs`, lines 14-27.
   - `mainPokemon` is passed into `HandleMenu` but never used. `menuChoice` is calculated but never used.
   - Risk: low, but it is a design smell. It makes method signatures noisier and suggests older logic was removed without cleaning the interface.

4. The party swapping flow is more complex than it needs to be and contains suspicious index logic.
   - File: `Battles/PartyFlow.cs`, lines 22-99.
   - The loops search by Pokemon name, but they also include an `i - 1` fallback when `i >= 3`. That fallback can match a previous Pokemon unexpectedly and is hard to justify from the UI behavior.
   - The method also prints debug-like text: `its working.`
   - Risk: medium. The current tests cover basic swapping, but this code is fragile and hard to reason about.

5. Save mapping depends heavily on Pokemon IDs and current factory data.
   - File: `Saves/GameStateMapper.cs`, lines 113-132.
   - Saves store Pokemon ID/name/health, then restore by looking up each ID in `GameData.CreatePokemon()`.
   - This is acceptable for the current model because Pokemon moves, type, speed, level, and base health are static. If future gameplay changes moves, levels, stats, or learned moves, those changes will be lost on load unless the DTO is expanded.

6. The project has a lot of static flow classes directly coupled to `Console`.
   - Examples: `Gameplay/GameLoop.cs`, `Gameplay/StartupFlow.cs`, `Gameplay/MenuFlow.cs`, `Battles/BattleHelpers.cs`, `Battles/PartyFlow.cs`.
   - This is workable for a small console game, but it makes behavior harder to test and reuse. Tests currently rely on redirecting `Console.In` and `Console.Out`.
   - A longer-term improvement would be an input/output abstraction, even a small `IConsole` or `IGameIO`, injected into flows.

### Architecture Feedback

The overall structure is much better than a single large `Program.cs`. The project now has clear folders for commands, gameplay flow, battles, map, player, Pokemon data, input, and saves. The save system is also layered reasonably well:

- `IGameSaveRepository` isolates persistence.
- `SqliteGameSaveRepository` owns SQLite details.
- `GameSaveSerializer` owns JSON.
- `GameStateMapper` owns conversion between live objects and save DTOs.
- `GameSaveService` coordinates save/load/delete.

That separation is solid. The biggest architectural weakness is still that the gameplay layer is mostly static and console-driven. This keeps the implementation simple, but it causes method signatures like `MenuFlow.HandleMenu(...)` to grow and makes test setup more awkward than it needs to be.

`GameState` is a useful improvement because it gives the runtime state one object to pass around. The next step would be to pass `GameState` more consistently instead of passing both `GameState` and individual pieces of it. For example, `MenuFlow.HandleMenu` receives `GameState`, `mainPlayer`, `mainPokemon`, every gym leader, and the champion. Most of those are already available through `GameState`.

### Design Smells

- Some comments describe history rather than current intent, such as "legacy" and "reference version" comments. Those can be useful during refactoring, but they eventually become noise.
- There are several magic numbers tied to command enum values and menu choices. The enum approach helps, but some code still switches on integer values instead of the enum itself.
- Save/load behavior is now split correctly between startup and in-game menu, but startup numbering is unusual because `1` disappears when a save exists while `2` and `3` remain. This matches the current request, but it should be documented in tests because it is easy for a future change to "fix" it incorrectly.
- `GameStateMapper.CreatePokemon` recreates the full Pokemon dictionary every time it restores one list. The current data set is small, so this is not a performance problem, but a shared lookup per apply operation would be cleaner.

### Bugs Or Errors

- Confirmed tests pass: `dotnet test` reports `103` passed, `0` failed.
- The two strongest behavioral bugs found by static review are:
  - delete confirmation invalid input does not re-prompt the confirmation prompt;
  - healing moves do not heal when health is exactly one healing amount below max.
- No compile errors or failing tests were found.

### Recommended Next Steps

1. Fix `StartupFlow.HandleDelete` so invalid yes/no input loops on the delete confirmation prompt.
2. Fix `BattleHelpers.UseHealingMove` so it only prints `Nothing happened` when health is already full.
3. Simplify `MenuFlow.HandleMenu` to accept `GameState` and `GameSaveService`, then read needed objects from `GameState`.
4. Refactor `PartyFlow.SwapPokemon` into smaller helper methods and remove the `i - 1` fallback unless there is a specific gameplay reason for it.
5. Consider introducing a small console I/O abstraction once the gameplay flows stabilize.
