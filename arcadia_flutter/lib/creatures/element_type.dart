enum ElementType { base, nature, mystic, thunder, draconic, cosmic, nuclear }

extension ElementTypeLabel on ElementType {
  String get label {
    switch (this) {
      case ElementType.base:
        return 'Base';
      case ElementType.nature:
        return 'Nature';
      case ElementType.mystic:
        return 'Mystic';
      case ElementType.thunder:
        return 'Thunder';
      case ElementType.draconic:
        return 'Draconic';
      case ElementType.cosmic:
        return 'Cosmic';
      case ElementType.nuclear:
        return 'Nuclear';
    }
  }
}
