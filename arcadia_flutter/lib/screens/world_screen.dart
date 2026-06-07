import 'package:flame/game.dart';
import 'package:flutter/material.dart';

import '../game/flame/arcadia_game.dart';
import '../services/mobile_game_session.dart';

class WorldScreen extends StatefulWidget {
  const WorldScreen({super.key, required this.gameSession, this.game});

  final MobileGameSession gameSession;
  final ArcadiaGame? game;

  @override
  State<WorldScreen> createState() => _WorldScreenState();
}

class _WorldScreenState extends State<WorldScreen> {
  late final ArcadiaGame _game;

  @override
  void initState() {
    super.initState();
    _game = widget.game ?? ArcadiaGame();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(widget.gameSession.currentRoom.name)),
      body: SafeArea(
        child: Stack(
          children: [
            Positioned.fill(
              child: Semantics(
                label:
                    'Arcadia exploration area. Tap to move, drag to pan, and '
                    'pinch to zoom.',
                child: ClipRect(child: GameWidget<ArcadiaGame>(game: _game)),
              ),
            ),
            const Positioned(
              left: 12,
              right: 12,
              bottom: 12,
              child: IgnorePointer(child: _ExplorationHint()),
            ),
          ],
        ),
      ),
    );
  }
}

class _ExplorationHint extends StatelessWidget {
  const _ExplorationHint();

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: BoxDecoration(
        color: Colors.black.withValues(alpha: 0.68),
        borderRadius: BorderRadius.circular(8),
      ),
      child: const Padding(
        padding: EdgeInsets.all(10),
        child: Text(
          'Tap to move. Drag to pan. Pinch to zoom.',
          textAlign: TextAlign.center,
          style: TextStyle(color: Colors.white),
        ),
      ),
    );
  }
}
