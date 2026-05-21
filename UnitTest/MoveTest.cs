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

        // Checks that the predefined Mystic move uses the expected name, type, and power.
        [Fact]
        public void MoveData_DeepseaRupture_UsesExpectedNameTypeAndPower()
        {
            Assert.Equal("Deepsea Rupture", MoveData.DEEPSEA_RUPTURE.Name);
            Assert.Equal(ElementType.Mystic, MoveData.DEEPSEA_RUPTURE.Type);
            Assert.Equal(8, MoveData.DEEPSEA_RUPTURE.Power);
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
    }
}
