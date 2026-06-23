import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_svg/flutter_svg.dart';

class BlobConstants {
  BlobConstants._();

  static const double minX = -68.1119;
  static const double minY = -22.7061;
  static const double bboxWidth = 692.07;
  static const double bboxHeight = 723.81;
  static const int blobColor = 0x8091B3FA;
}

class LeftBlobConstants {
  LeftBlobConstants._();

  static const double minX = -283.0;
  static const double minY = -411.0;
  static const double rangeX = 701.0;
  static const double rangeY = 675.0;
  static const int blobColor = 0xFFC8D9FD;
}

class RightBlobConstants {
  RightBlobConstants._();

  static const double minX = -4.0;
  static const double minY = -65.0;
  static const double rangeX = 692.0;
  static const double rangeY = 716.0;
  static const int blobColor = 0xFFC8D9FD;
}

class BlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = const Color(BlobConstants.blobColor)
      ..style = PaintingStyle.fill;

    Offset p(double x, double y) => Offset(
      (x - BlobConstants.minX) * (size.width / BlobConstants.bboxWidth),
      (y - BlobConstants.minY) * (size.height / BlobConstants.bboxHeight),
    );

    final path = Path();
    path.moveTo(p(361.121, 20.1921).dx, p(361.121, 20.1921).dy);
    path.cubicTo(
      p(439.838, 60.0738).dx,
      p(439.838, 60.0738).dy,
      p(424.442, 173.450).dx,
      p(424.442, 173.450).dy,
      p(468.624, 249.307).dx,
      p(468.624, 249.307).dy,
    );
    path.cubicTo(
      p(512.056, 323.874).dx,
      p(512.056, 323.874).dy,
      p(623.965, 367.438).dx,
      p(623.965, 367.438).dy,
      p(612.950, 452.555).dx,
      p(612.950, 452.555).dy,
    );
    path.cubicTo(
      p(601.791, 538.790).dx,
      p(601.791, 538.790).dy,
      p(498.866, 575.522).dx,
      p(498.866, 575.522).dy,
      p(421.201, 615.782).dx,
      p(421.201, 615.782).dy,
    );
    path.cubicTo(
      p(348.502, 653.468).dx,
      p(348.502, 653.468).dy,
      p(267.619, 701.103).dx,
      p(267.619, 701.103).dy,
      p(190.895, 671.343).dx,
      p(190.895, 671.343).dy,
    );
    path.cubicTo(
      p(117.311, 642.801).dx,
      p(117.311, 642.801).dy,
      p(112.674, 545.621).dx,
      p(112.674, 545.621).dy,
      p(70.139, 479.610).dx,
      p(70.139, 479.610).dy,
    );
    path.cubicTo(
      p(24.294, 408.462).dx,
      p(24.294, 408.462).dy,
      p(-68.112, 359.618).dx,
      p(-68.112, 359.618).dy,
      p(-64.140, 275.548).dx,
      p(-64.140, 275.548).dy,
    );
    path.cubicTo(
      p(-59.777, 183.193).dx,
      p(-59.777, 183.193).dy,
      p(10.417, 105.355).dx,
      p(10.417, 105.355).dy,
      p(90.081, 57.519).dx,
      p(90.081, 57.519).dy,
    );
    path.cubicTo(
      p(171.025, 8.9147).dx,
      p(171.025, 8.9147).dy,
      p(276.450, -22.7061).dx,
      p(276.450, -22.7061).dy,
      p(361.121, 20.1921).dx,
      p(361.121, 20.1921).dy,
    );
    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class LeftBlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = const Color(LeftBlobConstants.blobColor);

    double tx(double x) =>
        (x - LeftBlobConstants.minX) * (size.width / LeftBlobConstants.rangeX);
    double ty(double y) =>
        (y - LeftBlobConstants.minY) * (size.height / LeftBlobConstants.rangeY);

    final path = Path();
    path.moveTo(tx(64.0847), ty(-410.926));
    path.cubicTo(
      tx(150.329),
      ty(-392.25),
      tx(164.109),
      ty(-278.666),
      tx(226.041),
      ty(-216.45),
    );
    path.cubicTo(
      tx(286.92),
      ty(-155.293),
      tx(406.209),
      ty(-141.45),
      tx(417.08),
      ty(-56.3145),
    );
    path.cubicTo(
      tx(428.095),
      ty(29.9397),
      tx(337.807),
      ty(91.5091),
      tx(272.85),
      ty(150.104),
    );
    path.cubicTo(
      tx(212.047),
      ty(204.952),
      tx(145.84),
      ty(271.495),
      tx(64.0847),
      ty(262.108),
    );
    path.cubicTo(
      tx(-14.326),
      ty(253.105),
      tx(-43.3916),
      ty(160.257),
      tx(-101.239),
      ty(107.151),
    );
    path.cubicTo(
      tx(-163.588),
      ty(49.9117),
      tx(-265.344),
      ty(26.0272),
      tx(-282.764),
      ty(-56.3145),
    );
    path.cubicTo(
      tx(-301.901),
      ty(-146.77),
      tx(-253.678),
      ty(-239.831),
      tx(-188.702),
      ty(-306.26),
    );
    path.cubicTo(
      tx(-122.683),
      ty(-373.757),
      tx(-28.6832),
      ty(-431.014),
      tx(64.0847),
      ty(-410.926),
    );
    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class RightBlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = const Color(RightBlobConstants.blobColor);

    double tx(double x) =>
        (x - RightBlobConstants.minX) *
        (size.width / RightBlobConstants.rangeX);
    double ty(double y) =>
        (y - RightBlobConstants.minY) *
        (size.height / RightBlobConstants.rangeY);

    final path = Path();
    path.moveTo(tx(425.385), ty(-21.261));
    path.cubicTo(
      tx(504.102),
      ty(18.6207),
      tx(488.706),
      ty(131.997),
      tx(532.888),
      ty(207.854),
    );
    path.cubicTo(
      tx(576.32),
      ty(282.421),
      tx(688.228),
      ty(325.985),
      tx(677.214),
      ty(411.102),
    );
    path.cubicTo(
      tx(666.055),
      ty(497.337),
      tx(563.13),
      ty(534.069),
      tx(485.465),
      ty(574.329),
    );
    path.cubicTo(
      tx(412.766),
      ty(612.015),
      tx(331.882),
      ty(659.65),
      tx(255.159),
      ty(629.89),
    );
    path.cubicTo(
      tx(181.575),
      ty(601.348),
      tx(176.938),
      ty(504.168),
      tx(134.403),
      ty(438.157),
    );
    path.cubicTo(
      tx(88.558),
      ty(367.009),
      tx(-3.84794),
      ty(318.165),
      tx(0.123952),
      ty(234.095),
    );
    path.cubicTo(
      tx(4.48722),
      ty(141.74),
      tx(74.6805),
      ty(63.902),
      tx(154.344),
      ty(16.0663),
    );
    path.cubicTo(
      tx(235.289),
      ty(-32.5384),
      tx(340.714),
      ty(-64.1592),
      tx(425.385),
      ty(-21.261),
    );
    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class BellPainter extends CustomPainter {
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
    bodyPath.cubicTo(
      tx(395.347),
      ty(128.726),
      tx(398.194),
      ty(125.5),
      tx(401.875),
      ty(125.5),
    );
    bodyPath.lineTo(tx(404.004), ty(125.5));
    bodyPath.cubicTo(
      tx(407.689),
      ty(125.5),
      tx(410.54),
      ty(128.73),
      tx(410.083),
      ty(132.386),
    );
    bodyPath.lineTo(tx(410.002), ty(133.032));
    bodyPath.cubicTo(
      tx(409.808),
      ty(134.582),
      tx(410.372),
      ty(136.13),
      tx(411.516),
      ty(137.192),
    );
    bodyPath.lineTo(tx(411.571), ty(137.243));
    bodyPath.cubicTo(
      tx(413.256),
      ty(138.808),
      tx(413.583),
      ty(141.353),
      tx(412.348),
      ty(143.293),
    );
    bodyPath.cubicTo(
      tx(411.473),
      ty(144.668),
      tx(409.957),
      ty(145.5),
      tx(408.328),
      ty(145.5),
    );
    bodyPath.lineTo(tx(398.052), ty(145.5));
    bodyPath.cubicTo(
      tx(396.151),
      ty(145.5),
      tx(394.417),
      ty(144.414),
      tx(393.587),
      ty(142.703),
    );
    bodyPath.lineTo(tx(393.458), ty(142.437));
    bodyPath.cubicTo(
      tx(392.58),
      ty(140.627),
      tx(392.991),
      ty(138.456),
      tx(394.469),
      ty(137.091),
    );
    bodyPath.cubicTo(
      tx(395.551),
      ty(136.093),
      tx(396.085),
      ty(134.633),
      tx(395.902),
      ty(133.171),
    );
    bodyPath.close();

    canvas.drawPath(bodyPath, fillPaint);
    canvas.drawPath(bodyPath, strokePaint);

    final tailPath = Path();
    tailPath.moveTo(tx(398.229), ty(145.5));
    tailPath.lineTo(tx(398.729), ty(146.357));
    tailPath.cubicTo(
      tx(400.979),
      ty(150.214),
      tx(405.479),
      ty(150.214),
      tx(407.729),
      ty(146.357),
    );
    tailPath.lineTo(tx(408.229), ty(145.5));
    canvas.drawPath(tailPath, strokePaint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class AppTextField extends StatelessWidget {
  final TextEditingController controller;
  final String hint;
  final IconData icon;
  final bool isPassword;
  final bool obscureText;
  final VoidCallback? onToggleObscure;
  final TextInputType? keyboardType;

  const AppTextField({
    super.key,
    required this.controller,
    required this.hint,
    required this.icon,
    this.isPassword = false,
    this.obscureText = false,
    this.onToggleObscure,
    this.keyboardType,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 340.w,
      height: 65.h,
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.9),
        borderRadius: BorderRadius.circular(15.r),
        border: Border.all(color: Colors.black, width: 1.5),
      ),
      child: TextField(
        controller: controller,
        obscureText: isPassword ? obscureText : false,
        keyboardType: keyboardType,
        textAlign: TextAlign.right,
        decoration: InputDecoration(
          hintText: hint,
          prefixIcon: Icon(icon, color: Colors.grey),
          suffixIcon: isPassword
              ? IconButton(
                  icon: Icon(
                    obscureText ? Icons.visibility_off : Icons.visibility,
                    color: Colors.grey,
                  ),
                  onPressed: onToggleObscure,
                )
              : null,
          border: InputBorder.none,
          contentPadding: EdgeInsets.symmetric(
            vertical: 18.h,
            horizontal: 16.w,
          ),
        ),
      ),
    );
  }
}

/// حقل نص مخصص لشاشة استكمال المعلومات الشخصية (ProfileCompletionScreen)
/// يدعم وضعه بإحداثيات مطلقة (Positioned) ويدعم أيقونة SVG أو PNG،
/// مطابق تماماً للتصميم الأصلي بالشاشة.
class ProfileTextField extends StatelessWidget {
  final double scale;
  final double top;
  final String hintText;
  final String assetPath;
  final bool isSvg;
  final TextEditingController? controller;
  final double left;
  final double width;
  final double height;

  const ProfileTextField({
    super.key,
    required this.scale,
    required this.top,
    required this.hintText,
    required this.assetPath,
    required this.isSvg,
    this.controller,
    this.left = 50,
    this.width = 340,
    this.height = 75,
  });

  @override
  Widget build(BuildContext context) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          border: Border.all(color: Colors.black, width: 2 * scale),
          borderRadius: BorderRadius.circular(15 * scale),
        ),
        child: Stack(
          children: [
            Positioned(
              top: 0,
              bottom: 0,
              right: 60 * scale,
              left: 15 * scale,
              child: Directionality(
                textDirection: TextDirection.rtl,
                child: Center(
                  child: TextField(
                    controller: controller,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w500,
                      fontSize: 24 * scale,
                      color: Colors.black,
                    ),
                    decoration: InputDecoration(
                      hintText: hintText,
                      hintStyle: TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w500,
                        fontSize: 24 * scale,
                        color: Colors.black.withOpacity(0.5),
                      ),
                      border: InputBorder.none,
                      isDense: true,
                    ),
                  ),
                ),
              ),
            ),
            Positioned(
              top: 22 * scale,
              right: 15 * scale,
              width: 30 * scale,
              height: 30 * scale,
              child: isSvg
                  ? SvgPicture.asset(assetPath, fit: BoxFit.contain)
                  : Image.asset(assetPath, fit: BoxFit.contain),
            ),
          ],
        ),
      ),
    );
  }
}

class NavBarConstants {
  NavBarConstants._();

  static const List<IconData> icons = [
    Icons.home_rounded,
    Icons.check_circle_outline_rounded,
    Icons.favorite_border_rounded,
    Icons.calendar_month_outlined,
    Icons.explore_outlined,
  ];

  static const List<String> labels = ['الرئيسية', '', '', '', ''];

  static const Color activeColor = Color(0xFFF4A261);
  static const Color inactiveColor = Color(0xFF9E9E9E);
  static const Color navBgColor = Color(0x8091B3FA);
}

class AppColors {
  static const Color gradientTop = Color(0xFF91B3FA);
  static const Color gradientMiddle = Color(0xFFC8D9FD);
  static const Color gradientBottom = Color(0xFFFFFFFF);
  static const Color navBarBackground = Color(0xFFD1E3FF);

  static const BoxDecoration backgroundGradient = BoxDecoration(
    gradient: LinearGradient(
      begin: Alignment.topCenter,
      end: Alignment.bottomCenter,
      colors: [gradientTop, gradientMiddle, gradientBottom],
      stops: [0.0, 0.3, 0.7],
    ),
  );
}

class AppTextStyles {
  static const String fontFamily = 'Cairo';
  static const TextStyle headerTitle = TextStyle(
    fontFamily: fontFamily,
    fontSize: 40,
    fontWeight: FontWeight.w700,
    color: Colors.black,
    letterSpacing: 2.0,
  );
}

class AppIcons {
  static const double backArrowWidth = 50;
  static const double backArrowHeight = 50;
  static const String backArrowPath = 'assets/icons/arrowBack.svg';
}

extension ScaleExtension on BuildContext {
  double get scale => MediaQuery.of(this).size.width / 390;
}

class CustomBackButton extends StatelessWidget {
  const CustomBackButton({super.key});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 50 * context.scale,
      height: 50 * context.scale,
      child: IconButton(
        padding: EdgeInsets.zero,
        onPressed: () => Navigator.pop(context),
        icon: SvgPicture.asset(
          AppIcons.backArrowPath,
          width: AppIcons.backArrowWidth * context.scale,
          height: AppIcons.backArrowHeight * context.scale,
          fit: BoxFit.contain,
        ),
      ),
    );
  }
}

class CustomHeaderTitle extends StatelessWidget {
  final String title;

  const CustomHeaderTitle({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      textAlign: TextAlign.center,
      maxLines: 1,
      style: TextStyle(
        fontFamily: AppTextStyles.fontFamily,
        fontSize: 40 * context.scale,
        fontWeight: FontWeight.w700,
        color: Colors.black,
        height: 1.0,
      ),
    );
  }
}
