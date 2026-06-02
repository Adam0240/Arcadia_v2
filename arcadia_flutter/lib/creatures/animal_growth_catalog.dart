import '../player/player.dart';
import 'animal.dart';
import 'animal_catalog.dart';

class AnimalGrowthOption {
  const AnimalGrowthOption({
    required this.partyIndex,
    required this.currentAnimal,
    required this.adultAnimal,
  });

  final int partyIndex;
  final Animal currentAnimal;
  final Animal adultAnimal;
}

class AnimalGrowthCatalog {
  const AnimalGrowthCatalog._();

  static const Map<String, String> _adultSpeciesByBaseSpecies = {
    'CAT': 'LION',
    'DOG': 'WOLF',
    'HORSE': 'STALLION',
    'TURTLE': 'TORTOISE',
    'BIRD': 'EAGLE',
    'ANT': 'BEE',
    'CUB': 'BEAR',
    'SERPENT': 'DRAGON',
  };

  static List<AnimalGrowthOption> getGrowthOptions(Player player) {
    final options = <AnimalGrowthOption>[];

    for (var i = 0; i < player.animalInventory.length; i += 1) {
      final animal = player.animalInventory[i];

      if (player.getBond(animal.element) < 100) {
        continue;
      }

      final adultAnimal = tryCreateAdultForm(animal);
      if (adultAnimal != null) {
        options.add(
          AnimalGrowthOption(
            partyIndex: i,
            currentAnimal: animal,
            adultAnimal: adultAnimal,
          ),
        );
      }
    }

    return options;
  }

  static bool hasGrowthOptions(Player player) {
    return getGrowthOptions(player).isNotEmpty;
  }

  static Animal? tryCreateAdultForm(Animal animal) {
    final nameParts = animal.name.split('_');
    if (nameParts.length != 2) {
      return null;
    }

    final adultSpecies = _adultSpeciesByBaseSpecies[nameParts[1]];
    if (adultSpecies == null) {
      return null;
    }

    final adultName = '${nameParts[0]}_$adultSpecies';
    return AnimalCatalog.createAnimals()
        .singleWhere((candidate) => candidate.name == adultName)
        .clone();
  }
}
