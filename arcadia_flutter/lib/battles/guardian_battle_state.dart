import '../creatures/animal.dart';
import '../player/comp_player.dart';
import '../player/player.dart';
import 'battle_engine.dart';

class GuardianBattleState {
  GuardianBattleState._({
    required this.player,
    required this.guardian,
    required this.playerActiveIndex,
  });

  final Player player;
  final CompPlayer guardian;
  int playerActiveIndex;
  int guardianActiveIndex = 0;
  bool isComplete = false;

  Animal get playerAnimal => player.animalInventory[playerActiveIndex];
  Animal get guardianAnimal => guardian.animalInventory[guardianActiveIndex];
  bool get isGuardianPartyDefeated => !BattleEngine.hasUsableAnimals(guardian);
  bool get isPlayerPartyDefeated => !BattleEngine.hasUsableAnimals(player);
  List<int> get healthyPlayerSwitchIndexes {
    final indexes = <int>[];

    for (var i = 0; i < player.animalInventory.length; i += 1) {
      if (i != playerActiveIndex &&
          !BattleEngine.isDefeated(player.animalInventory[i])) {
        indexes.add(i);
      }
    }

    return indexes;
  }

  static GuardianBattleState create({
    required Player player,
    required CompPlayer guardian,
  }) {
    guardian.prepareForBattle();
    final nextAnimalIndex = BattleEngine.getNextHealthyAnimalIndex(player);

    return GuardianBattleState._(
      player: player,
      guardian: guardian,
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

  void switchPlayerAnimal(int index) {
    if (index < 0 || index >= player.animalInventory.length) {
      throw RangeError.index(index, player.animalInventory, 'index');
    }

    if (BattleEngine.isDefeated(player.animalInventory[index])) {
      throw StateError('Cannot switch to a defeated animal.');
    }

    playerActiveIndex = index;
  }

  bool useNextHealthyGuardianAnimal() {
    final nextAnimalIndex = BattleEngine.getNextHealthyAnimalIndex(
      guardian,
      startIndex: guardianActiveIndex + 1,
    );

    if (nextAnimalIndex == -1) {
      return false;
    }

    guardianActiveIndex = nextAnimalIndex;
    return true;
  }
}
