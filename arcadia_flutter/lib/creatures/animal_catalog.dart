import 'animal.dart';
import 'animal_element.dart';
import 'battle_move.dart';
import 'move_catalog.dart';

class AnimalCatalog {
  const AnimalCatalog._();

  static const int _defaultSpeed = 7;
  static const int _defaultLevel = 0;

  static final List<_SpeciesTemplate> _species = [
    _SpeciesTemplate(
      name: 'CAT',
      baseHealth: 75,
      baseMove1: MoveCatalog.pounce,
      baseMove2: MoveCatalog.felineReflex,
      usesAdvancedElementMoves: false,
      natureSpeedOverride: 9,
    ),
    _SpeciesTemplate(
      name: 'LION',
      baseHealth: 75,
      baseMove1: MoveCatalog.pounce,
      baseMove2: MoveCatalog.felineReflex,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'DOG',
      baseHealth: 45,
      baseMove1: MoveCatalog.loyalRush,
      baseMove2: MoveCatalog.wildChase,
      usesAdvancedElementMoves: false,
      natureBaseHealthOverride: 40,
    ),
    _SpeciesTemplate(
      name: 'WOLF',
      baseHealth: 80,
      baseMove1: MoveCatalog.loyalRush,
      baseMove2: MoveCatalog.wildChase,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'HORSE',
      baseHealth: 80,
      baseMove1: MoveCatalog.hoofKick,
      baseMove2: MoveCatalog.stampede,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'STALLION',
      baseHealth: 40,
      baseMove1: MoveCatalog.hoofKick,
      baseMove2: MoveCatalog.stampede,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'TURTLE',
      baseHealth: 80,
      baseMove1: MoveCatalog.headBash,
      baseMove2: MoveCatalog.deepRetreat,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'TORTOISE',
      baseHealth: 40,
      baseMove1: MoveCatalog.headBash,
      baseMove2: MoveCatalog.deepRetreat,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'BIRD',
      baseHealth: 80,
      baseMove1: MoveCatalog.beakStrike,
      baseMove2: MoveCatalog.quickTalon,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'EAGLE',
      baseHealth: 40,
      baseMove1: MoveCatalog.beakStrike,
      baseMove2: MoveCatalog.quickTalon,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'ANT',
      baseHealth: 65,
      baseMove1: MoveCatalog.mandibleBite,
      baseMove2: MoveCatalog.colonyRush,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'BEE',
      baseHealth: 80,
      baseMove1: MoveCatalog.mandibleBite,
      baseMove2: MoveCatalog.colonyRush,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'CUB',
      baseHealth: 70,
      baseMove1: MoveCatalog.playSwipe,
      baseMove2: MoveCatalog.tumbleRush,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'BEAR',
      baseHealth: 75,
      baseMove1: MoveCatalog.playSwipe,
      baseMove2: MoveCatalog.tumbleRush,
      usesAdvancedElementMoves: true,
    ),
    _SpeciesTemplate(
      name: 'SERPENT',
      baseHealth: 75,
      baseMove1: MoveCatalog.venomFang,
      baseMove2: MoveCatalog.shadowFang,
      usesAdvancedElementMoves: false,
    ),
    _SpeciesTemplate(
      name: 'DRAGON',
      baseHealth: 45,
      baseMove1: MoveCatalog.venomFang,
      baseMove2: MoveCatalog.shadowFang,
      usesAdvancedElementMoves: true,
    ),
  ];

  static final List<_ElementTemplate> _elements = [
    _ElementTemplate(
      prefix: 'N',
      element: AnimalElement.nature,
      basicMove1: MoveCatalog.thornWrap,
      basicMove2: MoveCatalog.verdantSurge,
      advancedMove1: MoveCatalog.bloom,
      advancedMove2: MoveCatalog.naturesWrath,
    ),
    _ElementTemplate(
      prefix: 'M',
      element: AnimalElement.mystic,
      basicMove1: MoveCatalog.currentRush,
      basicMove2: MoveCatalog.oceanPulse,
      advancedMove1: MoveCatalog.deepseaRupture,
      advancedMove2: MoveCatalog.tidalBreak,
    ),
    _ElementTemplate(
      prefix: 'T',
      element: AnimalElement.thunder,
      basicMove1: MoveCatalog.staticClaw,
      basicMove2: MoveCatalog.voltJab,
      advancedMove1: MoveCatalog.arcPulse,
      advancedMove2: MoveCatalog.thunderRift,
    ),
    _ElementTemplate(
      prefix: 'D',
      element: AnimalElement.draconic,
      basicMove1: MoveCatalog.emberBite,
      basicMove2: MoveCatalog.infernoRoar,
      advancedMove1: MoveCatalog.ragePulse,
      advancedMove2: MoveCatalog.dragonFall,
    ),
    _ElementTemplate(
      prefix: 'C',
      element: AnimalElement.cosmic,
      basicMove1: MoveCatalog.starFlick,
      basicMove2: MoveCatalog.lunarPulse,
      advancedMove1: MoveCatalog.cometStrike,
      advancedMove2: MoveCatalog.supernova,
    ),
    _ElementTemplate(
      prefix: 'NU',
      element: AnimalElement.nuclear,
      basicMove1: MoveCatalog.radBurst,
      basicMove2: MoveCatalog.falloutBite,
      advancedMove1: MoveCatalog.contaminate,
      advancedMove2: MoveCatalog.coreDetonation,
    ),
  ];

  static const Set<String> _adultSpecies = {
    'LION',
    'WOLF',
    'STALLION',
    'TORTOISE',
    'EAGLE',
    'BEE',
    'BEAR',
    'DRAGON',
  };

  static List<Animal> createAnimals() {
    final animals = [
      _createAnimalEntry(
        id: 0,
        name: 'NULL0',
        element: AnimalElement.nature,
        speed: 0,
        baseHealth: 0,
        level: 0,
        moves: [
          MoveCatalog.pounce,
          MoveCatalog.felineReflex,
          MoveCatalog.thornWrap,
          MoveCatalog.verdantSurge,
        ],
      ),
    ];

    var nextId = 1;

    for (final element in _elements) {
      for (final species in _species) {
        animals.add(_createAnimalFromTemplates(nextId, element, species));
        nextId += 1;
      }
    }

    return animals;
  }

  static List<Animal> createAdultAnimals() {
    return createAnimals()
        .where((animal) => _adultSpecies.contains(_getSpeciesName(animal.name)))
        .toList();
  }

  static List<Animal> createNatureAdultAnimals() {
    return createAdultAnimals()
        .where((animal) => animal.element == AnimalElement.nature)
        .toList();
  }

  static String _getSpeciesName(String animalName) {
    final nameParts = animalName.split('_');

    return nameParts.length == 2 ? nameParts[1] : animalName;
  }

  static Animal _createAnimalFromTemplates(
    int id,
    _ElementTemplate element,
    _SpeciesTemplate species,
  ) {
    final elementMoves = species.usesAdvancedElementMoves
        ? [element.advancedMove1, element.advancedMove2]
        : [element.basicMove1, element.basicMove2];
    final baseHealth = species.getBaseHealth(element.element);

    return _createAnimalEntry(
      id: id,
      name: '${element.prefix}_${species.name}',
      element: element.element,
      speed: species.getSpeed(element.element),
      baseHealth: baseHealth,
      level: _defaultLevel,
      moves: [
        species.baseMove1,
        species.baseMove2,
        elementMoves[0],
        elementMoves[1],
      ],
    );
  }

  static Animal _createAnimalEntry({
    required int id,
    required String name,
    required AnimalElement element,
    required int speed,
    required int baseHealth,
    required int level,
    required Iterable<BattleMove> moves,
  }) {
    return Animal(
      id: id,
      name: name,
      element: element,
      speed: speed,
      baseHealth: baseHealth,
      health: baseHealth,
      level: level,
      moves: moves,
    );
  }
}

class _SpeciesTemplate {
  const _SpeciesTemplate({
    required this.name,
    required this.baseHealth,
    required this.baseMove1,
    required this.baseMove2,
    required this.usesAdvancedElementMoves,
    this.natureSpeedOverride,
    this.natureBaseHealthOverride,
  });

  final String name;
  final int baseHealth;
  final BattleMove baseMove1;
  final BattleMove baseMove2;
  final bool usesAdvancedElementMoves;
  final int? natureSpeedOverride;
  final int? natureBaseHealthOverride;

  int getSpeed(AnimalElement element) {
    return element == AnimalElement.nature && natureSpeedOverride != null
        ? natureSpeedOverride!
        : AnimalCatalog._defaultSpeed;
  }

  int getBaseHealth(AnimalElement element) {
    return element == AnimalElement.nature && natureBaseHealthOverride != null
        ? natureBaseHealthOverride!
        : baseHealth;
  }
}

class _ElementTemplate {
  const _ElementTemplate({
    required this.prefix,
    required this.element,
    required this.basicMove1,
    required this.basicMove2,
    required this.advancedMove1,
    required this.advancedMove2,
  });

  final String prefix;
  final AnimalElement element;
  final BattleMove basicMove1;
  final BattleMove basicMove2;
  final BattleMove advancedMove1;
  final BattleMove advancedMove2;
}
