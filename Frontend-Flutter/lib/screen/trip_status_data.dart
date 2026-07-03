import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:tourism_app/bottomNavigationBar.dart';

class TripStatusData {
  final String statusMessage;

  const TripStatusData({required this.statusMessage});
}

class TripStatusScreen extends StatefulWidget {
  const TripStatusScreen({super.key});

  @override
  State<TripStatusScreen> createState() => _TripStatusScreenState();
}

class _TripStatusScreenState extends State<TripStatusScreen> {
  TripStatusData? _statusData;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadTripStatus();
  }

  Future<void> _loadTripStatus() async {
    await Future.delayed(const Duration(milliseconds: 300));
    if (mounted) {
      setState(() {
        _statusData = const TripStatusData(
          statusMessage: "بدأ اليوم الاول من الرحلة",
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

          if (!_isLoading && _statusData != null)
            Positioned(
              bottom: 0,
              left: 0,
              right: 0,
              child: _StaticTripCard(statusData: _statusData!),
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

class _StaticTripCard extends StatelessWidget {
  final TripStatusData statusData;
  const _StaticTripCard({required this.statusData});

  @override
  Widget build(BuildContext context) {
    return _CardShell(
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

          _StatusMessageBar(message: statusData.statusMessage),
        ],
      ),
    );
  }
}

class _CardShell extends StatelessWidget {
  final Widget child;

  const _CardShell({required this.child});

  @override
  Widget build(BuildContext context) {
    const topRadius = Radius.circular(70);
    const borderRadius = BorderRadius.only(topLeft: topRadius, topRight: topRadius);

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
            padding: EdgeInsets.fromLTRB(0, 8.h, 0, 135.h),
            child: child,
          ),
        ),
      ),
    );
  }
}

class _StatusMessageBar extends StatelessWidget {
  final String message;
  const _StatusMessageBar({required this.message});

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
        child: Center(
          child: Text(
            message,
            textDirection: TextDirection.rtl,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 22.sp,
              fontWeight: FontWeight.w500,
              color: Colors.black,
            ),
          ),
        ),
      ),
    );
  }
}
class _GradientBorderPainter extends StatelessWidget {
  final Widget child;
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
  bool shouldRepaint(covariant _BorderPainter old) => old.borderRadius != borderRadius;
}

// البار العلوي
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