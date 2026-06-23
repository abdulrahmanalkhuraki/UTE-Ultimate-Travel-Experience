import 'package:bloc/bloc.dart';
import 'package:dio/dio.dart';
import 'package:meta/meta.dart';
import 'package:ute_app/api/api_exception.dart';
import 'package:ute_app/api/auth_api.dart';



part 'forgot_password_state.dart';

class ForgotPasswordCubit extends Cubit<ForgotPasswordState> {
  ForgotPasswordCubit({AuthApi? authApi})
      : _authApi = authApi ?? AuthApi(),
        super(ForgotPasswordInitial());

  final AuthApi _authApi;

  Future<void> sendForgotPassword({required String email}) async {
    emit(ForgotPasswordLoading());
    try {
      final response = await _authApi.forgotPassword(email: email);
      emit(ForgotPasswordCodeSent(
        message: response.message ?? 'تم إرسال رمز التحقق إلى بريدك الإلكتروني.',
      ));
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(ForgotPasswordFailure(errorMessage: error.message));
    } catch (e) {
      emit(ForgotPasswordFailure(errorMessage: e.toString()));
    }
  }

  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
    required String confirmPassword,
  }) async {
    emit(ForgotPasswordLoading());
    try {
      await _authApi.resetPassword(
        email: email,
        code: code,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
      emit(ForgotPasswordResetSuccess());
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(ForgotPasswordFailure(errorMessage: error.message));
    } catch (e) {
      emit(ForgotPasswordFailure(errorMessage: e.toString()));
    }
  }

  void reset() => emit(ForgotPasswordInitial());
}
