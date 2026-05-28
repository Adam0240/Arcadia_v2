import '../player/comp_player.dart';
import 'guardian_definition.dart';

class GuardianState {
  const GuardianState({required this.definition, required this.character});

  final GuardianDefinition definition;
  final CompPlayer character;

  bool get defeated => character.defeated;
}
