# Project Overview

Arcadia_v2 is a C# console adventure/creature-battle game. The game loop drives player setup, map exploration, movement, menu handling, wild encounters, trainer/gym-style battles, party management, and save/load behavior.

The production project lives in `Arcadia_v2/`. Unit tests live in `UnitTest/` and reference the main project directly.

# Tech Stack

- Language: C#
- Runtime/framework: .NET 10.0
- App type: Console executable
- Nullable reference types: Enabled
- Implicit usings: Enabled
- Persistence: SQLite through `Microsoft.Data.Sqlite`
- Testing: xUnit with `Microsoft.NET.Test.Sdk`, `xunit.runner.visualstudio`, and `coverlet.collector`

# Folder Structure

- `Arcadia_v2/Program.cs` - application entry point; delegates to `GameLoop.Run()`.
- `Arcadia_v2/Battles/` - battle state, battle engine, battle helpers, trainer battle flow, party flow, and battle move selectors.
- `Arcadia_v2/Commands/` - command models, command definitions, and parser logic.
- `Arcadia_v2/Creatures/` - creature, move, factory, and static game data definitions.
- `Arcadia_v2/Gameplay/` - main game loop, setup flow, startup/menu/movement/gym/wild battle flows, room display, and shared game state.
- `Arcadia_v2/Input/` - game I/O abstraction and console input implementation.
- `Arcadia_v2/Map/` - map and room definitions.
- `Arcadia_v2/Player/` - player state and party/progression behavior.
- `Arcadia_v2/Saves/` - save models, serialization, state mapping, save service, and SQLite repository.
- `UnitTest/` - xUnit tests and fakes for production behavior.

# Architecture Rules

- Keep user interaction behind `IGameIO` where possible so gameplay logic remains testable.
- Keep `Program.Main()` thin; route application flow through gameplay classes.
- Put battle-specific behavior in `Battles/`, map behavior in `Map/`, persistence behavior in `Saves/`, and command parsing in `Commands/`.
- Prefer dependency injection through constructors or method parameters over direct console/database access in domain logic.
- Keep save/load translation in mapper and save service classes rather than spreading persistence details into gameplay flows.
- Old save compatibility is not required after major changes or refactors. Assume old saves will not be loaded unless explicitly told otherwise.
- Preserve deterministic logic in tests by using fakes such as fake I/O, fake repositories, and explicit battle move selectors.

# Coding Style Rules

- Follow the existing C# style: file-scoped behavior within the `Arcadia_v2` namespace, PascalCase for public types/members, camelCase for local variables and parameters.
- Keep nullable annotations meaningful; do not silence nullable warnings without checking the data flow.
- Use clear, direct method names that describe gameplay actions or state transitions.
- Keep methods focused. Move repeated battle, save, parser, or movement behavior into the appropriate helper/service class.
- Add comments only when they clarify non-obvious game rules, state transitions, or persistence behavior.
- Keep terminology consistent with the current domain, and prefer original Arcadia-specific names over terms that are too close to existing monster-collector IP.

# Testing Rules

- Run `dotnet test` from the repository root before considering changes complete.
- Add or update xUnit tests for parser changes, movement/map changes, battle rules, save/load behavior, and player party/progression behavior.
- Use the existing fake implementations in `UnitTest/` for I/O and save repositories instead of relying on real console input or real database files.
- Cover both successful paths and edge cases such as invalid commands, full parties, malformed save data, missing save data, and battle end conditions.
- Keep tests deterministic; avoid uncontrolled randomness in assertions.
- Ensure any new test created have a comment above them describing the test

# Restrictions / What Not To Do

- Do not put direct console reads/writes into domain logic when `IGameIO` can be used.
- Do not bypass save services or mappers by manually duplicating save conversion logic in gameplay classes.
- Do not add legacy save migration or backwards-compatibility code for major changes/refactors unless explicitly requested.
- Do not commit generated build output from `bin/` or `obj/`.
- Do not introduce broad rewrites or unrelated refactors while making targeted gameplay, parser, battle, or save changes.
- Do not add new external packages unless the change clearly needs them.
- Do not reintroduce direct Pokemon names, Pokemon-specific terms, or other avoidable third-party IP references in production-facing text.

# Current Plan


# Current Task
