import 'package:flutter/material.dart';

class AppThemes {
  // الوضع الفاتح
  static final ThemeData lightTheme = ThemeData(
    brightness: Brightness.light,
    scaffoldBackgroundColor: const Color(0xFFFFFFFF),
    primaryColor: const Color(0xFFF4A261),
    // ضيفي هنا باقي ألوان الوضع الفاتح (النصوص، البطاقات، الخ)
  );

  // الوضع الداكن
  static final ThemeData darkTheme = ThemeData(
    brightness: Brightness.dark,
    scaffoldBackgroundColor: const Color(0xFF121212), // لون خلفية أسود أو رمادي غامق
    primaryColor: const Color(0xFFF4A261),
    // ضيفي هنا باقي ألوان الوضع الداكن
  );
}