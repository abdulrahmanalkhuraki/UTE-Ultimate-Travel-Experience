part of 'login_cubit.dart';

@immutable
sealed class LoginState {}

final class LoginInitial extends LoginState {
  final bool obscurePassword;
  LoginInitial({this.obscurePassword = true});
}

final class LoginLoading extends LoginState {
  final bool obscurePassword;
  LoginLoading({this.obscurePassword = true});
}

final class LoginSuccess extends LoginState {
  final String token;
  LoginSuccess({required this.token});
}

final class LoginFailure extends LoginState {
  final String message;
  final bool obscurePassword;
  LoginFailure({required this.message, this.obscurePassword = true});
}