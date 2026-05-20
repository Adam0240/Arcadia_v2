# AnimalFactory Variant Refactor Plan

## Goal

Refactor `AnimalFactory` so each base creature's shared stats are defined once, while each elemental version only defines the data that changes: element and moves.

This avoids creating a full duplicated `CreateAnimalEntry()` block for every Nature/Mystic/Draconic/etc. version of the same creature.

## Current State

- `AnimalFactory.CreateAnimals()` currently creates each `Animal` with one hard-coded `CreateAnimalEntry()` call.
- Each entry repeats all animal data: id, name, element, speed, base health, health, level, and moves.
- `Animal` currently stores battle data.
- `AnimalElement` currently includes:
  - `Nature`
  - `Mystic`
  - `Thunder`
  - `Draconic`
  - `Cosmic`
  - `Nuclear`

## Target Shape

Split creature creation into three layers:

1. Base species data
   - Name
   - Speed
   - Base health
   - Any other stats that should stay the same across all element versions

2. Element variant data
   - Element
   - Move list
   - Any future per-element override data

3. Factory assembly
   - Combines one species definition with one variant definition
   - Creates the final `Animal`

## Proposed Types

Add private records inside `AnimalFactory` first. Keep them private unless other systems need access later.

```csharp
private sealed record AnimalSpecies(
    string Name,
    int Speed,
    int BaseHealth);

private sealed record AnimalVariant(
    AnimalElement Element,
    IReadOnlyList<Move> Moves);
```

## Proposed Factory Helper

Replace or overload the current helper with a version that accepts species and variant data.

```csharp
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
        health: species.BaseHealth,
        level: level,
        moves: variant.Moves);
}
```

## Example Cat Data

Define shared cat stats once.

```csharp
private static readonly AnimalSpecies CatSpecies = new(
    Name: "Cat",
    Speed: 9,
    BaseHealth: 75);
```

Define each elemental cat by only listing the differences.

```csharp
private static readonly Dictionary<AnimalElement, AnimalVariant> CatVariants = new()
{
    [AnimalElement.Nature] = new AnimalVariant(
        AnimalElement.Nature,
        new[] { MoveData.VineWhip, MoveData.RazerLeaf, MoveData.Bite, MoveData.Tackle }),

    [AnimalElement.Mystic] = new AnimalVariant(
        AnimalElement.Mystic,
        new[] { MoveData.WaterGun, MoveData.WaterPulse, MoveData.Bite, MoveData.Tackle }),

    [AnimalElement.Draconic] = new AnimalVariant(
        AnimalElement.Draconic,
        new[] { MoveData.FireFang, MoveData.Bite, MoveData.QuickAttack, MoveData.Tackle }),
};
```

Then create the actual animals from the shared species and selected variant.

```csharp
CreateAnimalEntry(1, CatSpecies, CatVariants[AnimalElement.Nature]),
CreateAnimalEntry(2, CatSpecies, CatVariants[AnimalElement.Mystic]),
CreateAnimalEntry(3, CatSpecies, CatVariants[AnimalElement.Draconic]),
```

## Implementation Steps

1. Add `AnimalSpecies` and `AnimalVariant` private records to `AnimalFactory`.
2. Add the new `CreateAnimalEntry(int id, AnimalSpecies species, AnimalVariant variant, int level = 0)` helper.
3. Convert one creature first, preferably `Cat`, to prove the pattern.
4. Run tests after the first conversion to catch naming/count/move expectation issues.
5. Decide whether animal names should be:
   - `"Cat"` for the default/base creature only
   - `"Nature Cat"`, `"Mystic Cat"`, etc. for every elemental version
6. Convert the remaining base creatures one at a time.
7. Update or replace tests that still refer to old Pokemon-to-element mappings.
8. Add tests that verify:
   - Variants of the same species share the same speed and base health
   - Variants have different elements
   - Variants have the expected moves

## Suggested Migration Order

1. Keep the existing factory working.
2. Add the new records/helper alongside the existing helper.
3. Convert `Cat`.
4. Convert two or three more creatures.
5. Once the pattern is stable, remove the old helper if it is no longer used.
6. Convert all remaining creatures.
7. Add all missing element variants.

## Design Notes

- Keep stats in species records when they should be identical across all element versions.
- Put moves in variant records because they are element-specific.
- Avoid subclassing animals per element unless the behavior itself changes.
- Avoid creating one full hard-coded method per variant unless variants will eventually have very different stats or behavior.
- If future variants need stat overrides, add optional override fields to `AnimalVariant` instead of duplicating every stat immediately.

## Open Decisions

- Should `Animal.Name` include the element, such as `"Mystic Cat"`, or should element display be handled separately?
- Should ids be assigned manually, or should the factory generate them as it expands variants?
- Should every species support every element, or should some species only have selected elemental versions?
