import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart'; 

class AppColors {
  static const Color gradientTop = Color(0xFF91B3FA);
  static const Color gradientMiddle = Color(0xFFC8D9FD);
  static const Color gradientBottom = Color(0xFFFFFFFF);
  static const Color navBarBackground = Color(0xFFD1E3FF);

  static const BoxDecoration backgroundGradient = BoxDecoration(
    gradient: LinearGradient(
      begin: Alignment.topCenter,
      end: Alignment.bottomCenter,
      colors: [gradientTop, gradientMiddle, gradientBottom],
      stops: [0.0, 0.3, 0.7],
    ),
  );
}

class AppTextStyles {
  static const String fontFamily = 'Cairo';
  static const TextStyle headerTitle = TextStyle(
    fontFamily: fontFamily,
    fontSize: 40,
    fontWeight: FontWeight.w700,
    color: Colors.black,
    letterSpacing: 2.0,
  );
}

class AppIcons {
  static const double backArrowWidth = 50;
  static const double backArrowHeight = 50;
  static const String backArrowPath = 'assets/icons/arrowBack.svg';

  static const String rectangle77 = 'assets/icons/Rectangle77.png';
  static const String rectangle88 = 'assets/icons/Rectangle88.png';
  static const String notification = 'assets/icons/Notification.png';
  static const String profileCircle = 'assets/icons/ProfileCircle.png';
  static const String persons = 'assets/icons/persons.png';
  static const String proCount = 'assets/icons/proCount.png';
  static const String note = 'assets/icons/note.png';
  static const String star = 'assets/icons/Star 4.png';
}


extension ScaleExtension on BuildContext {
  double get scale => MediaQuery.of(this).size.width / 390;
}

class CustomBackButton extends StatelessWidget {
  const CustomBackButton({super.key});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 50 * context.scale,
      height: 50 * context.scale,
      child: IconButton(
        padding: EdgeInsets.zero,
        onPressed: () => Navigator.pop(context),
        icon: SvgPicture.asset(
          AppIcons.backArrowPath,
          width: AppIcons.backArrowWidth * context.scale,
          height: AppIcons.backArrowHeight * context.scale,
          fit: BoxFit.contain,
        ),
      ),
    );
  }
}
class CustomHeaderTitle extends StatelessWidget {
  final String title;
  const CustomHeaderTitle({super.key, required this.title});

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      textAlign: TextAlign.center,
      maxLines: 1,
      // 1. إضافة هذا السطر ليصغر الخط تلقائياً عند ضيق المساحة
      softWrap: false,
      overflow: TextOverflow.fade,
      style: TextStyle(
        fontFamily: AppTextStyles.fontFamily,
        fontSize: 40 * context.scale,
        fontWeight: FontWeight.w700,
        color: Colors.black,
        height: 1.0,
      ),
    );
  }
}
class DashboardStats {
  final int enrolledTouristsCount;

  final int ratingsCount;

  final int publishedProgramsCount;

  final int reviewsCount;

  const DashboardStats({
    required this.enrolledTouristsCount,
    required this.ratingsCount,
    this.publishedProgramsCount = 0,
    this.reviewsCount = 0,
  });

  factory DashboardStats.fromJson(Map<String, dynamic> json) {
    return DashboardStats(
      enrolledTouristsCount: json['enrolled_tourists_count'] ?? 0,
      ratingsCount: json['ratings_count'] ?? 0,
      publishedProgramsCount: json['published_programs_count'] ?? 0,
      reviewsCount: json['reviews_count'] ?? 0,
    );
  }
}
