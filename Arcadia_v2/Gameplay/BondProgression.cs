#nullable enable

using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcadia_v2
{
    public static class BondProgression
    {
        private static readonly Dictionary<AnimalElement, string> RequiredFragmentsByElement = new()
        {
            [AnimalElement.Nature] = "Nature Star Fragment",
            [AnimalElement.Mystic] = "Mystic Star Fragment",
            [AnimalElement.Thunder] = "Thunder Star Fragment",
            [AnimalElement.Draconic] = "Draconic Star Fragment",
            [AnimalElement.Cosmic] = "Cosmic Star Fragment",
            [AnimalElement.Nuclear] = "Nuclear Star Fragment"
        };

        public static bool TryAddBond(Player player, AnimalElement element, int amount)
        {
            ArgumentNullException.ThrowIfNull(player);

            if (!RequiredFragmentsByElement.TryGetValue(element, out string? requiredFragment) ||
                !player.StarFragments.Contains(requiredFragment))
            {
                return false;
            }

            player.AddBond(element, amount);
            return true;
        }

        public static bool TryGetElementForStarFragment(string starFragment, out AnimalElement element)
        {
            foreach (KeyValuePair<AnimalElement, string> requiredFragment in RequiredFragmentsByElement)
            {
                if (requiredFragment.Value == starFragment)
                {
                    element = requiredFragment.Key;
                    return true;
                }
            }

            element = default;
            return false;
        }
    }
}
