import 'dart:io';

import 'package:image/image.dart' as img;

const defaultInputDirectory = 'assets/sprites/player';
const defaultOutputFileName = 'cosmic_cat_overworld.png';
const defaultFrameSize = 256;
const defaultSourceColumns = 4;
const defaultSourceRows = 4;
const expectedAnimationRows = 6;

const defaultSourceFileNames = [
  'cosmic_cat_iso_idle_down_v1.png',
  'cosmic_cat_iso_idle_right_v1.png',
  'cosmic_cat_iso_idle_up_v1.png',
  'cosmic_cat_iso_walk_down_v1.png',
  'cosmic_cat_iso_walk_right_v1.png',
  'cosmic_cat_iso_walk_up_v1.png',
];

Future<void> main(List<String> arguments) async {
  try {
    final options = _parseArguments(arguments);
    if (options.showHelp) {
      stdout.write(_usage);
      return;
    }

    final inputDirectory = Directory(options.inputDirectory);
    final outputFile = File(options.outputPath);
    final sourceFiles = options.sourceFileNames
        .map(
          (name) =>
              File('${inputDirectory.path}${Platform.pathSeparator}$name'),
        )
        .toList(growable: false);

    await combineOverworldSpriteSheets(
      sourceFiles: sourceFiles,
      outputFile: outputFile,
      frameSize: options.frameSize,
      sourceColumns: options.sourceColumns,
      sourceRows: options.sourceRows,
      onProgress: stderr.writeln,
    );

    stdout.writeln(outputFile.absolute.path);
  } on Object catch (error) {
    stderr.writeln('Failed to combine overworld sprites: $error');
    exitCode = 1;
  }
}

Future<File> combineOverworldSpriteSheets({
  required List<File> sourceFiles,
  required File outputFile,
  int frameSize = defaultFrameSize,
  int sourceColumns = defaultSourceColumns,
  int sourceRows = defaultSourceRows,
  void Function(String message)? onProgress,
}) async {
  final sourceImages = <img.Image>[];
  for (final sourceFile in sourceFiles) {
    if (!await sourceFile.exists()) {
      throw ArgumentError('Missing input file: ${sourceFile.absolute.path}');
    }

    onProgress?.call('Decoding ${sourceFile.absolute.path}');
    final decoded = img.decodePng(await sourceFile.readAsBytes());
    if (decoded == null) {
      throw FormatException(
        'Unable to decode PNG: ${sourceFile.absolute.path}',
      );
    }
    sourceImages.add(decoded);
  }

  onProgress?.call('Combining ${sourceImages.length} source sheets');
  final combined = combineOverworldImages(
    sourceImages,
    frameSize: frameSize,
    sourceColumns: sourceColumns,
    sourceRows: sourceRows,
  );
  await outputFile.parent.create(recursive: true);
  onProgress?.call('Encoding ${outputFile.absolute.path}');
  await outputFile.writeAsBytes(img.encodePng(combined), flush: true);
  return outputFile;
}

img.Image combineOverworldImages(
  List<img.Image> sourceImages, {
  int frameSize = defaultFrameSize,
  int sourceColumns = defaultSourceColumns,
  int sourceRows = defaultSourceRows,
}) {
  if (sourceImages.length != expectedAnimationRows) {
    throw ArgumentError(
      'Exactly $expectedAnimationRows source images are required.',
    );
  }
  if (frameSize <= 0 || sourceColumns <= 0 || sourceRows <= 0) {
    throw ArgumentError('Frame size and grid dimensions must be positive.');
  }

  final expectedWidth = frameSize * sourceColumns;
  final expectedHeight = frameSize * sourceRows;
  final framesPerSource = sourceColumns * sourceRows;
  for (var index = 0; index < sourceImages.length; index++) {
    final source = sourceImages[index];
    if (source.width != expectedWidth || source.height != expectedHeight) {
      throw ArgumentError(
        'Source ${index + 1} must be ${expectedWidth}x$expectedHeight, '
        'but was ${source.width}x${source.height}.',
      );
    }
  }

  final output = img.Image(
    width: frameSize * framesPerSource,
    height: frameSize * sourceImages.length,
    numChannels: 4,
  );

  for (var outputRow = 0; outputRow < sourceImages.length; outputRow++) {
    final source = sourceImages[outputRow];
    for (var frameIndex = 0; frameIndex < framesPerSource; frameIndex++) {
      final sourceColumn = frameIndex % sourceColumns;
      final sourceRow = frameIndex ~/ sourceColumns;
      img.compositeImage(
        output,
        source,
        dstX: frameIndex * frameSize,
        dstY: outputRow * frameSize,
        dstW: frameSize,
        dstH: frameSize,
        srcX: sourceColumn * frameSize,
        srcY: sourceRow * frameSize,
        srcW: frameSize,
        srcH: frameSize,
        blend: img.BlendMode.direct,
      );
    }
  }

  return output;
}

_CombineOptions _parseArguments(List<String> arguments) {
  final values = <String, String>{};
  var showHelp = false;

  for (var index = 0; index < arguments.length; index++) {
    final argument = arguments[index];
    if (argument == '--help' || argument == '-h') {
      showHelp = true;
      continue;
    }
    if (!argument.startsWith('--')) {
      throw ArgumentError('Unexpected argument: $argument');
    }

    final equalsIndex = argument.indexOf('=');
    if (equalsIndex > 2) {
      values[argument.substring(2, equalsIndex)] = argument.substring(
        equalsIndex + 1,
      );
      continue;
    }
    if (index + 1 >= arguments.length ||
        arguments[index + 1].startsWith('--')) {
      throw ArgumentError('Missing value for $argument');
    }
    values[argument.substring(2)] = arguments[++index];
  }

  const sourceKeys = [
    'idle-down',
    'idle-side',
    'idle-up',
    'walk-down',
    'walk-side',
    'walk-up',
  ];
  final knownKeys = {
    'input-dir',
    'output',
    'frame-size',
    'source-columns',
    'source-rows',
    ...sourceKeys,
  };
  final unknownKeys = values.keys.where((key) => !knownKeys.contains(key));
  if (unknownKeys.isNotEmpty) {
    throw ArgumentError('Unknown option: --${unknownKeys.first}');
  }

  final inputDirectory = values['input-dir'] ?? defaultInputDirectory;
  return _CombineOptions(
    showHelp: showHelp,
    inputDirectory: inputDirectory,
    outputPath:
        values['output'] ??
        '$inputDirectory${Platform.pathSeparator}$defaultOutputFileName',
    frameSize: _parsePositiveInt(values, 'frame-size', defaultFrameSize),
    sourceColumns: _parsePositiveInt(
      values,
      'source-columns',
      defaultSourceColumns,
    ),
    sourceRows: _parsePositiveInt(values, 'source-rows', defaultSourceRows),
    sourceFileNames: [
      for (var index = 0; index < sourceKeys.length; index++)
        values[sourceKeys[index]] ?? defaultSourceFileNames[index],
    ],
  );
}

int _parsePositiveInt(Map<String, String> values, String key, int fallback) {
  final value = values[key];
  if (value == null) {
    return fallback;
  }
  final parsed = int.tryParse(value);
  if (parsed == null || parsed <= 0) {
    throw ArgumentError('--$key must be a positive integer.');
  }
  return parsed;
}

class _CombineOptions {
  const _CombineOptions({
    required this.showHelp,
    required this.inputDirectory,
    required this.outputPath,
    required this.frameSize,
    required this.sourceColumns,
    required this.sourceRows,
    required this.sourceFileNames,
  });

  final bool showHelp;
  final String inputDirectory;
  final String outputPath;
  final int frameSize;
  final int sourceColumns;
  final int sourceRows;
  final List<String> sourceFileNames;
}

const _usage = '''
Combine six directional animation atlases into one overworld sprite sheet.

Usage:
  dart run tool/combine_overworld_sprites.dart [options]

Options:
  --input-dir PATH       Directory containing source sheets.
  --output PATH          Combined PNG output path.
  --idle-down FILE       Idle-down source filename.
  --idle-side FILE       Idle-right/side source filename.
  --idle-up FILE         Idle-up source filename.
  --walk-down FILE       Walk-down source filename.
  --walk-side FILE       Walk-right/side source filename.
  --walk-up FILE         Walk-up source filename.
  --frame-size NUMBER    Frame width and height. Default: 256.
  --source-columns NUM   Columns in each source atlas. Default: 4.
  --source-rows NUM      Rows in each source atlas. Default: 4.
  --help, -h             Show this help.
''';
