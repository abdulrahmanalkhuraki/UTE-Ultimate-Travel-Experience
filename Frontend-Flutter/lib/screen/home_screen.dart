import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:get/get.dart';
import 'package:latlong2/latlong.dart';
import 'package:ute_app/cubit/home_cubit/home_cubit.dart';
import 'package:ute_app/model/home_model.dart';
import 'package:ute_app/utils/constants.dart';

import 'dart:ui';
import 'login_screen.dart';


void _goToLogin() => Get.to(() => LoginScreen());

class HomeScreenProvider extends StatelessWidget {
  const HomeScreenProvider({super.key});
  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => HomeCubit()..loadHome(),
      child: const HomeScreen(),
    );
  }
}

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: BlocBuilder<HomeCubit, HomeState>(
        builder: (context, state) {
          if (state is HomeLoading || state is HomeInitial) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is HomeError) {
            return Center(child: Text(state.message));
          }
          return _HomeBody(data: state as HomeLoaded);
        },
      ),
    );
  }
}

class _HomeBody extends StatelessWidget {
  final HomeLoaded data;
  const _HomeBody({required this.data});

  @override
  Widget build(BuildContext context) {
    final cubit = context.read<HomeCubit>();
    // final screenH = MediaQuery.of(context).size.height;
    // final screenW = MediaQuery.of(context).size.width;

    return Stack(
      children: [
        // ── 1. flutter_map يملأ الشاشة كلها ─────────────────
        Positioned.fill(
          child: FlutterMap(
            options: const MapOptions(
              initialCenter: LatLng(33.5138, 36.2765), // دمشق
              initialZoom: 12,
            ),
            children: [
              TileLayer(
                urlTemplate: 'https://tile.openstreetmap.org/{z}/{x}/{y}.png',
                userAgentPackageName: 'com.example.ute_app',
              ),
            ],
          ),
        ),


        // ── 4. Top bar: avatar + name + bell ─────────────────
        Positioned(
          top: MediaQuery.of(context).padding.top + 16.h,
          left: 0,
          right: 0,
          child: _TopBar(
            userName: data.userName,
            onNotificationTap: cubit.onNotificationTap,
          ),
        ),

        // ── 5. البانل السفلي الشفاف ───────────────────────────
        Positioned(
          top: 375.h,
          left: 10,
          right: 10,
          bottom: 15,
          child: _BottomPanel(data: data),
        ),

        // ── 6. Bottom Navigation Bar ──────────────────────────
        Positioned(
          bottom: 3,
          left: 0,
          right: 0,
          child: _BottomNav(
            selectedIndex: data.selectedNavIndex,
            onTap: (_) => _goToLogin(),
          ),
        ),
      ],
    );
  }
}

class _TopBar extends StatelessWidget {
  final String userName;
  final VoidCallback onNotificationTap;
  const _TopBar({required this.userName, required this.onNotificationTap});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 20.w),
      child: Row(
        textDirection: TextDirection.rtl,
        children: [
          // الدائرة البرتقالية
          Container(
            width: 48.w,
            height: 48.w,
            decoration: const BoxDecoration(
              color: Color(0xFFF4A261),
              shape: BoxShape.circle,
            ),
          ),
          SizedBox(width: 10.w),

          // اسم المستخدم
          Expanded(
            child: Text(
              userName,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontSize: 17.sp,
                fontWeight: FontWeight.bold,
                color: const Color(0xFF2D264B),
                fontFamily: 'Cairo',
              ),
            ),
          ),

          // أيقونة الجرس
          GestureDetector(
            onTap: onNotificationTap,
            child: CustomPaint(size: Size(24.w, 26.h), painter: BellPainter()),
          ),
        ],
      ),
    );
  }
}

// ─────────────────────────────────────────────
//  البانل السفلي الشفاف
// ─────────────────────────────────────────────
class _BottomPanel extends StatelessWidget {
  final HomeLoaded data;
  const _BottomPanel({required this.data});

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(36.r),
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 12, sigmaY: 12),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(36.r),
            gradient: const LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              stops: [0.0, 0.4],
              colors: [Color(0xCC91B3FA), Color(0x2991B3FA)],
            ),
            border: Border.all(color: Colors.white.withOpacity(0.3), width: 1),
          ),
          padding: EdgeInsets.symmetric(horizontal: 20.w, vertical: 20.h),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // زر "حدد وجهتك"
              GestureDetector(
                onTap: () => _goToLogin(),
                child: Container(
                  height: 52.h,
                  decoration: BoxDecoration(
                    color: const Color(0xFFF4A261),
                    borderRadius: BorderRadius.circular(30.r),
                    boxShadow: [
                      BoxShadow(
                        color: const Color(0xFFF4A261).withOpacity(0.4),
                        blurRadius: 12,
                        offset: const Offset(0, 6),
                      ),
                    ],
                  ),
                  child: Center(
                    child: Text(
                      'حدد وجهتك',
                      style: TextStyle(
                        fontSize: 17.sp,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                        fontFamily: 'Cairo',
                      ),
                    ),
                  ),
                ),
              ),
              SizedBox(height: 12.h),

              // ─── الزرين ───────────────────────────────
              Row(
                textDirection: TextDirection.rtl,
                children: [
                  // زر برتقالي شفاف
                  Expanded(
                    child: GestureDetector(
                      onTap: () {
                        // TODO
                      },
                      child: Container(
                        height: 49.h,
                        decoration: BoxDecoration(
                          color: const Color(0xFFF4A261).withOpacity(0.5),
                          borderRadius: BorderRadius.circular(20.r),
                        ),
                        child: Center(
                          child: Text(
                            'أو قم بتصفح البرامج الجماعية',
                            style: TextStyle(
                              fontFamily: 'Cairo',
                              fontSize: 11.sp,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),

                  SizedBox(width: 10.w),

                  // زر أبيض
                  Expanded(
                    child: GestureDetector(
                      onTap: () => _goToLogin(),
                      child: Container(
                        height: 49.h,
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(20.r),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const Icon(
                              Icons.search,
                              color: Color(0xFFF4A261),
                              size: 20,
                            ),
                            SizedBox(width: 6.w),
                            Text(
                              'يمكنك البحث هنا',
                              style: TextStyle(
                                fontFamily: 'Cairo',
                                fontSize: 11.sp,
                                color: Colors.grey,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                ],
              ),

              SizedBox(height: 16.h),

              // عنوان القسم
              Align(
                alignment: Alignment.centerRight,
                child: Text(
                  'البرامج الجماعية الأكثر طلباً',
                  textDirection: TextDirection.rtl,
                  style: TextStyle(
                    fontSize: 14.sp,
                    fontWeight: FontWeight.w700,
                    color: const Color(0xFF2D264B),
                    fontFamily: 'Cairo',
                  ),
                ),
              ),

              SizedBox(height: 10.h),

              // قائمة الرحلات
              SizedBox(
                height: 150.h,
                child: ListView.separated(
                  scrollDirection: Axis.horizontal,
                  reverse: true,
                  itemCount: data.popularTrips.length,
                  separatorBuilder: (_, _) => SizedBox(width: 12.w),
                  itemBuilder: (context, index) {
                    return GestureDetector(
                      onTap: () => _goToLogin(),
                      child: _TripCard(trip: data.popularTrips[index]),
                    );
                  },
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _showDestinationSheet(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => const _DestinationSheet(),
    );
  }
}

// ─────────────────────────────────────────────
//  Trip card
// ─────────────────────────────────────────────
class _TripCard extends StatelessWidget {
  final TripModel trip;
  const _TripCard({required this.trip});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 150.w,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(18.r),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.12),
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(18.r),
        child: Stack(
          children: [
            Positioned.fill(
              child: Image.asset(
                trip.imagePath,
                fit: BoxFit.cover,
                errorBuilder: (_, _, _) => Container(
                  color: const Color(0xFF91B3FA).withOpacity(0.3),
                  child: Icon(
                    Icons.image_outlined,
                    color: Colors.white54,
                    size: 36.sp,
                  ),
                ),
              ),
            ),
            Positioned.fill(
              child: DecoratedBox(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [Colors.transparent, Colors.black.withOpacity(0.6)],
                  ),
                ),
              ),
            ),
            Positioned(
              bottom: 8.h,
              left: 8.w,
              right: 8.w,
              child: Text(
                trip.title,
                textDirection: TextDirection.rtl,
                textAlign: TextAlign.center,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  fontSize: 11.sp,
                  fontWeight: FontWeight.w600,
                  color: Colors.white,
                  fontFamily: 'Cairo',
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ─────────────────────────────────────────────
//  Destination bottom sheet
// ─────────────────────────────────────────────
class _DestinationSheet extends StatefulWidget {
  const _DestinationSheet();

  @override
  State<_DestinationSheet> createState() => _DestinationSheetState();
}

class _DestinationSheetState extends State<_DestinationSheet> {
  bool _showSearch = false;
  String _searchQuery = '';
  String? _selectedCountry;
  final TextEditingController _searchController = TextEditingController();

  final List<String> _countries = [
    'سوريا',
    'تونس',
    'الإمارات',
    'لبنان',
    'الأردن',
    'مصر',
    'تركيا',
    'السعودية',
  ];

  List<String> get _filtered => _searchQuery.isEmpty
      ? _countries
      : _countries.where((c) => c.contains(_searchQuery)).toList();

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 420.h,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(36.r)),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.12),
            blurRadius: 20,
            offset: const Offset(0, -4),
          ),
        ],
      ),
      child: Column(
        children: [
          SizedBox(height: 12.h),
          Container(
            width: 40.w,
            height: 4.h,
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(10.r),
            ),
          ),
          SizedBox(height: 20.h),

          Padding(
            padding: EdgeInsets.symmetric(horizontal: 20.w),
            child: AnimatedSwitcher(
              duration: const Duration(milliseconds: 250),
              child: _showSearch ? _buildSearchField() : _buildDefaultRow(),
            ),
          ),

          SizedBox(height: 16.h),

          Padding(
            padding: EdgeInsets.symmetric(horizontal: 20.w),
            child: Align(
              alignment: Alignment.centerRight,
              child: Text(
                'حدد الدولة',
                textDirection: TextDirection.rtl,
                style: TextStyle(
                  fontFamily: 'Cairo',
                  fontSize: 15.sp,
                  fontWeight: FontWeight.w700,
                  color: const Color(0xFF2D264B),
                ),
              ),
            ),
          ),

          SizedBox(height: 8.h),

          Expanded(
            child: ListView.builder(
              padding: EdgeInsets.symmetric(horizontal: 20.w),
              itemCount: _filtered.length,
              itemBuilder: (context, index) {
                final country = _filtered[index];
                final selected = _selectedCountry == country;
                return GestureDetector(
                  onTap: () {
                    setState(() => _selectedCountry = country);
                    // TODO: Navigator.push للصفحة التالية
                  },
                  child: Container(
                    margin: EdgeInsets.only(bottom: 6.h),
                    padding: EdgeInsets.symmetric(
                      horizontal: 16.w,
                      vertical: 12.h,
                    ),
                    decoration: BoxDecoration(
                      color: selected
                          ? const Color(0xFFF4A261).withOpacity(0.12)
                          : Colors.transparent,
                      borderRadius: BorderRadius.circular(12.r),
                    ),
                    child: Row(
                      textDirection: TextDirection.rtl,
                      children: [
                        Container(
                          width: 10.w,
                          height: 10.w,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            color: selected
                                ? const Color(0xFFF4A261)
                                : Colors.grey[400],
                          ),
                        ),
                        SizedBox(width: 12.w),
                        Text(
                          country,
                          style: TextStyle(
                            fontFamily: 'Cairo',
                            fontSize: 14.sp,
                            fontWeight: selected
                                ? FontWeight.w700
                                : FontWeight.w400,
                            color: selected
                                ? const Color(0xFF2D264B)
                                : Colors.grey[700],
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchField() {
    return TextField(
      key: const ValueKey('search'),
      controller: _searchController,
      textDirection: TextDirection.rtl,
      autofocus: true,
      onChanged: (v) => setState(() => _searchQuery = v),
      decoration: InputDecoration(
        hintText: 'ابحث عن دولة...',
        hintStyle: TextStyle(
          fontFamily: 'Cairo',
          fontSize: 14.sp,
          color: Colors.grey,
        ),
        prefixIcon: GestureDetector(
          onTap: () => setState(() {
            _showSearch = false;
            _searchQuery = '';
            _searchController.clear();
          }),
          child: const Icon(Icons.close, color: Colors.grey),
        ),
        suffixIcon: const Icon(Icons.search, color: Color(0xFFF4A261)),
        filled: true,
        fillColor: const Color(0xFFF5F5F5),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(30.r),
          borderSide: BorderSide.none,
        ),
        contentPadding: EdgeInsets.symmetric(vertical: 14.h, horizontal: 16.w),
      ),
    );
  }

  Widget _buildDefaultRow() {
    return Row(
      key: const ValueKey('default'),
      textDirection: TextDirection.rtl,
      children: [
        Expanded(
          child: GestureDetector(
            onTap: () {
              // TODO: انتقل لصفحة البرامج الجماعية
            },
            child: Container(
              height: 48.h,
              decoration: BoxDecoration(
                color: const Color(0xFFF5F5F5),
                borderRadius: BorderRadius.circular(30.r),
              ),
              child: Center(
                child: Text(
                  'أو اضغط للبرامج الجماعية',
                  style: TextStyle(
                    fontFamily: 'Cairo',
                    fontSize: 12.sp,
                    color: Colors.grey[600],
                  ),
                ),
              ),
            ),
          ),
        ),
        SizedBox(width: 10.w),
        GestureDetector(
          onTap: () => _goToLogin(),
          child: Container(
            width: 48.w,
            height: 48.h,
            decoration: const BoxDecoration(
              color: Color(0xFFF5F5F5),
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.search, color: Color(0xFFF4A261)),
          ),
        ),
      ],
    );
  }
}

// ─────────────────────────────────────────────
//  Bottom navigation bar
// ─────────────────────────────────────────────
// في home_screen.dart — استبدل _BottomNav كاملاً

class _BottomNav extends StatelessWidget {
  final int selectedIndex;
  final ValueChanged<int> onTap;
  const _BottomNav({required this.selectedIndex, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 10, vertical: 12),
      width: 372,
      height: 97,
      decoration: BoxDecoration(
        color: NavBarConstants.navBgColor,
        borderRadius: BorderRadius.circular(40),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: List.generate(NavBarConstants.icons.length, (i) {
          final selected = i == selectedIndex;
          return GestureDetector(
            onTap: () => _goToLogin(),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                selected
                    ? _PentagonIcon(icon: NavBarConstants.icons[i])
                    : Icon(
                        NavBarConstants.icons[i],
                        color: NavBarConstants.inactiveColor,
                        size: 26,
                      ),
                if (NavBarConstants.labels[i].isNotEmpty)
                  Padding(
                    padding: const EdgeInsets.only(top: 4),
                    child: Text(
                      NavBarConstants.labels[i],
                      style: TextStyle(
                        fontSize: 10,
                        fontFamily: 'Cairo',
                        color: selected
                            ? NavBarConstants.activeColor
                            : NavBarConstants.inactiveColor,
                      ),
                    ),
                  ),
              ],
            ),
          );
        }),
      ),
    );
  }
}

// ─── الشكل الخماسي ───────────────────────────
class _PentagonIcon extends StatelessWidget {
  final IconData icon;
  const _PentagonIcon({required this.icon});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 48,
      height: 48,
      child: Stack(
        alignment: Alignment.center,
        children: [
          // الخلفية البرتقالية بشكل مدوّر بس
          Container(
            width: 48,
            height: 48,
            decoration: const BoxDecoration(
              color: Color(0xFFF4A261),
              shape: BoxShape.circle,
            ),
          ),
          Icon(icon, color: Colors.white, size: 24),
        ],
      ),
    );
  }
}
