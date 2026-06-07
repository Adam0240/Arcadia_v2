import 'package:flutter/material.dart';

import '../map/game_map.dart';
import '../map/room_direction.dart';
import '../services/mobile_game_session.dart';
import '../widgets/map_controls.dart';
import '../widgets/room_artwork.dart';
import 'grow_animal_screen.dart';
import 'guardian_battle_screen.dart';
import 'reorder_party_screen.dart';
import 'swap_animal_screen.dart';
import 'start_menu_screen.dart';
import 'wild_battle_screen.dart';
import 'world_screen.dart';

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
                          ? MapMenuControls(
                              canReorderParty:
                                  _gameSession.player.animalInventory.length >=
                                  2,
                              onInventory: _showInventory,
                              onHealAnimals: _healAnimals,
                              onReorderParty: _openReorderParty,
                              canSwap: _gameSession.canSwapStoredAnimals,
                              onSwap: _openSwap,
                              canGrow: _gameSession.hasGrowthOptions,
                              onGrow: _openGrow,
                              onBond: _showBond,
                              onStarFragments: _showStarFragments,
                              onSave: _save,
                              onReturn: _closeMenu,
                            )
                          : MapDirectionControls(
                              canMove: _gameSession.canMove,
                              onMove: _move,
                              onInspect: _inspect,
                              onExplore: _openWorld,
                              onOpenMenu: _openMenu,
                              showEncounter: _gameSession.hasWildEncounter,
                              onEncounter: _startWildBattle,
                              guardianActionLabel:
                                  _gameSession.hasGuardianInCurrentRoom
                                  ? _gameSession
                                        .getCurrentChallengeActionLabel()
                                  : null,
                              onGuardian: _startGuardianBattle,
                              showEnding:
                                  _gameSession.currentRoom.isFinalRoom &&
                                  !_gameSession.hasWildEncounter,
                              onEnding: _showEndingChoice,
                            ),
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

  void _move(RoomDirection direction) {
    final result = _gameSession.move(direction);

    _setStatus(result.message);
  }

  void _inspect() {
    _setStatus(_gameSession.getFinalRoomMessage() ?? _gameSession.interact());
  }

  Future<void> _openWorld() async {
    await Navigator.of(context).push<void>(
      MaterialPageRoute(builder: (_) => WorldScreen(gameSession: _gameSession)),
    );
  }

  Future<void> _startWildBattle() async {
    try {
      final battleState = _gameSession.startWildBattle();
      await _pushStatusScreen(
        () => WildBattleScreen(
          gameSession: _gameSession,
          battleState: battleState,
        ),
      );
    } on Object catch (error) {
      _setStatusIfMounted(error.toString());
    }
  }

  Future<void> _startGuardianBattle() async {
    final unavailableMessage = _gameSession.getGuardianUnavailableMessage();

    if (unavailableMessage != null) {
      _setStatus(unavailableMessage);
      return;
    }

    try {
      final battleState = _gameSession.startGuardianBattle();
      await _pushStatusScreen(
        () => GuardianBattleScreen(
          gameSession: _gameSession,
          battleState: battleState,
        ),
      );
    } on Object catch (error) {
      _setStatusIfMounted(error.toString());
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
    _setStatus(_gameSession.player.getAnimalInventoryDisplay());
  }

  void _healAnimals() {
    _setStatus(_gameSession.healParty());
  }

  Future<void> _openGrow() async {
    await _pushStatusScreen(() => GrowAnimalScreen(gameSession: _gameSession));
  }

  Future<void> _openSwap() async {
    await _pushStatusScreen(() => SwapAnimalScreen(gameSession: _gameSession));
  }

  Future<void> _openReorderParty() async {
    await _pushStatusScreen(
      () => ReorderPartyScreen(gameSession: _gameSession),
    );
  }

  void _showBond() {
    _setStatus(_gameSession.player.getBondDisplay());
  }

  void _showStarFragments() {
    _setStatus(_gameSession.player.getStarFragmentDisplay());
  }

  Future<void> _showEndingChoice() async {
    try {
      await _gameSession.saveGame();
    } on Object catch (error) {
      if (!mounted) {
        return;
      }

      _setStatus('Save failed: $error');
      return;
    }

    if (!mounted) {
      return;
    }

    final shouldStay = await showDialog<bool>(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text('The End'),
          content: const Text('Do you wish to stay in this world?'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(false),
              child: const Text('No'),
            ),
            FilledButton(
              onPressed: () => Navigator.of(context).pop(true),
              child: const Text('Yes'),
            ),
          ],
        );
      },
    );

    if (!mounted || shouldStay == null) {
      return;
    }

    if (shouldStay) {
      _setStatus('You are welcome to stay in Arcadia.');
      return;
    }

    Navigator.of(context).pushReplacement(
      MaterialPageRoute<void>(
        builder: (_) =>
            StartMenuScreen(saveRepository: _gameSession.saveRepository),
      ),
    );
  }

  Future<void> _save() async {
    try {
      await _gameSession.saveGame();
    } on Object catch (error) {
      if (!mounted) {
        return;
      }

      _setStatus('Save failed: $error');

      return;
    }

    if (!mounted) {
      return;
    }

    _setStatus('Game saved.');
  }

  Future<void> _pushStatusScreen(Widget Function() buildScreen) async {
    final resultMessage = await Navigator.of(
      context,
    ).push<String>(MaterialPageRoute(builder: (_) => buildScreen()));

    if (!mounted || resultMessage == null) {
      return;
    }

    _setStatus(resultMessage);
  }

  void _setStatusIfMounted(String message) {
    if (!mounted) {
      return;
    }

    _setStatus(message);
  }

  void _setStatus(String message) {
    setState(() {
      _statusMessage = message;
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
