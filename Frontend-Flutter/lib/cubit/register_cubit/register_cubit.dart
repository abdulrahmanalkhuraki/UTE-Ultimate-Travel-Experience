import 'package:bloc/bloc.dart';
import 'package:dio/dio.dart';
import 'package:meta/meta.dart';
import 'package:ute_app/api/api_exception.dart';
import 'package:ute_app/api/auth_api.dart';



part 'register_state.dart';

class RegistrCubit extends Cubit<RegistrState> {
  RegistrCubit({AuthApi? authApi}) : _authApi = authApi ?? AuthApi(), super(RegistrInitial());

  final AuthApi _authApi;

  Future<void> register({
    required String email,
    required String password,
    required String confirmPassword,
  }) async {
    emit(RegistrLoading());
    try {
      final response = await _authApi.register(
        email: email,
        password: password,
        confirmPassword: confirmPassword,
      );
      emit(RegistrSuccess(
        message: response.message ?? 'تم إرسال رمز التحقق إلى بريدك الإلكتروني.',
        email: response.email ?? email,
      ));
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(RegistrFailure(message: error.message));
    } catch (e, stackTrace) {
      // ── تشخيص مؤقت: نطبع نوع الخطأ الحقيقي بالـ Debug Console ──
      // ignore: avoid_print
      print('REGISTER ERROR TYPE: ${e.runtimeType}');
      // ignore: avoid_print
      print('REGISTER ERROR MESSAGE: $e');
      // ignore: avoid_print
      print('STACK TRACE: $stackTrace');
      emit(RegistrFailure(message: 'خطأ غير متوقع: ${e.runtimeType} — ${e.toString()}'));
    }
  }

  void reset() => emit(RegistrInitial());
}