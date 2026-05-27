namespace Arcadia_Mobile.Creatures;

public static class GameData
{
    public static List<Animal> CreateAnimals()
    {
        return new List<Animal>(AnimalFactory.CreateAnimals());
    }

    public static List<Animal> CreateAdultAnimals()
    {
        return new List<Animal>(AnimalFactory.CreateAdultAnimals());
    }

    public static List<Animal> CreateNatureAdultAnimals()
    {
        return new List<Animal>(AnimalFactory.CreateNatureAdultAnimals());
    }
}
