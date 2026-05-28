import 'room_direction.dart';
import 'room_id.dart';

class Room {
  Room({
    required this.id,
    required this.name,
    required this.description,
    required this.imageName,
    required this.interactionText,
  });

  final RoomId id;
  final String name;
  final String description;
  final String imageName;
  final String interactionText;
  final Map<RoomDirection, Room> _exits = {};

  Map<RoomDirection, Room> get exits => Map.unmodifiable(_exits);

  void connect(RoomDirection direction, Room destination) {
    _exits[direction] = destination;
  }

  Room? getExit(RoomDirection direction) {
    return _exits[direction];
  }

  bool hasExit(RoomDirection direction) {
    return _exits.containsKey(direction);
  }
}
