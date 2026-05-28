import 'dart:io';

import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/saves/game_save_state.dart';
import 'package:arcadia_flutter/saves/json_game_save_repository.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies JSON save/load preserves player, room, visit, and encounter state.
  test('json save and load round trips session state', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_save_test_',
    );
    addTearDown(() async {
      if (await tempDirectory.exists()) {
        await tempDirectory.delete(recursive: true);
      }
    });

    final saveFile = File(
      '${tempDirectory.path}${Platform.pathSeparator}save.json',
    );
    final repository = JsonGameSaveRepository(saveFile);
    final session = MobileGameSession(GameMap(), saveRepository: repository);

    session.startNewGame('Nova');
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    session.player.addStarFragment('Nature Star Fragment');
    session.player.addBond(AnimalElement.nature, 60);
    session.player.animalInventory.first.health = 10;
    session.currentRoom.removeEncounterAnimal(
      session.currentRoom.encounterAnimals.single,
    );

    await session.saveGame();

    final restoredSession = MobileGameSession(
      GameMap(),
      saveRepository: repository,
    );
    final loaded = await restoredSession.loadGame();

    expect(loaded, isTrue);
    expect(restoredSession.playerName, 'Nova');
    expect(restoredSession.currentRoom.id, RoomId.road1);
    expect(restoredSession.visitedRoomIds, {
      RoomId.maiaStable,
      RoomId.ikena,
      RoomId.road1,
    });
    expect(restoredSession.player.starFragments, ['Nature Star Fragment']);
    expect(restoredSession.player.getBond(AnimalElement.nature), 60);
    expect(restoredSession.player.animalInventory.first.health, 10);
    expect(restoredSession.currentRoom.encounterAnimals, isEmpty);
  });

  // Verifies loading reports false when no JSON save exists.
  test('loadGame returns false when no save exists', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_save_missing_test_',
    );
    addTearDown(() async {
      if (await tempDirectory.exists()) {
        await tempDirectory.delete(recursive: true);
      }
    });

    final saveFile = File(
      '${tempDirectory.path}${Platform.pathSeparator}save.json',
    );
    final repository = JsonGameSaveRepository(saveFile);
    final session = MobileGameSession(GameMap(), saveRepository: repository);

    expect(await session.hasSave(), isFalse);
    expect(await session.loadGame(), isFalse);
  });

  // Verifies unsupported current-build save versions fail deliberately.
  test('restoreSaveState rejects unsupported save version', () {
    final session = MobileGameSession(GameMap());
    final saveState = session.createSaveState();
    final unsupportedSaveState = GameSaveState(
      version: saveState.version + 1,
      player: saveState.player,
      rooms: saveState.rooms,
      visitedRoomIds: saveState.visitedRoomIds,
    );

    expect(
      () => session.restoreSaveState(unsupportedSaveState),
      throwsFormatException,
    );
  });
}
