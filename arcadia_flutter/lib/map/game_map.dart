import 'room.dart';
import 'room_direction.dart';
import 'room_id.dart';

class GameMap {
  GameMap() {
    _roomsById = _createRooms();
    startRoom = getRoom(RoomId.maiaStable);
    _connectRooms();
  }

  late final Map<RoomId, Room> _roomsById;
  late final Room startRoom;

  Iterable<Room> get rooms => _roomsById.values;

  Room getRoom(RoomId roomId) {
    final room = _roomsById[roomId];

    if (room == null) {
      throw ArgumentError('Unknown room id: $roomId');
    }

    return room;
  }

  static Map<RoomId, Room> _createRooms() {
    return {
      RoomId.maiaStable: Room(
        id: RoomId.maiaStable,
        name: "Maia's Stable",
        description: 'Where new trainers obtain their first creature!',
        imageName: 'maias_stable.svg',
        interactionText:
            'Maia checks the starter pens and says the stable is ready for your journey.',
      ),
      RoomId.ikena: Room(
        id: RoomId.ikena,
        name: 'Ikena',
        description: 'Small peaceful town where heroes are born.',
        imageName: 'ikena.svg',
        interactionText:
            'A town guide points out the roads leaving Ikena and reminds you to prepare before traveling.',
      ),
      RoomId.road1: Room(
        id: RoomId.road1,
        name: 'Road 1',
        description:
            'Where you make your first step into your Arcadia journey!',
        imageName: 'road1.svg',
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
      ),
      RoomId.finalTrials: _createPlaceholderRoom(
        RoomId.finalTrials,
        'Final Trials',
        'Expert trainers and future titans all travel through here.',
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
      ),
    };
  }

  static Room _createPlaceholderRoom(
    RoomId id,
    String name,
    String description,
    String interactionText,
  ) {
    return Room(
      id: id,
      name: name,
      description: description,
      imageName: 'room_placeholder.svg',
      interactionText: interactionText,
    );
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

    maiaStable.connect(RoomDirection.north, ikena);

    ikena.connect(RoomDirection.north, theEnd);
    ikena.connect(RoomDirection.east, road6);
    ikena.connect(RoomDirection.south, road5);
    ikena.connect(RoomDirection.west, road1);

    road1.connect(RoomDirection.north, road8);
    road1.connect(RoomDirection.east, ikena);
    road1.connect(RoomDirection.south, road2);
    road1.connect(RoomDirection.west, maiaStable);

    road2.connect(RoomDirection.north, road1);
    road2.connect(RoomDirection.south, oakPass);

    oakPass.connect(RoomDirection.north, road2);
    oakPass.connect(RoomDirection.south, road3);

    road3.connect(RoomDirection.north, oakPass);
    road3.connect(RoomDirection.south, road4);

    road4.connect(RoomDirection.north, road3);
    road4.connect(RoomDirection.south, newNucleon);

    newNucleon.connect(RoomDirection.north, road4);
    newNucleon.connect(RoomDirection.east, road5);

    road5.connect(RoomDirection.north, ikena);
    road5.connect(RoomDirection.east, nucleon);
    road5.connect(RoomDirection.west, newNucleon);

    road6.connect(RoomDirection.north, finalTrials);
    road6.connect(RoomDirection.south, road7);
    road6.connect(RoomDirection.west, ikena);

    road7.connect(RoomDirection.north, road6);
    road7.connect(RoomDirection.south, wyrmrest);

    wyrmrest.connect(RoomDirection.north, road7);
    wyrmrest.connect(RoomDirection.south, mountains);

    mountains.connect(RoomDirection.north, wyrmrest);
    mountains.connect(RoomDirection.south, radioactiveWay);

    radioactiveWay.connect(RoomDirection.north, mountains);
    radioactiveWay.connect(RoomDirection.south, nucleon);

    nucleon.connect(RoomDirection.north, radioactiveWay);
    nucleon.connect(RoomDirection.west, road5);

    finalTrials.connect(RoomDirection.north, guardiansTower);
    finalTrials.connect(RoomDirection.south, road6);

    guardiansTower.connect(RoomDirection.east, finalTrials);
    guardiansTower.connect(RoomDirection.south, ikena);
    guardiansTower.connect(RoomDirection.west, road8);

    road8.connect(RoomDirection.north, guardiansTower);
    road8.connect(RoomDirection.south, road1);

    theEnd.connect(RoomDirection.south, ikena);
  }
}
