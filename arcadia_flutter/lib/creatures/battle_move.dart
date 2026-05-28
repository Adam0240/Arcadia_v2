import 'element_type.dart';
import 'move_effect.dart';

class BattleMove {
  BattleMove({
    required this.name,
    required this.type,
    required this.power,
    this.effect = MoveEffect.damage,
  }) {
    if (name.trim().isEmpty) {
      throw ArgumentError.value(name, 'name', 'Move name cannot be empty.');
    }

    if (power < 0) {
      throw RangeError.range(power, 0, null, 'power');
    }
  }

  final String name;
  final ElementType type;
  final int power;
  final MoveEffect effect;
}
