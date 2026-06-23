
import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get_core/src/get_main.dart';
import 'package:get/get_navigation/src/extension_navigation.dart';
import 'package:ute_app/screen/home_screen.dart';
class AreYouCompanyScreen extends StatelessWidget {
  const AreYouCompanyScreen({super.key});

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
            // 1. الخلفية الأصلية
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

            // 2. العنوان
            Positioned(
              top: 60 * scaleH,
              left: 0,
              right: 0,
              child: Text(
                'هل أنت شركة',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w700,
                  fontSize: 40 * scaleW,
                  color: const Color(0xFF000000),
                ),
              ),
            ),

            // 3. الصورة الرئيسية
            Positioned(
              top: 93 * scaleH,
              left: 27 * scaleW,
              right: 28 * scaleW,
              height: 350 * scaleH,
              child: Image.asset(
                'assets/images/company.png',
                fit: BoxFit.contain,
              ),
            ),

            // 4. الخط الفاصل
            Positioned(
              top: 420 * scaleH,
              left: 16 * scaleW,
              right: 16 * scaleW,
              child: Container(
                height: 1,
                color: const Color(0xFFD3D3D3),
              ),
            ),

            // 5. النص التفصيلي بالقياسات المطلوبة وتقسيم الكلمات الدقيق
            Positioned(
              top: 442 * scaleH,
              left: 16 * scaleW,
              width: 409 * scaleW,
              height: 246 * scaleH,
              child: Text(
                'يمكنك تسجيل الدخول كشركة سياحية لعرض البرامج السياحية التي تقدمها مما يسهل الحصول على سائحين راغبين بالالتحاق ببرنامجك الفريد.',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w300,
                  fontSize: 28 * scaleW,
                  height: 1.3,
                  color: const Color(0xFF000000),
                ),
              ),
            ),

            // 6. الأزرار ببنيتها الأصلية
            Positioned(
              top: 681 * scaleH,
              left: 87 * scaleW,
              right: 87 * scaleW,
              child: Column(
                children: [
                  SizedBox(
                    height: 81 * scaleH,
                    child: Material(
                      color: const Color(0xFFF4A261),
                      borderRadius: BorderRadius.circular(20),
                      child: InkWell(
                        onTap: () {},
                        child: Center(
                          child: Text(
                            'تسجيل الدخول كشركة\nسياحية',
                            textAlign: TextAlign.center,
                            style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 24 * scaleW, color: Colors.white),
                          ),
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 15 * scaleH),
                  SizedBox(
                    height: 60 * scaleH,
                    child: Material(
                      color: const Color(0xFFF4A261),
                      borderRadius: BorderRadius.circular(15),
                      child: InkWell(
                        onTap: () => Get.to(() =>  HomeScreenProvider()),
                        child: Center(
                          child: Text(
                            'المتابعة كسائح',
                            style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 24 * scaleW, color: Colors.white),
                          ),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),

            // 7. النص السفلي بالقياسات المطلوبة
            Positioned(
              top: 852 * scaleH,
              left: 15 * scaleW,
              width: 409 * scaleW,
              height: 87 * scaleH,
              child: RichText(
                textAlign: TextAlign.center,
                text: TextSpan(
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 24 * scaleW,
                    color: Colors.black,
                    fontWeight: FontWeight.w500,
                    height: 1.2,
                  ),
                  children: [
                    const TextSpan(text: 'لديك بعض الأسئلة؟\n'),
                    TextSpan(
                      text: 'المزيد من المعلومات حول الدخول\nكشركة سياحية',
                      style: const TextStyle(
                        color: Color(0xFFF4A261),
                        decoration: TextDecoration.underline,
                        decorationColor: Color(0xFFD3D3D3),
                      ),
                      recognizer: TapGestureRecognizer()..onTap = () {},
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
}