import 'package:flutter/material.dart';

import '../battles/wild_battle_result.dart';
import '../battles/wild_battle_state.dart';
import '../creatures/battle_move.dart';
import '../services/mobile_game_session.dart';
import '../widgets/battle_widgets.dart';

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
  bool _needsPlayerSwitch = false;

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
    final canUseMove = !widget.battleState.isComplete && !_needsPlayerSwitch;

    return Scaffold(
      appBar: AppBar(title: const Text('Wild Battle')),
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
                label: 'Wild Animal',
                animalName: wildAnimal.name,
                health: wildAnimal.health,
                baseHealth: wildAnimal.baseHealth,
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
                    extraActions: [
                      BattleAction(
                        label: 'Catch',
                        onPressed: canResolveDefeatedWildAnimal
                            ? _catchWildAnimal
                            : null,
                      ),
                      BattleAction(
                        label: 'Leave',
                        onPressed: canResolveDefeatedWildAnimal
                            ? _leaveWildAnimal
                            : null,
                      ),
                      BattleAction(
                        label: 'Run',
                        onPressed: _needsPlayerSwitch ? null : _run,
                      ),
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

  void _switchAnimal(int animalIndex) {
    final result = widget.gameSession.switchWildBattleAnimal(
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

  void _applyResult(WildBattleResult result) {
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
