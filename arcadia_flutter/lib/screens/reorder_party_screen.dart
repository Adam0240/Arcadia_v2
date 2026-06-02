import 'package:flutter/material.dart';

import '../creatures/animal.dart';
import '../services/mobile_game_session.dart';

class ReorderPartyScreen extends StatefulWidget {
  const ReorderPartyScreen({super.key, required this.gameSession});

  final MobileGameSession gameSession;

  @override
  State<ReorderPartyScreen> createState() => _ReorderPartyScreenState();
}

class _ReorderPartyScreenState extends State<ReorderPartyScreen> {
  Animal? _firstAnimal;
  Animal? _secondAnimal;

  @override
  void initState() {
    super.initState();
    final partyAnimals = widget.gameSession.player.animalInventory;
    _firstAnimal = partyAnimals.isEmpty ? null : partyAnimals.first;
    _secondAnimal = partyAnimals.length < 2 ? null : partyAnimals[1];
  }

  @override
  Widget build(BuildContext context) {
    final partyAnimals = widget.gameSession.player.animalInventory;

    return Scaffold(
      appBar: AppBar(title: const Text('Reorder Party')),
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
                        'First Animal',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      _AnimalRadioGroup(
                        partyAnimals: partyAnimals,
                        selectedAnimal: _firstAnimal,
                        onChanged: (animal) {
                          setState(() {
                            _firstAnimal = animal;
                          });
                        },
                      ),
                      const SizedBox(height: 16),
                      Text(
                        'Second Animal',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      _AnimalRadioGroup(
                        partyAnimals: partyAnimals,
                        selectedAnimal: _secondAnimal,
                        onChanged: (animal) {
                          setState(() {
                            _secondAnimal = animal;
                          });
                        },
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 48,
                child: ElevatedButton(
                  onPressed: _canSwap ? _swap : null,
                  child: const Text('Swap'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  bool get _canSwap {
    return _firstAnimal != null &&
        _secondAnimal != null &&
        _firstAnimal != _secondAnimal;
  }

  void _swap() {
    final firstAnimal = _firstAnimal;
    final secondAnimal = _secondAnimal;
    if (firstAnimal == null || secondAnimal == null) {
      return;
    }

    final partyAnimals = widget.gameSession.player.animalInventory;
    widget.gameSession.player.swapAnimalPositions(
      partyAnimals.indexOf(firstAnimal),
      partyAnimals.indexOf(secondAnimal),
    );

    Navigator.of(
      context,
    ).pop('Swapped ${firstAnimal.name} and ${secondAnimal.name}.');
  }
}

class _AnimalRadioGroup extends StatelessWidget {
  const _AnimalRadioGroup({
    required this.partyAnimals,
    required this.selectedAnimal,
    required this.onChanged,
  });

  final List<Animal> partyAnimals;
  final Animal? selectedAnimal;
  final ValueChanged<Animal?> onChanged;

  @override
  Widget build(BuildContext context) {
    return RadioGroup<Animal>(
      groupValue: selectedAnimal,
      onChanged: onChanged,
      child: Column(
        children: [
          for (final animal in partyAnimals)
            RadioListTile<Animal>(
              title: Text(
                '${animal.name} Health: ${animal.health}/${animal.baseHealth}',
              ),
              value: animal,
              selected: animal == selectedAnimal,
            ),
        ],
      ),
    );
  }
}
