import 'element_type.dart';
import 'move_effect.dart';

class BattleMove {
  const BattleMove({
    required this.name,
    required this.type,
    required this.power,
    this.effect = MoveEffect.damage,
  }) : assert(name.length > 0, 'Move name cannot be empty.'),
       assert(power >= 0, 'Move power cannot be negative.');

  final String name;
  final ElementType type;
  final int power;
  final MoveEffect effect;
}
