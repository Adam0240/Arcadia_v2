# Flutter Code Review

## Findings

### P1 - Wild battle `Run` stays enabled after a wild animal is defeated

References:
- `lib/services/mobile_game_session.dart:197`
- `lib/services/mobile_game_session.dart:199`
- `lib/services/mobile_game_session.dart:299`
- `lib/screens/wild_battle_screen.dart:37`
- `lib/screens/wild_battle_screen.dart:86`

When `useWildBattleMove` defeats the wild animal, it sets `battleState.isComplete = true` and returns `battleEnded: true`, but not `returnToMap`, so the player can choose `Catch` or `Leave`. The screen also keeps `Run` enabled because the button only checks `_needsPlayerSwitch`, not `battleState.isComplete` or `isWildDefeated`.

If the player taps `Run` at that point, `runFromWildBattle` returns to the map without removing the defeated encounter or restoring it. The room still reports a wild encounter, but the animal can be left in the room at `0` health. Re-entering the encounter can start a battle against an already-defeated wild animal and expose catch/leave behavior from an invalid state.

Suggested fix: disable `Run` once the wild animal is defeated, or make `runFromWildBattle` reject completed/defeated battle states. Add a widget/session test that defeats a wild animal, taps `Run`, and verifies the encounter state cannot remain as a defeated room encounter.

### P1 - The checked-in test suite is failing

References:
- `lib/game/flame/arcadia_player_component.dart:8`
- `lib/game/flame/arcadia_player_component.dart:9`
- `test/game/flame/arcadia_player_component_test.dart:97`

`flutter test` fails in `player applies the temporary Nature tint by default`. The implementation now defaults `enablePlayerElementTint` to `false` and the tint color to `Color.fromARGB(255, 249, 137, 0)`, while the test still expects tinting to be enabled with `Color(0xFF4CAF50)`.

This blocks the project from having a passing baseline and leaves the intended player tint behavior ambiguous. Either restore the expected default Nature tint or update the test/name/comments to match the new default-disabled behavior.

### P2 - Flame exploration parses door metadata but never uses it to update game state

References:
- `lib/game/flame/map_loader.dart:29`
- `lib/game/flame/map_loader.dart:96`
- `lib/game/flame/arcadia_game.dart:72`
- `lib/game/flame/arcadia_game.dart:230`
- `lib/screens/world_screen.dart:29`

The map loader extracts `DoorMetadata` and `ArcadiaGame` stores it, but the interaction path only converts taps into movement targets. No code checks whether the player reached a door, maps a door name to a `RoomDirection`/`RoomId`, calls `MobileGameSession.move`, or pops/refreshes `WorldScreen`. The `WorldScreen` title is driven by `gameSession.currentRoom`, but the Flame world cannot currently change that room.

The result is a split exploration model: the button map is authoritative, while Flame exploration is only a movement sandbox. That may be fine for a prototype, but the presence of door metadata makes it look like room transitions are partially implemented. Wire door bounds into session movement or remove/defer the unused door pipeline until it is supported.

### P2 - Save restore accepts partial room/guardian lists and silently merges them with defaults

References:
- `lib/services/mobile_game_session.dart:510`
- `lib/services/mobile_game_session.dart:512`
- `lib/services/mobile_game_session.dart:513`
- `lib/saves/game_save_mapper.dart:50`
- `lib/saves/game_save_mapper.dart:60`

`restoreSaveState` validates only the save version, then `restoreRooms` and `restoreGuardians` iterate over whatever entries are present. A malformed current-version save that omits rooms or guardians will not fail; omitted entities keep their constructor defaults. Loading that save can produce a hybrid state that never existed when saved.

For example, a save missing `RoomId.road8` would keep Road 8's default encounter/storage state, and a save missing a guardian would keep that guardian undefeated at its default room/team even if the player progress says otherwise. Current tests cover unsupported versions, but not incomplete current-version saves.

Suggested fix: validate that the saved room and guardian sets exactly match the current catalog before mutating the session, and fail the load with a `FormatException` if required entries are missing or duplicated.

### P3 - Map load failures are hidden from the player

References:
- `lib/game/flame/arcadia_game.dart:46`
- `lib/game/flame/arcadia_game.dart:63`
- `lib/game/flame/arcadia_game.dart:67`
- `lib/screens/world_screen.dart:34`
- `lib/screens/world_screen.dart:67`

`ArcadiaGame` stores `mapLoadError` when loading fails, but `WorldScreen` always shows the normal exploration hint. If the Tiled asset is missing or fails to load on a device, players see an empty green exploration area with normal controls and no explanation.

Suggested fix: expose the load state through a widget-facing callback/notifier or overlay, and show a concise error/fallback message when `mapLoaded` is false.

## Verification

- `flutter analyze` passes with no issues.
- `flutter test` fails with one failing test: `test/game/flame/arcadia_player_component_test.dart: player applies the temporary Nature tint by default`.
