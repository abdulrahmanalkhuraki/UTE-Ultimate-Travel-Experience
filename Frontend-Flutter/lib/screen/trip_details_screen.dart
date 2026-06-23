import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:ute_app/model/app_svg.dart';
import 'package:ute_app/screen/home_screen.dart';
import 'package:ute_app/utils/constants.dart';

class TripDetailsScreen extends StatelessWidget {
  const TripDetailsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    SystemChrome.setSystemUIOverlayStyle(
      const SystemUiOverlayStyle(
        statusBarColor: Colors.transparent,
        statusBarIconBrightness: Brightness.dark,
      ),
    );

    return Scaffold(
      backgroundColor: Colors.white,
      body: Column(
        children: [
          // ── 1. AppBar الجديد ──────────────────────────
          _buildAppBar(context),

          Expanded(
            child: Stack(
              children: [
                // ── 2. الصورة ─────────────────────────────────
                Positioned(
                  top: 0,
                  left: 0,
                  right: 0,
                  height: 300.h,
                  child: Image.asset(
                    'assets/images/paris_image.png',
                    fit: BoxFit.cover,
                    errorBuilder: (_, _, _) =>
                        Container(color: const Color(0xFFD6E4FF)),
                  ),
                ),

                // ── 3. البانل الشفاف ──────────────────────────
                Positioned(
                  top: 240.h,
                  left: 10,
                  right: 10,
                  bottom: 80,
                  child: FutureBuilder(
                    future: Future.delayed(Duration.zero),
                    builder: (context, snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        return const SizedBox.shrink();
                      }
                      return const _DetailsPanel();
                    },
                  ),
                ),

                // ── 4. BottomNav ──────────────────────────────
                Positioned(
                  bottom: -5,
                  left: 0,
                  right: 0,
                  child: _BottomNav(selectedIndex: 2, onTap: (_) {}),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAppBar(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(
        top: MediaQuery.of(context).padding.top + 10.h,
        bottom: 10.h,
        left: 20.w,
        right: 20.w,
      ),
      decoration: BoxDecoration(
        color: const Color(
          0xFFD6E4FF,
        ), // لون الخلفية الأزرق الفاتح كما في الصورة
        border: Border(
          bottom: BorderSide(
            color: Colors.blueAccent.withOpacity(0.3),
            width: 1,
          ),
        ),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          // الدائرة البرتقالية على اليسار
          Container(
            width: 48.w,
            height: 48.w,
            decoration: const BoxDecoration(
              color: Color(0xFFF4A261),
              shape: BoxShape.circle,
            ),
          ),

          // العنوان في المنتصف
          Text(
            'التفاصيل',
            style: TextStyle(
              fontSize: 24.sp,
              fontWeight: FontWeight.bold,
              color: const Color(0xFF2D264B),
              fontFamily: 'Cairo',
            ),
          ),

          // سهم الرجوع على اليمين
          GestureDetector(
            onTap: () => Get.to(HomeScreenProvider()),
            child: Icon(
              Icons.chevron_right,
              color: const Color(0xFF2D264B),
              size: 30.sp,
            ),
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────
//  البانل الشفاف (باقي الكود كما هو)
// ─────────────────────────────────────────────
class _DetailsPanel extends StatelessWidget {
  const _DetailsPanel();

  static BoxDecoration get _cardDecoration => BoxDecoration(
    color: Colors.transparent,
    borderRadius: BorderRadius.circular(14.r),
    border: Border.all(color: Colors.black, width: 1.5), 
  );

  static BoxDecoration get _shadowCardDecoration => BoxDecoration(
    color: Colors.transparent,
    borderRadius: BorderRadius.circular(14.r),
    border: Border.all(color: Colors.black, width: 1),
    boxShadow: const [
      BoxShadow(color: Colors.black26, blurRadius: 4, offset: Offset(0, 4)),
    ],
  );

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.only(
        topLeft: Radius.circular(40.r),
        topRight: Radius.circular(40.r),
      ),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 5, sigmaY: 5),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.only(
              topLeft: Radius.circular(40.r),
              topRight: Radius.circular(40.r),
            ),
            border: Border.all(
              color: Colors.black.withOpacity(0.5),
              width: 0.5,
            ),
            gradient: const LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              stops: [0.0, 0.0433, 0.1731],
              colors: [Color(0xFF91B3FA), Color(0x8C91B3FA), Color(0x1A91B3FA)],
            ),
          ),
          child: SingleChildScrollView(
            physics: const BouncingScrollPhysics(),
            padding: EdgeInsets.symmetric(horizontal: 16.w, vertical: 20.h),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Container(
                  padding: EdgeInsets.symmetric(vertical: 14.h),
                  child: Text(
                    'شركة مدري شو للسياحة والسفر',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontSize: 16.sp,
                      fontWeight: FontWeight.w700,
                      color: Colors.white,
                      fontFamily: 'Cairo',
                    ),
                  ),
                ),
                SizedBox(height: 10.h),
                Container(
                  padding: EdgeInsets.all(14.w),
                  decoration: _cardDecoration,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.end,
                        children: [
                          Text(
                            'وصف',
                            style: TextStyle(
                              fontSize: 14.sp,
                              fontWeight: FontWeight.w700,
                              color: const Color(0xFF2D264B),
                              fontFamily: 'Cairo',
                            ),
                          ),
                          SizedBox(width: 6.w),
                          const Icon(
                            Icons.subject_rounded,
                            size: 18,
                            color: Color(0xFF2D264B),
                          ),
                        ],
                      ),
                      SizedBox(height: 8.h),
                      Text(
                        'رحلة ساحرة مع الكثير من الاثارة التشويق وزيارة معظم الاماكن السياحية الجميلة في فرنسا متضمنة تذاكر الطيران والاقامة في الفنادق ووجبات فاخرة في افخر المطاعم، ما عليك سوا حزم أمتعتك والانضمام الى هذه المغامرة الشيقة.',
                        textAlign: TextAlign.right,
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontSize: 12.sp,
                          color: const Color(0xFF2D264B),
                          height: 1.7,
                          fontFamily: 'Cairo',
                        ),
                      ),
                      SizedBox(height: 12.h),
                      Divider(color: Colors.black.withOpacity(0.15)),
                      SizedBox(height: 12.h),
                      Row(
                        textDirection: TextDirection.rtl,
                        children: [
                          Expanded(
                            child: Row(
                              textDirection: TextDirection.rtl,
                              children: [
                                const Icon(
                                  Icons.flight,
                                  color: Color(0xFF2D264B),
                                  size: 18,
                                ),
                                SizedBox(width: 6.w),
                                Expanded(
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.end,
                                    children: [
                                      Text(
                                        '5 أيام',
                                        style: TextStyle(
                                          fontSize: 12.sp,
                                          fontWeight: FontWeight.w700,
                                          color: const Color(0xFF2D264B),
                                          fontFamily: 'Cairo',
                                        ),
                                      ),
                                      Text(
                                        'من 18\u060c6 إلى 28\u060c6\u060c2026',
                                        style: TextStyle(
                                          fontSize: 10.sp,
                                          color: Colors.grey[700],
                                          fontFamily: 'Cairo',
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Container(
                            width: 1,
                            height: 40,
                            color: Colors.black.withOpacity(0.15),
                            margin: EdgeInsets.symmetric(horizontal: 10.w),
                          ),
                          Expanded(
                            child: Row(
                              textDirection: TextDirection.rtl,
                              children: [
                                const Icon(
                                  Icons.location_on_outlined,
                                  color: Color(0xFF2D264B),
                                  size: 18,
                                ),
                                SizedBox(width: 6.w),
                                Expanded(
                                  child: Column(
                                    crossAxisAlignment: CrossAxisAlignment.end,
                                    children: [
                                      Text(
                                        'الإمارات العربية المتحدة',
                                        style: TextStyle(
                                          fontSize: 11.sp,
                                          fontWeight: FontWeight.w700,
                                          color: const Color(0xFF2D264B),
                                          fontFamily: 'Cairo',
                                        ),
                                      ),
                                      Text(
                                        'دبي-برج خليفة\nعجمان-متحف المستقبل\nأبو ظبي-الشارقة',
                                        style: TextStyle(
                                          fontSize: 9.sp,
                                          color: Colors.grey[700],
                                          fontFamily: 'Cairo',
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                SizedBox(height: 10.h),
                Row(
                  mainAxisAlignment: MainAxisAlignment.start,
                  children: List.generate(
                    5,
                    (i) => Icon(
                      i < 3 ? Icons.star_rounded : Icons.star_border_rounded,
                      color: i < 3 ? const Color(0xFFFFE600) : Colors.black,
                      size: 28,
                    ),
                  ),
                ),
                SizedBox(height: 14.h),
                Text(
                  'التفاصيل الكاملة للرحلة',
                  textAlign: TextAlign.right,
                  textDirection: TextDirection.rtl,
                  style: TextStyle(
                    fontSize: 14.sp,
                    fontWeight: FontWeight.w700,
                    color: const Color(0xFF2D264B),
                    fontFamily: 'Cairo',
                  ),
                ),
                SizedBox(height: 8.h),
                ...List.generate(5, (index) => _DayCard(dayNumber: index + 1)),
                const _ReviewsSection(), 
                SizedBox(height: 16.h),
                Row(
                  children: [
                    Expanded(
                      flex: 3,
                      child: Container(
                        padding: EdgeInsets.all(14.w),
                        decoration: _shadowCardDecoration,
                        child: Column(
                          children: [
                            Text(
                              'تكلفة الانضمام للبرنامج',
                              style: TextStyle(
                                fontSize: 11.sp,
                                color: Colors.grey[700],
                                fontFamily: 'Cairo',
                              ),
                            ),
                            Text(
                              '200,000,00 USD',
                              style: TextStyle(
                                fontSize: 20.sp,
                                fontWeight: FontWeight.w800,
                                color: const Color(0xFF2D264B),
                                fontFamily: 'Cairo',
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    SizedBox(width: 10.w),
                    Expanded(
                      flex: 2,
                      child: Container(
                        padding: EdgeInsets.all(14.w),
                        decoration: _shadowCardDecoration,
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.center,
                          children: [
                            const Icon(
                              Icons.hourglass_bottom_rounded,
                              color: Color(0xFF2D264B),
                              size: 20,
                            ),
                            Text(
                              'متبقي',
                              style: TextStyle(
                                fontSize: 11.sp,
                                color: Colors.grey[700],
                                fontFamily: 'Cairo',
                              ),
                            ),
                            Text(
                              '6',
                              style: TextStyle(
                                fontSize: 28.sp,
                                fontWeight: FontWeight.w800,
                                color: const Color(0xFF2D264B),
                                fontFamily: 'Cairo',
                              ),
                            ),
                            Text(
                              'لانتهاء التسجيل',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontSize: 9.sp,
                                color: Colors.grey[700],
                                fontFamily: 'Cairo',
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
                SizedBox(height: 16.h),
                GestureDetector(
                  onTap: () {},
                  child: Container(
                    height: 64.h,
                    decoration: BoxDecoration(
                      color: const Color(0xFFF4A261),
                      borderRadius: BorderRadius.circular(20.r),
                      border: Border.all(color: Colors.black, width: 1),
                      boxShadow: const [
                        BoxShadow(
                          color: Colors.black26,
                          blurRadius: 4,
                          offset: Offset(0, 4),
                        ),
                      ],
                    ),
                    child: Center(
                      child: Text(
                        'انضم الآن',
                        style: TextStyle(
                          fontSize: 20.sp,
                          fontWeight: FontWeight.bold,
                          color: Colors.white,
                          fontFamily: 'Cairo',
                        ),
                      ),
                    ),
                  ),
                ),
                SizedBox(height: 20.h),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _DayCard extends StatefulWidget {
  final int dayNumber;
  const _DayCard({required this.dayNumber});

  @override
  State<_DayCard> createState() => _DayCardState();
}

class _DayCardState extends State<_DayCard>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _controller;
  late Animation<double> _heightFactor;

  static const _dayNames = ['الأول', 'الثاني', 'الثالث', 'الرابع', 'الخامس'];
  // بيانات كل يوم
  static const _dayDetails = [
    'الوصول إلى المطار والانتقال إلى الفندق الفاخر. استقبال حار من فريق العمل وتسجيل الوصول. وجبة عشاء ترحيبية في مطعم فاخر.',
    'جولة في أبرز المعالم السياحية. زيارة برج إيفل والمتاحف الشهيرة. غداء في مطعم فرنسي أصيل.',
    'رحلة إلى القصور الملكية وحدائق فرساي. تسوق في الشانزليزيه. مساء ترفيهي مع عرض ضوئي.',
    'يوم للاسترخاء والسبا في الفندق. جولة بحرية على نهر السين. عشاء رومانسي فوق برج إيفل.',
    'تسوق أخير وزيارة الأسواق الشعبية. الانتقال إلى المطار. وداع بحفاوة من فريق البرنامج.',
  ];

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 300),
    );
    _heightFactor = CurvedAnimation(
      parent: _controller,
      curve: Curves.easeInOut,
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _toggle() {
    setState(() => _expanded = !_expanded);
    if (_expanded) {
      _controller.forward();
    } else {
      _controller.reverse();
    }
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: _toggle,
      child: Container(
        margin: EdgeInsets.only(bottom: 8.h),
        decoration: BoxDecoration(
          color: Colors.transparent,
          borderRadius: BorderRadius.circular(14.r),
          border: Border.all(color: Colors.black, width: 1.5),
        ),
        child: Column(
          children: [
            // ── رأس البطاقة ──────────────────────────────
            Padding(
              padding: EdgeInsets.symmetric(
                  horizontal: 16.w, vertical: 16.h),
              child: Row(
                textDirection: TextDirection.ltr,
                children: [
                  // سهم يتحول عند التوسع
                  AnimatedRotation(
                    turns: _expanded ? 0.25 : 0,
                    duration: const Duration(milliseconds: 300),
                    child: const Icon(
                      Icons.chevron_left,
                      color: Color(0xFF2D264B),
                      size: 22,
                    ),
                  ),
                  SizedBox(width: 8.w),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Text(
                          'اليوم ${_dayNames[widget.dayNumber - 1]}',
                          style: TextStyle(
                            fontSize: 14.sp,
                            fontWeight: FontWeight.w700,
                            color: const Color(0xFF2D264B),
                            fontFamily: 'Cairo',
                          ),
                        ),
                        Text(
                          'الوصول والاستقبال في الفندق',
                          style: TextStyle(
                            fontSize: 12.sp,
                            color: Colors.grey[600],
                            fontFamily: 'Cairo',
                          ),
                        ),
                      ],
                    ),
                  ),
                  SizedBox(width: 10.w),
                  SizedBox(
                    width: 47.w,
                    height: 52.h,
                    child: Stack(
                      alignment: Alignment.center,
                      children: [
                        SvgPicture.string(
                         AppSvg.calendar,
                          width: 47.w,
                          height: 52.h,
                        ),
                        Positioned(
                          top: 22.h,
                          child: Text(
                            '18/6',
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 9.sp,
                              fontWeight: FontWeight.w700,
                              color: const Color(0xFF2D264B),
                              fontFamily: 'Cairo',
                              height: 1.2,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),

            // ── التفاصيل المتوسعة ─────────────────────────
            SizeTransition(
              sizeFactor: _heightFactor,
              axisAlignment: -1,
              child: Container(
                width: double.infinity,
                padding: EdgeInsets.only(
                  left: 16.w,
                  right: 16.w,
                  bottom: 16.h,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Divider(
                      color: Colors.black.withOpacity(0.15),
                      height: 1,
                    ),
                    SizedBox(height: 10.h),
                    Text(
                      _dayDetails[widget.dayNumber - 1],
                      textAlign: TextAlign.right,
                      textDirection: TextDirection.rtl,
                      style: TextStyle(
                        fontSize: 12.sp,
                        color: const Color(0xFF2D264B),
                        height: 1.7,
                        fontFamily: 'Cairo',
                      ),
                    ),
                    SizedBox(height: 10.h),
                    // أيقونات الأنشطة
                    Row(
                      mainAxisAlignment: MainAxisAlignment.end,
                      children: [
                        _ActivityChip(
                            icon: Icons.hotel_rounded, label: 'فندق'),
                        SizedBox(width: 8.w),
                        _ActivityChip(
                            icon: Icons.restaurant_rounded, label: 'وجبات'),
                        SizedBox(width: 8.w),
                        _ActivityChip(
                            icon: Icons.directions_bus_rounded,
                            label: 'نقل'),
                      ],
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

// ── ويدجت صغير للأنشطة ──────────────────────────────────
class _ActivityChip extends StatelessWidget {
  final IconData icon;
  final String label;
  const _ActivityChip({required this.icon, required this.label});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.symmetric(horizontal: 8.w, vertical: 4.h),
      decoration: BoxDecoration(
        color: const Color(0xFFD6E4FF).withOpacity(0.5),
        borderRadius: BorderRadius.circular(8.r),
        border: Border.all(
          color: Colors.black.withOpacity(0.15),
          width: 1,
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: 10.sp,
              color: const Color(0xFF2D264B),
              fontFamily: 'Cairo',
            ),
          ),
          SizedBox(width: 4.w),
          Icon(icon, size: 14, color: const Color(0xFF2D264B)),
        ],
      ),
    );
  }
}

class _BottomNav extends StatelessWidget {
  final int selectedIndex;
  final ValueChanged<int> onTap;
  const _BottomNav({required this.selectedIndex, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
      height: 70.h,
      decoration: BoxDecoration(
        color: NavBarConstants.navBgColor,
        borderRadius: BorderRadius.circular(40),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: List.generate(NavBarConstants.icons.length, (i) {
          final selected = i == selectedIndex;
          return GestureDetector(
            onTap: () => onTap(i),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                selected
                    ? _PentagonIcon(icon: NavBarConstants.icons[i])
                    : Icon(
                        NavBarConstants.icons[i],
                        color: NavBarConstants.inactiveColor,
                        size: 26,
                      ),
                if (NavBarConstants.labels[i].isNotEmpty)
                  Text(
                    NavBarConstants.labels[i],
                    style: TextStyle(
                      fontSize: 10,
                      fontFamily: 'Cairo',
                      color: selected
                          ? NavBarConstants.activeColor
                          : NavBarConstants.inactiveColor,
                    ),
                  ),
              ],
            ),
          );
        }),
      ),
    );
  }
}

class _PentagonIcon extends StatelessWidget {
  final IconData icon;
  const _PentagonIcon({required this.icon});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 48,
      height: 48,
      child: Stack(
        alignment: Alignment.center,
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: const BoxDecoration(
              color: Color(0xFFF4A261),
              shape: BoxShape.circle,
            ),
          ),
          Icon(icon, color: Colors.white, size: 24),
        ],
      ),
    );
  }
}

class _ReviewsSection extends StatefulWidget {
  const _ReviewsSection();

  @override
  State<_ReviewsSection> createState() => _ReviewsSectionState();
}

class _ReviewsSectionState extends State<_ReviewsSection>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _controller;
  late Animation<double> _heightFactor;

  static const _reviews = [
    (
      name: 'أحمد الزهراني',
      stars: 5,
      text: 'رحلة رائعة ومنظمة باحترافية عالية! كل شيء كان مثاليًا من الفندق إلى المواصلات.',
      date: 'مارس 2025'
    ),
    (
      name: 'سارة العمري',
      stars: 4,
      text: 'تجربة ممتازة، الدليل السياحي كان محترفًا ومتعاونًا جدًا. سأكرر التجربة.',
      date: 'فبراير 2025'
    ),
    (
      name: 'محمد الغامدي',
      stars: 5,
      text: 'برنامج متكامل وشامل، لم نشعر بأي تعب أو إجهاد. شكرًا للشركة المنظمة!',
      date: 'يناير 2025'
    ),
  ];

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 350),
    );
    _heightFactor = CurvedAnimation(
      parent: _controller,
      curve: Curves.easeInOut,
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _toggle() {
    setState(() => _expanded = !_expanded);
    if (_expanded) {
      _controller.forward();
    } else {
      _controller.reverse();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // ── زر فتح/إغلاق المراجعات ───────────────────────
        GestureDetector(
          onTap: _toggle,
          child: Container(
            padding: EdgeInsets.symmetric(
                horizontal: 16.w, vertical: 14.h),
            decoration: BoxDecoration(
              color: Colors.transparent,
              borderRadius: BorderRadius.circular(14.r),
              border: Border.all(color: Colors.black, width: 1.5),
            ),
            child: Row(
              textDirection: TextDirection.rtl,
              children: [
                const Icon(Icons.rate_review_rounded,
                    color: Color(0xFF2D264B), size: 20),
                SizedBox(width: 8.w),
                Text(
                  'مراجعات السائحين',
                  style: TextStyle(
                    fontSize: 14.sp,
                    fontWeight: FontWeight.w700,
                    color: const Color(0xFF2D264B),
                    fontFamily: 'Cairo',
                  ),
                ),
                const Spacer(),
                // عدد المراجعات
                Container(
                  padding: EdgeInsets.symmetric(
                      horizontal: 8.w, vertical: 2.h),
                  decoration: BoxDecoration(
                    color: const Color(0xFFF4A261),
                    borderRadius: BorderRadius.circular(20.r),
                  ),
                  child: Text(
                    '${_reviews.length}',
                    style: TextStyle(
                      fontSize: 11.sp,
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                      fontFamily: 'Cairo',
                    ),
                  ),
                ),
                SizedBox(width: 8.w),
                AnimatedRotation(
                  turns: _expanded ? -0.25 : 0,
                  duration: const Duration(milliseconds: 300),
                  child: const Icon(
                    Icons.chevron_left,
                    color: Color(0xFF2D264B),
                    size: 22,
                  ),
                ),
              ],
            ),
          ),
        ),

        // ── قائمة المراجعات المتوسعة ─────────────────────
        SizeTransition(
          sizeFactor: _heightFactor,
          axisAlignment: -1,
          child: Column(
            children: [
              SizedBox(height: 8.h),
              ..._reviews.map((review) => _ReviewCard(review: review)),
            ],
          ),
        ),
      ],
    );
  }
}

// ── بطاقة مراجعة واحدة ──────────────────────────────────
class _ReviewCard extends StatelessWidget {
  final ({String name, int stars, String text, String date}) review;
  const _ReviewCard({required this.review});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.only(bottom: 8.h),
      padding: EdgeInsets.all(14.w),
      decoration: BoxDecoration(
        color: Colors.transparent,
        borderRadius: BorderRadius.circular(14.r),
        border: Border.all(
          color: Colors.black.withOpacity(0.3),
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          // اسم + تاريخ
          Row(
            textDirection: TextDirection.rtl,
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                review.date,
                style: TextStyle(
                  fontSize: 10.sp,
                  color: Colors.grey[500],
                  fontFamily: 'Cairo',
                ),
              ),
              Text(
                review.name,
                style: TextStyle(
                  fontSize: 13.sp,
                  fontWeight: FontWeight.w700,
                  color: const Color(0xFF2D264B),
                  fontFamily: 'Cairo',
                ),
              ),
            ],
          ),
          SizedBox(height: 6.h),
          // نجوم
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: List.generate(
              5,
              (i) => Icon(
                i < review.stars
                    ? Icons.star_rounded
                    : Icons.star_border_rounded,
                color: i < review.stars
                    ? const Color(0xFFFFE600)
                    : Colors.black,
                size: 18,
              ),
            ),
          ),
          SizedBox(height: 8.h),
          // نص المراجعة
          Text(
            review.text,
            textAlign: TextAlign.right,
            textDirection: TextDirection.rtl,
            style: TextStyle(
              fontSize: 12.sp,
              color: const Color(0xFF2D264B),
              height: 1.6,
              fontFamily: 'Cairo',
            ),
          ),
        ],
      ),
    );
  }
}
