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
      roomId: RoomId.ikena,
      teamAnimalIndexes: [5, 9],
      rewardStarFragment: 'Mystic Star Fragment',
      rewardElement: AnimalElement.mystic,
      requiredStarFragments: 2,
      introLines: [
        'The guardian of this sanctuary is: Mystic Guardian.',
        'Hi! My name is Mystic Guardian.',
        'This town may have been remade, but my battle technique is as good as it has ever been!',
      ],
      notEnoughStarFragmentsMessage:
          'You need to have 2 star fragments to battle this guardian!',
    ),
    GuardianDefinition(
      name: 'Thunder Guardian',
      roomId: RoomId.newNucleon,
      teamAnimalIndexes: [10, 16],
      rewardStarFragment: 'Thunder Star Fragment',
      rewardElement: AnimalElement.thunder,
      requiredStarFragments: 1,
      introLines: [
        'The guardian of this sanctuary is: Thunder Guardian.',
        'Hi! My name is Thunder Guardian.',
        'This sanctuary starts to really test your skill.',
        "Let's see if you're worthy of 3.",
      ],
      notEnoughStarFragmentsMessage:
          'You need to have 1 star fragment to battle this guardian!',
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
    GuardianDefinition(
      name: 'Elemental Titan',
      roomId: RoomId.guardiansTower,
      teamAnimalIndexes: [4, 8, 6, 18],
      rewardStarFragment: 'Cosmic Star Fragment',
      rewardElement: AnimalElement.cosmic,
      requiredStarFragments: 4,
      introLines: [
        'Elemental Sanctuary: Elemental Titan',
        'Hi! My name is Elemental Titan.',
        'Are you the strongest challenger in the region?',
        'You have to defeat me if you want to prove it!',
      ],
      notEnoughStarFragmentsMessage:
          'You need to have defeated all 4 sanctuaries in the region to face the Elemental Titan.',
      isElementalTitan: true,
    ),
  ];
}
