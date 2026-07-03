import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:tourism_app/bottomNavigationBar.dart';

class RequestRejectedDetails {
  final String statusLabel;
  final String companyMessage;
  final String reasonLabel;
  final String reasonText;
  final String cancelLabel;
  final String resendLabel;

  const RequestRejectedDetails({
    required this.statusLabel,
    required this.companyMessage,
    required this.reasonLabel,
    required this.reasonText,
    required this.cancelLabel,
    required this.resendLabel,
  });
}

class RequestRejectedScreen extends StatefulWidget {
  const RequestRejectedScreen({super.key});

  @override
  State<RequestRejectedScreen> createState() => _RequestRejectedScreenState();
}

class _RequestRejectedScreenState extends State<RequestRejectedScreen> {
  RequestRejectedDetails? _details;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadDetails();
  }

  Future<void> _loadDetails() async {
    await Future.delayed(const Duration(milliseconds: 300));
    if (!mounted) return;
    setState(() {
      _details = const RequestRejectedDetails(
        statusLabel: "تم الرفض",
        companyMessage:
        "رفضت شركة مدري شو انضمامك لبرنامج إلى الإمارات العربية المتحدة.",
        reasonLabel: "السبب",
        reasonText:
        "أضف وصفاً عاماً عن الرحلة مع ما ستضمنه من خدمات وفنادق ومطاعم بشكل مختصر يجذب السياح بمجرد قرائته",
        cancelLabel: "إلغاء",
        resendLabel: "إعادة طلب",
      );
      _isLoading = false;
    });
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
            child: _TopBar(userName: 'محمد عبد الرحمن', onNotificationTap: () {}),
          ),

          if (!_isLoading && _details != null)
            Positioned(
              bottom: 0,
              left: 0,
              right: 0,
              child: _DraggableRejectedCard(
                details: _details!,
                onCancel: () {},
                onResend: () {},
              ),
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

class _DraggableRejectedCard extends StatefulWidget {
  final RequestRejectedDetails details;
  final VoidCallback onCancel;
  final VoidCallback onResend;

  const _DraggableRejectedCard({
    required this.details,
    required this.onCancel,
    required this.onResend,
  });

  @override
  State<_DraggableRejectedCard> createState() => _DraggableRejectedCardState();
}

class _DraggableRejectedCardState extends State<_DraggableRejectedCard>
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
      child: _RejectedCardShell(
        isExpanded: _expanded,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 72.w,
              height: 4.h,
              margin: EdgeInsets.only(top: 8.h, bottom: 8.h),
              decoration: BoxDecoration(
                color: Colors.white.withOpacity(0.8),
                borderRadius: BorderRadius.circular(10.r),
              ),
            ),

            if (!_expanded)
              GestureDetector(
                onTap: _toggleCard,
                child: Padding(
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
                      mainAxisAlignment: MainAxisAlignment.center,
                      textDirection: TextDirection.rtl,
                      children: [
                        SizedBox(
                          width: 28.w,
                          height: 28.h,
                          child: Image.asset(
                            'assets/icons/Warning.png',
                            fit: BoxFit.contain,
                          ),
                        ),
                        SizedBox(width: 8.w),
                        Text(
                          widget.details.statusLabel,
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
                ),
              ),

            SizeTransition(
              sizeFactor: _anim,
              axisAlignment: -1,
              child: _RejectedDetailsPanel(
                details: widget.details,
                onCancel: widget.onCancel,
                onResend: widget.onResend,
                onClose: _toggleCard,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _RejectedDetailsPanel extends StatelessWidget {
  final RequestRejectedDetails details;
  final VoidCallback onCancel;
  final VoidCallback onResend;
  final VoidCallback onClose;

  const _RejectedDetailsPanel({
    required this.details,
    required this.onCancel,
    required this.onResend,
    required this.onClose,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 24.w),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          GestureDetector(
            onTap: onClose,
            child: Padding(
              padding: EdgeInsets.only(top: 2.h, bottom: 6.h),
              child: SvgPicture.asset(
                'assets/icons/arrow down.svg',
                width: 150.w,
                height: 100.h,
              ),
            ),
          ),

      Transform.translate(
        offset: Offset(0, -25.h),
        child:SizedBox(
            width: 112.w,
            height: 112.h,
            child: Image.asset(
              'assets/icons/Rejected.png',
              fit: BoxFit.contain,
            ),
        )  ),
      Transform.translate(
        offset: Offset(0, -40.h),
        child:Text(
          details.statusLabel,
            textDirection: TextDirection.rtl,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 36.sp,
              fontWeight: FontWeight.w500,
              color: Colors.black,
            ),
          ),),

      Transform.translate(
        offset: Offset(0, -30.h),
        child:SizedBox(
            width: 320.w,
            child: Text(
              details.companyMessage,
              textDirection: TextDirection.rtl,
              textAlign: TextAlign.center,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black54,
                height: 1.2,
              ),
            ),
        )),

          SizedBox(height: 12.h),

          Transform.translate(
            offset: Offset(0, -35.h),
            child: _ReasonCostBox(details: details),
          ),

          SizedBox(height: 16.h),
          Transform.translate(
            offset: Offset(0, -30.h),
            child: _RejectedActionButtons(onCancel: onCancel, onResend: onResend),
          ),
          SizedBox(height: 12.h),
        ],
      ),
    );
  }
}

class _ReasonCostBox extends StatelessWidget {
  final RequestRejectedDetails details;
  const _ReasonCostBox({required this.details});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 360.w,
      height: 145.h,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 17.h,
            left: 0,
            right: 0,
            child: SizedBox(
              width: 360.w,
              height: 128.h,
              child: Image.asset(
                  'assets/icons/CostRectangle.png',
                  fit: BoxFit.fill
              ),
            ),
          ),

          Positioned(
            top: 36.h,
            left: 20.w,
            right: 20.w,
            child: SizedBox(
              height: 95.h,
              child: Center(
                child: Text(
                  details.reasonText,
                  textDirection: TextDirection.rtl,
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 15.sp,
                    fontWeight: FontWeight.w400,
                    color: Colors.black87,
                    height: 1.3,
                  ),
                ),
              ),
            ),
          ),

          Positioned(
            top: 0,
            right: 20.w,
            child: Row(
              textDirection: TextDirection.rtl,
              children: [
                SizedBox(
                  width: 26.w,
                  height: 26.h,
                  child: Image.asset('assets/icons/Cause.png', fit: BoxFit.contain),
                ),
                SizedBox(width: 10.w),
                Container(
                  color: const Color(0xFFF1F7F3).withOpacity(0.01),
                  padding: EdgeInsets.symmetric(horizontal: 4.w),
                  child: Text(
                    details.reasonLabel,
                    textDirection: TextDirection.rtl,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 24.sp,
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
    );
  }
}

class _RejectedActionButtons extends StatelessWidget {
  final VoidCallback onCancel;
  final VoidCallback onResend;

  const _RejectedActionButtons({required this.onCancel, required this.onResend});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      textDirection: TextDirection.rtl,
      children: [
        SizedBox(
          width: 129.w,
          height: 46.h,
          child: ElevatedButton(
            onPressed: onResend,
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFFF4A261),
              elevation: 0,
              padding: EdgeInsets.zero,
              alignment: Alignment.centerRight,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12.r)),
            ),
              child: Text(
              'إعادة طلب',
              softWrap: false,
              overflow: TextOverflow.visible,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black,
              ),
            ),
            ),
        ),

        SizedBox(width: 24.w),

        SizedBox(
          width: 129.w,
          height: 46.h,
          child: OutlinedButton(
            onPressed: onCancel,
            style: OutlinedButton.styleFrom(
              side: const BorderSide(color: Color(0xFFDB1518), width: 1),
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12.r)),
            ),
            child: Text(
              'إلغاء',
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w500,
                color: const Color(0xFFDB1518),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _RejectedCardShell extends StatelessWidget {
  final Widget child;
  final bool isExpanded;

  const _RejectedCardShell({required this.child, required this.isExpanded});

  @override
  Widget build(BuildContext context) {
    const topRadius = Radius.circular(70);
    const borderRadius = BorderRadius.only(topLeft: topRadius, topRight: topRadius);

    return ClipRRect(
      borderRadius: borderRadius,
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
        child: _RejectedGradientBorderPainter(
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
                  Color(0xFFDB6A6A),
                  Color(0x8CDB6A6A),
                  Color(0x1ADB6A6A),
                  Color(0x00DB6A6A),
                ],
              ),
            ),
            padding: EdgeInsets.fromLTRB(0, 4.h, 0, isExpanded ? 85.h : 135.h),
            child: child,
          ),
        ),
      ),
    );
  }
}

class _RejectedGradientBorderPainter extends StatelessWidget {
  final Widget child;
  final BorderRadius borderRadius;

  const _RejectedGradientBorderPainter({required this.child, required this.borderRadius});

  @override
  Widget build(BuildContext context) {
    return CustomPaint(
      painter: _RejectedBorderPainter(borderRadius: borderRadius),
      child: child,
    );
  }
}

class _RejectedBorderPainter extends CustomPainter {
  final BorderRadius borderRadius;
  const _RejectedBorderPainter({required this.borderRadius});

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
  bool shouldRepaint(covariant _RejectedBorderPainter old) => old.borderRadius != borderRadius;
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
              color: Color(0xFFDB6A6A),
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