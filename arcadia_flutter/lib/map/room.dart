import '../creatures/animal.dart';
import 'room_direction.dart';
import 'room_id.dart';

class Room {
  Room({
    required this.id,
    required this.name,
    required this.description,
    required this.interactionText,
  });

  final RoomId id;
  final String name;
  final String description;
  final String interactionText;
  final Map<RoomDirection, Room> _exits = {};
  final List<Animal> _encounterAnimals = [];
  final List<Animal> _storedAnimals = [];

  Map<RoomDirection, Room> get exits => Map.unmodifiable(_exits);
  List<Animal> get encounterAnimals => List.unmodifiable(_encounterAnimals);
  List<Animal> get storedAnimals => List.unmodifiable(_storedAnimals);

  void connect(RoomDirection direction, Room destination) {
    _exits[direction] = destination;
  }

  Room? getExit(RoomDirection direction) {
    return _exits[direction];
  }

  bool hasExit(RoomDirection direction) {
    return _exits.containsKey(direction);
  }

  void setRoomAnimal(Animal animal) {
    _encounterAnimals.add(animal.clone());
  }

  void addEncounterAnimal(Animal animal) {
    _encounterAnimals.add(animal);
  }

  bool removeEncounterAnimal(Animal animal) {
    return _encounterAnimals.remove(animal);
  }

  void restoreEncounterAnimals(Iterable<Animal> encounterAnimals) {
    _encounterAnimals
      ..clear()
      ..addAll(encounterAnimals);
  }

  bool hasEncounterAnimals() {
    return _encounterAnimals.isNotEmpty;
  }

  void storeCapturedAnimal(Animal animal) {
    _storedAnimals.add(animal);
  }

  bool removeStoredAnimal(Animal animal) {
    return _storedAnimals.remove(animal);
  }

  void restoreStoredAnimals(Iterable<Animal> storedAnimals) {
    _storedAnimals
      ..clear()
      ..addAll(storedAnimals);
  }

  bool hasStoredAnimals() {
    return _storedAnimals.isNotEmpty;
  }
}
