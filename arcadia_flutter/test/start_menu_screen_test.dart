import 'package:arcadia_flutter/main.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/saves/game_save_repository.dart';
import 'package:arcadia_flutter/saves/game_save_state.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the app starts at the save-aware startup menu when no save exists.
  testWidgets('start menu shows new game when no save exists', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    expect(find.text('Arcadia'), findsOneWidget);
    expect(find.text('New Game'), findsWidgets);
    expect(find.text('Load Game'), findsNothing);
    expect(find.text('Delete Game'), findsNothing);

    await tester.tap(find.text('New Game'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField), 'Nova');
    await tester.tap(find.text('Start'));
    await tester.pumpAndSettle();

    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(find.text('Menu'), findsOneWidget);
  });

  // Verifies new game asks for a player name before entering the map.
  testWidgets('new game opens player name prompt', (WidgetTester tester) async {
    final saveRepository = _MemoryGameSaveRepository();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('New Game'));
    await tester.pumpAndSettle();

    expect(find.text('New Game'), findsWidgets);
    expect(find.text('Player Name'), findsOneWidget);
    expect(find.text('Start'), findsOneWidget);
    expect(find.text('Cancel'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsNothing);
  });

  // Verifies empty player names are rejected in the dialog.
  testWidgets('new game rejects empty player name', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('New Game'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField), '   ');
    await tester.tap(find.text('Start'));
    await tester.pumpAndSettle();

    expect(find.text('Player name cannot be empty.'), findsOneWidget);
    expect(find.text('Player Name'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsNothing);
  });

  // Verifies cancel returns to the start menu without creating a session.
  testWidgets('new game cancel stays on start menu', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('New Game'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Cancel'));
    await tester.pumpAndSettle();

    expect(find.text('New Game'), findsOneWidget);
    expect(find.text('Player Name'), findsNothing);
    expect(find.text("Maia's Stable"), findsNothing);
  });

  // Verifies the valid trimmed name is used for the created player.
  testWidgets('new game starts with entered player name', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('New Game'));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextField), ' Nova ');
    await tester.tap(find.text('Start'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Save'));
    await tester.pumpAndSettle();

    expect(saveRepository.savedPlayerName, 'Nova');
  });

  // Verifies existing saves expose load and delete actions.
  testWidgets('start menu shows load and delete when save exists', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();
    final savedSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    )..move(RoomDirection.north);
    await savedSession.saveGame();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    expect(find.text('New Game'), findsNothing);
    expect(find.text('Load Game'), findsOneWidget);
    expect(find.text('Delete Game'), findsOneWidget);
  });

  // Verifies loading from the startup menu opens the saved game state.
  testWidgets('start menu loads saved game', (WidgetTester tester) async {
    final saveRepository = _MemoryGameSaveRepository();
    final savedSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    )..move(RoomDirection.north);
    await savedSession.saveGame();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Load Game'));
    await tester.pumpAndSettle();

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text('Menu'), findsOneWidget);
  });

  // Verifies deleting a save returns the startup menu to new-game mode.
  testWidgets('start menu deletes saved game', (WidgetTester tester) async {
    final saveRepository = _MemoryGameSaveRepository();
    final savedSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );
    await savedSession.saveGame();

    await tester.pumpWidget(ArcadiaApp(saveRepository: saveRepository));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Delete Game'));
    await tester.pumpAndSettle();

    expect(find.text('Save data deleted.'), findsOneWidget);
    expect(find.text('New Game'), findsOneWidget);
    expect(find.text('Load Game'), findsNothing);
    expect(await saveRepository.exists(), isFalse);
  });

  // Verifies startup load errors are handled on the startup menu.
  testWidgets('start menu reports load failure', (WidgetTester tester) async {
    await tester.pumpWidget(
      ArcadiaApp(saveRepository: _FailingLoadGameSaveRepository()),
    );
    await tester.pumpAndSettle();

    await tester.tap(find.text('Load Game'));
    await tester.pumpAndSettle();

    expect(find.text('Save data could not be loaded.'), findsOneWidget);
    expect(find.text('Load Game'), findsOneWidget);
    expect(find.text('Delete Game'), findsOneWidget);
  });
}

class _MemoryGameSaveRepository implements GameSaveRepository {
  GameSaveState? _saveState;

  String? get savedPlayerName => _saveState?.player.name;

  @override
  Future<bool> exists() async {
    return _saveState != null;
  }

  @override
  Future<GameSaveState?> load() async {
    return _saveState;
  }

  @override
  Future<void> save(GameSaveState saveState) async {
    _saveState = saveState;
  }

  @override
  Future<bool> delete() async {
    final hadSave = _saveState != null;
    _saveState = null;
    return hadSave;
  }
}

class _FailingLoadGameSaveRepository implements GameSaveRepository {
  @override
  Future<bool> exists() async {
    return true;
  }

  @override
  Future<GameSaveState?> load() async {
    throw const FormatException('Bad save file.');
  }

  @override
  Future<void> save(GameSaveState saveState) async {}

  @override
  Future<bool> delete() async {
    return true;
  }
}
