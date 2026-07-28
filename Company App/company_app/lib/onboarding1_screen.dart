import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class Onboarding1Screen extends StatelessWidget {
  const Onboarding1Screen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scaleW = size.width / 440;
    final double scaleH = size.height / 956;

    return Scaffold(
      backgroundColor: Colors.white,
      body: Directionality(
        textDirection: TextDirection.rtl,
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            Positioned(
              top: 0,
              left: 0,
              right: 0,
              child: SvgPicture.asset(
                'assets/images/Vector.svg',
                width: size.width,
                fit: BoxFit.cover,
              ),
            ),
            Positioned(
              top: 60 * scaleH,
              right: 23 * scaleW,
              child: GestureDetector(
                onTap: () {
                  Navigator.of(context).pushReplacementNamed('/areyoucompany');
                },
                child: Text(
                  'تخطي',
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 32 * scaleW,
                    fontWeight: FontWeight.w500,
                    height: 1.0,
                    color: Colors.black,
                  ),
                ),
              ),
            ),
            Positioned(
              top: 99 * scaleH,
              left: -53 * scaleW,
              width: 536 * scaleW,
              height: 536 * scaleH,
              child: SvgPicture.asset(
                'assets/images/onBoarding1.svg',
                fit: BoxFit.contain,
              ),
            ),
            Positioned(
              top: 637 * scaleH,
              left: 12 * scaleW,
              width: 414 * scaleW,
              child: Center(
                child: Text(
                  'سهولة في العثور على وجهتك',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.w700,
                    fontSize: 32 * scaleW,
                    height: 1.0,
                    letterSpacing: 0,
                    color: Colors.black,
                  ),
                ),
              ),
            ),
            Positioned(
              top: 696 * scaleH,
              left: -10 * scaleW,
              width: 459 * scaleW,
              child: Center(
                child: Text(
                  'سهولة في العثور على وجهتك\nباستخدام خرائط',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.w500,
                    fontSize: 32 * scaleW,
                    height: 1.0,
                    letterSpacing: 0,
                    color: const Color(0xFF9E9E9E),
                  ),
                ),
              ),
            ),
            Positioned(
              top: 800 * scaleH,
              left: 0,
              right: 0,
              child: Directionality(
                textDirection: TextDirection.ltr,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    _buildDot(
                      width: 30 * scaleW,
                      height: 15 * scaleH,
                      color: const Color(0xFF91B3FA),
                    ),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(
                      width: 20 * scaleW,
                      height: 15 * scaleH,
                      color: const Color(0x4091B3FA),
                    ),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(
                      width: 20 * scaleW,
                      height: 15 * scaleH,
                      color: const Color(0x4091B3FA),
                    ),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(
                      width: 20 * scaleW,
                      height: 15 * scaleH,
                      color: const Color(0x4091B3FA),
                    ),
                  ],
                ),
              ),
            ),
            Positioned(
              top: 820 * scaleH,
              left: (440 - 318) / 2 * scaleW,
              child: Container(
                width: 318 * scaleW,
                height: 76 * scaleH,
                decoration: BoxDecoration(
                  color: const Color(0x8091B3FA),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Material(
                  color: Colors.transparent,
                  child: InkWell(
                    borderRadius: BorderRadius.circular(20),
                    onTap: () {
                      Navigator.of(
                        context,
                      ).pushReplacementNamed('/onboarding2');
                    },
                    child: Center(
                      child: Text(
                        'ابدأ رحلتك',
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * scaleW,
                          fontWeight: FontWeight.w500,
                          height: 1.0,
                          color: Colors.black,
                        ),
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDot({
    required double width,
    required double height,
    required Color color,
  }) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(15),
      ),
    );
  }
}
