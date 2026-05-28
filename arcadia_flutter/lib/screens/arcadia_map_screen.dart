import 'package:flutter/material.dart';

import '../map/game_map.dart';
import '../map/room_direction.dart';
import '../services/mobile_game_session.dart';
import '../widgets/room_artwork.dart';

class ArcadiaMapScreen extends StatefulWidget {
  const ArcadiaMapScreen({super.key, this.gameSession});

  final MobileGameSession? gameSession;

  @override
  State<ArcadiaMapScreen> createState() => _ArcadiaMapScreenState();
}

class _ArcadiaMapScreenState extends State<ArcadiaMapScreen> {
  late final MobileGameSession _gameSession;
  bool _isMenuOpen = false;
  String _statusMessage = 'The journey begins.';

  @override
  void initState() {
    super.initState();
    _gameSession = widget.gameSession ?? MobileGameSession(GameMap());
  }

  @override
  Widget build(BuildContext context) {
    final room = _gameSession.currentRoom;

    return Scaffold(
      appBar: AppBar(title: const Text('Arcadia')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            children: [
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      RoomArtwork(roomId: room.id, label: room.name),
                      const SizedBox(height: 16),
                      Text(
                        room.name,
                        style: Theme.of(context).textTheme.headlineMedium
                            ?.copyWith(fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        room.description,
                        style: Theme.of(context).textTheme.bodyLarge,
                      ),
                      const SizedBox(height: 16),
                      _StatusPanel(message: _statusMessage),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),
              _isMenuOpen ? _buildMenuControls() : _buildDirectionControls(),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildDirectionControls() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Row(
          children: [
            const Expanded(child: SizedBox()),
            Expanded(
              child: _ControlButton(
                label: 'North',
                onPressed: _canMove(RoomDirection.north)
                    ? () => _move(RoomDirection.north)
                    : null,
              ),
            ),
            const Expanded(child: SizedBox()),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: _ControlButton(
                label: 'West',
                onPressed: _canMove(RoomDirection.west)
                    ? () => _move(RoomDirection.west)
                    : null,
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _ControlButton(label: 'Inspect', onPressed: _inspect),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _ControlButton(
                label: 'East',
                onPressed: _canMove(RoomDirection.east)
                    ? () => _move(RoomDirection.east)
                    : null,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            const Expanded(child: SizedBox()),
            Expanded(
              child: _ControlButton(
                label: 'South',
                onPressed: _canMove(RoomDirection.south)
                    ? () => _move(RoomDirection.south)
                    : null,
              ),
            ),
            const Expanded(child: SizedBox()),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            const Expanded(child: SizedBox()),
            Expanded(
              child: _ControlButton(label: 'Menu', onPressed: _openMenu),
            ),
            const Expanded(child: SizedBox()),
          ],
        ),
      ],
    );
  }

  Widget _buildMenuControls() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _ControlButton(label: 'Save', onPressed: _save),
        const SizedBox(height: 8),
        _ControlButton(label: 'Return', onPressed: _closeMenu),
      ],
    );
  }

  bool _canMove(RoomDirection direction) {
    return _gameSession.canMove(direction);
  }

  void _move(RoomDirection direction) {
    final result = _gameSession.move(direction);

    setState(() {
      _statusMessage = result.message;
    });
  }

  void _inspect() {
    setState(() {
      _statusMessage = _gameSession.interact();
    });
  }

  void _openMenu() {
    setState(() {
      _isMenuOpen = true;
    });
  }

  void _closeMenu() {
    setState(() {
      _isMenuOpen = false;
    });
  }

  void _save() {
    setState(() {
      _statusMessage = 'Game saved.';
    });
  }
}

class _StatusPanel extends StatelessWidget {
  const _StatusPanel({required this.message});

  final String message;

  @override
  Widget build(BuildContext context) {
    final backgroundColor = Theme.of(context).brightness == Brightness.dark
        ? const Color(0xff212121)
        : const Color(0xffe1e1e1);

    return DecoratedBox(
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Text(message, style: Theme.of(context).textTheme.bodyMedium),
      ),
    );
  }
}

class _ControlButton extends StatelessWidget {
  const _ControlButton({required this.label, required this.onPressed});

  final String label;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 48,
      child: ElevatedButton(
        onPressed: onPressed,
        child: Text(label, textAlign: TextAlign.center),
      ),
    );
  }
}
