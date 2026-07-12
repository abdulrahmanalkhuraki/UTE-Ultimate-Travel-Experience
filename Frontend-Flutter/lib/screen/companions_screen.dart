import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'bottomNavigationBar.dart';
import 'add_companion_screen.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
import 'edit_companion_screen.dart';

class CompanionsScreen extends StatefulWidget {
  final List<Map<String, String>> companionsData;

  const CompanionsScreen({super.key, this.companionsData = const []});

  @override
  State<CompanionsScreen> createState() => _CompanionsScreenState();
}

class _CompanionsScreenState extends State<CompanionsScreen> {
  static const double navBarHeight = 90.0;
  static const double bottomSafeGap = 30.0;

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;

    final bool isDarkMode = context.watch<ThemeCubit>().state == ThemeMode.dark;
    final Color textColor = isDarkMode ? Colors.white : Colors.black;

    final List<Map<String, String>> companionsList =
        widget.companionsData.isNotEmpty
        ? widget.companionsData
        : [
            {
              "name": "عبد الرحمن الخرقي",
              "date": "20\\9\\2025",
              "age": "20",
              "relation": "زوجة",
              "times": "3",
              "id": "08784927594",
            },
            {
              "name": "أحمد المحمد",
              "date": "15\\8\\2025",
              "age": "25",
              "relation": "أخ",
              "times": "1",
              "id": "09876543210",
            },
          ];

    double buttonTop = 110.0 + (companionsList.length * 337.0);
    double scrollHeight = buttonTop + 150.0 + navBarHeight + bottomSafeGap;

    return Scaffold(
      backgroundColor: isDarkMode
          ? const Color(0xFF1E1E1E)
          : const Color(0xFFFFFFFF),
      body: Stack(
        children: [
          Directionality(
            textDirection: TextDirection.ltr,
            child: SingleChildScrollView(
              child: SizedBox(
                width: 440 * scale,
                height: scrollHeight * scale > size.height
                    ? scrollHeight * scale
                    : size.height,
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
                        colorFilter: isDarkMode
                            ? const ColorFilter.mode(
                                Color(0xFF223A5E),
                                BlendMode.srcIn,
                              )
                            : null,
                      ),
                    ),

                    Positioned(
                      top: 46 * scale,
                      left: 373 * scale,
                      width: 35 * scale,
                      height: 35 * scale,
                      child: GestureDetector(
                        onTap: () => Navigator.pop(context),
                        child: Icon(
                          Icons.keyboard_arrow_right,
                          size: 35 * scale,
                          color: textColor,
                        ),
                      ),
                    ),

                    Positioned(
                      top: 25 * scale,
                      left: 120 * scale,
                      width: 201 * scale,
                      height: 75 * scale,
                      child: Center(
                        child: Text(
                          'المرافقون',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Cairo',
                            fontWeight: FontWeight.w700,
                            fontSize: 40 * scale,
                            letterSpacing: 2 * scale,
                            height: 1.0,
                            color: textColor,
                          ),
                        ),
                      ),
                    ),

                    for (int i = 0; i < companionsList.length; i++)
                      ..._buildCompanionCard(
                        scale: scale,
                        cardTop: 100.0 + (i * 337.0),
                        data: companionsList[i],
                        textColor: textColor,
                        isDarkMode: isDarkMode,
                      ),

                    Positioned(
                      top: buttonTop * scale,
                      left: 54 * scale,
                      width: 332 * scale,
                      height: 65 * scale,
                      child: GestureDetector(
                        onTap: () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (context) => const AddCompanionScreen(),
                            ),
                          );
                        },
                        child: Container(
                          decoration: BoxDecoration(
                            color: const Color(0xFFF4A261),
                            borderRadius: BorderRadius.circular(20 * scale),
                          ),
                        ),
                      ),
                    ),
                    Positioned(
                      top: (buttonTop + 11) * scale,
                      left: 120 * scale,
                      width: 192 * scale,
                      height: 43 * scale,
                      child: IgnorePointer(
                        child: Directionality(
                          textDirection: TextDirection.rtl,
                          child: Center(
                            child: Text(
                              'إضافة مرافق',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 36 * scale,
                                height: 1.2,
                                color: const Color(0xFFFFFFFF),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                    Positioned(
                      top: (buttonTop + 15) * scale,
                      left: 336 * scale,
                      width: 35 * scale,
                      height: 35 * scale,
                      child: IgnorePointer(
                        child: SvgPicture.asset(
                          'assets/icons/Profile_Add.svg',
                          fit: BoxFit.contain,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
          const Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: AppBottomNavBar(selectedIndex: 4),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildCompanionCard({
    required double scale,
    required double cardTop,
    required Map<String, String> data,
    required Color textColor,
    required bool isDarkMode,
  }) {
    return [
      Positioned(
        top: cardTop * scale,
        left: 10 * scale,
        width: 420 * scale,
        height: 317 * scale,
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(70 * scale),
            border: Border.all(
              color: isDarkMode ? Colors.white24 : const Color(0x29000000),
              width: 2 * scale,
            ),
            gradient: isDarkMode
                ? const LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [Color(0xFF2C3E50), Color(0xFF1E272E)],
                  )
                : const LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    stops: [0.0, 0.1875, 0.5817],
                    colors: [
                      Color(0x80FFFFFF),
                      Color(0x8C91B3FA),
                      Color(0x1A91B3FA),
                    ],
                  ),
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(70 * scale),
            child: BackdropFilter(
              filter: ImageFilter.blur(sigmaX: 10.0, sigmaY: 10.0),
              child: Container(color: Colors.transparent),
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 21) * scale,
        left: 309 * scale,
        width: 100 * scale,
        height: 100 * scale,
        child: ClipOval(
          child: SvgPicture.asset(
            'assets/icons/Profile_Circle.svg',
            fit: BoxFit.cover,
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 49) * scale,
        left: 139 * scale,
        width: 164 * scale,
        height: 35 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Center(
            child: Text(
              data['name'] ?? '',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 22 * scale,
                fontWeight: FontWeight.w400,
                color: textColor,
                height: 1.2,
              ),
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 79) * scale,
        left: 206 * scale,
        width: 97 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Center(
            child: Text(
              'انضم معك في',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 16 * scale,
                fontWeight: FontWeight.w400,
                color: textColor,
                height: 1.2,
              ),
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 79) * scale,
        left: 134 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Center(
            child: Text(
              data['date'] ?? '',
              maxLines: 1,
              softWrap: false,
              textAlign: TextAlign.center,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 16 * scale,
                fontWeight: FontWeight.w400,
                color: textColor,
                height: 1.2,
              ),
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 124) * scale,
        left: 71 * scale,
        width: 299 * scale,
        height: 112 * scale,
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(25 * scale),
            border: Border.all(color: textColor, width: 1 * scale),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 145) * scale,
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
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 145) * scale,
        left: 209 * scale,
        width: 78 * scale,
        height: 19 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            'صلة القرابة:',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 16 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 145) * scale,
        left: 147 * scale,
        width: 46 * scale,
        height: 19 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            'رافقك:',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 16 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 168) * scale,
        left: 312 * scale,
        width: 21 * scale,
        height: 40 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            '${data['age']}\nسنة',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 11 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 175) * scale,
        left: 237 * scale,
        width: 22 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            data['relation'] ?? '',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 11 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 168) * scale,
        left: 158 * scale,
        width: 24 * scale,
        height: 40 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            '${data['times']}\nمرات',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 11 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 205) * scale,
        left: 247 * scale,
        width: 96 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            'الرقم التعريفي:',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 16 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 207) * scale,
        left: 150 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.ltr,
          child: Text(
            data['id'] ?? '',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 16 * scale,
              fontWeight: FontWeight.w400,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),

      Positioned(
        top: (cardTop + 165) * scale,
        left: 85 * scale,
        width: 24 * scale,
        height: 24 * scale,
        child: GestureDetector(
          onTap: () {},
          child: Container(
            decoration: BoxDecoration(
              border: Border.all(color: textColor, width: 1.2 * scale),
              borderRadius: BorderRadius.circular(6 * scale),
            ),
            child: Center(
              child: Icon(
                Icons.keyboard_arrow_left,
                size: 18 * scale,
                color: textColor,
              ),
            ),
          ),
        ),
      ),

      // 🌟 أيقونة إزالة مرافق (بالـ Positioned الأصلي تبعك 100%)
      // --- أيقونة إزالة مرافق ---
      Positioned(
        top: (cardTop + 247) * scale,
        left: 146.5 * scale, // نفس إزاحة حذف حسابي
        width: 32 * scale,
        height: 32 * scale,
        child: GestureDetector(
          onTap: () => _showDeleteDialog(context, scale, isDarkMode, textColor, data),
          child: SvgPicture.asset('assets/icons/Delete.svg', fit: BoxFit.contain),
        ),
      ),
      // --- نص إزالة مرافق ---
      Positioned(
        top: (cardTop + 283) * scale,
        left: 124 * scale,
        width: 85 * scale,
        height: 30 * scale,
        child: GestureDetector(
          onTap: () => _showDeleteDialog(context, scale, isDarkMode, textColor, data),
          child: Text(
            'ازالة مرافق',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w400,
              fontSize: 16 * scale,
              color: const Color(0xFFDB1518),
            ),
          ),
        ),
      ),

      // --- أيقونة تعديل المعلومات ---
      Positioned(
        top: (cardTop + 249) * scale,
        left: 266 * scale,
        width: 32 * scale,
        height: 32 * scale,
        child: GestureDetector(
          onTap: () {
            Navigator.push(context, MaterialPageRoute(builder: (context) => EditCompanionScreen(companionData: data)));
          },
          child: SvgPicture.asset('assets/icons/Edit.svg', fit: BoxFit.contain, colorFilter: ColorFilter.mode(textColor, BlendMode.srcIn)),
        ),
      ),
      // --- نص تعديل المعلومات ---
      Positioned(
        top: (cardTop + 283) * scale,
        left: 215 * scale,
        width: 125 * scale,
        height: 30 * scale,
        child: GestureDetector(
          onTap: () {
            Navigator.push(context, MaterialPageRoute(builder: (context) => EditCompanionScreen(companionData: data)));
          },
          child: Text(
            'تعديل المعلومات',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w400,
              fontSize: 16 * scale,
              color: textColor,
            ),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 283) * scale,
        left: 215 * scale,
        width: 125 * scale,
        height: 25 * scale,
        child: GestureDetector(
          onTap: () {
            Navigator.push(
              context,
              MaterialPageRoute(
                builder: (context) => const EditCompanionScreen(),
              ),
            );
          },
          child: Text(
            'تعديل المعلومات',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w400,
              fontSize: 16 * scale,
              color: textColor,
              height: 1.2,
            ),
          ),
        ),
      ),
    ];
  }

  // 🌟 دالة النافذة المنبثقة باللون والأيقونة اللي طلبتيها
  void _showDeleteDialog(
    BuildContext context,
    double scale,
    bool isDarkMode,
    Color textColor,
    Map<String, String> data,
  ) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          backgroundColor: isDarkMode ? const Color(0xFF2C3E50) : Colors.white,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(20 * scale),
          ),
          title: Directionality(
            textDirection: TextDirection.rtl,
            child: Row(
              children: [
                SvgPicture.asset(
                  'assets/icons/Delete.svg',
                  width: 32 * scale,
                  height: 32 * scale,
                ),
                SizedBox(width: 10 * scale),
                Text(
                  'إزالة مرافق',
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.bold,
                    fontSize: 22 * scale,
                    color: const Color(0xFFDB1518),
                  ),
                ),
              ],
            ),
          ),
          content: Directionality(
            textDirection: TextDirection.rtl,
            child: Text(
              'هل أنت متأكد من حذف "${data['name']}"؟',
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 18 * scale,
                color: textColor,
              ),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text(
                'تراجع',
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 18 * scale,
                  color: textColor,
                ),
              ),
            ),
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: Text(
                'حذف',
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.bold,
                  fontSize: 18 * scale,
                  color: const Color(0xFFDB1518),
                ),
              ),
            ),
          ],
        );
      },
    );
  }
}
