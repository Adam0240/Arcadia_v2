import '../creatures/animal.dart';
import '../creatures/battle_move.dart';
import '../creatures/move_effect.dart';
import '../player/generic_player.dart';
import '../player/player.dart';
import 'battle_move_result.dart';

class BattleEngine {
  const BattleEngine._();

  static const int maxPartySize = 6;

  static BattleMoveResult useMove({
    required Animal attacker,
    required Animal defender,
    required BattleMove move,
  }) {
    if (isHealingMove(move)) {
      final result = restoreHealth(attacker, move.power);

      return BattleMoveResult(
        type: result.type,
        moveName: move.name,
        amount: result.amount,
        targetHealth: result.targetHealth,
      );
    }

    final result = applyDamage(defender, move.power);

    return BattleMoveResult(
      type: result.type,
      moveName: move.name,
      amount: result.amount,
      targetHealth: result.targetHealth,
    );
  }

  static BattleMoveResult restoreHealth(Animal animal, int healingPower) {
    if (healingPower < 0) {
      throw RangeError.value(
        healingPower,
        'healingPower',
        'Healing power cannot be negative.',
      );
    }

    if (animal.health >= animal.baseHealth) {
      return BattleMoveResult(
        type: BattleMoveResultType.noEffect,
        moveName: '',
        amount: 0,
        targetHealth: animal.health,
      );
    }

    final originalHealth = animal.health;
    animal.health = (animal.health + healingPower).clamp(0, animal.baseHealth);

    return BattleMoveResult(
      type: BattleMoveResultType.healing,
      moveName: '',
      amount: animal.health - originalHealth,
      targetHealth: animal.health,
    );
  }

  static BattleMoveResult applyDamage(Animal defender, int damage) {
    if (damage < 0) {
      throw RangeError.value(damage, 'damage', 'Damage cannot be negative.');
    }

    defender.health = (defender.health - damage).clamp(0, defender.baseHealth);

    return BattleMoveResult(
      type: BattleMoveResultType.damage,
      moveName: '',
      amount: damage,
      targetHealth: defender.health,
    );
  }

  static bool isHealingMove(BattleMove move) {
    return move.effect == MoveEffect.heal;
  }

  static bool isDefeated(Animal animal) {
    return animal.health <= 0;
  }

  static bool hasUsableAnimals(GenericPlayer player) {
    return getNextHealthyAnimalIndex(player) >= 0;
  }

  static int getNextHealthyAnimalIndex(
    GenericPlayer player, {
    int startIndex = 0,
  }) {
    for (
      var index = startIndex;
      index < player.animalInventory.length;
      index++
    ) {
      if (!isDefeated(player.animalInventory[index])) {
        return index;
      }
    }

    return -1;
  }

  static bool tryCatchWildAnimal(Player player, Animal wildAnimal) {
    if (player.animalInventory.length >= maxPartySize) {
      return false;
    }

    player.addAnimal(wildAnimal);
    player.currentRoom.removeEncounterAnimal(wildAnimal);
    return true;
  }

  static void letWildAnimalRunAway(Player player, Animal wildAnimal) {
    player.currentRoom.removeEncounterAnimal(wildAnimal);
  }
}
