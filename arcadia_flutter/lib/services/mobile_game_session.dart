import '../battles/battle_engine.dart';
import '../battles/guardian_battle_result.dart';
import '../battles/guardian_battle_state.dart';
import '../battles/battle_move_result.dart';
import '../battles/wild_battle_result.dart';
import '../battles/wild_battle_state.dart';
import '../creatures/animal.dart';
import '../creatures/animal_element.dart';
import '../creatures/animal_growth_catalog.dart';
import '../creatures/battle_move.dart';
import '../creatures/game_creature_data.dart';
import '../guardians/guardian_state.dart';
import '../map/game_map.dart';
import '../map/room.dart';
import '../map/room_direction.dart';
import '../map/room_id.dart';
import '../player/player.dart';
import '../saves/game_save_mapper.dart';
import '../saves/game_save_repository.dart';
import '../saves/game_save_state.dart';
import '../saves/local_json_game_save_repository.dart';
import 'game_session_persistence.dart';
import 'guardian_progression.dart';
import 'move_result.dart';

class MobileGameSession {
  MobileGameSession(
    this._gameMap, {
    this.saveRepository = const LocalJsonGameSaveRepository(),
  }) : _persistence = GameSessionPersistence(saveRepository) {
    player = _createPlayer('Player', _gameMap.startRoom);
    _guardianProgression = GuardianProgression(_gameMap);
    _visitedRoomIds.add(currentRoom.id);
  }

  final GameMap _gameMap;
  final GameSaveRepository saveRepository;
  final GameSessionPersistence _persistence;
  final Set<RoomId> _visitedRoomIds = {};
  late final GuardianProgression _guardianProgression;

  late Player player;

  Room get currentRoom => player.currentRoom;
  String get playerName => player.name;

  Iterable<Room> get rooms => _gameMap.rooms;
  Iterable<GuardianState> get guardians => _guardianProgression.guardians;
  Set<RoomId> get visitedRoomIds => Set.unmodifiable(_visitedRoomIds);
  bool get hasWildEncounter => currentRoom.hasEncounterAnimals();
  bool get hasGuardianInCurrentRoom => currentGuardian != null;
  GuardianState? get currentGuardian {
    return _guardianProgression.guardianInRoom(currentRoom);
  }

  GuardianState get elementalTitan {
    return _guardianProgression.elementalTitan;
  }

  List<Animal> get currentEncounterAnimals => currentRoom.encounterAnimals;
  Room get road8Room => _gameMap.getRoom(RoomId.road8);
  List<Animal> get road8StoredAnimals => road8Room.storedAnimals;
  bool get canSwapStoredAnimals {
    return currentRoom.id == RoomId.road8 &&
        road8Room.hasStoredAnimals() &&
        player.animalInventory.isNotEmpty;
  }

  List<AnimalGrowthOption> get growthOptions {
    return AnimalGrowthCatalog.getGrowthOptions(player);
  }

  bool get hasGrowthOptions {
    return AnimalGrowthCatalog.hasGrowthOptions(player);
  }

  void startNewGame(String playerName) {
    if (playerName.trim().isEmpty) {
      throw ArgumentError('Player name cannot be empty.');
    }

    _gameMap.resetEncounterAnimals();
    _gameMap.resetStoredAnimals();
    _guardianProgression.reset();
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

    final blockedMessage = _getMovementBlockedMessage(destination);
    if (blockedMessage != null) {
      return MoveResult(moved: false, message: blockedMessage);
    }

    player.moveTo(destination);
    _visitedRoomIds.add(currentRoom.id);

    final elementalStarMessage = _tryAwardElementalStar();
    final message = elementalStarMessage == null
        ? 'Moved to ${currentRoom.name}.'
        : 'Moved to ${currentRoom.name}.\n$elementalStarMessage';

    return MoveResult(moved: true, message: message);
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

  String? getFinalRoomMessage() {
    if (!currentRoom.isFinalRoom) {
      return null;
    }

    if (currentRoom.hasEncounterAnimals()) {
      return 'Cosmic Voice: I knew you would eventually find your way here.\n'
          'Your potential was clear to me the first time you were in my presence.\n'
          'You have proven you are the best trainer in Arcadia. But are you stronger than the god of this region?\n'
          'Face me to find out if you truly are the best.';
    }

    return 'You have defeated all the strongest trainers in this region.';
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

      if (!BattleEngine.hasUsableAnimals(player)) {
        battleState.isComplete = true;
        messages.add('$defeatedAnimalName defeated.');
        messages.add('Battle lost, all animals in your party are defeated.');

        return WildBattleResult(
          message: messages.join('\n'),
          battleEnded: true,
        );
      }

      messages.add('$defeatedAnimalName defeated.');
      messages.add('Choose another animal.');

      return WildBattleResult(
        message: messages.join('\n'),
        needsPlayerSwitch: true,
      );
    }

    return WildBattleResult(message: messages.join('\n'));
  }

  WildBattleResult switchWildBattleAnimal(
    WildBattleState battleState,
    int animalIndex,
  ) {
    battleState.switchPlayerAnimal(animalIndex);

    return WildBattleResult(
      message: '${battleState.playerAnimal.name} steps in.',
    );
  }

  WildBattleResult catchWildAnimal(WildBattleState battleState) {
    if (!battleState.isWildDefeated) {
      return WildBattleResult(
        message: 'Defeat ${battleState.wildAnimal.name} before catching it.',
      );
    }

    if (player.animalInventory.length >= BattleEngine.maxPartySize) {
      battleState.wildAnimal.health = battleState.wildAnimal.baseHealth;
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

  String healParty() {
    if (!currentRoom.isTown) {
      return "Can only heal if you're in a town!";
    }

    for (final partyAnimal in player.animalInventory) {
      partyAnimal.health = partyAnimal.baseHealth;
    }

    return 'All your animals have been fully restored!';
  }

  String growAnimal(AnimalGrowthOption growthOption) {
    final partyIndex = player.animalInventory.indexOf(
      growthOption.currentAnimal,
    );

    if (partyIndex < 0) {
      throw ArgumentError.value(
        growthOption,
        'growthOption',
        'Animal is not in the player inventory.',
      );
    }

    if (player.getBond(growthOption.currentAnimal.element) < 100) {
      throw StateError('This animal is not ready to grow up.');
    }

    player.replaceAnimalAt(partyIndex, growthOption.adultAnimal);
    player.resetBond(growthOption.currentAnimal.element);

    return '${growthOption.currentAnimal.name} grew into ${growthOption.adultAnimal.name}!';
  }

  String? getGuardianUnavailableMessage() {
    return _guardianProgression.getUnavailableMessage(
      guardian: currentGuardian,
      player: player,
    );
  }

  GuardianBattleState startGuardianBattle() {
    return _guardianProgression.startBattle(
      player: player,
      guardian: currentGuardian,
    );
  }

  String getGuardianIntro(GuardianBattleState battleState) {
    return _guardianProgression.getIntro(battleState, player);
  }

  String getCurrentChallengeActionLabel() {
    return _guardianProgression.getCurrentChallengeActionLabel(currentGuardian);
  }

  String getGuardianBattleTitle(GuardianBattleState battleState) {
    return _guardianProgression.getBattleTitle(battleState);
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

      if (!BattleEngine.hasUsableAnimals(player)) {
        battleState.isComplete = true;
        messages.add('$defeatedAnimalName defeated.');
        messages.add('Battle lost, all animals in your party are defeated.');

        return GuardianBattleResult(
          message: messages.join('\n'),
          battleEnded: true,
          returnToMap: true,
        );
      }

      messages.add('$defeatedAnimalName defeated.');
      messages.add('Choose another animal.');

      return GuardianBattleResult(
        message: messages.join('\n'),
        needsPlayerSwitch: true,
      );
    }

    return GuardianBattleResult(message: messages.join('\n'));
  }

  GuardianBattleResult switchGuardianBattleAnimal(
    GuardianBattleState battleState,
    int animalIndex,
  ) {
    battleState.switchPlayerAnimal(animalIndex);

    return GuardianBattleResult(
      message: '${battleState.playerAnimal.name} steps in.',
    );
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
    await _persistence.save(createSaveState());
  }

  Future<bool> loadGame() async {
    final saveState = await _persistence.load();

    if (saveState == null) {
      return false;
    }

    restoreSaveState(saveState);

    return true;
  }

  Future<bool> hasSave() {
    return _persistence.exists();
  }

  Future<bool> deleteSave() {
    return _persistence.delete();
  }

  GameSaveState createSaveState() {
    return GameSaveMapper.capture(this);
  }

  void restoreSaveState(GameSaveState saveState) {
    GameSaveMapper.validateVersion(saveState);
    GameSaveMapper.restoreRooms(saveState.rooms, _gameMap);
    GameSaveMapper.restoreGuardians(saveState.guardians, guardians, _gameMap);
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

  String? _getMovementBlockedMessage(Room destination) {
    final requirement = _gameMap.getMovementRequirement(
      currentRoom,
      destination,
    );

    if (requirement.requiredStarFragments > 0 &&
        player.starFragments.length < requirement.requiredStarFragments) {
      return 'You need to obtain ${requirement.requiredStarFragments} star fragment(s) before this way unlocks!\n'
          'You currently have ${player.starFragments.length} total star fragments.';
    }

    final requiredElement = requirement.requiredAnimalElement;
    if (requiredElement != null &&
        !player.animalInventory.any(
          (animal) => animal.element == requiredElement,
        )) {
      return 'You need a ${requiredElement.label} animal on your team before this way unlocks!';
    }

    if (requirement.requiresElementalTitanDefeat && !elementalTitan.defeated) {
      return 'You are not ready to go here yet. You must defeat the Elemental Titan to proceed.';
    }

    return null;
  }

  String? _tryAwardElementalStar() {
    if (currentRoom.id != RoomId.maiaStable ||
        !elementalTitan.defeated ||
        !player.starFragments.contains('Cosmic Star Fragment') ||
        player.starFragments.contains('Elemental Star')) {
      return null;
    }

    player.addStarFragment('Elemental Star');
    return 'Returning to the town where your journey began, you check your bag. The star fragments have merged into an Elemental Star.';
  }

  GuardianBattleResult _finishGuardianVictory(
    GuardianBattleState battleState,
    List<String> messages,
  ) {
    return _guardianProgression.finishVictory(
      battleState: battleState,
      player: player,
      messages: messages,
    );
  }
}
