import 'game_save_state.dart';

abstract class GameSaveRepository {
  Future<void> save(GameSaveState saveState);
  Future<GameSaveState?> load();
  Future<bool> exists();
  Future<bool> delete();
}
