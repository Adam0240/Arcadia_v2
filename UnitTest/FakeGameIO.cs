using Arcadia_v2;
using System.Text;

namespace UnitTest;

internal sealed class FakeGameIO : IGameIO
{
    private readonly Queue<string> mInput;
    private readonly StringBuilder mOutput = new();

    public FakeGameIO(params string[] input)
    {
        mInput = new Queue<string>(input);
    }

    public string OutputText => mOutput.ToString();

    public string ReadLine()
    {
        return mInput.Dequeue();
    }

    public void Write(string message)
    {
        mOutput.Append(message);
    }

    public void WriteLine(string message = "")
    {
        mOutput.AppendLine(message);
    }
}
