import '../saves/game_save_repository.dart';
import '../saves/game_save_state.dart';

class GameSessionPersistence {
  const GameSessionPersistence(this._repository);

  final GameSaveRepository _repository;

  Future<void> save(GameSaveState saveState) {
    return _repository.save(saveState);
  }

  Future<GameSaveState?> load() {
    return _repository.load();
  }

  Future<bool> exists() {
    return _repository.exists();
  }

  Future<bool> delete() {
    return _repository.delete();
  }
}
