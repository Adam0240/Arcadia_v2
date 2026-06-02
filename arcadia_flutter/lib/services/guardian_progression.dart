import '../battles/battle_engine.dart';
import '../battles/guardian_battle_result.dart';
import '../battles/guardian_battle_state.dart';
import '../creatures/game_creature_data.dart';
import '../guardians/guardian_catalog.dart';
import '../guardians/guardian_state.dart';
import '../map/game_map.dart';
import '../map/room.dart';
import '../player/comp_player.dart';
import '../player/player.dart';

class GuardianProgression {
  GuardianProgression(this._gameMap) {
    _guardians = _createGuardians(_gameMap);
  }

  final GameMap _gameMap;
  late final List<GuardianState> _guardians;

  Iterable<GuardianState> get guardians => List.unmodifiable(_guardians);

  GuardianState? guardianInRoom(Room room) {
    for (final guardian in _guardians) {
      if (guardian.character.currentRoom.id == room.id) {
        return guardian;
      }
    }

    return null;
  }

  GuardianState get elementalTitan {
    return _guardians.singleWhere(
      (guardian) => guardian.definition.isElementalTitan,
    );
  }

  void reset() {
    final guardianAnimals = GameCreatureData.createAnimals();

    for (final guardian in _guardians) {
      guardian.character
        ..defeated = false
        ..moveTo(_gameMap.getRoom(guardian.definition.roomId))
        ..restoreStarFragments([guardian.definition.rewardStarFragment])
        ..setBattleTeam(
          guardian.definition.teamAnimalIndexes.map(
            (animalIndex) => guardianAnimals[animalIndex],
          ),
        );
    }
  }

  String? getUnavailableMessage({
    required GuardianState? guardian,
    required Player player,
  }) {
    if (guardian == null) {
      return 'No guardian in area.';
    }

    if (guardian.defeated) {
      return guardian.definition.isElementalTitan
          ? 'You already defeated the Elemental Titan. Perhaps a little ways north will provide one final challenge.'
          : "You already defeated this sanctuary's guardian.";
    }

    final requiredStarFragments = guardian.definition.requiredStarFragments;
    if (player.starFragments.length < requiredStarFragments) {
      return guardian.definition.notEnoughStarFragmentsMessage ??
          'You need to have $requiredStarFragments star fragments to battle this guardian!';
    }

    if (!BattleEngine.hasUsableAnimals(player)) {
      return 'All animals in your party are defeated.';
    }

    return null;
  }

  GuardianBattleState startBattle({
    required GuardianState? guardian,
    required Player player,
  }) {
    final unavailableMessage = getUnavailableMessage(
      guardian: guardian,
      player: player,
    );
    if (unavailableMessage != null) {
      throw StateError(unavailableMessage);
    }

    return GuardianBattleState.create(
      player: player,
      guardian: guardian!.character,
    );
  }

  String getIntro(GuardianBattleState battleState, Player player) {
    final guardian = _getGuardianStateForCharacter(battleState.guardian);
    return [
      ...guardian.definition.introLines,
      '${player.name} vs ${battleState.guardian.name}',
      'You sent out ${battleState.playerAnimal.name}.',
      '${battleState.guardian.name} sent out ${battleState.guardianAnimal.name}.',
    ].join('\n');
  }

  String getCurrentChallengeActionLabel(GuardianState? guardian) {
    return guardian?.definition.isElementalTitan == true
        ? 'Elemental Titan'
        : 'Guardian';
  }

  String getBattleTitle(GuardianBattleState battleState) {
    final guardian = _getGuardianStateForCharacter(battleState.guardian);
    return guardian.definition.isElementalTitan
        ? 'Elemental Titan Battle'
        : 'Guardian Battle';
  }

  GuardianBattleResult finishVictory({
    required GuardianBattleState battleState,
    required Player player,
    required List<String> messages,
  }) {
    final guardian = _getGuardianStateForCharacter(battleState.guardian);

    battleState.isComplete = true;
    guardian.character.defeated = true;
    player.addStarFragment(guardian.definition.rewardStarFragment);
    player.addBond(guardian.definition.rewardElement, 100);

    messages.add('${guardian.character.name} defeated.');
    messages.add(
      'Congratulations! You defeated me. Please take this star fragment to honor your victory.',
    );

    return GuardianBattleResult(
      message: messages.join('\n'),
      battleEnded: true,
      returnToMap: true,
    );
  }

  static List<GuardianState> _createGuardians(GameMap gameMap) {
    final guardianAnimals = GameCreatureData.createAnimals();

    return GuardianCatalog.definitions.map((definition) {
      final guardian =
          CompPlayer(
              name: definition.name,
              startingRoom: gameMap.getRoom(definition.roomId),
            )
            ..setBattleTeam(
              definition.teamAnimalIndexes.map(
                (animalIndex) => guardianAnimals[animalIndex],
              ),
            )
            ..addStarFragment(definition.rewardStarFragment);

      return GuardianState(definition: definition, character: guardian);
    }).toList();
  }

  GuardianState _getGuardianStateForCharacter(CompPlayer character) {
    return _guardians.singleWhere(
      (guardian) => guardian.character == character,
    );
  }
}
