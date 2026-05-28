import 'package:flutter/material.dart';

import 'saves/game_save_repository.dart';
import 'saves/local_json_game_save_repository.dart';
import 'screens/start_menu_screen.dart';

void main() {
  runApp(const ArcadiaApp());
}

class ArcadiaApp extends StatelessWidget {
  const ArcadiaApp({super.key, this.saveRepository});

  final GameSaveRepository? saveRepository;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Arcadia',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.green),
        useMaterial3: true,
      ),
      home: StartMenuScreen(
        saveRepository: saveRepository ?? const LocalJsonGameSaveRepository(),
      ),
    );
  }
}
