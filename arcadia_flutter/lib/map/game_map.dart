import '../creatures/animal.dart';
import '../creatures/animal_element.dart';
import '../creatures/game_creature_data.dart';
import 'movement_requirement.dart';
import 'room.dart';
import 'room_direction.dart';
import 'room_id.dart';

class GameMap {
  GameMap() {
    _roomsById = _createRooms();
    startRoom = getRoom(RoomId.maiaStable);
    _connectRooms();
    _addMovementRequirements();
    _populateWildAnimals();
  }

  late final Map<RoomId, Room> _roomsById;
  final Map<({RoomId from, RoomId to}), MovementRequirement>
  _movementRequirements = {};
  late final Room startRoom;

  Iterable<Room> get rooms => _roomsById.values;

  void resetEncounterAnimals() {
    for (final room in rooms) {
      room.restoreEncounterAnimals(const []);
    }

    _populateWildAnimals();
  }

  void resetStoredAnimals() {
    for (final room in rooms) {
      room.restoreStoredAnimals(const []);
    }
  }

  Room getRoom(RoomId roomId) {
    final room = _roomsById[roomId];

    if (room == null) {
      throw ArgumentError('Unknown room id: $roomId');
    }

    return room;
  }

  MovementRequirement getMovementRequirement(
    Room currentRoom,
    Room destination,
  ) {
    return _movementRequirements[(from: currentRoom.id, to: destination.id)] ??
        MovementRequirement.none;
  }

  static Map<RoomId, Room> _createRooms() {
    return {
      RoomId.maiaStable: Room(
        id: RoomId.maiaStable,
        name: "Maia's Stable",
        description: 'Where new guardians obtain their first creature!',
        interactionText:
            'Maia checks the starter pens and says the stable is ready for your journey.',
      ),
      RoomId.ikena: Room(
        id: RoomId.ikena,
        name: 'Ikena',
        description: 'Small peaceful town where heroes are born.',
        interactionText:
            'A town guide points out the roads leaving Ikena and reminds you to prepare before traveling.',
        isTown: true,
      ),
      RoomId.road1: Room(
        id: RoomId.road1,
        name: 'Road 1',
        description:
            'Where you make your first step into your Arcadia journey!',
        interactionText:
            'Tall grass rustles nearby, but this prototype keeps encounters disabled.',
      ),
      RoomId.road2: _createPlaceholderRoom(
        RoomId.road2,
        'Road 2',
        'A quiet path leading deeper into Arcadia.',
        'The path ahead is clear, with signs of wild creatures nearby.',
      ),
      RoomId.oakPass: _createPlaceholderRoom(
        RoomId.oakPass,
        'Oak Pass',
        'Town surrounded by trees and forest creatures.',
        'The old trees sway above the pass while travelers rest beneath them.',
        isTown: true,
      ),
      RoomId.road3: _createPlaceholderRoom(
        RoomId.road3,
        'Road 3',
        'A wooded road stretching south from Oak Pass.',
        'Branches shade the path and make the road feel calm but watchful.',
      ),
      RoomId.road4: _createPlaceholderRoom(
        RoomId.road4,
        'Road 4',
        'Tunnel',
        'The tunnel walls echo softly as you inspect the passage.',
      ),
      RoomId.newNucleon: _createPlaceholderRoom(
        RoomId.newNucleon,
        'New Nucleon',
        'Founded after Nucleon incident.',
        'Residents keep rebuilding, determined to make the town safer than before.',
        isTown: true,
      ),
      RoomId.road5: _createPlaceholderRoom(
        RoomId.road5,
        'Road 5',
        'A crossroads between Ikena, New Nucleon, and Nucleon.',
        'Tracks split across the road, showing steady travel in several directions.',
      ),
      RoomId.road6: _createPlaceholderRoom(
        RoomId.road6,
        'Road 6',
        'A road branching east from Ikena.',
        'The route opens toward stronger challenges beyond town.',
      ),
      RoomId.road7: _createPlaceholderRoom(
        RoomId.road7,
        'Road 7',
        'A route leading toward Wyrmrest.',
        'Warm winds move across the path from the dragon lands ahead.',
      ),
      RoomId.wyrmrest: _createPlaceholderRoom(
        RoomId.wyrmrest,
        'Wyrmrest',
        'Home of Dragons and Dragon Masters.',
        'Dragon banners hang over stone paths throughout Wyrmrest.',
        isTown: true,
      ),
      RoomId.mountains: _createPlaceholderRoom(
        RoomId.mountains,
        'Mountains',
        'Steep highlands south of Wyrmrest.',
        'Loose stones shift underfoot as the mountain trail climbs and bends.',
      ),
      RoomId.radioactiveWay: _createPlaceholderRoom(
        RoomId.radioactiveWay,
        'Radioactive Way',
        'A dangerous route scarred by the Nucleon incident.',
        'Warning markers line the road and faint light pulses in the distance.',
      ),
      RoomId.nucleon: _createPlaceholderRoom(
        RoomId.nucleon,
        'Nucleon',
        'The town at the center of the old incident.',
        'The streets are quiet, but the place still feels important.',
        isTown: true,
      ),
      RoomId.finalTrials: _createPlaceholderRoom(
        RoomId.finalTrials,
        'Final Trials',
        'Expert guardians and future titans all travel through here.',
        'The air feels heavier here, as if the road expects you to prove yourself.',
      ),
      RoomId.guardiansTower: _createPlaceholderRoom(
        RoomId.guardiansTower,
        'Guardian Tower',
        "Where you find out if you're the best!",
        'The tower rises above Arcadia, waiting for those ready to face its guardians.',
      ),
      RoomId.road8: _createPlaceholderRoom(
        RoomId.road8,
        'Road 8',
        'A road connecting Road 1 to Guardian Tower.',
        'The route feels like a return path and a final approach at the same time.',
      ),
      RoomId.theEnd: _createPlaceholderRoom(
        RoomId.theEnd,
        'The End',
        'Decide where you wish to stay.',
        'Everything grows still here, leaving only the weight of your final choice.',
        isFinalRoom: true,
      ),
    };
  }

  static Room _createPlaceholderRoom(
    RoomId id,
    String name,
    String description,
    String interactionText, {
    bool isTown = false,
    bool isFinalRoom = false,
  }) {
    return Room(
      id: id,
      name: name,
      description: description,
      interactionText: interactionText,
      isTown: isTown,
      isFinalRoom: isFinalRoom,
    );
  }

  static void _connectBothWays(
    Room from,
    RoomDirection direction,
    Room destination,
  ) {
    from.connect(direction, destination);
    destination.connect(direction.opposite, from);
  }

  static void _connectOneWay(
    Room from,
    RoomDirection direction,
    Room destination,
  ) {
    from.connect(direction, destination);
  }

  void _connectRooms() {
    final maiaStable = getRoom(RoomId.maiaStable);
    final ikena = getRoom(RoomId.ikena);
    final road1 = getRoom(RoomId.road1);
    final road2 = getRoom(RoomId.road2);
    final oakPass = getRoom(RoomId.oakPass);
    final road3 = getRoom(RoomId.road3);
    final road4 = getRoom(RoomId.road4);
    final newNucleon = getRoom(RoomId.newNucleon);
    final road5 = getRoom(RoomId.road5);
    final road6 = getRoom(RoomId.road6);
    final road7 = getRoom(RoomId.road7);
    final wyrmrest = getRoom(RoomId.wyrmrest);
    final mountains = getRoom(RoomId.mountains);
    final radioactiveWay = getRoom(RoomId.radioactiveWay);
    final nucleon = getRoom(RoomId.nucleon);
    final finalTrials = getRoom(RoomId.finalTrials);
    final guardiansTower = getRoom(RoomId.guardiansTower);
    final road8 = getRoom(RoomId.road8);
    final theEnd = getRoom(RoomId.theEnd);

    _connectOneWay(maiaStable, RoomDirection.north, ikena);
    _connectOneWay(road1, RoomDirection.west, maiaStable);

    _connectBothWays(ikena, RoomDirection.north, theEnd);
    _connectBothWays(ikena, RoomDirection.east, road6);
    _connectBothWays(ikena, RoomDirection.south, road5);
    _connectBothWays(ikena, RoomDirection.west, road1);

    _connectBothWays(road1, RoomDirection.north, road8);
    _connectBothWays(road1, RoomDirection.south, road2);

    _connectBothWays(road2, RoomDirection.south, oakPass);
    _connectBothWays(oakPass, RoomDirection.south, road3);
    _connectBothWays(road3, RoomDirection.south, road4);
    _connectBothWays(road4, RoomDirection.south, newNucleon);

    _connectBothWays(newNucleon, RoomDirection.east, road5);
    _connectBothWays(road5, RoomDirection.east, nucleon);

    _connectBothWays(road6, RoomDirection.north, finalTrials);
    _connectBothWays(road6, RoomDirection.south, road7);
    _connectBothWays(road7, RoomDirection.south, wyrmrest);
    _connectBothWays(wyrmrest, RoomDirection.south, mountains);
    _connectBothWays(mountains, RoomDirection.south, radioactiveWay);
    _connectBothWays(radioactiveWay, RoomDirection.south, nucleon);

    _connectOneWay(finalTrials, RoomDirection.north, guardiansTower);
    _connectOneWay(guardiansTower, RoomDirection.east, finalTrials);
    _connectOneWay(guardiansTower, RoomDirection.south, ikena);
    _connectOneWay(guardiansTower, RoomDirection.west, road8);
    _connectOneWay(road8, RoomDirection.north, guardiansTower);
  }

  void _addMovementRequirements() {
    _addMovementRequirement(
      RoomId.ikena,
      RoomId.road6,
      requiredStarFragments: 3,
    );
    _addMovementRequirement(
      RoomId.road5,
      RoomId.nucleon,
      requiredStarFragments: 4,
    );
    _addMovementRequirement(
      RoomId.ikena,
      RoomId.road5,
      requiredAnimalElement: AnimalElement.mystic,
    );
    _addMovementRequirement(
      RoomId.newNucleon,
      RoomId.road5,
      requiredAnimalElement: AnimalElement.mystic,
    );
    _addMovementRequirement(
      RoomId.road8,
      RoomId.guardiansTower,
      requiresElementalTitanDefeat: true,
    );
    _addMovementRequirement(
      RoomId.ikena,
      RoomId.theEnd,
      requiresElementalTitanDefeat: true,
    );
  }

  void _addMovementRequirement(
    RoomId from,
    RoomId to, {
    int requiredStarFragments = 0,
    AnimalElement? requiredAnimalElement,
    bool requiresElementalTitanDefeat = false,
  }) {
    _movementRequirements[(from: from, to: to)] = MovementRequirement(
      requiredStarFragments: requiredStarFragments,
      requiredAnimalElement: requiredAnimalElement,
      requiresElementalTitanDefeat: requiresElementalTitanDefeat,
    );
  }

  void _populateWildAnimals() {
    final mapAnimals = GameCreatureData.createAnimals();
    final nuclearDragon = mapAnimals.singleWhere(
      (animal) => animal.name == 'NU_DRAGON',
    );

    _addAnimalToRoom(RoomId.road1, mapAnimals[3]);
    _addAnimalToRoom(RoomId.road2, mapAnimals[15]);
    _addAnimalToRoom(RoomId.road2, mapAnimals[12]);
    _addAnimalToRoom(RoomId.road3, mapAnimals[9]);
    _addAnimalToRoom(RoomId.road3, mapAnimals[11]);
    _addAnimalToRoom(RoomId.road4, mapAnimals[10]);
    _addAnimalToRoom(RoomId.road5, mapAnimals[7]);
    _addAnimalToRoom(RoomId.road6, mapAnimals[14]);
    _addAnimalToRoom(RoomId.road7, mapAnimals[4]);
    _addAnimalToRoom(RoomId.road7, mapAnimals[13]);
    _addAnimalToRoom(RoomId.mountains, mapAnimals[8]);
    _addAnimalToRoom(RoomId.radioactiveWay, mapAnimals[6]);
    _addAnimalToRoom(RoomId.finalTrials, mapAnimals[17]);
    _addAnimalToRoom(RoomId.theEnd, nuclearDragon);
  }

  void _addAnimalToRoom(RoomId roomId, Animal animal) {
    getRoom(roomId).setRoomAnimal(animal);
  }
}
