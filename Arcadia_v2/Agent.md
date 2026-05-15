Battle logic cleanup plan.

Goal:
- Clean up the current battle logic so battle rules are easier to understand, test, and maintain.
- Keep changes incremental and covered by focused tests.
- Preserve current gameplay behavior unless a test or explicit note says otherwise.

Current issue:
- `BattleHelpers`, `WildBattleFlow`, and `TrainerBattleFlow` mix battle rules, input prompts, output text, turn flow, faint handling, catch/release behavior, and rewards.
- This makes the battle code harder to reason about because state changes and presentation are often handled in the same method.

Target direction:
- Battle rule code should own state changes such as damage, healing, faint checks, and active Pokemon selection.
- Flow classes should coordinate input/output and call battle rule methods.
- Result objects or small return values should describe what happened so flows can print messages without duplicating rule decisions.
- Shared battle behavior should live in one place instead of being repeated differently across wild and trainer battles.

Implementation slices:

1. Completed: extract move resolution.
   - Add a rule method such as `BattleEngine.UseMove(Pokemon attacker, Pokemon defender, Move move)`.
   - It should handle attack moves, healing moves, health clamping, and no-op healing when already full.
   - It should return a result object instead of printing directly.
   - Move existing damage/healing tests or add equivalent tests for the new rule method.
   - Added `BattleEngine`, `BattleMoveResult`, and focused rule tests.

2. Completed: extract faint and party utility rules.
   - Add helpers such as `IsFainted(Pokemon pokemon)`, `HasUsablePokemon(Player player)`, and `GetNextHealthyPokemonIndex(Player player)`.
   - Keep user prompts in flow classes, not in rule helpers.
   - Add tests for zero-health, negative-health, and party-with-no-healthy-Pokemon cases.
   - Added shared `BattleEngine` faint/party helpers and updated wild/trainer/helper battle code to use them.

3. Completed: introduce a small battle state model where it reduces duplication.
   - Track active Pokemon explicitly instead of relying on repeated `PokemonInventory[0]` access everywhere.
   - Keep the model small and only add it where it simplifies current wild or trainer battle code.
   - Do not introduce broad abstractions until the simpler rule extraction is complete.
   - Added `BattleState` for active player/opponent Pokemon and migrated wild/trainer battle loops to use it.

4. Refactor trainer battles onto shared battle rules.
   - Preserve badge reward behavior.
   - Preserve trainer team reset behavior from `PrepareForBattle`.
   - Add regression tests for opponent fainting, next Pokemon selection, and battle completion.

5. Refactor wild battles onto shared battle rules.
   - Preserve catch/release behavior.
   - Preserve room encounter removal when Pokemon are caught or run away.
   - Add regression tests for catch, full-party release, and no-catch run-away paths.

Acceptance criteria:
- Existing tests pass after each slice.
- New battle rule code has focused unit tests that do not require console redirection.
- Battle rule code does not depend on `Console` or `IGameIO`.
- `WildBattleFlow` and `TrainerBattleFlow` contain orchestration and presentation, not low-level damage/healing rules.
- The cleanup makes the current battle code easier to read without changing player-facing behavior.
