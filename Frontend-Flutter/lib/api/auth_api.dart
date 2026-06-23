import 'package:dio/dio.dart';
import 'package:ute_app/api/api_serv.dart';
import 'package:ute_app/api/dio_client.dart';
import 'package:ute_app/model/auth_model.dart';



class AuthApi {
  AuthApi({Dio? dio}) : _dio = dio ?? DioClient.instance.dio;

  final Dio _dio;

  /// هذا التطبيق مخصص للسائح فقط (تطبيق الشركة منفصل) —
  /// فبنثبت roleName دايماً على Tourist بدون أي اختيار بالواجهة.
  static const String _touristRole = 'Tourist';

  Future<RegisterResponse> register({
    required String email,
    required String password,
    required String confirmPassword,
  }) async {
    final response = await _dio.post(
      ApiServ.authRegister,
      data: FormData.fromMap({
        'email': email,
        'password': password,
        'confirmPassword': confirmPassword,
        'roleName': _touristRole,
      }),
    );
    return RegisterResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AuthResponse> verifyOtp({
    required String email,
    required String code,
  }) async {
    final response = await _dio.post(
      ApiServ.authVerifyOtp,
      data: FormData.fromMap({
        'email': email,
        'code': code,
      }),
    );
    return AuthResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<OtpResponse> resendOtp({required String email}) async {
    final response = await _dio.post(
      ApiServ.authResendOtp,
      data: FormData.fromMap({'email': email}),
    );
    return OtpResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AuthResponse> login({
    required String email,
    required String password,
  }) async {
    final response = await _dio.post(
      ApiServ.authLogin,
      data: FormData.fromMap({
        'email': email,
        'password': password,
      }),
    );
    return AuthResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<OtpResponse> forgotPassword({required String email}) async {
    final response = await _dio.post(
      ApiServ.authForgotPassword,
      data: FormData.fromMap({'email': email}),
    );
    return OtpResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _dio.post(
      ApiServ.authResetPassword,
      data: FormData.fromMap({
        'email': email,
        'code': code,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      }),
    );
  }

  Future<void> logout() async {
    await _dio.post(ApiServ.authLogout);
  }
}