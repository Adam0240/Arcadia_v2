import 'package:flutter/material.dart';

import '../map/room_direction.dart';

class MapDirectionControls extends StatelessWidget {
  const MapDirectionControls({
    super.key,
    required this.canMove,
    required this.onMove,
    required this.onInspect,
    required this.onOpenMenu,
    required this.showEncounter,
    required this.onEncounter,
    required this.guardianActionLabel,
    required this.onGuardian,
    required this.showEnding,
    required this.onEnding,
  });

  final bool Function(RoomDirection direction) canMove;
  final ValueChanged<RoomDirection> onMove;
  final VoidCallback onInspect;
  final VoidCallback onOpenMenu;
  final bool showEncounter;
  final VoidCallback onEncounter;
  final String? guardianActionLabel;
  final VoidCallback onGuardian;
  final bool showEnding;
  final VoidCallback onEnding;

  @override
  Widget build(BuildContext context) {
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
              child: MapControlButton(label: 'Inspect', onPressed: onInspect),
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
        _CenteredControlButton(label: 'Menu', onPressed: onOpenMenu),
        if (showEncounter) ...[
          const SizedBox(height: 8),
          _CenteredControlButton(label: 'Encounter', onPressed: onEncounter),
        ],
        if (guardianActionLabel != null) ...[
          const SizedBox(height: 8),
          _CenteredControlButton(
            label: guardianActionLabel!,
            onPressed: onGuardian,
          ),
        ],
        if (showEnding) ...[
          const SizedBox(height: 8),
          _CenteredControlButton(label: 'Ending', onPressed: onEnding),
        ],
      ],
    );
  }

  Widget _buildDirectionButton(RoomDirection direction) {
    return MapControlButton(
      label: direction.label,
      onPressed: canMove(direction) ? () => onMove(direction) : null,
    );
  }
}

class MapMenuControls extends StatelessWidget {
  const MapMenuControls({
    super.key,
    required this.canReorderParty,
    required this.onInventory,
    required this.onHealAnimals,
    required this.onReorderParty,
    required this.canSwap,
    required this.onSwap,
    required this.canGrow,
    required this.onGrow,
    required this.onBond,
    required this.onStarFragments,
    required this.onSave,
    required this.onReturn,
  });

  final bool canReorderParty;
  final VoidCallback onInventory;
  final VoidCallback onHealAnimals;
  final VoidCallback onReorderParty;
  final bool canSwap;
  final VoidCallback onSwap;
  final bool canGrow;
  final VoidCallback onGrow;
  final VoidCallback onBond;
  final VoidCallback onStarFragments;
  final VoidCallback onSave;
  final VoidCallback onReturn;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        MapControlButton(label: 'Inventory', onPressed: onInventory),
        const SizedBox(height: 8),
        MapControlButton(label: 'Heal Animals', onPressed: onHealAnimals),
        const SizedBox(height: 8),
        if (canReorderParty) ...[
          MapControlButton(label: 'Reorder Party', onPressed: onReorderParty),
          const SizedBox(height: 8),
        ],
        if (canSwap) ...[
          MapControlButton(label: 'Swap', onPressed: onSwap),
          const SizedBox(height: 8),
        ],
        if (canGrow) ...[
          MapControlButton(label: 'Grow', onPressed: onGrow),
          const SizedBox(height: 8),
        ],
        MapControlButton(label: 'Bond', onPressed: onBond),
        const SizedBox(height: 8),
        MapControlButton(label: 'Star Fragments', onPressed: onStarFragments),
        const SizedBox(height: 8),
        MapControlButton(label: 'Save', onPressed: onSave),
        const SizedBox(height: 8),
        MapControlButton(label: 'Return', onPressed: onReturn),
      ],
    );
  }
}

class _CenteredControlButton extends StatelessWidget {
  const _CenteredControlButton({required this.label, required this.onPressed});

  final String label;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        const Expanded(child: SizedBox()),
        Expanded(
          child: MapControlButton(label: label, onPressed: onPressed),
        ),
        const Expanded(child: SizedBox()),
      ],
    );
  }
}

class MapControlButton extends StatelessWidget {
  const MapControlButton({
    super.key,
    required this.label,
    required this.onPressed,
  });

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
