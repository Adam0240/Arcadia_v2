import 'package:arcadia_flutter/game/flame/arcadia_game.dart';
import 'package:arcadia_flutter/game/flame/map_loader.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/screens/world_screen.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flame/game.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flame world renders safely without an available Tiled map.
  testWidgets('world screen renders plain exploration prototype', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(GameMap());

    await tester.pumpWidget(
      MaterialApp(
        home: WorldScreen(gameSession: session, game: _createFallbackGame()),
      ),
    );
    await tester.pump();

    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(find.byType(GameWidget<ArcadiaGame>), findsOneWidget);
    expect(
      find.text('Tap to move. Drag to pan. Pinch to zoom.'),
      findsOneWidget,
    );
  });

  // Verifies taps are converted from the GameWidget canvas into world coordinates.
  testWidgets('world screen moves player target to tapped canvas position', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(GameMap());

    await tester.pumpWidget(
      MaterialApp(
        home: WorldScreen(gameSession: session, game: _createFallbackGame()),
      ),
    );
    await tester.pump();
    await tester.pump();

    final gameWidgetFinder = find.byType(GameWidget<ArcadiaGame>);
    final gameWidget = tester.widget<GameWidget<ArcadiaGame>>(gameWidgetFinder);
    final game = gameWidget.game!;
    await tester.runAsync(() => game.loaded);
    await tester.pump();
    final gameRect = tester.getRect(gameWidgetFinder);
    const localTap = Offset(80, 120);

    await tester.tapAt(gameRect.topLeft + localTap);
    await tester.pump(const Duration(milliseconds: 50));

    final targetOnCanvas = game.camera.localToGlobal(
      game.player.targetPosition,
    );
    expect(targetOnCanvas, closeToVector(Vector2(80, 120)));
  });

  // Verifies taps near the bottom keep the whole player visible.
  testWidgets('world screen clamps bottom tap inside visible canvas', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(GameMap());

    await tester.pumpWidget(
      MaterialApp(
        home: WorldScreen(gameSession: session, game: _createFallbackGame()),
      ),
    );
    await tester.pump();
    await tester.pump();

    final gameWidgetFinder = find.byType(GameWidget<ArcadiaGame>);
    final gameWidget = tester.widget<GameWidget<ArcadiaGame>>(gameWidgetFinder);
    final game = gameWidget.game!;
    await tester.runAsync(() => game.loaded);
    await tester.pump();
    final gameRect = tester.getRect(gameWidgetFinder);
    final localTap = Offset(80, gameRect.height - 1);

    await tester.tapAt(gameRect.topLeft + localTap);
    await tester.pump(const Duration(milliseconds: 50));

    final targetOnCanvas = game.camera.localToGlobal(
      game.player.targetPosition,
    );
    expect(targetOnCanvas.x, closeTo(80, 0.001));
    expect(
      targetOnCanvas.y,
      closeTo(gameRect.height - game.player.size.y / 2, 0.001),
    );
  });
}

ArcadiaGame _createFallbackGame() {
  return ArcadiaGame(
    mapLoader: ArcadiaMapLoader(
      componentLoader:
          (fileName, destinationTileSize, {required prefix}) async {
            throw StateError('Test fallback.');
          },
    ),
  );
}

Matcher closeToVector(Vector2 expected) {
  return predicate<Vector2>(
    (actual) =>
        (actual.x - expected.x).abs() < 0.001 &&
        (actual.y - expected.y).abs() < 0.001,
    'is close to $expected',
  );
}
