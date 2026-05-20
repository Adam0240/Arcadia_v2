#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia_v2
{
    // Builds the hard-coded animal roster separately from the runtime model.
    public static class AnimalFactory
    {
        private static readonly AnimalElement[] ElementOrder =
        {
            AnimalElement.Nature,
            AnimalElement.Mystic,
            AnimalElement.Thunder,
            AnimalElement.Draconic,
            AnimalElement.Cosmic,
            AnimalElement.Nuclear
        };

        private static readonly AnimalSpecies[] Species =
        {
            new(
                SpeciesId: AnimalSpeciesId.Cat,
                Name: "Cat",
                Speed: 9,
                BaseHealth: 75,
                MoveSets: CreateMoveSets(
                    Elemental(MoveData.Fury),
                    Neutral(MoveData.SpeedAttack),
                    Neutral(MoveData.DefensiveMove),
                    Elemental(MoveData.Bomb))),

            new(
                SpeciesId: AnimalSpeciesId.Lion,
                Name: "Lion",
                Speed: 7,
                BaseHealth: 75,
                MoveSets: CreateLegacyMoveSets(
                    natureMoves: new[] { MoveData.Sunlight, MoveData.Confusion, MoveData.Psychic, MoveData.Moonblast },
                    natureOverride: new[]
                    {
                        Elemental(MoveData.Storm),
                        Neutral(MoveData.SpeedAttack),
                        Neutral(MoveData.DefensiveMove),
                        Elemental(MoveData.Roar)
                    })),

            new(
                SpeciesId: AnimalSpeciesId.Dog,
                Name: "Dog",
                Speed: 7,
                BaseHealth: 40,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Tackle, MoveData.VineWhip, MoveData.RazerLeaf, MoveData.QuickAttack })),

            new(
                SpeciesId: AnimalSpeciesId.Wolf,
                Name: "Wolf",
                Speed: 7,
                BaseHealth: 80,
                MoveSets: CreateLegacyMoveSets(
                    natureMoves: new[] { MoveData.PetalBlizzard, MoveData.SolarBeam, MoveData.Earthquake, MoveData.Sunlight },
                    natureOverride: new[]
                    {
                        Elemental(MoveData.Storm),
                        Neutral(MoveData.SpeedAttack),
                        Neutral(MoveData.DefensiveMove),
                        Elemental(MoveData.Howl)
                    })),

            new(
                SpeciesId: AnimalSpeciesId.Horse,
                Name: "Horse",
                Speed: 7,
                BaseHealth: 80,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.SeedBomb, MoveData.RazerLeaf, MoveData.Earthquake, MoveData.Psychic })),

            new(
                SpeciesId: AnimalSpeciesId.Stallion,
                Name: "Stallion",
                Speed: 7,
                BaseHealth: 40,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.WaterGun, MoveData.QuickAttack, MoveData.WaterPulse, MoveData.Bite })),

            new(
                SpeciesId: AnimalSpeciesId.Turtle,
                Name: "Turtle",
                Speed: 7,
                BaseHealth: 80,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Bite, MoveData.HydroPump, MoveData.Surf, MoveData.QuickAttack })),

            new(
                SpeciesId: AnimalSpeciesId.Tortoise,
                Name: "Tortoise",
                Speed: 7,
                BaseHealth: 40,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Splash, MoveData.Tackle, MoveData.QuickAttack, MoveData.Growl })),

            new(
                SpeciesId: AnimalSpeciesId.Bird,
                Name: "Bird",
                Speed: 7,
                BaseHealth: 80,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Bite, MoveData.HydroPump, MoveData.Earthquake, MoveData.Surf })),

            new(
                SpeciesId: AnimalSpeciesId.Eagle,
                Name: "Eagle",
                Speed: 7,
                BaseHealth: 40,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Tackle, MoveData.Ember, MoveData.FireFang, MoveData.QuickAttack })),

            new(
                SpeciesId: AnimalSpeciesId.Ant,
                Name: "Ant",
                Speed: 7,
                BaseHealth: 65,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Flamethrower, MoveData.QuickAttack, MoveData.Bite, MoveData.FlameWheel })),

            new(
                SpeciesId: AnimalSpeciesId.Bee,
                Name: "Bee",
                Speed: 7,
                BaseHealth: 80,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.WingAttack, MoveData.Flamethrower, MoveData.FireBlitz, MoveData.QuickAttack })),

            new(
                SpeciesId: AnimalSpeciesId.Cub,
                Name: "Cub",
                Speed: 7,
                BaseHealth: 70,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake })),

            new(
                SpeciesId: AnimalSpeciesId.Bear,
                Name: "Bear",
                Speed: 7,
                BaseHealth: 75,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake })),

            new(
                SpeciesId: AnimalSpeciesId.Serpent,
                Name: "Serpent",
                Speed: 7,
                BaseHealth: 75,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Spark, MoveData.Thunderbolt, MoveData.QuickAttack, MoveData.Surf })),

            new(
                SpeciesId: AnimalSpeciesId.Dragon,
                Name: "Dragon",
                Speed: 7,
                BaseHealth: 45,
                MoveSets: CreateLegacyMoveSets(new[] { MoveData.Tackle, MoveData.Peck, MoveData.QuickAttack, MoveData.WingAttack }))
        };

        private enum AnimalSpeciesId
        {
            Cat = 1,
            Lion = 2,
            Dog = 3,
            Wolf = 4,
            Horse = 5,
            Stallion = 6,
            Turtle = 7,
            Tortoise = 8,
            Bird = 9,
            Eagle = 10,
            Ant = 11,
            Bee = 12,
            Cub = 13,
            Bear = 14,
            Serpent = 15,
            Dragon = 16
        }

        private sealed record AnimalSpecies(
            AnimalSpeciesId SpeciesId,
            string Name,
            int Speed,
            int BaseHealth,
            IReadOnlyDictionary<AnimalElement, IReadOnlyList<MoveSlot>> MoveSets);

        private sealed record AnimalVariant(
            AnimalElement Element,
            IReadOnlyList<Move> Moves);

        private abstract record MoveSlot
        {
            public abstract Move CreateMove(AnimalElement element);
        }

        private sealed record ElementalMoveSlot(MoveTemplate Template) : MoveSlot
        {
            public override Move CreateMove(AnimalElement element)
            {
                return MoveData.CreateElementMove(element, Template);
            }
        }

        private sealed record NeutralMoveSlot(MoveTemplate Template) : MoveSlot
        {
            public override Move CreateMove(AnimalElement element)
            {
                return MoveData.CreateNeutralMove(Template);
            }
        }

        private sealed record FixedMoveSlot(Move Move) : MoveSlot
        {
            public override Move CreateMove(AnimalElement element)
            {
                return Move;
            }
        }

        private static MoveSlot Elemental(MoveTemplate template)
        {
            return new ElementalMoveSlot(template);
        }

        private static MoveSlot Neutral(MoveTemplate template)
        {
            return new NeutralMoveSlot(template);
        }

        private static MoveSlot Fixed(Move move)
        {
            return new FixedMoveSlot(move);
        }

        private static IReadOnlyDictionary<AnimalElement, IReadOnlyList<MoveSlot>> CreateMoveSets(params MoveSlot[] moveSlots)
        {
            Dictionary<AnimalElement, IReadOnlyList<MoveSlot>> moveSets = new();

            foreach (AnimalElement element in ElementOrder)
            {
                moveSets[element] = moveSlots;
            }

            return moveSets;
        }

        private static IReadOnlyDictionary<AnimalElement, IReadOnlyList<MoveSlot>> CreateLegacyMoveSets(
            IReadOnlyList<Move> natureMoves,
            IReadOnlyList<MoveSlot>? natureOverride = null)
        {
            Dictionary<AnimalElement, IReadOnlyList<MoveSlot>> moveSets = new()
            {
                [AnimalElement.Nature] = natureOverride ?? FixedMoves(natureMoves),
                [AnimalElement.Mystic] = FixedMoves(new[] { MoveData.WaterGun, MoveData.WaterPulse, MoveData.HydroPump, MoveData.Surf }),
                [AnimalElement.Thunder] = FixedMoves(new[] { MoveData.Spark, MoveData.Thunderbolt, MoveData.Thunder, MoveData.QuickAttack }),
                [AnimalElement.Draconic] = FixedMoves(new[] { MoveData.FireFang, MoveData.Flamethrower, MoveData.WingAttack, MoveData.Earthquake }),
                [AnimalElement.Cosmic] = FixedMoves(new[] { MoveData.Confusion, MoveData.Psychic, MoveData.Moonlight, MoveData.Moonblast }),
                [AnimalElement.Nuclear] = FixedMoves(new[] { MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake, MoveData.DarkPulse })
            };

            return moveSets;
        }

        private static IReadOnlyList<MoveSlot> FixedMoves(IReadOnlyList<Move> moves)
        {
            return moves.Select(Fixed).ToArray();
        }

        private static AnimalVariant CreateVariant(AnimalElement element, IReadOnlyList<MoveSlot> moveSlots)
        {
            return new AnimalVariant(
                element,
                moveSlots.Select(slot => slot.CreateMove(element)).ToArray());
        }

        private static Animal CreateAnimalEntry(
            int id,
            AnimalSpecies species,
            AnimalVariant variant,
            int level = 0)
        {
            return new Animal(
                id: id,
                name: $"{variant.Element} {species.Name}",
                element: variant.Element,
                speed: species.Speed,
                baseHealth: species.BaseHealth,
                currentHealth: species.BaseHealth,
                level: level,
                moves: variant.Moves);
        }

        public static IReadOnlyList<Animal> CreateAnimals()
        {
            ValidateSpecies();

            List<Animal> animals = new();

            foreach (AnimalElement element in ElementOrder)
            {
                foreach (AnimalSpecies species in Species)
                {
                    AnimalVariant variant = CreateVariant(element, GetMoveSet(species, element));
                    int id = CreateAnimalId(species.SpeciesId, element);
                    animals.Add(CreateAnimalEntry(id, species, variant));
                }
            }

            return animals;
        }

        private static int CreateAnimalId(AnimalSpeciesId speciesId, AnimalElement element)
        {
            return ((int)element * 100) + (int)speciesId;
        }

        private static IReadOnlyList<MoveSlot> GetMoveSet(AnimalSpecies species, AnimalElement element)
        {
            if (species.MoveSets.TryGetValue(element, out IReadOnlyList<MoveSlot>? moveSet))
            {
                return moveSet;
            }

            throw new KeyNotFoundException($"Missing {element} move set for {species.Name}.");
        }

        private static void ValidateSpecies()
        {
            foreach (AnimalSpecies species in Species)
            {
                foreach (AnimalElement element in ElementOrder)
                {
                    if (!species.MoveSets.TryGetValue(element, out IReadOnlyList<MoveSlot>? moveSet))
                    {
                        throw new InvalidOperationException($"Missing {element} move set for {species.Name}.");
                    }

                    if (moveSet.Count is < 1 or > 4)
                    {
                        throw new InvalidOperationException($"{element} {species.Name} must have between 1 and 4 moves.");
                    }
                }
            }
        }
    }
}
