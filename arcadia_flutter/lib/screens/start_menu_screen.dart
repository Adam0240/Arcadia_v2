import 'package:flutter/material.dart';

import '../map/game_map.dart';
import '../saves/game_save_repository.dart';
import '../saves/local_json_game_save_repository.dart';
import '../services/mobile_game_session.dart';
import 'arcadia_map_screen.dart';

class StartMenuScreen extends StatefulWidget {
  const StartMenuScreen({
    super.key,
    this.saveRepository = const LocalJsonGameSaveRepository(),
  });

  final GameSaveRepository saveRepository;

  @override
  State<StartMenuScreen> createState() => _StartMenuScreenState();
}

class _StartMenuScreenState extends State<StartMenuScreen> {
  bool _isCheckingSaveState = true;
  bool _hasSave = false;
  String _statusMessage = '';

  @override
  void initState() {
    super.initState();
    _refreshSaveState();
  }

  Future<void> _refreshSaveState({String? statusMessage}) async {
    setState(() {
      _isCheckingSaveState = true;
      if (statusMessage != null) {
        _statusMessage = statusMessage;
      }
    });

    try {
      final hasSave = await widget.saveRepository.exists();

      if (!mounted) {
        return;
      }

      setState(() {
        _hasSave = hasSave;
        _isCheckingSaveState = false;
      });
    } on Object {
      if (!mounted) {
        return;
      }

      setState(() {
        _hasSave = false;
        _isCheckingSaveState = false;
        _statusMessage = 'Save data could not be checked.';
      });
    }
  }

  Future<void> _startNewGame() async {
    final playerName = await _promptForPlayerName();

    if (playerName == null || !mounted) {
      return;
    }

    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: widget.saveRepository,
    );

    gameSession.startNewGame(playerName);
    _openGame(gameSession);
  }

  Future<String?> _promptForPlayerName() {
    return showDialog<String>(
      context: context,
      builder: (context) => const _PlayerNameDialog(),
    );
  }

  Future<void> _loadGame() async {
    final gameSession = MobileGameSession(
      GameMap(),
      saveRepository: widget.saveRepository,
    );

    try {
      final loaded = await gameSession.loadGame();

      if (!mounted) {
        return;
      }

      if (loaded) {
        _openGame(gameSession);
        return;
      }

      await _refreshSaveState(statusMessage: 'No saved game found.');
    } on Object {
      if (!mounted) {
        return;
      }

      await _refreshSaveState(statusMessage: 'Save data could not be loaded.');
    }
  }

  Future<void> _deleteGame() async {
    try {
      final deleted = await widget.saveRepository.delete();

      if (!mounted) {
        return;
      }

      await _refreshSaveState(
        statusMessage: deleted ? 'Save data deleted.' : 'No save data found.',
      );
    } on Object {
      if (!mounted) {
        return;
      }

      await _refreshSaveState(statusMessage: 'Save data could not be deleted.');
    }
  }

  void _openGame(MobileGameSession gameSession) {
    Navigator.of(context).pushReplacement(
      MaterialPageRoute<void>(
        builder: (_) => ArcadiaMapScreen(gameSession: gameSession),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(28),
          child: Center(
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 420),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    'Arcadia',
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.displayMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 28),
                  if (_isCheckingSaveState)
                    Text(
                      'Checking save data...',
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodyMedium,
                    )
                  else if (_hasSave) ...[
                    _MenuButton(label: 'Load Game', onPressed: _loadGame),
                    const SizedBox(height: 12),
                    _MenuButton(label: 'Delete Game', onPressed: _deleteGame),
                  ] else
                    _MenuButton(label: 'New Game', onPressed: _startNewGame),
                  if (_statusMessage.isNotEmpty) ...[
                    const SizedBox(height: 28),
                    Text(
                      _statusMessage,
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodyMedium,
                    ),
                  ],
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _MenuButton extends StatelessWidget {
  const _MenuButton({required this.label, required this.onPressed});

  final String label;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 52,
      child: ElevatedButton(onPressed: onPressed, child: Text(label)),
    );
  }
}

class _PlayerNameDialog extends StatefulWidget {
  const _PlayerNameDialog();

  @override
  State<_PlayerNameDialog> createState() => _PlayerNameDialogState();
}

class _PlayerNameDialogState extends State<_PlayerNameDialog> {
  final TextEditingController _controller = TextEditingController();
  String? _errorText;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _startGame() {
    final playerName = _controller.text.trim();

    if (playerName.isEmpty) {
      setState(() {
        _errorText = 'Player name cannot be empty.';
      });
      return;
    }

    Navigator.of(context).pop(playerName);
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('New Game'),
      content: TextField(
        controller: _controller,
        autofocus: true,
        decoration: InputDecoration(
          labelText: 'Player Name',
          errorText: _errorText,
        ),
        onSubmitted: (_) => _startGame(),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        FilledButton(onPressed: _startGame, child: const Text('Start')),
      ],
    );
  }
}
