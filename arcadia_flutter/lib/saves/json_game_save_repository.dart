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
    final decodedJson = jsonDecode(content);
    if (decodedJson is! Map) {
      throw const FormatException('Save file root must be a JSON object.');
    }

    return GameSaveState.fromJson(_toJsonObject(decodedJson));
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

  @override
  Future<bool> delete() async {
    if (!await file.exists()) {
      return false;
    }

    await file.delete();
    return true;
  }
}

Map<String, Object?> _toJsonObject(Map<dynamic, dynamic> json) {
  final object = <String, Object?>{};

  for (final entry in json.entries) {
    final key = entry.key;
    if (key is! String) {
      throw const FormatException('Save file object keys must be strings.');
    }

    object[key] = entry.value;
  }

  return object;
}
