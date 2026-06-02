import 'dart:convert';
import 'dart:io';

import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/creatures/game_creature_data.dart';
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

  // Verifies caught wild animals and removed room encounters persist through save/load.
  test('save and load preserves caught wild animal state', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_caught_save_test_',
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
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;
    session.catchWildAnimal(battleState);

    await session.saveGame();

    final restoredSession = MobileGameSession(
      GameMap(),
      saveRepository: repository,
    );
    final loaded = await restoredSession.loadGame();

    expect(loaded, isTrue);
    expect(restoredSession.currentRoom.id, RoomId.road1);
    expect(restoredSession.currentRoom.encounterAnimals, isEmpty);
    expect(
      restoredSession.player.animalInventory.map((animal) => animal.name),
      ['N_CAT', 'N_DOG', 'N_DOG'],
    );
  });

  // Verifies Road 8 stored animals persist separately from wild encounters.
  test('save and load preserves Road 8 stored animals', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_road8_storage_save_test_',
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
    final animals = GameCreatureData.createAnimals();
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);

    for (final animal in animals.skip(4).take(4)) {
      session.player.addAnimal(animal.clone());
    }

    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;
    session.catchWildAnimal(battleState);

    await session.saveGame();

    final restoredSession = MobileGameSession(
      GameMap(),
      saveRepository: repository,
    );
    final loaded = await restoredSession.loadGame();

    expect(loaded, isTrue);
    expect(restoredSession.currentRoom.id, RoomId.road1);
    expect(restoredSession.currentRoom.encounterAnimals, isEmpty);
    expect(restoredSession.road8StoredAnimals.map((animal) => animal.name), [
      'N_DOG',
    ]);
    expect(restoredSession.road8Room.encounterAnimals, isEmpty);
  });

  // Verifies guardian defeated state persists through save/load.
  test('save and load preserves guardian state', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_guardian_save_test_',
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
    final natureGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Nature Guardian',
    );
    final elementalTitan = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Elemental Titan',
    );
    natureGuardian.character.defeated = true;
    elementalTitan.character.defeated = true;

    await session.saveGame();

    final restoredSession = MobileGameSession(
      GameMap(),
      saveRepository: repository,
    );
    final loaded = await restoredSession.loadGame();
    final restoredNatureGuardian = restoredSession.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Nature Guardian',
    );
    final restoredElementalTitan = restoredSession.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Elemental Titan',
    );

    expect(loaded, isTrue);
    expect(restoredNatureGuardian.defeated, isTrue);
    expect(restoredElementalTitan.defeated, isTrue);
    expect(restoredNatureGuardian.character.currentRoom.id, RoomId.oakPass);
    expect(
      restoredElementalTitan.character.currentRoom.id,
      RoomId.guardiansTower,
    );
    expect(
      restoredNatureGuardian.character.battleTeamTemplate.map(
        (animal) => animal.name,
      ),
      ['N_DOG', 'N_BEAR'],
    );
  });

  // Verifies grown animals and reset bond persist through save/load.
  test('save and load preserves grown animal state', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_growth_save_test_',
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
    session.player.addBond(AnimalElement.nature, 100);
    session.growAnimal(
      session.growthOptions.singleWhere(
        (option) => option.currentAnimal.name == 'N_CAT',
      ),
    );

    await session.saveGame();

    final restoredSession = MobileGameSession(
      GameMap(),
      saveRepository: repository,
    );
    final loaded = await restoredSession.loadGame();

    expect(loaded, isTrue);
    expect(
      restoredSession.player.animalInventory.map((animal) => animal.name),
      ['N_LION', 'N_DOG'],
    );
    expect(restoredSession.player.getBond(AnimalElement.nature), 0);
  });

  // Verifies unsupported current-build save versions fail deliberately.
  test('restoreSaveState rejects unsupported save version', () {
    final session = MobileGameSession(GameMap());
    final saveState = session.createSaveState();
    final unsupportedSaveState = GameSaveState(
      version: saveState.version + 1,
      player: saveState.player,
      rooms: saveState.rooms,
      guardians: saveState.guardians,
      visitedRoomIds: saveState.visitedRoomIds,
    );

    expect(
      () => session.restoreSaveState(unsupportedSaveState),
      throwsFormatException,
    );
  });

  // Verifies pre-current version 3 saves are not silently accepted.
  test('restoreSaveState rejects previous version 3 save', () {
    final session = MobileGameSession(GameMap());
    final saveState = session.createSaveState();
    final oldVersionSaveState = GameSaveState(
      version: 3,
      player: saveState.player,
      rooms: saveState.rooms,
      guardians: saveState.guardians,
      visitedRoomIds: saveState.visitedRoomIds,
    );

    expect(
      () => session.restoreSaveState(oldVersionSaveState),
      throwsFormatException,
    );
  });

  // Verifies save loading reports malformed root JSON through FormatException.
  test('json load rejects non-object save root', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_bad_root_save_test_',
    );
    addTearDown(() async {
      if (await tempDirectory.exists()) {
        await tempDirectory.delete(recursive: true);
      }
    });

    final saveFile = File(
      '${tempDirectory.path}${Platform.pathSeparator}save.json',
    );
    await saveFile.writeAsString('[]');

    expect(
      JsonGameSaveRepository(saveFile).load(),
      throwsA(
        isA<FormatException>().having(
          (error) => error.message,
          'message',
          contains('root'),
        ),
      ),
    );
  });

  // Verifies missing required save fields report a controlled FormatException.
  test('json load rejects missing required save field', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_missing_field_save_test_',
    );
    addTearDown(() async {
      if (await tempDirectory.exists()) {
        await tempDirectory.delete(recursive: true);
      }
    });

    final saveFile = File(
      '${tempDirectory.path}${Platform.pathSeparator}save.json',
    );
    final saveJson = MobileGameSession(GameMap()).createSaveState().toJson()
      ..remove('player');
    await saveFile.writeAsString(jsonEncode(saveJson));

    expect(
      JsonGameSaveRepository(saveFile).load(),
      throwsA(
        isA<FormatException>().having(
          (error) => error.message,
          'message',
          contains('player'),
        ),
      ),
    );
  });

  // Verifies invalid enum names report a controlled FormatException.
  test('json load rejects invalid enum value', () async {
    final tempDirectory = await Directory.systemTemp.createTemp(
      'arcadia_invalid_enum_save_test_',
    );
    addTearDown(() async {
      if (await tempDirectory.exists()) {
        await tempDirectory.delete(recursive: true);
      }
    });

    final saveFile = File(
      '${tempDirectory.path}${Platform.pathSeparator}save.json',
    );
    final saveJson = MobileGameSession(GameMap()).createSaveState().toJson();
    final playerJson = Map<String, Object?>.from(saveJson['player'] as Map)
      ..['currentRoomId'] = 'unknownRoom';
    saveJson['player'] = playerJson;
    await saveFile.writeAsString(jsonEncode(saveJson));

    expect(
      JsonGameSaveRepository(saveFile).load(),
      throwsA(
        isA<FormatException>().having(
          (error) => error.message,
          'message',
          contains('currentRoomId'),
        ),
      ),
    );
  });
}
