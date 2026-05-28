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

extension RoomDirectionNavigation on RoomDirection {
  RoomDirection get opposite {
    switch (this) {
      case RoomDirection.north:
        return RoomDirection.south;
      case RoomDirection.east:
        return RoomDirection.west;
      case RoomDirection.south:
        return RoomDirection.north;
      case RoomDirection.west:
        return RoomDirection.east;
    }
  }
}
