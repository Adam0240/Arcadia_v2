import 'package:arcadia_flutter/game/flame/walk_path_graph.dart';
import 'package:flame/components.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies consecutive polyline points produce a connected route.
  test('graph creates route from connected polyline points', () {
    final graph = WalkPathGraph.fromPolylines([
      [Vector2(0, 0), Vector2(100, 0), Vector2(100, 100)],
    ]);

    final route = graph.route(Vector2(10, 20), Vector2(120, 90));

    expect(route.first, closeToVector(Vector2(10, 0)));
    expect(route, contains(closeToVector(Vector2(100, 0))));
    expect(route.last, closeToVector(Vector2(100, 90)));
  });

  // Verifies nearby endpoints are merged into one trail junction.
  test('graph connects nearby polyline endpoints', () {
    final graph = WalkPathGraph.fromPolylines([
      [Vector2(0, 0), Vector2(100, 0)],
      [Vector2(108, 4), Vector2(200, 4)],
    ]);

    final route = graph.route(Vector2.zero(), Vector2(200, 4));

    expect(route, isNotEmpty);
    expect(route.last, closeToVector(Vector2(200, 4)));
  });

  // Verifies crossing polyline segments are split into a connected junction.
  test('graph connects intersecting polyline segments', () {
    final graph = WalkPathGraph.fromPolylines([
      [Vector2(0, 50), Vector2(100, 50)],
      [Vector2(50, 0), Vector2(50, 100)],
    ]);

    final route = graph.route(Vector2(0, 50), Vector2(50, 100));

    expect(route, contains(closeToVector(Vector2(50, 50))));
    expect(route.last, closeToVector(Vector2(50, 100)));
  });

  // Verifies arbitrary positions snap to the nearest point along a graph edge.
  test('nearest graph point projects onto edge', () {
    final graph = WalkPathGraph.fromPolylines([
      [Vector2(0, 0), Vector2(100, 0)],
    ]);

    final snap = graph.nearestPoint(Vector2(40, 30));

    expect(snap, isNotNull);
    expect(snap!.position, closeToVector(Vector2(40, 0)));
  });
}

Matcher closeToVector(Vector2 expected) {
  return predicate<Vector2>(
    (actual) => actual.distanceTo(expected) < 0.001,
    'is close to $expected',
  );
}
