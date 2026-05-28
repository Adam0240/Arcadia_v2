import 'dart:convert';
import 'dart:io';

import 'game_save_repository.dart';
import 'game_save_state.dart';

class JsonGameSaveRepository implements GameSaveRepository {
  const JsonGameSaveRepository(this.file);

  final File file;

  @override
  Future<bool> exists() {
    return file.exists();
  }

  @override
  Future<GameSaveState?> load() async {
    if (!await file.exists()) {
      return null;
    }

    final content = await file.readAsString();
    final json = Map<String, Object?>.from(jsonDecode(content) as Map);

    return GameSaveState.fromJson(json);
  }

  @override
  Future<void> save(GameSaveState saveState) async {
    final parent = file.parent;

    if (!await parent.exists()) {
      await parent.create(recursive: true);
    }

    const encoder = JsonEncoder.withIndent('  ');
    await file.writeAsString(encoder.convert(saveState.toJson()));
  }
}
