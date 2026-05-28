import 'package:arcadia_flutter/creatures/animal_catalog.dart';
import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/player/comp_player.dart';
import 'package:arcadia_flutter/player/player.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies player construction requires a valid name and stores the starting room.
  test('player validates name and starts in provided room', () {
    final gameMap = GameMap();
    final startingRoom = gameMap.startRoom;
    final player = Player(name: 'Ari', startingRoom: startingRoom);

    expect(player.name, 'Ari');
    expect(player.currentRoom, same(startingRoom));
    expect(
      () => Player(name: ' ', startingRoom: startingRoom),
      throwsArgumentError,
    );
  });

  // Verifies star fragments are unique and reject blank names.
  test('star fragments are unique and displayable', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);

    expect(player.getStarFragmentDisplay(), 'You have no star fragments!');
    expect(() => player.addStarFragment(' '), throwsArgumentError);

    player.addStarFragment('Nature Star Fragment');
    player.addStarFragment('Nature Star Fragment');
    player.addStarFragment('Mystic Star Fragment');

    expect(player.starFragments, [
      'Nature Star Fragment',
      'Mystic Star Fragment',
    ]);
    expect(
      player.getStarFragmentDisplay(),
      'Star Fragments:\nNature Star Fragment\nMystic Star Fragment',
    );
  });

  // Verifies bond values start at zero, cap at 100, and reject negative additions.
  test('bond values add, cap, reset, and display by element', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);

    expect(player.bondByElement.length, AnimalElement.values.length);
    for (final element in AnimalElement.values) {
      expect(player.getBond(element), 0);
    }

    player.addBond(AnimalElement.nature, 40);
    player.addBond(AnimalElement.nature, 80);

    expect(player.getBond(AnimalElement.nature), 100);
    expect(() => player.addBond(AnimalElement.nature, -1), throwsRangeError);

    player.resetBond(AnimalElement.nature);

    expect(player.getBond(AnimalElement.nature), 0);
    expect(
      player.getBondDisplay(),
      'Bond:\nNature 0%/100%\nMystic 0%/100%\nThunder 0%/100%\nDraconic 0%/100%\nCosmic 0%/100%\nNuclear 0%/100%',
    );
  });

  // Verifies restoreBond clamps values and defaults missing elements to zero.
  test('restoreBond clamps saved values and defaults missing elements', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);

    player.restoreBond({
      AnimalElement.nature: -10,
      AnimalElement.mystic: 35,
      AnimalElement.thunder: 120,
    });

    expect(player.getBond(AnimalElement.nature), 0);
    expect(player.getBond(AnimalElement.mystic), 35);
    expect(player.getBond(AnimalElement.thunder), 100);
    expect(player.getBond(AnimalElement.draconic), 0);
    expect(player.getBond(AnimalElement.cosmic), 0);
    expect(player.getBond(AnimalElement.nuclear), 0);
  });

  // Verifies animal inventory operations preserve ownership inside the player model.
  test('animal inventory add remove get swap replace and display work', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);
    final animals = AnimalCatalog.createAnimals();
    final natureCat = animals.singleWhere((animal) => animal.name == 'N_CAT');
    final natureDog = animals.singleWhere((animal) => animal.name == 'N_DOG');
    final natureHorse = animals.singleWhere(
      (animal) => animal.name == 'N_HORSE',
    );

    expect(player.getAnimalInventoryDisplay(), "Inventory is Empty! :'( ");

    player.addAnimal(natureCat);
    player.addAnimal(natureDog);

    expect(player.animalInventory, [natureCat, natureDog]);
    expect(player.getAnimalAt(1), same(natureDog));
    expect(
      player.getAnimalInventoryDisplay(),
      'Inventory List:\nN_CAT Health: 75\nN_DOG Health: 40',
    );

    player.swapAnimalPositions(0, 1);

    expect(player.animalInventory, [natureDog, natureCat]);

    player.replaceAnimalAt(1, natureHorse);

    expect(player.animalInventory, [natureDog, natureHorse]);
    expect(player.removeAnimal(natureDog), isTrue);
    expect(player.removeAnimal(natureCat), isFalse);
    expect(player.animalInventory, [natureHorse]);
  });

  // Verifies restore helpers replace existing player state.
  test('restore helpers replace name star fragments and inventory', () {
    final player = Player(name: 'Ari', startingRoom: GameMap().startRoom);
    final animals = AnimalCatalog.createAnimals();
    final natureCat = animals.singleWhere((animal) => animal.name == 'N_CAT');
    final natureDog = animals.singleWhere((animal) => animal.name == 'N_DOG');

    player.addStarFragment('Old Fragment');
    player.addAnimal(natureCat);

    player.restoreName('Nova');
    player.restoreStarFragments([
      'Mystic Star Fragment',
      'Mystic Star Fragment',
    ]);
    player.restoreAnimalInventory([natureDog]);

    expect(player.name, 'Nova');
    expect(player.starFragments, ['Mystic Star Fragment']);
    expect(player.animalInventory, [natureDog]);
    expect(() => player.restoreName(' '), throwsArgumentError);
  });

  // Verifies moveTo changes the player's current room.
  test('moveTo updates current room', () {
    final gameMap = GameMap();
    final player = Player(name: 'Ari', startingRoom: gameMap.startRoom);
    final ikena = gameMap.getRoom(RoomId.ikena);

    player.moveTo(ikena);

    expect(player.currentRoom, same(ikena));
  });

  // Verifies computer players clone battle templates and rebuild active teams.
  test('comp player clones battle team templates and prepares fresh teams', () {
    final guardian = CompPlayer(
      name: 'Guardian',
      startingRoom: GameMap().startRoom,
    );
    final animals = AnimalCatalog.createAnimals();
    final natureDog = animals.singleWhere((animal) => animal.name == 'N_DOG');
    final mysticLion = animals.singleWhere((animal) => animal.name == 'M_LION');

    guardian.setBattleTeam([natureDog, mysticLion]);

    expect(guardian.battleTeamTemplate, hasLength(2));
    expect(guardian.animalInventory, hasLength(2));
    expect(guardian.battleTeamTemplate.first, isNot(same(natureDog)));
    expect(
      guardian.animalInventory.first,
      isNot(same(guardian.battleTeamTemplate.first)),
    );
    expect(guardian.battleTeamTemplate.first.name, natureDog.name);
    expect(guardian.animalInventory.first.name, natureDog.name);

    guardian.animalInventory.first.health = 1;
    guardian.prepareForBattle();

    expect(guardian.animalInventory.first.health, natureDog.baseHealth);
    expect(guardian.battleTeamTemplate.first.health, natureDog.baseHealth);
  });

  // Verifies computer player defeated state can be toggled.
  test('comp player defeated state can be toggled', () {
    final guardian = CompPlayer(
      name: 'Guardian',
      startingRoom: GameMap().startRoom,
    );

    expect(guardian.defeated, isFalse);

    guardian.defeated = true;

    expect(guardian.defeated, isTrue);
  });
}
