class WildBattleResult {
  const WildBattleResult({
    required this.message,
    this.battleEnded = false,
    this.returnToMap = false,
    this.needsPlayerSwitch = false,
  });

  final String message;
  final bool battleEnded;
  final bool returnToMap;
  final bool needsPlayerSwitch;
}
