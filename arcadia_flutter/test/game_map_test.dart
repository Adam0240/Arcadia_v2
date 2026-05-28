import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flutter map starts in the same room as the MAUI map.
  test('session starts at Maia Stable', () {
    final session = MobileGameSession(GameMap());

    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(session.currentRoom.name, "Maia's Stable");
    expect(session.visitedRoomIds, contains(RoomId.maiaStable));
  });

  // Verifies valid movement updates the current room and status message.
  test('moving north from Maia Stable enters Ikena', () {
    final session = MobileGameSession(GameMap());

    final result = session.move(RoomDirection.north);

    expect(result.moved, isTrue);
    expect(result.message, 'Moved to Ikena.');
    expect(session.currentRoom.id, RoomId.ikena);
    expect(session.visitedRoomIds, contains(RoomId.ikena));
  });

  // Verifies invalid movement leaves the player in place.
  test('moving south from Maia Stable is blocked', () {
    final session = MobileGameSession(GameMap());

    final result = session.move(RoomDirection.south);

    expect(result.moved, isFalse);
    expect(result.message, 'You cannot travel that way from here.');
    expect(session.currentRoom.id, RoomId.maiaStable);
  });

  // Verifies inspect returns the current room interaction text.
  test('interact returns current room interaction text', () {
    final session = MobileGameSession(GameMap());

    expect(
      session.interact(),
      'Maia checks the starter pens and says the stable is ready for your journey.',
    );
  });
}
