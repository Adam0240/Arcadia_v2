import 'dart:ui';

import 'package:flame/cache.dart';
import 'package:flame/components.dart';
import 'package:flame_tiled/flame_tiled.dart';

class ArcadiaMapLoadResult {
  const ArcadiaMapLoadResult({
    this.component,
    this.mapSize,
    this.spawnPosition,
    this.collisionRectangles = const [],
    this.doors = const [],
    this.walkPathPolylines = const [],
    this.error,
  });

  final TiledComponent? component;
  final Vector2? mapSize;
  final Vector2? spawnPosition;
  final List<Rect> collisionRectangles;
  final List<DoorMetadata> doors;
  final List<List<Vector2>> walkPathPolylines;
  final Object? error;

  bool get loaded => component != null;
}

class DoorMetadata {
  const DoorMetadata({required this.name, required this.bounds});

  final String name;
  final Rect bounds;
}

typedef TiledMapComponentLoader =
    Future<TiledComponent> Function(
      String fileName,
      Vector2 destinationTileSize, {
      required String prefix,
    });

class ArcadiaMapLoader {
  const ArcadiaMapLoader({this.componentLoader = _loadTiledComponent});

  static const String firstTownFileName = 'first_town.tmx';
  static const String mapsPrefix = 'assets/maps/';
  static const String collisionLayerName = 'Collisions';
  static const String doorsLayerName = 'Doors';
  static const String spawnLayerName = 'Spawn';
  static const String walkPathsLayerName = 'WalkPaths';
  static const String playerSpawnName = 'player_spawn';
  static final Vector2 tileSize = Vector2.all(32);

  final TiledMapComponentLoader componentLoader;

  Future<ArcadiaMapLoadResult> loadFirstTown() async {
    try {
      final component = await componentLoader(
        firstTownFileName,
        tileSize,
        prefix: mapsPrefix,
      );
      final metadata = extractMetadata(component.tileMap.map);

      return ArcadiaMapLoadResult(
        component: component,
        mapSize: component.size.clone(),
        spawnPosition: metadata.spawnPosition,
        collisionRectangles: metadata.collisionRectangles,
        doors: metadata.doors,
        walkPathPolylines: metadata.walkPathPolylines,
      );
    } on Object catch (error) {
      return ArcadiaMapLoadResult(error: error);
    }
  }

  static ArcadiaMapMetadata extractMetadata(TiledMap map) {
    final collisionObjects = _objectsInLayer(map, collisionLayerName);
    final doorObjects = _objectsInLayer(map, doorsLayerName);
    final spawnObjects = _objectsInLayer(map, spawnLayerName);
    final walkPathLayer = _objectLayer(map, walkPathsLayerName);
    final playerSpawn = spawnObjects
        .where((object) => object.name == playerSpawnName)
        .firstOrNull;

    return ArcadiaMapMetadata(
      spawnPosition: playerSpawn == null
          ? null
          : Vector2(playerSpawn.x, playerSpawn.y),
      collisionRectangles: collisionObjects
          .where((object) => object.isRectangle)
          .map(_toRect)
          .toList(growable: false),
      doors: doorObjects
          .where((object) => object.isRectangle)
          .map(
            (object) =>
                DoorMetadata(name: object.name, bounds: _toRect(object)),
          )
          .toList(growable: false),
      walkPathPolylines: walkPathLayer == null
          ? const []
          : walkPathLayer.objects
                .where((object) => object.isPolyline)
                .map(
                  (object) => object.polyline
                      .map(
                        (point) => Vector2(
                          object.x + point.x + walkPathLayer.offsetX,
                          object.y + point.y + walkPathLayer.offsetY,
                        ),
                      )
                      .toList(growable: false),
                )
                .where((polyline) => polyline.length >= 2)
                .toList(growable: false),
    );
  }

  static List<TiledObject> _objectsInLayer(TiledMap map, String layerName) {
    return _objectLayer(map, layerName)?.objects ?? const [];
  }

  static ObjectGroup? _objectLayer(TiledMap map, String layerName) {
    for (final layer in map.layers) {
      if (layer.name == layerName && layer is ObjectGroup) {
        return layer;
      }
    }

    return null;
  }

  static Rect _toRect(TiledObject object) {
    return Rect.fromLTWH(object.x, object.y, object.width, object.height);
  }

  static Future<TiledComponent> _loadTiledComponent(
    String fileName,
    Vector2 destinationTileSize, {
    required String prefix,
  }) {
    return TiledComponent.load(
      fileName,
      destinationTileSize,
      prefix: prefix,
      images: Images(prefix: prefix),
    );
  }
}

class ArcadiaMapMetadata {
  const ArcadiaMapMetadata({
    required this.spawnPosition,
    required this.collisionRectangles,
    required this.doors,
    required this.walkPathPolylines,
  });

  final Vector2? spawnPosition;
  final List<Rect> collisionRectangles;
  final List<DoorMetadata> doors;
  final List<List<Vector2>> walkPathPolylines;
}
