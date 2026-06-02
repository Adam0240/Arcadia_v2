import 'package:arcadia_flutter/creatures/game_creature_data.dart';
import 'package:arcadia_flutter/creatures/animal_element.dart';
import 'package:arcadia_flutter/map/game_map.dart';
import 'package:arcadia_flutter/map/room_direction.dart';
import 'package:arcadia_flutter/map/room_id.dart';
import 'package:arcadia_flutter/saves/game_save_repository.dart';
import 'package:arcadia_flutter/saves/game_save_state.dart';
import 'package:arcadia_flutter/screens/arcadia_map_screen.dart';
import 'package:arcadia_flutter/services/mobile_game_session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flutter map screen renders the initial map content.
  testWidgets('map screen shows initial room and movement controls', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    expect(find.text('Arcadia'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(
      find.text('Where new guardians obtain their first creature!'),
      findsOneWidget,
    );
    expect(find.text('The journey begins.'), findsOneWidget);
    expect(find.text('North'), findsOneWidget);
    expect(find.text('Inspect'), findsOneWidget);
    expect(find.text('Menu'), findsOneWidget);
  });

  // Verifies movement buttons follow the current room exits.
  testWidgets('movement updates room and disabled exits match current room', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    final southButton = tester.widget<ElevatedButton>(
      find.widgetWithText(ElevatedButton, 'South'),
    );
    expect(southButton.onPressed, isNull);

    await tester.tap(find.text('North'));
    await tester.pump();

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text('Moved to Ikena.'), findsOneWidget);

    final westButton = tester.widget<ElevatedButton>(
      find.widgetWithText(ElevatedButton, 'West'),
    );
    expect(westButton.onPressed, isNotNull);
  });

  // Verifies inspect displays the room interaction text.
  testWidgets('inspect updates status message', (WidgetTester tester) async {
    await tester.pumpWidget(_buildMapScreen());

    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(
      find.text(
        'Maia checks the starter pens and says the stable is ready for your journey.\nAnimals Nearby: None',
      ),
      findsOneWidget,
    );
  });

  // Verifies inspect displays nearby animals after moving to a wild route.
  testWidgets('inspect shows nearby animals in route rooms', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    await tester.tap(find.text('North'));
    await tester.pump();
    await tester.tap(find.text('West'));
    await tester.pump();
    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(
      find.text(
        'Tall grass rustles nearby, but this prototype keeps encounters disabled.\nAnimals Nearby: N_DOG',
      ),
      findsOneWidget,
    );
  });

  // Verifies a route room can enter a wild battle and catch the defeated animal.
  testWidgets('encounter opens wild battle and catch returns to map', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    session.currentRoom.encounterAnimals.single.health = 1;

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Encounter'));
    await tester.pumpAndSettle();

    expect(find.text('Wild Battle'), findsOneWidget);
    expect(find.text('Wild Animal'), findsOneWidget);
    expect(find.text('N_DOG'), findsWidgets);

    await tester.tap(find.text('Pounce'));
    await tester.pumpAndSettle();
    await tester.ensureVisible(find.text('Catch'));
    await tester.tap(find.text('Catch'));
    await tester.pumpAndSettle();

    expect(find.text('Road 1'), findsOneWidget);
    expect(find.text('You caught N_DOG!'), findsOneWidget);
    expect(session.currentRoom.encounterAnimals, isEmpty);
  });

  // Verifies Ikena exposes Mystic Guardian requirements.
  testWidgets('ikena guardian action reports mystic requirements', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    )..move(RoomDirection.north);

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text('Guardian'), findsOneWidget);

    await tester.tap(find.text('Guardian'));
    await tester.pump();

    expect(
      find.text('You need to have 2 star fragments to battle this guardian!'),
      findsOneWidget,
    );
  });

  // Verifies Ikena opens the Mystic Guardian battle when requirements are met.
  testWidgets('ikena guardian action opens mystic guardian battle', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    )..move(RoomDirection.north);
    session.player
      ..addStarFragment('Nature Star Fragment')
      ..addStarFragment('Mystic Star Fragment');

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Guardian'));
    await tester.pumpAndSettle();

    expect(find.text('Guardian Battle'), findsOneWidget);
    expect(find.text('Mystic Guardian'), findsWidgets);
  });

  // Verifies New Nucleon exposes Thunder Guardian requirements.
  testWidgets('new nucleon guardian action reports thunder requirements', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );
    session.restore('Nova', RoomId.newNucleon, [RoomId.maiaStable]);

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    expect(find.text('New Nucleon'), findsOneWidget);
    expect(find.text('Guardian'), findsOneWidget);

    await tester.tap(find.text('Guardian'));
    await tester.pump();

    expect(
      find.text('You need to have 1 star fragment to battle this guardian!'),
      findsOneWidget,
    );
  });

  // Verifies guardian action opens a guardian battle when requirements are met.
  testWidgets('guardian action opens guardian battle', (
    WidgetTester tester,
  ) async {
    final session =
        MobileGameSession(
            GameMap(),
            saveRepository: _MemoryGameSaveRepository(),
          )
          ..move(RoomDirection.north)
          ..move(RoomDirection.west)
          ..move(RoomDirection.south)
          ..move(RoomDirection.south);

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    expect(find.text('Oak Pass'), findsOneWidget);
    expect(find.text('Guardian'), findsOneWidget);

    await tester.tap(find.text('Guardian'));
    await tester.pumpAndSettle();

    expect(find.text('Guardian Battle'), findsOneWidget);
    expect(find.text('Nature Guardian'), findsWidgets);
    expect(find.text('N_DOG'), findsOneWidget);
  });

  // Verifies menu swaps the movement grid for player info and save controls.
  testWidgets('menu toggles between movement and save controls', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();
    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );

    await tester.pumpWidget(_buildMapScreen(gameSession: gameSession));

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(find.text('Inventory'), findsOneWidget);
    expect(find.text('Heal Animals'), findsOneWidget);
    expect(find.text('Reorder Party'), findsOneWidget);
    expect(find.text('Bond'), findsOneWidget);
    expect(find.text('Star Fragments'), findsOneWidget);
    expect(find.text('Save'), findsOneWidget);
    expect(find.text('Load'), findsNothing);
    expect(find.text('Swap'), findsNothing);
    expect(find.text('Grow'), findsNothing);
    expect(find.text('Return'), findsOneWidget);
    expect(find.text('North'), findsNothing);

    await tester.tap(find.text('Save'));
    await tester.pumpAndSettle();
    expect(find.text('Game saved.'), findsOneWidget);
    expect(await saveRepository.exists(), isTrue);

    await tester.ensureVisible(find.text('Return'));
    await tester.tap(find.text('Return'));
    await tester.pump();
    expect(find.text('North'), findsOneWidget);
    expect(find.text('Save'), findsNothing);
  });

  // Verifies menu healing restores all party animals while the player is in town.
  testWidgets('menu heal restores damaged party in town', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    )..move(RoomDirection.north);

    for (final animal in session.player.animalInventory) {
      animal.health = 1;
    }

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Heal Animals'));
    await tester.pump();

    expect(
      find.text('All your animals have been fully restored!'),
      findsOneWidget,
    );
    for (final animal in session.player.animalInventory) {
      expect(animal.health, animal.baseHealth);
    }
  });

  // Verifies menu healing is rejected outside town and does not change party health.
  testWidgets('menu heal reports non-town requirement', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );

    for (final animal in session.player.animalInventory) {
      animal.health = 1;
    }

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Heal Animals'));
    await tester.pump();

    expect(find.text("Can only heal if you're in a town!"), findsOneWidget);
    for (final animal in session.player.animalInventory) {
      expect(animal.health, 1);
    }
  });

  // Verifies menu growth appears for eligible animals and grows the selected animal.
  testWidgets('menu grow opens growth screen and grows selected animal', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );
    session.player.addBond(AnimalElement.nature, 100);

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(find.text('Grow'), findsOneWidget);

    await tester.tap(find.text('Grow'));
    await tester.pumpAndSettle();

    expect(find.text('Grow Animals'), findsOneWidget);
    expect(find.text('N_CAT -> N_LION'), findsOneWidget);
    expect(find.text('N_DOG -> N_WOLF'), findsOneWidget);

    await tester.tap(find.widgetWithText(ElevatedButton, 'Grow'));
    await tester.pumpAndSettle();

    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(find.text('N_CAT grew into N_LION!'), findsOneWidget);
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_LION',
      'N_DOG',
    ]);
    expect(session.player.getBond(AnimalElement.nature), 0);
  });

  // Verifies general party reorder swaps two selected party animals.
  testWidgets('menu reorder party swaps selected animals', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Reorder Party'));
    await tester.pumpAndSettle();

    expect(find.text('Reorder Party'), findsOneWidget);

    await tester.tap(find.widgetWithText(ElevatedButton, 'Swap'));
    await tester.pumpAndSettle();

    expect(find.text('Swapped N_CAT and N_DOG.'), findsOneWidget);
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_DOG',
      'N_CAT',
    ]);
  });

  // Verifies the expanded menu can scroll instead of overflowing on short screens.
  testWidgets('menu controls fit short screens', (WidgetTester tester) async {
    tester.view.devicePixelRatio = 1;
    tester.view.physicalSize = const Size(390, 360);
    addTearDown(tester.view.resetPhysicalSize);
    addTearDown(tester.view.resetDevicePixelRatio);

    final saveRepository = _MemoryGameSaveRepository();
    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );

    await tester.pumpWidget(_buildMapScreen(gameSession: gameSession));

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(tester.takeException(), isNull);
    expect(find.text('Inventory'), findsOneWidget);
    expect(find.text('Return'), findsOneWidget);
  });

  // Verifies Road 8 shows stored animals and allows swapping from the menu.
  testWidgets('road 8 swap exchanges stored and inventory animals', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );
    session.move(RoomDirection.north);
    session.move(RoomDirection.west);
    _fillParty(session);
    final battleState = session.startWildBattle();
    battleState.wildAnimal.health = 0;
    session.catchWildAnimal(battleState);
    session.move(RoomDirection.north);

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(find.textContaining('Stored Animals: N_DOG'), findsOneWidget);

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(find.text('Swap'), findsOneWidget);

    await tester.tap(find.text('Swap'));
    await tester.pumpAndSettle();

    expect(find.text('Swap Animals'), findsOneWidget);
    expect(find.text('Inventory'), findsOneWidget);
    expect(find.text('Road 8 Storage'), findsOneWidget);

    await tester.tap(find.widgetWithText(ElevatedButton, 'Swap'));
    await tester.pumpAndSettle();

    expect(find.text('Road 8'), findsOneWidget);
    expect(find.text('Swapped N_CAT for N_DOG.'), findsOneWidget);
    expect(session.player.animalInventory.map((animal) => animal.name), [
      'N_DOG',
      'N_WOLF',
      'N_HORSE',
      'N_STALLION',
      'N_TURTLE',
      'N_DOG',
    ]);
    expect(session.road8StoredAnimals.map((animal) => animal.name), ['N_CAT']);
  });

  // Verifies final room ending action can keep the player in Arcadia.
  testWidgets('final room ending can keep player in world', (
    WidgetTester tester,
  ) async {
    final session = MobileGameSession(
      GameMap(),
      saveRepository: _MemoryGameSaveRepository(),
    );
    session.restore('Nova', RoomId.theEnd, [RoomId.maiaStable]);
    session.currentRoom.removeEncounterAnimal(
      session.currentRoom.encounterAnimals.single,
    );

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    expect(find.text('Ending'), findsOneWidget);

    await tester.tap(find.text('Ending'));
    await tester.pumpAndSettle();

    expect(find.text('Do you wish to stay in this world?'), findsOneWidget);

    await tester.tap(find.text('Yes'));
    await tester.pumpAndSettle();

    expect(find.text('You are welcome to stay in Arcadia.'), findsOneWidget);
    expect(find.text('The End'), findsOneWidget);
  });

  // Verifies the final ending autosaves before returning to the start menu.
  testWidgets('final room ending autosaves before leaving world', (
    WidgetTester tester,
  ) async {
    final saveRepository = _MemoryGameSaveRepository();
    final session = MobileGameSession(
      GameMap(),
      saveRepository: saveRepository,
    );
    session.restore('Nova', RoomId.theEnd, [RoomId.maiaStable]);
    session.currentRoom.removeEncounterAnimal(
      session.currentRoom.encounterAnimals.single,
    );

    await tester.pumpWidget(_buildMapScreen(gameSession: session));

    await tester.tap(find.text('Ending'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('No'));
    await tester.pumpAndSettle();

    expect(await saveRepository.exists(), isTrue);
    expect(find.text('Load Game'), findsOneWidget);

    await tester.tap(find.text('Load Game'));
    await tester.pumpAndSettle();

    expect(find.text('The End'), findsOneWidget);
    expect(find.text('Ending'), findsOneWidget);
  });

  // Verifies menu inventory displays the player's starter animals.
  testWidgets('menu inventory displays starter animals', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Inventory'));
    await tester.pump();

    expect(
      find.text('Inventory List:\nN_CAT Health: 75\nN_DOG Health: 40'),
      findsOneWidget,
    );
  });

  // Verifies menu bond displays all element bond values.
  testWidgets('menu bond displays element bond values', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Bond'));
    await tester.pump();

    expect(
      find.text(
        'Bond:\nNature 0%/100%\nMystic 0%/100%\nThunder 0%/100%\nDraconic 0%/100%\nCosmic 0%/100%\nNuclear 0%/100%',
      ),
      findsOneWidget,
    );
  });

  // Verifies menu star fragments displays the initial empty state.
  testWidgets('menu star fragments displays empty state', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(_buildMapScreen());

    await tester.tap(find.text('Menu'));
    await tester.pump();
    await tester.tap(find.text('Star Fragments'));
    await tester.pump();

    expect(find.text('You have no star fragments!'), findsOneWidget);
  });

  // Verifies the screen follows a changed injected game session.
  testWidgets('map screen updates when injected session changes', (
    WidgetTester tester,
  ) async {
    final firstSession = MobileGameSession(GameMap());
    final secondSession = MobileGameSession(GameMap())
      ..move(RoomDirection.north);

    await tester.pumpWidget(_buildMapScreen(gameSession: firstSession));

    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(find.text('Ikena'), findsNothing);

    await tester.pumpWidget(_buildMapScreen(gameSession: secondSession));

    expect(find.text('Ikena'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsNothing);
    expect(find.text('The journey begins.'), findsOneWidget);
  });
}

Widget _buildMapScreen({MobileGameSession? gameSession}) {
  return MaterialApp(
    home: ArcadiaMapScreen(
      gameSession:
          gameSession ??
          MobileGameSession(
            GameMap(),
            saveRepository: _MemoryGameSaveRepository(),
          ),
    ),
  );
}

void _fillParty(MobileGameSession session) {
  final animals = GameCreatureData.createAnimals();

  for (final animal in animals.skip(4).take(4)) {
    session.player.addAnimal(animal.clone());
  }
}

class _MemoryGameSaveRepository implements GameSaveRepository {
  GameSaveState? _saveState;

  @override
  Future<bool> exists() async {
    return _saveState != null;
  }

  @override
  Future<GameSaveState?> load() async {
    return _saveState;
  }

  @override
  Future<void> save(GameSaveState saveState) async {
    _saveState = saveState;
  }

  @override
  Future<bool> delete() async {
    final hadSave = _saveState != null;
    _saveState = null;
    return hadSave;
  }
}
