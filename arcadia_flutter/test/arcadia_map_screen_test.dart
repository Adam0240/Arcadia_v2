import 'package:arcadia_flutter/main.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  // Verifies the Flutter screen renders the same initial map content as MAUI.
  testWidgets('map screen shows initial room and movement controls', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    expect(find.text('Arcadia'), findsOneWidget);
    expect(find.text("Maia's Stable"), findsOneWidget);
    expect(
      find.text('Where new trainers obtain their first creature!'),
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
    await tester.pumpWidget(const ArcadiaApp());

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
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Inspect'));
    await tester.pump();

    expect(
      find.text(
        'Maia checks the starter pens and says the stable is ready for your journey.',
      ),
      findsOneWidget,
    );
  });

  // Verifies menu swaps the movement grid for Save and Return controls.
  testWidgets('menu toggles between movement and save controls', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ArcadiaApp());

    await tester.tap(find.text('Menu'));
    await tester.pump();

    expect(find.text('Save'), findsOneWidget);
    expect(find.text('Return'), findsOneWidget);
    expect(find.text('North'), findsNothing);

    await tester.tap(find.text('Save'));
    await tester.pump();
    expect(find.text('Game saved.'), findsOneWidget);

    await tester.tap(find.text('Return'));
    await tester.pump();
    expect(find.text('North'), findsOneWidget);
    expect(find.text('Save'), findsNothing);
  });
}
