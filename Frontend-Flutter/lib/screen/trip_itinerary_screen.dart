import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';

import 'package:tourism_app/bottomNavigationBar.dart';

class TripActivity {
  final String title;
  final String timeRange;
  final String description;

  const TripActivity({
    required this.title,
    required this.timeRange,
    required this.description,
  });
}

class DayItinerary {
  final String dayLabel;
  final String dateBadge;
  final String subtitle;
  final List<TripActivity> activities;

  const DayItinerary({
    required this.dayLabel,
    required this.dateBadge,
    required this.subtitle,
    required this.activities,
  });
}

class TripDetails {
  final String locationName;
  final DayItinerary day;

  const TripDetails({
    required this.locationName,
    required this.day,
  });
}

class TripItineraryScreen extends StatefulWidget {
  const TripItineraryScreen({super.key});

  @override
  State<TripItineraryScreen> createState() => _TripItineraryScreenState();
}

class _TripItineraryScreenState extends State<TripItineraryScreen> {
  TripDetails? _tripDetails;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadTripDetails();
  }

  Future<void> _loadTripDetails() async {
    await Future.delayed(const Duration(milliseconds: 300));
    if (mounted) {
      setState(() {
        _tripDetails = const TripDetails(
          locationName: "عجمان_متحف المستقبل",
          day: DayItinerary(
            dayLabel: "اليوم الأول",
            dateBadge: "18\\6",
            subtitle: "الأنشطة الثلاث التالية",
            activities: [
              TripActivity(
                title: "افطار فرنسي تقليدي",
                timeRange: "8:30ص - 9:30ص",
                description:
                "بدء اليوم بوجبة افطار في مقهى باريس الدافئ وكرواسان طازج",
              ),
              TripActivity(
                title: "افطار فرنسي تقليدي",
                timeRange: "8:30ص - 9:30ص",
                description:
                "بدء اليوم بوجبة افطار في مقهى باريس الدافئ وكرواسان طازج",
              ),
              TripActivity(
                title: "افطار فرنسي تقليدي",
                timeRange: "8:30ص - 9:30ص",
                description:
                "بدء اليوم بوجبة افطار في مقهى باريس الدافئ وكرواسان طازج",
              ),
            ],
          ),
        );
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Positioned.fill(
            child: FlutterMap(
              options: const MapOptions(
                initialCenter: LatLng(33.5138, 36.2765),
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

          Positioned(
            top: MediaQuery.of(context).padding.top + 16.h,
            left: 0,
            right: 0,
            child: _TopBar(
              userName: 'محمد عبد الرحمن',
              onNotificationTap: () {},
            ),
          ),

          if (!_isLoading && _tripDetails != null)
            Positioned(
              bottom: 0,
              left: 0,
              right: 0,
              child: _DraggableTripCard(details: _tripDetails!),
            ),

          Positioned(
            bottom: 3,
            left: 0,
            right: 0,
            child: AppBottomNavBar(selectedIndex: 0),
          ),
        ],
      ),
    );
  }
}

class _DraggableTripCard extends StatefulWidget {
  final TripDetails details;
  const _DraggableTripCard({required this.details});

  @override
  State<_DraggableTripCard> createState() => _DraggableTripCardState();
}

class _DraggableTripCardState extends State<_DraggableTripCard>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _ctrl;
  late Animation<double> _anim;

  @override
  void initState() {
    super.initState();
    _ctrl = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 350));
    _anim = CurvedAnimation(parent: _ctrl, curve: Curves.easeInOut);
  }

  @override
  void dispose() {
    _ctrl.dispose();
    super.dispose();
  }

  void _toggleCard() {
    setState(() {
      _expanded = !_expanded;
      _expanded ? _ctrl.forward() : _ctrl.reverse();
    });
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onVerticalDragEnd: (dragDetails) {
        if (dragDetails.primaryVelocity == null) return;
        if (dragDetails.primaryVelocity! < -150 && !_expanded) _toggleCard();
        if (dragDetails.primaryVelocity! > 150 && _expanded) _toggleCard();
      },
      child: _CardShell(
        isExpanded: _expanded,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 72.w,
              height: 4.h,
              margin: EdgeInsets.only(top: 4.h, bottom: 12.h),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(10.r),
              ),
            ),
            GestureDetector(
              onTap: _toggleCard,
              child: _LocationSearchBar(locationName: widget.details.locationName),
            ),

            SizeTransition(
              sizeFactor: _anim,
              axisAlignment: -1,
              child: _DayItineraryPanel(day: widget.details.day),
            ),
          ],
        ),
      ),
    );
  }
}

class _CardShell extends StatelessWidget {
  final Widget child;
  final bool isExpanded;

  const _CardShell({required this.child, required this.isExpanded});

  @override
  Widget build(BuildContext context) {
    const topRadius = Radius.circular(70);
    const borderRadius =
    BorderRadius.only(topLeft: topRadius, topRight: topRadius);

    return ClipRRect(
      borderRadius: borderRadius,
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: _GradientBorderPainter(
          borderRadius: borderRadius,
          child: Container(
            width: double.infinity,
            decoration: const BoxDecoration(
              borderRadius: borderRadius,
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                stops: [0.0, 0.043, 0.173, 1.0],
                colors: [
                  Color(0xFF91B3FA),
                  Color(0x8C91B3FA),
                  Color(0x1A91B3FA),
                  Color(0x0091B3FA),
                ],
              ),
            ),
            padding: EdgeInsets.fromLTRB(0, 8.h, 0, isExpanded ? 115.h : 135.h),
            child: child,
          ),
        ),
      ),
    );
  }
}

class _GradientBorderPainter extends StatelessWidget {
  final Widget child;
  final BorderRadius borderRadius;

  const _GradientBorderPainter(
      {required this.child, required this.borderRadius});

  @override
  Widget build(BuildContext context) {
    return CustomPaint(
      painter: _BorderPainter(borderRadius: borderRadius),
      child: child,
    );
  }
}

class _BorderPainter extends CustomPainter {
  final BorderRadius borderRadius;
  const _BorderPainter({required this.borderRadius});

  @override
  void paint(Canvas canvas, Size size) {
    final topR = borderRadius.topLeft.x;
    final path = Path();

    path.moveTo(0, size.height);
    path.lineTo(0, topR);
    path.arcToPoint(Offset(topR, 0), radius: Radius.circular(topR), clockwise: true);
    path.lineTo(size.width - topR, 0);
    path.arcToPoint(Offset(size.width, topR), radius: Radius.circular(topR), clockwise: true);
    path.lineTo(size.width, size.height);

    final paint = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = 1.0
      ..shader = LinearGradient(
        begin: Alignment.topCenter,
        end: Alignment.bottomCenter,
        colors: [
          Colors.black.withOpacity(0.35),
          const Color(0xFFDADADA).withOpacity(0.1),
        ],
      ).createShader(Rect.fromLTWH(0, 0, size.width, size.height));

    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant _BorderPainter old) =>
      old.borderRadius != borderRadius;
}

class _LocationSearchBar extends StatelessWidget {
  final String locationName;
  const _LocationSearchBar({required this.locationName});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 24.w),
      child: Container(
        width: double.infinity,
        height: 65.h,
        padding: EdgeInsets.symmetric(horizontal: 16.w),
        decoration: const BoxDecoration(
          image: DecorationImage(
            image: AssetImage('assets/icons/successRectangle.png'),
            fit: BoxFit.fill,
          ),
        ),
        child: Row(
          textDirection: TextDirection.rtl,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            SizedBox(
              width: 30.w,
              height: 30.h,
              child: SvgPicture.asset(
                'assets/icons/Location_Add.svg',
                fit: BoxFit.contain,
              ),
            ),
            SizedBox(width: 8.w),
            Text(
              locationName,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w500,
                color: Colors.black,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
class _DayItineraryPanel extends StatelessWidget {
  final DayItinerary day;
  const _DayItineraryPanel({required this.day});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Padding(
        padding: EdgeInsets.symmetric(horizontal: 24.w),
        child: Column(
          textDirection: TextDirection.rtl,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              textDirection: TextDirection.rtl,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                SizedBox(
                  width: 40.w,
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      SizedBox(height: 16.h),
                      SizedBox(
                        width: 40.w,
                        height: 45.h,
                        child: Stack(
                          alignment: Alignment.center,
                          children: [
                            SvgPicture.asset(
                              'assets/icons/calender.svg',
                              fit: BoxFit.contain,
                              width: 40.w,
                              height: 45.h,
                            ),
                            Padding(
                              padding: EdgeInsets.only(top: 10.h),
                              child: Text(
                                day.dateBadge,
                                style: TextStyle(
                                  fontFamily: 'Tajawal',
                                  fontSize: 11.sp,
                                  fontWeight: FontWeight.w600,
                                  color: Colors.black,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                      SizedBox(height: 4.h),
                      Image.asset(
                        'assets/icons/clock.png',
                        width: 18.w,
                        height: 18.h,
                        fit: BoxFit.contain,
                      ),
                    ],
                  ),
                ),
                SizedBox(width: 16.w),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      SizedBox(height: 18.h),
                      Text(
                        day.dayLabel,
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 24.sp,
                          fontWeight: FontWeight.w500,
                          color: Colors.black,
                        ),
                      ),
                      SizedBox(height: 2.h),
                      Text(
                        day.subtitle,
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 16.sp,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),

            Stack(
              clipBehavior: Clip.none,
              children: [
                Positioned(
                  right: 20.w,
                  top: -8.h,
                  bottom: 49.h,
                  child: Container(
                    width: 1.2.w,
                      color: Colors.grey.shade600
                  ),
                ),

                Column(
                  children: List.generate(day.activities.length, (index) {
                    final activity = day.activities[index];

                    return Padding(
                      padding: EdgeInsets.only(
                        bottom: index == day.activities.length - 1 ? 0 : 16.h,
                      ),
                      child: Stack(
                        clipBehavior: Clip.none,
                        children: [
                          Positioned(
                            right: 20.w,
                            top: 13.h,
                            left: 190.w,
                            child: Image.asset(
                              'assets/icons/path1.png',
                              height: 8.h,
                              fit: BoxFit.fill,
                              color: Colors.black,
                              colorBlendMode: BlendMode.srcIn,
                            ),
                          ),

                          Padding(
                            padding: EdgeInsets.only(right: 32.w),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.end,
                              children: [
                                Row(
                                  textDirection: TextDirection.rtl,
                                  mainAxisAlignment: MainAxisAlignment.start,
                                  children: [
                                    Text(
                                      activity.timeRange,
                                      style: TextStyle(
                                        fontFamily: 'Tajawal',
                                        fontSize: 16.sp,
                                        fontWeight: FontWeight.w400,
                                        color: Colors.black,
                                      ),
                                    ),
                                    SizedBox(width: 12.w),
                                    Text(
                                      activity.title,
                                      style: TextStyle(
                                        fontFamily: 'Tajawal',
                                        fontSize: 16.sp,
                                        fontWeight: FontWeight.w700,
                                        color: Colors.black,
                                      ),
                                    ),
                                  ],
                                ),
                                SizedBox(height: 6.h),
                                Row(
                                  textDirection: TextDirection.rtl,
                                  crossAxisAlignment: CrossAxisAlignment.center,
                                  children: [
                                    Image.asset(
                                      'assets/icons/setting.png',
                                      width: 24.w,
                                      height: 24.h,
                                      fit: BoxFit.contain,
                                    ),
                                    SizedBox(width: 8.w),
                                    Expanded(
                                      child: Text(
                                        activity.description,
                                        textDirection: TextDirection.rtl,
                                        textAlign: TextAlign.right,
                                        style: TextStyle(
                                          fontFamily: 'Tajawal',
                                          fontSize: 14.sp,
                                          fontWeight: FontWeight.w400,
                                          color: Colors.black,
                                          height: 1.3,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    );
                  }),
                ),
              ],
            ),
          ],
        ),
      ),
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
          Container(
            width: 48.w,
            height: 48.w,
            decoration: const BoxDecoration(
              color: Color(0xFF91B3FA),
              shape: BoxShape.circle,
            ),
          ),
          SizedBox(width: 10.w),
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
          GestureDetector(
            onTap: onNotificationTap,
            child: Icon(Icons.notifications_outlined,
                size: 26.sp, color: const Color(0xFF2D264B)),
          ),
        ],
      ),
    );
  }
}
