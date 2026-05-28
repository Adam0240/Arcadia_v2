import '../creatures/animal_element.dart';
import '../map/room_id.dart';
import 'guardian_definition.dart';

class GuardianCatalog {
  const GuardianCatalog._();

  static const List<GuardianDefinition> definitions = [
    GuardianDefinition(
      name: 'Nature Guardian',
      roomId: RoomId.oakPass,
      teamAnimalIndexes: [3, 14],
      rewardStarFragment: 'Nature Star Fragment',
      rewardElement: AnimalElement.nature,
      requiredStarFragments: 0,
      introLines: [
        'Hi! My name is Nature Guardian.',
        'This is the first sanctuary new challengers typically face.',
        "That doesn't mean you're about to win easy!",
      ],
    ),
    GuardianDefinition(
      name: 'Mystic Guardian',
      roomId: RoomId.newNucleon,
      teamAnimalIndexes: [5, 9],
      rewardStarFragment: 'Mystic Star Fragment',
      rewardElement: AnimalElement.mystic,
      requiredStarFragments: 0,
      introLines: [
        'The guardian of this sanctuary is: Mystic Guardian.',
        'Hi! My name is Mystic Guardian.',
        'This town may have been remade, but my battle technique is as good as it has ever been!',
      ],
    ),
    GuardianDefinition(
      name: 'Thunder Guardian',
      roomId: RoomId.ikena,
      teamAnimalIndexes: [10, 16],
      rewardStarFragment: 'Thunder Star Fragment',
      rewardElement: AnimalElement.thunder,
      requiredStarFragments: 2,
      introLines: [
        'The guardian of this sanctuary is: Thunder Guardian.',
        'Hi! My name is Thunder Guardian.',
        'This sanctuary starts to really test your skill.',
        "Let's see if you're worthy of 3.",
      ],
      notEnoughStarFragmentsMessage:
          'You need to have 2 star fragments to battle this guardian!',
    ),
    GuardianDefinition(
      name: 'Draconic Guardian',
      roomId: RoomId.wyrmrest,
      teamAnimalIndexes: [13, 17],
      rewardStarFragment: 'Draconic Star Fragment',
      rewardElement: AnimalElement.draconic,
      requiredStarFragments: 0,
      introLines: [
        'The guardian of this sanctuary is: Draconic Guardian.',
        'Hi! My name is Draconic Guardian.',
        'This is the final sanctuary for challengers.',
        'If you beat me, you can face the Elemental Titan. Too bad your journey ends here.',
      ],
    ),
  ];
}
