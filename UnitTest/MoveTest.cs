using System.Reflection;
using Arcadia_v2.Creatures;

namespace UnitTest
{
    public class MoveTest
    {
        // Checks that creating a move with an empty name throws an argument exception.
        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("", ElementType.Draconic, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with a whitespace name throws an argument exception.
        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("   ", ElementType.Draconic, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with negative power throws an argument out of range exception.
        [Fact]
        public void Constructor_NegativePower_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Move("EMBER", ElementType.Draconic, -1));
            Assert.Equal("power", exception.ParamName);
        }

        // Checks that creating a move requires an explicit behavior category.
        [Fact]
        public void Constructor_UnspecifiedEffect_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("EMBER", ElementType.Draconic, 7, MoveEffect.Unspecified));
            Assert.Equal("effect", exception.ParamName);
        }

        // Checks that the predefined Mystic move uses the expected name, type, and power.
        [Fact]
        public void MoveData_DeepseaRupture_UsesExpectedNameTypeAndPower()
        {
            Assert.Equal("Deepsea Rupture", MoveData.DEEPSEA_RUPTURE.Name);
            Assert.Equal(ElementType.Mystic, MoveData.DEEPSEA_RUPTURE.Type);
            Assert.Equal(8, MoveData.DEEPSEA_RUPTURE.Power);
            Assert.Equal(MoveEffect.Damage, MoveData.DEEPSEA_RUPTURE.Effect);
        }

        // Checks that predefined moves use the expected enum values instead of fragile string types.
        [Fact]
        public void MoveData_UsesExpectedEnumTypes()
        {
            Assert.Equal(ElementType.Base, MoveData.POUNCE.Type);
            Assert.Equal(ElementType.Mystic, MoveData.CURRENT_RUSH.Type);
            Assert.Equal(ElementType.Nature, MoveData.THORNWRAP.Type);
            Assert.Equal(ElementType.Nuclear, MoveData.RAD_BURST.Type);
            Assert.Equal(ElementType.Thunder, MoveData.VOLT_JAB.Type);
        }

        // Checks that Bloom is the current predefined healing move.
        [Fact]
        public void MoveData_Bloom_IsHealingMove()
        {
            Assert.Equal("Bloom", MoveData.BLOOM.Name);
            Assert.Equal(ElementType.Nature, MoveData.BLOOM.Type);
            Assert.Equal(10, MoveData.BLOOM.Power);
            Assert.Equal(MoveEffect.Heal, MoveData.BLOOM.Effect);
        }

        // Checks that every predefined move has the intended user-facing display name.
        [Fact]
        public void MoveData_PredefinedDisplayNames_AreIntentional()
        {
            Dictionary<string, string> expectedNames = new(StringComparer.Ordinal)
            {
                ["POUNCE"] = "Pounce",
                ["FELINE_REFLEX"] = "Feline Reflex",
                ["LOYAL_RUSH"] = "Loyal Rush",
                ["WILD_CHASE"] = "Wild Chase",
                ["HOOF_KICK"] = "Hoof Kick",
                ["STAMPEDE"] = "Stampede",
                ["HEAD_BASH"] = "Head Bash",
                ["DEEP_RETREAT"] = "Deep Retreat",
                ["BEAK_STRIKE"] = "Beak Strike",
                ["QUICK_TALON"] = "Quick Talon",
                ["MANDIBLE_BITE"] = "Mandible Bite",
                ["COLONY_RUSH"] = "Colony Rush",
                ["PLAY_SWIPE"] = "Play Swipe",
                ["TUMBLE_RUSH"] = "Tumble Rush",
                ["VENOM_FANG"] = "Venom Fang",
                ["SHADOW_FANG"] = "Shadow Fang",
                ["THORNWRAP"] = "Thorn Wrap",
                ["VERDANT_SURGE"] = "Verdant Surge",
                ["BLOOM"] = "Bloom",
                ["NATURES_WRATH"] = "Nature's Wrath",
                ["CURRENT_RUSH"] = "Current Rush",
                ["OCEAN_PULSE"] = "Ocean Pulse",
                ["DEEPSEA_RUPTURE"] = "Deepsea Rupture",
                ["TIDAL_BREAK"] = "Tidal Break",
                ["STATIC_CLAW"] = "Static Claw",
                ["VOLT_JAB"] = "Volt Jab",
                ["ARC_PULSE"] = "Arc Pulse",
                ["THUNDER_RIFT"] = "Thunder Rift",
                ["EMBER_BITE"] = "Ember Bite",
                ["INFERNO_ROAR"] = "Inferno Roar",
                ["RAGE_PULSE"] = "Rage Pulse",
                ["DRAGON_FALL"] = "Dragon's Fall",
                ["STAR_FLICK"] = "Star Flick",
                ["LUNAR_PULSE"] = "Lunar Pulse",
                ["COMET_STRIKE"] = "Comet Strike",
                ["SUPERNOVA"] = "Supernova",
                ["RAD_BURST"] = "Rad Burst",
                ["FALLOUT_BITE"] = "Fallout Bite",
                ["CONTAMINATE"] = "Contaminate",
                ["CORE_DETONATION"] = "Core Detonation"
            };
            Dictionary<string, Move> actualMoves = typeof(MoveData)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(Move))
                .ToDictionary(field => field.Name, field => (Move)field.GetValue(null)!);

            Assert.Equal(expectedNames.Keys.OrderBy(name => name), actualMoves.Keys.OrderBy(name => name));

            foreach ((string fieldName, string expectedDisplayName) in expectedNames)
            {
                Assert.Equal(expectedDisplayName, actualMoves[fieldName].Name);
            }
        }
    }
}
