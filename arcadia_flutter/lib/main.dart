import 'package:flutter/material.dart';

import 'screens/arcadia_map_screen.dart';

void main() {
  runApp(const ArcadiaApp());
}

class ArcadiaApp extends StatelessWidget {
  const ArcadiaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Arcadia',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.green),
        useMaterial3: true,
      ),
      home: const ArcadiaMapScreen(),
    );
  }
}
