import 'package:flutter/material.dart';
import 'dart:ui';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [

         //Blob left
          Positioned(
            top: -450,
            left: -287,
            child: CustomPaint(
              size: const Size(705, 678),
              painter: _LeftBlobPainter(),
            ),
          ),

        //Blob Right
          Positioned(
            top: -10,
            left: 230,
            child: CustomPaint(
              size: const Size(705, 651),
              painter: _RightBlobPainter(),
            ),
          ),

        // circul yellow
          Positioned(
            top: 149 - 42,
            left: 69 - 45,
            child: Container(
              width: 90,
              height: 84,
              decoration: const BoxDecoration(
                color: Color(0xFFF4A261),
                shape: BoxShape.circle,
              ),
            ),
          ),

         // Bell 
          Positioned(
            top: 125,
            left: 395,
            child: CustomPaint(
              size: const Size(22, 24),
              painter: _BellPainter(),
            ),
          ),

      //  down  
          Positioned(
            top: 652,
            left: 10,
            child: ClipRRect(
              borderRadius: BorderRadius.circular(70),
              child: BackdropFilter(
                filter: ImageFilter.blur(sigmaX: 5, sigmaY: 5),
                child: Container(
                  width: 420,
                  height: 703,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(70),
                    gradient: const LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      stops: [0.0, 0.25],
                      colors: [
                        Color(0xCC91B3FA), // opacity 0.8
                        Color(0x2991B3FA), // opacity 0.16
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

 class _LeftBlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = const Color(0xFFC8D9FD);

    const double minX = -283.0;
    const double minY = -411.0;
    const double rangeX = 701.0;
    const double rangeY = 675.0;

    double tx(double x) => (x - minX) * (size.width / rangeX);
    double ty(double y) => (y - minY) * (size.height / rangeY);

    final path = Path();
    path.moveTo(tx(64.0847), ty(-410.926));
    path.cubicTo(tx(150.329), ty(-392.25),   tx(164.109), ty(-278.666), tx(226.041), ty(-216.45));
    path.cubicTo(tx(286.92),  ty(-155.293),  tx(406.209), ty(-141.45),  tx(417.08),  ty(-56.3145));
    path.cubicTo(tx(428.095), ty(29.9397),   tx(337.807), ty(91.5091),  tx(272.85),  ty(150.104));
    path.cubicTo(tx(212.047), ty(204.952),   tx(145.84),  ty(271.495),  tx(64.0847), ty(262.108));
    path.cubicTo(tx(-14.326), ty(253.105),   tx(-43.3916),ty(160.257),  tx(-101.239),ty(107.151));
    path.cubicTo(tx(-163.588),ty(49.9117),   tx(-265.344),ty(26.0272),  tx(-282.764),ty(-56.3145));
    path.cubicTo(tx(-301.901),ty(-146.77),   tx(-253.678),ty(-239.831), tx(-188.702),ty(-306.26));
    path.cubicTo(tx(-122.683),ty(-373.757),  tx(-28.6832), ty(-431.014),tx(64.0847), ty(-410.926));
    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _RightBlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = const Color(0xFFC8D9FD);

    const double minX = -4.0;
    const double minY = -65.0;
    const double rangeX = 692.0;
    const double rangeY = 716.0;

    double tx(double x) => (x - minX) * (size.width / rangeX);
    double ty(double y) => (y - minY) * (size.height / rangeY);

    final path = Path();
    path.moveTo(tx(425.385), ty(-21.261));
    path.cubicTo(tx(504.102), ty(18.6207),  tx(488.706), ty(131.997),  tx(532.888), ty(207.854));
    path.cubicTo(tx(576.32),  ty(282.421),  tx(688.228), ty(325.985),  tx(677.214), ty(411.102));
    path.cubicTo(tx(666.055), ty(497.337),  tx(563.13),  ty(534.069),  tx(485.465), ty(574.329));
    path.cubicTo(tx(412.766), ty(612.015),  tx(331.882), ty(659.65),   tx(255.159), ty(629.89));
    path.cubicTo(tx(181.575), ty(601.348),  tx(176.938), ty(504.168),  tx(134.403), ty(438.157));
    path.cubicTo(tx(88.558),  ty(367.009),  tx(-3.84794),ty(318.165),  tx(0.123952),ty(234.095));
    path.cubicTo(tx(4.48722), ty(141.74),   tx(74.6805), ty(63.902),   tx(154.344), ty(16.0663));
    path.cubicTo(tx(235.289), ty(-32.5384), tx(340.714), ty(-64.1592), tx(425.385), ty(-21.261));
    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _BellPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final fillPaint = Paint()
      ..color = const Color(0xFFF4A261)
      ..style = PaintingStyle.fill;

    final strokePaint = Paint()
      ..color = const Color(0xFF2D264B)
      ..strokeWidth = 1.5
      ..style = PaintingStyle.stroke
      ..strokeCap = StrokeCap.round;

    double tx(double x) => (x - 393.0) * (size.width / 20.0);
    double ty(double y) => (y - 125.0) * (size.height / 26.0);

    final bodyPath = Path();
    bodyPath.moveTo(tx(395.803), ty(132.378));
    bodyPath.cubicTo(tx(395.347), ty(128.726), tx(398.194), ty(125.5), tx(401.875), ty(125.5));
    bodyPath.lineTo(tx(404.004), ty(125.5));
    bodyPath.cubicTo(tx(407.689), ty(125.5), tx(410.54), ty(128.73), tx(410.083), ty(132.386));
    bodyPath.lineTo(tx(410.002), ty(133.032));
    bodyPath.cubicTo(tx(409.808), ty(134.582), tx(410.372), ty(136.13), tx(411.516), ty(137.192));
    bodyPath.lineTo(tx(411.571), ty(137.243));
    bodyPath.cubicTo(tx(413.256), ty(138.808), tx(413.583), ty(141.353), tx(412.348), ty(143.293));
    bodyPath.cubicTo(tx(411.473), ty(144.668), tx(409.957), ty(145.5), tx(408.328), ty(145.5));
    bodyPath.lineTo(tx(398.052), ty(145.5));
    bodyPath.cubicTo(tx(396.151), ty(145.5), tx(394.417), ty(144.414), tx(393.587), ty(142.703));
    bodyPath.lineTo(tx(393.458), ty(142.437));
    bodyPath.cubicTo(tx(392.58), ty(140.627), tx(392.991), ty(138.456), tx(394.469), ty(137.091));
    bodyPath.cubicTo(tx(395.551), ty(136.093), tx(396.085), ty(134.633), tx(395.902), ty(133.171));
    bodyPath.close();

    canvas.drawPath(bodyPath, fillPaint);
    canvas.drawPath(bodyPath, strokePaint);

    final tailPath = Path();
    tailPath.moveTo(tx(398.229), ty(145.5));
    tailPath.lineTo(tx(398.729), ty(146.357));
    tailPath.cubicTo(tx(400.979), ty(150.214), tx(405.479), ty(150.214), tx(407.729), ty(146.357));
    tailPath.lineTo(tx(408.229), ty(145.5));
    canvas.drawPath(tailPath, strokePaint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}