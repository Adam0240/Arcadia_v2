#nullable enable

namespace Arcadia_v2
{
    public enum MoveType
    {
        Neutral,
        Nature,
        Mystic,
        Thunder,
        Draconic,
        Cosmic,
        Nuclear
    }

    public enum MoveEffect
    {
        Damage,
        Healing
    }

    // Represents one battle move and its immutable combat values.
    public class Move
    {
        public string Name { get; }
        public MoveType Type { get; }
        public int Power { get; }
        public MoveEffect Effect { get; }

        // Compatibility aliases for callers that use move-specific member names.
        public string MoveName => Name;
        public int MovePower => Power;

        public Move(string name, MoveType type, int power, MoveEffect effect = MoveEffect.Damage)
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
            Effect = effect;
        }
    }

    public sealed record MoveTemplate(
        string Name,
        int Power,
        MoveEffect Effect = MoveEffect.Damage);

    // Shared predefined moves used by Pokemon factory data and tests.
    public static class MoveData
    {
        // Shared move templates
        public static readonly MoveTemplate Fury = new("Fury", 10);
        public static readonly MoveTemplate Storm = new("Storm", 10);
        public static readonly MoveTemplate SpeedAttack = new("Speed Attack", 5);
        public static readonly MoveTemplate DefensiveMove = new("Defensive Move", 1);
        public static readonly MoveTemplate Bomb = new("Bomb", 12);
        public static readonly MoveTemplate Roar = new("Roar", 8);
        public static readonly MoveTemplate Howl = new("Howl", 8);

        // Lion Moves
        public static readonly Move Tackle = new Move("TACKLE", MoveType.Neutral, 5);
        public static readonly Move QuickAttack = new Move("QUICKATTACK", MoveType.Neutral, 5);
        public static readonly Move Growl = new Move("GROWL", MoveType.Neutral, 0);
        public static readonly Move Leer = new Move("LEER", MoveType.Neutral, 0);

        // Dog Moves
        public static readonly Move Ember = new Move("EMBER", MoveType.Draconic, 7);
        public static readonly Move FireFang = new Move("FIREFANG", MoveType.Draconic, 8);
        public static readonly Move Flamethrower = new Move("FLAMETHROWER", MoveType.Draconic, 10);
        public static readonly Move FireBlitz = new Move("FIREBLITZ", MoveType.Draconic, 12);
        public static readonly Move FlameWheel = new Move("FLAMEWHEEL", MoveType.Draconic, 8);

        // Wolf Moves
        public static readonly Move WaterGun = new Move("WATERGUN", MoveType.Mystic, 6);
        public static readonly Move WaterPulse = new Move("WATERPULSE", MoveType.Mystic, 7);
        public static readonly Move AquaTail = new Move("AQUATAIL", MoveType.Mystic, 8);
        public static readonly Move HydroPump = new Move("HYDROPUMP", MoveType.Mystic, 12);
        public static readonly Move Surf = new Move("SURF", MoveType.Mystic, 10);
        public static readonly Move Splash = new Move("SPLASH", MoveType.Mystic, 0);

        // Horse Moves
        public static readonly Move VineWhip = new Move("VINEWHIP", MoveType.Nature, 7);
        public static readonly Move RazerLeaf = new Move("RAZERLEAF", MoveType.Nature, 10);
        public static readonly Move SeedBomb = new Move("SEEDBOMB", MoveType.Nature, 12);
        public static readonly Move PetalBlizzard = new Move("PETALBLIZZARD", MoveType.Nature, 14);
        public static readonly Move Synthesis = new Move("SYNTHESIS", MoveType.Nature, 5, MoveEffect.Healing);
        public static readonly Move SolarBeam = new Move("SOLARBEAM", MoveType.Nature, 12);

        // Stallion Moves
        public static readonly Move StringShot = new Move("STRINGSHOT", MoveType.Nature, 2);
        public static readonly Move BugBuzz = new Move("BUGBUZZ", MoveType.Nature, 2);
        public static readonly Move FuryCutter = new Move("FURYCUTTER", MoveType.Nature, 7);
        public static readonly Move Harden = new Move("HARDEN", MoveType.Nature, 2);

        // Turtle Moves
        public static readonly Move RockSmash = new Move("ROCKSMASH", MoveType.Nuclear, 7);
        public static readonly Move RollOut = new Move("ROLLOUT", MoveType.Nuclear, 7);
        public static readonly Move Earthquake = new Move("EARTHQUAKE", MoveType.Nuclear, 12);

        // Tortoise Moves
        public static readonly Move Peck = new Move("PECK", MoveType.Neutral, 5);
        public static readonly Move WingAttack = new Move("WINGATTACK", MoveType.Neutral, 7);
        public static readonly Move Gust = new Move("GUST", MoveType.Neutral, 7);
        public static readonly Move Hurricane = new Move("HURRICANE", MoveType.Neutral, 10);
        public static readonly Move Roost = new Move("ROOST", MoveType.Neutral, 5, MoveEffect.Healing);
        public static readonly Move Bounce = new Move("BOUNCE", MoveType.Neutral, 10);

        // Bird Moves
        public static readonly Move LowSweep = new Move("LOWSWEEP", MoveType.Neutral, 8);
        public static readonly Move BulkUp = new Move("BULKUP", MoveType.Neutral, 5);
        public static readonly Move DynamicPunch = new Move("DYNAMICPUNCH", MoveType.Neutral, 5);
        public static readonly Move KarateChop = new Move("KARATECHOP", MoveType.Neutral, 5);
        public static readonly Move Revenge = new Move("REVENGE", MoveType.Neutral, 5);

        // Eagle Moves
        public static readonly Move Confusion = new Move("CONFUSION", MoveType.Cosmic, 7);
        public static readonly Move Psychic = new Move("PSYCHIC", MoveType.Cosmic, 8);
        public static readonly Move Moonblast = new Move("MOONBLAST", MoveType.Cosmic, 10);
        public static readonly Move Moonlight = new Move("MOONLIGHT", MoveType.Cosmic, 10, MoveEffect.Healing);
        public static readonly Move Sunlight = new Move("SUNLIGHT", MoveType.Cosmic, 10, MoveEffect.Healing);

        // Ant Moves
        public static readonly Move DarkPulse = new Move("DARKPULSE", MoveType.Nuclear, 10);
        public static readonly Move Bite = new Move("BITE", MoveType.Nuclear, 8);

        // Bee Moves
        public static readonly Move Spark = new Move("SPARK", MoveType.Thunder, 7);
        public static readonly Move Thunderbolt = new Move("THUNDERBOLT", MoveType.Thunder, 10);
        public static readonly Move Thunder = new Move("THUNDER", MoveType.Thunder, 12);

        // Cub Moves
        public static readonly Move PawStrike = new Move("PAWSTRIKE", MoveType.Nuclear, 10);
        public static readonly Move Gnaw = new Move("GNAW", MoveType.Nuclear, 8);

        // Bear Moves
        public static readonly Move Maul = new Move("MAUL", MoveType.Thunder, 7);

        public static Move CreateElementMove(AnimalElement element, MoveTemplate template)
        {
            return new Move(
                name: $"{GetElementMoveNamePrefix(element)} {template.Name}",
                type: GetMoveTypeForElement(element),
                power: template.Power,
                effect: template.Effect);
        }

        public static Move CreateNeutralMove(MoveTemplate template)
        {
            return new Move(
                name: template.Name,
                type: MoveType.Neutral,
                power: template.Power,
                effect: template.Effect);
        }

        private static string GetElementMoveNamePrefix(AnimalElement element)
        {
            return element == AnimalElement.Nature
                ? "Nature's"
                : element.ToString();
        }

        private static MoveType GetMoveTypeForElement(AnimalElement element)
        {
            return element switch
            {
                AnimalElement.Nature => MoveType.Nature,
                AnimalElement.Mystic => MoveType.Mystic,
                AnimalElement.Thunder => MoveType.Thunder,
                AnimalElement.Draconic => MoveType.Draconic,
                AnimalElement.Cosmic => MoveType.Cosmic,
                AnimalElement.Nuclear => MoveType.Nuclear,
                _ => MoveType.Neutral
            };
        }
    }
}
