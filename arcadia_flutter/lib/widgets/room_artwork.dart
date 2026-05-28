import 'package:flutter/material.dart';

import '../map/room_id.dart';

class RoomArtwork extends StatelessWidget {
  const RoomArtwork({super.key, required this.roomId, required this.label});

  final RoomId roomId;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Semantics(
      image: true,
      label: label,
      child: ClipRRect(
        borderRadius: BorderRadius.circular(8),
        child: CustomPaint(
          painter: _RoomArtworkPainter(roomId),
          child: const SizedBox(height: 240, width: double.infinity),
        ),
      ),
    );
  }
}

class _RoomArtworkPainter extends CustomPainter {
  _RoomArtworkPainter(this.roomId);

  final RoomId roomId;

  @override
  void paint(Canvas canvas, Size size) {
    canvas.save();
    canvas.scale(size.width / 960, size.height / 540);

    switch (roomId) {
      case RoomId.maiaStable:
        _paintMaiaStable(canvas);
      case RoomId.ikena:
        _paintIkena(canvas);
      case RoomId.road1:
        _paintRoad1(canvas);
      default:
        _paintPlaceholder(canvas);
    }

    canvas.restore();
  }

  @override
  bool shouldRepaint(_RoomArtworkPainter oldDelegate) {
    return oldDelegate.roomId != roomId;
  }

  static Paint _fill(int color) {
    return Paint()
      ..style = PaintingStyle.fill
      ..color = Color(color);
  }

  static Paint _stroke(int color, double width) {
    return Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = width
      ..strokeCap = StrokeCap.round
      ..color = Color(color);
  }

  static Rect _rect(double left, double top, double width, double height) {
    return Rect.fromLTWH(left, top, width, height);
  }

  static RRect _roundRect(
    double left,
    double top,
    double width,
    double height,
    double radius,
  ) {
    return RRect.fromRectAndRadius(
      _rect(left, top, width, height),
      Radius.circular(radius),
    );
  }

  static void _paintMaiaStable(Canvas canvas) {
    canvas.drawRect(_rect(0, 0, 960, 540), _fill(0xff7fbf8f));
    canvas.drawRect(_rect(0, 315, 960, 225), _fill(0xffd9b37a));
    canvas.drawRRect(_roundRect(225, 165, 510, 250, 8), _fill(0xff8b4f31));
    canvas.drawRect(_rect(190, 145, 580, 70), _fill(0xff5a2e22));

    final roof = Path()
      ..moveTo(180, 145)
      ..lineTo(480, 35)
      ..lineTo(780, 145)
      ..close();
    canvas.drawPath(roof, _fill(0xff6b2f2a));

    canvas.drawRect(_rect(395, 260, 170, 155), _fill(0xff3f2a22));
    canvas.drawRect(_rect(260, 235, 88, 68), _fill(0xfff3d694));
    canvas.drawRect(_rect(612, 235, 88, 68), _fill(0xfff3d694));

    final sky = Path()
      ..moveTo(0, 110)
      ..cubicTo(160, 70, 285, 82, 420, 118)
      ..cubicTo(590, 164, 725, 120, 960, 84)
      ..lineTo(960, 0)
      ..lineTo(0, 0)
      ..close();
    canvas.drawPath(sky, _fill(0xff6aa4c8));
    canvas.drawCircle(const Offset(780, 82), 38, _fill(0xffffd67a));

    final path = Path()
      ..moveTo(250, 430)
      ..cubicTo(340, 405, 420, 406, 492, 430)
      ..cubicTo(580, 460, 690, 455, 790, 420);
    canvas.drawPath(path, _stroke(0xff60432f, 20));
  }

  static void _paintIkena(Canvas canvas) {
    canvas.drawRect(_rect(0, 0, 960, 540), _fill(0xff8ac7d9));
    canvas.drawRect(_rect(0, 310, 960, 230), _fill(0xff73a85f));

    final hill = Path()
      ..moveTo(0, 310)
      ..cubicTo(130, 250, 260, 270, 390, 300)
      ..cubicTo(560, 338, 710, 286, 960, 250)
      ..lineTo(960, 540)
      ..lineTo(0, 540)
      ..close();
    canvas.drawPath(hill, _fill(0xff5f944f));

    canvas.drawRRect(_roundRect(125, 245, 150, 120, 6), _fill(0xffefe1bd));
    final roofOne = Path()
      ..moveTo(105, 245)
      ..lineTo(200, 175)
      ..lineTo(295, 245)
      ..close();
    canvas.drawPath(roofOne, _fill(0xffbd5952));

    canvas.drawRRect(_roundRect(430, 220, 175, 145, 6), _fill(0xffead0a1));
    final roofTwo = Path()
      ..moveTo(405, 220)
      ..lineTo(518, 135)
      ..lineTo(630, 220)
      ..close();
    canvas.drawPath(roofTwo, _fill(0xff426f8f));

    canvas.drawRRect(_roundRect(720, 260, 125, 105, 6), _fill(0xfff0dfc2));
    final roofThree = Path()
      ..moveTo(700, 260)
      ..lineTo(782, 200)
      ..lineTo(865, 260)
      ..close();
    canvas.drawPath(roofThree, _fill(0xffb85a3d));

    canvas.drawRect(_rect(486, 285, 60, 80), _fill(0xff614231));
    final road = Path()
      ..moveTo(120, 430)
      ..cubicTo(260, 390, 365, 402, 470, 430)
      ..cubicTo(595, 464, 702, 455, 850, 402);
    canvas.drawPath(road, _stroke(0xffe8ca8a, 28));
  }

  static void _paintRoad1(Canvas canvas) {
    canvas.drawRect(_rect(0, 0, 960, 540), _fill(0xff6fb1cf));
    canvas.drawRect(_rect(0, 280, 960, 260), _fill(0xff4f8f48));

    final road = Path()
      ..moveTo(410, 540)
      ..cubicTo(430, 455, 455, 385, 480, 320)
      ..cubicTo(505, 385, 530, 455, 550, 540)
      ..close();
    canvas.drawPath(road, _fill(0xffd7bd7a));

    final roadCenter = Path()
      ..moveTo(455, 540)
      ..cubicTo(465, 454, 475, 390, 480, 320)
      ..cubicTo(485, 390, 495, 454, 505, 540)
      ..close();
    canvas.drawPath(roadCenter, _fill(0xffbb9d5d));

    canvas.drawCircle(const Offset(140, 255), 78, _fill(0xff3f7c43));
    canvas.drawRect(_rect(126, 285, 28, 95), _fill(0xff6d4a2f));
    canvas.drawCircle(const Offset(780, 240), 92, _fill(0xff376f3c));
    canvas.drawRect(_rect(763, 290, 34, 110), _fill(0xff65452d));

    final grass = Path()
      ..moveTo(0, 380)
      ..cubicTo(110, 342, 225, 350, 326, 392)
      ..cubicTo(420, 432, 535, 430, 640, 390)
      ..cubicTo(755, 346, 840, 345, 960, 378)
      ..lineTo(960, 540)
      ..lineTo(0, 540)
      ..close();
    canvas.drawPath(grass, _fill(0xff3d793b));

    final leftGrass = Path()
      ..moveTo(90, 455)
      ..cubicTo(215, 415, 330, 422, 430, 462);
    canvas.drawPath(leftGrass, _stroke(0xff91bf62, 20));

    final rightGrass = Path()
      ..moveTo(620, 458)
      ..cubicTo(720, 420, 810, 416, 900, 442);
    canvas.drawPath(rightGrass, _stroke(0xff91bf62, 20));
  }

  static void _paintPlaceholder(Canvas canvas) {
    canvas.drawRect(_rect(0, 0, 960, 540), _fill(0xff6f98a8));

    final farHill = Path()
      ..moveTo(0, 315)
      ..cubicTo(140, 260, 285, 275, 420, 320)
      ..cubicTo(565, 368, 710, 340, 960, 275)
      ..lineTo(960, 540)
      ..lineTo(0, 540)
      ..close();
    canvas.drawPath(farHill, _fill(0xff5d8d62));

    final nearHill = Path()
      ..moveTo(0, 380)
      ..cubicTo(125, 350, 240, 358, 350, 395)
      ..cubicTo(470, 435, 610, 420, 735, 365)
      ..cubicTo(820, 328, 895, 315, 960, 328)
      ..lineTo(960, 540)
      ..lineTo(0, 540)
      ..close();
    canvas.drawPath(nearHill, _fill(0xff486f46));

    final road = Path()
      ..moveTo(390, 540)
      ..cubicTo(420, 455, 445, 382, 480, 305)
      ..cubicTo(515, 382, 540, 455, 570, 540)
      ..close();
    canvas.drawPath(road, _fill(0xffd8bf7d));

    final roadCenter = Path()
      ..moveTo(455, 540)
      ..cubicTo(465, 455, 474, 390, 480, 305)
      ..cubicTo(486, 390, 495, 455, 505, 540)
      ..close();
    canvas.drawPath(roadCenter, _fill(0xffbca061));

    canvas.drawCircle(const Offset(175, 245), 72, _fill(0xff3e7345));
    canvas.drawRect(_rect(162, 285, 26, 95), _fill(0xff65442d));
    canvas.drawCircle(const Offset(760, 225), 88, _fill(0xff3c6e45));
    canvas.drawRect(_rect(744, 280, 32, 112), _fill(0xff65442d));

    final mountain = Path()
      ..moveTo(330, 220)
      ..lineTo(480, 105)
      ..lineTo(630, 220)
      ..close();
    canvas.drawPath(mountain, _fill(0xdd6d4f7d));

    final building = Path()
      ..moveTo(330, 220)
      ..lineTo(630, 220)
      ..lineTo(585, 305)
      ..lineTo(375, 305)
      ..close();
    canvas.drawPath(building, _fill(0xeeead0a1));

    final path = Path()
      ..moveTo(360, 430)
      ..cubicTo(430, 405, 520, 405, 600, 430);
    canvas.drawPath(path, _stroke(0xffeee0ad, 18));
  }
}
