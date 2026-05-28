enum BattleMoveResultType { damage, healing, noEffect }

class BattleMoveResult {
  const BattleMoveResult({
    required this.type,
    required this.moveName,
    required this.amount,
    required this.targetHealth,
  });

  final BattleMoveResultType type;
  final String moveName;
  final int amount;
  final int targetHealth;
}
