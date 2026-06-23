part of 'otp_cubit.dart';

@immutable
sealed class OtpState {}

final class OtpInitial extends OtpState {}

final class OtpLoading extends OtpState {}

final class OtpSuccess extends OtpState {
  final String token;

  OtpSuccess({required this.token});
}

final class OtpResent extends OtpState {
  final String message;

  OtpResent({required this.message});
}

final class OtpFailure extends OtpState {
  final String message;

  OtpFailure({required this.message});
}
