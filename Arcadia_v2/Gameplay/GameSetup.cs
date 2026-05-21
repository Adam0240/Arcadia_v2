#nullable enable

using System;
using System.Collections.Generic;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Builds the initial game state and prints the story/setup text before the main loop starts.
    public static class GameSetup
    {
        public static GameState Initialize(IGameIO io)
        {
            ArgumentNullException.ThrowIfNull(io);

            PrintGuide(io);
            PrintIntroStory(io);

            GameState gameState = CreateInitialState(Program.GetName(io));

            PrintPostNameStory(io, gameState.MainPlayer);
            RoomDisplay.Print(io, gameState.MainPlayer.CurrentRoom);

            return gameState;
        }

        public static GameState CreateForLoad()
        {
            return CreateInitialState("Loaded Player");
        }

        private static GameState CreateInitialState(string playerName)
        {
            List<Animal> mainAnimals = GameData.CreateAnimals();
            List<Animal> gymAnimals = GameData.CreateAnimals();

            Map.Map gameMap = new Map.Map();

            Player mainPlayer = new Player(playerName, gameMap.StartRoom);

            CompPlayer gymLeader1 = new CompPlayer("Mrs. Mcmann", gameMap.GymLeader1Room);
            gymLeader1.SetBattleTeam(new[] { gymAnimals[3], gymAnimals[14] });
            gymLeader1.AddBadge("Grass Badge");

            CompPlayer gymLeader2 = new CompPlayer("Minofo", gameMap.GymLeader2Room);
            gymLeader2.SetBattleTeam(new[] { gymAnimals[5], gymAnimals[9] });
            gymLeader2.AddBadge("Water Badge");

            CompPlayer gymLeader3 = new CompPlayer("Golden", gameMap.GymLeader3Room);
            gymLeader3.SetBattleTeam(new[] { gymAnimals[10], gymAnimals[16] });
            gymLeader3.AddBadge("Rock Badge");

            CompPlayer gymLeader4 = new CompPlayer("Wiggins", gameMap.GymLeader4Room);
            gymLeader4.SetBattleTeam(new[] { gymAnimals[13], gymAnimals[17] });
            gymLeader4.AddBadge("Dragon Badge");

            CompPlayer arcadiaChampion = new CompPlayer("Adam", gameMap.ChampionRoom);
            arcadiaChampion.SetBattleTeam(new[] { gymAnimals[4], gymAnimals[8], gymAnimals[6], gymAnimals[18] });
            arcadiaChampion.AddBadge("Champion Badge");

            mainPlayer.AddAnimal(mainAnimals[1]);
            mainPlayer.AddAnimal(mainAnimals[2]);

            return new GameState(
                gameMap,
                mainAnimals,
                gymAnimals,
                mainPlayer,
                gymLeader1,
                gymLeader2,
                gymLeader3,
                gymLeader4,
                arcadiaChampion);
        }

        private static void PrintGuide(IGameIO io)
        {
            io.WriteLine("Game Guide:");
            io.WriteLine("Tip: Set console as full screen for best experience.");
            io.WriteLine("You can move by typing 'go' followed by the direction (north/south/east/west)");
            io.WriteLine("You complete an action by typing 'action; or 'a'. this will display the options you can use\n");
        }

        private static void PrintIntroStory(IGameIO io)
        {
            io.WriteLine("One night at your house you hear a storm. Your cats are freaking out and you see lightning dancing outside your windows.");
            io.WriteLine("While you're praying the tarp on your roof holds up, a strike of lightning rips through your home. You find yourself blinded by intense white light.");
            io.WriteLine("The last thing you hear is the roar of thunder, briefly making you believe the voice of God was shouting upon you. You black out...\n");

            io.WriteLine("Professor Acacia, traveling through what locals now call Old Nucleon,");
            io.WriteLine("is mapping the now abandoned forest while observing creatures in the area.");
            io.WriteLine("While observing a newly hatched N_CUB, the sky is overtaken with clouds and it starts thundering and lightning.");
            io.WriteLine("Without warning, there is a flash of light and the storm dissapates as quickly as it appeared.");
            io.WriteLine("Briefly blinded, the Professor blinks a few times and suddenly gasps.");
            io.WriteLine("Lying a few feet in front of her is a young adult, unconcious and barely breathing.\n");

            io.WriteLine("You wake up in a comforatable bed, disoriented and unsure of exactly where you are..");
            io.WriteLine("The last thing you remember is a bright white light and the sound of thunder.");
            io.WriteLine("You hear a knock on the door, and the professor comes in.\n");

            io.WriteLine("Professsor Aracia: I'm glad your finally awake!");
            io.WriteLine("You've been unconsious for so long I was worried I'd have to carry you to the hospital! ");
            io.WriteLine("Can I ask what your name is?\n");
        }

        private static void PrintPostNameStory(IGameIO io, Player mainPlayer)
        {
            io.WriteLine($"\n\nOkay, {mainPlayer.Name}, nice to meet you. Do you remember what happened?");
            io.WriteLine("You say no, and explain all you remember was the light and thunder.");
            io.WriteLine("Suddenly, from under your bed are two quick flashes of red light. Two creatures appear in front of you.");

            io.WriteLine("Professor: Oh wow are these your creatures? They look very well taken care of.");
            io.WriteLine("You look at them and although you can't remember, you feel as though you've known these two for a long time.");
            io.WriteLine("You look under your bed where they had appeared. There is also a note.");
            io.WriteLine("The note reads: These two have been by your side for many years. Therefore it only seemed right they make this journey with you.");
            io.WriteLine("Your goal is to become the best. How you accomplish that is up to you.");
            io.WriteLine("You will find me when you have acheived all you can in this region.");
            io.WriteLine("Welcome to the world of Arcadia!\n");

            io.WriteLine("You show the professor the note.");
            io.WriteLine(" Professor: To become the very best.. It's quite a mystery how you appeared, but it seems as though someone has a plan for you..");
            io.WriteLine("To become the very best in this region I suppose could mean becoming the Champion..");
            io.WriteLine("Its a long road, but rewarding and you are sure to have many different encounters while working towards your goal.");
            io.WriteLine("You have to defeat the 4 gyms in this region known as 'Arcadia'. Only then can you challenge the region champion.");
            io.WriteLine("It seems you already have a good start with those two by your side.");
            io.WriteLine("This region is full of amazing creatures so go explore and get stronger!");
            io.WriteLine("I can't wait to see how your story unfolds!\n");
        }
    }
}
