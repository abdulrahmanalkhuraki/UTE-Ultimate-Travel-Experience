import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:tourism_app/bottomNavigationBar.dart';

class TripDetails {
  final int    daysLeft;
  final String tripDescription;
  final String meetingPlace;
  final String tripStartLabel;
  final String tripTime;
  final String flightSectionTitle;
  final String flightDetail;
  final String timerGifPath;

  const TripDetails({
    required this.daysLeft,
    required this.tripDescription,
    required this.meetingPlace,
    required this.tripStartLabel,
    required this.tripTime,
    required this.flightSectionTitle,
    required this.flightDetail,
    this.timerGifPath = 'assets/images/Timer.gif',
  });

  factory TripDetails.fromJson(Map<String, dynamic> json) {
    return TripDetails(
      daysLeft:            json['days_left']             as int,
      tripDescription:     json['trip_description']      as String,
      meetingPlace:        json['meeting_place']         as String,
      tripStartLabel:      json['trip_start_label']      as String,
      tripTime:            json['trip_time']             as String,
      flightSectionTitle:  json['flight_section_title']  as String,
      flightDetail:        json['flight_detail']         as String,
      timerGifPath:        json['timer_gif_path']        as String?
          ?? 'assets/images/Timer.gif',
    );
  }
}

class TripMapScreen extends StatefulWidget {
  const TripMapScreen({super.key});

  @override
  State<TripMapScreen> createState() => _TripMapScreenState();
}

class _TripMapScreenState extends State<TripMapScreen> {
  TripDetails? _tripDetails;
  bool   _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadTripDetails();
  }

  Future<void> _loadTripDetails() async {
    try {
      await Future.delayed(const Duration(milliseconds: 800));

      final details = const TripDetails(
        daysLeft:           20,
        tripDescription:    'باقي لبدأ رحلتك إلى الإمارات العربية المتحدة ابدأ بالاستعداد لهذه الرحلة الممتعة',
        meetingPlace:       'مكان الالتقاء قبل الانطلاق في الرحلة سيكون في "ساحة الأمويين"',
        tripStartLabel:     'ستبدأ الرحلة',
        tripTime:           '8:30ص-9:30ص',
        flightSectionTitle: 'الصعود إلى الطائرة',
        flightDetail:       'ذهاب الطائرة من مطار دمشق إلى مطار دبي بمحطة توقف واحدة في الأردن',
      );

      if (mounted) setState(() { _tripDetails = details; _isLoading = false; });
    } catch (e) {
      if (mounted) setState(() { _error = 'فشل تحميل بيانات الرحلة'; _isLoading = false; });
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
            left: 0, right: 0,
            child: _TopBar(
              userName: 'محمد عبد الرحمن',
              onNotificationTap: () {},
            ),
          ),

          Positioned(
            bottom: 0,
            left: 10,
            right: 10,
            child: _isLoading
                ? _LoadingCard()
                : _error != null
                ? _ErrorCard(message: _error!)
                : _DraggableTimerCard(details: _tripDetails!),
          ),

          Positioned(
            bottom: 3, left: 0, right: 0,
            child: AppBottomNavBar(selectedIndex: 0),
          ),
        ],
      ),
    );
  }
}

class _LoadingCard extends StatelessWidget {
  @override
  Widget build(BuildContext context) => _CardShell(
    child: SizedBox(
      height: 90.h,
      child: const Center(child: CircularProgressIndicator(color: Color(0xFFF4A261))),
    ),
  );
}

class _ErrorCard extends StatelessWidget {
  final String message;
  const _ErrorCard({required this.message});

  @override
  Widget build(BuildContext context) => _CardShell(
    child: SizedBox(
      height: 90.h,
      child: Center(
        child: Text(message,
            textDirection: TextDirection.rtl,
            style: TextStyle(fontFamily: 'Tajawal', fontSize: 14.sp, color: Colors.red)),
      ),
    ),
  );
}

class _DraggableTimerCard extends StatefulWidget {
  final TripDetails details;
  const _DraggableTimerCard({required this.details});

  @override
  State<_DraggableTimerCard> createState() => _DraggableTimerCardState();
}

class _DraggableTimerCardState extends State<_DraggableTimerCard>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _ctrl;
  late Animation<double>   _anim;

  @override
  void initState() {
    super.initState();
    _ctrl = AnimationController(vsync: this, duration: const Duration(milliseconds: 350));
    _anim = CurvedAnimation(parent: _ctrl, curve: Curves.easeInOut);
  }

  @override
  void dispose() { _ctrl.dispose(); super.dispose(); }

  void _toggle() {
    setState(() {
      _expanded = !_expanded;
      _expanded ? _ctrl.forward() : _ctrl.reverse();
    });
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onVerticalDragEnd: (d) {
        if (d.primaryVelocity == null) return;
        if (d.primaryVelocity! < -150 && !_expanded) _toggle();
        if (d.primaryVelocity! >  150 &&  _expanded) _toggle();
      },
      child: _CardShell(
        isExpanded: _expanded,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            GestureDetector(
              onTap: _toggle,
              child: Container(
                width: 72.w, height: 4.h,
                margin: EdgeInsets.only(bottom: 12.h),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(10.r),
                ),
              ),
            ),

            GestureDetector(
              onTap: _toggle,
              child: SizedBox(
                width: double.infinity,
                height: 90.h,
                child: Stack(
                  children: [
                    Positioned.fill(
                      child: Image.asset(
                        'assets/icons/remainingDays.png',
                        fit: BoxFit.fill,
                      ),
                    ),
                    Positioned.fill(
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        textDirection: TextDirection.rtl,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: [
                          Padding(
                            padding: EdgeInsets.only(right: 22.w),
                            child: Text(
                              '${widget.details.daysLeft}',
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 64.sp,
                                fontWeight: FontWeight.w400,
                                color: Colors.black,
                              ),
                            ),
                          ),
                          SizedBox(
                            width: 100.w, height: 100.h,
                            child: Image.asset(widget.details.timerGifPath, fit: BoxFit.contain),
                          ),
                          Padding(
                            padding: EdgeInsets.only(left: 22.w),
                            child: Text(
                              'يوم',
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 48.sp,
                                fontWeight: FontWeight.w400,
                                color: Colors.black,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),

            SizeTransition(
              sizeFactor: _anim,
              axisAlignment: -1,
              child: _DetailsPanel(details: widget.details),
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

  const _CardShell({required this.child, this.isExpanded = false});

  @override
  Widget build(BuildContext context) {
    const topRadius   = Radius.circular(70);
    final borderRadius = BorderRadius.only(
      topLeft:  topRadius,
      topRight: topRadius,
    );

    return ClipRRect(
      borderRadius: borderRadius,
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: _GradientBorderPainter(
          borderRadius: borderRadius,
          child: Container(
            decoration: BoxDecoration(
              borderRadius: borderRadius,
              gradient: const LinearGradient(
                begin: Alignment.topCenter,
                end:   Alignment.bottomCenter,
                stops: [0.0, 0.043, 0.173, 1.0],
                colors: [
                  Color(0xFFF4A261),
                  Color(0x8CF4A261),
                  Color(0x1AF4A261),
                  Color(0x00F4A261),
                ],
              ),
            ),
            padding: EdgeInsets.fromLTRB(14.w, 12.h, 14.w, isExpanded ? 115.h : 130.h),
            child: child,
          ),
        ),
      ),
    );
  }
}

class _GradientBorderPainter extends StatelessWidget {
  final Widget       child;
  final BorderRadius borderRadius;

  const _GradientBorderPainter({required this.child, required this.borderRadius});

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
    path.arcToPoint(Offset(topR, 0),
        radius: Radius.circular(topR), clockwise: true);
    path.lineTo(size.width - topR, 0);
    path.arcToPoint(Offset(size.width, topR),
        radius: Radius.circular(topR), clockwise: true);
    path.lineTo(size.width, size.height);

    final paint = Paint()
      ..style       = PaintingStyle.stroke
      ..strokeWidth = 1.0
      ..shader      = LinearGradient(
        begin:  Alignment.topCenter,
        end:    Alignment.bottomCenter,
        colors: [
          Colors.black.withOpacity(0.50),
          const Color(0xFFDADADA),
        ],
      ).createShader(Rect.fromLTWH(0, 0, size.width, size.height));

    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant _BorderPainter old) =>
      old.borderRadius != borderRadius;
}

class _DetailsPanel extends StatelessWidget {
  final TripDetails details;
  const _DetailsPanel({required this.details});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(top: 16.h),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [

          Text(
            details.tripDescription,
            textDirection: TextDirection.rtl,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 24.sp,
              fontWeight: FontWeight.w400,
              color: Colors.black,
              height: 1.4,
            ),
          ),
          SizedBox(height: 14.h),

          Row(
            textDirection: TextDirection.rtl,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              SizedBox(
                width: 30.w, height: 30.h,
                child: SvgPicture.asset(
                  'assets/icons/Location_Add.svg',
                  fit: BoxFit.contain,
                ),
              ),

              SizedBox(width: 8.w),
              Expanded(
                child: Text(
                  details.meetingPlace,
                  textDirection: TextDirection.rtl,
                  textAlign: TextAlign.right,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 20.sp,
                    fontWeight: FontWeight.w400,
                    color: Colors.black,
                    height: 1.4,
                  ),
                ),
              ),
            ],
          ),
          SizedBox(height: 10.h),

          Row(
            textDirection: TextDirection.rtl,
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                details.tripStartLabel,
                textDirection: TextDirection.rtl,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 20.sp,
                  fontWeight: FontWeight.w500,
                  color: Colors.black,
                ),
              ),
              Text(
                details.tripTime,
                textDirection: TextDirection.rtl,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 20.sp,
                  fontWeight: FontWeight.w500,
                  color: Colors.black,
                ),
              ),
            ],
          ),
          SizedBox(height: 12.h),

          Stack(
            children: [
              Image.asset(
                'assets/icons/airplaneInfo.png',
                width: double.infinity,
                fit: BoxFit.fill,
              ),
              Padding(
                padding: EdgeInsets.symmetric(horizontal: 14.w, vertical: 10.h),
                child: Row(
                  textDirection: TextDirection.rtl,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    SizedBox(
                      width: 24.w, height: 24.h,
                      child: Image.asset(
                        'assets/icons/setting.png',
                        fit: BoxFit.contain,
                      ),
                    ),
                    SizedBox(width: 10.w),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.end,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(
                            details.flightSectionTitle,
                            textDirection: TextDirection.rtl,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20.sp,
                              fontWeight: FontWeight.w500,
                              color: Colors.black,
                            ),
                          ),
                          SizedBox(height: 6.h),
                          Text(
                            details.flightDetail,
                            textDirection: TextDirection.rtl,
                            textAlign: TextAlign.right,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 16.sp,
                              fontWeight: FontWeight.w400,
                              color: Colors.black,
                              height: 1.4,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),

        ],
      ),
    );
  }
}

class _TopBar extends StatelessWidget {
  final String       userName;
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
            width: 48.w, height: 48.w,
            decoration: const BoxDecoration(
              color: Color(0xFFF4A261),
              shape: BoxShape.circle,
            ),
          ),
          SizedBox(width: 10.w),
          Expanded(
            child: Text(
              userName,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontSize: 17.sp, fontWeight: FontWeight.bold,
                color: const Color(0xFF2D264B), fontFamily: 'Cairo',
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