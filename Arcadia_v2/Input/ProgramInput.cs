#nullable enable

using System;
using Arcadia_v2.Commands;

namespace Arcadia_v2
{
    // Shared console input helpers used by the legacy game flow.
    public static partial class Program
    {
        public static string ReadTrimmedInput()
        {
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static string ReadUpperTrimmedInput()
        {
            return Parser.ToUpperCase(ReadTrimmedInput());
        }

        public static string GetName()
        {
            Console.WriteLine("Enter your name");
            return ReadTrimmedInput();
        }
    }
}
