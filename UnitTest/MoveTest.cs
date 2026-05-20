using Arcadia_v2;

namespace UnitTest
{
    public class MoveTest
    {
        // Checks that creating a move with an empty name throws an argument exception.
        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("", MoveType.Draconic, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with a whitespace name throws an argument exception.
        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("   ", MoveType.Draconic, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with negative power throws an argument out of range exception.
        [Fact]
        public void Constructor_NegativePower_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Move("EMBER", MoveType.Draconic, -1));
            Assert.Equal("power", exception.ParamName);
        }

        // Checks that the predefined psychic move uses the corrected move name and enum type.
        [Fact]
        public void MoveData_Psychic_UsesExpectedNameTypeAndPower()
        {
            Assert.Equal("PSYCHIC", MoveData.Psychic.Name);
            Assert.Equal(MoveType.Cosmic, MoveData.Psychic.Type);
            Assert.Equal(8, MoveData.Psychic.Power);
        }

        // Checks that predefined moves use the expected enum values instead of fragile string types.
        [Fact]
        public void MoveData_UsesExpectedEnumTypes()
        {
            Assert.Equal(MoveType.Neutral, MoveData.Tackle.Type);
            Assert.Equal(MoveType.Mystic, MoveData.Surf.Type);
            Assert.Equal(MoveType.Nature, MoveData.SolarBeam.Type);
            Assert.Equal(MoveType.Nuclear, MoveData.Bite.Type);
            Assert.Equal(MoveType.Thunder, MoveData.Thunderbolt.Type);
        }

        // Checks that moves default to damage unless a healing effect is explicitly assigned.
        [Fact]
        public void Constructor_DefaultsToDamageEffect()
        {
            Move move = new("TACKLE", MoveType.Neutral, 5);

            Assert.Equal(MoveEffect.Damage, move.Effect);
        }

        // Checks that predefined healing moves carry behavior metadata instead of relying on their names.
        [Fact]
        public void MoveData_HealingMovesUseHealingEffect()
        {
            Assert.Equal(MoveEffect.Healing, MoveData.Moonlight.Effect);
            Assert.Equal(MoveEffect.Healing, MoveData.Sunlight.Effect);
            Assert.Equal(MoveEffect.Healing, MoveData.Synthesis.Effect);
            Assert.Equal(MoveEffect.Healing, MoveData.Roost.Effect);
        }
    }
}
