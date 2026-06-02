import '../creatures/animal_element.dart';

class MovementRequirement {
  const MovementRequirement({
    this.requiredStarFragments = 0,
    this.requiredAnimalElement,
    this.requiresElementalTitanDefeat = false,
  });

  static const none = MovementRequirement();

  final int requiredStarFragments;
  final AnimalElement? requiredAnimalElement;
  final bool requiresElementalTitanDefeat;
}
