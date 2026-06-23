part of 'forgot_password_cubit.dart';

@immutable
sealed class ForgotPasswordState {}

final class ForgotPasswordInitial extends ForgotPasswordState {}

final class ForgotPasswordLoading extends ForgotPasswordState {}

final class ForgotPasswordCodeSent extends ForgotPasswordState {
  final String message;

  ForgotPasswordCodeSent({required this.message});
}

final class ForgotPasswordResetSuccess extends ForgotPasswordState {}

final class ForgotPasswordFailure extends ForgotPasswordState {
  final String errorMessage;

  ForgotPasswordFailure({required this.errorMessage});
}
