import '../battles/battle_engine.dart';
import '../battles/guardian_battle_result.dart';
import '../battles/guardian_battle_state.dart';
import '../battles/battle_move_result.dart';
import '../battles/wild_battle_result.dart';
import '../battles/wild_battle_state.dart';
import '../creatures/animal.dart';
import '../creatures/animal_element.dart';
import '../creatures/battle_move.dart';
import '../creatures/game_creature_data.dart';
import '../guardians/guardian_catalog.dart';
import '../guardians/guardian_state.dart';
import '../map/game_map.dart';
import '../map/room.dart';
import '../map/room_direction.dart';
import '../map/room_id.dart';
import '../player/comp_player.dart';
import '../player/player.dart';
import '../saves/game_save_mapper.dart';
import '../saves/game_save_repository.dart';
import '../saves/game_save_state.dart';
import '../saves/local_json_game_save_repository.dart';
import 'move_result.dart';

class MobileGameSession {
  MobileGameSession(
    this._gameMap, {
    this.saveRepository = const LocalJsonGameSaveRepository(),
  }) {
    player = _createPlayer('Player', _gameMap.startRoom);
    _guardians = _createGuardians(_gameMap);
    _visitedRoomIds.add(currentRoom.id);
  }

  final GameMap _gameMap;
  final GameSaveRepository saveRepository;
  final Set<RoomId> _visitedRoomIds = {};
  late final List<GuardianState> _guardians;

  late Player player;

  Room get currentRoom => player.currentRoom;
  String get playerName => player.name;

  Iterable<Room> get rooms => _gameMap.rooms;
  Iterable<GuardianState> get guardians => List.unmodifiable(_guardians);
  Set<RoomId> get visitedRoomIds => Set.unmodifiable(_visitedRoomIds);
  bool get hasWildEncounter => currentRoom.hasEncounterAnimals();
  bool get hasGuardianInCurrentRoom => currentGuardian != null;
  GuardianState? get currentGuardian {
    for (final guardian in _guardians) {
      if (guardian.character.currentRoom.id == currentRoom.id) {
        return guardian;
      }
    }

    return null;
  }

  List<Animal> get currentEncounterAnimals => currentRoom.encounterAnimals;
  Room get road8Room => _gameMap.getRoom(RoomId.road8);
  List<Animal> get road8StoredAnimals => road8Room.storedAnimals;
  bool get canSwapStoredAnimals {
    return currentRoom.id == RoomId.road8 &&
        road8Room.hasStoredAnimals() &&
        player.animalInventory.isNotEmpty;
  }

  void startNewGame(String playerName) {
    if (playerName.trim().isEmpty) {
      throw ArgumentError('Player name cannot be empty.');
    }

    _gameMap.resetEncounterAnimals();
    _gameMap.resetStoredAnimals();
    _resetGuardians();
    player = _createPlayer(playerName.trim(), _gameMap.startRoom);
    _visitedRoomIds.clear();
    _visitedRoomIds.add(currentRoom.id);
  }

  void restore(
    String playerName,
    RoomId currentRoomId,
    Iterable<RoomId> visitedRoomIds,
  ) {
    final currentRoom = _gameMap.getRoom(currentRoomId);
    player = _createPlayer(playerName.trim(), currentRoom);
    _visitedRoomIds
      ..clear()
      ..addAll(visitedRoomIds)
      ..add(currentRoom.id);
  }

  bool canMove(RoomDirection direction) {
    return currentRoom.hasExit(direction);
  }

  MoveResult move(RoomDirection direction) {
    final destination = currentRoom.getExit(direction);

    if (destination == null) {
      return const MoveResult(
        moved: false,
        message: 'You cannot travel that way from here.',
      );
    }

    player.moveTo(destination);
    _visitedRoomIds.add(currentRoom.id);

    return MoveResult(moved: true, message: 'Moved to ${currentRoom.name}.');
  }

  String interact() {
    final nearbyAnimals = currentRoom.hasEncounterAnimals()
        ? currentRoom.encounterAnimals.map((animal) => animal.name).join(', ')
        : 'None';
    final storedAnimals = currentRoom.hasStoredAnimals()
        ? currentRoom.storedAnimals.map((animal) => animal.name).join(', ')
        : 'None';

    if (currentRoom.id == RoomId.road8) {
      return '${currentRoom.interactionText}\nAnimals Nearby: $nearbyAnimals\nStored Animals: $storedAnimals';
    }

    return '${currentRoom.interactionText}\nAnimals Nearby: $nearbyAnimals';
  }

  WildBattleState startWildBattle() {
    if (!BattleEngine.hasUsableAnimals(player)) {
      throw StateError('All animals in your party are defeated.');
    }

    if (!currentRoom.hasEncounterAnimals()) {
      throw StateError('No animals nearby.');
    }

    return WildBattleState.create(
      player: player,
      wildAnimal: currentRoom.encounterAnimals.first,
    );
  }

  WildBattleResult useWildBattleMove(
    WildBattleState battleState,
    BattleMove move,
  ) {
    if (battleState.isComplete) {
      return const WildBattleResult(
        message: 'The battle is already over.',
        battleEnded: true,
      );
    }

    final messages = <String>[];
    final playerResult = BattleEngine.useMove(
      attacker: battleState.playerAnimal,
      defender: battleState.wildAnimal,
      move: move,
    );
    messages.add(_formatMoveResult(battleState.playerAnimal, playerResult));

    if (battleState.isWildDefeated) {
      _awardWildBattleProgress(battleState.wildAnimal);
      battleState.isComplete = true;
      messages.add('${battleState.wildAnimal.name} defeated.');
      messages.add('Catch ${battleState.wildAnimal.name} or leave it behind.');

      return WildBattleResult(message: messages.join('\n'), battleEnded: true);
    }

    final wildMove = _selectComputerMove(battleState.wildAnimal);
    final wildResult = BattleEngine.useMove(
      attacker: battleState.wildAnimal,
      defender: battleState.playerAnimal,
      move: wildMove,
    );
    messages.add(_formatMoveResult(battleState.wildAnimal, wildResult));

    if (BattleEngine.isDefeated(battleState.playerAnimal)) {
      final defeatedAnimalName = battleState.playerAnimal.name;

      if (battleState.useFirstHealthyPlayerAnimal()) {
        messages.add(
          '$defeatedAnimalName defeated. ${battleState.playerAnimal.name} steps in.',
        );
      } else {
        battleState.isComplete = true;
        messages.add('$defeatedAnimalName defeated.');
        messages.add('Battle lost, all animals in your party are defeated.');

        return WildBattleResult(
          message: messages.join('\n'),
          battleEnded: true,
        );
      }
    }

    return WildBattleResult(message: messages.join('\n'));
  }

  WildBattleResult catchWildAnimal(WildBattleState battleState) {
    if (!battleState.isWildDefeated) {
      return WildBattleResult(
        message: 'Defeat ${battleState.wildAnimal.name} before catching it.',
      );
    }

    if (player.animalInventory.length >= BattleEngine.maxPartySize) {
      currentRoom.removeEncounterAnimal(battleState.wildAnimal);
      storeCapturedAnimalAtRoad8(battleState.wildAnimal);
      battleState.isComplete = true;

      return WildBattleResult(
        message:
            'You caught ${battleState.wildAnimal.name}! It was sent to Road 8.',
        battleEnded: true,
        returnToMap: true,
      );
    }

    BattleEngine.tryCatchWildAnimal(player, battleState.wildAnimal);
    battleState.isComplete = true;

    return WildBattleResult(
      message: 'You caught ${battleState.wildAnimal.name}!',
      battleEnded: true,
      returnToMap: true,
    );
  }

  WildBattleResult leaveWildAnimal(WildBattleState battleState) {
    if (!battleState.isWildDefeated) {
      return WildBattleResult(
        message: '${battleState.wildAnimal.name} is still ready to fight.',
      );
    }

    BattleEngine.letWildAnimalRunAway(player, battleState.wildAnimal);
    battleState.isComplete = true;

    return WildBattleResult(
      message: '${battleState.wildAnimal.name} ran away!',
      battleEnded: true,
      returnToMap: true,
    );
  }

  WildBattleResult runFromWildBattle(WildBattleState battleState) {
    battleState.isComplete = true;

    return const WildBattleResult(
      message: 'You ran from the battle.',
      battleEnded: true,
      returnToMap: true,
    );
  }

  String? getGuardianUnavailableMessage() {
    final guardian = currentGuardian;

    if (guardian == null) {
      return 'No guardian in area.';
    }

    if (guardian.defeated) {
      return "You already defeated this sanctuary's guardian.";
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

  GuardianBattleState startGuardianBattle() {
    final unavailableMessage = getGuardianUnavailableMessage();
    if (unavailableMessage != null) {
      throw StateError(unavailableMessage);
    }

    return GuardianBattleState.create(
      player: player,
      guardian: currentGuardian!.character,
    );
  }

  String getGuardianIntro(GuardianBattleState battleState) {
    final guardian = _getGuardianStateForCharacter(battleState.guardian);
    return [
      ...guardian.definition.introLines,
      '${player.name} vs ${battleState.guardian.name}',
      'You sent out ${battleState.playerAnimal.name}.',
      '${battleState.guardian.name} sent out ${battleState.guardianAnimal.name}.',
    ].join('\n');
  }

  GuardianBattleResult useGuardianBattleMove(
    GuardianBattleState battleState,
    BattleMove move,
  ) {
    if (battleState.isComplete) {
      return const GuardianBattleResult(
        message: 'The battle is already over.',
        battleEnded: true,
      );
    }

    final messages = <String>[];
    final playerResult = BattleEngine.useMove(
      attacker: battleState.playerAnimal,
      defender: battleState.guardianAnimal,
      move: move,
    );
    messages.add(_formatMoveResult(battleState.playerAnimal, playerResult));

    if (BattleEngine.isDefeated(battleState.guardianAnimal)) {
      final defeatedAnimalName = battleState.guardianAnimal.name;

      if (battleState.useNextHealthyGuardianAnimal()) {
        messages.add('$defeatedAnimalName defeated.');
        messages.add(
          '${battleState.guardian.name} sent out ${battleState.guardianAnimal.name}.',
        );
      } else {
        return _finishGuardianVictory(battleState, messages);
      }
    }

    final guardianMove = _selectComputerMove(battleState.guardianAnimal);
    final guardianResult = BattleEngine.useMove(
      attacker: battleState.guardianAnimal,
      defender: battleState.playerAnimal,
      move: guardianMove,
    );
    messages.add(_formatMoveResult(battleState.guardianAnimal, guardianResult));

    if (BattleEngine.isDefeated(battleState.playerAnimal)) {
      final defeatedAnimalName = battleState.playerAnimal.name;

      if (battleState.useFirstHealthyPlayerAnimal()) {
        messages.add(
          '$defeatedAnimalName defeated. ${battleState.playerAnimal.name} steps in.',
        );
      } else {
        battleState.isComplete = true;
        messages.add('$defeatedAnimalName defeated.');
        messages.add('Battle lost, all animals in your party are defeated.');

        return GuardianBattleResult(
          message: messages.join('\n'),
          battleEnded: true,
          returnToMap: true,
        );
      }
    }

    return GuardianBattleResult(message: messages.join('\n'));
  }

  void storeCapturedAnimalAtRoad8(Animal animal) {
    road8Room.storeCapturedAnimal(animal);
  }

  void swapStoredAnimal({
    required Animal partyAnimal,
    required Animal storedAnimal,
  }) {
    if (currentRoom.id != RoomId.road8) {
      throw StateError('Stored animals can only be swapped at Road 8.');
    }

    if (!player.animalInventory.contains(partyAnimal)) {
      throw ArgumentError.value(
        partyAnimal,
        'partyAnimal',
        'Animal is not in the player inventory.',
      );
    }

    if (!road8Room.storedAnimals.contains(storedAnimal)) {
      throw ArgumentError.value(
        storedAnimal,
        'storedAnimal',
        'Animal is not stored at Road 8.',
      );
    }

    player.removeAnimal(partyAnimal);
    road8Room.removeStoredAnimal(storedAnimal);
    player.addAnimal(storedAnimal);
    road8Room.storeCapturedAnimal(partyAnimal);
  }

  Future<void> saveGame() async {
    await saveRepository.save(createSaveState());
  }

  Future<bool> loadGame() async {
    final saveState = await saveRepository.load();

    if (saveState == null) {
      return false;
    }

    restoreSaveState(saveState);

    return true;
  }

  Future<bool> hasSave() {
    return saveRepository.exists();
  }

  Future<bool> deleteSave() {
    return saveRepository.delete();
  }

  GameSaveState createSaveState() {
    return GameSaveMapper.capture(this);
  }

  void restoreSaveState(GameSaveState saveState) {
    GameSaveMapper.validateVersion(saveState);
    GameSaveMapper.restoreRooms(saveState.rooms, _gameMap);
    GameSaveMapper.restoreGuardians(saveState.guardians, _guardians, _gameMap);
    player = GameSaveMapper.restorePlayer(saveState.player, _gameMap);
    _visitedRoomIds
      ..clear()
      ..addAll(saveState.visitedRoomIds)
      ..add(currentRoom.id);
  }

  static Player _createPlayer(String playerName, Room startingRoom) {
    final animals = GameCreatureData.createAnimals();

    return Player(name: playerName, startingRoom: startingRoom)
      ..addAnimal(animals[1].clone())
      ..addAnimal(animals[3].clone());
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

  void _resetGuardians() {
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

  static BattleMove _selectComputerMove(Animal animal) {
    return animal.moves.firstWhere(
      (move) => !BattleEngine.isHealingMove(move),
      orElse: () => animal.moves.first,
    );
  }

  static String _formatMoveResult(Animal attacker, BattleMoveResult result) {
    switch (result.type) {
      case BattleMoveResultType.damage:
        return '${attacker.name} used ${result.moveName} for ${result.amount} damage.';
      case BattleMoveResultType.healing:
        return '${attacker.name} used ${result.moveName} and restored ${result.amount} health.';
      case BattleMoveResultType.noEffect:
        return '${attacker.name} used ${result.moveName}, but it had no effect.';
    }
  }

  void _awardWildBattleProgress(Animal wildAnimal) {
    if (wildAnimal.name == 'NU_DRAGON') {
      player.addStarFragment('Nuclear Star Fragment');
    }

    _tryAddBond(wildAnimal.element, 50);
  }

  void _tryAddBond(AnimalElement element, int amount) {
    if (player.starFragments.any(
      (fragment) => fragment.startsWith(element.label),
    )) {
      player.addBond(element, amount);
    }
  }

  GuardianState _getGuardianStateForCharacter(CompPlayer character) {
    return _guardians.singleWhere(
      (guardian) => guardian.character == character,
    );
  }

  GuardianBattleResult _finishGuardianVictory(
    GuardianBattleState battleState,
    List<String> messages,
  ) {
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
}
