namespace Arcadia_Mobile.Creatures;

public enum ElementType
{
    Base,
    Nature,
    Mystic,
    Thunder,
    Draconic,
    Cosmic,
    Nuclear
}

public enum MoveEffect
{
    Unspecified,
    Damage,
    Heal
}

public sealed class Move
{
    public Move(string name, ElementType type, int power)
        : this(name, type, power, MoveEffect.Damage)
    {
    }

    public Move(string name, ElementType type, int power, MoveEffect effect)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Move name cannot be empty.", nameof(name));
        }

        if (power < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(power), "Move power cannot be negative.");
        }

        if (effect == MoveEffect.Unspecified)
        {
            throw new ArgumentException("Move effect must be specified.", nameof(effect));
        }

        Name = name;
        Type = type;
        Power = power;
        Effect = effect;
    }

    public string Name { get; }
    public ElementType Type { get; }
    public int Power { get; }
    public MoveEffect Effect { get; }
    public string MoveName => Name;
    public int MovePower => Power;
}
