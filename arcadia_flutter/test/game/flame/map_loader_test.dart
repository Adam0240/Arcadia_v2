import 'package:arcadia_flutter/game/flame/map_loader.dart';
import 'package:flame/components.dart';
import 'package:flame_tiled/flame_tiled.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the declared first town asset parses as a Tiled map.
  testWidgets('first town map parses from declared assets', (
    WidgetTester tester,
  ) async {
    final contents = await rootBundle.loadString(
      '${ArcadiaMapLoader.mapsPrefix}${ArcadiaMapLoader.firstTownFileName}',
    );
    final map = await TiledMap.fromString(
      contents,
      (key) => throw StateError('Unexpected tileset: $key'),
    );

    expect(map.tileWidth, 32);
    expect(map.tileHeight, 32);
    expect(ArcadiaMapLoader.extractMetadata(map).walkPathPolylines, isNotEmpty);
  });

  // Verifies map loading failures return fallback data instead of throwing.
  test('map loader returns controlled fallback result on failure', () async {
    final loader = ArcadiaMapLoader(
      componentLoader:
          (fileName, destinationTileSize, {required prefix}) async {
            throw StateError('Missing map.');
          },
    );

    final result = await loader.loadFirstTown();

    expect(result.loaded, isFalse);
    expect(result.error, isA<StateError>());
    expect(result.collisionRectangles, isEmpty);
    expect(result.doors, isEmpty);
    expect(result.walkPathPolylines, isEmpty);
    expect(result.mapSize, isNull);
    expect(result.spawnPosition, isNull);
  });

  // Verifies expected Tiled object layers produce exploration metadata.
  test('map metadata extracts spawn collision doors and walk paths', () {
    final map = TiledMap(
      width: 20,
      height: 20,
      tileWidth: 32,
      tileHeight: 32,
      layers: [
        ObjectGroup(
          name: ArcadiaMapLoader.collisionLayerName,
          objects: [
            TiledObject(
              id: 1,
              x: 32,
              y: 64,
              width: 96,
              height: 32,
              rectangle: true,
            ),
          ],
        ),
        ObjectGroup(
          name: ArcadiaMapLoader.doorsLayerName,
          objects: [
            TiledObject(
              id: 2,
              name: 'stable_door',
              x: 160,
              y: 192,
              width: 32,
              height: 32,
              rectangle: true,
            ),
          ],
        ),
        ObjectGroup(
          name: ArcadiaMapLoader.spawnLayerName,
          objects: [
            TiledObject(
              id: 3,
              name: ArcadiaMapLoader.playerSpawnName,
              x: 80,
              y: 112,
              point: true,
            ),
          ],
        ),
        ObjectGroup(
          name: ArcadiaMapLoader.walkPathsLayerName,
          offsetX: 4,
          offsetY: 6,
          objects: [
            TiledObject(
              id: 4,
              x: 100,
              y: 200,
              polyline: [
                Point(x: 0, y: 0),
                Point(x: 20, y: 10),
                Point(x: 40, y: 10),
              ],
            ),
          ],
        ),
      ],
    );

    final metadata = ArcadiaMapLoader.extractMetadata(map);

    expect(metadata.spawnPosition, Vector2(80, 112));
    expect(metadata.collisionRectangles, [const Rect.fromLTWH(32, 64, 96, 32)]);
    expect(metadata.doors, hasLength(1));
    expect(metadata.doors.single.name, 'stable_door');
    expect(metadata.doors.single.bounds, const Rect.fromLTWH(160, 192, 32, 32));
    expect(metadata.walkPathPolylines, hasLength(1));
    expect(metadata.walkPathPolylines.single, [
      Vector2(104, 206),
      Vector2(124, 216),
      Vector2(144, 216),
    ]);
  });

  // Verifies missing expected object layers are handled as empty metadata.
  test('map metadata safely handles missing expected layers', () {
    final map = TiledMap(width: 20, height: 20, tileWidth: 32, tileHeight: 32);

    final metadata = ArcadiaMapLoader.extractMetadata(map);

    expect(metadata.spawnPosition, isNull);
    expect(metadata.collisionRectangles, isEmpty);
    expect(metadata.doors, isEmpty);
    expect(metadata.walkPathPolylines, isEmpty);
  });
}
