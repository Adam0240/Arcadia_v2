#nullable enable

using System;
using System.Collections.Generic;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Builds the initial game state and prints the legacy story/setup text before the main loop starts.
    public static class GameSetup
    {
        public static GameState Initialize()
        {
            PrintGuide();
            PrintIntroStory();

            GameState gameState = CreateInitialState(Program.GetName());

            PrintPostNameStory(gameState.MainPlayer);
            RoomDisplay.Print(gameState.MainPlayer.CurrentRoom);

            return gameState;
        }

        public static GameState CreateForLoad()
        {
            return CreateInitialState("Loaded Player");
        }

        private static GameState CreateInitialState(string playerName)
        {
            List<Pokemon> mainPokemon = GameData.CreatePokemon();
            List<Pokemon> gymPokemon = GameData.CreatePokemon();

            Map.Map gameMap = new Map.Map();

            Player mainPlayer = new Player(playerName, gameMap.StartRoom);

            CompPlayer gymLeader1 = new CompPlayer("Mrs. Mcmann", gameMap.GymLeader1Room);
            gymLeader1.SetBattleTeam(new[] { gymPokemon[3], gymPokemon[14] });
            gymLeader1.AddBadge("Grass Badge");

            CompPlayer gymLeader2 = new CompPlayer("Minofo", gameMap.GymLeader2Room);
            gymLeader2.SetBattleTeam(new[] { gymPokemon[5], gymPokemon[9] });
            gymLeader2.AddBadge("Water Badge");

            CompPlayer gymLeader3 = new CompPlayer("Golden", gameMap.GymLeader3Room);
            gymLeader3.SetBattleTeam(new[] { gymPokemon[10], gymPokemon[16] });
            gymLeader3.AddBadge("Rock Badge");

            CompPlayer gymLeader4 = new CompPlayer("Wiggins", gameMap.GymLeader4Room);
            gymLeader4.SetBattleTeam(new[] { gymPokemon[13], gymPokemon[17] });
            gymLeader4.AddBadge("Dragon Badge");

            CompPlayer arcadiaChampion = new CompPlayer("Adam", gameMap.ChampionRoom);
            arcadiaChampion.SetBattleTeam(new[] { gymPokemon[4], gymPokemon[8], gymPokemon[6], gymPokemon[18] });
            arcadiaChampion.AddBadge("Champion Badge");

            mainPlayer.AddPokemon(mainPokemon[1]);
            mainPlayer.AddPokemon(mainPokemon[2]);

            return new GameState(
                gameMap,
                mainPokemon,
                gymPokemon,
                mainPlayer,
                gymLeader1,
                gymLeader2,
                gymLeader3,
                gymLeader4,
                arcadiaChampion);
        }

        private static void PrintGuide()
        {
            Console.WriteLine("Game Guide:");
            Console.WriteLine("Tip: Set console as full screen for best experience.");
            Console.WriteLine("You can move by typing 'go' followed by the direction (north/south/east/west)");
            Console.WriteLine("You complete an action by typing 'action; or 'a'. this will display the options you can use\n");
        }

        private static void PrintIntroStory()
        {
            Console.WriteLine("One night at your house you hear a storm. Your cats are freaking out and you see lightning dancing outside your windows.");
            Console.WriteLine("While you're praying the tarp on your roof holds up, a strike of lightning rips through your home. You find yourself blinded by intense white light.");
            Console.WriteLine("The last thing you hear is the roar of thunder, briefly making you believe the voice of God was shouting upon you. You black out...\n");

            Console.WriteLine("Professor Acacia, traveling through what locals now call Old Nucleon,");
            Console.WriteLine("is mapping the now abandoned forest while observing Pokemon in the area.");
            Console.WriteLine("While observing a newly hatched Charmander, the sky is overtaken with clouds and it starts thundering and lightning.");
            Console.WriteLine("Without warning, there is a flash of light and the storm dissapates as quickly as it appeared.");
            Console.WriteLine("Briefly blinded, the Professor blinks a few times and suddenly gasps.");
            Console.WriteLine("Lying a few feet in front of her is a young adult, unconcious and barely breathing.\n");

            Console.WriteLine("You wake up in a comforatable bed, disoriented and unsure of exactly where you are..");
            Console.WriteLine("The last thing you remember is a bright white light and the sound of thunder.");
            Console.WriteLine("You hear a knock on the door, and the professor comes in.\n");

            Console.WriteLine("Professsor Aracia: I'm glad your finally awake!");
            Console.WriteLine("You've been unconsious for so long I was worried I'd have to transport you in a pokeball to the hospital! ");
            Console.WriteLine("Can I ask what your name is?\n");
        }

        private static void PrintPostNameStory(Player mainPlayer)
        {
            Console.WriteLine($"\n\nOkay, {mainPlayer.Name}, nice to meet you. Do you remember what happened?");
            Console.WriteLine("You say no, and explain all you remember was the light and thunder.");
            Console.WriteLine("Suddenly, from under your bed are two quick flashes of red light. Two pokemon appear in front of you.");

            Console.WriteLine("Professor: Oh wow are these your Pokemon? They look very well taken care of.");
            Console.WriteLine("You look at them and although you can't remember, you feel as though you've known these two for a long time.");
            Console.WriteLine("You look under your bed where they had appeared. There are also two pokeballs with a note.");
            Console.WriteLine("The note reads: These two have been by your side for many years. Therefore it only seemed right they make this journey with you.");
            Console.WriteLine("Your goal is to become the best. How you accomplish that is up to you.");
            Console.WriteLine("You will find me when you have acheived all you can in this region.");
            Console.WriteLine("Welcome to the world of Pokemon!\n");

            Console.WriteLine("You show the professor the note.");
            Console.WriteLine(" Professor: To become the very best.. It's quite a mystery how you appeared, but it seems as though someone has a plan for you..");
            Console.WriteLine("To become the very best in this region I suppose could mean becoming the Champion..");
            Console.WriteLine("Its a long road, but rewarding and you are sure to have many different encounters while working towards your goal.");
            Console.WriteLine("You have to defeat the 4 gyms in this region known as 'Arcadia'. Only then can you challenge the region champion.");
            Console.WriteLine("It seems you already have a good start with those two by your side.");
            Console.WriteLine("This region is full of amazing Pokemon so go explore and get stronger!");
            Console.WriteLine("I can't wait to see how your story unfolds!\n");
        }
    }
}
