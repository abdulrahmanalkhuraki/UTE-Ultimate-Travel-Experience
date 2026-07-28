import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class GuidesScreen extends StatefulWidget {
  const GuidesScreen({super.key});

  @override
  State<GuidesScreen> createState() => _GuidesScreenState();
}

class _GuidesScreenState extends State<GuidesScreen> {
  static const double navBarHeight =
      90.0; // تركتها لحجز المساحة وتجنب تداخل العناصر مستقبلاً
  static const double bottomSafeGap = 30.0;

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;

    // تثبيت الألوان على الوضع الفاتح الافتراضي
    const Color textColor = Colors.black;
    const Color backgroundColor = Color(0xFFFFFFFF);

    // بيانات تجريبية للمرشدين لعرضها في الواجهة أولاً
    final List<Map<String, String>> guidesList = [
      {
        "name": "عبد الرحمن الخرقي",
        "date": "20\\9\\2025",
        "age": "30",
        "specialty": "تاريخي",
        "tours": "15",
        "id": "08784927594",
      },
      {
        "name": "أحمد المحمد",
        "date": "15\\8\\2025",
        "age": "28",
        "specialty": "طبيعة",
        "tours": "42",
        "id": "09876543210",
      },
    ];

    double buttonTop = 110.0 + (guidesList.length * 337.0);
    double scrollHeight = buttonTop + 150.0 + navBarHeight + bottomSafeGap;

    return Scaffold(
      backgroundColor: backgroundColor,
      body: Directionality(
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
                // الخلفية العلوية المتجهة
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

                // زر الرجوع الخلفي
                Positioned(
                  top: 42 * scale,
                  left: 370 * scale,
                  width: 50 * scale,
                  height: 50 * scale,
                  child: GestureDetector(
                    onTap: () {
                      // مساحة فارغة حالياً
                    },
                    child: SvgPicture.asset(
                      'assets/icons/Right.svg',
                      width: 50 * scale,
                      height: 50 * scale,
                      fit: BoxFit.contain,
                      colorFilter: const ColorFilter.mode(
                        textColor,
                        BlendMode.srcIn,
                      ),
                    ),
                  ),
                ),

                // عنوان الصفحة
                Positioned(
                  top: 25 * scale,
                  left: 120 * scale,
                  width: 201 * scale,
                  height: 75 * scale,
                  child: Center(
                    child: Text(
                      'المرشدون',
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

                // عرض بطاقات المرشدين
                for (int i = 0; i < guidesList.length; i++)
                  ..._buildGuideCard(
                    scale: scale,
                    cardTop: 100.0 + (i * 337.0),
                    data: guidesList[i],
                    textColor: textColor,
                  ),
                // زر إضافة مرشد جديد
                Positioned(
                  top: buttonTop * scale,
                  left: 54 * scale,
                  width: 332 * scale,
                  height: 65 * scale,
                  child: GestureDetector(
                    onTap: () {
                      print("اضغط لإضافة مرشد جديد");
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
                          'إضافة مرشد',
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
    );
  }

  // ميثود بناء بطاقة المرشد
  List<Widget> _buildGuideCard({
    required double scale,
    required double cardTop,
    required Map<String, String> data,
    required Color textColor,
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
              color: const Color(0x29000000),
              width: 2 * scale,
            ),
            gradient: const LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              stops: [0.0, 0.1875, 0.5817],
              colors: [Color(0x80FFFFFF), Color(0x8C91B3FA), Color(0x1A91B3FA)],
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
              'انضم للشركة في',
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
            'التخصص:',
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
            'رحلاته:',
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
        left: 209 * scale,
        width: 78 * scale,
        height: 25 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            data['specialty'] ?? '',
            textAlign: TextAlign.center,
            maxLines: 1,
            softWrap: false,
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
            '${data['tours']}\nرحلة',
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

      // --- إزالة مرشد ---
      Positioned(
        top: (cardTop + 247) * scale,
        left: 146.5 * scale,
        width: 32 * scale,
        height: 32 * scale,
        child: GestureDetector(
          onTap: () => _showDeleteDialog(context, scale, textColor, data),
          child: SvgPicture.asset(
            'assets/icons/amenities_Delete 2.svg',
            fit: BoxFit.contain,
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 283) * scale,
        left: 124 * scale,
        width: 85 * scale,
        height: 30 * scale,
        child: GestureDetector(
          onTap: () => _showDeleteDialog(context, scale, textColor, data),
          child: Text(
            'إزالة مرشد',
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

      // --- تعديل المعلومات ---
      Positioned(
        top: (cardTop + 249) * scale,
        left: 266 * scale,
        width: 32 * scale,
        height: 32 * scale,
        child: GestureDetector(
          onTap: () {
            print("تعديل معلومات المرشد: ${data['name']}");
          },
          child: SvgPicture.asset(
            'assets/icons/Edit_Square.svg',
            fit: BoxFit.contain,
            colorFilter: ColorFilter.mode(textColor, BlendMode.srcIn),
          ),
        ),
      ),
      Positioned(
        top: (cardTop + 286) * scale,
        left: 224 * scale,
        width: 116 * scale,
        height: 19 * scale,
        child: GestureDetector(
          onTap: () {
            print("تعديل معلومات المرشد: ${data['name']}");
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

  // نافذة الحذف المنبثقة
  void _showDeleteDialog(
    BuildContext context,
    double scale,
    Color textColor,
    Map<String, String> data,
  ) {
    const Color confirmColor = Color(0xFFDB6262);
    const Color dialogBgColor = Colors.white;
    const Color dialogBorderColor = Colors.black;
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return Dialog(
          backgroundColor: Colors.transparent,
          elevation: 0,
          child: Container(
            width: 380 * scale,
            padding: EdgeInsets.all(24 * scale),
            decoration: BoxDecoration(
              color: dialogBgColor,
              borderRadius: BorderRadius.circular(25 * scale),
              border: Border.all(color: dialogBorderColor, width: 1.5 * scale),
              boxShadow: const [
                BoxShadow(
                  color: Color(0x33000000),
                  blurRadius: 20,
                  offset: Offset(0, 10),
                ),
              ],
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 70 * scale,
                  height: 70 * scale,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      stops: const [0.0, 0.0433, 0.1731, 1.0],
                      colors: [
                        confirmColor,
                        confirmColor.withOpacity(0.55),
                        confirmColor.withOpacity(0.10),
                        confirmColor.withOpacity(0.10),
                      ],
                    ),
                  ),
                  child: Center(
                    child: Icon(
                      Icons.person_remove_rounded,
                      color: confirmColor,
                      size: 35 * scale,
                    ),
                  ),
                ),
                SizedBox(height: 16 * scale),
                Text(
                  'إزالة مرشد',
                  style: TextStyle(
                    fontFamily: 'Cairo',
                    fontSize: 26 * scale,
                    fontWeight: FontWeight.w700,
                    color: Colors.black,
                  ),
                ),
                SizedBox(height: 12 * scale),
                Text(
                  'هل أنت متأكد من حذف "${data['name']}"؟\nسيتم مسح بيانات المرشد ولا يمكن التراجع عن هذا الإجراء.',
                  textAlign: TextAlign.center,
                  textDirection: TextDirection.rtl,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 18 * scale,
                    fontWeight: FontWeight.w400,
                    color: Colors.black87,
                    height: 1.4,
                  ),
                ),
                SizedBox(height: 24 * scale),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    GestureDetector(
                      onTap: () => Navigator.pop(context),
                      child: Container(
                        width: 120 * scale,
                        height: 50 * scale,
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(15 * scale),
                          border: Border.all(
                            color: dialogBorderColor,
                            width: 1.2 * scale,
                          ),
                        ),
                        child: Center(
                          child: Text(
                            'تراجع',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20 * scale,
                              fontWeight: FontWeight.w500,
                              color: Colors.black,
                            ),
                          ),
                        ),
                      ),
                    ),
                    GestureDetector(
                      onTap: () {
                        Navigator.pop(context);
                      },
                      child: Container(
                        width: 130 * scale,
                        height: 50 * scale,
                        decoration: BoxDecoration(
                          color: confirmColor,
                          borderRadius: BorderRadius.circular(15 * scale),
                        ),
                        child: Center(
                          child: Text(
                            'حذف',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 18 * scale,
                              fontWeight: FontWeight.w500,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
