import '../creatures/animal.dart';
import 'generic_player.dart';

class CompPlayer extends GenericPlayer {
  CompPlayer({required super.name, required super.startingRoom});

  final List<Animal> _battleTeamTemplate = [];

  bool defeated = false;

  List<Animal> get battleTeamTemplate => List.unmodifiable(_battleTeamTemplate);

  void setBattleTeam(Iterable<Animal> templateAnimals) {
    _battleTeamTemplate
      ..clear()
      ..addAll(templateAnimals.map((animal) => animal.clone()));

    prepareForBattle();
  }

  void prepareForBattle() {
    clearAnimalInventory();

    for (final animal in _battleTeamTemplate) {
      addAnimal(animal.clone());
    }
  }
}
