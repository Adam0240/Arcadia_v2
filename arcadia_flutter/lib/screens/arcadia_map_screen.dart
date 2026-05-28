import 'package:flutter/material.dart';

import '../map/game_map.dart';
import '../map/room_direction.dart';
import '../services/mobile_game_session.dart';
import '../widgets/room_artwork.dart';
import 'guardian_battle_screen.dart';
import 'swap_animal_screen.dart';
import 'wild_battle_screen.dart';

class ArcadiaMapScreen extends StatefulWidget {
  const ArcadiaMapScreen({super.key, this.gameSession});

  final MobileGameSession? gameSession;

  @override
  State<ArcadiaMapScreen> createState() => _ArcadiaMapScreenState();
}

class _ArcadiaMapScreenState extends State<ArcadiaMapScreen> {
  late MobileGameSession _gameSession;
  bool _isMenuOpen = false;
  String _statusMessage = 'The journey begins.';

  @override
  void initState() {
    super.initState();
    _gameSession = widget.gameSession ?? MobileGameSession(GameMap());
  }

  @override
  void didUpdateWidget(covariant ArcadiaMapScreen oldWidget) {
    super.didUpdateWidget(oldWidget);

    if (widget.gameSession == oldWidget.gameSession) {
      return;
    }

    _gameSession = widget.gameSession ?? MobileGameSession(GameMap());
    _isMenuOpen = false;
    _statusMessage = 'The journey begins.';
  }

  @override
  Widget build(BuildContext context) {
    final room = _gameSession.currentRoom;

    return Scaffold(
      appBar: AppBar(title: const Text('Arcadia')),
      body: SafeArea(
        child: LayoutBuilder(
          builder: (context, constraints) {
            final maxControlsHeight = (constraints.maxHeight * 0.75).clamp(
              0.0,
              360.0,
            );

            return Padding(
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
                  ConstrainedBox(
                    constraints: BoxConstraints(maxHeight: maxControlsHeight),
                    child: SingleChildScrollView(
                      child: _isMenuOpen
                          ? _buildMenuControls()
                          : _buildDirectionControls(),
                    ),
                  ),
                ],
              ),
            );
          },
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
            Expanded(child: _buildDirectionButton(RoomDirection.north)),
            const Expanded(child: SizedBox()),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(child: _buildDirectionButton(RoomDirection.west)),
            const SizedBox(width: 8),
            Expanded(
              child: _ControlButton(label: 'Inspect', onPressed: _inspect),
            ),
            const SizedBox(width: 8),
            Expanded(child: _buildDirectionButton(RoomDirection.east)),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            const Expanded(child: SizedBox()),
            Expanded(child: _buildDirectionButton(RoomDirection.south)),
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
        if (_gameSession.hasWildEncounter) ...[
          const SizedBox(height: 8),
          Row(
            children: [
              const Expanded(child: SizedBox()),
              Expanded(
                child: _ControlButton(
                  label: 'Encounter',
                  onPressed: _startWildBattle,
                ),
              ),
              const Expanded(child: SizedBox()),
            ],
          ),
        ],
        if (_gameSession.hasGuardianInCurrentRoom) ...[
          const SizedBox(height: 8),
          Row(
            children: [
              const Expanded(child: SizedBox()),
              Expanded(
                child: _ControlButton(
                  label: 'Guardian',
                  onPressed: _startGuardianBattle,
                ),
              ),
              const Expanded(child: SizedBox()),
            ],
          ),
        ],
      ],
    );
  }

  Widget _buildDirectionButton(RoomDirection direction) {
    return _ControlButton(
      label: direction.label,
      onPressed: _canMove(direction) ? () => _move(direction) : null,
    );
  }

  Widget _buildMenuControls() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _ControlButton(label: 'Inventory', onPressed: _showInventory),
        const SizedBox(height: 8),
        if (_gameSession.canSwapStoredAnimals) ...[
          _ControlButton(label: 'Swap', onPressed: _openSwap),
          const SizedBox(height: 8),
        ],
        _ControlButton(label: 'Bond', onPressed: _showBond),
        const SizedBox(height: 8),
        _ControlButton(label: 'Star Fragments', onPressed: _showStarFragments),
        const SizedBox(height: 8),
        _ControlButton(
          label: 'Save',
          onPressed: () {
            _save();
          },
        ),
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

  Future<void> _startWildBattle() async {
    try {
      final battleState = _gameSession.startWildBattle();
      final resultMessage = await Navigator.of(context).push<String>(
        MaterialPageRoute(
          builder: (_) => WildBattleScreen(
            gameSession: _gameSession,
            battleState: battleState,
          ),
        ),
      );

      if (!mounted || resultMessage == null) {
        return;
      }

      setState(() {
        _statusMessage = resultMessage;
      });
    } on Object catch (error) {
      setState(() {
        _statusMessage = error.toString();
      });
    }
  }

  Future<void> _startGuardianBattle() async {
    final unavailableMessage = _gameSession.getGuardianUnavailableMessage();

    if (unavailableMessage != null) {
      setState(() {
        _statusMessage = unavailableMessage;
      });
      return;
    }

    try {
      final battleState = _gameSession.startGuardianBattle();
      final resultMessage = await Navigator.of(context).push<String>(
        MaterialPageRoute(
          builder: (_) => GuardianBattleScreen(
            gameSession: _gameSession,
            battleState: battleState,
          ),
        ),
      );

      if (!mounted || resultMessage == null) {
        return;
      }

      setState(() {
        _statusMessage = resultMessage;
      });
    } on Object catch (error) {
      setState(() {
        _statusMessage = error.toString();
      });
    }
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

  void _showInventory() {
    setState(() {
      _statusMessage = _gameSession.player.getAnimalInventoryDisplay();
    });
  }

  Future<void> _openSwap() async {
    final resultMessage = await Navigator.of(context).push<String>(
      MaterialPageRoute(
        builder: (_) => SwapAnimalScreen(gameSession: _gameSession),
      ),
    );

    if (!mounted || resultMessage == null) {
      return;
    }

    setState(() {
      _statusMessage = resultMessage;
    });
  }

  void _showBond() {
    setState(() {
      _statusMessage = _gameSession.player.getBondDisplay();
    });
  }

  void _showStarFragments() {
    setState(() {
      _statusMessage = _gameSession.player.getStarFragmentDisplay();
    });
  }

  Future<void> _save() async {
    try {
      await _gameSession.saveGame();
    } on Object catch (error) {
      if (!mounted) {
        return;
      }

      setState(() {
        _statusMessage = 'Save failed: $error';
      });

      return;
    }

    if (!mounted) {
      return;
    }

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
