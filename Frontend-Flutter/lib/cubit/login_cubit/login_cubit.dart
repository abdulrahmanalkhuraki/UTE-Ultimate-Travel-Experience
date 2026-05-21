import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';

part 'login_state.dart';

class LoginCubit extends Cubit<LoginState> {
  LoginCubit() : super(LoginInitial());

  // ── getter مشترك ──
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
    if (s is LoginFailure) emit(LoginFailure(message: s.message, obscurePassword: newVal));
  }

  Future<void> login({required String email, required String password}) async {
    emit(LoginLoading(obscurePassword: _currentObscure));
    try {
      await Future.delayed(const Duration(milliseconds: 500));
      emit(LoginSuccess(token: _fakeLogin(email: email, password: password)));
    } catch (error) {
      emit(LoginFailure(message: error.toString(), obscurePassword: _currentObscure));
    }
  }

  void reset() => emit(LoginInitial());

  String _fakeLogin({required String email, required String password}) {
    if (email.isEmpty || password.isEmpty) throw Exception('البريد وكلمة المرور مطلوبان.');
    if (!email.contains('@')) throw Exception('أدخل بريد إلكتروني صحيح.');
    if (password.length < 6) throw Exception('كلمة المرور يجب أن تكون 6 أحرف على الأقل.');
    return 'token_${DateTime.now().millisecondsSinceEpoch}';
  }
}