#nullable enable

using Arcadia_v2;
using System.Collections.Generic;

namespace Arcadia_v2.Creatures
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
                //Nature
                CreateAnimalEntry(
                    id: 0,
                    name: "NULL0",
                    element: AnimalElement.Nature,
                    speed: 0,
                    baseHealth: 0,
                    health: 0,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),

                CreateAnimalEntry(
                    id: 1,
                    name: "N_CAT",
                    element: AnimalElement.Nature,
                    speed: 9,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),

                CreateAnimalEntry(
                    id: 2,
                    name: "N_LION",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.BLOOM, MoveData.NATURES_WRATH }),

                CreateAnimalEntry(
                    id: 3,
                    name: "N_DOG",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),
                CreateAnimalEntry(
                    id: 4,
                    name: "N_WOLF",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.BLOOM, MoveData.NATURES_WRATH }),
                CreateAnimalEntry(
                    id: 5,
                    name: "N_HORSE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),

                CreateAnimalEntry(
                    id: 6,
                    name: "N_STALLION",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.BLOOM, MoveData.NATURES_WRATH }),
                CreateAnimalEntry(
                    id: 7,
                    name: "N_TURTLE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),
                CreateAnimalEntry(
                    id: 8,
                    name: "N_TORTOISE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.BLOOM, MoveData.NATURES_WRATH }),
                CreateAnimalEntry(
                    id: 9,
                    name: "N_BIRD",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),

                CreateAnimalEntry(
                    id: 10,
                    name: "N_EAGLE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.BLOOM, MoveData.NATURES_WRATH }),
                CreateAnimalEntry(
                    id: 11,
                    name: "N_ANT",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),
                CreateAnimalEntry(
                    id: 12,
                    name: "N_BEE",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.BLOOM, MoveData.NATURES_WRATH }),

                CreateAnimalEntry(
                    id: 13,
                    name: "N_CUB",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),
                CreateAnimalEntry(
                    id: 14,
                    name: "N_BEAR",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.BLOOM, MoveData.NATURES_WRATH }),

                CreateAnimalEntry(
                    id: 15,
                    name: "N_SERPENT",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.THORNWRAP, MoveData.VERDANT_SURGE }),

                CreateAnimalEntry(
                    id: 16,
                    name: "N_DRAGON",
                    element: AnimalElement.Nature,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.BLOOM, MoveData.NATURES_WRATH }),

                //Mystic
                CreateAnimalEntry(
                    id: 17,
                    name: "M_CAT",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 18,
                    name: "M_LION",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 19,
                    name: "M_DOG",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 20,
                    name: "M_WOLF",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 21,
                    name: "M_HORSE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 22,
                    name: "M_STALLION",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 23,
                    name: "M_TURTLE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 24,
                    name: "M_TORTOISE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 25,
                    name: "M_BIRD",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 26,
                    name: "M_EAGLE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 27,
                    name: "M_ANT",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 28,
                    name: "M_BEE",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 29,
                    name: "M_CUB",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 30,
                    name: "M_BEAR",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                CreateAnimalEntry(
                    id: 31,
                    name: "M_SERPENT",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.CURRENT_RUSH, MoveData.OCEON_PULSE }),

                CreateAnimalEntry(
                    id: 32,
                    name: "M_DRAGON",
                    element: AnimalElement.Mystic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK }),

                //Thunder
                CreateAnimalEntry(
                    id: 33,
                    name: "T_CAT",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 34,
                    name: "T_LION",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 35,
                    name: "T_DOG",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 36,
                    name: "T_WOLF",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 37,
                    name: "T_HORSE",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 38,
                    name: "T_STALLION",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 39,
                    name: "T_TURTLE",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 40,
                    name: "T_TORTOISE",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 41,
                    name: "T_BIRD",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 42,
                    name: "T_EAGLE",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 43,
                    name: "T_ANT",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 44,
                    name: "T_BEE",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 45,
                    name: "T_CUB",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 46,
                    name: "T_BEAR",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                CreateAnimalEntry(
                    id: 47,
                    name: "T_SERPENT",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.STATIC_CLAW, MoveData.VOLT_JAB }),

                CreateAnimalEntry(
                    id: 48,
                    name: "T_DRAGON",
                    element: AnimalElement.Thunder,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT }),

                //Draconic
                CreateAnimalEntry(
                    id: 49,
                    name: "D_CAT",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 50,
                    name: "D_LION",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 51,
                    name: "D_DOG",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 52,
                    name: "D_WOLF",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 53,
                    name: "D_HORSE",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 54,
                    name: "D_STALLION",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 55,
                    name: "D_TURTLE",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 56,
                    name: "D_TORTOISE",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 57,
                    name: "D_BIRD",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 58,
                    name: "D_EAGLE",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 59,
                    name: "D_ANT",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 60,
                    name: "D_BEE",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 61,
                    name: "D_CUB",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 62,
                    name: "D_BEAR",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                CreateAnimalEntry(
                    id: 63,
                    name: "D_SERPENT",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR }),

                CreateAnimalEntry(
                    id: 64,
                    name: "D_DRAGON",
                    element: AnimalElement.Draconic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL }),

                //Cosmic
                CreateAnimalEntry(
                    id: 65,
                    name: "C_CAT",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 66,
                    name: "C_LION",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 67,
                    name: "C_DOG",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 68,
                    name: "C_WOLF",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 69,
                    name: "C_HORSE",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 70,
                    name: "C_STALLION",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 71,
                    name: "C_TURTLE",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 72,
                    name: "C_TORTOISE",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 73,
                    name: "C_BIRD",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 74,
                    name: "C_EAGLE",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 75,
                    name: "C_ANT",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 76,
                    name: "C_BEE",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 77,
                    name: "C_CUB",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 78,
                    name: "C_BEAR",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                CreateAnimalEntry(
                    id: 79,
                    name: "C_SERPENT",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE }),

                CreateAnimalEntry(
                    id: 80,
                    name: "C_DRAGON",
                    element: AnimalElement.Cosmic,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.COMET_STRIKE, MoveData.SUPERNOVA }),

                //Nuclear
                CreateAnimalEntry(
                    id: 81,
                    name: "NU_CAT",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 82,
                    name: "NU_LION",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 83,
                    name: "NU_DOG",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 84,
                    name: "NU_WOLF",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 85,
                    name: "NU_HORSE",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 86,
                    name: "NU_STALLION",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HOOF_KICK, MoveData.STAMPEDE, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 87,
                    name: "NU_TURTLE",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 88,
                    name: "NU_TORTOISE",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 89,
                    name: "NU_BIRD",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 90,
                    name: "NU_EAGLE",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 40,
                    health: 40,
                    level: 0,
                    moves: new[] { MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 91,
                    name: "NU_ANT",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 65,
                    health: 65,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 92,
                    name: "NU_BEE",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 80,
                    health: 80,
                    level: 0,
                    moves: new[] { MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 93,
                    name: "NU_CUB",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 70,
                    health: 70,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 94,
                    name: "NU_BEAR",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),

                CreateAnimalEntry(
                    id: 95,
                    name: "NU_SERPENT",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 75,
                    health: 75,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.RAD_BURST, MoveData.FALLOUT_BITE }),

                CreateAnimalEntry(
                    id: 96,
                    name: "NU_DRAGON",
                    element: AnimalElement.Nuclear,
                    speed: 7,
                    baseHealth: 45,
                    health: 45,
                    level: 0,
                    moves: new[] { MoveData.VENOM_FANG, MoveData.SHADOW_FANG, MoveData.CONTAMINATE, MoveData.CORE_DETONATION }),
            };

            return animals;
        }
    }
}

