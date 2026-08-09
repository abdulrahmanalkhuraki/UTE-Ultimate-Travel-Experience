import 'package:flutter/material.dart';
import 'dart:math' as math;
import 'dart:ui' as ui;
import '../app_constants.dart';

class TouristsArcWidget extends StatelessWidget {
  final int current;
  final int max;
  const TouristsArcWidget({super.key, required this.current, required this.max});

  @override
  Widget build(BuildContext context) {
    final double ratio = max > 0 ? current / max : 0.0;
    final double sz = 131 * context.scale;
    return SizedBox(
      width: sz, height: sz,
      child: Stack(
        alignment: Alignment.center,
        children: [
          CustomPaint(size: Size(sz, sz), painter: TouristsArcPainter(ratio: ratio)),
          Positioned(
            top: 38 * context.scale,
            child: Text("$current/$max", style: TextStyle(fontFamily: 'Tajawal', fontSize: 18 * context.scale, fontWeight: FontWeight.w600, color: Colors.black)),
          ),
          Positioned(
            top: 62 * context.scale,
            child: Image.asset('assets/icons/persons.png', width: 30 * context.scale, height: 30 * context.scale, fit: BoxFit.contain),
          ),
        ],
      ),
    );
  }
}

class CalendarDaysWidget extends StatelessWidget {
  final int days;
  final String iconPath;
  const CalendarDaysWidget({super.key, required this.days, this.iconPath = 'assets/icons/almanac.png'});

  @override
  Widget build(BuildContext context) {
    return Stack(
      alignment: Alignment.center,
      children: [
        Image.asset(iconPath, width: 82 * context.scale, height: 90 * context.scale, fit: BoxFit.contain),
        Positioned(top: 8 * context.scale, child: Text("يوم", style: TextStyle(fontFamily: 'Tajawal', fontSize: 20 * context.scale, fontWeight: FontWeight.w400, color: Colors.black))),
        Positioned(bottom: 12 * context.scale, child: Text("$days", style: TextStyle(fontFamily: 'Tajawal', fontSize: 32 * context.scale, fontWeight: FontWeight.w400, color: Colors.black))),
      ],
    );
  }
}

class TouristsArcPainter extends CustomPainter {
  final double ratio;
  const TouristsArcPainter({required this.ratio});

  @override
  void paint(Canvas canvas, Size size) {
    final double sx = size.width / 132.0;
    final Offset center = Offset(66 * sx, 62 * (size.height / 131.0));
    final double radius = 41 * sx;
    final double strokeWidth = 10 * sx;
    final Rect arcRect = Rect.fromCircle(center: center, radius: radius);
    const double startAngle = 150.0 * math.pi / 180.0;
    const double totalSweep = 240.0 * math.pi / 180.0;

    canvas.drawArc(arcRect, startAngle, totalSweep, false,
        Paint()..style = PaintingStyle.stroke..strokeWidth = strokeWidth..strokeCap = StrokeCap.round..color = const Color(0xFFF4A261).withValues(alpha: 0.3));

    if (ratio > 0) {
      final double sweepAngle = totalSweep * ratio.clamp(0.0, 1.0);
      canvas.drawArc(arcRect, startAngle, sweepAngle, false,
          Paint()..style = PaintingStyle.stroke..strokeWidth = strokeWidth..strokeCap = StrokeCap.round
            ..shader = ui.Gradient.linear(
              Offset(center.dx + radius * math.cos(startAngle), center.dy + radius * math.sin(startAngle)),
              Offset(center.dx + radius * math.cos(startAngle + sweepAngle), center.dy + radius * math.sin(startAngle + sweepAngle)),
              [const Color(0xFFF4A261).withValues(alpha: 0.5), const Color(0xFFF4A261)],
              [0.0, 1.0],
            ));
    }

    final double dotsRadius = (radius - strokeWidth / 2) - 2 * sx;
    for (int i = 0; i <= 30; i++) {
      final double angle = startAngle + (totalSweep / 30) * i;
      final bool isFilled = ratio > 0 && i <= (30 * ratio).floor();
      canvas.drawCircle(
        Offset(center.dx + dotsRadius * math.cos(angle), center.dy + dotsRadius * math.sin(angle)),
        1.5 * sx,
        Paint()..style = PaintingStyle.fill..color = isFilled ? const Color(0xFFF4A261) : const Color(0xFFF4A261).withValues(alpha: 0.25),
      );
    }
  }

  @override
  bool shouldRepaint(TouristsArcPainter old) => old.ratio != ratio;
}