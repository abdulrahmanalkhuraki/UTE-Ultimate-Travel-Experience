import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:tourism_app/bottomNavigationBar.dart';

class RequestAcceptedDetails {
  final String statusLabel;
  final String companyMessage;
  final String confirmMessage;
  final String costTitle;
  final String perPersonLabel;
  final String perPersonCost;
  final String totalLabel;
  final String totalCost;

  const RequestAcceptedDetails({
    required this.statusLabel,
    required this.companyMessage,
    required this.confirmMessage,
    required this.costTitle,
    required this.perPersonLabel,
    required this.perPersonCost,
    required this.totalLabel,
    required this.totalCost,
  });
}

class RequestAcceptedScreen extends StatefulWidget {
  const RequestAcceptedScreen({super.key});

  @override
  State<RequestAcceptedScreen> createState() => _RequestAcceptedScreenState();
}

class _RequestAcceptedScreenState extends State<RequestAcceptedScreen> {
  RequestAcceptedDetails? _details;
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
      _details = const RequestAcceptedDetails(
        statusLabel: "تم قبول طلبك",
        companyMessage:
        "قبلت شركة مدري شو طلب تفضيلاتك وانضمامك إلى رحلة إلى الإمارات العربية المتحدة والذي سيبدأ في 2026\\6\\16",
        confirmMessage:
        "يرجى قبول التكلفة الجديدة لتاكيد الحجز علماً أنه سيتم حسم التكلفة الإجمالية تلقائياً بعد قبولك.",
        costTitle: "التكلفة الجديدة",
        perPersonLabel: "للشخص الواحد:",
        perPersonCost: "2000",
        totalLabel: "التكلفة الإجمالية:",
        totalCost: "2000",
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
              child: _DraggableAcceptedCard(
                details: _details!,
                onAccept: () {},
                onReject: () {},
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

class _DraggableAcceptedCard extends StatefulWidget {
  final RequestAcceptedDetails details;
  final VoidCallback onAccept;
  final VoidCallback onReject;

  const _DraggableAcceptedCard({
    required this.details,
    required this.onAccept,
    required this.onReject,
  });

  @override
  State<_DraggableAcceptedCard> createState() => _DraggableAcceptedCardState();
}

class _DraggableAcceptedCardState extends State<_DraggableAcceptedCard>
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
                                width: 32.w,
                                height: 32.h,
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
              child: _AcceptedDetailsPanel(
                details: widget.details,
                onAccept: widget.onAccept,
                onReject: widget.onReject,
                onClose: _toggleCard,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _AcceptedDetailsPanel extends StatelessWidget {
  final RequestAcceptedDetails details;
  final VoidCallback onAccept;
  final VoidCallback onReject;
  final VoidCallback onClose;

  const _AcceptedDetailsPanel({
    required this.details,
    required this.onAccept,
    required this.onReject,
    required this.onClose,
  });

  @override
  Widget build(BuildContext context) {
    final maxHeight = MediaQuery.of(context).size.height * 0.72;

    return Container(
      constraints: BoxConstraints(maxHeight: maxHeight),
      child: SingleChildScrollView(
        physics: const BouncingScrollPhysics(),
        padding: EdgeInsets.symmetric(horizontal: 24.w),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            GestureDetector(
              onTap: onClose,
              child: Padding(
                padding: EdgeInsets.only(top: 0.1.h, bottom: 2.h),
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
              width: 340.w,
              height: 200.h,
              child: SvgPicture.asset('assets/images/true.svg',
                  fit: BoxFit.contain),
            ),),
            Transform.translate(
              offset: Offset(0, -55.h),
              child:Text(
              details.statusLabel,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 36.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black,
              ),
            ), ),

        Transform.translate(
          offset: Offset(0, -50.h),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              SizedBox(
                width: 380.w,
                child: Text(
                  details.companyMessage,
                  textDirection: TextDirection.rtl,
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 24.sp,
                    fontWeight: FontWeight.w400,
                    color: Colors.black,
                    height: 1.3,
                  ),
                ),
              ),

            SizedBox(height: 10.h),

            SizedBox(
              width: 342.w,
              child: Text(
                details.confirmMessage,
                textDirection: TextDirection.rtl,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 24.sp,
                  fontWeight: FontWeight.w400,
                  color: Colors.black,
                  height: 1.3,
                ),
              ),
            ),

            SizedBox(height: 16.h),

            _CostBox(details: details),

            SizedBox(height: 24.h),

            _ActionButtons(onAccept: onAccept, onReject: onReject),

            SizedBox(height: 10.h),
          ],
        ),
        )]) ),
    );
  }
}

class _CostBox extends StatelessWidget {
  final RequestAcceptedDetails details;
  const _CostBox({required this.details});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 360.w,
      height: 135.h,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Positioned(
            top: 17.h,
            left: 0,
            right: 0,
            child: SizedBox(
              width: 360.w,
              height: 118.h,
              child: Image.asset('assets/icons/CostRectangle.png', fit: BoxFit.fill),
            ),
          ),

          Positioned(
            top: 35.h,
            left: 20.w,
            child: _CostValue(amount: details.perPersonCost),
          ),
          Positioned(
            top: 42.h,
            right: 20.w,
            child: SizedBox(
              width: 134.w,
              height: 24.h,
              child: Text(
                details.perPersonLabel,
                textDirection: TextDirection.rtl,
                textAlign: TextAlign.right,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 20.sp,
                  fontWeight: FontWeight.w500,
                  color: Colors.black,
                ),
              ),
            ),
          ),

          Positioned(
            top: 78.h,
            left: 20.w,
            child: _CostValue(amount: details.totalCost),
          ),
          Positioned(
            top: 86.h,
            right: 20.w,
            child: SizedBox(
              width: 140.w,
              height: 24.h,
              child: Text(
                details.totalLabel,
                textDirection: TextDirection.rtl,
                textAlign: TextAlign.right,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 20.sp,
                  fontWeight: FontWeight.w500,
                  color: Colors.black,
                ),
              ),
            ),
          ),

          Positioned(
            top: 0,
            right: 16.w,
            child: Row(
              textDirection: TextDirection.rtl,
              children: [
                SizedBox(
                  width: 30.w,
                  height: 30.h,
                  child: Image.asset('assets/icons/cost.png', fit: BoxFit.contain),
                ),
                SizedBox(width: 6.w),
                Container(
                  color: const Color(0xFFF1F7F3).withOpacity(0.01),
                  padding: EdgeInsets.symmetric(horizontal: 4.w),
                  child: SizedBox(
                    width: 100.w,
                    height: 19.h,
                    child: Text(
                      details.costTitle,
                      textDirection: TextDirection.rtl,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 16.sp,
                        fontWeight: FontWeight.w500,
                        color: Colors.black,
                      ),
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

class _CostValue extends StatelessWidget {
  final String amount;
  const _CostValue({required this.amount});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 38.h,
      child: Text(
        "\$ $amount",
        style: TextStyle(
          fontFamily: 'AgencyFB',
          fontSize: 32.sp,
          fontWeight: FontWeight.w400,
          color: Colors.black,
        ),
      ),
    );
  }
}

class _ActionButtons extends StatelessWidget {
  final VoidCallback onAccept;
  final VoidCallback onReject;

  const _ActionButtons({required this.onAccept, required this.onReject});

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
            onPressed: onAccept,
            style: ElevatedButton.styleFrom(
              backgroundColor: const Color(0xFFF4A261),
              elevation: 0,
              padding: EdgeInsets.zero,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15.r)),
            ),
            child: Text(
              'قبول',
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w400,
                color: Colors.black,
              ),
            ),
          ),
        ),

        SizedBox(width: 40.w),

        SizedBox(
          width: 129.w,
          height: 46.h,
          child: OutlinedButton(
            onPressed: onReject,
            style: OutlinedButton.styleFrom(
              side: const BorderSide(color: Color(0xFFDB1518), width: 1),
              padding: EdgeInsets.zero,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(15.r)),
            ),
            child: Text(
              'رفض',
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24.sp,
                fontWeight: FontWeight.w400,
                color: const Color(0xFFDB1518),
              ),
            ),
          ),
        ),
      ],
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
            padding: EdgeInsets.fromLTRB(0, 8.h, 0, isExpanded ? 90.h : 135.h),
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