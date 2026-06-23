part of 'register_cubit.dart';

@immutable
sealed class RegistrState {}

final class RegistrInitial extends RegistrState {}

final class RegistrLoading extends RegistrState {}

final class RegistrSuccess extends RegistrState {
  final String message;
  final String email;

  RegistrSuccess({required this.message, required this.email});
}

final class RegistrFailure extends RegistrState {
  final String message;

  RegistrFailure({required this.message});
}
