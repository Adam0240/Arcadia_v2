# Project Overview

arcadia_flutter is the Flutter client for Arcadia. It should provide a touch-friendly mobile UI for the Arcadia adventure/creature-battle game while preserving the main game's domain rules, terminology, and save behavior.

The Flutter project lives in `arcadia_flutter/`. The existing console game lives in `Arcadia_v2/`, and unit tests live in `UnitTest/`.
The Flutter app now contains the main Arcadia gameplay feature set. Use `arcadia_flutter/` as the primary implementation source for ordinary Flutter work. Use `Arcadia_v2/` only when a rule, data value, dialogue line, or intended behavior is ambiguous and needs source-of-truth verification.

# Tech Stack

- Language: Dart
- Framework: Flutter
- App type: Cross-platform Flutter application
- Primary target: Android emulator/device
- UI: Flutter widgets, Material components where appropriate, and touch-first layouts
- State: Keep screen state and gameplay state separated using focused classes/services
- Testing: Flutter/Dart tests for Flutter-specific behavior

# Folder Structure

- `arcadia_flutter/pubspec.yaml` - Flutter package metadata, dependencies, assets, and project configuration.
- `arcadia_flutter/lib/main.dart` - app entry point and root widget.
- `arcadia_flutter/lib/map/` - mobile-adapted room ids, room directions, room models, and the connected `GameMap`.
- `arcadia_flutter/lib/creatures/` - mobile-adapted creature domain models, element enums, move catalog, animal catalog, and creature data facade.
- `arcadia_flutter/lib/battles/` - Flutter battle rules, move result models, wild battle state, and guardian battle state.
- `arcadia_flutter/lib/guardians/` - mobile-adapted guardian definitions and runtime guardian state.
- `arcadia_flutter/lib/player/` - mobile-adapted player models for human players, computer players, inventory, star fragments, bond, and battle team templates.
- `arcadia_flutter/lib/saves/` - Flutter JSON save state models, save/load mapper, and repository implementations.
- `arcadia_flutter/lib/services/` - gameplay/session state such as the current room, visited rooms, movement results, and interaction text.
- `arcadia_flutter/lib/screens/` - Flutter screens, including the start menu, playable map screen, wild battle screen, guardian battle screen, swap screen, and movement/menu controls.
- `arcadia_flutter/lib/widgets/` - reusable UI widgets such as custom room artwork.
- `arcadia_flutter/android/` - Android platform project and configuration.
- `arcadia_flutter/ios/`, `macos/`, `linux/`, `web/`, and `windows/` - generated platform folders. Do not edit platform files unless the feature requires platform-specific configuration.
- `arcadia_flutter/test/` - Flutter-specific tests for this project. Use this as the default location for new Flutter tests.
- `Arcadia_v2/` - console reference implementation only.

# Architecture Rules

- Treat `arcadia_flutter/` as the primary implementation target and source of current mobile behavior.
- Treat `Arcadia_v2/` as a reference implementation for ambiguous original game rules, data, and intended behavior, not as an edit target for Flutter work.
- Do not modify existing files in `Arcadia_v2/` unless the user explicitly instructs you to do so.
- Build the Flutter app to preserve the console game's domain rules and terminology while using Flutter-specific UI and services.
- When Flutter needs logic from the console project, create an adapted Dart implementation in the appropriate `arcadia_flutter/lib/` folder instead of editing or directly depending on the C# source.
- Keep duplicated Flutter logic organized by responsibility so it can evolve separately from console-specific flow.
- Keep Flutter widgets focused on UI composition, navigation, and user interaction. Put gameplay decisions, battle rules, save mapping, and state transitions outside widget build methods.
- Keep reusable game/session behavior in services or state classes; keep mobile-adapted domain models in responsibility-based folders such as `map/`, `creatures/`, `battles/`, `player/`, and `saves/`.
- Expand the existing playable exploration flow incrementally. Add rooms, interactions, inventory, dialogue, saves, and battles as separate focused steps instead of folding them all into one widget or service.
- Do not copy console-specific flow into the Flutter app. Adapt the same domain concepts to screens, buttons, menus, dialogs, and mobile navigation.
- Do not use console input/output patterns in Flutter code.
- If shared gameplay code across projects is eventually needed, discuss that explicitly first. Do not extract or move code out of `Arcadia_v2/` as part of ordinary Flutter work.
- Preserve Arcadia-specific terminology and avoid third-party monster-collector IP references in user-facing text.

# Current Flutter Architecture

- `main.dart` starts `ArcadiaApp`, applies the app theme, and routes to `StartMenuScreen`.
- `StartMenuScreen` is the startup screen. It checks whether a save exists, shows `New Game` when no save exists, shows `Load Game` and `Delete Game` when a save exists, prompts for a valid player name before starting a new game, sends new games through `IntroStoryScreen`, and loads saved games directly into `ArcadiaMapScreen`.
- `IntroStoryScreen` presents a concise mobile-adapted opening story before entering the map for a new game. Loaded games skip the intro.
- `ArcadiaMapScreen` is the playable exploration screen. It displays the current room artwork, room name, description, status message, directional controls, Inspect, Encounter when wild animals are present, Guardian when a guardian is in the current room, and Menu.
- `WildBattleScreen` is the playable wild encounter screen. It shows the active player animal, the wild animal, health totals, move buttons, battle messages, Catch/Leave after defeat, and Run. When the active player animal is defeated and healthy party animals remain, the screen asks the player to choose the next animal.
- `GuardianBattleScreen` is the playable guardian/titan battle screen. It shows the active player animal, the active opponent animal, health totals, move buttons, battle messages, and manual switch choices after active player defeat. Guardian and Titan battles do not expose catch, leave, or run actions.
- Direction buttons are enabled only when the current room has an exit in that direction. Movement calls `MobileGameSession.move`, checks movement requirements, updates the current room, records the visit, may award the Elemental Star, and refreshes the status message.
- The Menu button swaps the movement controls for Inventory, Heal Animals, Reorder Party, Road 8 Swap when available, Grow when available, Bond, Star Fragments, Save, and Return controls. Inventory/Bond/Star Fragments are read-only status panel views backed by the active player. Save uses the Flutter JSON save repository. Loading and deletion happen only from `StartMenuScreen`.
- `GameMap` creates the full current room list, connects rooms with directional exits, and owns movement requirements. The map starts at `RoomId.maiaStable`.
- `GameMap` also populates room encounter animals from `GameCreatureData` using the console reference assignments. Encounter animals are visible through Inspect and can be fought through the Encounter button.
- `Room`, `RoomId`, `RoomDirection`, and `MovementRequirement` are the mobile map domain model. `Room` owns local encounter animal state as cloned animals, separate Road 8 stored captured animal state, town metadata, and final-room metadata. Keep new room metadata, exits, movement requirements, and initial encounter assignments in `GameMap` unless the map grows enough to justify splitting data from construction.
- `MobileGameSession` owns the active `Player`, visited room ids, restore/start-new-game helpers, movement validation, and room interactions. Current-room state should flow through `player.currentRoom`; the session keeps a compatibility getter for UI code.
- New sessions create the main player with the console reference starter animals from `GameCreatureData.createAnimals()`: `N_CAT` and `N_DOG`.
- `saves/` contains the Flutter save/load layer. `GameSaveMapper` captures/restores session state, player state, visited rooms, room encounter animals, Road 8 stored captured animals, guardian state, and Elemental Titan state. `LocalJsonGameSaveRepository` stores `arcadia_save.json` in the app documents directory through `path_provider`; tests can use `JsonGameSaveRepository` with a temp file or an in-memory repository.
- `RoomArtwork` uses Flutter canvas painting for room visuals. Maia's Stable, Ikena, and Road 1 have specific art; the other rooms currently use the shared placeholder scene.
- `creatures/` contains the Flutter-side creature data layer. `AnimalCatalog` builds the generated animal roster from Dart species/element templates, `MoveCatalog` owns shared battle moves, and `GameCreatureData` is the public facade for creating animal lists.
- `guardians/` contains trainer challenge definitions for the four sanctuary guardians and the Elemental Titan, including location, team animal indexes, reward star fragment, reward element, star fragment gate, intro text, and Titan flag. `GuardianState` pairs each definition with a runtime `CompPlayer`.
- `battles/` contains low-level battle behavior. `BattleEngine` applies damage/healing, checks defeated animals, finds healthy party members, and catches/removes wild animals. `WildBattleState` tracks one active wild battle without owning UI. `GuardianBattleState` tracks one active guardian/titan battle without owning UI.
- `player/` contains the Flutter-side player state layer. `GenericPlayer` owns room position, star fragments, creature inventory, bond, display helpers, and restore helpers. `Player` is the human player type, and `CompPlayer` adds defeated state plus cloned battle team templates.

# Current Map Coverage

- Implemented rooms: Maia's Stable, Ikena, Road 1, Road 2, Oak Pass, Road 3, Road 4, New Nucleon, Road 5, Road 6, Road 7, Wyrmrest, Mountains, Radioactive Way, Nucleon, Final Trials, Guardian Tower, Road 8, and The End.
- Implemented traversal uses north/east/south/west exits from `GameMap._connectRooms()`. Add or change routes there so UI controls, session movement, and tests stay aligned.
- Placeholder rooms have descriptions and interaction text so the full map can be explored before every location has final art or full encounter logic.
- Wild animals have been assigned to route rooms from the console reference map and are displayed by Inspect as `Animals Nearby`.
- Rooms with wild animals expose an Encounter action. A wild battle can damage/heal, ask the player to manually choose the next healthy animal after active animal defeat, let the player run, catch a defeated wild animal into inventory, or leave the defeated animal behind. Catching or leaving removes the room encounter; running keeps it available.
- If the player's party is full when a defeated wild animal is caught, the caught animal is restored to full health, removed from the encounter room, and stored at Road 8 instead of forcing an immediate release. Road 8 stored animals are captured animals, not wild encounters, so they are visible through Inspect as `Stored Animals` and do not trigger battle. This is an intentional mobile gameplay change from the console release prompt.
- Road 8 exposes a Swap menu action only when the player is currently at Road 8 and at least one stored animal exists. Swap exchanges one inventory animal with one stored Road 8 animal.
- Reorder Party is available from the menu when the player has at least two party animals and swaps two inventory animal positions.
- Towns expose Heal Animals through the menu. Healing is only allowed in console-equivalent town rooms and restores every party animal to base health.
- Grow is available from the menu when a party animal has 100% bond in its element and has an adult form. Growing replaces the selected animal with its adult form and resets that element's bond to 0.
- Guardian rooms expose a Guardian action. Nature Guardian is at Oak Pass, Mystic Guardian is at Ikena and requires 2 star fragments, Thunder Guardian is at New Nucleon and requires 1 star fragment, and Draconic Guardian is at Wyrmrest.
- Guardian Tower exposes an Elemental Titan action. Elemental Titan requires 4 star fragments, awards the Cosmic Star Fragment, and gates late-game routes.
- Defeating a guardian or Titan marks that trainer defeated, awards that trainer's star fragment once through normal player fragment uniqueness rules, and adds 100 bond to the matching element. Guardian/Titan battle teams are rebuilt from templates before each attempt so damage does not carry across attempts.
- Movement requirements gate selected routes by star fragment count, required Mystic party animal, or Elemental Titan defeat. `MobileGameSession.move` owns requirement checks and blocked messages.
- Returning to Maia's Stable after Elemental Titan defeat with the Cosmic Star Fragment awards the Elemental Star once.
- The End is a final room. Before the final encounter is cleared, Inspect shows the final challenge message. After the final encounter is cleared, an Ending action autosaves the game before asking whether the player wants to stay in Arcadia or return to the start menu. Loading after ending returns to The End just before the final choice.

# UI Rules

- Build for touch first: large enough tap targets, simple navigation paths, readable text, and clear feedback for actions.
- Prefer small, reusable widgets for repeated UI patterns such as room descriptions, movement controls, action buttons, status panels, battle choices, and inventory rows.
- Keep layout responsive across phone, tablet, desktop windows, and emulator sizes. Avoid fixed dimensions unless the element must keep a stable size.
- Use Flutter theming for repeated colors, typography, spacing, and component styling instead of scattering hard-coded styling across widgets.
- Keep widget `build` methods readable. Move repeated layout sections into private widgets or separate files once they become meaningful.
- Include semantic labels or accessible text for buttons, images, battle actions, menus, and important status text where appropriate.
- Replace the starter Flutter counter template assets and text when building real Arcadia screens.

# Coding Style Rules

- Follow standard Dart style: `PascalCase` for types, `camelCase` for variables, methods, parameters, and members, and `snake_case.dart` for file names.
- Keep null safety meaningful; fix nullable flow issues through clear modeling rather than using unnecessary force unwraps.
- Use descriptive names for screens, widgets, services, state classes, commands, and routes.
- Keep methods focused and move repeated UI/game orchestration into helpers or services.
- Add comments only when they clarify non-obvious Flutter lifecycle behavior, platform differences, game rules, or persistence behavior.
- Do not introduce new external packages unless the Flutter feature clearly needs them.
- If at any point you have a question or need a decision about a design choice, ask directly.

# Testing and Verification Rules

- Put new Flutter-specific tests in `arcadia_flutter/test/` unless the user explicitly asks for a different location.
- Ensure any new test created has a comment above it describing the test.
- Add tests around gameplay state, command/action handling, map movement, battle rules, save/load mapping, and other deterministic logic adapted for Flutter.
- Prefer testing extracted Dart logic and widget behavior directly instead of relying only on emulator checks.
- Keep tests deterministic; avoid uncontrolled randomness in assertions.
- Run Flutter tests from the Flutter project when changes touch Flutter logic or widgets:
  - `flutter test`
- Run static analysis before considering Flutter code changes complete when practical:
  - `flutter analyze`
- For Android work, build or run the Android target when the Flutter SDK and emulator/device are available:
  - `flutter run`
  - `flutter build apk`

# Android Emulator Diagnostics

- If the Flutter app installs, starts, and immediately closes on the Android emulator, check Android logcat for the real runtime exception before changing code.
- `adb` may not be on the system `PATH` in this workspace. The Android SDK copy currently used by this machine is:
  - `C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe`
- From the repository root, read recent emulator logs with:
  - `& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' logcat -d -t 1200`
- To check only Android runtime crashes, use:
  - `& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' logcat -d -t 1000 AndroidRuntime:E '*:S'`
- Common Flutter startup crashes may come from missing assets, invalid platform configuration, plugin setup errors, bad async initialization, or runtime exceptions during root widget creation.

# Restrictions / What Not To Do

- Do not commit generated build output from `build/`, `.dart_tool/`, or platform build directories.
- Do not modify existing files under `Arcadia_v2/` for Flutter implementation work unless the user explicitly asks for it.
- Do not move files out of `Arcadia_v2/` into shared projects or libraries without explicit approval.
- Do not reference console-only behavior directly from Flutter UI when the mobile app needs an adapted Dart implementation.
- Do not put game rules, battle calculations, save conversion, or command parsing directly in widget build methods.
- Do not duplicate large sections of console gameplay flow in Flutter screens.
- Do not bypass existing save services or mappers once Flutter persistence is wired in.
- Do not add platform-specific behavior in shared Dart files without guarding or abstracting it.
- Do not make broad rewrites to the console project while working on targeted Flutter changes unless explicitly requested.
- Do not add legacy save migration or backwards-compatibility code for major changes/refactors unless explicitly requested.

# Current Plan

- Perform a full code review of the arcadia_flutter application.

# Current Task
- Complete a full code review of only the arcadia_flutter app. Look for design issues, code smells, poor architecture, unnessessary duplication, or areas that can be improved. Put your findings in the file "flutter_code_review.md"
