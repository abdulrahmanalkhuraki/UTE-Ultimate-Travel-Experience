part of 'home_cubit.dart';

@immutable
sealed class HomeState {}

final class HomeInitial extends HomeState {}
 
class HomeLoading extends HomeState {
   HomeLoading();
}
 
class HomeLoaded extends HomeState {
  final String userName;
  final int selectedNavIndex;
  final List<TripModel> popularTrips;
 
   HomeLoaded({
    required this.userName,
    required this.selectedNavIndex,
    required this.popularTrips,
  });
 
  HomeLoaded copyWith({
    String? userName,
    int? selectedNavIndex,
    List<TripModel>? popularTrips,
  }) {
    return HomeLoaded(
      userName: userName ?? this.userName,
      selectedNavIndex: selectedNavIndex ?? this.selectedNavIndex,
      popularTrips: popularTrips ?? this.popularTrips,
    );
  }
 
  List<Object?> get props => [userName, selectedNavIndex, popularTrips];
}
 
class HomeError extends HomeState {
  final String message;
   HomeError(this.message);
 
  List<Object?> get props => [message];
}