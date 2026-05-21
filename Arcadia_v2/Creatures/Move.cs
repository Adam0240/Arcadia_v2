#nullable enable

namespace Arcadia_v2
{
    public enum MoveType
    {
        Normal,
        Fire,
        Water,
        Grass,
        Bug,
        Ground,
        Flying,
        Fighting,
        Psychic,
        Dark,
        Electric
    }

    // Represents one battle move and its immutable combat values.
    public class Move
    {
        public string Name { get; }
        public MoveType Type { get; }
        public int Power { get; }

        // Compatibility aliases for callers that use move-specific member names.
        public string MoveName => Name;
        public int MovePower => Power;

        public Move(string name, MoveType type, int power)
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
        // Normal Moves
        public static readonly Move Tackle = new Move("TACKLE", MoveType.Normal, 5);
        public static readonly Move QuickAttack = new Move("QUICKATTACK", MoveType.Normal, 5);
        public static readonly Move Growl = new Move("GROWL", MoveType.Normal, 0);
        public static readonly Move Leer = new Move("LEER", MoveType.Normal, 0);

        // Fire Moves
        public static readonly Move Ember = new Move("EMBER", MoveType.Fire, 7);
        public static readonly Move FireFang = new Move("FIREFANG", MoveType.Fire, 8);
        public static readonly Move Flamethrower = new Move("FLAMETHROWER", MoveType.Fire, 10);
        public static readonly Move FireBlitz = new Move("FIREBLITZ", MoveType.Fire, 12);
        public static readonly Move FlameWheel = new Move("FLAMEWHEEL", MoveType.Fire, 8);

        // Water Moves
        public static readonly Move WaterGun = new Move("WATERGUN", MoveType.Water, 6);
        public static readonly Move WaterPulse = new Move("WATERPULSE", MoveType.Water, 7);
        public static readonly Move AquaTail = new Move("AQUATAIL", MoveType.Water, 8);
        public static readonly Move HydroPump = new Move("HYDROPUMP", MoveType.Water, 12);
        public static readonly Move Surf = new Move("SURF", MoveType.Water, 10);
        public static readonly Move Splash = new Move("SPLASH", MoveType.Water, 0);

        // Grass Moves
        public static readonly Move VineWhip = new Move("VINEWHIP", MoveType.Grass, 7);
        public static readonly Move RazerLeaf = new Move("RAZERLEAF", MoveType.Grass, 10);
        public static readonly Move SeedBomb = new Move("SEEDBOMB", MoveType.Grass, 12);
        public static readonly Move PetalBlizzard = new Move("PETALBLIZZARD", MoveType.Grass, 14);
        public static readonly Move Synthesis = new Move("SYNTHESIS", MoveType.Grass, 5);
        public static readonly Move SolarBeam = new Move("SOLARBEAM", MoveType.Grass, 12);

        // Bug Moves
        public static readonly Move StringShot = new Move("STRINGSHOT", MoveType.Bug, 2);
        public static readonly Move BugBuzz = new Move("BUGBUZZ", MoveType.Bug, 2);
        public static readonly Move FuryCutter = new Move("FURYCUTTER", MoveType.Bug, 7);
        public static readonly Move Harden = new Move("HARDEN", MoveType.Bug, 2);

        // Ground Moves
        public static readonly Move RockSmash = new Move("ROCKSMASH", MoveType.Ground, 7);
        public static readonly Move RollOut = new Move("ROLLOUT", MoveType.Ground, 7);
        public static readonly Move Earthquake = new Move("EARTHQUAKE", MoveType.Ground, 12);

        // Flying Moves
        public static readonly Move Peck = new Move("PECK", MoveType.Flying, 5);
        public static readonly Move WingAttack = new Move("WINGATTACK", MoveType.Flying, 7);
        public static readonly Move Gust = new Move("GUST", MoveType.Flying, 7);
        public static readonly Move Hurricane = new Move("HURRICANE", MoveType.Flying, 10);
        public static readonly Move Roost = new Move("ROOST", MoveType.Flying, 5);
        public static readonly Move Bounce = new Move("BOUNCE", MoveType.Flying, 10);

        // Fighting Moves
        public static readonly Move LowSweep = new Move("LOWSWEEP", MoveType.Fighting, 8);
        public static readonly Move BulkUp = new Move("BULKUP", MoveType.Fighting, 5);
        public static readonly Move DynamicPunch = new Move("DYNAMICPUNCH", MoveType.Fighting, 5);
        public static readonly Move KarateChop = new Move("KARATECHOP", MoveType.Fighting, 5);
        public static readonly Move Revenge = new Move("REVENGE", MoveType.Fighting, 5);

        // Psychic Moves
        public static readonly Move Confusion = new Move("CONFUSION", MoveType.Psychic, 7);
        public static readonly Move Psychic = new Move("PSYCHIC", MoveType.Psychic, 8);
        public static readonly Move Moonblast = new Move("MOONBLAST", MoveType.Psychic, 10);
        public static readonly Move Moonlight = new Move("MOONLIGHT", MoveType.Psychic, 10);
        public static readonly Move Sunlight = new Move("SUNLIGHT", MoveType.Psychic, 10);

        // Dark Moves
        public static readonly Move DarkPulse = new Move("DARKPULSE", MoveType.Dark, 10);
        public static readonly Move Bite = new Move("BITE", MoveType.Dark, 8);

        // Electric Moves
        public static readonly Move Spark = new Move("SPARK", MoveType.Electric, 7);
        public static readonly Move Thunderbolt = new Move("THUNDERBOLT", MoveType.Electric, 10);
        public static readonly Move Thunder = new Move("THUNDER", MoveType.Electric, 12);
    }
}
