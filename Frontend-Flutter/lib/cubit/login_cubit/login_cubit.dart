import 'package:bloc/bloc.dart';
import 'package:dio/dio.dart';
import 'package:meta/meta.dart';
import 'package:ute_app/api/api_exception.dart';
import 'package:ute_app/api/api_serv.dart';
import 'package:ute_app/api/auth_api.dart';



part 'login_state.dart';

class LoginCubit extends Cubit<LoginState> {
  LoginCubit({AuthApi? authApi}) : _authApi = authApi ?? AuthApi(), super(LoginInitial());

  final AuthApi _authApi;

  bool get _currentObscure {
    final s = state;
    if (s is LoginInitial) return s.obscurePassword;
    if (s is LoginLoading) return s.obscurePassword;
    if (s is LoginFailure) return s.obscurePassword;
    return true;
  }

  void toggleObscurePassword() {
    final newVal = !_currentObscure;
    final s = state;
    if (s is LoginInitial) emit(LoginInitial(obscurePassword: newVal));
    if (s is LoginFailure) {
      emit(LoginFailure(message: s.message, obscurePassword: newVal));
    }
  }

  Future<void> login({required String email, required String password}) async {
    emit(LoginLoading(obscurePassword: _currentObscure));
    try {
      final response = await _authApi.login(email: email, password: password);
      final token = response.token;
      if (token == null || token.isEmpty) {
        emit(LoginFailure(
          message: 'لم يتم استلام رمز الدخول من الخادم.',
          obscurePassword: _currentObscure,
        ));
        return;
      }
      await ApiServ.setAuthToken(token);
      emit(LoginSuccess(token: token));
    } on DioException catch (e) {
      final error = ApiException.fromDioException(e);
      emit(LoginFailure(message: error.message, obscurePassword: _currentObscure));
    } catch (e) {
      emit(LoginFailure(message: e.toString(), obscurePassword: _currentObscure));
    }
  }

  void reset() => emit(LoginInitial());
}
