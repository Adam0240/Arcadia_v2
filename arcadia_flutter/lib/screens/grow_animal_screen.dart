import 'package:flutter/material.dart';

import '../creatures/animal_growth_catalog.dart';
import '../services/mobile_game_session.dart';

class GrowAnimalScreen extends StatefulWidget {
  const GrowAnimalScreen({super.key, required this.gameSession});

  final MobileGameSession gameSession;

  @override
  State<GrowAnimalScreen> createState() => _GrowAnimalScreenState();
}

class _GrowAnimalScreenState extends State<GrowAnimalScreen> {
  AnimalGrowthOption? _selectedOption;

  @override
  void initState() {
    super.initState();
    _selectedOption = widget.gameSession.growthOptions.firstOrNull;
  }

  @override
  Widget build(BuildContext context) {
    final growthOptions = widget.gameSession.growthOptions;

    return Scaffold(
      appBar: AppBar(title: const Text('Grow Animals')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: SingleChildScrollView(
                  child: RadioGroup<AnimalGrowthOption>(
                    groupValue: _selectedOption,
                    onChanged: (option) {
                      setState(() {
                        _selectedOption = option;
                      });
                    },
                    child: Column(
                      children: [
                        for (final option in growthOptions)
                          RadioListTile<AnimalGrowthOption>(
                            title: Text(
                              '${option.currentAnimal.name} -> ${option.adultAnimal.name}',
                            ),
                            subtitle: Text(
                              'Health: ${option.adultAnimal.baseHealth}',
                            ),
                            value: option,
                            selected: option == _selectedOption,
                          ),
                      ],
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 48,
                child: ElevatedButton(
                  onPressed: _selectedOption == null ? null : _grow,
                  child: const Text('Grow'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _grow() {
    final option = _selectedOption;
    if (option == null) {
      return;
    }

    final resultMessage = widget.gameSession.growAnimal(option);
    Navigator.of(context).pop(resultMessage);
  }
}
