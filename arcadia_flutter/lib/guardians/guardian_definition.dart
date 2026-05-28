import '../creatures/animal_element.dart';
import '../map/room_id.dart';

class GuardianDefinition {
  const GuardianDefinition({
    required this.name,
    required this.roomId,
    required this.teamAnimalIndexes,
    required this.rewardStarFragment,
    required this.rewardElement,
    required this.requiredStarFragments,
    required this.introLines,
    this.notEnoughStarFragmentsMessage,
  });

  final String name;
  final RoomId roomId;
  final List<int> teamAnimalIndexes;
  final String rewardStarFragment;
  final AnimalElement rewardElement;
  final int requiredStarFragments;
  final List<String> introLines;
  final String? notEnoughStarFragmentsMessage;
}
