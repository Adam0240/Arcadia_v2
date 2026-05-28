import '../creatures/animal.dart';
import '../player/player.dart';
import 'battle_engine.dart';

class WildBattleState {
  WildBattleState._({
    required this.player,
    required this.wildAnimal,
    required this.playerActiveIndex,
  });

  final Player player;
  final Animal wildAnimal;
  int playerActiveIndex;
  bool isComplete = false;

  Animal get playerAnimal => player.animalInventory[playerActiveIndex];
  bool get isWildDefeated => BattleEngine.isDefeated(wildAnimal);
  bool get isPlayerPartyDefeated => !BattleEngine.hasUsableAnimals(player);

  static WildBattleState create({
    required Player player,
    required Animal wildAnimal,
  }) {
    final nextAnimalIndex = BattleEngine.getNextHealthyAnimalIndex(player);

    return WildBattleState._(
      player: player,
      wildAnimal: wildAnimal,
      playerActiveIndex: nextAnimalIndex == -1 ? 0 : nextAnimalIndex,
    );
  }

  bool useFirstHealthyPlayerAnimal() {
    final nextAnimalIndex = BattleEngine.getNextHealthyAnimalIndex(player);

    if (nextAnimalIndex == -1) {
      playerActiveIndex = 0;
      return false;
    }

    playerActiveIndex = nextAnimalIndex;
    return true;
  }
}
