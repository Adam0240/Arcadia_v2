#nullable enable

namespace Arcadia_v2
{
    // Handles guardian and Elemental Titan interactions from the menu flow.
    public static class SanctuaryFlow
    {
        public static void HandleSanctuaryInteraction(
            IGameIO io,
            GameState gameState)
        {
            Player mainPlayer = gameState.MainPlayer;
            CompPlayer guardian1 = gameState.Guardian1;
            CompPlayer guardian2 = gameState.Guardian2;
            CompPlayer guardian3 = gameState.Guardian3;
            CompPlayer guardian4 = gameState.Guardian4;
            CompPlayer elementalTitan = gameState.ElementalTitan;

            if (TryHandleTrainer(
                io,
                gameState,
                guardian1,
                requiredStarFragments: 0,
                introLines: new[]
                {
                    $"\nHi! My name is {guardian1.Name}",
                    "This is the first sanctuary new challengers typically face.",
                    "That doesn't mean you're about to win easy!"
                },
                notEnoughStarFragmentsMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this sanctuary's guardian."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                guardian2,
                requiredStarFragments: 0,
                introLines: new[]
                {
                    $"The guardian of this sanctuary is: {guardian2.Name}",
                    $"\nHi! My name is {guardian2.Name}",
                    "This town may have been remade, but my battle technique is as good as it's ever been!"
                },
                notEnoughStarFragmentsMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this sanctuary's guardian."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                guardian3,
                requiredStarFragments: 2,
                introLines: new[]
                {
                    $"The guardian of this sanctuary is: {guardian3.Name}",
                    $"\nHi! My name is {guardian3.Name}",
                    "This sanctuary starts to really test your skill, which is why I require challengers to have at least 2 star fragments.",
                    "Let's see if you're worthy of 3."
                },
                notEnoughStarFragmentsMessage: "You need to have 2 star fragments to battle this guardian!",
                alreadyDefeatedMessage: "You already defeated this sanctuary's guardian."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                guardian4,
                requiredStarFragments: 0,
                introLines: new[]
                {
                    $"The guardian of this sanctuary is: {guardian4.Name}",
                    $"\nHi! My name is {guardian4.Name}",
                    "This is the final sanctuary for challengers.",
                    "If you beat me, you can face the Elemental Titan. Too bad your journey ends here."
                },
                notEnoughStarFragmentsMessage: string.Empty,
                alreadyDefeatedMessage: "You already defeated this sanctuary's guardian."))
            {
                return;
            }

            if (TryHandleTrainer(
                io,
                gameState,
                elementalTitan,
                requiredStarFragments: 4,
                introLines: new[]
                {
                    $"Elemental Sanctuary: {elementalTitan.Name}",
                    $"\nHi! My name is {elementalTitan.Name}",
                    "Are you the strongest challenger in the region?",
                    "You have to defeat me if you want to prove it!"
                },
                notEnoughStarFragmentsMessage:
                    $"You need to have defeated all 4 sanctuaries in the region to face the Elemental Titan.\nYou currently only have: {mainPlayer.StarFragments.Count} star fragments.",
                alreadyDefeatedMessage:
                    "You already defeated the Elemental Titan. Perhaps a little ways north will provide one final challenge."))
            {
                return;
            }

            io.WriteLine("No sanctuary in area.");
        }

        // Handles the shared trainer interaction pattern:
        // check location, check defeated status, check star fragment requirement, ask for battle, then start battle.
        private static bool TryHandleTrainer(
            IGameIO io,
            GameState gameState,
            CompPlayer trainer,
            int requiredStarFragments,
            string[] introLines,
            string notEnoughStarFragmentsMessage,
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

            if (mainPlayer.StarFragments.Count < requiredStarFragments)
            {
                io.WriteLine(notEnoughStarFragmentsMessage);
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
