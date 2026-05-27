namespace Arcadia_Mobile.Creatures;

public static class AnimalFactory
{
    private const int DefaultSpeed = 7;
    private const int DefaultLevel = 0;

    private static readonly SpeciesTemplate[] Species =
    {
        new("CAT", 75, MoveData.POUNCE, MoveData.FELINE_REFLEX, UsesAdvancedElementMoves: false, NatureSpeedOverride: 9),
        new("LION", 75, MoveData.POUNCE, MoveData.FELINE_REFLEX, UsesAdvancedElementMoves: true),
        new("DOG", 45, MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, UsesAdvancedElementMoves: false, NatureBaseHealthOverride: 40),
        new("WOLF", 80, MoveData.LOYAL_RUSH, MoveData.WILD_CHASE, UsesAdvancedElementMoves: true),
        new("HORSE", 80, MoveData.HOOF_KICK, MoveData.STAMPEDE, UsesAdvancedElementMoves: false),
        new("STALLION", 40, MoveData.HOOF_KICK, MoveData.STAMPEDE, UsesAdvancedElementMoves: true),
        new("TURTLE", 80, MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, UsesAdvancedElementMoves: false),
        new("TORTOISE", 40, MoveData.HEAD_BASH, MoveData.DEEP_RETREAT, UsesAdvancedElementMoves: true),
        new("BIRD", 80, MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, UsesAdvancedElementMoves: false),
        new("EAGLE", 40, MoveData.BEAK_STRIKE, MoveData.QUICK_TALON, UsesAdvancedElementMoves: true),
        new("ANT", 65, MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, UsesAdvancedElementMoves: false),
        new("BEE", 80, MoveData.MANDIBLE_BITE, MoveData.COLONY_RUSH, UsesAdvancedElementMoves: true),
        new("CUB", 70, MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, UsesAdvancedElementMoves: false),
        new("BEAR", 75, MoveData.PLAY_SWIPE, MoveData.TUMBLE_RUSH, UsesAdvancedElementMoves: true),
        new("SERPENT", 75, MoveData.VENOM_FANG, MoveData.SHADOW_FANG, UsesAdvancedElementMoves: false),
        new("DRAGON", 45, MoveData.VENOM_FANG, MoveData.SHADOW_FANG, UsesAdvancedElementMoves: true)
    };

    private static readonly ElementTemplate[] Elements =
    {
        new("N", AnimalElement.Nature, MoveData.THORNWRAP, MoveData.VERDANT_SURGE, MoveData.BLOOM, MoveData.NATURES_WRATH),
        new("M", AnimalElement.Mystic, MoveData.CURRENT_RUSH, MoveData.OCEAN_PULSE, MoveData.DEEPSEA_RUPTURE, MoveData.TIDAL_BREAK),
        new("T", AnimalElement.Thunder, MoveData.STATIC_CLAW, MoveData.VOLT_JAB, MoveData.ARC_PULSE, MoveData.THUNDER_RIFT),
        new("D", AnimalElement.Draconic, MoveData.EMBER_BITE, MoveData.INFERNO_ROAR, MoveData.RAGE_PULSE, MoveData.DRAGON_FALL),
        new("C", AnimalElement.Cosmic, MoveData.STAR_FLICK, MoveData.LUNAR_PULSE, MoveData.COMET_STRIKE, MoveData.SUPERNOVA),
        new("NU", AnimalElement.Nuclear, MoveData.RAD_BURST, MoveData.FALLOUT_BITE, MoveData.CONTAMINATE, MoveData.CORE_DETONATION)
    };

    public static IReadOnlyList<Animal> CreateAnimals()
    {
        List<Animal> animals = new()
        {
            CreateAnimalEntry(
                id: 0,
                name: "NULL0",
                element: AnimalElement.Nature,
                speed: 0,
                baseHealth: 0,
                level: 0,
                moves: new[] { MoveData.POUNCE, MoveData.FELINE_REFLEX, MoveData.THORNWRAP, MoveData.VERDANT_SURGE })
        };

        int nextId = 1;

        foreach (ElementTemplate element in Elements)
        {
            foreach (SpeciesTemplate species in Species)
            {
                animals.Add(CreateAnimalEntry(nextId, element, species));
                ++nextId;
            }
        }

        return animals;
    }

    public static IReadOnlyList<Animal> CreateAdultAnimals()
    {
        HashSet<string> adultSpecies = new(StringComparer.Ordinal)
        {
            "LION",
            "WOLF",
            "STALLION",
            "TORTOISE",
            "EAGLE",
            "BEE",
            "BEAR",
            "DRAGON"
        };

        return CreateAnimals()
            .Where(animal => adultSpecies.Contains(GetSpeciesName(animal.Name)))
            .ToList();
    }

    public static IReadOnlyList<Animal> CreateNatureAdultAnimals()
    {
        return CreateAdultAnimals()
            .Where(animal => animal.Element == AnimalElement.Nature)
            .ToList();
    }

    private static string GetSpeciesName(string animalName)
    {
        string[] nameParts = animalName.Split('_', 2);
        return nameParts.Length == 2 ? nameParts[1] : animalName;
    }

    private static Animal CreateAnimalEntry(int id, ElementTemplate element, SpeciesTemplate species)
    {
        Move[] elementMoves = species.UsesAdvancedElementMoves
            ? new[] { element.AdvancedMove1, element.AdvancedMove2 }
            : new[] { element.BasicMove1, element.BasicMove2 };
        int baseHealth = species.GetBaseHealth(element.Element);

        return CreateAnimalEntry(
            id: id,
            name: $"{element.Prefix}_{species.Name}",
            element: element.Element,
            speed: species.GetSpeed(element.Element),
            baseHealth: baseHealth,
            level: DefaultLevel,
            moves: new[] { species.BaseMove1, species.BaseMove2, elementMoves[0], elementMoves[1] });
    }

    private static Animal CreateAnimalEntry(
        int id,
        string name,
        AnimalElement element,
        int speed,
        int baseHealth,
        int level,
        IEnumerable<Move> moves)
    {
        return new Animal(
            id: id,
            name: name,
            element: element,
            speed: speed,
            baseHealth: baseHealth,
            health: baseHealth,
            level: level,
            moves: moves);
    }

    private sealed record SpeciesTemplate(
        string Name,
        int BaseHealth,
        Move BaseMove1,
        Move BaseMove2,
        bool UsesAdvancedElementMoves,
        int? NatureSpeedOverride = null,
        int? NatureBaseHealthOverride = null)
    {
        public int GetSpeed(AnimalElement element)
        {
            return element == AnimalElement.Nature && NatureSpeedOverride.HasValue
                ? NatureSpeedOverride.Value
                : DefaultSpeed;
        }

        public int GetBaseHealth(AnimalElement element)
        {
            return element == AnimalElement.Nature && NatureBaseHealthOverride.HasValue
                ? NatureBaseHealthOverride.Value
                : BaseHealth;
        }
    }

    private sealed record ElementTemplate(
        string Prefix,
        AnimalElement Element,
        Move BasicMove1,
        Move BasicMove2,
        Move AdvancedMove1,
        Move AdvancedMove2);
}
