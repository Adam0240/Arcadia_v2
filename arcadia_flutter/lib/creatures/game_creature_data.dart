import 'animal.dart';
import 'animal_catalog.dart';

class GameCreatureData {
  const GameCreatureData._();

  static List<Animal> createAnimals() {
    return AnimalCatalog.createAnimals();
  }

  static List<Animal> createAdultAnimals() {
    return AnimalCatalog.createAdultAnimals();
  }

  static List<Animal> createNatureAdultAnimals() {
    return AnimalCatalog.createNatureAdultAnimals();
  }
}
