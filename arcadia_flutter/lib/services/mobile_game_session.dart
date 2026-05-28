import '../map/game_map.dart';
import '../map/room.dart';
import '../map/room_direction.dart';
import '../map/room_id.dart';
import 'move_result.dart';

class MobileGameSession {
  MobileGameSession(this._gameMap) {
    currentRoom = _gameMap.startRoom;
    _visitedRoomIds.add(currentRoom.id);
  }

  final GameMap _gameMap;
  final Set<RoomId> _visitedRoomIds = {};

  late Room currentRoom;
  String playerName = '';

  Set<RoomId> get visitedRoomIds => Set.unmodifiable(_visitedRoomIds);

  void startNewGame(String playerName) {
    if (playerName.trim().isEmpty) {
      throw ArgumentError('Player name cannot be empty.');
    }

    this.playerName = playerName.trim();
    _visitedRoomIds.clear();
    currentRoom = _gameMap.startRoom;
    _visitedRoomIds.add(currentRoom.id);
  }

  void restore(
    String playerName,
    RoomId currentRoomId,
    Iterable<RoomId> visitedRoomIds,
  ) {
    this.playerName = playerName.trim();
    currentRoom = _gameMap.getRoom(currentRoomId);
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

    currentRoom = destination;
    _visitedRoomIds.add(currentRoom.id);

    return MoveResult(moved: true, message: 'Moved to ${currentRoom.name}.');
  }

  String interact() {
    return currentRoom.interactionText;
  }
}
