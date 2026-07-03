import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';

import 'package:tourism_app/bottomNavigationBar.dart';


class BookingDetails {
  final String statusLabel;
  final String companyMessage;
  final String bookingNumber;
  final String wishMessage;

  const BookingDetails({
    required this.statusLabel,
    required this.companyMessage,
    required this.bookingNumber,
    required this.wishMessage,
  });
}

class BookingSuccessScreen extends StatefulWidget {
  const BookingSuccessScreen({super.key});

  @override
  State<BookingSuccessScreen> createState() => _BookingSuccessScreenState();
}

class _BookingSuccessScreenState extends State<BookingSuccessScreen> {
  BookingDetails? _bookingDetails;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadBookingDetails();
  }

  Future<void> _loadBookingDetails() async {
    await Future.delayed(const Duration(milliseconds: 300));
    if (mounted) {
      setState(() {
        _bookingDetails = const BookingDetails(
          statusLabel: "تم الحجز بنجاح",
          companyMessage: "قبلت شركة مدري شو انضمامك لرحلة إلى الإمارات العربية المتحدة والذي ستبدأ in 2026/6/16",
          bookingNumber: "8882700199821",
          wishMessage: "نرجو أن تكون رحلة ممتعة",
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
            left: 0, right: 0,
            child: _TopBar(
              userName: 'محمد عبد الرحمن',
              onNotificationTap: () {},
            ),
          ),

          if (!_isLoading && _bookingDetails != null)
            Positioned(
              bottom: 0,
              left: 0, right: 0,
              child: _DraggableSuccessCard(details: _bookingDetails!),
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

class _DraggableSuccessCard extends StatefulWidget {
  final BookingDetails details;
  const _DraggableSuccessCard({required this.details});

  @override
  State<_DraggableSuccessCard> createState() => _DraggableSuccessCardState();
}

class _DraggableSuccessCardState extends State<_DraggableSuccessCard>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _ctrl;
  late Animation<double> _anim;

  @override
  void initState() {
    super.initState();
    _ctrl = AnimationController(vsync: this, duration: const Duration(milliseconds: 350));
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
              width: 72.w, height: 4.h,
              margin: EdgeInsets.only(top: 4.h, bottom: 12.h),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(10.r),
              ),
            ),

            if (!_expanded)
              GestureDetector(
                onTap: _toggleCard,
                child: Padding(
                  padding: EdgeInsets.symmetric(horizontal: 14.w),
                  child: SizedBox(
                    width: double.infinity,
                    height: 90.h,
                    child: Stack(
                      children: [
                        Positioned.fill(
                          child: Image.asset(
                            'assets/icons/successRectangle.png',
                            fit: BoxFit.fill,
                          ),
                        ),
                        Positioned.fill(
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            textDirection: TextDirection.rtl,
                            children: [
                              SizedBox(
                                width: 32.w, height: 32.h,
                                child: Image.asset(
                                  'assets/icons/success.png',
                                  fit: BoxFit.contain,
                                ),
                              ),
                              SizedBox(width: 12.w),
                              Text(
                                widget.details.statusLabel,
                                style: TextStyle(
                                  fontFamily: 'Tajawal',
                                  fontSize: 26.sp,
                                  fontWeight: FontWeight.w500,
                                  color: Colors.black,
                                ),
                              ),

                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),

            SizeTransition(
              sizeFactor: _anim,
              axisAlignment: -1,
              child: _SuccessDetailsPanel(
                details: widget.details,
                onCollapseTap: _toggleCard,
              ),
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
                  Color(0xFF7FBF8E),
                  Color(0x8C7FBF8E),
                  Color(0x1A7FBF8E),
                  Color(0x007FBF8E),
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

class _SuccessDetailsPanel extends StatelessWidget {
  final BookingDetails details;
  final VoidCallback onCollapseTap;

  const _SuccessDetailsPanel({required this.details, required this.onCollapseTap});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      physics: const NeverScrollableScrollPhysics(),
      child: Padding(
        padding: EdgeInsets.symmetric(horizontal: 24.w),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            GestureDetector(
              child: Container(
                child: SvgPicture.asset(
                  'assets/icons/arrow down.svg',
                ),
              ),
            ),

            SizedBox(
              width: 234.w, height: 220.h,
              child: SvgPicture.asset(
                'assets/images/true.svg',
                fit: BoxFit.contain,
              ),
            ),

            Text(
              details.statusLabel,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 30.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black,
              ),
            ),
            SizedBox(height: 12.h),

            Text(
              details.companyMessage,
              textDirection: TextDirection.rtl,
              textAlign: TextAlign.center,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 18.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black,
                height: 1.4,
              ),
            ),
            SizedBox(height: 20.h),

            Text(
              "رقم الحجز: ${details.bookingNumber}",
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 19.sp,
                fontWeight: FontWeight.w600,
                color: Colors.black,
              ),
            ),
            SizedBox(height: 8.h),

            Text(
              details.wishMessage,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 16.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black54,
              ),
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
            width: 48.w, height: 48.w,
            decoration: const BoxDecoration(
              color: Color(0xFF7FBF8E),
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
            child: Icon(Icons.notifications_outlined, size: 26.sp, color: const Color(0xFF2D264B)),
          ),
        ],
      ),
    );
  }
}