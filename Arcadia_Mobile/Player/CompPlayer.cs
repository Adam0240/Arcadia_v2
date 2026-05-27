using Arcadia_Mobile.Creatures;
using Arcadia_Mobile.Map;

namespace Arcadia_Mobile.Player;

public sealed class CompPlayer : GenericPlayer
{
    private readonly List<Animal> battleTeamTemplate = new();

    public CompPlayer(string name, Room startingRoom)
        : base(name, startingRoom)
    {
    }

    public bool Defeated { get; set; }
    public IReadOnlyList<Animal> BattleTeamTemplate => battleTeamTemplate;

    public void SetBattleTeam(IEnumerable<Animal> templateAnimals)
    {
        ArgumentNullException.ThrowIfNull(templateAnimals);

        battleTeamTemplate.Clear();

        foreach (Animal animal in templateAnimals)
        {
            ArgumentNullException.ThrowIfNull(animal);
            battleTeamTemplate.Add(animal.Clone());
        }

        PrepareForBattle();
    }

    public void PrepareForBattle()
    {
        ClearAnimalInventory();

        foreach (Animal animal in battleTeamTemplate)
        {
            AddAnimal(animal.Clone());
        }
    }
}
