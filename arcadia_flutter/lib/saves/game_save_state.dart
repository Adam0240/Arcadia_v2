import '../creatures/animal_element.dart';
import '../creatures/element_type.dart';
import '../creatures/move_effect.dart';
import '../map/room_id.dart';

class GameSaveState {
  const GameSaveState({
    required this.version,
    required this.player,
    required this.rooms,
    required this.guardians,
    required this.visitedRoomIds,
  });

  final int version;
  final PlayerSaveState player;
  final List<RoomSaveState> rooms;
  final List<GuardianSaveState> guardians;
  final List<RoomId> visitedRoomIds;

  Map<String, Object?> toJson() {
    return {
      'version': version,
      'player': player.toJson(),
      'rooms': rooms.map((room) => room.toJson()).toList(),
      'guardians': guardians.map((guardian) => guardian.toJson()).toList(),
      'visitedRoomIds': visitedRoomIds.map((roomId) => roomId.name).toList(),
    };
  }

  factory GameSaveState.fromJson(Map<String, Object?> json) {
    return GameSaveState(
      version: json['version'] as int,
      player: PlayerSaveState.fromJson(json['player'] as Map<String, Object?>),
      rooms: (json['rooms'] as List<Object?>)
          .map((room) => RoomSaveState.fromJson(room as Map<String, Object?>))
          .toList(),
      guardians: (json['guardians'] as List<Object?>).map((guardian) {
        return GuardianSaveState.fromJson(guardian as Map<String, Object?>);
      }).toList(),
      visitedRoomIds: (json['visitedRoomIds'] as List<Object?>)
          .map((roomId) => _parseEnum(RoomId.values, roomId as String))
          .toList(),
    );
  }
}

class PlayerSaveState {
  const PlayerSaveState({
    required this.name,
    required this.currentRoomId,
    required this.starFragments,
    required this.bond,
    required this.animalInventory,
  });

  final String name;
  final RoomId currentRoomId;
  final List<String> starFragments;
  final List<BondSaveState> bond;
  final List<AnimalSaveState> animalInventory;

  Map<String, Object?> toJson() {
    return {
      'name': name,
      'currentRoomId': currentRoomId.name,
      'starFragments': starFragments,
      'bond': bond.map((bondState) => bondState.toJson()).toList(),
      'animalInventory': animalInventory
          .map((animal) => animal.toJson())
          .toList(),
    };
  }

  factory PlayerSaveState.fromJson(Map<String, Object?> json) {
    return PlayerSaveState(
      name: json['name'] as String,
      currentRoomId: _parseEnum(RoomId.values, json['currentRoomId'] as String),
      starFragments: (json['starFragments'] as List<Object?>).cast<String>(),
      bond: (json['bond'] as List<Object?>).map((bondState) {
        return BondSaveState.fromJson(bondState as Map<String, Object?>);
      }).toList(),
      animalInventory: (json['animalInventory'] as List<Object?>).map((animal) {
        return AnimalSaveState.fromJson(animal as Map<String, Object?>);
      }).toList(),
    );
  }
}

class RoomSaveState {
  const RoomSaveState({
    required this.roomId,
    required this.encounterAnimals,
    required this.storedAnimals,
  });

  final RoomId roomId;
  final List<AnimalSaveState> encounterAnimals;
  final List<AnimalSaveState> storedAnimals;

  Map<String, Object?> toJson() {
    return {
      'roomId': roomId.name,
      'encounterAnimals': encounterAnimals
          .map((animal) => animal.toJson())
          .toList(),
      'storedAnimals': storedAnimals.map((animal) => animal.toJson()).toList(),
    };
  }

  factory RoomSaveState.fromJson(Map<String, Object?> json) {
    return RoomSaveState(
      roomId: _parseEnum(RoomId.values, json['roomId'] as String),
      encounterAnimals: (json['encounterAnimals'] as List<Object?>).map((
        animal,
      ) {
        return AnimalSaveState.fromJson(animal as Map<String, Object?>);
      }).toList(),
      storedAnimals: (json['storedAnimals'] as List<Object?>).map((animal) {
        return AnimalSaveState.fromJson(animal as Map<String, Object?>);
      }).toList(),
    );
  }
}

class GuardianSaveState {
  const GuardianSaveState({
    required this.name,
    required this.roomId,
    required this.defeated,
    required this.starFragments,
    required this.battleTeamTemplate,
  });

  final String name;
  final RoomId roomId;
  final bool defeated;
  final List<String> starFragments;
  final List<AnimalSaveState> battleTeamTemplate;

  Map<String, Object?> toJson() {
    return {
      'name': name,
      'roomId': roomId.name,
      'defeated': defeated,
      'starFragments': starFragments,
      'battleTeamTemplate': battleTeamTemplate
          .map((animal) => animal.toJson())
          .toList(),
    };
  }

  factory GuardianSaveState.fromJson(Map<String, Object?> json) {
    return GuardianSaveState(
      name: json['name'] as String,
      roomId: _parseEnum(RoomId.values, json['roomId'] as String),
      defeated: json['defeated'] as bool,
      starFragments: (json['starFragments'] as List<Object?>).cast<String>(),
      battleTeamTemplate: (json['battleTeamTemplate'] as List<Object?>).map((
        animal,
      ) {
        return AnimalSaveState.fromJson(animal as Map<String, Object?>);
      }).toList(),
    );
  }
}

class AnimalSaveState {
  const AnimalSaveState({
    required this.id,
    required this.name,
    required this.element,
    required this.level,
    required this.health,
    required this.baseHealth,
    required this.speed,
    required this.moves,
  });

  final int id;
  final String name;
  final AnimalElement element;
  final int level;
  final int health;
  final int baseHealth;
  final int speed;
  final List<MoveSaveState> moves;

  Map<String, Object?> toJson() {
    return {
      'id': id,
      'name': name,
      'element': element.name,
      'level': level,
      'health': health,
      'baseHealth': baseHealth,
      'speed': speed,
      'moves': moves.map((move) => move.toJson()).toList(),
    };
  }

  factory AnimalSaveState.fromJson(Map<String, Object?> json) {
    return AnimalSaveState(
      id: json['id'] as int,
      name: json['name'] as String,
      element: _parseEnum(AnimalElement.values, json['element'] as String),
      level: json['level'] as int,
      health: json['health'] as int,
      baseHealth: json['baseHealth'] as int,
      speed: json['speed'] as int,
      moves: (json['moves'] as List<Object?>)
          .map((move) => MoveSaveState.fromJson(move as Map<String, Object?>))
          .toList(),
    );
  }
}

class BondSaveState {
  const BondSaveState({required this.element, required this.percent});

  final AnimalElement element;
  final int percent;

  Map<String, Object?> toJson() {
    return {'element': element.name, 'percent': percent};
  }

  factory BondSaveState.fromJson(Map<String, Object?> json) {
    return BondSaveState(
      element: _parseEnum(AnimalElement.values, json['element'] as String),
      percent: json['percent'] as int,
    );
  }
}

class MoveSaveState {
  const MoveSaveState({
    required this.name,
    required this.type,
    required this.power,
    required this.effect,
  });

  final String name;
  final ElementType type;
  final int power;
  final MoveEffect effect;

  Map<String, Object?> toJson() {
    return {
      'name': name,
      'type': type.name,
      'power': power,
      'effect': effect.name,
    };
  }

  factory MoveSaveState.fromJson(Map<String, Object?> json) {
    return MoveSaveState(
      name: json['name'] as String,
      type: _parseEnum(ElementType.values, json['type'] as String),
      power: json['power'] as int,
      effect: _parseEnum(MoveEffect.values, json['effect'] as String),
    );
  }
}

T _parseEnum<T extends Enum>(List<T> values, String name) {
  return values.singleWhere((value) => value.name == name);
}
