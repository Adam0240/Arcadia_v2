#nullable enable

using System;

namespace Arcadia_v2
{
    // Tracks the active animals for one battle without owning input or output.
    public sealed class BattleState
    {
        private readonly Player mPlayer;
        private readonly GenericPlayer? mOpponent;
        private readonly Animal? mWildAnimal;

        private BattleState(Player player, GenericPlayer? opponent, Animal? wildAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);

            if (opponent == null && wildAnimal == null)
            {
                throw new ArgumentException("Battle state requires either a trainer opponent or a wild animal.");
            }

            mPlayer = player;
            mOpponent = opponent;
            mWildAnimal = wildAnimal;
        }

        public int PlayerActiveIndex { get; private set; }
        public int OpponentActiveIndex { get; private set; }

        public Animal PlayerAnimal => mPlayer.AnimalInventory[PlayerActiveIndex];
        public Animal OpponentAnimal => mOpponent == null
            ? mWildAnimal!
            : mOpponent.AnimalInventory[OpponentActiveIndex];

        public bool IsOver => BattleEngine.IsBattleOver(PlayerAnimal, OpponentAnimal);

        public static BattleState CreateTrainerBattle(Player player, GenericPlayer opponent)
        {
            ArgumentNullException.ThrowIfNull(opponent);
            BattleState battleState = new BattleState(player, opponent, wildAnimal: null);
            battleState.UseFirstHealthyPlayerAnimal();
            return battleState;
        }

        public static BattleState CreateWildBattle(Player player, Animal wildAnimal)
        {
            ArgumentNullException.ThrowIfNull(wildAnimal);
            BattleState battleState = new BattleState(player, opponent: null, wildAnimal);
            battleState.UseFirstHealthyPlayerAnimal();
            return battleState;
        }

        public void UseFirstPlayerAnimal()
        {
            PlayerActiveIndex = 0;
        }

        public bool UseFirstHealthyPlayerAnimal()
        {
            int nextAnimalIndex = BattleEngine.GetNextHealthyAnimalIndex(mPlayer);

            if (nextAnimalIndex == -1)
            {
                PlayerActiveIndex = 0;
                return false;
            }

            PlayerActiveIndex = nextAnimalIndex;
            return true;
        }

        public bool TrySwitchOpponentToNextHealthyAnimal(int startIndex)
        {
            if (mOpponent == null)
            {
                return false;
            }

            int nextAnimalIndex = BattleEngine.GetNextHealthyAnimalIndex(mOpponent, startIndex);

            if (nextAnimalIndex == -1)
            {
                return false;
            }

            OpponentActiveIndex = nextAnimalIndex;
            return true;
        }
    }
}
