#nullable enable

namespace Arcadia_v2
{
    // Handles gym leader and champion interactions from the menu flow.
    public static class GymFlow
    {
        public static void HandleGymInteraction(
            IGameIO io,
            GameState gameState)
        {
            Player mainPlayer = gameState.MainPlayer;
            CompPlayer gymLeader1 = gameState.GymLeader1;
            CompPlayer gymLeader2 = gameState.GymLeader2;
            CompPlayer gymLeader3 = gameState.GymLeader3;
            CompPlayer gymLeader4 = gameState.GymLeader4;
            CompPlayer arcadiaChampion = gameState.ArcadiaChampion;

            if (TryHandleTrainer(
                io,
                gameState,
                gymLeader1,
                requiredBadges: 0,
                introLines: new[]
                {
                    $"\nHi! My name is {gymLeader1.Name}",
                    "This is the first Gym new trainers typically challenge.",
                    "That doesn't mean you're about to win easy!"
                },
                notEnoughBadgesMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this gym."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                gymLeader2,
                requiredBadges: 0,
                introLines: new[]
                {
                    $"The gym leader of this town is: {gymLeader2.Name}",
                    $"\nHi! My name is {gymLeader2.Name}",
                    "This town may have been remade, but my battle technique is as good as it's ever been!"
                },
                notEnoughBadgesMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this gym."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                gymLeader3,
                requiredBadges: 2,
                introLines: new[]
                {
                    $"The gym leader of this town is: {gymLeader3.Name}",
                    $"\nHi! My name is {gymLeader3.Name}",
                    "This gym starts to really test your skill, which is why I require challengers to have at least 2 badges.",
                    "Let's see if you're worthy of 3."
                },
                notEnoughBadgesMessage: "You need to have 2 badges to battle this gym!",
                alreadyDefeatedMessage: "You already defeated this gym."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                gymLeader4,
                requiredBadges: 0,
                introLines: new[]
                {
                    $"The gym leader of this town is: {gymLeader4.Name}",
                    $"\nHi! My name is {gymLeader4.Name}",
                    "This is the final Gym for gym challengers.",
                    "If you beat me, you can face the champion. Too bad your journey ends here."
                },
                notEnoughBadgesMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this gym."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                arcadiaChampion,
                requiredBadges: 4,
                introLines: new[]
                {
                    $"Arcadia Champion: {arcadiaChampion.Name}",
                    $"\nHi! My name is {arcadiaChampion.Name}",
                    "Are you the strongest trainer in the region?",
                    "You have to defeat me if you want to prove it!"
                },
                notEnoughBadgesMessage:
                    $"You need to have battled all 4 gyms in the region to face the Champion.\nYou currently only have: {mainPlayer.Badges.Count} badges.",
                alreadyDefeatedMessage:
                    "You already are Champion of this region. Perhaps a little ways north will provide one final challenge."))
            {
                return;
            }

            io.WriteLine("No gym in area.");
        }

        // Handles the shared trainer interaction pattern:
        // check location, check defeated status, check badge requirement, ask for battle, then start battle.
        private static bool TryHandleTrainer(
            IGameIO io,
            GameState gameState,
            CompPlayer trainer,
            int requiredBadges,
            string[] introLines,
            string notEnoughBadgesMessage,
            string alreadyDefeatedMessage)
        {
            Player mainPlayer = gameState.MainPlayer;

            if (mainPlayer.CurrentRoom != trainer.CurrentRoom)
            {
                return false;
            }

            if (trainer.Defeated)
            {
                io.WriteLine(alreadyDefeatedMessage);
                return true;
            }

            if (mainPlayer.Badges.Count < requiredBadges)
            {
                io.WriteLine(notEnoughBadgesMessage);
                return true;
            }

            foreach (string line in introLines)
            {
                io.WriteLine(line);
            }

            io.WriteLine("Ready to battle?");

            if (BattleHelpers.IsYes(Program.ReadUpperTrimmedInput(io)))
            {
                TrainerBattleFlow.Run(io, gameState, trainer);
            }

            return true;
        }
    }
}
