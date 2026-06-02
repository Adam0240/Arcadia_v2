import 'package:arcadia_flutter/creatures/animal.dart';
import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/creatures/animal_growth_catalog.dart';
import 'package:arcadia_flutter/creatures/battle_move.dart';
import 'package:arcadia_flutter/creatures/element_type.dart';
import 'package:arcadia_flutter/creatures/game_creature_data.dart';
import 'package:arcadia_flutter/creatures/move_catalog.dart';
import 'package:arcadia_flutter/creatures/move_effect.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/player/player.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies creature elements preserve the reference ordering.
  test('animal elements are ordered like the reference data', () {
    expect(AnimalElement.values, [
      AnimalElement.nature,
      AnimalElement.mystic,
      AnimalElement.thunder,
      AnimalElement.draconic,
      AnimalElement.cosmic,
      AnimalElement.nuclear,
    ]);
  });

  // Verifies move catalog entries preserve important reference values.
  test('move catalog contains expected values', () {
    expect(MoveCatalog.pounce.name, 'Pounce');
    expect(MoveCatalog.pounce.type, ElementType.base);
    expect(MoveCatalog.pounce.power, 5);
    expect(MoveCatalog.pounce.effect, MoveEffect.damage);

    expect(MoveCatalog.bloom.name, 'Bloom');
    expect(MoveCatalog.bloom.type, ElementType.nature);
    expect(MoveCatalog.bloom.power, 10);
    expect(MoveCatalog.bloom.effect, MoveEffect.heal);

    expect(MoveCatalog.coreDetonation.name, 'Core Detonation');
    expect(MoveCatalog.coreDetonation.type, ElementType.nuclear);
    expect(MoveCatalog.coreDetonation.power, 10);
  });

  // Verifies the generated roster size, ids, and first placeholder entry.
  test('createAnimals returns full stable roster with sequential ids', () {
    final animals = GameCreatureData.createAnimals();

    expect(animals, hasLength(97));
    expect(animals.map((animal) => animal.id), List.generate(97, (id) => id));
    expect(animals.first.name, 'NULL0');
    expect(animals.first.element, AnimalElement.nature);
    expect(animals.first.speed, 0);
    expect(animals.first.baseHealth, 0);
    expect(animals.first.health, 0);
    expect(animals.first.moves.map((move) => move.name), [
      'Pounce',
      'Feline Reflex',
      'Thorn Wrap',
      'Verdant Surge',
    ]);
  });

  // Verifies generated animal stats and moves match representative reference entries.
  test('generated animals match reference template values', () {
    final animals = GameCreatureData.createAnimals();

    final natureCat = animals.singleWhere((animal) => animal.name == 'N_CAT');
    expect(natureCat.id, 1);
    expect(natureCat.element, AnimalElement.nature);
    expect(natureCat.speed, 9);
    expect(natureCat.baseHealth, 75);
    expect(natureCat.health, 75);
    expect(natureCat.level, 0);
    expect(natureCat.moves.map((move) => move.name), [
      'Pounce',
      'Feline Reflex',
      'Thorn Wrap',
      'Verdant Surge',
    ]);

    final natureDog = animals.singleWhere((animal) => animal.name == 'N_DOG');
    expect(natureDog.id, 3);
    expect(natureDog.speed, 7);
    expect(natureDog.baseHealth, 40);
    expect(natureDog.moves.map((move) => move.name), [
      'Loyal Rush',
      'Wild Chase',
      'Thorn Wrap',
      'Verdant Surge',
    ]);

    final mysticLion = animals.singleWhere((animal) => animal.name == 'M_LION');
    expect(mysticLion.id, 18);
    expect(mysticLion.element, AnimalElement.mystic);
    expect(mysticLion.speed, 7);
    expect(mysticLion.baseHealth, 75);
    expect(mysticLion.moves.map((move) => move.name), [
      'Pounce',
      'Feline Reflex',
      'Deepsea Rupture',
      'Tidal Break',
    ]);

    final nuclearDragon = animals.singleWhere(
      (animal) => animal.name == 'NU_DRAGON',
    );
    expect(nuclearDragon.id, 96);
    expect(nuclearDragon.element, AnimalElement.nuclear);
    expect(nuclearDragon.speed, 7);
    expect(nuclearDragon.baseHealth, 45);
    expect(nuclearDragon.moves.map((move) => move.name), [
      'Venom Fang',
      'Shadow Fang',
      'Contaminate',
      'Core Detonation',
    ]);
  });

  // Verifies adult roster helpers preserve reference filtering rules.
  test('adult roster helpers return expected animals', () {
    final adultAnimals = GameCreatureData.createAdultAnimals();
    final natureAdultAnimals = GameCreatureData.createNatureAdultAnimals();

    expect(adultAnimals, hasLength(48));
    expect(natureAdultAnimals, hasLength(8));
    expect(
      natureAdultAnimals.every(
        (animal) => animal.element == AnimalElement.nature,
      ),
      isTrue,
    );
    expect(natureAdultAnimals.map((animal) => animal.name), [
      'N_LION',
      'N_WOLF',
      'N_STALLION',
      'N_TORTOISE',
      'N_EAGLE',
      'N_BEE',
      'N_BEAR',
      'N_DRAGON',
    ]);
  });

  // Verifies growth options map base species to the matching adult species.
  test('growth catalog maps base forms to adult forms', () {
    final animals = GameCreatureData.createAnimals();
    final expectedAdultByBase = {
      'N_CAT': 'N_LION',
      'N_DOG': 'N_WOLF',
      'N_HORSE': 'N_STALLION',
      'N_TURTLE': 'N_TORTOISE',
      'N_BIRD': 'N_EAGLE',
      'N_ANT': 'N_BEE',
      'N_CUB': 'N_BEAR',
      'N_SERPENT': 'N_DRAGON',
    };

    for (final entry in expectedAdultByBase.entries) {
      final adultAnimal = AnimalGrowthCatalog.tryCreateAdultForm(
        animals.singleWhere((animal) => animal.name == entry.key),
      );

      expect(adultAnimal?.name, entry.value);
    }
  });

  // Verifies growth options require full matching element bond.
  test('growth options require full element bond', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);
    final natureCat = GameCreatureData.createAnimals().singleWhere(
      (animal) => animal.name == 'N_CAT',
    );
    player.addAnimal(natureCat);

    expect(AnimalGrowthCatalog.getGrowthOptions(player), isEmpty);

    player.addBond(AnimalElement.nature, 100);

    final options = AnimalGrowthCatalog.getGrowthOptions(player);
    expect(options, hasLength(1));
    expect(options.single.currentAnimal.name, 'N_CAT');
    expect(options.single.adultAnimal.name, 'N_LION');
  });

  // Verifies adult animals are not eligible to grow again.
  test('growth catalog does not return options for adult forms', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);
    final natureLion = GameCreatureData.createAnimals().singleWhere(
      (animal) => animal.name == 'N_LION',
    );
    player
      ..addAnimal(natureLion)
      ..addBond(AnimalElement.nature, 100);

    expect(AnimalGrowthCatalog.getGrowthOptions(player), isEmpty);
  });

  // Verifies cloned animals preserve values while allowing separate health changes.
  test('animal clone returns a separate animal instance', () {
    final original = GameCreatureData.createAnimals().singleWhere(
      (animal) => animal.name == 'N_CAT',
    );
    final clone = original.clone();

    expect(clone, isNot(same(original)));
    expect(clone.id, original.id);
    expect(clone.name, original.name);
    expect(clone.element, original.element);
    expect(clone.speed, original.speed);
    expect(clone.baseHealth, original.baseHealth);
    expect(clone.health, original.health);
    expect(clone.level, original.level);
    expect(clone.moves, original.moves);

    clone.health = 10;

    expect(clone.health, 10);
    expect(original.health, 75);
  });

  // Verifies creature constructors enforce invalid data in release builds too.
  test('animal and move constructors validate runtime data', () {
    expect(
      () => BattleMove(name: ' ', type: ElementType.base, power: 1),
      throwsArgumentError,
    );
    expect(
      () => BattleMove(name: 'Scratch', type: ElementType.base, power: -1),
      throwsRangeError,
    );
    expect(
      () => Animal(
        id: 1,
        name: ' ',
        element: AnimalElement.nature,
        speed: 1,
        baseHealth: 10,
        health: 10,
        level: 1,
        moves: [MoveCatalog.pounce],
      ),
      throwsArgumentError,
    );
    expect(
      () => Animal(
        id: 1,
        name: 'TEST',
        element: AnimalElement.nature,
        speed: -1,
        baseHealth: 10,
        health: 10,
        level: 1,
        moves: [MoveCatalog.pounce],
      ),
      throwsRangeError,
    );
  });
}
