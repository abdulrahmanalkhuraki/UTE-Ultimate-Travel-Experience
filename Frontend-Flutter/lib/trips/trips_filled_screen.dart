import 'package:flutter/material.dart';
import '../search_screen.dart';
import '../app_constants.dart';
import 'trip_models.dart';
import 'current_trips_tab.dart';
import 'past_trips_tab.dart';
import 'cancelled_trips_tab.dart';

// ════════════════════════════════════════════════════════
// الشاشة الرئيسية – تدير التبويبات فقط
// البيانات ستأتي من الباك إند وتُمرَّر لكل تبويب
// ════════════════════════════════════════════════════════
class TripsFilledScreen extends StatefulWidget {
  const TripsFilledScreen({super.key});

  @override
  State<TripsFilledScreen> createState() => _TripsFilledScreenState();
}

class _TripsFilledScreenState extends State<TripsFilledScreen> {
  int selectedTab = 1;

  final List<CurrentTripModel> _currentTrips = [
    const CurrentTripModel(
      tripDaysAgo: 'قبل 7 أيام',
      countryName: 'الإمارات العربية المتحدة',
      tripRoute: 'دبي-برج خليفة-عجمان-متحف المستقبل-أبو ظبي_الشارقة',
      passengerNames: 'حجزت أنت ومحمد مدري شو ومدري مين المدريميني',
      bookingNumber: '8882700199821',
      daysToRegistrationEnd: 3,
      daysToStart: 10,
      currentTourists: 30,
      maxTourists: 60,
      starCount: 5,
      tripImagePath: 'assets/images/uae_trip.jpg',
    ),
    const CurrentTripModel(
      countryName: 'المملكة العربية السعودية',
      tripDaysAgo: 'قبل 7 أيام',
      tripRoute: 'دبي-برج خليفة-عجمان-متحف المستقبل-أبو ظبي_الشارقة',
      passengerNames: 'حجزت أنت ومحمد مدري شو ومدري مين المدريميني',
      bookingNumber: '8882700199821',
      daysToRegistrationEnd: 3,
      daysToStart: 10,
      currentTourists: 30,
      maxTourists: 60,
      starCount: 5,
      tripImagePath: 'assets/images/uae_trip.jpg',
    ),
  ];

  final List<PastTripModel> _pastTrips = [
    const PastTripModel(
      timeAgo: 'قبل 5 اسابيع',
      countryName: 'الإمارات العربية المتحدة',
      tripRoute: 'دبي-برج خليفة-متحف المستقبل-أبو ظبي_الشارقة',
      passengerNames: 'ذهبت أنت و محمد مدري شو ومدري مين المدريميني',
      joinDate: r'اضممت إلى البرنامج في 2026\5\16',
      duration: r'استمر البرنامج لمدة 10 أيام من 2026\5\20 إلى 2026\5\30',
      costLabel: 'تكلفة البرنامج للشخص الواحد',
      costAmount: r'$ 2000',
      starCount: 5,
      tripImagePath: 'assets/images/uae_trip.jpg',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Container(
            width: double.infinity,
            height: double.infinity,
            decoration: AppColors.backgroundGradient,
          ),
          SafeArea(
            child: Column(
              children: [
                Padding(
                  padding: EdgeInsets.only(
                    top: 20 * context.scale,
                    left: 20 * context.scale,
                    right: 20 * context.scale,
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      GestureDetector(
                        onTap: () => Navigator.push(
                          context,
                          PageRouteBuilder(
                            pageBuilder: (_, __, ___) => const SearchScreen(),
                          ),
                        ),
                        child: Hero(
                          tag: 'search_bar_transition',
                          child: Container(
                            width: 56 * context.scale,
                            height: 49 * context.scale,
                            decoration: BoxDecoration(
                              color: Colors.white.withOpacity(0.10),
                              borderRadius: BorderRadius.circular(20 * context.scale),
                              border: Border.all(color: Colors.black, width: 2),
                            ),
                            child: Center(
                              child: Image.asset(
                                'assets/icons/searchIcon.png',
                                width: 35 * context.scale,
                                height: 35 * context.scale,
                                fit: BoxFit.contain,
                              ),
                            ),
                          ),
                        ),
                      ),
                      Expanded(
                        child: FittedBox(
                          fit: BoxFit.scaleDown,
                          alignment: Alignment.center,
                          child: CustomHeaderTitle(title: 'رحلاتي'),
                        ),
                      ),
                      const CustomBackButton(),
                    ],
                  ),
                ),

                Padding(
                  padding: EdgeInsets.symmetric(
                    vertical: 8 * context.scale,
                    horizontal: 32 * context.scale,
                  ),
                  child: Container(
                    width: double.infinity,
                    height: 1.2,
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        colors: [
                          const Color(0xFF666666).withOpacity(0.5),
                          Colors.black,
                          const Color(0xFF666666).withOpacity(0.5),
                        ],
                      ),
                    ),
                  ),
                ),

                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 15 * context.scale),
                  child: Container(
                    width: 370 * context.scale,
                    height: 65 * context.scale,
                    decoration: BoxDecoration(
                      color: const Color(0xFFF4A261).withOpacity(0.36),
                      borderRadius: BorderRadius.circular(20 * context.scale),
                    ),
                    child: Row(
                      children: [
                        Expanded(child: _buildTab('الملغاة', 2)),
                        Expanded(child: _buildTab('الحالية', 1)),
                        Expanded(child: _buildTab('السابقة', 0)),
                      ],
                    ),
                  ),
                ),

                Expanded(child: _buildTabContent()),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabContent() {
    switch (selectedTab) {
      case 1:
        return CurrentTripsTab(trips: _currentTrips);
      case 0:
        return PastTripsTab(trips: _pastTrips);
      case 2:
        return const CancelledTripsTab();
      default:
        return const SizedBox();
    }
  }

  Widget _buildTab(String title, int index) {
    final bool isActive = selectedTab == index;
    return GestureDetector(
      onTap: () => setState(() => selectedTab = index),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        margin: EdgeInsets.all(4 * context.scale),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: isActive ? const Color(0xFFF4A261) : Colors.transparent,
          borderRadius: BorderRadius.circular(20 * context.scale),
        ),
        child: Text(
          title,
          style: TextStyle(
            fontFamily: 'Tajawal',
            fontSize: isActive ? 32 * context.scale : 28 * context.scale,
            fontWeight: isActive ? FontWeight.w500 : FontWeight.w400,
            color: isActive ? Colors.black : const Color(0xFF8E8E93),
          ),
        ),
      ),
    );
  }
}
