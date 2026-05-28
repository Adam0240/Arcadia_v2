# Project Overview

arcadia_flutter is the Flutter client for Arcadia. It should provide a touch-friendly mobile UI for the Arcadia adventure/creature-battle game while preserving the main game's domain rules, terminology, and save behavior.

The Flutter project lives in `arcadia_flutter/`. The existing console game lives in `Arcadia_v2/`, the partially built .NET MAUI app lives in `Arcadia_Mobile/`, and unit tests live in `UnitTest/`.

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
- `arcadia_flutter/lib/player/` - mobile-adapted player models for human players, computer players, inventory, star fragments, bond, and battle team templates.
- `arcadia_flutter/lib/saves/` - Flutter JSON save state models, save/load mapper, and repository implementations.
- `arcadia_flutter/lib/services/` - gameplay/session state such as the current room, visited rooms, movement results, and interaction text.
- `arcadia_flutter/lib/screens/` - Flutter screens, including the playable map screen and its movement/menu controls.
- `arcadia_flutter/lib/widgets/` - reusable UI widgets such as custom room artwork.
- `arcadia_flutter/android/` - Android platform project and configuration.
- `arcadia_flutter/ios/`, `macos/`, `linux/`, `web/`, and `windows/` - generated platform folders. Do not edit platform files unless the feature requires platform-specific configuration.
- `arcadia_flutter/test/` - Flutter-specific tests for this project. Use this as the default location for new Flutter tests.
- `Arcadia_v2/` - console reference implementation only.
- `Arcadia_Mobile/` - .NET MAUI reference implementation only.

# Architecture Rules

- Treat `Arcadia_v2/` as the reference implementation for gameplay logic, not as an edit target for Flutter work.
- Treat `Arcadia_Mobile/` as a reference for the current mobile feature set and screen behavior, not as an edit target for Flutter work.
- Do not modify existing files in `Arcadia_v2/` or `Arcadia_Mobile/` unless the user explicitly instructs you to do so.
- Build the Flutter app to match the current behavior and visible functionality of `Arcadia_Mobile/` first.
- When Flutter needs logic from the console project, create an adapted Dart implementation in the appropriate `arcadia_flutter/lib/` folder instead of editing or directly depending on the C# source.
- Keep duplicated Flutter logic organized by responsibility so it can evolve separately from console-specific and MAUI-specific flow.
- Keep Flutter widgets focused on UI composition, navigation, and user interaction. Put gameplay decisions, battle rules, save mapping, and state transitions outside widget build methods.
- Keep reusable game/session behavior in services or state classes; keep mobile-adapted domain models in responsibility-based folders such as `map/`, `creatures/`, `battles/`, `player/`, and `saves/`.
- Expand the existing playable exploration flow incrementally. Add rooms, interactions, inventory, dialogue, saves, and battles as separate focused steps instead of folding them all into one widget or service.
- Do not copy console-specific flow into the Flutter app. Adapt the same domain concepts to screens, buttons, menus, dialogs, and mobile navigation.
- Do not use console input/output patterns in Flutter code.
- If shared gameplay code across projects is eventually needed, discuss that explicitly first. Do not extract or move code out of `Arcadia_v2/` as part of ordinary Flutter work.
- Preserve Arcadia-specific terminology and avoid third-party monster-collector IP references in user-facing text.

# Current Flutter Architecture

- `main.dart` starts `ArcadiaApp`, applies the app theme, and routes directly to `ArcadiaMapScreen`.
- `ArcadiaMapScreen` is the first playable screen. It displays the current room artwork, room name, description, status message, directional controls, Inspect, and Menu.
- Direction buttons are enabled only when the current room has an exit in that direction. Movement calls `MobileGameSession.move`, updates the current room, records the visit, and refreshes the status message.
- The Menu button swaps the movement controls for Inventory, Bond, Star Fragments, Save, Load, and Return controls. Inventory/Bond/Star Fragments are read-only status panel views backed by the active player. Save/Load use the Flutter JSON save repository.
- `GameMap` creates the full current room list and connects rooms with directional exits. The map starts at `RoomId.maiaStable`.
- `GameMap` also populates room encounter animals from `GameCreatureData` using the console reference assignments. Encounter animals are visible through Inspect only; battle and catch behavior are not wired yet.
- `Room`, `RoomId`, and `RoomDirection` are the mobile map domain model. `Room` owns local encounter animal state as cloned animals. Keep new room metadata, exits, and initial encounter assignments in `GameMap` unless the map grows enough to justify splitting data from construction.
- `MobileGameSession` owns the active `Player`, visited room ids, restore/start-new-game helpers, movement validation, and room interactions. Current-room state should flow through `player.currentRoom`; the session keeps a compatibility getter for UI code.
- New sessions create the main player with the console reference starter animals from `GameCreatureData.createAnimals()`: `N_CAT` and `N_DOG`.
- `saves/` contains the Flutter save/load layer. `GameSaveMapper` captures/restores session state, player state, visited rooms, and room encounter animals. `LocalJsonGameSaveRepository` stores `arcadia_save.json` in the app documents directory through `path_provider`; tests can use `JsonGameSaveRepository` with a temp file or an in-memory repository.
- `RoomArtwork` uses Flutter canvas painting for room visuals. Maia's Stable, Ikena, and Road 1 have specific art; the other rooms currently use the shared placeholder scene.
- `creatures/` contains the Flutter-side creature data layer. `AnimalCatalog` builds the generated animal roster from Dart species/element templates, `MoveCatalog` owns static battle moves, and `GameCreatureData` is the public facade for creating animal lists.
- `player/` contains the Flutter-side player state layer. `GenericPlayer` owns room position, star fragments, creature inventory, bond, display helpers, and restore helpers. `Player` is the human player type, and `CompPlayer` adds defeated state plus cloned battle team templates.

# Current Map Coverage

- Implemented rooms: Maia's Stable, Ikena, Road 1, Road 2, Oak Pass, Road 3, Road 4, New Nucleon, Road 5, Road 6, Road 7, Wyrmrest, Mountains, Radioactive Way, Nucleon, Final Trials, Guardian Tower, Road 8, and The End.
- Implemented traversal uses north/east/south/west exits from `GameMap._connectRooms()`. Add or change routes there so UI controls, session movement, and tests stay aligned.
- Placeholder rooms have descriptions and interaction text so the full map can be explored before every location has final art or full encounter logic.
- Wild animals have been assigned to route rooms from the console reference map and are displayed by Inspect as `Animals Nearby`.

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
- Do not modify existing files under `Arcadia_Mobile/` for Flutter implementation work unless the user explicitly asks for it.
- Do not move files out of `Arcadia_v2/` or `Arcadia_Mobile/` into shared projects or libraries without explicit approval.
- Do not reference console-only behavior directly from Flutter UI when the mobile app needs an adapted Dart implementation.
- Do not put game rules, battle calculations, save conversion, or command parsing directly in widget build methods.
- Do not duplicate large sections of console gameplay flow in Flutter screens.
- Do not bypass existing save services or mappers once Flutter persistence is wired in.
- Do not add platform-specific behavior in shared Dart files without guarding or abstracting it.
- Do not make broad rewrites to the console or MAUI projects while working on targeted Flutter changes unless explicitly requested.
- Do not add legacy save migration or backwards-compatibility code for major changes/refactors unless explicitly requested.

# Current Plan

- Continue building on the Flutter creature and player data layers by wiring them into map encounters, battles, and save/load only as those features are implemented.

# Current Task
- Creature data has been brought into `arcadia_flutter/lib/creatures/`, player state has been brought into `arcadia_flutter/lib/player/`, `MobileGameSession` now creates/owns the active player, map rooms now expose nearby wild animals through Inspect, the menu can show read-only inventory/bond/star fragment status, and JSON save/load is wired through `arcadia_flutter/lib/saves/`. Next work should integrate those data layers into battles and catching rather than editing the reference projects.
