#nullable enable

using Arcadia_v2;

namespace Arcadia_v2.Creatures
{
    public enum ElementType
    {
        Base,
        Nature,
        Mystic,
        Thunder,
        Draconic,
        Cosmic,
        Nuclear
    }

    // Represents one battle move and its immutable combat values.
    public class Move
    {
        public string Name { get; }
        public ElementType Type { get; }
        public int Power { get; }

        // Compatibility aliases for callers that use move-specific member names.
        public string MoveName => Name;
        public int MovePower => Power;

        public Move(string name, ElementType type, int power)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Move name cannot be empty.", nameof(name));
            }

            if (power < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(power), "Move power cannot be negative.");
            }

            Name = name;
            Type = type;
            Power = power;
        }
    }

    // Shared predefined moves used by Pokemon factory data and tests.
    public static class MoveData
    {
        // Base Moves

        //Cat
        public static readonly Move POUNCE = new Move("Pounce", ElementType.Base, 5);
        public static readonly Move FELINE_REFLEX = new Move("Feline Reflex", ElementType.Base, 3);

        //Dog
        public static readonly Move LOYAL_RUSH = new Move("Loyal Rush", ElementType.Base, 5);
        public static readonly Move WILD_CHASE = new Move("Wild Chase", ElementType.Base, 3);

        //Horse
        public static readonly Move HOOF_KICK = new Move("Hoof Kick", ElementType.Base, 5);
        public static readonly Move STAMPEDE = new Move("Stampede", ElementType.Base, 4);

        //Turtle
        public static readonly Move HEAD_BASH = new Move("Head Bash", ElementType.Base, 0);
        public static readonly Move DEEP_RETREAT = new Move("Deep Retreat", ElementType.Base, 0);

        //Bird
        public static readonly Move BEAK_STRIKE = new Move("Beak Strike", ElementType.Base, 0);
        public static readonly Move QUICK_TALON = new Move("Quick Talon", ElementType.Base, 0);

        //Ant
        public static readonly Move MANDIBLE_BITE = new Move("Mandible Bite", ElementType.Base, 0);
        public static readonly Move COLONY_RUSH = new Move("LEER", ElementType.Base, 0);

        //Cub
        public static readonly Move PLAY_SWIPE = new Move("Play Swipe", ElementType.Base, 0);
        public static readonly Move TUMBLE_RUSH = new Move("Tumble Rush", ElementType.Base, 0);

        //Serpent
        public static readonly Move VENOM_FANG = new Move("Venom Fang", ElementType.Base, 0);
        public static readonly Move SHADOW_FANG = new Move("Shadow Fang", ElementType.Base, 0);


        // Nature Moves
        public static readonly Move THORNWRAP = new Move("Thorn Wrap", ElementType.Nature, 7);
        public static readonly Move VERDANT_SURGE = new Move("Verdant Surge", ElementType.Nature, 8);
        public static readonly Move BLOOM = new Move("Bloom", ElementType.Nature, 10);
        public static readonly Move NATURES_WRATH = new Move("Nature's Wrath", ElementType.Nature, 12);

        // Mystic Moves
        public static readonly Move CURRENT_RUSH = new Move("Current Rush", ElementType.Mystic, 6);
        public static readonly Move OCEON_PULSE = new Move("Oceon's Pulse", ElementType.Mystic, 7);
        public static readonly Move DEEPSEA_RUPTURE = new Move("Deepsea Rupture", ElementType.Mystic, 8);
        public static readonly Move TIDAL_BREAK = new Move("Tital Break", ElementType.Mystic, 12);

        // Thunder Moves
        public static readonly Move STATIC_CLAW = new Move("Static Claw", ElementType.Thunder, 7);
        public static readonly Move VOLT_JAB = new Move("Volt Jab", ElementType.Thunder, 10);
        public static readonly Move ARC_PULSE = new Move("Arc Pulse", ElementType.Thunder, 12);
        public static readonly Move THUNDER_RIFT = new Move("Thunder Rift", ElementType.Thunder, 14);

        // Draconic Moves
        public static readonly Move EMBER_BITE = new Move("Ember Bite", ElementType.Draconic, 2);
        public static readonly Move INFERNO_ROAR = new Move("Inferno Roar", ElementType.Draconic, 2);
        public static readonly Move RAGE_PULSE = new Move("Rage Pulse", ElementType.Draconic, 7);
        public static readonly Move DRAGON_FALL = new Move("Dragon's Fall", ElementType.Draconic, 2);

        // Cosmic Moves
        public static readonly Move STAR_FLICK = new Move("Star Flick", ElementType.Cosmic, 7);
        public static readonly Move LUNAR_PULSE = new Move("Lunar Pulse", ElementType.Cosmic, 7);
        public static readonly Move COMET_STRIKE = new Move("Comet Strike", ElementType.Cosmic, 12);
        public static readonly Move SUPERNOVA = new Move("Supernova", ElementType.Cosmic, 12);

        // Nuclear Moves
        public static readonly Move RAD_BURST = new Move("Rad Burst", ElementType.Nuclear, 5);
        public static readonly Move FALLOUT_BITE = new Move("Fallout Bite", ElementType.Nuclear, 7);
        public static readonly Move CONTAMINATE = new Move("Contaminate", ElementType.Nuclear, 7);
        public static readonly Move CORE_DETONATION = new Move("Core Detonation", ElementType.Nuclear, 10);
    }
}
