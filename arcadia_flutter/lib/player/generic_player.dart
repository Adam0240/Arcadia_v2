import '../creatures/animal.dart';
import '../creatures/animal_element.dart';
import '../map/room.dart';

abstract class GenericPlayer {
  GenericPlayer({required String name, required Room startingRoom})
    : _name = _validateName(name),
      _currentRoom = startingRoom,
      _bondByElement = _createEmptyBondMap();

  final List<String> _starFragments = [];
  final List<Animal> _animalInventory = [];
  final Map<AnimalElement, int> _bondByElement;

  String _name;
  Room _currentRoom;

  String get name => _name;
  Room get currentRoom => _currentRoom;
  List<String> get starFragments => List.unmodifiable(_starFragments);
  List<Animal> get animalInventory => List.unmodifiable(_animalInventory);
  Map<AnimalElement, int> get bondByElement => Map.unmodifiable(_bondByElement);

  void addStarFragment(String starFragment) {
    final normalizedStarFragment = _validateStarFragment(starFragment);

    if (!_starFragments.contains(normalizedStarFragment)) {
      _starFragments.add(normalizedStarFragment);
    }
  }

  String getStarFragmentDisplay() {
    if (_starFragments.isEmpty) {
      return 'You have no star fragments!';
    }

    return 'Star Fragments:\n${_starFragments.join('\n')}';
  }

  void addBond(AnimalElement element, int amount) {
    if (amount < 0) {
      throw RangeError.value(
        amount,
        'amount',
        'Bond amount cannot be negative.',
      );
    }

    _bondByElement[element] = (_bondByElement[element]! + amount).clamp(0, 100);
  }

  int getBond(AnimalElement element) {
    return _bondByElement[element]!;
  }

  void resetBond(AnimalElement element) {
    _bondByElement[element] = 0;
  }

  String getBondDisplay() {
    final bondLines = ['Bond:'];

    for (final element in AnimalElement.values) {
      bondLines.add('${element.label} ${_bondByElement[element]}%/100%');
    }

    return bondLines.join('\n');
  }

  String getAnimalInventoryDisplay() {
    if (_animalInventory.isEmpty) {
      return "Inventory is Empty! :'( ";
    }

    final inventoryLines = ['Inventory List:'];

    for (final animal in _animalInventory) {
      inventoryLines.add('${animal.name} Health: ${animal.health}');
    }

    return inventoryLines.join('\n');
  }

  void addAnimal(Animal animal) {
    _animalInventory.add(animal);
  }

  bool removeAnimal(Animal animal) {
    return _animalInventory.remove(animal);
  }

  Animal getAnimalAt(int index) {
    return _animalInventory[index];
  }

  void swapAnimalPositions(int firstIndex, int secondIndex) {
    final temp = _animalInventory[firstIndex];
    _animalInventory[firstIndex] = _animalInventory[secondIndex];
    _animalInventory[secondIndex] = temp;
  }

  void replaceAnimalAt(int index, Animal replacement) {
    _animalInventory[index] = replacement;
  }

  void restoreStarFragments(Iterable<String> starFragments) {
    _starFragments.clear();

    for (final starFragment in starFragments) {
      addStarFragment(starFragment);
    }
  }

  void restoreName(String name) {
    _name = _validateName(name);
  }

  void restoreAnimalInventory(Iterable<Animal> animalInventory) {
    _animalInventory
      ..clear()
      ..addAll(animalInventory);
  }

  void restoreBond(Map<AnimalElement, int> bondByElement) {
    for (final element in AnimalElement.values) {
      final bond = bondByElement[element] ?? 0;
      _bondByElement[element] = bond.clamp(0, 100);
    }
  }

  void moveTo(Room room) {
    _currentRoom = room;
  }

  void clearAnimalInventory() {
    _animalInventory.clear();
  }

  static Map<AnimalElement, int> _createEmptyBondMap() {
    return {for (final element in AnimalElement.values) element: 0};
  }

  static String _validateName(String name) {
    if (name.trim().isEmpty) {
      throw ArgumentError.value(name, 'name', 'Player name cannot be empty.');
    }

    return name;
  }

  static String _validateStarFragment(String starFragment) {
    if (starFragment.trim().isEmpty) {
      throw ArgumentError.value(
        starFragment,
        'starFragment',
        'Star fragment name cannot be empty.',
      );
    }

    return starFragment;
  }
}
