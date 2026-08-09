import 'package:flutter/material.dart';
import 'app_constants.dart';

const double kFigmaCanvasWidth = 430;

const String kDashboardFontFamily = 'Tajawal';

/// مقياس خاص بهذا الهيكل فقط، مبني على عرض فريم فيغما الحقيقي (430px)
/// بدل الاعتماد على context.scale العام (المبني على 390px في app_constants.dart)
/// هذا يضمن أن كل القياسات هنا (Positioned + الخطوط) تتناسب تماماً مع فيغما
/// وتكبر/تصغر ديناميكياً حسب حجم شاشة الجهاز، دون التأثير على أي ملف آخر.
extension FigmaScaleExtension on BuildContext {
  double get figmaScale => MediaQuery.of(this).size.width / kFigmaCanvasWidth;
}

class DashboardHeaderSection extends StatelessWidget {
  final DashboardStats stats;

  const DashboardHeaderSection({super.key, required this.stats});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: MediaQuery.of(context).size.width,
      height: 397 * context.figmaScale,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 30 * context.figmaScale,
            left: 25 * context.figmaScale,
            width: 90 * context.figmaScale,
            height: 90 * context.figmaScale,
            child: Image.asset(AppIcons.profileCircle, fit: BoxFit.contain),
          ),

          // ---------------- أيقونة الإشعارات ----------------
          // Layout: width 60, height 60, top 55, left 355
          Positioned(
            top: 45 * context.figmaScale,
            left: 320 * context.figmaScale,
            width: 60 * context.figmaScale,
            height: 60 * context.figmaScale,
            child: Image.asset(AppIcons.notification, fit: BoxFit.contain),
          ),

          // ---------------- بطاقة "عدد البرامج" (يسار) ----------------
          // خلفية البطاقة فقط لها مواصفات مؤكدة من فيغما:
          // Rectangle 111141438: width 200, height 125, top 132, left 13
          Positioned(
            top: 122 * context.figmaScale,
            left: 15 * context.figmaScale,
            width: 200 * context.figmaScale,
            height: 125 * context.figmaScale,
            child: _ProgramsCardPlaceholder(
              programsCount: stats.publishedProgramsCount,
            ),
          ),

          // ---------------- بطاقة "عدد السائحين" (يمين) ----------------
          // خلفية البطاقة: Rectangle 111141437: width 200, height 125, top 132, left 227
          Positioned(
            top: 122 * context.figmaScale,
            left: 218 * context.figmaScale,
            width: 200 * context.figmaScale,
            height: 125 * context.figmaScale,
            child: Stack(
              clipBehavior: Clip.none,
              children: [
                Image.asset(
                  AppIcons.rectangle77,
                  width: 200 * context.figmaScale,
                  height: 125 * context.figmaScale,
                  fit: BoxFit.fill,
                ),

                // النص: "عدد السائحين المنضمين لبرامجك"
                // Layout (نسبي للبطاقة): width 160, height 48, top 10, left 20
                Positioned(
                  top: 10 * context.figmaScale,
                  left: 20 * context.figmaScale,
                  width: 160 * context.figmaScale,
                  height: 48 * context.figmaScale,
                  child: _AppText(
                    'عدد السائحين المنضمين لبرامجك',
                    fontSize: 20 * context.figmaScale,
                    
                  ),
                ),

                // أيقونة الأشخاص
                // Layout (نسبي): width 50, height 50, top 59, left 122
                Positioned(
                  top: 55 * context.figmaScale,
                  left: 122 * context.figmaScale,
                  width: 50 * context.figmaScale,
                  height: 50 * context.figmaScale,
                  child: Image.asset(AppIcons.persons, fit: BoxFit.contain),
                ),

                // العدد "300"
                // Layout (نسبي): width 58, height 43, top 63, left 34
                Positioned(
                  top: 50 * context.figmaScale,
                  left: 34 * context.figmaScale,
                  width: 58 * context.figmaScale,
                  height: 43 * context.figmaScale,
                  child: _AppText(
                    '${stats.enrolledTouristsCount}',
                    fontSize: 36 * context.figmaScale,
                  ),
                ),

                // النص "سائح"
                // Layout (نسبي): width 45, height 24, top 94, left 40
                Positioned(
                  top: 83 * context.figmaScale,
                  left: 40 * context.figmaScale,
                  width: 45 * context.figmaScale,
                  height: 24 * context.figmaScale,
                  child: _AppText('سائح', fontSize: 20 * context.figmaScale),
                ),
              ],
            ),
          ),

          // ---------------- بطاقة "التقييمات والمراجعات" ----------------
          // خلفية البطاقة: Rectangle 111141438 (البرتقالية): width 403, height 125, top 272, left 18
          Positioned(
            top: 245 * context.figmaScale,
            left: 18 * context.figmaScale,
            width: 403 * context.figmaScale,
            height: 125 * context.figmaScale,
            child: Stack(
              clipBehavior: Clip.none,
              children: [
                Image.asset(
                  AppIcons.rectangle88,
                  width: 403 * context.figmaScale,
                  height: 125 * context.figmaScale,
                  fit: BoxFit.fill,
                ),

                // العنوان: "التقييمات والمراجعات التي أضافها السائحين عنك"
                // Layout (نسبي): width 285, height 48, top 15, left 59
                Positioned(
                  top: 5 * context.figmaScale,
                  left: 59 * context.figmaScale,
                  width: 285 * context.figmaScale,
                  height: 48 * context.figmaScale,
                  child: _AppText(
                    'التقييمات والمراجعات التي اضافها السائحين عنك',
                    fontSize: 20 * context.figmaScale,
                  ),
                ),

                // ---- نصف "التقييم" (يمين) - مواصفات مؤكدة من فيغما ----
                // أيقونة النجمة
                // Layout (نسبي): width 50, height 50, top 39, left 301
                Positioned(
                  top: 39 * context.figmaScale,
                  left: 301 * context.figmaScale,
                  width: 50 * context.figmaScale,
                  height: 50 * context.figmaScale,
                  child: Image.asset(AppIcons.star, fit: BoxFit.contain),
                ),

                // العدد "300"
                // Layout (نسبي): width 58, height 43, top 89, left 263
                Positioned(
                  top: 80 * context.figmaScale,
                  left: 263 * context.figmaScale,
                  width: 58 * context.figmaScale,
                  height: 43 * context.figmaScale,
                  child: _AppText(
                    '${stats.ratingsCount}',
                    fontSize: 36 * context.figmaScale,
                  ),
                ),

                // النص "تقييم"
                // Layout (نسبي): width 43, height 24, top 92, left 335
                Positioned(
                  top: 88 * context.figmaScale,
                  left: 335 * context.figmaScale,
                  width: 43 * context.figmaScale,
                  height: 24 * context.figmaScale,
                  child: _AppText('تقييم', fontSize: 20 * context.figmaScale),
                ),

                // ---- نصف "المراجعة" (يسار) ----
                // TODO: لم تصلنا بعد مواصفات فيغما الدقيقة لأيقونة note.png
                // ولا لموضع نص "مراجعة" والعدد الخاص بها.
                Positioned(
                  top: 39 * context.figmaScale,
                  left: 52 * context.figmaScale,
                  width: 45 * context.figmaScale,
                  height: 45 * context.figmaScale,
                  child: Image.asset(AppIcons.note, fit: BoxFit.contain),
                ),
                Positioned(
                  top: 80 * context.figmaScale,
                  left: 25 * context.figmaScale,
                  width: 58 * context.figmaScale,
                  height: 43 * context.figmaScale,
                  child: _AppText(
                    '${stats.reviewsCount}',
                    fontSize: 36 * context.figmaScale,
                  ),
                ),
                Positioned(
                  top: 88 * context.figmaScale,
                  left: 95 * context.figmaScale,
                  width: 60 * context.figmaScale,
                  height: 24 * context.figmaScale,
                  child: _AppText('مراجعة', fontSize: 20 * context.figmaScale),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// بطاقة "عدد البرامج" (يسار)

class _ProgramsCardPlaceholder extends StatelessWidget {
  final int programsCount;
  const _ProgramsCardPlaceholder({required this.programsCount});

  @override
  Widget build(BuildContext context) {
    return Stack(
      clipBehavior: Clip.none,
      children: [
        Image.asset(
          AppIcons.rectangle77,
          width: 200 * context.figmaScale,
          height: 125 * context.figmaScale,
          fit: BoxFit.fill,
        ),
        Positioned(
          top: 10 * context.figmaScale,
          left: 20 * context.figmaScale,
          width: 160 * context.figmaScale,
          height: 48 * context.figmaScale,
          child: _AppText(
            'عدد البرامج السياحية المنشورة',
            fontSize: 20 * context.figmaScale,
          ),
        ),
        Positioned(
          top: 55 * context.figmaScale,
          left: 122 * context.figmaScale,
          width: 50 * context.figmaScale,
          height: 50 * context.figmaScale,
          child: Image.asset(AppIcons.proCount, fit: BoxFit.contain),
        ),
        Positioned(
          top: 50 * context.figmaScale,
          left: 34 * context.figmaScale,
          width: 58 * context.figmaScale,
          height: 43 * context.figmaScale,
          child: _AppText('$programsCount', fontSize: 36 * context.figmaScale),
        ),
        Positioned(
          top: 83 * context.figmaScale,
          left: 40 * context.figmaScale,
          width: 45 * context.figmaScale,
          height: 24 * context.figmaScale,
          child: _AppText('برامج', fontSize: 20 * context.figmaScale),
        ),
      ],
    );
  }
}

/// عنوان "أكثر برامج طلباً"
/// قياسات فيغما الدقيقة (بالنسبة لكامل الفريم 430px):
/// Width 221 / Height 43 / Top 412 / Left 162
/// Font: Tajawal / Weight 400 / Size 36 / Line height 100% / Letter spacing 0%
/// Horizontal align: Center / Vertical align: Middle / Color: #000000
/// (الهيكل العام ينتهي عند top 397، لذلك الهامش العلوي هنا = 412 - 397 = 15)
class MostRequestedProgramsTitle extends StatelessWidget {
  const MostRequestedProgramsTitle({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(
        top: -10 * context.figmaScale,
        left: 162 * context.figmaScale,
      ),
      child: SizedBox(
        width: 221 * context.figmaScale,
        height: 43 * context.figmaScale,
        child: Center(
          child: Text(
            'أكثر برامج طلباً',
            textAlign: TextAlign.center,
            textDirection: TextDirection.rtl,
            style: TextStyle(
              fontFamily: kDashboardFontFamily,
              fontWeight: FontWeight.w400,
              fontSize: 36 * context.figmaScale,
              height: 1.0,
              letterSpacing: 0,
              color: const Color(0xFF000000),
            ),
          ),
        ),
      ),
    );
  }
}

/// نص موحّد بخط Tajawal حسب مواصفات فيغما
/// (Weight 400 / Regular, Line height 100%, Letter spacing 0%,
///  Horizontal align: Center, Vertical align: Middle, Color: #000000)
class _AppText extends StatelessWidget {
  final String text;
  final double fontSize;

  const _AppText(this.text, {required this.fontSize});

  @override
  Widget build(BuildContext context) {
    // FittedBox(scaleDown) يضمن أن النص يبقى دائماً داخل حدود الصندوق
    // (width/height) المحدد له في Positioned، فإذا كان النص طويلاً ولا يتسع
    // بحجم الخط الأصلي، يصغّر تلقائياً بدل أن يلتف لأسطر إضافية تتداخل مع
    // العناصر المجاورة (كما كان يحدث مع عنوان "عدد السائحين المنضمين لبرامجك").
    // لا يغيّر هذا أي قياس/موضع، فقط يمنع التداخل البصري.
    return FittedBox(
      fit: BoxFit.scaleDown,
      child: Text(
        text,
        textAlign: TextAlign.center,
        textDirection: TextDirection.rtl,
        style: TextStyle(
          fontFamily: kDashboardFontFamily,
          fontWeight: FontWeight.w400,
          fontSize: fontSize,
          height: 1.0,
          letterSpacing: 0,
          color: const Color(0xFF000000),
        ),
      ),
    );
  }
}
