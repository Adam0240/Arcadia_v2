import '../creatures/animal.dart';
import '../creatures/battle_move.dart';
import '../guardians/guardian_state.dart';
import '../map/game_map.dart';
import '../map/room.dart';
import '../player/player.dart';
import '../services/mobile_game_session.dart';
import 'game_save_state.dart';

class GameSaveMapper {
  const GameSaveMapper._();

  static const int currentVersion = 4;

  static GameSaveState capture(MobileGameSession session) {
    return GameSaveState(
      version: currentVersion,
      player: _capturePlayer(session.player),
      rooms: session.rooms.map(_captureRoom).toList(),
      guardians: session.guardians.map(_captureGuardian).toList(),
      visitedRoomIds: session.visitedRoomIds.toList(),
    );
  }

  static void validateVersion(GameSaveState saveState) {
    if (saveState.version != currentVersion) {
      throw FormatException(
        'Unsupported save version ${saveState.version}. Expected $currentVersion.',
      );
    }
  }

  static Player restorePlayer(PlayerSaveState saveState, GameMap gameMap) {
    final player = Player(
      name: saveState.name,
      startingRoom: gameMap.getRoom(saveState.currentRoomId),
    );

    player.restoreStarFragments(saveState.starFragments);
    player.restoreBond({
      for (final bond in saveState.bond) bond.element: bond.percent,
    });
    player.restoreAnimalInventory(
      saveState.animalInventory.map(_restoreAnimal),
    );

    return player;
  }

  static void restoreRooms(List<RoomSaveState> roomStates, GameMap gameMap) {
    for (final roomState in roomStates) {
      gameMap.getRoom(roomState.roomId)
        ..restoreEncounterAnimals(
          roomState.encounterAnimals.map(_restoreAnimal),
        )
        ..restoreStoredAnimals(roomState.storedAnimals.map(_restoreAnimal));
    }
  }

  static void restoreGuardians(
    List<GuardianSaveState> guardianStates,
    Iterable<GuardianState> guardians,
    GameMap gameMap,
  ) {
    for (final guardianState in guardianStates) {
      final guardian = guardians.singleWhere(
        (guardian) => guardian.character.name == guardianState.name,
      );

      guardian.character
        ..restoreName(guardianState.name)
        ..moveTo(gameMap.getRoom(guardianState.roomId))
        ..defeated = guardianState.defeated
        ..restoreStarFragments(guardianState.starFragments)
        ..setBattleTeam(guardianState.battleTeamTemplate.map(_restoreAnimal));
    }
  }

  static PlayerSaveState _capturePlayer(Player player) {
    return PlayerSaveState(
      name: player.name,
      currentRoomId: player.currentRoom.id,
      starFragments: player.starFragments,
      bond: player.bondByElement.entries.map((entry) {
        return BondSaveState(element: entry.key, percent: entry.value);
      }).toList(),
      animalInventory: player.animalInventory.map(_captureAnimal).toList(),
    );
  }

  static RoomSaveState _captureRoom(Room room) {
    return RoomSaveState(
      roomId: room.id,
      encounterAnimals: room.encounterAnimals.map(_captureAnimal).toList(),
      storedAnimals: room.storedAnimals.map(_captureAnimal).toList(),
    );
  }

  static GuardianSaveState _captureGuardian(GuardianState guardian) {
    return GuardianSaveState(
      name: guardian.character.name,
      roomId: guardian.character.currentRoom.id,
      defeated: guardian.character.defeated,
      starFragments: guardian.character.starFragments,
      battleTeamTemplate: guardian.character.battleTeamTemplate
          .map(_captureAnimal)
          .toList(),
    );
  }

  static AnimalSaveState _captureAnimal(Animal animal) {
    return AnimalSaveState(
      id: animal.id,
      name: animal.name,
      element: animal.element,
      level: animal.level,
      health: animal.health,
      baseHealth: animal.baseHealth,
      speed: animal.speed,
      moves: animal.moves.map(_captureMove).toList(),
    );
  }

  static MoveSaveState _captureMove(BattleMove move) {
    return MoveSaveState(
      name: move.name,
      type: move.type,
      power: move.power,
      effect: move.effect,
    );
  }

  static Animal _restoreAnimal(AnimalSaveState saveState) {
    return Animal(
      id: saveState.id,
      name: saveState.name,
      element: saveState.element,
      speed: saveState.speed,
      baseHealth: saveState.baseHealth,
      health: saveState.health,
      level: saveState.level,
      moves: saveState.moves.map(_restoreMove),
    );
  }

  static BattleMove _restoreMove(MoveSaveState saveState) {
    return BattleMove(
      name: saveState.name,
      type: saveState.type,
      power: saveState.power,
      effect: saveState.effect,
    );
  }
}
