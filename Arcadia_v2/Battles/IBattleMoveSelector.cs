#nullable enable

using Arcadia_v2.Creatures;

namespace Arcadia_v2
{
    public interface IBattleMoveSelector
    {
        Move SelectMove(Animal animal);
    }
}
