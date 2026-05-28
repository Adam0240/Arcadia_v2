import 'battle_move.dart';
import 'element_type.dart';
import 'move_effect.dart';

class MoveCatalog {
  const MoveCatalog._();

  static const BattleMove pounce = BattleMove(
    name: 'Pounce',
    type: ElementType.base,
    power: 5,
  );
  static const BattleMove felineReflex = BattleMove(
    name: 'Feline Reflex',
    type: ElementType.base,
    power: 3,
  );

  static const BattleMove loyalRush = BattleMove(
    name: 'Loyal Rush',
    type: ElementType.base,
    power: 5,
  );
  static const BattleMove wildChase = BattleMove(
    name: 'Wild Chase',
    type: ElementType.base,
    power: 3,
  );

  static const BattleMove hoofKick = BattleMove(
    name: 'Hoof Kick',
    type: ElementType.base,
    power: 5,
  );
  static const BattleMove stampede = BattleMove(
    name: 'Stampede',
    type: ElementType.base,
    power: 4,
  );

  static const BattleMove headBash = BattleMove(
    name: 'Head Bash',
    type: ElementType.base,
    power: 0,
  );
  static const BattleMove deepRetreat = BattleMove(
    name: 'Deep Retreat',
    type: ElementType.base,
    power: 0,
  );

  static const BattleMove beakStrike = BattleMove(
    name: 'Beak Strike',
    type: ElementType.base,
    power: 0,
  );
  static const BattleMove quickTalon = BattleMove(
    name: 'Quick Talon',
    type: ElementType.base,
    power: 0,
  );

  static const BattleMove mandibleBite = BattleMove(
    name: 'Mandible Bite',
    type: ElementType.base,
    power: 0,
  );
  static const BattleMove colonyRush = BattleMove(
    name: 'Colony Rush',
    type: ElementType.base,
    power: 0,
  );

  static const BattleMove playSwipe = BattleMove(
    name: 'Play Swipe',
    type: ElementType.base,
    power: 0,
  );
  static const BattleMove tumbleRush = BattleMove(
    name: 'Tumble Rush',
    type: ElementType.base,
    power: 0,
  );

  static const BattleMove venomFang = BattleMove(
    name: 'Venom Fang',
    type: ElementType.base,
    power: 0,
  );
  static const BattleMove shadowFang = BattleMove(
    name: 'Shadow Fang',
    type: ElementType.base,
    power: 0,
  );

  static const BattleMove thornWrap = BattleMove(
    name: 'Thorn Wrap',
    type: ElementType.nature,
    power: 7,
  );
  static const BattleMove verdantSurge = BattleMove(
    name: 'Verdant Surge',
    type: ElementType.nature,
    power: 8,
  );
  static const BattleMove bloom = BattleMove(
    name: 'Bloom',
    type: ElementType.nature,
    power: 10,
    effect: MoveEffect.heal,
  );
  static const BattleMove naturesWrath = BattleMove(
    name: "Nature's Wrath",
    type: ElementType.nature,
    power: 12,
  );

  static const BattleMove currentRush = BattleMove(
    name: 'Current Rush',
    type: ElementType.mystic,
    power: 6,
  );
  static const BattleMove oceanPulse = BattleMove(
    name: 'Ocean Pulse',
    type: ElementType.mystic,
    power: 7,
  );
  static const BattleMove deepseaRupture = BattleMove(
    name: 'Deepsea Rupture',
    type: ElementType.mystic,
    power: 8,
  );
  static const BattleMove tidalBreak = BattleMove(
    name: 'Tidal Break',
    type: ElementType.mystic,
    power: 12,
  );

  static const BattleMove staticClaw = BattleMove(
    name: 'Static Claw',
    type: ElementType.thunder,
    power: 7,
  );
  static const BattleMove voltJab = BattleMove(
    name: 'Volt Jab',
    type: ElementType.thunder,
    power: 10,
  );
  static const BattleMove arcPulse = BattleMove(
    name: 'Arc Pulse',
    type: ElementType.thunder,
    power: 12,
  );
  static const BattleMove thunderRift = BattleMove(
    name: 'Thunder Rift',
    type: ElementType.thunder,
    power: 14,
  );

  static const BattleMove emberBite = BattleMove(
    name: 'Ember Bite',
    type: ElementType.draconic,
    power: 2,
  );
  static const BattleMove infernoRoar = BattleMove(
    name: 'Inferno Roar',
    type: ElementType.draconic,
    power: 2,
  );
  static const BattleMove ragePulse = BattleMove(
    name: 'Rage Pulse',
    type: ElementType.draconic,
    power: 7,
  );
  static const BattleMove dragonFall = BattleMove(
    name: "Dragon's Fall",
    type: ElementType.draconic,
    power: 2,
  );

  static const BattleMove starFlick = BattleMove(
    name: 'Star Flick',
    type: ElementType.cosmic,
    power: 7,
  );
  static const BattleMove lunarPulse = BattleMove(
    name: 'Lunar Pulse',
    type: ElementType.cosmic,
    power: 7,
  );
  static const BattleMove cometStrike = BattleMove(
    name: 'Comet Strike',
    type: ElementType.cosmic,
    power: 12,
  );
  static const BattleMove supernova = BattleMove(
    name: 'Supernova',
    type: ElementType.cosmic,
    power: 12,
  );

  static const BattleMove radBurst = BattleMove(
    name: 'Rad Burst',
    type: ElementType.nuclear,
    power: 5,
  );
  static const BattleMove falloutBite = BattleMove(
    name: 'Fallout Bite',
    type: ElementType.nuclear,
    power: 7,
  );
  static const BattleMove contaminate = BattleMove(
    name: 'Contaminate',
    type: ElementType.nuclear,
    power: 7,
  );
  static const BattleMove coreDetonation = BattleMove(
    name: 'Core Detonation',
    type: ElementType.nuclear,
    power: 10,
  );
}
