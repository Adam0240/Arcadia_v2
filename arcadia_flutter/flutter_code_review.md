# Flutter Code Review

Scope: `arcadia_flutter` only. This review focused on design issues, code smells, architecture, duplication, and maintainability risks.

## Findings

### Complete: save version was not advanced after incompatible progression changes

- File: `lib/saves/game_save_mapper.dart:13`
- Related restore path: `lib/saves/game_save_mapper.dart:60`, `lib/services/mobile_game_session.dart:510`

Original finding: `GameSaveMapper.currentVersion` was still `3`, but recent gameplay changes added new progression-critical state and meaning: the Elemental Titan guardian, final ending state, revised guardian locations/requirements, Road 8 storage behavior, and movement gates. The restore path validated only the numeric version, then restored whatever guardian records existed in the save. That meant an older version-3 save from before the latest features could be accepted as current, leaving newly added default state mixed with old saved state.

Resolution: bumped `GameSaveMapper.currentVersion` to `4` and added `restoreSaveState rejects previous version 3 save` in `test/saves/game_save_test.dart:281`. Older version-3 saves are now rejected deliberately instead of being mixed with the current progression state.

### Complete: `MobileGameSession` is carrying too many responsibilities

- File: `lib/services/mobile_game_session.dart:26`
- Examples: movement at `lib/services/mobile_game_session.dart:109`, wild battle flow at `lib/services/mobile_game_session.dart:180`, guardian battle flow at `lib/services/mobile_game_session.dart:361`, save/load at `lib/services/mobile_game_session.dart:482`, movement gates at `lib/services/mobile_game_session.dart:609`

Original finding: `MobileGameSession` owned map traversal, route requirements, wild battle resolution, guardian/titan battle resolution, catch/storage behavior, healing, growth, final-room messaging, save/load, guardian initialization/reset, reward rules, and many UI-facing strings. That made it the central place for nearly every gameplay change, increasing regression risk and making isolated testing harder.

Resolution: extracted guardian setup, reset, availability checks, intro/title text, challenge labels, and victory rewards into `lib/services/guardian_progression.dart`. Also routed repository save/load/exists/delete calls through `lib/services/game_session_persistence.dart`. `MobileGameSession` keeps the existing public API for the UI/tests, but it now delegates guardian progression and persistence responsibilities to focused collaborators.

### Complete: wild and guardian battle screens duplicate the same UI components and flow shape

- Files: `lib/screens/wild_battle_screen.dart`, `lib/screens/guardian_battle_screen.dart`
- Duplicates: `_AnimalBattlePanel`, `_BattleMessage`, `_BattleButton`, switch-animal rendering, move-button rendering, and result application patterns

Original finding: the wild and guardian battle screens were similar enough that UI changes had to be made twice. This was visible in the duplicated private widgets and nearly identical battle panels/messages/buttons. Future battle UX changes, accessibility tweaks, or styling updates could drift between the two screens.

Resolution: extracted `BattleAnimalPanel`, `BattleMessage`, `BattleButton`, `BattleActionList`, `BattleAction`, and `BattleSwitchOption` into `lib/widgets/battle_widgets.dart`. Both battle screens now share the common panels, message styling, button styling, switch-option rendering, and move-button rendering while keeping their battle-specific actions and result handling local.

### Complete: `ArcadiaMapScreen` mixes map presentation, menu composition, navigation, and command handling

- File: `lib/screens/arcadia_map_screen.dart:15`
- Extracted controls: `lib/widgets/map_controls.dart:5`, `lib/widgets/map_controls.dart:92`, `lib/widgets/map_controls.dart:176`
- Centralized navigation/status helpers: `lib/screens/arcadia_map_screen.dart:299`, `lib/screens/arcadia_map_screen.dart:319`

Original finding: `ArcadiaMapScreen` had become the main UI coordinator for movement, inspect/status text, conditional encounter buttons, guardian buttons, menu actions, save handling, growth/swap/reorder navigation, and the ending dialog. The widget was still readable, but it was trending toward a screen-level controller with many unrelated reasons to change.

Resolution: extracted movement and menu button composition into `MapDirectionControls` and `MapMenuControls` in `lib/widgets/map_controls.dart`. Also centralized repeated screen navigation/status updates through `_pushStatusScreen`, `_setStatusIfMounted`, and `_setStatus` in `ArcadiaMapScreen`. The screen still coordinates gameplay actions, but the control layout and repeated route-result handling are no longer embedded throughout the widget.

### Complete: save JSON parsing is brittle for malformed or partial data

- File: `lib/saves/game_save_state.dart:31`
- Parser helpers: `lib/saves/game_save_state.dart:264`, `lib/saves/game_save_state.dart:344`, `lib/saves/game_save_state.dart:372`
- Repository root validation: `lib/saves/json_game_save_repository.dart:26`

Original finding: the save-state factories cast directly from decoded JSON, and `_parseEnum` used `singleWhere` without a controlled error path. Bad or partial save files could therefore throw `TypeError`, `StateError`, or other runtime exceptions instead of a consistent `FormatException` with a useful field-level message. The start menu caught broad errors, so the app could recover, but debugging bad saves and writing precise tests was harder than it needed to be.

Resolution: added typed parser helpers for required fields, strings, integers, booleans, objects, lists, object lists, string lists, enum fields, and enum lists in `game_save_state.dart`. Invalid or missing save data now reports `FormatException` with the relevant field name. `JsonGameSaveRepository.load` also validates that the decoded JSON root is an object before constructing `GameSaveState`.

## Test Gaps

- Complete: previous-version save rejection is covered by `test/saves/game_save_test.dart:281`.
- Complete: malformed-save parsing is covered by `test/saves/game_save_test.dart:300`, `test/saves/game_save_test.dart:328`, and `test/saves/game_save_test.dart:358`.

## Notes

The app has good coverage around recent gameplay behavior, including guardian location requirements, Road 8 storage, final ending autosave, and save/load round trips. The main risks now are less about missing gameplay features and more about keeping the growing Flutter version maintainable as features continue to land.
