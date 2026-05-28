enum RoomDirection { north, east, south, west }

extension RoomDirectionLabel on RoomDirection {
  String get label {
    switch (this) {
      case RoomDirection.north:
        return 'North';
      case RoomDirection.east:
        return 'East';
      case RoomDirection.south:
        return 'South';
      case RoomDirection.west:
        return 'West';
    }
  }
}
