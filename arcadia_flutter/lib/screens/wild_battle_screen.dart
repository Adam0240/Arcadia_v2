import 'package:flutter/material.dart';

import '../battles/wild_battle_result.dart';
import '../battles/wild_battle_state.dart';
import '../creatures/battle_move.dart';
import '../services/mobile_game_session.dart';

class WildBattleScreen extends StatefulWidget {
  const WildBattleScreen({
    super.key,
    required this.gameSession,
    required this.battleState,
  });

  final MobileGameSession gameSession;
  final WildBattleState battleState;

  @override
  State<WildBattleScreen> createState() => _WildBattleScreenState();
}

class _WildBattleScreenState extends State<WildBattleScreen> {
  late String _battleMessage;

  @override
  void initState() {
    super.initState();
    _battleMessage = 'A wild ${widget.battleState.wildAnimal.name} attacked!';
  }

  @override
  Widget build(BuildContext context) {
    final playerAnimal = widget.battleState.playerAnimal;
    final wildAnimal = widget.battleState.wildAnimal;
    final canResolveDefeatedWildAnimal = widget.battleState.isWildDefeated;

    return Scaffold(
      appBar: AppBar(title: const Text('Wild Battle')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              _AnimalBattlePanel(
                label: 'Your Animal',
                animalName: playerAnimal.name,
                health: playerAnimal.health,
                baseHealth: playerAnimal.baseHealth,
              ),
              const SizedBox(height: 12),
              _AnimalBattlePanel(
                label: 'Wild Animal',
                animalName: wildAnimal.name,
                health: wildAnimal.health,
                baseHealth: wildAnimal.baseHealth,
              ),
              const SizedBox(height: 16),
              _BattleMessage(message: _battleMessage),
              const SizedBox(height: 16),
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      for (final move in playerAnimal.moves) ...[
                        _BattleButton(
                          label: move.name,
                          onPressed: widget.battleState.isComplete
                              ? null
                              : () => _useMove(move),
                        ),
                        const SizedBox(height: 8),
                      ],
                      _BattleButton(
                        label: 'Catch',
                        onPressed: canResolveDefeatedWildAnimal
                            ? _catchWildAnimal
                            : null,
                      ),
                      const SizedBox(height: 8),
                      _BattleButton(
                        label: 'Leave',
                        onPressed: canResolveDefeatedWildAnimal
                            ? _leaveWildAnimal
                            : null,
                      ),
                      const SizedBox(height: 8),
                      _BattleButton(label: 'Run', onPressed: _run),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _useMove(BattleMove move) {
    final result = widget.gameSession.useWildBattleMove(
      widget.battleState,
      move,
    );
    _applyResult(result);
  }

  void _catchWildAnimal() {
    final result = widget.gameSession.catchWildAnimal(widget.battleState);
    _applyResult(result);
  }

  void _leaveWildAnimal() {
    final result = widget.gameSession.leaveWildAnimal(widget.battleState);
    _applyResult(result);
  }

  void _run() {
    final result = widget.gameSession.runFromWildBattle(widget.battleState);
    _applyResult(result);
  }

  void _applyResult(WildBattleResult result) {
    if (result.returnToMap) {
      Navigator.of(context).pop(result.message);
      return;
    }

    setState(() {
      _battleMessage = result.message;
    });
  }
}

class _AnimalBattlePanel extends StatelessWidget {
  const _AnimalBattlePanel({
    required this.label,
    required this.animalName,
    required this.health,
    required this.baseHealth,
  });

  final String label;
  final String animalName;
  final int health;
  final int baseHealth;

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: BoxDecoration(
        border: Border.all(color: Theme.of(context).colorScheme.outline),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: Theme.of(context).textTheme.labelLarge),
            const SizedBox(height: 4),
            Text(animalName, style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 4),
            Text('Health: $health/$baseHealth'),
          ],
        ),
      ),
    );
  }
}

class _BattleMessage extends StatelessWidget {
  const _BattleMessage({required this.message});

  final String message;

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(8),
      ),
      child: Padding(padding: const EdgeInsets.all(12), child: Text(message)),
    );
  }
}

class _BattleButton extends StatelessWidget {
  const _BattleButton({required this.label, required this.onPressed});

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
