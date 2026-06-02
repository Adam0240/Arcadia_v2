import 'package:arcadia_flutter/creatures/game_creature_data.dart';
import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flutter map starts in the expected opening room.
  test('session starts at Maia Stable', () {
    final session = MobileGameSession(GameMap());

    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(session.currentRoom.name, "Maia's Stable");
    expect(session.player.name, 'Player');
    expect(session.player.currentRoom, same(session.currentRoom));
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_CAT',
      'N_DOG',
    ]);
    expect(session.visitedRoomIds, contains(RoomId.maiaStable));
  });

  // Verifies starting a new game creates a named player with starter animals.
  test('startNewGame creates named player with starter animals', () {
    final session = MobileGameSession(GameMap());

    session.startNewGame(' Nova ');

    expect(session.playerName, 'Nova');
    expect(session.player.name, 'Nova');
    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(session.player.currentRoom, same(session.currentRoom));
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_CAT',
      'N_DOG',
    ]);
    expect(session.visitedRoomIds, {RoomId.maiaStable});
  });

  // Verifies starting a new game restores canonical room encounter placement.
  test('startNewGame resets room encounter animals', () {
    final session = MobileGameSession(GameMap());

    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    session.currentRoom.removeEncounterAnimal(
      session.currentRoom.encounterAnimals.single,
    );
    expect(session.currentRoom.encounterAnimals, isEmpty);

    session.startNewGame('Nova');

    expect(
      session.rooms
          .singleWhere((room) => room.id == RoomId.road1)
          .encounterAnimals
          .map((animal) => animal.name),
      ['N_DOG'],
    );
  });

  // Verifies valid movement updates the current room and status message.
  test('moving north from Maia Stable enters Ikena', () {
    final session = MobileGameSession(GameMap());

    final result = session.move(RoomDirection.north);

    expect(result.moved, isTrue);
    expect(result.message, 'Moved to Ikena.');
    expect(session.currentRoom.id, RoomId.ikena);
    expect(session.player.currentRoom.id, RoomId.ikena);
    expect(session.visitedRoomIds, contains(RoomId.ikena));
  });

  // Verifies invalid movement leaves the player in place.
  test('moving south from Maia Stable is blocked', () {
    final session = MobileGameSession(GameMap());

    final result = session.move(RoomDirection.south);

    expect(result.moved, isFalse);
    expect(result.message, 'You cannot travel that way from here.');
    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(session.player.currentRoom.id, RoomId.maiaStable);
  });

  // Verifies restore recreates player state at the saved room and visit set.
  test('restore recreates player at saved room', () {
    final session = MobileGameSession(GameMap());

    session.restore('Saved Player', RoomId.road5, [RoomId.maiaStable]);

    expect(session.playerName, 'Saved Player');
    expect(session.currentRoom.id, RoomId.road5);
    expect(session.player.currentRoom, same(session.currentRoom));
    expect(session.visitedRoomIds, {RoomId.maiaStable, RoomId.road5});
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_CAT',
      'N_DOG',
    ]);
  });

  // Verifies inspect returns the current room interaction text.
  test('interact returns current room interaction text', () {
    final session = MobileGameSession(GameMap());

    expect(
      session.interact(),
      'Maia checks the starter pens and says the stable is ready for your journey.\nAnimals Nearby: None',
    );
  });

  // Verifies inspect includes nearby animal names for rooms with encounters.
  test('interact returns nearby animals for current room', () {
    final session = MobileGameSession(GameMap());

    session.move(RoomDirection.north);
    session.move(RoomDirection.west);

    expect(
      session.interact(),
      'Tall grass rustles nearby, but this prototype keeps encounters disabled.\nAnimals Nearby: N_DOG',
    );
  });

  // Verifies every declared room id has a concrete room in the map.
  test('every room id resolves to a map room', () {
    final map = GameMap();

    for (final roomId in RoomId.values) {
      expect(map.getRoom(roomId).id, roomId);
    }

    expect(map.rooms.map((room) => room.id), unorderedEquals(RoomId.values));
  });

  // Verifies Flutter town metadata matches the console map's heal-eligible rooms.
  test('map marks console town rooms as towns', () {
    final map = GameMap();
    final townRoomIds = map.rooms
        .where((room) => room.isTown)
        .map((room) => room.id);

    expect(
      townRoomIds,
      unorderedEquals([
        RoomId.ikena,
        RoomId.oakPass,
        RoomId.newNucleon,
        RoomId.wyrmrest,
        RoomId.nucleon,
      ]),
    );
  });

  // Verifies the session fully heals party animals only while standing in a town.
  test('healParty restores animals in town', () {
    final session = MobileGameSession(GameMap());
    session.move(RoomDirection.north);

    for (final animal in session.player.animalInventory) {
      animal.health = 1;
    }

    final message = session.healParty();

    expect(message, 'All your animals have been fully restored!');
    for (final animal in session.player.animalInventory) {
      expect(animal.health, animal.baseHealth);
    }
  });

  // Verifies the session rejects healing outside towns and leaves health unchanged.
  test('healParty does not restore animals outside town', () {
    final session = MobileGameSession(GameMap());

    for (final animal in session.player.animalInventory) {
      animal.health = 1;
    }

    final message = session.healParty();

    expect(message, "Can only heal if you're in a town!");
    for (final animal in session.player.animalInventory) {
      expect(animal.health, 1);
    }
  });

  // Verifies star fragment route gates block movement until requirements are met.
  test('move enforces star fragment route gates', () {
    final session = MobileGameSession(GameMap())..move(RoomDirection.north);

    final blocked = session.move(RoomDirection.east);

    expect(blocked.moved, isFalse);
    expect(blocked.message, contains('You need to obtain 3 star fragment(s)'));
    expect(session.currentRoom.id, RoomId.ikena);

    session.player
      ..addStarFragment('Nature Star Fragment')
      ..addStarFragment('Mystic Star Fragment')
      ..addStarFragment('Thunder Star Fragment');

    final moved = session.move(RoomDirection.east);

    expect(moved.moved, isTrue);
    expect(session.currentRoom.id, RoomId.road6);
  });

  // Verifies animal-element route gates require a matching party animal.
  test('move enforces required animal element gates', () {
    final session = MobileGameSession(GameMap())..move(RoomDirection.north);

    final blocked = session.move(RoomDirection.south);

    expect(blocked.moved, isFalse);
    expect(blocked.message, contains('You need a Mystic animal'));
    expect(session.currentRoom.id, RoomId.ikena);

    final mysticLion = GameCreatureData.createAnimals().singleWhere(
      (animal) => animal.name == 'M_LION',
    );
    session.player.addAnimal(mysticLion.clone());

    final moved = session.move(RoomDirection.south);

    expect(moved.moved, isTrue);
    expect(session.currentRoom.id, RoomId.road5);
  });

  // Verifies Elemental Titan gates block late-game exits until the Titan is defeated.
  test('move enforces elemental titan defeat gates', () {
    final session = MobileGameSession(GameMap())
      ..move(RoomDirection.north)
      ..move(RoomDirection.west)
      ..move(RoomDirection.north);

    final blocked = session.move(RoomDirection.north);

    expect(blocked.moved, isFalse);
    expect(blocked.message, contains('defeat the Elemental Titan'));
    expect(session.currentRoom.id, RoomId.road8);

    session.elementalTitan.character.defeated = true;

    final moved = session.move(RoomDirection.north);

    expect(moved.moved, isTrue);
    expect(session.currentRoom.id, RoomId.guardiansTower);
  });

  // Verifies returning to Maia's Stable after Titan victory awards the Elemental Star.
  test(
    'returning to Maia Stable after titan victory awards elemental star',
    () {
      final session = MobileGameSession(GameMap())
        ..move(RoomDirection.north)
        ..move(RoomDirection.west);
      session.elementalTitan.character.defeated = true;
      session.player.addStarFragment('Cosmic Star Fragment');

      final result = session.move(RoomDirection.west);

      expect(result.moved, isTrue);
      expect(session.currentRoom.id, RoomId.maiaStable);
      expect(session.player.starFragments, contains('Elemental Star'));
      expect(result.message, contains('Elemental Star'));
    },
  );

  // Verifies session growth replaces the selected party animal and resets bond.
  test('growAnimal replaces party animal and resets element bond', () {
    final session = MobileGameSession(GameMap());
    session.player.addBond(AnimalElement.nature, 100);

    final options = session.growthOptions;
    final result = session.growAnimal(
      options.singleWhere((option) => option.currentAnimal.name == 'N_CAT'),
    );

    expect(result, 'N_CAT grew into N_LION!');
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_LION',
      'N_DOG',
    ]);
    expect(session.player.getBond(AnimalElement.nature), 0);
    expect(session.hasGrowthOptions, isFalse);
  });

  // Verifies session growth rejects stale options for animals no longer owned.
  test('growAnimal rejects options for animals outside inventory', () {
    final session = MobileGameSession(GameMap());
    session.player.addBond(AnimalElement.nature, 100);
    final option = session.growthOptions.first;

    session.player.removeAnimal(option.currentAnimal);

    expect(() => session.growAnimal(option), throwsArgumentError);
  });

  // Verifies the Elemental Titan is available at Guardian Tower with its reward.
  test('elemental titan challenge is available at guardian tower', () {
    final session = MobileGameSession(GameMap());
    session.restore('Nova', RoomId.guardiansTower, [RoomId.maiaStable]);

    expect(session.getCurrentChallengeActionLabel(), 'Elemental Titan');
    expect(
      session.getGuardianUnavailableMessage(),
      contains('defeated all 4 sanctuaries'),
    );

    session.player
      ..addStarFragment('Nature Star Fragment')
      ..addStarFragment('Mystic Star Fragment')
      ..addStarFragment('Thunder Star Fragment')
      ..addStarFragment('Draconic Star Fragment');

    final battleState = session.startGuardianBattle();

    expect(battleState.guardian.name, 'Elemental Titan');
    expect(
      battleState.guardian.battleTeamTemplate.map((animal) => animal.name),
      ['N_WOLF', 'N_TORTOISE', 'N_STALLION', 'M_LION'],
    );
  });

  // Verifies final room messaging changes after the final encounter is cleared.
  test('final room message changes after final encounter is cleared', () {
    final session = MobileGameSession(GameMap());
    session.restore('Nova', RoomId.theEnd, [RoomId.maiaStable]);

    expect(session.getFinalRoomMessage(), contains('Cosmic Voice'));

    session.currentRoom.removeEncounterAnimal(
      session.currentRoom.encounterAnimals.single,
    );

    expect(
      session.getFinalRoomMessage(),
      'You have defeated all the strongest trainers in this region.',
    );
  });

  // Verifies every connected destination is also registered in the map.
  test('all connected destinations are registered map rooms', () {
    final map = GameMap();

    for (final room in map.rooms) {
      for (final destination in room.exits.values) {
        expect(map.getRoom(destination.id), same(destination));
      }
    }
  });

  // Verifies every room can be reached from the start room through exits.
  test('all rooms are reachable from the start room', () {
    final map = GameMap();
    final visitedRoomIds = <RoomId>{};
    final pendingRooms = [map.startRoom];

    while (pendingRooms.isNotEmpty) {
      final room = pendingRooms.removeLast();

      if (!visitedRoomIds.add(room.id)) {
        continue;
      }

      pendingRooms.addAll(room.exits.values);
    }

    expect(visitedRoomIds, unorderedEquals(RoomId.values));
  });

  // Verifies the complete expected map route table.
  test('map exposes the expected route table', () {
    final map = GameMap();
    final expectedRoutes = <RoomId, Map<RoomDirection, RoomId>>{
      RoomId.maiaStable: {RoomDirection.north: RoomId.ikena},
      RoomId.ikena: {
        RoomDirection.north: RoomId.theEnd,
        RoomDirection.east: RoomId.road6,
        RoomDirection.south: RoomId.road5,
        RoomDirection.west: RoomId.road1,
      },
      RoomId.road1: {
        RoomDirection.north: RoomId.road8,
        RoomDirection.east: RoomId.ikena,
        RoomDirection.south: RoomId.road2,
        RoomDirection.west: RoomId.maiaStable,
      },
      RoomId.road2: {
        RoomDirection.north: RoomId.road1,
        RoomDirection.south: RoomId.oakPass,
      },
      RoomId.oakPass: {
        RoomDirection.north: RoomId.road2,
        RoomDirection.south: RoomId.road3,
      },
      RoomId.road3: {
        RoomDirection.north: RoomId.oakPass,
        RoomDirection.south: RoomId.road4,
      },
      RoomId.road4: {
        RoomDirection.north: RoomId.road3,
        RoomDirection.south: RoomId.newNucleon,
      },
      RoomId.newNucleon: {
        RoomDirection.north: RoomId.road4,
        RoomDirection.east: RoomId.road5,
      },
      RoomId.road5: {
        RoomDirection.north: RoomId.ikena,
        RoomDirection.east: RoomId.nucleon,
        RoomDirection.west: RoomId.newNucleon,
      },
      RoomId.road6: {
        RoomDirection.north: RoomId.finalTrials,
        RoomDirection.south: RoomId.road7,
        RoomDirection.west: RoomId.ikena,
      },
      RoomId.road7: {
        RoomDirection.north: RoomId.road6,
        RoomDirection.south: RoomId.wyrmrest,
      },
      RoomId.wyrmrest: {
        RoomDirection.north: RoomId.road7,
        RoomDirection.south: RoomId.mountains,
      },
      RoomId.mountains: {
        RoomDirection.north: RoomId.wyrmrest,
        RoomDirection.south: RoomId.radioactiveWay,
      },
      RoomId.radioactiveWay: {
        RoomDirection.north: RoomId.mountains,
        RoomDirection.south: RoomId.nucleon,
      },
      RoomId.nucleon: {
        RoomDirection.north: RoomId.radioactiveWay,
        RoomDirection.west: RoomId.road5,
      },
      RoomId.finalTrials: {
        RoomDirection.north: RoomId.guardiansTower,
        RoomDirection.south: RoomId.road6,
      },
      RoomId.guardiansTower: {
        RoomDirection.east: RoomId.finalTrials,
        RoomDirection.south: RoomId.ikena,
        RoomDirection.west: RoomId.road8,
      },
      RoomId.road8: {
        RoomDirection.north: RoomId.guardiansTower,
        RoomDirection.south: RoomId.road1,
      },
      RoomId.theEnd: {RoomDirection.south: RoomId.ikena},
    };

    for (final roomId in RoomId.values) {
      final room = map.getRoom(roomId);
      final expectedExits = expectedRoutes[roomId]!;
      final actualExits = room.exits.map(
        (direction, destination) => MapEntry(direction, destination.id),
      );

      expect(
        actualExits,
        expectedExits,
        reason: 'Unexpected exits for $roomId',
      );
    }
  });

  // Verifies wild animals are assigned to rooms from the console reference map.
  test('map populates expected wild animals by room', () {
    final map = GameMap();
    final expectedAnimalsByRoom = <RoomId, List<String>>{
      RoomId.maiaStable: [],
      RoomId.ikena: [],
      RoomId.road1: ['N_DOG'],
      RoomId.road2: ['N_SERPENT', 'N_BEE'],
      RoomId.oakPass: [],
      RoomId.road3: ['N_BIRD', 'N_ANT'],
      RoomId.road4: ['N_EAGLE'],
      RoomId.newNucleon: [],
      RoomId.road5: ['N_TURTLE'],
      RoomId.road6: ['N_BEAR'],
      RoomId.road7: ['N_WOLF', 'N_CUB'],
      RoomId.wyrmrest: [],
      RoomId.mountains: ['N_TORTOISE'],
      RoomId.radioactiveWay: ['N_STALLION'],
      RoomId.nucleon: [],
      RoomId.finalTrials: ['M_CAT'],
      RoomId.guardiansTower: [],
      RoomId.road8: [],
      RoomId.theEnd: ['NU_DRAGON'],
    };

    for (final roomId in RoomId.values) {
      final room = map.getRoom(roomId);

      expect(
        room.encounterAnimals.map((animal) => animal.name),
        expectedAnimalsByRoom[roomId],
        reason: 'Unexpected encounter animals for $roomId',
      );
    }
  });

  // Verifies room encounter state uses cloned animals instead of catalog instances.
  test('room wild animals are cloned into room encounter state', () {
    final map = GameMap();
    final road1Animal = map.getRoom(RoomId.road1).encounterAnimals.single;

    road1Animal.health = 1;

    final freshMap = GameMap();
    final freshRoad1Animal = freshMap
        .getRoom(RoomId.road1)
        .encounterAnimals
        .single;

    expect(road1Animal.name, freshRoad1Animal.name);
    expect(freshRoad1Animal.health, freshRoad1Animal.baseHealth);
  });

  // Verifies captured animals sent to Road 8 after a full party are restored first.
  test('full-party catch stores restored animal at road 8', () {
    final session = MobileGameSession(GameMap());
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    _fillParty(session);

    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;

    final result = session.catchWildAnimal(battleState);
    final storedAnimal = session.road8StoredAnimals.single;

    expect(result.message, 'You caught N_DOG! It was sent to Road 8.');
    expect(storedAnimal.name, 'N_DOG');
    expect(storedAnimal.health, storedAnimal.baseHealth);
  });

  // Verifies normal map routes are reciprocal through opposite directions.
  test('normal map routes have reciprocal exits', () {
    final map = GameMap();
    final reciprocalRoutes =
        <({RoomId from, RoomDirection direction, RoomId to})>[
          (
            from: RoomId.ikena,
            direction: RoomDirection.north,
            to: RoomId.theEnd,
          ),
          (from: RoomId.ikena, direction: RoomDirection.east, to: RoomId.road6),
          (
            from: RoomId.ikena,
            direction: RoomDirection.south,
            to: RoomId.road5,
          ),
          (from: RoomId.ikena, direction: RoomDirection.west, to: RoomId.road1),
          (
            from: RoomId.road1,
            direction: RoomDirection.north,
            to: RoomId.road8,
          ),
          (
            from: RoomId.road1,
            direction: RoomDirection.south,
            to: RoomId.road2,
          ),
          (
            from: RoomId.road2,
            direction: RoomDirection.south,
            to: RoomId.oakPass,
          ),
          (
            from: RoomId.oakPass,
            direction: RoomDirection.south,
            to: RoomId.road3,
          ),
          (
            from: RoomId.road3,
            direction: RoomDirection.south,
            to: RoomId.road4,
          ),
          (
            from: RoomId.road4,
            direction: RoomDirection.south,
            to: RoomId.newNucleon,
          ),
          (
            from: RoomId.newNucleon,
            direction: RoomDirection.east,
            to: RoomId.road5,
          ),
          (
            from: RoomId.road5,
            direction: RoomDirection.east,
            to: RoomId.nucleon,
          ),
          (
            from: RoomId.road6,
            direction: RoomDirection.north,
            to: RoomId.finalTrials,
          ),
          (
            from: RoomId.road6,
            direction: RoomDirection.south,
            to: RoomId.road7,
          ),
          (
            from: RoomId.road7,
            direction: RoomDirection.south,
            to: RoomId.wyrmrest,
          ),
          (
            from: RoomId.wyrmrest,
            direction: RoomDirection.south,
            to: RoomId.mountains,
          ),
          (
            from: RoomId.mountains,
            direction: RoomDirection.south,
            to: RoomId.radioactiveWay,
          ),
          (
            from: RoomId.radioactiveWay,
            direction: RoomDirection.south,
            to: RoomId.nucleon,
          ),
        ];

    for (final route in reciprocalRoutes) {
      final from = map.getRoom(route.from);
      final to = map.getRoom(route.to);

      expect(from.getExit(route.direction), same(to));
      expect(to.getExit(route.direction.opposite), same(from));
    }
  });

  // Verifies special non-grid routes are documented as one-way exits.
  test('special map routes are explicit one-way exits', () {
    final map = GameMap();
    final oneWayRoutes = <({RoomId from, RoomDirection direction, RoomId to})>[
      (
        from: RoomId.maiaStable,
        direction: RoomDirection.north,
        to: RoomId.ikena,
      ),
      (
        from: RoomId.road1,
        direction: RoomDirection.west,
        to: RoomId.maiaStable,
      ),
      (
        from: RoomId.finalTrials,
        direction: RoomDirection.north,
        to: RoomId.guardiansTower,
      ),
      (
        from: RoomId.guardiansTower,
        direction: RoomDirection.east,
        to: RoomId.finalTrials,
      ),
      (
        from: RoomId.guardiansTower,
        direction: RoomDirection.south,
        to: RoomId.ikena,
      ),
      (
        from: RoomId.guardiansTower,
        direction: RoomDirection.west,
        to: RoomId.road8,
      ),
      (
        from: RoomId.road8,
        direction: RoomDirection.north,
        to: RoomId.guardiansTower,
      ),
    ];

    for (final route in oneWayRoutes) {
      final from = map.getRoom(route.from);
      final to = map.getRoom(route.to);

      expect(from.getExit(route.direction), same(to));
      expect(to.getExit(route.direction.opposite), isNot(same(from)));
    }
  });
}

void _fillParty(MobileGameSession session) {
  final animals = GameCreatureData.createAnimals();

  for (final animal in animals.skip(4).take(4)) {
    session.player.addAnimal(animal.clone());
  }
}
