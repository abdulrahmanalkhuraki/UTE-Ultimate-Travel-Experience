import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ThemeCubit extends Cubit<ThemeMode> {
  // الوضع الافتراضي هو الفاتح، وبمجرد تشغيل الكيوبت رح يقرأ آخر اختيار للمستخدم
  ThemeCubit() : super(ThemeMode.light) {
    _loadTheme();
  }

  // دالة للتبديل بين الوضعين (تُستدعى عند الضغط على الزر)
  void toggleTheme() async {
    final newTheme = state == ThemeMode.light ? ThemeMode.dark : ThemeMode.light;
    emit(newTheme); // تحديث الواجهة فوراً

    // حفظ الاختيار في ذاكرة الجوال
    final prefs = await SharedPreferences.getInstance();
    prefs.setBool('isDark', newTheme == ThemeMode.dark);
  }

  // دالة لقراءة الاختيار المحفوظ عند فتح التطبيق
  void _loadTheme() async {
    final prefs = await SharedPreferences.getInstance();
    final isDark = prefs.getBool('isDark') ?? false;
    emit(isDark ? ThemeMode.dark : ThemeMode.light);
  }
}