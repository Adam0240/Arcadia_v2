import 'dart:collection';
import 'dart:ui';

import 'package:flame/components.dart';

// Replace these temporary values with the selected animal's element color
// configuration when elemental overworld variants are wired into game data.
const bool enablePlayerElementTint = false;
const Color playerElementTintColor = Color.fromARGB(255, 249, 137, 0);
const double playerElementTintStrength = 0.6;

enum PlayerFacing { up, down, left, right }

enum PlayerAnimationState {
  idleUp,
  idleDown,
  idleRight,
  walkUp,
  walkDown,
  walkRight,
}

class ArcadiaPlayerComponent extends PositionComponent {
  ArcadiaPlayerComponent({
    Vector2? initialPosition,
    this.moveSpeed = 174.6,
    this.elementTintEnabled = enablePlayerElementTint,
    this.elementTintColor = playerElementTintColor,
    this.elementTintStrength = playerElementTintStrength,
  }) : assert(
         elementTintStrength >= 0 && elementTintStrength <= 1,
         'Element tint strength must be between 0 and 1.',
       ),
       targetPosition = initialPosition?.clone() ?? Vector2.zero(),
       _visual = SpriteAnimationGroupComponent<PlayerAnimationState>(
         position: Vector2(16, 32),
         size: Vector2(24, 48),
         anchor: Anchor.bottomCenter,
         playing: true,
       ),
       super(
         position: initialPosition?.clone() ?? Vector2.zero(),
         size: Vector2.all(32),
         anchor: Anchor.center,
       ) {
    _applyElementTint();
    add(_visual);
  }

  static const double _stopDistance = 1;
  static final Vector2 _downVisualSize = Vector2(24, 48);
  static final Vector2 _upVisualSize = Vector2(38, 40);
  static final Vector2 _horizontalVisualSize = Vector2.all(40);

  final double moveSpeed;
  final bool elementTintEnabled;
  final Color elementTintColor;
  final double elementTintStrength;
  final SpriteAnimationGroupComponent<PlayerAnimationState> _visual;
  final ListQueue<Vector2> _waypoints = ListQueue();

  Vector2 targetPosition;
  PlayerFacing _facing = PlayerFacing.down;
  PlayerAnimationState _animationState = PlayerAnimationState.idleDown;

  bool get isMoving => _waypoints.isNotEmpty;
  bool get isFacingLeft => _facing == PlayerFacing.left;
  bool get playing => _visual.playing;
  PlayerFacing get facing => _facing;
  PlayerAnimationState get animationState => _animationState;
  SpriteAnimation? get animation => _visual.animation;
  Map<PlayerAnimationState, SpriteAnimation>? get animations =>
      _visual.animations;
  Vector2 get visualSize => _visual.size;
  bool get isVisualMirrored => _visual.scale.x < 0;
  int? get currentAnimationFrame => _visual.animationTicker?.currentIndex;
  ColorFilter? get visualColorFilter => _visual.paint.colorFilter;

  void configureAnimations(
    Map<PlayerAnimationState, SpriteAnimation> animations,
  ) {
    _visual.animations = animations;
    _syncVisualState();
  }

  void moveTo(Vector2 target) {
    _waypoints
      ..clear()
      ..add(target.clone());
    targetPosition = target.clone();
  }

  void followWaypoints(Iterable<Vector2> waypoints) {
    _waypoints.clear();
    for (final waypoint in waypoints) {
      if (position.distanceTo(waypoint) > _stopDistance) {
        _waypoints.add(waypoint.clone());
      }
    }
    targetPosition = _waypoints.isEmpty
        ? position.clone()
        : _waypoints.last.clone();
  }

  @override
  void update(double dt) {
    var remainingDistance = moveSpeed * dt;
    while (_waypoints.isNotEmpty) {
      final waypoint = _waypoints.first;
      final distance = position.distanceTo(waypoint);
      _setFacingFromDirection(waypoint - position);
      _setAnimationState(_walkingAnimationForFacing());
      if (distance <= _stopDistance) {
        position.setFrom(waypoint);
        _waypoints.removeFirst();
        continue;
      }
      if (remainingDistance >= distance) {
        position.setFrom(waypoint);
        _waypoints.removeFirst();
        remainingDistance -= distance;
        continue;
      }

      final direction = waypoint - position;
      position.add(direction.normalized() * remainingDistance);
      break;
    }

    if (_waypoints.isEmpty) {
      targetPosition = position.clone();
      _setAnimationState(_idleAnimationForFacing());
    }

    super.update(dt);
  }

  void _setFacingFromDirection(Vector2 direction) {
    if (direction.isZero()) {
      return;
    }

    final previousFacing = _facing;
    if (direction.x.abs() > direction.y.abs()) {
      _facing = direction.x < 0 ? PlayerFacing.left : PlayerFacing.right;
    } else {
      _facing = direction.y < 0 ? PlayerFacing.up : PlayerFacing.down;
    }
    if (_facing != previousFacing) {
      _syncVisualState();
    }
  }

  PlayerAnimationState _walkingAnimationForFacing() {
    return switch (_facing) {
      PlayerFacing.up => PlayerAnimationState.walkUp,
      PlayerFacing.down => PlayerAnimationState.walkDown,
      PlayerFacing.left || PlayerFacing.right => PlayerAnimationState.walkRight,
    };
  }

  PlayerAnimationState _idleAnimationForFacing() {
    return switch (_facing) {
      PlayerFacing.up => PlayerAnimationState.idleUp,
      PlayerFacing.down => PlayerAnimationState.idleDown,
      PlayerFacing.left || PlayerFacing.right => PlayerAnimationState.idleRight,
    };
  }

  void _setAnimationState(PlayerAnimationState state) {
    if (_animationState == state) {
      return;
    }

    _animationState = state;
    _syncVisualState();
  }

  void _syncVisualState() {
    if (_visual.animations != null) {
      _visual.current = _animationState;
    }

    _visual.size.setFrom(switch (_facing) {
      PlayerFacing.up => _upVisualSize,
      PlayerFacing.down => _downVisualSize,
      PlayerFacing.left || PlayerFacing.right => _horizontalVisualSize,
    });
    _visual.scale.x = _facing == PlayerFacing.left ? -1 : 1;
  }

  void _applyElementTint() {
    _visual.paint.colorFilter = elementTintEnabled
        ? ColorFilter.mode(
            elementTintColor.withValues(alpha: elementTintStrength),
            BlendMode.srcATop,
          )
        : null;
  }
}
