using Arcadia_v2;

namespace UnitTest;

[Collection("Console Input")]
public class ProgramInputTests
{
    // Verifies that the trimmed input helper removes surrounding whitespace and preserves case.
    [Fact]
    public void ReadTrimmedInput_TrimsWhitespaceWithoutChangingCase()
    {
        TextReader originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("  Ash Ketchum  " + Environment.NewLine));

            string result = Program.ReadTrimmedInput();

            Assert.Equal("Ash Ketchum", result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }

    // Verifies that the normalized input helper trims whitespace and uppercases the entered command text.
    [Fact]
    public void ReadUpperTrimmedInput_TrimsAndUppercasesConsoleInput()
    {
        TextReader originalIn = Console.In;

        try
        {
            Console.SetIn(new StringReader("  go North  " + Environment.NewLine));

            string result = Program.ReadUpperTrimmedInput();

            Assert.Equal("GO NORTH", result);
        }
        finally
        {
            Console.SetIn(originalIn);
        }
    }
}
