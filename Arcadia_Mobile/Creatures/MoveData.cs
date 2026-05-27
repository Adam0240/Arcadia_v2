namespace Arcadia_Mobile.Creatures;

public static class MoveData
{
    public static readonly Move POUNCE = new("Pounce", ElementType.Base, 5);
    public static readonly Move FELINE_REFLEX = new("Feline Reflex", ElementType.Base, 3);

    public static readonly Move LOYAL_RUSH = new("Loyal Rush", ElementType.Base, 5);
    public static readonly Move WILD_CHASE = new("Wild Chase", ElementType.Base, 3);

    public static readonly Move HOOF_KICK = new("Hoof Kick", ElementType.Base, 5);
    public static readonly Move STAMPEDE = new("Stampede", ElementType.Base, 4);

    public static readonly Move HEAD_BASH = new("Head Bash", ElementType.Base, 0);
    public static readonly Move DEEP_RETREAT = new("Deep Retreat", ElementType.Base, 0);

    public static readonly Move BEAK_STRIKE = new("Beak Strike", ElementType.Base, 0);
    public static readonly Move QUICK_TALON = new("Quick Talon", ElementType.Base, 0);

    public static readonly Move MANDIBLE_BITE = new("Mandible Bite", ElementType.Base, 0);
    public static readonly Move COLONY_RUSH = new("Colony Rush", ElementType.Base, 0);

    public static readonly Move PLAY_SWIPE = new("Play Swipe", ElementType.Base, 0);
    public static readonly Move TUMBLE_RUSH = new("Tumble Rush", ElementType.Base, 0);

    public static readonly Move VENOM_FANG = new("Venom Fang", ElementType.Base, 0);
    public static readonly Move SHADOW_FANG = new("Shadow Fang", ElementType.Base, 0);

    public static readonly Move THORNWRAP = new("Thorn Wrap", ElementType.Nature, 7);
    public static readonly Move VERDANT_SURGE = new("Verdant Surge", ElementType.Nature, 8);
    public static readonly Move BLOOM = new("Bloom", ElementType.Nature, 10, MoveEffect.Heal);
    public static readonly Move NATURES_WRATH = new("Nature's Wrath", ElementType.Nature, 12);

    public static readonly Move CURRENT_RUSH = new("Current Rush", ElementType.Mystic, 6);
    public static readonly Move OCEAN_PULSE = new("Ocean Pulse", ElementType.Mystic, 7);
    public static readonly Move DEEPSEA_RUPTURE = new("Deepsea Rupture", ElementType.Mystic, 8);
    public static readonly Move TIDAL_BREAK = new("Tidal Break", ElementType.Mystic, 12);

    public static readonly Move STATIC_CLAW = new("Static Claw", ElementType.Thunder, 7);
    public static readonly Move VOLT_JAB = new("Volt Jab", ElementType.Thunder, 10);
    public static readonly Move ARC_PULSE = new("Arc Pulse", ElementType.Thunder, 12);
    public static readonly Move THUNDER_RIFT = new("Thunder Rift", ElementType.Thunder, 14);

    public static readonly Move EMBER_BITE = new("Ember Bite", ElementType.Draconic, 2);
    public static readonly Move INFERNO_ROAR = new("Inferno Roar", ElementType.Draconic, 2);
    public static readonly Move RAGE_PULSE = new("Rage Pulse", ElementType.Draconic, 7);
    public static readonly Move DRAGON_FALL = new("Dragon's Fall", ElementType.Draconic, 2);

    public static readonly Move STAR_FLICK = new("Star Flick", ElementType.Cosmic, 7);
    public static readonly Move LUNAR_PULSE = new("Lunar Pulse", ElementType.Cosmic, 7);
    public static readonly Move COMET_STRIKE = new("Comet Strike", ElementType.Cosmic, 12);
    public static readonly Move SUPERNOVA = new("Supernova", ElementType.Cosmic, 12);

    public static readonly Move RAD_BURST = new("Rad Burst", ElementType.Nuclear, 5);
    public static readonly Move FALLOUT_BITE = new("Fallout Bite", ElementType.Nuclear, 7);
    public static readonly Move CONTAMINATE = new("Contaminate", ElementType.Nuclear, 7);
    public static readonly Move CORE_DETONATION = new("Core Detonation", ElementType.Nuclear, 10);
}
