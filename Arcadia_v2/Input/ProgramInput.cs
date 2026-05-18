#nullable enable

using System;
using Arcadia_v2.Commands;

namespace Arcadia_v2
{
    // Shared input helpers used by the game flow.
    public static partial class Program
    {
        public static string ReadTrimmedInput(IGameIO io)
        {
            ArgumentNullException.ThrowIfNull(io);
            return io.ReadLine().Trim();
        }

        public static string ReadUpperTrimmedInput(IGameIO io)
        {
            return Parser.ToUpperCase(ReadTrimmedInput(io));
        }

        public static string GetName(IGameIO io)
        {
            ArgumentNullException.ThrowIfNull(io);

            while (true)
            {
                io.WriteLine("Enter your name");
                string name = ReadTrimmedInput(io);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }

                io.WriteLine("Name cannot be empty.");
            }
        }
    }
}
