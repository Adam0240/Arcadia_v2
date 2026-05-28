enum AnimalElement { nature, mystic, thunder, draconic, cosmic, nuclear }

extension AnimalElementLabel on AnimalElement {
  String get label {
    switch (this) {
      case AnimalElement.nature:
        return 'Nature';
      case AnimalElement.mystic:
        return 'Mystic';
      case AnimalElement.thunder:
        return 'Thunder';
      case AnimalElement.draconic:
        return 'Draconic';
      case AnimalElement.cosmic:
        return 'Cosmic';
      case AnimalElement.nuclear:
        return 'Nuclear';
    }
  }
}
