import 'dart:io';

import 'package:path_provider/path_provider.dart';

import 'game_save_repository.dart';
import 'game_save_state.dart';
import 'json_game_save_repository.dart';

class LocalJsonGameSaveRepository implements GameSaveRepository {
  const LocalJsonGameSaveRepository({this.fileName = 'arcadia_save.json'});

  final String fileName;

  @override
  Future<bool> exists() async {
    return (await _repository()).exists();
  }

  @override
  Future<GameSaveState?> load() async {
    return (await _repository()).load();
  }

  @override
  Future<void> save(GameSaveState saveState) async {
    await (await _repository()).save(saveState);
  }

  @override
  Future<bool> delete() async {
    return (await _repository()).delete();
  }

  Future<JsonGameSaveRepository> _repository() async {
    final directory = await getApplicationDocumentsDirectory();
    final separator = Platform.pathSeparator;

    return JsonGameSaveRepository(File('${directory.path}$separator$fileName'));
  }
}
