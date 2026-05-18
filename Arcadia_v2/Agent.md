## Overview

Replace the current Pokemon model usage with the new Animal model in a controlled migration step.

The repo currently still uses `Pokemon`, `PokemonType`, and Pokemon-based collections throughout the game and tests. A new replacement model already exists in the codebase as `Animal` with `AnimalElement`.

## Goal For This Stage

Complete the first migration stage from Pokemon to Animal without changing the game's core behavior beyond the requested element remapping.

At the end of this stage:

- Production code should use `Animal` instead of `Pokemon`.
- Element checks should use `AnimalElement` instead of `PokemonType`.
- Pokemon creation should be replaced by animal creation using the new element rules below.
- The existing "must have a water-type Pokemon" progression check should become a "must have a mystic animal" check.
- `Pokemon.cs` should no longer be the active model used by the project.

## Element Mapping Rules

Use these temporary migration rules for all existing creature roster entries:

- Any creature that was previously `PokemonType.Water` should become `AnimalElement.Mystic`.
- Every other creature should become `AnimalElement.Nature` for now.

Do not try to preserve the old detailed type spread in this stage. This step is intentionally temporary and simplified.

## Required Code Changes

1. Replace the core model usage.

- Update code that currently depends on `Pokemon` so it depends on `Animal` instead.
- Update constructor calls, method parameters, return types, local variables, lists, and properties.
- Update `Clone()` and any save/load reconstruction paths to use `Animal`.

2. Replace the type system usage.

- Replace `PokemonType` references with `AnimalElement`.
- Replace the `Type` property usage with the `Element` property where applicable.
- Update any logic that compares elements, especially movement unlock checks.

3. Update factory/data creation.

- Replace the current Pokemon factory/data flow with Animal-based creation.
- Update roster creation so all non-water entries use `AnimalElement.Nature`.
- Update former water entries to use `AnimalElement.Mystic`.
- Keep the current stats, moves, ids, and names unchanged unless required by the model rename.

4. Update save and game setup flow.

- Update `GameData`, factory usage, and save restoration code so they return and rebuild `Animal` instances.
- Preserve current save behavior as much as possible within this migration step.
- If save DTO names remain Pokemon-based for now, that is acceptable in this stage as long as runtime objects are migrated correctly.

5. Update gameplay checks and text where required.

- Replace the team-check logic that currently looks for `PokemonType.Water` so it checks for `AnimalElement.Mystic`.
- Update player-facing text that specifically refers to the movement requirement as "Water-type Pokemon" if that text is directly tied to the check being changed.

6. Update tests.

- Update affected unit tests so they compile against `Animal` and `AnimalElement`.
- Update any assertions that depend on the old water-type gating rule so they now reflect the mystic-element rule.

## Out Of Scope For This Stage

- Do not redesign moves, stats, battle flow, or save schema names unless required to complete the rename.
- Do not attempt a full lore/text rewrite from Pokemon to Animal everywhere in the game unless the text directly blocks clarity or test correctness.
- Do not introduce a second migration layer that keeps both `Pokemon` and `Animal` active in production code unless that is absolutely necessary to keep the build passing.

## Suggested Implementation Order

1.. Migrate the core creature model usages from `Pokemon` to `Animal`.
2. Migrate `PokemonType` comparisons to `AnimalElement`.
3. Update roster/factory creation and `GameData`.
4. Update movement gating from water to mystic.
5. Update tests and fix any compile/runtime fallout.
6. Run the full unit test project.

## Acceptance Criteria

- Production code compiles using `Animal` as the active runtime creature model.
- `PokemonType` is no longer used in active production logic for creature elements.
- Former water-type roster entries now use `AnimalElement.Mystic`.
- All other roster entries now use `AnimalElement.Nature`.
- The progression check that used to require a water-type Pokemon now requires a mystic animal instead.
- Relevant tests are updated and passing.
