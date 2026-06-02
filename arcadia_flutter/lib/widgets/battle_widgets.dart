import 'package:flutter/material.dart';

import '../creatures/battle_move.dart';

class BattleAction {
  const BattleAction({required this.label, required this.onPressed});

  final String label;
  final VoidCallback? onPressed;
}

class BattleSwitchOption {
  const BattleSwitchOption({required this.index, required this.label});

  final int index;
  final String label;
}

class BattleAnimalPanel extends StatelessWidget {
  const BattleAnimalPanel({
    super.key,
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

class BattleMessage extends StatelessWidget {
  const BattleMessage({super.key, required this.message});

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

class BattleActionList extends StatelessWidget {
  const BattleActionList({
    super.key,
    required this.moves,
    required this.canUseMoves,
    required this.onMove,
    required this.switchOptions,
    required this.onSwitch,
    this.extraActions = const [],
  });

  final Iterable<BattleMove> moves;
  final bool canUseMoves;
  final ValueChanged<BattleMove> onMove;
  final Iterable<BattleSwitchOption> switchOptions;
  final ValueChanged<int> onSwitch;
  final List<BattleAction> extraActions;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        for (final switchOption in switchOptions) ...[
          BattleButton(
            label: switchOption.label,
            onPressed: () => onSwitch(switchOption.index),
          ),
          const SizedBox(height: 8),
        ],
        for (final move in moves) ...[
          BattleButton(
            label: move.name,
            onPressed: canUseMoves ? () => onMove(move) : null,
          ),
          const SizedBox(height: 8),
        ],
        for (final action in extraActions) ...[
          BattleButton(label: action.label, onPressed: action.onPressed),
          const SizedBox(height: 8),
        ],
      ],
    );
  }
}

class BattleButton extends StatelessWidget {
  const BattleButton({super.key, required this.label, required this.onPressed});

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
