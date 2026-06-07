import 'dart:ui';

import 'package:arcadia_flutter/game/flame/arcadia_player_component.dart';
import 'package:flame/components.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the player moves directly toward its selected target.
  test('player moves toward target in a straight line', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.moveTo(Vector2(100, 0));
    player.update(0.5);

    expect(player.position.x, closeTo(50, 0.001));
    expect(player.position.y, closeTo(0, 0.001));
    expect(player.isMoving, isTrue);
  });

  // Verifies the player stops exactly at its target.
  test('player stops when it reaches target', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.moveTo(Vector2(25, 0));
    player.update(1);

    expect(player.position, Vector2(25, 0));
    expect(player.isMoving, isFalse);
  });

  // Verifies the player consumes several trail waypoints without stopping between them.
  test('player follows multiple waypoints', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.followWaypoints([Vector2(100, 0), Vector2(100, 100)]);
    player.update(1.5);

    expect(player.position.x, closeTo(100, 0.001));
    expect(player.position.y, closeTo(50, 0.001));
    expect(player.targetPosition, Vector2(100, 100));
    expect(player.isMoving, isTrue);

    player.update(0.5);

    expect(player.position, Vector2(100, 100));
    expect(player.isMoving, isFalse);
  });

  // Verifies snapping to a close waypoint does not move backward on the next segment.
  test('player preserves movement budget after snapping to close waypoint', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.followWaypoints([Vector2(10, 0), Vector2(100, 0)]);
    player.update(0.095);
    player.update(0.1);

    expect(player.position.x, closeTo(20, 0.001));
    expect(player.position.y, closeTo(0, 0.001));
  });

  test('player animation keeps playing while moving and idle', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    expect(player.playing, isTrue);

    player.moveTo(Vector2(100, 0));
    player.update(0.5);

    expect(player.playing, isTrue);

    player.update(0.5);

    expect(player.playing, isTrue);
  });

  test('player default move speed is reduced by three percent', () {
    final player = ArcadiaPlayerComponent(initialPosition: Vector2.zero());

    expect(player.moveSpeed, closeTo(174.6, 0.001));
  });

  test('player applies the temporary Nature tint by default', () {
    final player = ArcadiaPlayerComponent(initialPosition: Vector2.zero());

    expect(player.elementTintEnabled, isTrue);
    expect(player.elementTintColor, const Color(0xFF4CAF50));
    expect(player.elementTintStrength, 0.6);
    expect(player.visualColorFilter, isNotNull);
  });

  test('player element tint can be disabled with one value', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      elementTintEnabled: false,
    );

    expect(player.elementTintEnabled, isFalse);
    expect(player.visualColorFilter, isNull);
  });

  test('player uses mirrored right animation when moving left', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.moveTo(Vector2(-100, 0));
    player.update(0.1);

    expect(player.isFacingLeft, isTrue);
    expect(player.animationState, PlayerAnimationState.walkRight);
    expect(player.isVisualMirrored, isTrue);

    player.moveTo(Vector2(100, 0));
    player.update(0.1);

    expect(player.isFacingLeft, isFalse);
    expect(player.animationState, PlayerAnimationState.walkRight);
    expect(player.isVisualMirrored, isFalse);
  });

  test('player uses directional walk and matching idle animations', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    expect(player.facing, PlayerFacing.down);
    expect(player.animationState, PlayerAnimationState.idleDown);

    player.moveTo(Vector2(0, -10));
    player.update(0.1);

    expect(player.facing, PlayerFacing.up);
    expect(player.animationState, PlayerAnimationState.idleUp);
    expect(player.visualSize, Vector2(38, 40));

    player.moveTo(Vector2(0, 100));
    player.update(0.1);

    expect(player.facing, PlayerFacing.down);
    expect(player.animationState, PlayerAnimationState.walkDown);
    expect(player.visualSize, Vector2(24, 48));

    player.update(1);

    expect(player.animationState, PlayerAnimationState.idleDown);
  });

  test('player preserves last facing direction when becoming idle', () {
    final player = ArcadiaPlayerComponent(
      initialPosition: Vector2.zero(),
      moveSpeed: 100,
    );

    player.moveTo(Vector2(-10, 0));
    player.update(0.1);

    expect(player.facing, PlayerFacing.left);
    expect(player.animationState, PlayerAnimationState.idleRight);
    expect(player.isVisualMirrored, isTrue);
  });
}
