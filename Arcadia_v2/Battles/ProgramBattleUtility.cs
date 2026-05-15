#nullable enable

using System;

namespace Arcadia_v2
{
    // Shared battle helper methods used by the legacy combat flow.
    public static partial class Program
    {
        // Damage is clamped at zero here so battle HP never underflows into negative values.
        public static void ApplyDamage(Pokemon target, int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            target.Health = Math.Max(0, target.Health - damage);
        }
    }
}
