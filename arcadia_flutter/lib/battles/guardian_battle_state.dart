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
