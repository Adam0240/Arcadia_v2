import 'package:arcadia_flutter/main.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/saves/game_save_repository.dart';
import 'package:arcadia_flutter/saves/game_save_state.dart';
import 'package:arcadia_flutter/screens/arcadia_map_screen.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flutter screen renders the same initial map content as MAUI.
  testWidgets('map screen shows initial room and movement controls', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    expect(find.text('Arcadia'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(
      find.text('Where new trainers obtain their first creature!'),
      findsOneWidget,
    );
    expect(find.text('The journey begins.'), findsOneWidget);
    expect(find.text('North'), findsOneWidget);
    expect(find.text('Inspect'), findsOneWidget);
    expect(find.text('Menu'), findsOneWidget);
  });

  // Verifies movement buttons follow the current room exits.
  testWidgets('movement updates room and disabled exits match current room', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    final southButton = tester.widget<ElevatedButton>(
      find.widgetWithText(ElevatedButton, 'South'),
    );
    expect(southButton.onPressed, isNull);

    await tester.tap(find.text('North'));
    await tester.pump();

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text('Moved to Ikena.'), findsOneWidget);

    final westButton = tester.widget<ElevatedButton>(
      find.widgetWithText(ElevatedButton, 'West'),
    );
    expect(westButton.onPressed, isNotNull);
  });

  // Verifies inspect displays the room interaction text.
  testWidgets('inspect updates status message', (WidgetTester tester) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(
      find.text(
        'Maia checks the starter pens and says the stable is ready for your journey.\nAnimals Nearby: None',
      ),
      findsOneWidget,
    );
  });

  // Verifies inspect displays nearby animals after moving to a wild route.
  testWidgets('inspect shows nearby animals in route rooms', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('North'));
    await tester.pump();
    await tester.tap(find.text('West'));
    await tester.pump();
    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(
      find.text(
        'Tall grass rustles nearby, but this prototype keeps encounters disabled.\nAnimals Nearby: N_DOG',
      ),
      findsOneWidget,
    );
  });

  // Verifies menu swaps the movement grid for player info and save controls.
  testWidgets('menu toggles between movement and save controls', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();
    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );

    await tester.pumpWidget(
      MaterialApp(home: ArcadiaMapScreen(gameSession: gameSession)),
    );

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(find.text('Inventory'), findsOneWidget);
    expect(find.text('Bond'), findsOneWidget);
    expect(find.text('Star Fragments'), findsOneWidget);
    expect(find.text('Save'), findsOneWidget);
    expect(find.text('Load'), findsOneWidget);
    expect(find.text('Return'), findsOneWidget);
    expect(find.text('North'), findsNothing);

    await tester.tap(find.text('Save'));
    await tester.pumpAndSettle();
    expect(find.text('Game saved.'), findsOneWidget);
    expect(await saveRepository.exists(), isTrue);

    await tester.tap(find.text('Return'));
    await tester.pump();
    expect(find.text('North'), findsOneWidget);
    expect(find.text('Save'), findsNothing);
  });

  // Verifies menu inventory displays the player's starter animals.
  testWidgets('menu inventory displays starter animals', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Inventory'));
    await tester.pump();

    expect(
      find.text('Inventory List:\nN_CAT Health: 75\nN_DOG Health: 40'),
      findsOneWidget,
    );
  });

  // Verifies menu bond displays all element bond values.
  testWidgets('menu bond displays element bond values', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Bond'));
    await tester.pump();

    expect(
      find.text(
        'Bond:\nNature 0%/100%\nMystic 0%/100%\nThunder 0%/100%\nDraconic 0%/100%\nCosmic 0%/100%\nNuclear 0%/100%',
      ),
      findsOneWidget,
    );
  });

  // Verifies menu star fragments displays the initial empty state.
  testWidgets('menu star fragments displays empty state', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Star Fragments'));
    await tester.pump();

    expect(find.text('You have no star fragments!'), findsOneWidget);
  });

  // Verifies the screen follows a changed injected game session.
  testWidgets('map screen updates when injected session changes', (
    WidgetTester tester,
  ) async {
    final firstSession = MobileGameSession(GameMap());
    final secondSession = MobileGameSession(GameMap())
      ..move(RoomDirection.north);

    await tester.pumpWidget(
      MaterialApp(home: ArcadiaMapScreen(gameSession: firstSession)),
    );

    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(find.text('Ikena'), findsNothing);

    await tester.pumpWidget(
      MaterialApp(home: ArcadiaMapScreen(gameSession: secondSession)),
    );

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsNothing);
    expect(find.text('The journey begins.'), findsOneWidget);
  });

  // Verifies the menu load action restores a saved session.
  testWidgets('menu load restores saved game state', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();
    final savedSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    )..move(RoomDirection.north);
    await savedSession.saveGame();

    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );

    await tester.pumpWidget(
      MaterialApp(home: ArcadiaMapScreen(gameSession: gameSession)),
    );

    expect(find.text("Maia's Stable"), findsOneWidget);

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Load'));
    await tester.pumpAndSettle();

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text('Game loaded.'), findsOneWidget);
    expect(find.text('North'), findsOneWidget);
  });
}

class _MemoryGameSaveRepository implements GameSaveRepository {
  GameSaveState? _saveState;

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
}
