import 'package:bloc/bloc.dart';
import 'package:dio/dio.dart';
import 'package:meta/meta.dart';
import 'package:ute_app/api/api_exception.dart';
import 'package:ute_app/api/api_serv.dart';
import 'package:ute_app/api/auth_api.dart';



part 'otp_state.dart';

class OtpCubit extends Cubit<OtpState> {
  OtpCubit({AuthApi? authApi}) : _authApi = authApi ?? AuthApi(), super(OtpInitial());

  final AuthApi _authApi;

  Future<void> verifyOtp({required String email, required String code}) async {
    emit(OtpLoading());
    try {
      final response = await _authApi.verifyOtp(email: email, code: code);
      final token = response.token;
      if (token == null || token.isEmpty) {
        emit(OtpFailure(message: 'لم يتم استلام رمز الدخول من الخادم.'));
        return;
      }
      await ApiServ.setAuthToken(token);
      emit(OtpSuccess(token: token));
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(OtpFailure(message: error.message));
    } catch (e) {
      emit(OtpFailure(message: e.toString()));
    }
  }

  Future<void> resendOtp({required String email}) async {
    emit(OtpLoading());
    try {
      final response = await _authApi.resendOtp(email: email);
      emit(OtpResent(message: response.message ?? 'تم إعادة إرسال الرمز.'));
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(OtpFailure(message: error.message));
    } catch (e) {
      emit(OtpFailure(message: e.toString()));
    }
  }

  void reset() => emit(OtpInitial());
}
