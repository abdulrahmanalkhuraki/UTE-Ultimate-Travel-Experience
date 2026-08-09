import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

class FirstStepScreen extends StatelessWidget {
  final int touristsCount;
  final int programsCount;
  final int reviewsCount;
  final int ratingsCount;

  const FirstStepScreen({
    Key? key,
    this.touristsCount = 300,
    this.programsCount = 5,
    this.reviewsCount = 300,
    this.ratingsCount = 300,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final screenSize = MediaQuery.of(context).size;
    final screenWidth = screenSize.width;
    final screenHeight = screenSize.height;

    final double titleFontSize = screenWidth * 0.038;
    final double numberFontSize = screenWidth * 0.075;
    final double labelFontSize = screenWidth * 0.042;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: SingleChildScrollView(
          child: Padding(
            padding: EdgeInsets.symmetric(horizontal: screenWidth * 0.04),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(height: screenHeight * 0.02),

                // صف أيقونة الملف الشخصي والإشعارات
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    GestureDetector(
                      onTap: () {},
                      child: Image.asset(
                        'assets/icons/ProfileCircle.png',
                        width: screenWidth * 0.18,
                        height: screenWidth * 0.18,
                        fit: BoxFit.contain,
                      ),
                    ),
                    GestureDetector(
                      onTap: () {},
                      child: Image.asset(
                        'assets/icons/Notification.png',
                        width: screenWidth * 0.13,
                        height: screenWidth * 0.13,
                        fit: BoxFit.contain,
                      ),
                    ),
                  ],
                ),

                SizedBox(height: screenHeight * 0.02),

                // المربعات الزرقاء والتنسيق الداخلي
                Row(
                  children: [
                    // المربع الأول: عدد البرامج السياحية المنشورة
                    Expanded(
                      child: Stack(
                        children: [
                          Image.asset(
                            'assets/icons/Rectangle77.png',
                            fit: BoxFit.contain,
                            width: double.infinity,
                          ),
                          Padding(
                            padding: EdgeInsets.all(screenWidth * 0.035),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.center,
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  'عدد البرامج\nالسياحية المنشورة',
                                  textAlign: TextAlign.center,
                                  style: TextStyle(
                                    fontFamily: 'Tajawal',
                                    fontSize: titleFontSize,
                                    fontWeight: FontWeight.w400,
                                    color: Colors.black,
                                    height: 1.1,
                                  ),
                                ),
                                Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  crossAxisAlignment: CrossAxisAlignment.center,
                                  children: [
                                    Column(
                                      crossAxisAlignment: CrossAxisAlignment.center,
                                      children: [
                                        Text(
                                          '$programsCount',
                                          style: TextStyle(
                                            fontFamily: 'Tajawal',
                                            fontSize: numberFontSize,
                                            fontWeight: FontWeight.w400,
                                            color: Colors.black,
                                            height: 1.0,
                                          ),
                                        ),
                                        SizedBox(height: screenHeight * 0.004),
                                        Text(
                                          'برامج',
                                          style: TextStyle(
                                            fontFamily: 'Tajawal',
                                            fontSize: labelFontSize,
                                            fontWeight: FontWeight.w400,
                                            color: Colors.black,
                                            height: 1.0,
                                          ),
                                        ),
                                      ],
                                    ),
                                    SizedBox(width: screenWidth * 0.05),
                                    Image.asset(
                                      'assets/icons/proCount.png',
                                      width: screenWidth * 0.12,
                                      height: screenWidth * 0.12,
                                      fit: BoxFit.contain,
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),

                    SizedBox(width: screenWidth * 0.04),

                    // المربع الثاني: عدد السائحين المنضمين لبرنامجك
                    Expanded(
                      child: Stack(
                        children: [
                          Image.asset(
                            'assets/icons/Rectangle77.png',
                            fit: BoxFit.contain,
                            width: double.infinity,
                          ),
                          Padding(
                            padding: EdgeInsets.all(screenWidth * 0.035),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.center,
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  'عدد السائحين\nالمنضمين لبرنامجك',
                                  textAlign: TextAlign.center,
                                  style: TextStyle(
                                    fontFamily: 'Tajawal',
                                    fontSize: titleFontSize,
                                    fontWeight: FontWeight.w400,
                                    color: Colors.black,
                                    height: 1.1,
                                  ),
                                ),
                                Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  crossAxisAlignment: CrossAxisAlignment.center,
                                  children: [
                                    Column(
                                      crossAxisAlignment: CrossAxisAlignment.center,
                                      children: [
                                        Text(
                                          '$touristsCount',
                                          style: TextStyle(
                                            fontFamily: 'Tajawal',
                                            fontSize: numberFontSize,
                                            fontWeight: FontWeight.w400,
                                            color: Colors.black,
                                            height: 1.0,
                                          ),
                                        ),
                                        SizedBox(height: screenHeight * 0.004),
                                        Text(
                                          'سائح',
                                          style: TextStyle(
                                            fontFamily: 'Tajawal',
                                            fontSize: labelFontSize,
                                            fontWeight: FontWeight.w400,
                                            color: Colors.black,
                                            height: 1.0,
                                          ),
                                        ),
                                      ],
                                    ),
                                    SizedBox(width: screenWidth * 0.05),
                                    SvgPicture.asset(
                                      'assets/icons/personss.svg',
                                      width: screenWidth * 0.12,
                                      height: screenWidth * 0.12,
                                      fit: BoxFit.contain,
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),

                SizedBox(height: screenHeight * 0.015),

                // المستطيل البرتقالي (التقييمات والمراجعات)
                SizedBox(
                  width: double.infinity,
                  child: Stack(
                    children: [
                      GestureDetector(
                        onTap: () {},
                        child: Image.asset(
                          'assets/icons/Rectangle88.png',
                          fit: BoxFit.contain,
                          width: double.infinity,
                        ),
                      ),
                      Padding(
                        padding: EdgeInsets.all(screenWidth * 0.035),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.center,
                          children: [
                            Text(
                              'التقييمات والمراجعات التي اضافها\nالسائحين عنك',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: titleFontSize,
                                fontWeight: FontWeight.w400,
                                color: Colors.black,
                                height: 1.1,
                              ),
                            ),
                            Transform.translate(
                              offset: Offset(0, -screenHeight * 0.012),
                              child: Padding(
                                padding: EdgeInsets.symmetric(horizontal: screenWidth * 0.04),
                                child: Row(
                                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Column(
                                      crossAxisAlignment: CrossAxisAlignment.center,
                                      children: [
                                        Image.asset(
                                          'assets/icons/note.png',
                                          width: screenWidth * 0.11,
                                          height: screenWidth * 0.11,
                                          fit: BoxFit.contain,
                                        ),
                                        SizedBox(height: screenHeight * 0.002),
                                        Row(
                                          crossAxisAlignment: CrossAxisAlignment.center,
                                          children: [
                                            Text(
                                              '$reviewsCount',
                                              style: TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: numberFontSize,
                                                fontWeight: FontWeight.w400,
                                                color: Colors.black,
                                                height: 1.0,
                                              ),
                                            ),
                                            SizedBox(width: screenWidth * 0.02),
                                            Text(
                                              'مراجعة',
                                              style: TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: labelFontSize,
                                                fontWeight: FontWeight.w400,
                                                color: Colors.black,
                                                height: 1.0,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ],
                                    ),
                                    Column(
                                      crossAxisAlignment: CrossAxisAlignment.center,
                                      children: [
                                        Image.asset(
                                          'assets/icons/Star 4.png',
                                          width: screenWidth * 0.11,
                                          height: screenWidth * 0.11,
                                          fit: BoxFit.contain,
                                        ),
                                        SizedBox(height: screenHeight * 0.002),
                                        Row(
                                          crossAxisAlignment: CrossAxisAlignment.center,
                                          children: [
                                            Text(
                                              '$ratingsCount',
                                              style: TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: numberFontSize,
                                                fontWeight: FontWeight.w400,
                                                color: Colors.black,
                                                height: 1.0,
                                              ),
                                            ),
                                            SizedBox(width: screenWidth * 0.02),
                                            Text(
                                              'تقييم',
                                              style: TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: labelFontSize,
                                                fontWeight: FontWeight.w400,
                                                color: Colors.black,
                                                height: 1.0,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),

                SizedBox(height: 1),

                Align(
                  alignment: Alignment.centerRight,
                  child: Text(
                     ":أكثر برامج طلباً ",
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: screenWidth * 0.065,
                      fontWeight: FontWeight.w500,
                      color: Colors.black,
                    ),
                  ),
                ),

                SizedBox(height: screenHeight * 0.01),

                // عرض الكاردات الديناميكية
                Row(
                  children: [
                    Expanded(
                      child: EmptyProgramCard(
                        title: 'The Nautilus Maldives',
                        location: 'The Nautilus Maldives, Baa Atoll',
                        rating: '4.6',
                        onTap: () {},
                      ),
                    ),
                    SizedBox(width: screenWidth * 0.04),
                    Expanded(
                      child: EmptyProgramCard(
                        title: 'Erin-Ijesha Falls',
                        location: 'Ekiti, Nigeria',
                        rating: '4.6',
                        onTap: () {},
                      ),
                    ),
                  ],
                ),

                SizedBox(height: screenHeight * 0.03),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

// ويدجت الكارد بالتنسيق المطلوب على 3 أسطر
class EmptyProgramCard extends StatelessWidget {
  final String title;
  final String location;
  final String rating;
  final VoidCallback onTap;

  const EmptyProgramCard({
    Key? key,
    required this.title,
    required this.location,
    required this.rating,
    required this.onTap,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        height: screenWidth * 0.52,
        decoration: BoxDecoration(
          color: Colors.grey[200],
          borderRadius: BorderRadius.circular(24),
          border: Border.all(color: Colors.grey.shade300),
        ),
        child: Stack(
          children: [
            Positioned(
              top: 12,
              right: 12,
              child: Image.asset(
                'assets/icons/export.png',
                width: 25,
                height: 25,
                fit: BoxFit.contain,
              ),
            ),

            Positioned(
              bottom: 12,
              left: 6,
              right: 6,
              height: 50,
              child: Image.asset(
                'assets/icons/Rectangle 143.png',
                fit: BoxFit.fill,
              ),
            ),

            Positioned(
              bottom: 14,
              left: 14,
              right: 14,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    title,
                    style: const TextStyle(
                      fontFamily: 'Lato',
                      fontSize: 13,
                      fontWeight: FontWeight.w500,
                      color: Colors.white,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 1),
                  
                  Row(
                    children: [
                      Image.asset(
                        'assets/icons/location.png',
                        width: 9,
                        height: 11,
                        fit: BoxFit.contain,
                      ),
                      const SizedBox(width: 4),
                      Expanded(
                        child: Text(
                          location,
                          style: const TextStyle(
                            fontFamily: 'Lato',
                            fontSize: 10,
                            fontWeight: FontWeight.w400,
                            color: Colors.white,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 1),

                  FittedBox(
                    fit: BoxFit.scaleDown,
                    alignment: Alignment.centerLeft,
                    child:Row(
                    children: [
                      Image.asset(
                        'assets/icons/stars.png',
                        width: 75,
                        height: 12,
                        fit: BoxFit.contain,
                      ),
                      const SizedBox(width: 6),
                      Text(
                        rating,
                        style: const TextStyle(
                          fontFamily: 'Lato',
                          fontSize: 11,
                          fontWeight: FontWeight.w500,
                          color: Colors.white,
                        ),
                      ),
                    ],
                  ),)
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}