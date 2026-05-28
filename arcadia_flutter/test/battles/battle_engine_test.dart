import 'package:arcadia_flutter/battles/battle_engine.dart';
import 'package:arcadia_flutter/battles/battle_move_result.dart';
import 'package:arcadia_flutter/creatures/game_creature_data.dart';
import 'package:arcadia_flutter/creatures/move_catalog.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies damage moves reduce health without dropping below zero.
  test('useMove applies damage and clamps health at zero', () {
    final attacker = GameCreatureData.createAnimals()[1];
    final defender = GameCreatureData.createAnimals()[3]..health = 3;

    final result = BattleEngine.useMove(
      attacker: attacker,
      defender: defender,
      move: MoveCatalog.pounce,
    );

    expect(result.type, BattleMoveResultType.damage);
    expect(result.amount, 5);
    expect(result.targetHealth, 0);
    expect(defender.health, 0);
  });

  // Verifies healing moves restore health up to base health.
  test('useMove applies healing and clamps health at base health', () {
    final animal = GameCreatureData.createAnimals()[1]..health = 70;

    final result = BattleEngine.useMove(
      attacker: animal,
      defender: GameCreatureData.createAnimals()[3],
      move: MoveCatalog.bloom,
    );

    expect(result.type, BattleMoveResultType.healing);
    expect(result.amount, 5);
    expect(result.targetHealth, 75);
    expect(animal.health, 75);
  });

  // Verifies catching adds a wild animal to the player and removes it from the room.
  test('tryCatchWildAnimal adds animal and removes encounter', () {
    final session = MobileGameSession(GameMap())
      ..move(RoomDirection.north)
      ..move(RoomDirection.west);
    final wildAnimal = session.currentRoom.encounterAnimals.single;
    final initialInventoryCount = session.player.animalInventory.length;

    final caught = BattleEngine.tryCatchWildAnimal(session.player, wildAnimal);

    expect(caught, isTrue);
    expect(
      session.player.animalInventory,
      hasLength(initialInventoryCount + 1),
    );
    expect(session.player.animalInventory.last, same(wildAnimal));
    expect(session.currentRoom.encounterAnimals, isEmpty);
  });
}
