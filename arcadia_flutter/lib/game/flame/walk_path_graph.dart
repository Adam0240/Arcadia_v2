import 'dart:math' as math;

import 'package:flame/components.dart';

class WalkPathGraph {
  WalkPathGraph._(this.nodes, this._adjacency);

  static const double defaultMergeThreshold = 12;
  static const double _epsilon = 0.000001;

  final List<Vector2> nodes;
  final Map<int, Map<int, double>> _adjacency;

  bool get isEmpty => _adjacency.values.every((neighbors) => neighbors.isEmpty);

  factory WalkPathGraph.fromPolylines(
    Iterable<List<Vector2>> polylines, {
    double mergeThreshold = defaultMergeThreshold,
  }) {
    final segments = <_SourceSegment>[];
    for (final polyline in polylines) {
      for (var index = 0; index < polyline.length - 1; index++) {
        final start = polyline[index];
        final end = polyline[index + 1];
        if (start.distanceTo(end) > _epsilon) {
          segments.add(_SourceSegment(start.clone(), end.clone()));
        }
      }
    }

    final splitPoints = [
      for (final segment in segments)
        <_SegmentPoint>[
          _SegmentPoint(0, segment.start),
          _SegmentPoint(1, segment.end),
        ],
    ];

    for (var first = 0; first < segments.length; first++) {
      for (var second = first + 1; second < segments.length; second++) {
        final intersection = _intersection(segments[first], segments[second]);
        if (intersection == null) {
          continue;
        }
        splitPoints[first].add(
          _SegmentPoint(intersection.firstAmount, intersection.position),
        );
        splitPoints[second].add(
          _SegmentPoint(intersection.secondAmount, intersection.position),
        );
      }
    }

    final builder = _GraphBuilder(mergeThreshold);
    for (var index = 0; index < segments.length; index++) {
      final points = splitPoints[index]
        ..sort((a, b) => a.amount.compareTo(b.amount));
      for (var pointIndex = 0; pointIndex < points.length - 1; pointIndex++) {
        builder.connect(
          points[pointIndex].position,
          points[pointIndex + 1].position,
        );
      }
    }

    final adjacency = <int, Map<int, double>>{
      for (final entry in builder.adjacency.entries)
        entry.key: Map<int, double>.unmodifiable(entry.value),
    };
    return WalkPathGraph._(
      List.unmodifiable(builder.nodes),
      Map<int, Map<int, double>>.unmodifiable(adjacency),
    );
  }

  WalkPathSnap? nearestPoint(Vector2 position) {
    WalkPathSnap? nearest;
    var nearestDistance = double.infinity;

    for (final edge in _edges) {
      final projected = _projectOntoSegment(
        position,
        nodes[edge.startNode],
        nodes[edge.endNode],
      );
      final distance = position.distanceTo(projected.position);
      if (distance < nearestDistance) {
        nearestDistance = distance;
        nearest = WalkPathSnap(
          position: projected.position,
          startNode: edge.startNode,
          endNode: edge.endNode,
          startDistance: projected.amount * edge.length,
          endDistance: (1 - projected.amount) * edge.length,
        );
      }
    }

    return nearest;
  }

  List<Vector2> route(Vector2 start, Vector2 target) {
    final startSnap = nearestPoint(start);
    final targetSnap = nearestPoint(target);
    if (startSnap == null || targetSnap == null) {
      return const [];
    }

    var bestDistance = double.infinity;
    List<int>? bestNodeRoute;
    for (final startOption in startSnap._nodeOptions) {
      for (final targetOption in targetSnap._nodeOptions) {
        final graphRoute = _shortestNodeRoute(
          startOption.node,
          targetOption.node,
        );
        if (graphRoute == null) {
          continue;
        }
        final totalDistance =
            startOption.distance + graphRoute.distance + targetOption.distance;
        if (totalDistance < bestDistance) {
          bestDistance = totalDistance;
          bestNodeRoute = graphRoute.nodes;
        }
      }
    }

    if (startSnap.isOnSameEdgeAs(targetSnap)) {
      final directDistance = startSnap.position.distanceTo(targetSnap.position);
      if (directDistance <= bestDistance) {
        return _deduplicate([startSnap.position, targetSnap.position]);
      }
    }

    if (bestNodeRoute == null) {
      return const [];
    }

    return _deduplicate([
      startSnap.position,
      for (final node in bestNodeRoute) nodes[node],
      targetSnap.position,
    ]);
  }

  Iterable<_GraphEdge> get _edges sync* {
    for (final entry in _adjacency.entries) {
      for (final neighbor in entry.value.entries) {
        if (entry.key < neighbor.key) {
          yield _GraphEdge(entry.key, neighbor.key, neighbor.value);
        }
      }
    }
  }

  _NodeRoute? _shortestNodeRoute(int start, int target) {
    final distances = <int, double>{start: 0};
    final previous = <int, int>{};
    final unvisited = nodes.indexed.map((entry) => entry.$1).toSet();

    while (unvisited.isNotEmpty) {
      int? current;
      var currentDistance = double.infinity;
      for (final candidate in unvisited) {
        final distance = distances[candidate] ?? double.infinity;
        if (distance < currentDistance) {
          current = candidate;
          currentDistance = distance;
        }
      }

      if (current == null || currentDistance == double.infinity) {
        return null;
      }
      if (current == target) {
        final route = <int>[current];
        while (previous.containsKey(route.last)) {
          route.add(previous[route.last]!);
        }
        return _NodeRoute(
          route.reversed.toList(growable: false),
          currentDistance,
        );
      }

      unvisited.remove(current);
      for (final neighbor in _adjacency[current]!.entries) {
        if (!unvisited.contains(neighbor.key)) {
          continue;
        }
        final candidateDistance = currentDistance + neighbor.value;
        if (candidateDistance < (distances[neighbor.key] ?? double.infinity)) {
          distances[neighbor.key] = candidateDistance;
          previous[neighbor.key] = current;
        }
      }
    }

    return null;
  }

  static _Intersection? _intersection(
    _SourceSegment first,
    _SourceSegment second,
  ) {
    final firstDirection = first.end - first.start;
    final secondDirection = second.end - second.start;
    final denominator = _cross(firstDirection, secondDirection);
    if (denominator.abs() <= _epsilon) {
      return null;
    }

    final offset = second.start - first.start;
    final firstAmount = _cross(offset, secondDirection) / denominator;
    final secondAmount = _cross(offset, firstDirection) / denominator;
    if (firstAmount < -_epsilon ||
        firstAmount > 1 + _epsilon ||
        secondAmount < -_epsilon ||
        secondAmount > 1 + _epsilon) {
      return null;
    }

    return _Intersection(
      firstAmount.clamp(0, 1).toDouble(),
      secondAmount.clamp(0, 1).toDouble(),
      first.start + firstDirection * firstAmount,
    );
  }

  static _Projection _projectOntoSegment(
    Vector2 point,
    Vector2 start,
    Vector2 end,
  ) {
    final segment = end - start;
    final lengthSquared = segment.length2;
    if (lengthSquared <= _epsilon) {
      return _Projection(0, start.clone());
    }

    final amount = ((point - start).dot(segment) / lengthSquared)
        .clamp(0, 1)
        .toDouble();
    return _Projection(amount, start + segment * amount);
  }

  static double _cross(Vector2 first, Vector2 second) {
    return first.x * second.y - first.y * second.x;
  }

  static List<Vector2> _deduplicate(Iterable<Vector2> points) {
    final result = <Vector2>[];
    for (final point in points) {
      if (result.isEmpty || result.last.distanceTo(point) > _epsilon) {
        result.add(point.clone());
      }
    }
    return result;
  }
}

class WalkPathSnap {
  const WalkPathSnap({
    required this.position,
    required this.startNode,
    required this.endNode,
    required this.startDistance,
    required this.endDistance,
  });

  final Vector2 position;
  final int startNode;
  final int endNode;
  final double startDistance;
  final double endDistance;

  List<_NodeOption> get _nodeOptions => [
    _NodeOption(startNode, startDistance),
    _NodeOption(endNode, endDistance),
  ];

  bool isOnSameEdgeAs(WalkPathSnap other) {
    return (startNode == other.startNode && endNode == other.endNode) ||
        (startNode == other.endNode && endNode == other.startNode);
  }
}

class _GraphBuilder {
  _GraphBuilder(this.mergeThreshold);

  final double mergeThreshold;
  final List<Vector2> nodes = [];
  final Map<int, Map<int, double>> adjacency = {};

  void connect(Vector2 start, Vector2 end) {
    final startNode = _nodeFor(start);
    final endNode = _nodeFor(end);
    if (startNode == endNode) {
      return;
    }

    final distance = nodes[startNode].distanceTo(nodes[endNode]);
    adjacency[startNode]![endNode] = math.min(
      adjacency[startNode]![endNode] ?? double.infinity,
      distance,
    );
    adjacency[endNode]![startNode] = math.min(
      adjacency[endNode]![startNode] ?? double.infinity,
      distance,
    );
  }

  int _nodeFor(Vector2 point) {
    for (var index = 0; index < nodes.length; index++) {
      if (nodes[index].distanceTo(point) <= mergeThreshold) {
        return index;
      }
    }

    nodes.add(point.clone());
    adjacency[nodes.length - 1] = {};
    return nodes.length - 1;
  }
}

class _SourceSegment {
  const _SourceSegment(this.start, this.end);

  final Vector2 start;
  final Vector2 end;
}

class _SegmentPoint {
  const _SegmentPoint(this.amount, this.position);

  final double amount;
  final Vector2 position;
}

class _Intersection {
  const _Intersection(this.firstAmount, this.secondAmount, this.position);

  final double firstAmount;
  final double secondAmount;
  final Vector2 position;
}

class _Projection {
  const _Projection(this.amount, this.position);

  final double amount;
  final Vector2 position;
}

class _GraphEdge {
  const _GraphEdge(this.startNode, this.endNode, this.length);

  final int startNode;
  final int endNode;
  final double length;
}

class _NodeOption {
  const _NodeOption(this.node, this.distance);

  final int node;
  final double distance;
}

class _NodeRoute {
  const _NodeRoute(this.nodes, this.distance);

  final List<int> nodes;
  final double distance;
}
