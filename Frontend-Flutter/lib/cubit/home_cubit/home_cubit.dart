import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';
import 'package:ute_app/model/home_model.dart';

part 'home_state.dart';

class HomeCubit extends Cubit<HomeState> {
  HomeCubit() : super(HomeInitial());

  Future<void> loadHome() async {
    emit( HomeLoading());
 
    try {
      // Replace with your real data source / API call
      await Future.delayed(const Duration(milliseconds: 300));
 
      emit( HomeLoaded(
        userName: 'Kabten',
        selectedNavIndex: 0,
        popularTrips: [
          TripModel(title: 'رحلة جماعية فاخرة إلى دبي',   imagePath: 'assets/images/dubai.jpg', tripType: 'luxury'),
          TripModel(title: 'رحلة فاخرة إلى المالديف',      imagePath: 'assets/images/maldives.jpg', tripType: 'luxury'),
        ],
      ));
    } catch (e) {
      emit(HomeError(e.toString()));
    }
  }
 
  // ── Bottom nav tap ──
  void onNavTap(int index) {
    if (state is HomeLoaded) {
      emit((state as HomeLoaded).copyWith(selectedNavIndex: index));
    }
  }
 
  // ── Notification bell tap ──
  void onNotificationTap() {
    // navigate to notifications page or show sheet
  }
 
  // ── Search submitted ──
  void onSearch(String query) {
    // filter trips or navigate to search results
  }
}
