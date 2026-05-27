using Arcadia_Mobile.Creatures;
using Arcadia_Mobile.Map;

namespace Arcadia_Mobile.Player;

public abstract class GenericPlayer
{
    private readonly List<string> starFragments = new();
    private readonly List<Animal> animalInventory = new();
    private readonly Dictionary<AnimalElement, int> bondByElement = CreateEmptyBondMap();
    private Room currentRoom;

    protected GenericPlayer(string name, Room startingRoom)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player name cannot be empty.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(startingRoom);

        Name = name;
        currentRoom = startingRoom;
    }

    public string Name { get; private set; }
    public Room CurrentRoom => currentRoom;
    public IReadOnlyList<string> StarFragments => starFragments;
    public IReadOnlyList<Animal> AnimalInventory => animalInventory;
    public IReadOnlyDictionary<AnimalElement, int> BondByElement => bondByElement;

    public void AddStarFragment(string starFragment)
    {
        if (string.IsNullOrWhiteSpace(starFragment))
        {
            throw new ArgumentException("Star fragment name cannot be empty.", nameof(starFragment));
        }

        if (!starFragments.Contains(starFragment))
        {
            starFragments.Add(starFragment);
        }
    }

    public string GetStarFragmentDisplay()
    {
        if (starFragments.Count == 0)
        {
            return "You have no star fragments!";
        }

        return "Star Fragments:\n" + string.Join("\n", starFragments);
    }

    public void AddBond(AnimalElement element, int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Bond amount cannot be negative.");
        }

        bondByElement[element] = Math.Min(100, bondByElement[element] + amount);
    }

    public int GetBond(AnimalElement element)
    {
        return bondByElement[element];
    }

    public void ResetBond(AnimalElement element)
    {
        bondByElement[element] = 0;
    }

    public string GetBondDisplay()
    {
        List<string> bondLines = new() { "Bond:" };

        foreach (AnimalElement element in GetOrderedElements())
        {
            bondLines.Add($"{element} {bondByElement[element]}%/100%");
        }

        return string.Join("\n", bondLines);
    }

    public string GetAnimalInventoryDisplay()
    {
        if (animalInventory.Count == 0)
        {
            return "Inventory is Empty! :'( ";
        }

        List<string> inventoryLines = new() { "Inventory List:" };

        foreach (Animal animal in animalInventory)
        {
            inventoryLines.Add($"{animal.Name} Health: {animal.Health}");
        }

        return string.Join("\n", inventoryLines);
    }

    public void AddAnimal(Animal animal)
    {
        ArgumentNullException.ThrowIfNull(animal);
        animalInventory.Add(animal);
    }

    public bool RemoveAnimal(Animal animal)
    {
        ArgumentNullException.ThrowIfNull(animal);
        return animalInventory.Remove(animal);
    }

    public Animal GetAnimalAt(int index)
    {
        return animalInventory[index];
    }

    public void SwapAnimalPositions(int firstIndex, int secondIndex)
    {
        Animal temp = animalInventory[firstIndex];
        animalInventory[firstIndex] = animalInventory[secondIndex];
        animalInventory[secondIndex] = temp;
    }

    public void ReplaceAnimalAt(int index, Animal replacement)
    {
        ArgumentNullException.ThrowIfNull(replacement);
        animalInventory[index] = replacement;
    }

    protected void ClearAnimalInventory()
    {
        animalInventory.Clear();
    }

    public void RestoreStarFragments(IEnumerable<string> starFragments)
    {
        ArgumentNullException.ThrowIfNull(starFragments);

        this.starFragments.Clear();

        foreach (string starFragment in starFragments)
        {
            AddStarFragment(starFragment);
        }
    }

    public void RestoreName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Player name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    public void RestoreAnimalInventory(IEnumerable<Animal> animalInventory)
    {
        ArgumentNullException.ThrowIfNull(animalInventory);

        this.animalInventory.Clear();

        foreach (Animal animal in animalInventory)
        {
            AddAnimal(animal);
        }
    }

    public void RestoreBond(IReadOnlyDictionary<AnimalElement, int> bondByElement)
    {
        ArgumentNullException.ThrowIfNull(bondByElement);

        foreach (AnimalElement element in GetOrderedElements())
        {
            int bond = bondByElement.TryGetValue(element, out int savedBond)
                ? savedBond
                : 0;

            this.bondByElement[element] = Math.Clamp(bond, 0, 100);
        }
    }

    public void MoveTo(Room room)
    {
        ArgumentNullException.ThrowIfNull(room);
        currentRoom = room;
    }

    private static Dictionary<AnimalElement, int> CreateEmptyBondMap()
    {
        return GetOrderedElements().ToDictionary(element => element, _ => 0);
    }

    private static IReadOnlyList<AnimalElement> GetOrderedElements()
    {
        return new[]
        {
            AnimalElement.Nature,
            AnimalElement.Mystic,
            AnimalElement.Thunder,
            AnimalElement.Draconic,
            AnimalElement.Cosmic,
            AnimalElement.Nuclear
        };
    }
}
