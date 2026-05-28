import '../creatures/game_creature_data.dart';
import '../map/game_map.dart';
import '../map/room.dart';
import '../map/room_direction.dart';
import '../map/room_id.dart';
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
    _visitedRoomIds.add(currentRoom.id);
  }

  final GameMap _gameMap;
  final GameSaveRepository saveRepository;
  final Set<RoomId> _visitedRoomIds = {};

  late Player player;

  Room get currentRoom => player.currentRoom;
  String get playerName => player.name;

  Iterable<Room> get rooms => _gameMap.rooms;
  Set<RoomId> get visitedRoomIds => Set.unmodifiable(_visitedRoomIds);

  void startNewGame(String playerName) {
    if (playerName.trim().isEmpty) {
      throw ArgumentError('Player name cannot be empty.');
    }

    _gameMap.resetEncounterAnimals();
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

    return '${currentRoom.interactionText}\nAnimals Nearby: $nearbyAnimals';
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
}
