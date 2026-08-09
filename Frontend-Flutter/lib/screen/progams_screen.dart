import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'app_constants.dart';
import 'trip_shared_widgets.dart'; 


class JoinedTourist {
  final String name;
  final int daysAgo;
  final String? photoUrl; 

  JoinedTourist({required this.name, required this.daysAgo, this.photoUrl});
}

class Program {
  final String tripDaysAgo;
  final String status;
  final String name;
  final String imagePath;
  final String tourists; 
  final String maxTourists;
  final int daysToRegister;
  final int daysToStart;
  final int stars;
  final List<JoinedTourist> joinedTourists; 

  Program({
    required this.tripDaysAgo,
    required this.status,
    required this.name,
    required this.imagePath,
    required this.tourists,
    required this.maxTourists,
    required this.daysToRegister,
    required this.daysToStart,
    required this.stars,
    required this.joinedTourists,
  });
}

// ════════════════════════════════════════════════════════
// موديل برنامج "ملغى / مرفوض" (تبويب الملغاة)
// ════════════════════════════════════════════════════════
class CancelledProgram {
  final String photoPath;
  final String daysAgoText; 
  final String statusLabel; 
  final String name;
  final String descriptionText;
  final String? reasonText; 

  CancelledProgram({
    required this.photoPath,
    required this.daysAgoText,
    required this.statusLabel,
    required this.name,
    required this.descriptionText,
    this.reasonText,
  });
}

// ════════════════════════════════════════════════════════
// موديل برنامج "سابق" (تبويب السابقة)
// ════════════════════════════════════════════════════════
class PreviousProgram {
  final String photoPath;
  final String daysAgoText;
  final int timesCount; 
  final String name;
  final int reviewsCount; 
  final int totalEarned; 
  final int touristsCount; 
  final int stars;

  PreviousProgram({
    required this.photoPath,
    required this.daysAgoText,
    required this.timesCount,
    required this.name,
    required this.reviewsCount,
    required this.totalEarned,
    required this.touristsCount,
    required this.stars,
  });
}

final List<Program> myPrograms = [
  Program(
    tripDaysAgo: 'قبل 7 أيام',
    status: 'للمرة الخامسة',
    name: "برنامج اسم البرنامج",
    imagePath: 'assets/icons/Rectangle12.png',
    tourists: "30",
    maxTourists: '60',
    daysToRegister: 6,
    daysToStart: 10,
    stars: 5,
    joinedTourists: [
      JoinedTourist(name: 'عبد الرحمن الخرقي', daysAgo: 5),
      JoinedTourist(name: 'عبد الرحمن الخرقي', daysAgo: 7),
      JoinedTourist(name: 'عبد الرحمن الخرقي', daysAgo: 11),
    ],
  ),
  Program(
    tripDaysAgo: 'قبل 3 أيام',
    status: 'للمرة الاولى',
    name: "رحلة جبال الألب",
    imagePath: 'assets/icons/Rectangle12.png',
    tourists: "40",
    maxTourists: '70',
    daysToRegister: 2,
    daysToStart: 15,
    stars: 4,
    joinedTourists: [
      JoinedTourist(name: 'سارة العتيبي', daysAgo: 2),
      JoinedTourist(name: 'محمد القحطاني', daysAgo: 3),
    ],
  ),
];

final List<CancelledProgram> cancelledPrograms = [
  CancelledProgram(
    photoPath: 'assets/icons/Rectangle5.png',
    daysAgoText: 'قبل 5 اسابيع',
    statusLabel: 'الغاء',
    name: 'برنامج اسم البرنامج',
    descriptionText: 'قمت بإلغاء هذا البرنامج في 2026\\5\\19',
  ),
  CancelledProgram(
    photoPath: 'assets/icons/Rectangle5.png',
    daysAgoText: 'قبل 5 اسابيع',
    statusLabel: 'رفض',
    name: 'برنامج اسم البرنامج',
    descriptionText: 'تم رفض البرنامج من قبل مدير التطبيق',
    reasonText:
        'أضف وصفاً عاماً عن الرحلة مع ما ستتضمنه من خدمات وفنادق ومطاعم بشكل مختصر يجذب السياح بمجرد قرائته',
  ),
];

final List<PreviousProgram> previousPrograms = [
  PreviousProgram(
    photoPath: 'assets/icons/Rectangle12.png',
    daysAgoText: 'قبل 5 اسابيع',
    timesCount: 1,
    name: 'برنامج اسم البرنامج',
    reviewsCount: 300,
    totalEarned: 2500000,
    touristsCount: 300,
    stars: 5,
  ),
  PreviousProgram(
    photoPath: 'assets/icons/Rectangle12.png',
    daysAgoText: 'قبل 11 اسابيع',
    timesCount: 8,
    name: 'برنامج اسم البرنامج',
    reviewsCount: 500,
    totalEarned: 300000,
    touristsCount: 410,
    stars: 5,
  ),
  PreviousProgram(
    photoPath: 'assets/icons/Rectangle12.png',
    daysAgoText: 'قبل 3 اسابيع',
    timesCount: 4,
    name: 'برنامج اسم البرنامج',
    reviewsCount: 700,
    totalEarned: 74000,
    touristsCount: 200,
    stars: 5,
  ),
];

class ProgramsScreen extends StatefulWidget {
  const ProgramsScreen({super.key});

  @override
  State<ProgramsScreen> createState() => _ProgramsScreenState();
}

class _ProgramsScreenState extends State<ProgramsScreen> {
  int _selectedTabIndex = 1;
  bool _isExpanded = false;

  final program = myPrograms[0];

  double get _cardWidth => 380 * context.scale;
  double get _cardHeight => (_isExpanded ? 540 : 220) * context.scale;
  String get _cardBackgroundAsset => _isExpanded
      ? 'assets/icons/Rectangle33.png' 
      : 'assets/icons/Rectangle12.png'; 

  String _formatAmount(int amount) {
    final String digits = amount.toString();
    final StringBuffer buffer = StringBuffer();
    final int length = digits.length;
    for (int i = 0; i < length; i++) {
      buffer.write(digits[i]);
      final int remaining = length - i - 1;
      if (remaining > 0 && remaining % 3 == 0) {
        buffer.write(',');
      }
    }
    return buffer.toString();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Container(
            width: double.infinity,
            height: double.infinity,
            decoration: AppColors.backgroundGradient,
          ),
          SafeArea(
            child: Column(
              children: [
                Padding(
                  padding: EdgeInsets.only(
                    top: 20 * context.scale,
                    left: 20 * context.scale,
                    right: 20 * context.scale,
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                       children: [
                   
                    const SizedBox(width: 48),

                        Expanded(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: Text(
                              ' برامجي',
                              textAlign: TextAlign.center,
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: const TextStyle(
                                fontFamily: 'Cairo',
                                fontWeight: FontWeight.w700,
                                fontSize: 40,
                                height: 1.0,
                                letterSpacing: 2,
                                color: Color(0xFF000000),
                              ),
                            ),
                          ),
                        ),
                        const CustomBackButton(),
                    ],
                  ),
                ),

                Padding(
                  padding: EdgeInsets.symmetric(
                    vertical: 8 * context.scale,
                    horizontal: 32 * context.scale,
                  ),
                  child: Container(
                    width: double.infinity,
                    height: 1.2,
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          const Color(0xFF666666).withOpacity(0.5),
                          Colors.black,
                          const Color(0xFF666666).withOpacity(0.5),
                        ],
                      ),
                    ),
                  ),
                ),

                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 15 * context.scale),
                  child: Container(
                    width: 370 * context.scale,
                    height: 65 * context.scale,
                    decoration: BoxDecoration(
                      color: const Color(0xFFF4A261).withOpacity(0.36),
                      borderRadius: BorderRadius.circular(20 * context.scale),
                    ),
                    child: Row(
                      children: [
                        Expanded(child: _buildTab('الملغاة', 2)),
                        Expanded(child: _buildTab('الحالية', 1)),
                        Expanded(child: _buildTab('السابقة', 0)),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 10),

                Expanded(
                  child: Builder(
                    builder: (context) {
                      if (_selectedTabIndex == 1) {
                        return SingleChildScrollView(
                          child: _buildProgramCard(),
                        );
                      } else if (_selectedTabIndex == 2) {
                        return ListView.builder(
                          padding: EdgeInsets.only(
                            top: 50 * context.scale,
                            left: 10 * context.scale,
                            right: 10 * context.scale,
                            bottom: 30 * context.scale,
                          ),
                          itemCount: cancelledPrograms.length,
                          itemBuilder: (context, index) =>
                              _buildCancelledCard(cancelledPrograms[index]),
                        );
                      } else {
                        return SingleChildScrollView(
                          padding: EdgeInsets.only(
                            top:5 * context.scale, 
                            left: 10 * context.scale,
                            right: 10 * context.scale,
                            bottom: 30 * context.scale,
                          ),
                          child: Column(
                            children: previousPrograms.map((program) {
                              bool isFirst =
                                  previousPrograms.indexOf(program) == 0;
                              return _buildPreviousCard(
                                program,
                                isFirst: isFirst,
                              );
                            }).toList(),
                          ),
                        );
                      }
                    },
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  // ══════════════════════════════════════════════════
  //  تبويب الحالية"
  // ══════════════════════════════════════════════════
  Widget _buildProgramCard() {
    return GestureDetector(
      onTap: () {
        setState(() {
          _isExpanded = !_isExpanded;
        });
      },
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeInOut,
        margin: EdgeInsets.only(top: 50 * context.scale),
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            AnimatedContainer(
              duration: const Duration(milliseconds: 300),
              curve: Curves.easeInOut,
              width: _cardWidth,
              height: _cardHeight,
              decoration: BoxDecoration(
                image: DecorationImage(
                  image: AssetImage(_cardBackgroundAsset),
                  fit: BoxFit.fill,
                ),
              ),
            ),

            Positioned(
              top: -50 * context.scale,
              left: 0,
              right: 0,
              child: Center(
                child: Container(
                  width: 123 * context.scale,
                  height: 79 * context.scale,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(40 * context.scale),
                    border: Border.all(
                      color: Colors.white,
                      width: 2 * context.scale,
                    ),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.2),
                        blurRadius: 10,
                        offset: const Offset(0, 4),
                      ),
                    ],
                    image: DecorationImage(
                      image: AssetImage(program.imagePath),
                      fit: BoxFit.cover,
                    ),
                  ),
                ),
              ),
            ),

            Positioned(
              top: 12 * context.scale,
              right: 25 * context.scale,
              child: Text(
                program.tripDaysAgo,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 16 * context.scale,
                ),
              ),
            ),

            Positioned(
              top: 12 * context.scale,
              left: 25 * context.scale,
              child: Text(
                program.status,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 16 * context.scale,
                  fontWeight: FontWeight.w400,
                  color: Colors.black,
                ),
              ),
            ),

            Positioned(
              top: 32 * context.scale,
              left: 0,
              right: 0,
              child: Center(
                child: Text(
                  program.name,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 24 * context.scale,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ),

            Positioned(
              top: 57 * context.scale,
              left: 15 * context.scale,
              right: 15 * context.scale,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  Column(
                    children: [
                      CalendarDaysWidget(days: program.daysToRegister),
                      SizedBox(height: 6 * context.scale),
                      SizedBox(
                        width: 80 * context.scale,
                        child: Text(
                          "باقي لانتهاء التسجيل",
                          textAlign: TextAlign.center,
                          softWrap: true,
                          style: TextStyle(
                            fontFamily: 'Tajawal',
                            fontSize: 16 * context.scale,
                            color: Colors.black,
                          ),
                        ),
                      ),
                    ],
                  ),

                  Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      TouristsArcWidget(
                        current: int.parse(program.tourists),
                        max: int.parse(program.maxTourists),
                      ),
                      Transform.translate(
                        offset: Offset(0, -25 * context.scale),
                        child: Text(
                          "السائحين المنضمين",
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Tajawal',
                            fontSize: 20 * context.scale,
                            color: Colors.black,
                          ),
                        ),
                      ),
                    ],
                  ),

                  Column(
                    children: [
                      CalendarDaysWidget(days: program.daysToStart),
                      SizedBox(height: 6 * context.scale),
                      SizedBox(
                        width: 80 * context.scale,
                        child: Text(
                          "باقي لبدأ الرحلة",
                          textAlign: TextAlign.center,
                          softWrap: true,
                          style: TextStyle(
                            fontFamily: 'Tajawal',
                            fontSize: 16 * context.scale,
                            color: Colors.black,
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),

            
            if (_isExpanded) ...[
              Positioned(
                top: 220 * context.scale,
                left: 20,
                right: 0,
                child: Center(
                  child: Text(
                    "السائحين المنضمين معك",
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 24 * context.scale,
                      fontWeight: FontWeight.w400,
                      color: Colors.black,
                    ),
                  ),
                ),
              ),

              Positioned(
                top: 215 * context.scale,
                right: 20 * context.scale,
                child: Image.asset(
                  'assets/icons/Rectangle22.png',
                  width: 90 * context.scale,
                  height: 34 * context.scale,
                  fit: BoxFit.contain,
                ),
              ),
              Positioned(
                top: 248 * context.scale, 
                right:55 *context.scale, 
                child: Container(
                  width: 2 * context.scale,
                  height:20 *context.scale, 
                  color: Colors.black,
                ),
              ),

              Positioned(
                top: 270 * context.scale,
                left: 20 * context.scale,
                right: 20 * context.scale,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: program.joinedTourists
                      .map((tourist) => _buildTouristRow(tourist))
                      .toList(),
                ),
              ),

              Positioned(
                top: 460 * context.scale,
                left: 0,
                right: 0,
                child: Center(
                  child: GestureDetector(
                    onTap: () {

                      // TODO: اربط هنا التنقل الفعلي لتفاصيل البرنامج
                    
                    },
                    child: Container(
                      width: 179 * context.scale,
                      height: 51 * context.scale,
                      decoration: BoxDecoration(
                        color: const Color(0xFFF4A261),
                        borderRadius: BorderRadius.circular(16 * context.scale),
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(
                            "تفاصيل البرنامج",
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20 * context.scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black,
                            ),
                          ),
                          SizedBox(width: 6 * context.scale),
                          Image.asset(
                            'assets/icons/Foreign.png',
                            width: 18 * context.scale,
                            height: 18 * context.scale,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
            ],

            Positioned(
              bottom: -1 * context.scale,
              left: 25 * context.scale,
              child: Row(
                children: List.generate(
                  program.stars,
                  (i) => Padding(
                    padding: EdgeInsets.only(right: 1 * context.scale),
                    child: Image.asset(
                      'assets/icons/Star3.png',
                      width: 20 * context.scale,
                      height: 20 * context.scale,
                      fit: BoxFit.contain,
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

  Widget _buildTouristRow(JoinedTourist tourist) {
    const double circleSize = 46;

    return Container(
      margin: EdgeInsets.only(bottom: 10 * context.scale),
      width: double.infinity,
      height: 49 * context.scale,
      decoration: BoxDecoration(
        image: DecorationImage(
          image: AssetImage('assets/icons/Rectangle422.png'),
          fit: BoxFit.fill,
        ),
      ),
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 0,
            bottom: 0,
            left: 16 * context.scale,
            child: Center(
              child: Text(
                "منذ ${tourist.daysAgo} أيام",
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 16 * context.scale,
                  fontWeight: FontWeight.w400,
                  color: Colors.black,
                ),
              ),
            ),
          ),

          Positioned.fill(
            child: Align(
              alignment: const Alignment(0.2, 0),
              child: Text(
                tourist.name,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 20 * context.scale,
                  fontWeight: FontWeight.w400,
                  color: Colors.black,
                ),
              ),
            ),
          ),

           Positioned(
            top: ((49 - circleSize) / 2 - 3) * context.scale,
            right: 14 * context.scale,
            child: Container(
              width: circleSize * context.scale,
              height: circleSize * context.scale,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.25),
                    blurRadius: 4,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: ClipOval(
                child: tourist.photoUrl != null && tourist.photoUrl!.isNotEmpty
                    ? Image.network(
                        tourist.photoUrl!,
                        fit: BoxFit.cover,
                        // في حال فشل تحميل الصورة من الباك، نعرض خلفية برتقالية بديلة
                        errorBuilder: (context, error, stackTrace) =>
                            Container(color: const Color(0xFFF4A261)),
                      )
                    : Container(color: const Color(0xFFF4A261)),
              ),
            ),
          ),
        ],
      ),
    );
  }

//تبويب الملغاة
  Widget _buildCancelledCard(CancelledProgram p) {
    return p.reasonText != null
        ? _buildRejectedCard(p)
        : _buildCancelledSimpleCard(p);
  }

  // ══════════════════════════════════════════════════
  Widget _buildCancelledSimpleCard(CancelledProgram p) {
    return Container(
      margin: EdgeInsets.only(bottom: 30 * context.scale),
      width: 400 * context.scale,
      height: 187 * context.scale,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 1 * context.scale,
            left: 0 * context.scale,
            right: 0 * context.scale,
            height: 170 * context.scale,
            child: Image(
              image: AssetImage('assets/icons/Rectangle5.png'),
              fit: BoxFit.fill,
            ),
          ),

          Positioned(
            top: -45 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Container(
                width: 123 * context.scale,
                height: 79 * context.scale,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(40 * context.scale),
                  border: Border.all(
                    color: Colors.white,
                    width: 2 * context.scale,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.2),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ],
                  image: DecorationImage(
                    image: AssetImage(p.photoPath),
                    fit: BoxFit.cover,
                  ),
                ),
              ),
            ),
          ),

          Positioned(
            top: 12 * context.scale,
            right: 20 * context.scale,
            child: Text(
              p.daysAgoText,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontWeight: FontWeight.w400,
                fontSize: 16 * context.scale,
                color: Colors.black,
              ),
            ),
          ),

          Positioned(
            top: 12 * context.scale,
            left: 20 * context.scale,
            child: Text(
              p.statusLabel,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontWeight: FontWeight.w400,
                fontSize: 16 * context.scale,
                color: Colors.black,
              ),
            ),
          ),

          Positioned(
            top: 38 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                p.name,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 24 * context.scale,
                  color: Colors.black,
                ),
              ),
            ),
          ),

          Positioned(
            top: 75 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 20 * context.scale),
                child: Text(
                  p.descriptionText,
                  textAlign: TextAlign.center,
                  overflow: TextOverflow.ellipsis,
                  softWrap: false,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.w400,
                    fontSize: 20 * context.scale,
                    color: Colors.black,
                  ),
                ),
              ),
            ),
          ),
          Positioned(
            top: 110 * context.scale,
            left: 0 * context.scale,
            right: 0 * context.scale,
            child: Center(
              child: GestureDetector(
                onTap: () {

                  // TODO: اربط هنا منطق إعادة نشر البرنامج فعليًا
                
                },
                child: Container(
                  width: 197 * context.scale,
                  height: 43 * context.scale,
                  decoration: BoxDecoration(
                    color: const Color(0xFFF4A261),
                    borderRadius: BorderRadius.circular(15 * context.scale),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        "  إعادة نشر  ",
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 20 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                      SizedBox(width: 10 * context.scale),
                      SvgPicture.asset(
                        'assets/icons/Re.svg',
                        width: 24 * context.scale,
                        height: 24 * context.scale,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildRejectedCard(CancelledProgram p) {
    return Container(
      margin: EdgeInsets.only(bottom: 20 * context.scale),
      width: 400 * context.scale,
      height: 400 * context.scale,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 1 * context.scale,
            left: 0 * context.scale,
            right: 0 * context.scale,
            height: 400 * context.scale,
            child: Image(
              image: AssetImage('assets/icons/Rectangle52.png'),
              fit: BoxFit.fill,
            ),
          ),

          Positioned(
            top: -45 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Container(
                width: 123 * context.scale,
                height: 79 * context.scale,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(40 * context.scale),
                  border: Border.all(
                    color: Colors.white,
                    width: 2 * context.scale,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.2),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ],
                  image: DecorationImage(
                    image: AssetImage(p.photoPath),
                    fit: BoxFit.cover,
                  ),
                ),
              ),
            ),
          ),

          Positioned(
            top: 12 * context.scale,
            right: 20 * context.scale,
            child: Text(
              p.daysAgoText,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontWeight: FontWeight.w400,
                fontSize: 16 * context.scale,
                color: Colors.black,
              ),
            ),
          ),

          Positioned(
            top: 12 * context.scale,
            left: 20 * context.scale,
            child: Text(
              p.statusLabel,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontWeight: FontWeight.w400,
                fontSize: 16 * context.scale,
                color: Colors.black,
              ),
            ),
          ),

          Positioned(
            top: 38 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                p.name,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 24 * context.scale,
                  color: Colors.black,
                ),
              ),
            ),
          ),
          Positioned(
            top: 75 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 20 * context.scale),
                child: Text(
                  p.descriptionText,
                  textAlign: TextAlign.center,
                  overflow: TextOverflow.ellipsis,
                  softWrap: false,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.w400,
                    fontSize: 20 * context.scale,
                    color: Colors.black,
                  ),
                ),
              ),
            ),
          ),

          Positioned(
            top: 100 * context.scale,
            left: 20 * context.scale, 
            child: Container(
              width: 337 * context.scale,
              height: 204 * context.scale, 
              decoration: const BoxDecoration(
                image: DecorationImage(
                  image: AssetImage('assets/icons/reason_square.png'),
                  fit: BoxFit.fill,
                ),
              ),
              padding: EdgeInsets.symmetric(
                horizontal: 20 * context.scale,
                vertical: 10 * context.scale,
              ),
              child: TextField(
                maxLines: null,
                textAlign: TextAlign.center, 
                textAlignVertical: TextAlignVertical.center, 
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 18 * context.scale,
                  color: Colors.black,
                ),
                decoration: InputDecoration(
                  border: InputBorder.none,
                  contentPadding: EdgeInsets.symmetric(
                    vertical:
                        40 *
                        context.scale, 
                    horizontal: 20 * context.scale,
                  ),
                  hintText:
                      "أضف وصفاً عاماً عن الرحلة مع ما ستتضمنه من خدمات وفنادق ومطاعم بشكل مختصر يجذب السياح بمجرد قرائته",
                  hintStyle: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 16 * context.scale,
                    color: Colors.grey,
                  ),
                ),
              ),
            ),
          ),

          Positioned(
            top: 310 * context.scale,
            left: 0 * context.scale,
            right: 0 * context.scale,
            child: Center(
              child: GestureDetector(
                onTap: () {
                  // TODO: اربط هنا منطق إعادة نشر البرنامج فعليًا
                },
                child: Container(
                  width: 197 * context.scale,
                  height: 43 * context.scale,
                  decoration: BoxDecoration(
                    color: const Color(0xFFF4A261),
                    borderRadius: BorderRadius.circular(15 * context.scale),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        "  إعادة نشر  ",
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 20 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                      SizedBox(width: 10 * context.scale),
                      SvgPicture.asset(
                        'assets/icons/Re.svg',
                        width: 24 * context.scale,
                        height: 24 * context.scale,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  // تبويب السابقة

  Widget _buildPreviousCard(PreviousProgram p, {bool isFirst = false}) {
    return Transform.translate(
      offset: Offset(0, isFirst ? -20 * context.scale : -15),
      child: Container(
        margin: EdgeInsets.only(top: 70 * context.scale),
        width: double.infinity,
        child: Stack(
          clipBehavior: Clip.none,
          children: [
            Container(
              width: double.infinity,
              decoration: const BoxDecoration(
                image: DecorationImage(
                  image: AssetImage('assets/icons/Rectangle12.png'),
                  fit: BoxFit.fill,
                ),
              ),
              padding: EdgeInsets.only(
                top: 10 * context.scale, 
                left: 14 * context.scale,
                right: 14 * context.scale,
                bottom: 8 * context.scale, 
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Row(
                        mainAxisSize: MainAxisSize.min,
                        textDirection: TextDirection.ltr,
                        children: [
                          Text(
                            'من المرات',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontWeight: FontWeight.w400,
                              fontSize: 16 * context.scale,
                              color: Colors.black,
                            ),
                          ),
                          SizedBox(width: 4 * context.scale),
                          Text(
                            '${p.timesCount}',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontWeight: FontWeight.w400,
                              fontSize: 16 * context.scale,
                              color: Colors.black,
                            ),
                          ),
                        ],
                      ),
                      Text(
                        p.daysAgoText,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                    ],
                  ),

                  SizedBox(height: 2 * context.scale),

                  Text(
                    p.name,
                    textAlign: TextAlign.center,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w500,
                      fontSize: 24 * context.scale,
                    ),
                  ),

                  SizedBox(height: 8 * context.scale),

                 Row(
                    crossAxisAlignment: CrossAxisAlignment.start, 
                    children: [
                      Expanded(
                        child: Column(
                          children: [
                            Image.asset(
                              'assets/icons/evaluation.png',
                              width: 60 * context.scale,
                                height: 50 * context.scale
                                ),
                            SizedBox(height: 8 * context.scale),
                            Text(
                              '${p.reviewsCount}',
                              style: TextStyle(
                              fontFamily: 'Tajawal',
                                fontSize: 36 * context.scale,
                                fontWeight: FontWeight.w400
                                )),
                            Text(
                              'تقييم ومراجعة', 
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 16 * context.scale)),
                          ],
                        ),
                      ),
                      
                      Expanded(
                        child: Column(
                          children: [
                            Image.asset(
                              'assets/icons/card-up.png',
                              width: 50 * context.scale,
                              height: 50 * context.scale
                                ),
                            SizedBox(height: 8 * context.scale),

                            Text(
                              '${_formatAmount(p.totalEarned)}',
                            style: TextStyle(
                              fontSize: 24 * context.scale,
                              fontFamily: 'Tajawal', 
                              fontWeight: FontWeight.w400
                              )
                              ),
                            Text('\$', style: TextStyle(
                              fontSize: 24 * context.scale,
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                height: 0.8)
                                ),
                            Text('إجمالي المبلغ المستفاد',
                            textAlign: TextAlign.center,
                              style: TextStyle(
                                fontSize: 16 * context.scale,
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                )
                                ),
                          ],
                        ),
                      ),

                      Expanded(
                        child: Column(
                          children: [
                            Image.asset(
                              'assets/icons/tourists.png',
                              width: 50 * context.scale, 
                              height: 50 * context.scale
                              ),
                            SizedBox(height: 8 * context.scale),
                            Text('${p.touristsCount}', 
                            style: TextStyle(
                              fontSize: 36 * context.scale,
                              fontFamily: 'Tajawal',
                              fontWeight: FontWeight.w400
                              )),
                            Text(
                              'سائح انضم إلى هذا البرنامج', 
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontSize: 16 * context.scale,
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                )),
                          ],
                        ),
                      ),
                    ],
                  ),

                  SizedBox(height: 5 * context.scale),                 
                ],
              ),
            ),
                Positioned(
              bottom: 2 * context.scale,
              left: 25 * context.scale,
              child: Row(
                children: List.generate(
                  program.stars,
                  (i) => Padding(
                    padding: EdgeInsets.only(right: 1 * context.scale),
                    child: Image.asset(
                      'assets/icons/Star3.png',
                      width: 20 * context.scale,
                      height: 20 * context.scale,
                      fit: BoxFit.contain,
                    ),
                  ),
                ),
              ),
            ),
            Positioned(
              top: -45 * context.scale,
              left: 0,
              right: 0,
              child: Center(
                child: Container(
                  width: 123 * context.scale,
                  height: 79 * context.scale,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(40 * context.scale),
                    border: Border.all(
                      color: Colors.white,
                      width: 2 * context.scale,
                    ),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.2),
                        blurRadius: 10,
                        offset: const Offset(0, 4),
                      ),
                    ],
                    image: DecorationImage(
                      image: AssetImage(p.photoPath),
                      fit: BoxFit.cover,
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
   Widget _buildTab(String title, int index) {
    final bool isSelected = _selectedTabIndex == index;
    return GestureDetector(
      onTap: () {
        setState(() {
          _selectedTabIndex = index;
          _isExpanded = false; 
        });
      },
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        margin: EdgeInsets.all(4 * context.scale),
        decoration: BoxDecoration(
          color: isSelected ? const Color(0xFFF4A261) : Colors.transparent,
          borderRadius: BorderRadius.circular(20 * context.scale),
        ),
        alignment: Alignment.center,
        child: Text(
          title,
          textAlign: TextAlign.center,
          style: TextStyle(
            fontFamily: 'Tajawal',
            fontSize: isSelected ? 32 * context.scale : 28 * context.scale,
            fontWeight: isSelected ? FontWeight.w500 : FontWeight.w400,
            color: isSelected ? Colors.black : const Color(0xFF8E8E93),
          ),
        ),
      ),
    );
  }
}
