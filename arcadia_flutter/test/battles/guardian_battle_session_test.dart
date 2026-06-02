import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies guardians are created at their sanctuary rooms with their reference teams.
  test('guardians are initialized with locations teams and rewards', () {
    final session = MobileGameSession(GameMap());

    final natureGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Nature Guardian',
    );
    final mysticGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Mystic Guardian',
    );
    final thunderGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Thunder Guardian',
    );

    expect(natureGuardian.character.currentRoom.id, RoomId.oakPass);
    expect(mysticGuardian.character.currentRoom.id, RoomId.ikena);
    expect(thunderGuardian.character.currentRoom.id, RoomId.newNucleon);
    expect(natureGuardian.character.starFragments, ['Nature Star Fragment']);
    expect(
      natureGuardian.character.battleTeamTemplate.map((animal) => animal.name),
      ['N_DOG', 'N_BEAR'],
    );
  });

  // Verifies Ikena hosts the Mystic Guardian and enforces requirements.
  test('ikena mystic guardian reports star fragment requirements', () {
    final session = MobileGameSession(GameMap())..move(RoomDirection.north);

    expect(session.currentRoom.id, RoomId.ikena);
    expect(session.hasGuardianInCurrentRoom, isTrue);
    expect(session.currentGuardian?.character.name, 'Mystic Guardian');
    expect(
      session.getGuardianUnavailableMessage(),
      'You need to have 2 star fragments to battle this guardian!',
    );
    expect(session.startGuardianBattle, throwsStateError);
  });

  // Verifies New Nucleon hosts the Thunder Guardian and enforces requirements.
  test('new nucleon thunder guardian reports star fragment requirements', () {
    final session = MobileGameSession(GameMap());
    session.restore('Nova', RoomId.newNucleon, [RoomId.maiaStable]);

    expect(session.currentRoom.id, RoomId.newNucleon);
    expect(session.hasGuardianInCurrentRoom, isTrue);
    expect(session.currentGuardian?.character.name, 'Thunder Guardian');
    expect(
      session.getGuardianUnavailableMessage(),
      'You need to have 1 star fragment to battle this guardian!',
    );
    expect(session.startGuardianBattle, throwsStateError);
  });

  // Verifies guardian battles send out the next healthy guardian animal.
  test('guardian battle switches to next guardian animal', () {
    final session = _createOakPassSession();
    final battleState = session.startGuardianBattle();
    battleState.guardianAnimal.health = 1;

    final result = session.useGuardianBattleMove(
      battleState,
      battleState.playerAnimal.moves.first,
    );

    expect(result.returnToMap, isFalse);
    expect(result.message, contains('N_DOG defeated.'));
    expect(result.message, contains('Nature Guardian sent out N_BEAR.'));
    expect(battleState.guardianAnimal.name, 'N_BEAR');
  });

  // Verifies defeating a guardian awards the fragment and full matching bond.
  test('defeating guardian awards star fragment and full bond', () {
    final session = _createOakPassSession();
    final battleState = session.startGuardianBattle();
    battleState.guardianAnimal.health = 1;

    session.useGuardianBattleMove(
      battleState,
      battleState.playerAnimal.moves.first,
    );
    battleState.guardianAnimal.health = 1;

    final result = session.useGuardianBattleMove(
      battleState,
      battleState.playerAnimal.moves.first,
    );

    final natureGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Nature Guardian',
    );

    expect(result.returnToMap, isTrue);
    expect(result.message, contains('Nature Guardian defeated.'));
    expect(natureGuardian.defeated, isTrue);
    expect(session.player.starFragments, ['Nature Star Fragment']);
    expect(session.player.getBond(AnimalElement.nature), 100);
  });

  // Verifies each guardian battle attempt starts from a fresh guardian team.
  test('starting guardian battle prepares a fresh guardian team', () {
    final session = _createOakPassSession();
    final firstBattleState = session.startGuardianBattle();
    firstBattleState.guardianAnimal.health = 1;

    final secondBattleState = session.startGuardianBattle();

    expect(
      secondBattleState.guardianAnimal.health,
      secondBattleState.guardianAnimal.baseHealth,
    );
  });

  // Verifies starting a new game resets guardian defeated state.
  test('startNewGame resets guardian state', () {
    final session = _createOakPassSession();
    final natureGuardian = session.currentGuardian!;
    natureGuardian.character.defeated = true;

    session.startNewGame('Nova');

    final resetNatureGuardian = session.guardians.singleWhere(
      (guardian) => guardian.character.name == 'Nature Guardian',
    );
    expect(session.currentRoom.id, RoomId.maiaStable);
    expect(resetNatureGuardian.defeated, isFalse);
    expect(resetNatureGuardian.character.currentRoom.id, RoomId.oakPass);
  });
}

MobileGameSession _createOakPassSession() {
  return MobileGameSession(GameMap())
    ..move(RoomDirection.north)
    ..move(RoomDirection.west)
    ..move(RoomDirection.south)
    ..move(RoomDirection.south);
}
