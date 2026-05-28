import 'package:arcadia_flutter/creatures/game_creature_data.dart';
import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies starting a wild battle uses the player's healthy lead and room encounter.
  test('startWildBattle uses current room encounter animal', () {
    final session = _createRoadOneSession();

    final battleState = session.startWildBattle();

    expect(battleState.playerAnimal.name, 'N_CAT');
    expect(battleState.wildAnimal.name, 'N_DOG');
  });

  // Verifies defeating and catching a wild animal updates inventory and room state.
  test('catch defeated wild animal adds inventory and removes encounter', () {
    final session = _createRoadOneSession();
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;

    final result = session.catchWildAnimal(battleState);

    expect(result.returnToMap, isTrue);
    expect(result.message, 'You caught N_DOG!');
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_CAT',
      'N_DOG',
      'N_DOG',
    ]);
    expect(session.currentRoom.encounterAnimals, isEmpty);
  });

  // Verifies running from battle leaves the room encounter available.
  test('run from wild battle keeps encounter in room', () {
    final session = _createRoadOneSession();
    final battleState = session.startWildBattle();

    final result = session.runFromWildBattle(battleState);

    expect(result.returnToMap, isTrue);
    expect(result.message, 'You ran from the battle.');
    expect(session.currentRoom.encounterAnimals.map((animal) => animal.name), [
      'N_DOG',
    ]);
  });

  // Verifies leaving a defeated animal removes the room encounter.
  test('leave defeated wild animal removes encounter', () {
    final session = _createRoadOneSession();
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;

    final result = session.leaveWildAnimal(battleState);

    expect(result.returnToMap, isTrue);
    expect(result.message, 'N_DOG ran away!');
    expect(session.currentRoom.encounterAnimals, isEmpty);
  });

  // Verifies defeating a wild animal with a matching fragment awards bond.
  test('defeating wild animal awards matching bond when fragment exists', () {
    final session = _createRoadOneSession();
    session.player.addStarFragment('Nature Star Fragment');
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 1;

    final result = session.useWildBattleMove(
      battleState,
      battleState.playerAnimal.moves.first,
    );

    expect(result.battleEnded, isTrue);
    expect(session.player.getBond(AnimalElement.nature), 50);
  });

  // Verifies catching with a full inventory stores the animal at Road 8.
  test('full inventory catch sends wild animal to Road 8 storage', () {
    final session = _createRoadOneSession();
    _fillParty(session);
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;

    final result = session.catchWildAnimal(battleState);

    expect(result.returnToMap, isTrue);
    expect(result.message, 'You caught N_DOG! It was sent to Road 8.');
    expect(session.player.animalInventory, hasLength(6));
    expect(session.currentRoom.encounterAnimals, isEmpty);
    expect(session.road8StoredAnimals.map((animal) => animal.name), ['N_DOG']);
    expect(session.road8Room.hasEncounterAnimals(), isFalse);
  });

  // Verifies Road 8 swaps one stored animal with one party animal.
  test('swapStoredAnimal exchanges inventory and Road 8 storage animals', () {
    final session = _createRoadOneSession();
    _fillParty(session);
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;
    session.catchWildAnimal(battleState);
    session.move(RoomDirection.north);

    expect(session.currentRoom.id, RoomId.road8);
    expect(session.canSwapStoredAnimals, isTrue);

    final partyAnimal = session.player.animalInventory.first;
    final storedAnimal = session.road8StoredAnimals.single;
    session.swapStoredAnimal(
      partyAnimal: partyAnimal,
      storedAnimal: storedAnimal,
    );

    expect(session.player.animalInventory.contains(storedAnimal), isTrue);
    expect(session.player.animalInventory.contains(partyAnimal), isFalse);
    expect(session.road8StoredAnimals, [partyAnimal]);
  });

  // Verifies starting over clears Road 8 captured storage on a reused session.
  test('startNewGame clears Road 8 stored animals', () {
    final session = _createRoadOneSession();
    _fillParty(session);
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;
    session.catchWildAnimal(battleState);

    session.startNewGame('New Player');

    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(session.road8StoredAnimals, isEmpty);
    expect(session.playerName, 'New Player');
  });
}

MobileGameSession _createRoadOneSession() {
  return MobileGameSession(GameMap())
    ..move(RoomDirection.north)
    ..move(RoomDirection.west);
}

void _fillParty(MobileGameSession session) {
  final animals = GameCreatureData.createAnimals();

  for (final animal in animals.skip(4).take(4)) {
    session.player.addAnimal(animal.clone());
  }
}
