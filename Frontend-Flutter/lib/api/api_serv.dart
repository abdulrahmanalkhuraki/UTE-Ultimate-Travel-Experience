import 'package:shared_preferences/shared_preferences.dart';

/// الإعدادات المركزية للـ API (الرابط + Token + المسارات).
///
/// الطلبات الفعلية تبقى في ملفات منفصلة مثل [AuthApi] عبر Dio —
/// وليس هنا كاملة مثل مشروع albazaqar الذي يستخدم package `http`.
class ApiServ {
  ApiServ._();

  static const String _tokenKey = 'auth_token';

  /// الرابط الأساسي — تُضاف عليه مسارات الباك إند كما هي.
  static const String baseUrl = 'http://10.179.186.127:5000/api';

  static String? _authToken;

  static String? get authToken => _authToken;

  static bool get isLoggedIn =>
      _authToken != null && _authToken!.isNotEmpty;

  static Future<void> setAuthToken(String? token) async {
    _authToken = token;
    final prefs = await SharedPreferences.getInstance();

    if (token != null && token.isNotEmpty) {
      await prefs.setString(_tokenKey, token);
    } else {
      await prefs.remove(_tokenKey);
    }
  }

  static Future<void> loadAuthToken() async {
    final prefs = await SharedPreferences.getInstance();
    _authToken = prefs.getString(_tokenKey);
  }

  static Future<void> clearAuthToken() => setAuthToken(null);

  // ── Auth ──
  static const String authRegister = '/Auth/register';
  static const String authVerifyOtp = '/Auth/verify-otp';
  static const String authResendOtp = '/Auth/resend-otp';
  static const String authLogin = '/Auth/login';
  static const String authForgotPassword = '/Auth/forgot-password';
  static const String authResetPassword = '/Auth/reset-password';
  static const String authLogout = '/Auth/logout';
  static const String authMe = '/Auth/me';
}
