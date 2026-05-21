import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';

part 'register_state.dart';

class RegistrCubit extends Cubit<RegistrState> {
  RegistrCubit() : super(RegistrInitial());

  
void register({required String email, required String password}) async {
    emit(RegistrLoading());
    try {
      await Future.delayed(const Duration(milliseconds: 500));
      _fakeRegister(email: email, password: password);
      emit(RegistrSuccess(message: 'تم التسجيل بنجاح!'));
    } catch (error) {
      emit(RegistrFailure(message: error.toString()));
    }
  }

  void reset() => emit(RegistrInitial());

  void _fakeRegister({required String email, required String password}) {
    if (email.isEmpty || password.isEmpty) throw Exception('البريد وكلمة المرور مطلوبان.');
    if (!email.contains('@')) throw Exception('أدخل بريد إلكتروني صحيح.');
    if (password.length < 6) throw Exception('كلمة المرور يجب أن تكون 6 أحرف على الأقل.');
  }

  Future<void> simulateNetworkDelay() async {
    await Future.delayed(const Duration(seconds: 2));
  }

   Future<void> simulateNetworkDelayWithError() async {
    await Future.delayed(const Duration(seconds: 2));
    throw Exception('خطأ في الشبكة. حاول مرة أخرى.'); 
  
}

}