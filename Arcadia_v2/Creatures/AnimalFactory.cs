#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2
{
    // Builds the hard-coded animal roster separately from the runtime model.
    public static class AnimalFactory
    {
        private static Animal CreateAnimalEntry(
            int id,
            string name,
            AnimalElement element,
            int speed,
            int baseHealth,
            int health,
            int level,
            params Move[] moves)
        {
            return new Animal(
                id: id,
                name: name,
                element: element,
                speed: speed,
                baseHealth: baseHealth,
                health: health,
                level: level,
                moves: moves);
        }

        public static IReadOnlyList<Animal> CreateAnimals()
        {
            List<Animal> animals = new List<Animal>
            {
                CreateAnimalEntry(
                    id: 0,
                    name: "Null0",
                    element: AnimalElement.Nature,
                    speed: 0,
                    baseHealth: 0,
                    health: 0,
                    level: 0,
                    moves: new[] { MoveData.Tackle, MoveData.Tackle, MoveData.Tackle, MoveData.Tackle }),

                CreateAnimalEntry(
                    id: 1,
                    name: "CAT",
                    element: AnimalElement.Nature,
                    speed: 9,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.Moonlight, MoveData.DarkPulse, MoveData.Bite, MoveData.Tackle }),

                CreateAnimalEntry(
                    id: 2,
                    name: "LION",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.Sunlight, MoveData.Confusion, MoveData.Psychic, MoveData.Moonblast }),

                CreateAnimalEntry(
                    id: 3,
                    name: "DOG",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.Tackle, MoveData.VineWhip, MoveData.RazerLeaf, MoveData.QuickAttack }),
                CreateAnimalEntry(
                    id: 4,
                    name: "WOLF",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.PetalBlizzard, MoveData.SolarBeam, MoveData.Earthquake, MoveData.Sunlight }),
                CreateAnimalEntry(
                    id: 5,
                    name: "HORSE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.SeedBomb, MoveData.RazerLeaf, MoveData.Earthquake, MoveData.Psychic }),

                CreateAnimalEntry(
                    id: 6,
                    name: "STALLION",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.WaterGun, MoveData.QuickAttack, MoveData.WaterPulse, MoveData.Bite }),
                CreateAnimalEntry(
                    id: 7,
                    name: "TURTLE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.Bite, MoveData.HydroPump, MoveData.Surf, MoveData.QuickAttack }),
                CreateAnimalEntry(
                    id: 8,
                    name: "TORTOISE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.Splash, MoveData.Tackle, MoveData.QuickAttack, MoveData.Growl }),
                CreateAnimalEntry(
                    id: 9,
                    name: "BIRD",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.Bite, MoveData.HydroPump, MoveData.Earthquake, MoveData.Surf }),

                CreateAnimalEntry(
                    id: 10,
                    name: "EAGLE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.Tackle, MoveData.Ember, MoveData.FireFang, MoveData.QuickAttack }),
                CreateAnimalEntry(
                    id: 11,
                    name: "ANT",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.Flamethrower, MoveData.QuickAttack, MoveData.Bite, MoveData.FlameWheel }),
                CreateAnimalEntry(
                    id: 12,
                    name: "BEE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.WingAttack, MoveData.Flamethrower, MoveData.FireBlitz, MoveData.QuickAttack }),

                CreateAnimalEntry(
                    id: 13,
                    name: "CUB",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake }),
                CreateAnimalEntry(
                    id: 14,
                    name: "BEAR",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake }),

                CreateAnimalEntry(
                    id: 15,
                    name: "SERPENT",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.Spark, MoveData.Thunderbolt, MoveData.QuickAttack, MoveData.Surf }),

                CreateAnimalEntry(
                    id: 16,
                    name: "DRAGON",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.Tackle, MoveData.Peck, MoveData.QuickAttack, MoveData.WingAttack }),

                CreateAnimalEntry(
                    id: 17,
                    name: "Mystic Cat",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake }),

                CreateAnimalEntry(
                    id: 18,
                    name: "Mystic Lion",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.Spark, MoveData.Thunderbolt, MoveData.QuickAttack, MoveData.Surf }),

                CreateAnimalEntry(
                    id: 19,
                    name: "Mystic Dog",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.Tackle, MoveData.Peck, MoveData.QuickAttack, MoveData.WingAttack }),
            };

            return animals;
        }
    }
}
