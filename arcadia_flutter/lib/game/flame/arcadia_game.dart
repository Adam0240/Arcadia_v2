import 'dart:math' as math;
import 'dart:ui';

import 'package:flame/components.dart';
import 'package:flame/events.dart';
import 'package:flame/experimental.dart';
import 'package:flame/game.dart';

import 'arcadia_player_component.dart';
import 'collision_manager.dart';
import 'map_loader.dart';
import 'walk_path_graph.dart';

class ArcadiaGame extends FlameGame with TapCallbacks, DragCallbacks {
  ArcadiaGame({
    ArcadiaPlayerComponent? player,
    this._mapLoader = const ArcadiaMapLoader(),
  }) : player = player ?? ArcadiaPlayerComponent(),
       super();

  final ArcadiaPlayerComponent player;
  final ArcadiaMapLoader _mapLoader;

  static const double explorationZoom = 1;
  static const double minimumExplorationZoom = 0.75;
  static const double maximumExplorationZoom = 2.5;
  static const String playerSpriteSheetPath = 'player/cosmic_cat_overworld.png';
  static const int playerSpriteFrameCount = 16;
  static const double playerSpriteFrameStepTime = 0.1;
  static final Vector2 playerSpriteCellSize = Vector2.all(256);
  static const Map<PlayerAnimationState, int> playerSpriteRows = {
    PlayerAnimationState.idleDown: 0,
    PlayerAnimationState.idleRight: 1,
    PlayerAnimationState.idleUp: 2,
    PlayerAnimationState.walkDown: 3,
    PlayerAnimationState.walkRight: 4,
    PlayerAnimationState.walkUp: 5,
  };

  final Map<int, Vector2> _dragPositions = {};
  double? _pinchStartDistance;
  double? _pinchStartZoom;
  CollisionManager collisionManager = CollisionManager();
  WalkPathGraph? walkPathGraph;
  List<DoorMetadata> doors = const [];
  Object? mapLoadError;
  bool mapLoaded = false;
  Vector2? mapSize;

  @override
  Color backgroundColor() => const Color(0xff75a85a);

  @override
  Future<void> onLoad() async {
    await super.onLoad();

    images.prefix = 'assets/sprites/';
    camera.viewfinder.anchor = Anchor.topLeft;

    final mapResult = await _mapLoader.loadFirstTown();
    final mapComponent = mapResult.component;
    mapSize = mapResult.mapSize?.clone();
    if (mapComponent != null) {
      world.add(mapComponent);
      mapLoaded = true;
    } else {
      mapLoadError = mapResult.error;
    }
    collisionManager = CollisionManager(mapResult.collisionRectangles);
    final graph = WalkPathGraph.fromPolylines(mapResult.walkPathPolylines);
    walkPathGraph = graph.isEmpty ? null : graph;
    doors = mapResult.doors;
    player.position = mapResult.spawnPosition ?? camera.globalToLocal(size / 2);
    player.moveTo(player.position);
    player.configureAnimations(await _loadPlayerAnimations());
    world.add(player);
    if (mapSize != null) {
      _configureMapCamera(mapSize!);
    }
  }

  Future<Map<PlayerAnimationState, SpriteAnimation>>
  _loadPlayerAnimations() async {
    final spriteSheet = await images.load(playerSpriteSheetPath);
    final animations = <PlayerAnimationState, SpriteAnimation>{};
    for (final entry in playerSpriteRows.entries) {
      final state = entry.key;
      final crop = _playerSpriteCrop(state);
      animations[state] = SpriteAnimation.fromFrameData(
        spriteSheet,
        SpriteAnimationData(
          List.generate(playerSpriteFrameCount, (index) {
            return SpriteAnimationFrameData(
              srcPosition:
                  Vector2(
                    index * playerSpriteCellSize.x,
                    entry.value * playerSpriteCellSize.y,
                  ) +
                  crop.offset,
              srcSize: crop.size,
              stepTime: playerSpriteFrameStepTime,
            );
          }),
        ),
      );
    }
    return animations;
  }

  static _PlayerSpriteCrop _playerSpriteCrop(PlayerAnimationState state) {
    return switch (state) {
      PlayerAnimationState.idleDown || PlayerAnimationState.walkDown =>
        _PlayerSpriteCrop(offset: Vector2(80, 32), size: Vector2(96, 192)),
      PlayerAnimationState.idleUp || PlayerAnimationState.walkUp =>
        _PlayerSpriteCrop(offset: Vector2(64, 64), size: Vector2(128, 136)),
      PlayerAnimationState.idleRight || PlayerAnimationState.walkRight =>
        _PlayerSpriteCrop(offset: Vector2.all(64), size: Vector2.all(128)),
    };
  }

  @override
  void onGameResize(Vector2 size) {
    super.onGameResize(size);

    final currentMapSize = mapSize;
    if (currentMapSize != null) {
      _updateMapCameraZoom(currentMapSize);
    }
  }

  @override
  void onTapUp(TapUpEvent event) {
    super.onTapUp(event);
    setTargetFromCanvasPosition(event.canvasPosition);
  }

  @override
  void onDragStart(DragStartEvent event) {
    super.onDragStart(event);
    camera.stop();
    _dragPositions[event.pointerId] = event.canvasPosition;
    _setPinchStartIfNeeded();
  }

  @override
  void onDragUpdate(DragUpdateEvent event) {
    _dragPositions[event.pointerId] = event.canvasEndPosition;
    if (_dragPositions.length < 2) {
      panCameraByCanvasDelta(event.canvasDelta);
      return;
    }

    final positions = _dragPositions.values.take(2).toList();
    final startDistance = _pinchStartDistance;
    final startZoom = _pinchStartZoom;
    if (startDistance == null || startDistance <= 0 || startZoom == null) {
      _setPinchStartIfNeeded();
    } else {
      zoomCameraTo(
        startZoom * ((positions[0] - positions[1]).length / startDistance),
        focalCanvasPosition: (positions[0] + positions[1]) / 2,
      );
    }
  }

  @override
  void onDragEnd(DragEndEvent event) {
    _endDrag(event.pointerId);
    super.onDragEnd(event);
  }

  @override
  void onDragCancel(DragCancelEvent event) {
    _endDrag(event.pointerId);
    super.onDragCancel(event);
  }

  void _setPinchStartIfNeeded() {
    if (_dragPositions.length < 2) {
      return;
    }
    final positions = _dragPositions.values.take(2).toList();
    _pinchStartDistance = (positions[0] - positions[1]).length;
    _pinchStartZoom = camera.viewfinder.zoom;
  }

  void _endDrag(int pointerId) {
    _dragPositions.remove(pointerId);
    _pinchStartDistance = null;
    _pinchStartZoom = null;
    _setPinchStartIfNeeded();
  }

  void panCameraByCanvasDelta(Vector2 canvasDelta) {
    final currentMapSize = mapSize;
    if (currentMapSize == null) {
      return;
    }

    camera.stop();
    camera.viewfinder.position -= canvasDelta / camera.viewfinder.zoom;
    _clampCameraPosition(currentMapSize);
  }

  void zoomCameraByScale(double scale, {Vector2? focalCanvasPosition}) {
    zoomCameraTo(
      camera.viewfinder.zoom * scale,
      focalCanvasPosition: focalCanvasPosition,
    );
  }

  void zoomCameraTo(double zoom, {Vector2? focalCanvasPosition}) {
    final currentMapSize = mapSize;
    if (currentMapSize == null) {
      return;
    }

    camera.stop();
    final focalWorldPosition = focalCanvasPosition == null
        ? null
        : camera.globalToLocal(focalCanvasPosition);
    camera.viewfinder.zoom = _clampMapZoom(zoom, currentMapSize);
    if (focalWorldPosition != null) {
      final worldPositionAfterZoom = camera.globalToLocal(focalCanvasPosition!);
      camera.viewfinder.position += focalWorldPosition - worldPositionAfterZoom;
    }
    _clampCameraPosition(currentMapSize);
  }

  void setTargetFromCanvasPosition(Vector2 canvasPosition) {
    final worldPosition = camera.globalToLocal(canvasPosition);
    final target = _clampToVisibleWorld(worldPosition);
    if (collisionManager.isBlocked(target)) {
      return;
    }

    final graph = walkPathGraph;
    if (graph == null) {
      player.moveTo(target);
      return;
    }

    final route = graph.route(player.position, target);
    if (route.isNotEmpty && !collisionManager.isBlocked(route.last)) {
      player.followWaypoints(route);
    }
  }

  Vector2 _clampToVisibleWorld(Vector2 target) {
    final bounds = mapSize == null
        ? camera.visibleWorldRect
        : Rect.fromLTWH(0, 0, mapSize!.x, mapSize!.y);
    final halfWidth = player.size.x / 2;
    final halfHeight = player.size.y / 2;

    return Vector2(
      target.x.clamp(bounds.left + halfWidth, bounds.right - halfWidth),
      target.y.clamp(bounds.top + halfHeight, bounds.bottom - halfHeight),
    );
  }

  void _configureMapCamera(Vector2 size) {
    camera.viewfinder
      ..anchor = Anchor.center
      ..visibleGameSize = null;
    _updateMapCameraZoom(size);
    camera
      ..follow(player, snap: true)
      ..setBounds(
        Rectangle.fromLTWH(0, 0, size.x, size.y),
        considerViewport: true,
      );
  }

  void _updateMapCameraZoom(Vector2 size) {
    camera.viewfinder.zoom = _clampMapZoom(camera.viewfinder.zoom, size);
    _clampCameraPosition(size);
  }

  double _clampMapZoom(double zoom, Vector2 size) {
    final viewportSize = camera.viewport.virtualSize;
    if (viewportSize.x <= 0 || viewportSize.y <= 0) {
      return zoom;
    }
    final minimumCoverZoom = math.max(
      viewportSize.x / size.x,
      viewportSize.y / size.y,
    );
    final minimumZoom = math.max(minimumExplorationZoom, minimumCoverZoom);
    final maximumZoom = math.max(maximumExplorationZoom, minimumZoom);
    return zoom.clamp(minimumZoom, maximumZoom);
  }

  void _clampCameraPosition(Vector2 size) {
    final viewportSize = camera.viewport.virtualSize;
    final zoom = camera.viewfinder.zoom;
    if (viewportSize.x <= 0 || viewportSize.y <= 0 || zoom <= 0) {
      return;
    }

    final halfVisibleWidth = viewportSize.x / zoom / 2;
    final halfVisibleHeight = viewportSize.y / zoom / 2;
    camera.viewfinder.position = Vector2(
      camera.viewfinder.position.x.clamp(
        halfVisibleWidth,
        size.x - halfVisibleWidth,
      ),
      camera.viewfinder.position.y.clamp(
        halfVisibleHeight,
        size.y - halfVisibleHeight,
      ),
    );
  }
}

class _PlayerSpriteCrop {
  const _PlayerSpriteCrop({required this.offset, required this.size});

  final Vector2 offset;
  final Vector2 size;
}
