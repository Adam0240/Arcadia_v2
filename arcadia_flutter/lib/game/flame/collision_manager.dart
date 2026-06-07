import 'dart:ui';

import 'package:flame/components.dart';

class CollisionManager {
  CollisionManager([Iterable<Rect> blockedRectangles = const []])
    : _blockedRectangles = List.unmodifiable(blockedRectangles);

  final List<Rect> _blockedRectangles;

  List<Rect> get blockedRectangles => _blockedRectangles;

  bool isBlocked(Vector2 position) {
    final point = Offset(position.x, position.y);
    return _blockedRectangles.any((rectangle) => rectangle.contains(point));
  }
}
