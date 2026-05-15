#nullable enable

namespace Arcadia_v2
{
    // Entry point for the text adventure. The main gameplay flow now runs from GameLoop.
    public static partial class Program
    {
        public static void Main()
        {
            GameLoop.Run();
        }
    }
}
