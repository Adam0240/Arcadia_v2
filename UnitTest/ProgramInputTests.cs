using Arcadia_v2;

namespace UnitTest;

public class ProgramInputTests
{
    // Verifies that the trimmed input helper removes surrounding whitespace and preserves case.
    [Fact]
    public void ReadTrimmedInput_TrimsWhitespaceWithoutChangingCase()
    {
        FakeGameIO io = new("  Riley Ketchum  ");

        string result = Program.ReadTrimmedInput(io);

        Assert.Equal("Riley Ketchum", result);
    }

    // Verifies that the normalized input helper trims whitespace and uppercases the entered command text.
    [Fact]
    public void ReadUpperTrimmedInput_TrimsAndUppercasesConsoleInput()
    {
        FakeGameIO io = new("  go North  ");

        string result = Program.ReadUpperTrimmedInput(io);

        Assert.Equal("GO NORTH", result);
    }
}
