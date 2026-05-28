import 'package:flutter/material.dart';

import '../creatures/animal.dart';
import '../services/mobile_game_session.dart';

class SwapAnimalScreen extends StatefulWidget {
  const SwapAnimalScreen({super.key, required this.gameSession});

  final MobileGameSession gameSession;

  @override
  State<SwapAnimalScreen> createState() => _SwapAnimalScreenState();
}

class _SwapAnimalScreenState extends State<SwapAnimalScreen> {
  Animal? _selectedPartyAnimal;
  Animal? _selectedStoredAnimal;

  @override
  void initState() {
    super.initState();
    _selectedPartyAnimal =
        widget.gameSession.player.animalInventory.firstOrNull;
    _selectedStoredAnimal = widget.gameSession.road8StoredAnimals.firstOrNull;
  }

  @override
  Widget build(BuildContext context) {
    final partyAnimals = widget.gameSession.player.animalInventory;
    final storedAnimals = widget.gameSession.road8StoredAnimals;

    return Scaffold(
      appBar: AppBar(title: const Text('Swap Animals')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Inventory',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      RadioGroup<Animal>(
                        groupValue: _selectedPartyAnimal,
                        onChanged: (animal) {
                          setState(() {
                            _selectedPartyAnimal = animal;
                          });
                        },
                        child: Column(
                          children: [
                            for (final animal in partyAnimals)
                              RadioListTile<Animal>(
                                title: Text(_formatAnimal(animal)),
                                value: animal,
                                selected: animal == _selectedPartyAnimal,
                              ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 16),
                      Text(
                        'Road 8 Storage',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      RadioGroup<Animal>(
                        groupValue: _selectedStoredAnimal,
                        onChanged: (animal) {
                          setState(() {
                            _selectedStoredAnimal = animal;
                          });
                        },
                        child: Column(
                          children: [
                            for (final animal in storedAnimals)
                              RadioListTile<Animal>(
                                title: Text(_formatAnimal(animal)),
                                value: animal,
                                selected: animal == _selectedStoredAnimal,
                              ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 48,
                child: ElevatedButton(
                  onPressed:
                      _selectedPartyAnimal == null ||
                          _selectedStoredAnimal == null
                      ? null
                      : _swap,
                  child: const Text('Swap'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _swap() {
    final partyAnimal = _selectedPartyAnimal;
    final storedAnimal = _selectedStoredAnimal;

    if (partyAnimal == null || storedAnimal == null) {
      return;
    }

    widget.gameSession.swapStoredAnimal(
      partyAnimal: partyAnimal,
      storedAnimal: storedAnimal,
    );

    Navigator.of(
      context,
    ).pop('Swapped ${partyAnimal.name} for ${storedAnimal.name}.');
  }

  static String _formatAnimal(Animal animal) {
    return '${animal.name} Health: ${animal.health}/${animal.baseHealth}';
  }
}
