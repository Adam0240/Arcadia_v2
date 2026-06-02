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
      version: _readInt(json, 'version'),
      player: PlayerSaveState.fromJson(_readObject(json, 'player')),
      rooms: _readObjectList(json, 'rooms', RoomSaveState.fromJson),
      guardians: _readObjectList(json, 'guardians', GuardianSaveState.fromJson),
      visitedRoomIds: _readEnumList(json, 'visitedRoomIds', RoomId.values),
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
      name: _readString(json, 'name'),
      currentRoomId: _readEnum(json, 'currentRoomId', RoomId.values),
      starFragments: _readStringList(json, 'starFragments'),
      bond: _readObjectList(json, 'bond', BondSaveState.fromJson),
      animalInventory: _readObjectList(
        json,
        'animalInventory',
        AnimalSaveState.fromJson,
      ),
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
      roomId: _readEnum(json, 'roomId', RoomId.values),
      encounterAnimals: _readObjectList(
        json,
        'encounterAnimals',
        AnimalSaveState.fromJson,
      ),
      storedAnimals: _readObjectList(
        json,
        'storedAnimals',
        AnimalSaveState.fromJson,
      ),
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
      name: _readString(json, 'name'),
      roomId: _readEnum(json, 'roomId', RoomId.values),
      defeated: _readBool(json, 'defeated'),
      starFragments: _readStringList(json, 'starFragments'),
      battleTeamTemplate: _readObjectList(
        json,
        'battleTeamTemplate',
        AnimalSaveState.fromJson,
      ),
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
      id: _readInt(json, 'id'),
      name: _readString(json, 'name'),
      element: _readEnum(json, 'element', AnimalElement.values),
      level: _readInt(json, 'level'),
      health: _readInt(json, 'health'),
      baseHealth: _readInt(json, 'baseHealth'),
      speed: _readInt(json, 'speed'),
      moves: _readObjectList(json, 'moves', MoveSaveState.fromJson),
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
      element: _readEnum(json, 'element', AnimalElement.values),
      percent: _readInt(json, 'percent'),
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
      name: _readString(json, 'name'),
      type: _readEnum(json, 'type', ElementType.values),
      power: _readInt(json, 'power'),
      effect: _readEnum(json, 'effect', MoveEffect.values),
    );
  }
}

typedef _JsonObject = Map<String, Object?>;

Object? _readRequired(_JsonObject json, String field) {
  if (!json.containsKey(field)) {
    throw FormatException('Missing required save field "$field".');
  }

  return json[field];
}

String _readString(_JsonObject json, String field) {
  return _readStringValue(_readRequired(json, field), field);
}

String _readStringValue(Object? value, String field) {
  if (value is String) {
    return value;
  }

  throw FormatException('Expected save field "$field" to be a string.');
}

int _readInt(_JsonObject json, String field) {
  final value = _readRequired(json, field);
  if (value is int) {
    return value;
  }

  throw FormatException('Expected save field "$field" to be an integer.');
}

bool _readBool(_JsonObject json, String field) {
  final value = _readRequired(json, field);
  if (value is bool) {
    return value;
  }

  throw FormatException('Expected save field "$field" to be a boolean.');
}

_JsonObject _readObject(_JsonObject json, String field) {
  return _readObjectValue(_readRequired(json, field), field);
}

_JsonObject _readObjectValue(Object? value, String field) {
  if (value is Map) {
    final object = <String, Object?>{};

    for (final entry in value.entries) {
      final key = entry.key;
      if (key is! String) {
        throw FormatException(
          'Expected save field "$field" object keys to be strings.',
        );
      }

      object[key] = entry.value;
    }

    return object;
  }

  throw FormatException('Expected save field "$field" to be an object.');
}

List<Object?> _readList(_JsonObject json, String field) {
  final value = _readRequired(json, field);
  if (value is List) {
    return value;
  }

  throw FormatException('Expected save field "$field" to be a list.');
}

List<String> _readStringList(_JsonObject json, String field) {
  final values = _readList(json, field);
  return [
    for (var index = 0; index < values.length; index++)
      _readStringValue(values[index], '$field[$index]'),
  ];
}

List<T> _readObjectList<T>(
  _JsonObject json,
  String field,
  T Function(_JsonObject json) parse,
) {
  final values = _readList(json, field);
  return [
    for (var index = 0; index < values.length; index++)
      parse(_readObjectValue(values[index], '$field[$index]')),
  ];
}

T _readEnum<T extends Enum>(_JsonObject json, String field, List<T> values) {
  return _parseEnum(values, _readString(json, field), field);
}

List<T> _readEnumList<T extends Enum>(
  _JsonObject json,
  String field,
  List<T> values,
) {
  final names = _readStringList(json, field);
  return [
    for (var index = 0; index < names.length; index++)
      _parseEnum(values, names[index], '$field[$index]'),
  ];
}

T _parseEnum<T extends Enum>(List<T> values, String name, String field) {
  for (final value in values) {
    if (value.name == name) {
      return value;
    }
  }

  throw FormatException('Unknown save field "$field" value "$name".');
}
