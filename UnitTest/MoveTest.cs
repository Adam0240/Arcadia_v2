using Arcadia_v2;

namespace UnitTest
{
    public class MoveTest
    {
        // Checks that creating a move with an empty name throws an argument exception.
        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("", MoveType.Fire, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with a whitespace name throws an argument exception.
        [Fact]
        public void Constructor_WhitespaceName_ThrowsArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new Move("   ", MoveType.Fire, 7));
            Assert.Equal("name", exception.ParamName);
        }

        // Checks that creating a move with negative power throws an argument out of range exception.
        [Fact]
        public void Constructor_NegativePower_ThrowsArgumentOutOfRangeException()
        {
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Move("EMBER", MoveType.Fire, -1));
            Assert.Equal("power", exception.ParamName);
        }

        // Checks that the predefined psychic move uses the corrected move name and enum type.
        [Fact]
        public void MoveData_Psychic_UsesExpectedNameTypeAndPower()
        {
            Assert.Equal("PSYCHIC", MoveData.Psychic.Name);
            Assert.Equal(MoveType.Psychic, MoveData.Psychic.Type);
            Assert.Equal(8, MoveData.Psychic.Power);
        }

        // Checks that predefined moves use the expected enum values instead of fragile string types.
        [Fact]
        public void MoveData_UsesExpectedEnumTypes()
        {
            Assert.Equal(MoveType.Normal, MoveData.Tackle.Type);
            Assert.Equal(MoveType.Water, MoveData.Surf.Type);
            Assert.Equal(MoveType.Grass, MoveData.SolarBeam.Type);
            Assert.Equal(MoveType.Dark, MoveData.Bite.Type);
            Assert.Equal(MoveType.Electric, MoveData.Thunderbolt.Type);
        }
    }
}
