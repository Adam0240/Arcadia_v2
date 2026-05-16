#nullable enable

using System;

namespace Arcadia_v2
{
    // Tracks the active Pokemon for one battle without owning input or output.
    public sealed class BattleState
    {
        private readonly Player mPlayer;
        private readonly GenericPlayer? mOpponent;
        private readonly Pokemon? mWildPokemon;

        private BattleState(Player player, GenericPlayer? opponent, Pokemon? wildPokemon)
        {
            ArgumentNullException.ThrowIfNull(player);

            if (opponent == null && wildPokemon == null)
            {
                throw new ArgumentException("Battle state requires either a trainer opponent or a wild Pokemon.");
            }

            mPlayer = player;
            mOpponent = opponent;
            mWildPokemon = wildPokemon;
        }

        public int PlayerActiveIndex { get; private set; }
        public int OpponentActiveIndex { get; private set; }

        public Pokemon PlayerPokemon => mPlayer.PokemonInventory[PlayerActiveIndex];

        public Pokemon OpponentPokemon => mOpponent == null
            ? mWildPokemon!
            : mOpponent.PokemonInventory[OpponentActiveIndex];

        public bool IsOver => BattleEngine.IsBattleOver(PlayerPokemon, OpponentPokemon);

        public static BattleState CreateTrainerBattle(Player player, GenericPlayer opponent)
        {
            ArgumentNullException.ThrowIfNull(opponent);
            BattleState battleState = new BattleState(player, opponent, wildPokemon: null);
            battleState.UseFirstHealthyPlayerPokemon();
            return battleState;
        }

        public static BattleState CreateWildBattle(Player player, Pokemon wildPokemon)
        {
            ArgumentNullException.ThrowIfNull(wildPokemon);
            BattleState battleState = new BattleState(player, opponent: null, wildPokemon);
            battleState.UseFirstHealthyPlayerPokemon();
            return battleState;
        }

        public void UseFirstPlayerPokemon()
        {
            PlayerActiveIndex = 0;
        }

        public bool UseFirstHealthyPlayerPokemon()
        {
            int nextPokemonIndex = BattleEngine.GetNextHealthyPokemonIndex(mPlayer);

            if (nextPokemonIndex == -1)
            {
                PlayerActiveIndex = 0;
                return false;
            }

            PlayerActiveIndex = nextPokemonIndex;
            return true;
        }

        public bool TrySwitchOpponentToNextHealthyPokemon(int startIndex)
        {
            if (mOpponent == null)
            {
                return false;
            }

            int nextPokemonIndex = BattleEngine.GetNextHealthyPokemonIndex(mOpponent, startIndex);

            if (nextPokemonIndex == -1)
            {
                return false;
            }

            OpponentActiveIndex = nextPokemonIndex;
            return true;
        }
    }
}
