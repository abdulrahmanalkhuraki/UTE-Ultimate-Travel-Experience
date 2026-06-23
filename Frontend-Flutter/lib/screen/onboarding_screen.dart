import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:ute_app/screen/areyoucompany_screen.dart';

// ─────────────────────────────────────────
//  Onboarding Screen 1
// ─────────────────────────────────────────
class OnboardingScreen extends StatelessWidget {
  const OnboardingScreen({super.key});

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
                  Get.off(() => const AreYouCompanyScreen());
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
                    _buildDot(width: 30 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildDot(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
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
                      Get.off(() => const Onboarding2Screen());
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

  Widget _buildDot({required double width, required double height, required Color color}) {
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

// ─────────────────────────────────────────
//  Onboarding Screen 2
// ─────────────────────────────────────────
class Onboarding2Screen extends StatelessWidget {
  const Onboarding2Screen({super.key});

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
                  Get.off(() => const AreYouCompanyScreen());
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
              top: 77 * scaleH,
              left: -30 * scaleW,
              width: 500 * scaleW,
              height: 500 * scaleH,
              child: SvgPicture.asset(
                'assets/images/onBoarding2.svg',
                fit: BoxFit.contain,
              ),
            ),
            Positioned(
              top: 637 * scaleH,
              left: 12 * scaleW,
              width: 414 * scaleW,
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
            Positioned(
              top: 696 * scaleH,
              left: -10 * scaleW,
              width: 459 * scaleW,
              child: Text(
                'سهولة في العثور على وجهتك\nباستخدام خرائط',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 32 * scaleW,
                  height: 1.2,
                  letterSpacing: 0,
                  color: const Color(0xFF9E9E9E),
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
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 30 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                  ],
                ),
              ),
            ),
            Positioned(
              top: 820 * scaleH,
              left: 40 * scaleW,
              right: 40 * scaleW,
              child: Directionality(
                textDirection: TextDirection.ltr,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    _buildSideButton(
                      icon: Icons.arrow_back_ios_new_rounded,
                      backgroundColor: const Color(0x2691B3FA),
                      onTap: () {
                        Get.off(() => const OnboardingScreen());
                      },
                    ),
                    _buildSideButton(
                      icon: Icons.arrow_forward_ios_rounded,
                      backgroundColor: const Color(0x8091B3FA),
                      onTap: () {
                        Get.off(() => const Onboarding3Screen());
                      },
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRect({required double width, required double height, required Color color}) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(15),
      ),
    );
  }

  Widget _buildSideButton({
    required IconData icon,
    required Color backgroundColor,
    required VoidCallback onTap,
  }) {
    return Container(
      width: 75,
      height: 76,
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: const Color(0xFF999999).withOpacity(0.3),
          width: 0.5,
        ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: onTap,
          child: Center(child: Icon(icon, color: Colors.black87, size: 22)),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────
//  Onboarding Screen 3
// ─────────────────────────────────────────
class Onboarding3Screen extends StatelessWidget {
  const Onboarding3Screen({super.key});

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
                  Get.off(() =>  const AreYouCompanyScreen());
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
              top: 92 * scaleH,
              left: -29 * scaleW,
              width: 500 * scaleW,
              height: 500 * scaleH,
              child: SvgPicture.asset(
                'assets/images/onBoarding3.svg',
                fit: BoxFit.contain,
              ),
            ),
            Positioned(
              top: 637 * scaleH,
              left: 12 * scaleW,
              width: 414 * scaleW,
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
            Positioned(
              top: 696 * scaleH,
              left: -10 * scaleW,
              width: 459 * scaleW,
              child: Text(
                'سهولة في العثور على وجهتك\nباستخدام خرائط',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 32 * scaleW,
                  height: 1.2,
                  letterSpacing: 0,
                  color: const Color(0xFF9E9E9E),
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
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA).withOpacity(0.25)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 30 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA).withOpacity(0.25)),
                  ],
                ),
              ),
            ),
            Positioned(
              top: 820 * scaleH,
              left: 30 * scaleW,
              right: 30 * scaleW,
              child: Directionality(
                textDirection: TextDirection.ltr,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    _buildSideButton(
                      icon: Icons.arrow_back_ios_new_rounded,
                      backgroundColor: const Color(0x2691B3FA),
                      onTap: () {
                        Get.off(() => const Onboarding2Screen());
                      },
                    ),
                    _buildSideButton(
                      icon: Icons.arrow_forward_ios_rounded,
                      backgroundColor: const Color(0x8091B3FA),
                      onTap: () {
                        Get.off(() => const Onboarding4Screen());
                      },
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRect({required double width, required double height, required Color color}) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(15),
      ),
    );
  }

  Widget _buildSideButton({
    required IconData icon,
    required Color backgroundColor,
    required VoidCallback onTap,
  }) {
    return Container(
      width: 75,
      height: 76,
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: const Color(0xFF999999).withOpacity(0.3),
          width: 0.5,
        ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: onTap,
          child: Center(child: Icon(icon, color: Colors.black87, size: 22)),
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────
//  Onboarding Screen 4
// ─────────────────────────────────────────
class Onboarding4Screen extends StatelessWidget {
  const Onboarding4Screen({super.key});

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scaleW = size.width / 440;
    final double scaleH = size.height / 956;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
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
              top: 48 * scaleH,
              left: 336 * scaleW,
              width: 81 * scaleW,
              height: 38 * scaleH,
              child: GestureDetector(
                onTap: () {
                  Get.off(() => const AreYouCompanyScreen());
                },
                child: Container(
                  alignment: Alignment.center,
                  child: Text(
                    'تخطي',
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 32 * scaleW,
                      fontWeight: FontWeight.w500,
                      height: 1.0,
                      letterSpacing: 0,
                      color: const Color(0xFF000000),
                    ),
                  ),
                ),
              ),
            ),
            Positioned(
              top: 93 * scaleH,
              left: -27 * scaleW,
              width: 492 * scaleW,
              height: 492 * scaleH,
              child: Image.asset(
                'assets/images/onBoarding4.png',
                fit: BoxFit.cover,
              ),
            ),
            Positioned(
              top: 637 * scaleH,
              left: 12 * scaleW,
              width: 414 * scaleW,
              child: Text(
                'سهولة في العثور على وجهتك',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w700,
                  fontSize: 32 * scaleW,
                  height: 1.0,
                  letterSpacing: 0,
                  color: const Color(0xFF000000),
                ),
              ),
            ),
            Positioned(
              top: 696 * scaleH,
              left: -10 * scaleW,
              width: 459 * scaleW,
              child: Text(
                'سهولة في العثور على وجهتك\nباستخدام خرائط',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 32 * scaleW,
                  height: 1.2,
                  letterSpacing: 0,
                  color: const Color(0xFF9E9E9E),
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
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 20 * scaleW, height: 15 * scaleH, color: const Color(0x4091B3FA)),
                    SizedBox(width: 10 * scaleW),
                    _buildRect(width: 30 * scaleW, height: 15 * scaleH, color: const Color(0xFF91B3FA)),
                  ],
                ),
              ),
            ),
            Positioned(
              top: 820 * scaleH,
              left: 30 * scaleW,
              right: 30 * scaleW,
              child: Directionality(
                textDirection: TextDirection.ltr,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    _buildSideButton(
                      icon: Icons.arrow_back_ios_new_rounded,
                      backgroundColor: const Color(0x2691B3FA),
                      onTap: () {
                        Get.off(() => const Onboarding3Screen());
                      },
                    ),
                    _buildSideButton(
                      icon: Icons.arrow_forward_ios_rounded,
                      backgroundColor: const Color(0x8091B3FA),
                      onTap: () {
                        Get.off(() => const AreYouCompanyScreen());
                      },
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRect({required double width, required double height, required Color color}) {
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(15),
      ),
    );
  }

  Widget _buildSideButton({
    required IconData icon,
    required Color backgroundColor,
    required VoidCallback onTap,
  }) {
    return Container(
      width: 75,
      height: 76,
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: const Color(0xFF999999).withOpacity(0.5),
          width: 0.5,
        ),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(20),
          onTap: onTap,
          child: Center(child: Icon(icon, color: Colors.black87, size: 22)),
        ),
      ),
    );
  }
}