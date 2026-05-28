import 'package:flutter/material.dart';

import '../battles/guardian_battle_result.dart';
import '../battles/guardian_battle_state.dart';
import '../creatures/battle_move.dart';
import '../services/mobile_game_session.dart';

class GuardianBattleScreen extends StatefulWidget {
  const GuardianBattleScreen({
    super.key,
    required this.gameSession,
    required this.battleState,
  });

  final MobileGameSession gameSession;
  final GuardianBattleState battleState;

  @override
  State<GuardianBattleScreen> createState() => _GuardianBattleScreenState();
}

class _GuardianBattleScreenState extends State<GuardianBattleScreen> {
  late String _battleMessage;

  @override
  void initState() {
    super.initState();
    _battleMessage = widget.gameSession.getGuardianIntro(widget.battleState);
  }

  @override
  Widget build(BuildContext context) {
    final playerAnimal = widget.battleState.playerAnimal;
    final guardianAnimal = widget.battleState.guardianAnimal;

    return Scaffold(
      appBar: AppBar(title: const Text('Guardian Battle')),
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
                label: widget.battleState.guardian.name,
                animalName: guardianAnimal.name,
                health: guardianAnimal.health,
                baseHealth: guardianAnimal.baseHealth,
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
    final result = widget.gameSession.useGuardianBattleMove(
      widget.battleState,
      move,
    );
    _applyResult(result);
  }

  void _applyResult(GuardianBattleResult result) {
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
