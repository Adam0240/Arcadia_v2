#nullable enable

using System;

namespace Arcadia_v2
{
    public interface IGameIO
    {
        string ReadLine();
        void Write(string message);
        void WriteLine(string message = "");
    }

    public sealed class ConsoleGameIO : IGameIO
    {
        public string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLine(string message = "")
        {
            Console.WriteLine(message);
        }
    }
}
