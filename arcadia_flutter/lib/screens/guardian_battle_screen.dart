import 'package:flutter/material.dart';

import '../battles/guardian_battle_result.dart';
import '../battles/guardian_battle_state.dart';
import '../creatures/battle_move.dart';
import '../services/mobile_game_session.dart';
import '../widgets/battle_widgets.dart';

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
  bool _needsPlayerSwitch = false;

  @override
  void initState() {
    super.initState();
    _battleMessage = widget.gameSession.getGuardianIntro(widget.battleState);
  }

  @override
  Widget build(BuildContext context) {
    final playerAnimal = widget.battleState.playerAnimal;
    final guardianAnimal = widget.battleState.guardianAnimal;
    final canUseMove = !widget.battleState.isComplete && !_needsPlayerSwitch;

    return Scaffold(
      appBar: AppBar(
        title: Text(
          widget.gameSession.getGuardianBattleTitle(widget.battleState),
        ),
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              BattleAnimalPanel(
                label: 'Your Animal',
                animalName: playerAnimal.name,
                health: playerAnimal.health,
                baseHealth: playerAnimal.baseHealth,
              ),
              const SizedBox(height: 12),
              BattleAnimalPanel(
                label: widget.battleState.guardian.name,
                animalName: guardianAnimal.name,
                health: guardianAnimal.health,
                baseHealth: guardianAnimal.baseHealth,
              ),
              const SizedBox(height: 16),
              BattleMessage(message: _battleMessage),
              const SizedBox(height: 16),
              Expanded(
                child: SingleChildScrollView(
                  child: BattleActionList(
                    moves: playerAnimal.moves,
                    canUseMoves: canUseMove,
                    onMove: _useMove,
                    switchOptions: _buildSwitchOptions(),
                    onSwitch: _switchAnimal,
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

  void _switchAnimal(int animalIndex) {
    final result = widget.gameSession.switchGuardianBattleAnimal(
      widget.battleState,
      animalIndex,
    );
    _applyResult(result);
  }

  List<BattleSwitchOption> _buildSwitchOptions() {
    if (!_needsPlayerSwitch) {
      return const [];
    }

    return [
      for (final animalIndex in widget.battleState.healthyPlayerSwitchIndexes)
        BattleSwitchOption(
          index: animalIndex,
          label:
              'Send ${widget.battleState.player.animalInventory[animalIndex].name}',
        ),
    ];
  }

  void _applyResult(GuardianBattleResult result) {
    if (result.returnToMap) {
      Navigator.of(context).pop(result.message);
      return;
    }

    setState(() {
      _battleMessage = result.message;
      _needsPlayerSwitch = result.needsPlayerSwitch;
    });
  }
}
