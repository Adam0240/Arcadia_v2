import 'dart:ui';

import 'package:arcadia_flutter/game/flame/arcadia_game.dart';
import 'package:arcadia_flutter/game/flame/arcadia_player_component.dart';
import 'package:arcadia_flutter/game/flame/map_loader.dart';
import 'package:flame/components.dart';
import 'package:flame/game.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies a Tiled player_spawn position replaces the default player position.
  testWidgets('game uses loaded map spawn position', (
    WidgetTester tester,
  ) async {
    const expectedSpawn = Offset(96, 128);
    final spawnGame = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          spawnPosition: Vector2(expectedSpawn.dx, expectedSpawn.dy),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: spawnGame));
    await tester.pump();
    await tester.runAsync(() => spawnGame.loaded);
    await tester.pump();

    expect(spawnGame.player.position, Vector2(96, 128));
    expect(spawnGame.player.animation, isNotNull);
    expect(
      spawnGame.player.animations,
      hasLength(PlayerAnimationState.values.length),
    );
    expect(
      spawnGame.player.animation!.frames,
      hasLength(ArcadiaGame.playerSpriteFrameCount),
    );
    expect(
      spawnGame.player.animation!.frames.first.sprite.srcPosition,
      Vector2(80, 32),
    );
    expect(
      spawnGame.player.animation!.frames.last.sprite.srcPosition,
      Vector2(3920, 32),
    );
    expect(
      spawnGame.player.animation!.frames.first.sprite.srcSize,
      Vector2(96, 192),
    );
    expect(spawnGame.player.animationState, PlayerAnimationState.idleDown);
    expect(spawnGame.player.playing, isTrue);
    expect(spawnGame.player.size, Vector2.all(32));
    expect(spawnGame.player.visualSize, Vector2(24, 48));
    expect(
      spawnGame
          .player
          .animations![PlayerAnimationState.walkUp]!
          .frames
          .first
          .sprite
          .srcPosition,
      Vector2(64, 1344),
    );
  });

  testWidgets('player animation advances while moving and idle', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(spawnPosition: Vector2.zero()),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    await tester.runAsync(() => game.loaded);
    await tester.pump();

    game.player.moveTo(Vector2(100, 0));
    game.player.update(ArcadiaGame.playerSpriteFrameStepTime * 2);

    expect(game.player.playing, isTrue);
    expect(game.player.animationState, PlayerAnimationState.walkRight);

    game.player.update(1);

    expect(game.player.playing, isTrue);
    expect(game.player.animationState, PlayerAnimationState.idleRight);
  });

  // Verifies loaded maps use a uniform camera centered around the player.
  testWidgets('game uses zoomed player-follow camera', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    expect(game.camera.viewfinder.anchor, Anchor.center);
    expect(game.camera.viewfinder.position, Vector2(704, 560));
    expect(game.camera.viewfinder.visibleGameSize, isNull);
    expect(game.camera.viewfinder.zoom, ArcadiaGame.explorationZoom);
    expect(game.camera.viewfinder.transform.scale.x, closeTo(1, 0.001));
    expect(game.camera.viewfinder.transform.scale.y, closeTo(1, 0.001));
    expect(game.camera.visibleWorldRect.width, closeTo(800, 0.001));
    expect(game.camera.visibleWorldRect.height, closeTo(600, 0.001));
  });

  // Verifies the zoomed camera follows player movement.
  testWidgets('game camera follows player movement', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.player.position = Vector2(800, 600);
    await tester.pump(const Duration(milliseconds: 16));

    expect(game.camera.viewfinder.position.x, closeTo(800, 0.001));
    expect(game.camera.viewfinder.position.y, closeTo(600, 0.001));
  });

  // Verifies camera panning does not move the player or change its target.
  testWidgets('game pans camera without moving player', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    final originalPlayerPosition = game.player.position.clone();
    final originalPlayerTarget = game.player.targetPosition.clone();

    game.panCameraByCanvasDelta(Vector2(100, 50));
    await tester.pump(const Duration(milliseconds: 16));

    expect(game.camera.viewfinder.position.x, closeTo(604, 0.001));
    expect(game.camera.viewfinder.position.y, closeTo(510, 0.001));
    expect(game.player.position, originalPlayerPosition);
    expect(game.player.targetPosition, originalPlayerTarget);
  });

  // Verifies a normal tap moves the player without recentering a panned camera.
  testWidgets('game tap keeps manually panned camera position', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.panCameraByCanvasDelta(Vector2(100, 50));
    final pannedCameraPosition = game.camera.viewfinder.position.clone();
    game.setTargetFromCanvasPosition(Vector2(500, 300));
    await tester.pump(const Duration(milliseconds: 16));

    expect(game.camera.viewfinder.position, pannedCameraPosition);
    expect(game.player.targetPosition, isNot(Vector2(704, 560)));
  });

  // Verifies a drag gesture pans the camera without becoming a movement tap.
  testWidgets('drag gesture pans camera without moving player', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    final originalPlayerPosition = game.player.position.clone();
    final originalPlayerTarget = game.player.targetPosition.clone();

    await tester.drag(
      find.byType(GameWidget<ArcadiaGame>),
      const Offset(100, 50),
    );
    await tester.pump(const Duration(milliseconds: 50));

    expect(game.camera.viewfinder.position.x, closeTo(604, 0.001));
    expect(game.camera.viewfinder.position.y, closeTo(510, 0.001));
    expect(game.player.position, originalPlayerPosition);
    expect(game.player.targetPosition, originalPlayerTarget);
  });

  // Verifies camera zoom does not move the player or change its target.
  testWidgets('game zooms camera without moving player', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    final originalPlayerPosition = game.player.position.clone();
    final originalPlayerTarget = game.player.targetPosition.clone();

    game.zoomCameraByScale(1.5, focalCanvasPosition: Vector2(500, 300));
    await tester.pump(const Duration(milliseconds: 16));

    expect(game.camera.viewfinder.zoom, closeTo(1.5, 0.001));
    expect(game.player.position, originalPlayerPosition);
    expect(game.player.targetPosition, originalPlayerTarget);
  });

  // Verifies zoom cannot expose space beyond the map or become impractical.
  testWidgets('game clamps camera zoom range', (WidgetTester tester) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.zoomCameraByScale(100);
    expect(game.camera.viewfinder.zoom, ArcadiaGame.maximumExplorationZoom);

    game.zoomCameraByScale(0.0001);
    expect(game.camera.viewfinder.zoom, ArcadiaGame.minimumExplorationZoom);
  });

  // Verifies zooming out near a map edge cannot expose empty space.
  testWidgets('zoomed-out camera remains inside map bounds', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.panCameraByCanvasDelta(Vector2(10000, 10000));
    game.zoomCameraByScale(0.0001, focalCanvasPosition: Vector2.zero());
    await tester.pump(const Duration(milliseconds: 16));

    expect(game.camera.visibleWorldRect.left, greaterThanOrEqualTo(-0.001));
    expect(game.camera.visibleWorldRect.top, greaterThanOrEqualTo(-0.001));
    expect(game.camera.visibleWorldRect.right, lessThanOrEqualTo(1408.001));
    expect(game.camera.visibleWorldRect.bottom, lessThanOrEqualTo(1120.001));
  });

  // Verifies a two-finger pinch reaches the camera zoom controls.
  testWidgets('pinch gesture zooms camera without moving player', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          mapSize: Vector2(1408, 1120),
          spawnPosition: Vector2(704, 560),
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    await tester.pump();
    final originalPlayerPosition = game.player.position.clone();
    final originalPlayerTarget = game.player.targetPosition.clone();
    final center = tester.getCenter(find.byType(GameWidget<ArcadiaGame>));
    final firstFinger = await tester.startGesture(
      center - const Offset(50, 0),
      pointer: 1,
    );
    final secondFinger = await tester.startGesture(
      center + const Offset(50, 0),
      pointer: 2,
    );

    await tester.pump();
    await firstFinger.moveTo(center - const Offset(100, 0));
    await secondFinger.moveTo(center + const Offset(100, 0));
    await tester.pump();
    await firstFinger.up();
    await secondFinger.up();
    await tester.pump(const Duration(milliseconds: 50));

    expect(game.camera.viewfinder.zoom, greaterThan(1));
    expect(game.player.position, originalPlayerPosition);
    expect(game.player.targetPosition, originalPlayerTarget);
  });

  // Verifies taps inside a collision rectangle do not change the movement target.
  testWidgets('game rejects blocked tap target', (WidgetTester tester) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        const ArcadiaMapLoadResult(
          collisionRectangles: [Rect.fromLTWH(40, 40, 100, 100)],
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    final originalTarget = game.player.targetPosition.clone();

    game.setTargetFromCanvasPosition(Vector2(80, 80));

    expect(game.player.targetPosition, originalTarget);
  });

  // Verifies missing WalkPaths falls back to straight-line tap movement.
  testWidgets('game falls back to unblocked straight-line tap target', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        const ArcadiaMapLoadResult(
          collisionRectangles: [Rect.fromLTWH(40, 40, 100, 100)],
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.setTargetFromCanvasPosition(Vector2(200, 200));

    expect(game.player.targetPosition, Vector2(200, 200));
  });

  // Verifies taps use a connected trail route when WalkPaths are available.
  testWidgets('game routes tap target through walk paths', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          spawnPosition: Vector2(20, 20),
          walkPathPolylines: [
            [Vector2(20, 20), Vector2(100, 20), Vector2(100, 200)],
          ],
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();

    game.setTargetFromCanvasPosition(Vector2(130, 180));

    expect(game.player.targetPosition.x, closeTo(100, 0.001));
    expect(game.player.targetPosition.y, closeTo(180, 0.001));

    game.player.update(1);

    expect(game.player.position.x, closeTo(100, 0.001));
    expect(game.player.position.y, greaterThan(20));
  });

  // Verifies collision rejection happens before a blocked tap snaps to a trail.
  testWidgets('game rejects blocked tap before walk path snapping', (
    WidgetTester tester,
  ) async {
    final game = ArcadiaGame(
      mapLoader: _FakeMapLoader(
        ArcadiaMapLoadResult(
          spawnPosition: Vector2(20, 20),
          collisionRectangles: const [Rect.fromLTWH(120, 120, 40, 40)],
          walkPathPolylines: [
            [Vector2(20, 20), Vector2(100, 20), Vector2(100, 200)],
          ],
        ),
      ),
    );

    await tester.pumpWidget(GameWidget(game: game));
    await tester.pump();
    final originalTarget = game.player.targetPosition.clone();

    game.setTargetFromCanvasPosition(Vector2(140, 140));

    expect(game.player.targetPosition, originalTarget);
  });
}

class _FakeMapLoader extends ArcadiaMapLoader {
  const _FakeMapLoader(this.result);

  final ArcadiaMapLoadResult result;

  @override
  Future<ArcadiaMapLoadResult> loadFirstTown() async {
    return result;
  }
}
