import 'package:flutter/material.dart';

import '../services/mobile_game_session.dart';
import 'arcadia_map_screen.dart';

class IntroStoryScreen extends StatelessWidget {
  const IntroStoryScreen({super.key, required this.gameSession});

  final MobileGameSession gameSession;

  static const List<String> _storyLines = [
    'One night, a storm tears through your old life in a flash of light and thunder.',
    'You wake in Arcadia, watched over by Professor Aracia, with only fragments of memory.',
    'Two creatures appear beside you, familiar even though you cannot remember why.',
    'A note says they have always been by your side, and that your goal is to become the best.',
    'Professor Aracia points you toward the sanctuaries, the Elemental Titan, and the long road ahead.',
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Welcome to Arcadia')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      for (final line in _storyLines) ...[
                        Text(
                          line,
                          style: Theme.of(context).textTheme.bodyLarge,
                        ),
                        const SizedBox(height: 16),
                      ],
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 52,
                child: FilledButton(
                  onPressed: () {
                    Navigator.of(context).pushReplacement(
                      MaterialPageRoute<void>(
                        builder: (_) =>
                            ArcadiaMapScreen(gameSession: gameSession),
                      ),
                    );
                  },
                  child: const Text('Begin Journey'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
