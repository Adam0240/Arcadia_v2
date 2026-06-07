import 'package:flutter_test/flutter_test.dart';
import 'package:image/image.dart' as img;

import '../../tool/combine_overworld_sprites.dart';

void main() {
  test('combines source frames into row-major animation rows', () {
    const frameSize = 2;
    const sourceColumns = 2;
    const sourceRows = 2;
    final sources = List.generate(6, (sourceIndex) {
      final source = img.Image(width: 4, height: 4, numChannels: 4);
      for (var frameIndex = 0; frameIndex < 4; frameIndex++) {
        final red = sourceIndex * 20 + frameIndex;
        final alpha = 100 + sourceIndex * 10 + frameIndex;
        final frame = img.Image(
          width: frameSize,
          height: frameSize,
          numChannels: 4,
        )..clear(img.ColorRgba8(red, 30, 40, alpha));
        img.compositeImage(
          source,
          frame,
          dstX: (frameIndex % sourceColumns) * frameSize,
          dstY: (frameIndex ~/ sourceColumns) * frameSize,
          blend: img.BlendMode.direct,
        );
      }
      return source;
    });

    final output = combineOverworldImages(
      sources,
      frameSize: frameSize,
      sourceColumns: sourceColumns,
      sourceRows: sourceRows,
    );

    expect(output.width, 8);
    expect(output.height, 12);
    expect(output.numChannels, 4);
    for (var sourceIndex = 0; sourceIndex < sources.length; sourceIndex++) {
      for (var frameIndex = 0; frameIndex < 4; frameIndex++) {
        final pixel = output.getPixel(
          frameIndex * frameSize,
          sourceIndex * frameSize,
        );
        expect(pixel.r, sourceIndex * 20 + frameIndex);
        expect(pixel.g, 30);
        expect(pixel.b, 40);
        expect(pixel.a, 100 + sourceIndex * 10 + frameIndex);
      }
    }
  });

  test('rejects source sheets with unexpected dimensions', () {
    final sources = List.generate(
      expectedAnimationRows,
      (index) => img.Image(width: index == 0 ? 3 : 4, height: 4),
    );

    expect(
      () => combineOverworldImages(
        sources,
        frameSize: 2,
        sourceColumns: 2,
        sourceRows: 2,
      ),
      throwsArgumentError,
    );
  });

  test('requires all six animation rows', () {
    expect(
      () => combineOverworldImages([img.Image(width: 4, height: 4)]),
      throwsArgumentError,
    );
  });
}
