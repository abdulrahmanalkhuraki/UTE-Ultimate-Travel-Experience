import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import 'bottomNavigationBar.dart';

class SettingsScreen extends StatefulWidget {
  final String userName;
  final String userEmail;
  final String userAge;
  final String userLocation;
  final String userId;

  const SettingsScreen({
    Key? key,
    required this.userName,
    required this.userEmail,
    required this.userAge,
    required this.userLocation,
    required this.userId,
  }) : super(key: key);

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
      body: Directionality(
        textDirection: TextDirection.ltr,
        child: SingleChildScrollView(
          child: SizedBox(
            width: 440 * scale,
            height: 1680 * scale,
            child: Stack(
              clipBehavior: Clip.none,
              children: [
                // ✅ Vector نفس AddCompanionScreen
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

                // سهم الرجوع
                Positioned(
                  top: 46 * scale,
                  left: 373 * scale,
                  width: 35 * scale,
                  height: 35 * scale,
                  child: Icon(
                    Icons.keyboard_arrow_right,
                    size: 35 * scale,
                    color: Colors.black,
                  ),
                ),

                // عنوان الصفحة
                Positioned(
                  top: 25 * scale,
                  left: 131 * scale,
                  width: 178 * scale,
                  height: 75 * scale,
                  child: Center(
                    child: Text(
                      'الاعدادات',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Cairo',
                        fontWeight: FontWeight.w700,
                        fontSize: 40 * scale,
                        letterSpacing: 2 * scale,
                        height: 1.0,
                        color: const Color(0xFF000000),
                      ),
                    ),
                  ),
                ),

                // البطاقة الزجاجية profileCard
                Positioned(
                  top: 100 * scale,
                  left: 10 * scale,
                  width: 420 * scale,
                  height: 317 * scale,
                  child: SvgPicture.asset(
                    'assets/images/profileCard.svg',
                    fit: BoxFit.fill,
                  ),
                ),

                // Union
                Positioned(
                  top: 100 * scale,
                  left: -2 * scale,
                  width: 130 * scale,
                  height: 130 * scale,
                  child: SvgPicture.asset(
                    'assets/icons/Union.svg',
                    fit: BoxFit.fill,
                  ),
                ),

                // الدائرة المحيطة بالشمس
                Positioned(
                  top: 188 * scale,
                  left: 2 * scale,
                  width: 49 * scale,
                  height: 35 * scale,
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(20 * scale),
                    child: BackdropFilter(
                      filter: ImageFilter.blur(sigmaX: 10.0, sigmaY: 10.0),
                      child: Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(20 * scale),
                          gradient: const LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            stops: [0.0, 0.1875, 0.5817],
                            colors: [
                              Color(0xFFF4A261),
                              Color(0x8CF4A261),
                              Color(0x1AF4A261),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ),

                // أيقونة الشمس
                Positioned(
                  top: 193 * scale,
                  left: 14 * scale,
                  width: 24 * scale,
                  height: 24 * scale,
                  child: SvgPicture.asset(
                    'assets/icons/Sun.svg',
                    fit: BoxFit.contain,
                  ),
                ),

                // الصورة الشخصية
                Positioned(
                  top: 121 * scale,
                  left: 309 * scale,
                  width: 100 * scale,
                  height: 100 * scale,
                  child: Container(
                    decoration: const BoxDecoration(shape: BoxShape.circle),
                    child: ClipOval(
                      child: SvgPicture.asset(
                        'assets/icons/Profile_Circle.svg',
                        fit: BoxFit.cover,
                      ),
                    ),
                  ),
                ),

                // اسم المستخدم
                Positioned(
                  top: 149 * scale,
                  left: 139 * scale,
                  width: 164 * scale,
                  height: 26 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      widget.userName,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 22 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),

                // الإيميل
                Positioned(
                  top: 170 * scale,
                  left: 151 * scale,
                  width: 140 * scale,
                  height: 40 * scale,
                  child: Text(
                    widget.userEmail,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 16 * scale,
                      fontWeight: FontWeight.w400,
                      color: Colors.black,
                      height: 1.2,
                    ),
                  ),
                ),

                // المستطيل الداخلي
                Positioned(
                  top: 224 * scale,
                  left: 71 * scale,
                  width: 299 * scale,
                  height: 112 * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(25 * scale),
                      border: Border.all(color: Colors.black, width: 1 * scale),
                    ),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(25 * scale),
                      child: BackdropFilter(
                        filter: ImageFilter.blur(sigmaX: 10.0, sigmaY: 10.0),
                        child: Container(color: Colors.transparent),
                      ),
                    ),
                  ),
                ),

                _buildSquareArrow(scale: scale, top: 267, left: 86),

                // العمر label
                Positioned(
                  top: 245 * scale,
                  left: 303 * scale,
                  width: 40 * scale,
                  height: 19 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'العمر:',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 16 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),

                // قيمة العمر
                Positioned(
                  top: 265 * scale,
                  left: 303 * scale,
                  width: 40 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      '${widget.userAge}\nسنة',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 12 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.2,
                      ),
                    ),
                  ),
                ),

                // الموقع label
                Positioned(
                  top: 245 * scale,
                  left: 168 * scale,
                  width: 93 * scale,
                  height: 19 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'الموقع الحالي:',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 16 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),

                // قيمة الموقع
                Positioned(
                  top: 265 * scale,
                  left: 140 * scale,
                  width: 150 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      widget.userLocation,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 11 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.2,
                      ),
                    ),
                  ),
                ),

                // الرقم التعريفي
                Positioned(
                  top: 305 * scale,
                  left: 130 * scale,
                  width: 180 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'الرقم التعريفي: ${widget.userId}',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 13 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                      ),
                    ),
                  ),
                ),

                // حذف الحساب
                Positioned(
                  top: 350 * scale,
                  left: 146.5 * scale,
                  width: 32 * scale,
                  height: 32 * scale,
                  child: SvgPicture.asset('assets/icons/Delete.svg'),
                ),
                Positioned(
                  top: 383 * scale,
                  left: 122 * scale,
                  width: 81 * scale,
                  height: 30 * scale,
                  child: Text(
                    'حذف حسابي',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w400,
                      fontSize: 16 * scale,
                      color: const Color(0xFFDB1518),
                    ),
                  ),
                ),

                // تسجيل الخروج
                Positioned(
                  top: 350 * scale,
                  left: 264.5 * scale,
                  width: 32 * scale,
                  height: 32 * scale,
                  child: SvgPicture.asset('assets/icons/Logout.svg'),
                ),
                Positioned(
                  top: 383 * scale,
                  left: 232 * scale,
                  width: 97 * scale,
                  height: 30 * scale,
                  child: Text(
                    'تسجيل الخروج',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w400,
                      fontSize: 16 * scale,
                      color: const Color(0xFFDB1518),
                    ),
                  ),
                ),

                // اللغة
                _buildListContainer(
                  scale: scale,
                  top: 447,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 467,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/laguage.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 474,
                  left: 227,
                  width: 46,
                  height: 26,
                  text: 'اللغة',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 474, left: 54),

                // العملة
                _buildListContainer(
                  scale: scale,
                  top: 542,
                  left: 241,
                  width: 160,
                  height: 80,
                  hasShadow: true,
                ),
                _buildListContainer(
                  scale: scale,
                  top: 542,
                  left: 39,
                  width: 160,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 562,
                  left: 341,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/coin.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 572,
                  left: 271,
                  width: 60,
                  height: 26,
                  text: 'العملة',
                  fontSize: 22,
                ),
                _buildText(
                  scale: scale,
                  top: 569,
                  left: 104,
                  width: 70,
                  height: 26,
                  text: 'ل.س.ج',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 569, left: 54),

                // إرسال إشعار
                _buildListContainer(
                  scale: scale,
                  top: 637,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildText(
                  scale: scale,
                  top: 653,
                  left: 131,
                  width: 254,
                  height: 52,
                  text: 'ارسال اشعار عن برامج\nالشركات اللتي تم تفضيلها:',
                  fontSize: 22,
                ),
                Positioned(
                  top: 659 * scale,
                  left: 59 * scale,
                  width: 68 * scale,
                  height: 35 * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      color: const Color(0xFF91B3FA),
                      borderRadius: BorderRadius.circular(17.5 * scale),
                    ),
                  ),
                ),
                Positioned(
                  top: 662.5 * scale,
                  left: 63 * scale,
                  width: 28 * scale,
                  height: 28 * scale,
                  child: Container(
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                    ),
                  ),
                ),
                _buildIcon(
                  scale: scale,
                  top: 664,
                  left: 65,
                  width: 24,
                  height: 24,
                  assetPath: 'assets/icons/SMS.svg',
                ),

                // المرافقون
                _buildListContainer(
                  scale: scale,
                  top: 732,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 752,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/companions.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 759,
                  left: 211,
                  width: 91,
                  height: 26,
                  text: 'المرافقون',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 759, left: 54),

                // الأدوات والخرائط
                _buildListContainer(
                  scale: scale,
                  top: 827,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 855,
                  left: 329,
                  width: 24,
                  height: 24,
                  assetPath: 'assets/icons/Edit.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 854,
                  left: 104,
                  width: 254,
                  height: 26,
                  text: 'الأدوات والخرائط',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 854, left: 54),

                // الإبلاغ عن سائح
                _buildListContainer(
                  scale: scale,
                  top: 922,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 942,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/Danger_Triangle.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 949,
                  left: 162,
                  width: 140,
                  height: 26,
                  text: 'الإبلاغ عن سائح',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 949, left: 54),

                // تقييم UTE
                _buildListContainer(
                  scale: scale,
                  top: 1017,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 1037,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/Like.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 1044,
                  left: 190,
                  width: 120,
                  height: 26,
                  text: 'تقييم UTE',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 1044, left: 54),

                // تواصل مع فريق الدعم
                _buildListContainer(
                  scale: scale,
                  top: 1112,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                Positioned(
                  top: 1132 * scale,
                  left: 321 * scale,
                  width: 40 * scale,
                  height: 40 * scale,
                  child: Image.asset(
                    'assets/icons/support.png',
                    fit: BoxFit.contain,
                  ),
                ),
                _buildText(
                  scale: scale,
                  top: 1139,
                  left: 117,
                  width: 197,
                  height: 26,
                  text: 'تواصل مع فريق الدعم',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 1139, left: 54),

                // إعادة تعيين كلمة المرور
                _buildListContainer(
                  scale: scale,
                  top: 1207,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 1227,
                  left: 331,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/Refresh.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 1234,
                  left: 100,
                  width: 218,
                  height: 26,
                  text: 'إعادة تعيين كلمة المرور',
                  fontSize: 22,
                ),
                _buildSquareArrow(scale: scale, top: 1234, left: 54),

                // شريط التنقل
                Positioned(
                  bottom: 0,
                  left: 0,
                  right: 0,
                  child: AppBottomNavBar(selectedIndex: 4),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildListContainer({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    bool hasShadow = false,
  }) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(25 * scale),
          border: Border.all(color: Colors.black, width: 1 * scale),
          boxShadow: hasShadow
              ? [
                  BoxShadow(
                    color: const Color(0x40000000),
                    offset: Offset(0, 4 * scale),
                    blurRadius: 25 * scale,
                    spreadRadius: 4 * scale,
                  ),
                ]
              : null,
        ),
      ),
    );
  }

  Widget _buildText({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    required String text,
    required double fontSize,
  }) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: Directionality(
        textDirection: TextDirection.rtl,
        child: Center(
          child: Text(
            text,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w400,
              fontSize: fontSize * scale,
              height: 1.0,
              color: Colors.black,
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildIcon({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    required String assetPath,
  }) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: SvgPicture.asset(assetPath, fit: BoxFit.contain),
    );
  }

  Widget _buildSquareArrow({
    required double scale,
    required double top,
    required double left,
  }) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: 26 * scale,
      height: 26 * scale,
      child: Container(
        decoration: BoxDecoration(
          border: Border.all(color: Colors.black, width: 1.2 * scale),
          borderRadius: BorderRadius.circular(6 * scale),
        ),
        child: Center(
          child: Icon(
            Icons.keyboard_arrow_left,
            size: 20 * scale,
            color: Colors.black,
          ),
        ),
      ),
    );
  }
}
