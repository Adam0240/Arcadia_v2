import 'battle_move.dart';
import 'element_type.dart';
import 'move_effect.dart';

class MoveCatalog {
  const MoveCatalog._();

  static final BattleMove pounce = BattleMove(
    name: 'Pounce',
    type: ElementType.base,
    power: 5,
  );
  static final BattleMove felineReflex = BattleMove(
    name: 'Feline Reflex',
    type: ElementType.base,
    power: 3,
  );

  static final BattleMove loyalRush = BattleMove(
    name: 'Loyal Rush',
    type: ElementType.base,
    power: 5,
  );
  static final BattleMove wildChase = BattleMove(
    name: 'Wild Chase',
    type: ElementType.base,
    power: 3,
  );

  static final BattleMove hoofKick = BattleMove(
    name: 'Hoof Kick',
    type: ElementType.base,
    power: 5,
  );
  static final BattleMove stampede = BattleMove(
    name: 'Stampede',
    type: ElementType.base,
    power: 4,
  );

  static final BattleMove headBash = BattleMove(
    name: 'Head Bash',
    type: ElementType.base,
    power: 0,
  );
  static final BattleMove deepRetreat = BattleMove(
    name: 'Deep Retreat',
    type: ElementType.base,
    power: 0,
  );

  static final BattleMove beakStrike = BattleMove(
    name: 'Beak Strike',
    type: ElementType.base,
    power: 0,
  );
  static final BattleMove quickTalon = BattleMove(
    name: 'Quick Talon',
    type: ElementType.base,
    power: 0,
  );

  static final BattleMove mandibleBite = BattleMove(
    name: 'Mandible Bite',
    type: ElementType.base,
    power: 0,
  );
  static final BattleMove colonyRush = BattleMove(
    name: 'Colony Rush',
    type: ElementType.base,
    power: 0,
  );

  static final BattleMove playSwipe = BattleMove(
    name: 'Play Swipe',
    type: ElementType.base,
    power: 0,
  );
  static final BattleMove tumbleRush = BattleMove(
    name: 'Tumble Rush',
    type: ElementType.base,
    power: 0,
  );

  static final BattleMove venomFang = BattleMove(
    name: 'Venom Fang',
    type: ElementType.base,
    power: 0,
  );
  static final BattleMove shadowFang = BattleMove(
    name: 'Shadow Fang',
    type: ElementType.base,
    power: 0,
  );

  static final BattleMove thornWrap = BattleMove(
    name: 'Thorn Wrap',
    type: ElementType.nature,
    power: 7,
  );
  static final BattleMove verdantSurge = BattleMove(
    name: 'Verdant Surge',
    type: ElementType.nature,
    power: 8,
  );
  static final BattleMove bloom = BattleMove(
    name: 'Bloom',
    type: ElementType.nature,
    power: 10,
    effect: MoveEffect.heal,
  );
  static final BattleMove naturesWrath = BattleMove(
    name: "Nature's Wrath",
    type: ElementType.nature,
    power: 12,
  );

  static final BattleMove currentRush = BattleMove(
    name: 'Current Rush',
    type: ElementType.mystic,
    power: 6,
  );
  static final BattleMove oceanPulse = BattleMove(
    name: 'Ocean Pulse',
    type: ElementType.mystic,
    power: 7,
  );
  static final BattleMove deepseaRupture = BattleMove(
    name: 'Deepsea Rupture',
    type: ElementType.mystic,
    power: 8,
  );
  static final BattleMove tidalBreak = BattleMove(
    name: 'Tidal Break',
    type: ElementType.mystic,
    power: 12,
  );

  static final BattleMove staticClaw = BattleMove(
    name: 'Static Claw',
    type: ElementType.thunder,
    power: 7,
  );
  static final BattleMove voltJab = BattleMove(
    name: 'Volt Jab',
    type: ElementType.thunder,
    power: 10,
  );
  static final BattleMove arcPulse = BattleMove(
    name: 'Arc Pulse',
    type: ElementType.thunder,
    power: 12,
  );
  static final BattleMove thunderRift = BattleMove(
    name: 'Thunder Rift',
    type: ElementType.thunder,
    power: 14,
  );

  static final BattleMove emberBite = BattleMove(
    name: 'Ember Bite',
    type: ElementType.draconic,
    power: 2,
  );
  static final BattleMove infernoRoar = BattleMove(
    name: 'Inferno Roar',
    type: ElementType.draconic,
    power: 2,
  );
  static final BattleMove ragePulse = BattleMove(
    name: 'Rage Pulse',
    type: ElementType.draconic,
    power: 7,
  );
  static final BattleMove dragonFall = BattleMove(
    name: "Dragon's Fall",
    type: ElementType.draconic,
    power: 2,
  );

  static final BattleMove starFlick = BattleMove(
    name: 'Star Flick',
    type: ElementType.cosmic,
    power: 7,
  );
  static final BattleMove lunarPulse = BattleMove(
    name: 'Lunar Pulse',
    type: ElementType.cosmic,
    power: 7,
  );
  static final BattleMove cometStrike = BattleMove(
    name: 'Comet Strike',
    type: ElementType.cosmic,
    power: 12,
  );
  static final BattleMove supernova = BattleMove(
    name: 'Supernova',
    type: ElementType.cosmic,
    power: 12,
  );

  static final BattleMove radBurst = BattleMove(
    name: 'Rad Burst',
    type: ElementType.nuclear,
    power: 5,
  );
  static final BattleMove falloutBite = BattleMove(
    name: 'Fallout Bite',
    type: ElementType.nuclear,
    power: 7,
  );
  static final BattleMove contaminate = BattleMove(
    name: 'Contaminate',
    type: ElementType.nuclear,
    power: 7,
  );
  static final BattleMove coreDetonation = BattleMove(
    name: 'Core Detonation',
    type: ElementType.nuclear,
    power: 10,
  );
}
