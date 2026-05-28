import 'animal_element.dart';
import 'battle_move.dart';

class Animal {
  Animal({
    required this.id,
    required this.name,
    required this.element,
    required this.speed,
    required this.baseHealth,
    required int health,
    required this.level,
    required Iterable<BattleMove> moves,
  }) : assert(name.trim().isNotEmpty, 'Animal name cannot be empty.'),
       assert(speed >= 0, 'Speed cannot be negative.'),
       assert(baseHealth >= 0, 'Base health cannot be negative.'),
       assert(level >= 0, 'Level cannot be negative.'),
       _moves = List.unmodifiable(moves) {
    if (_moves.isEmpty || _moves.length > 4) {
      throw ArgumentError.value(
        moves,
        'moves',
        'Animal must have between 1 and 4 moves.',
      );
    }

    this.health = health;
  }

  final int id;
  final String name;
  final AnimalElement element;
  final int speed;
  final int baseHealth;
  final int level;
  final List<BattleMove> _moves;

  late int _health;

  int get health => _health;

  set health(int value) {
    if (value < 0 || value > baseHealth) {
      throw RangeError.range(value, 0, baseHealth, 'health');
    }

    _health = value;
  }

  List<BattleMove> get moves => _moves;

  Animal clone() {
    return Animal(
      id: id,
      name: name,
      element: element,
      speed: speed,
      baseHealth: baseHealth,
      health: health,
      level: level,
      moves: moves,
    );
  }
}
