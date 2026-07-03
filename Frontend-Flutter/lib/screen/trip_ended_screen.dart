import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart' hide Path;
import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:tourism_app/bottomNavigationBar.dart';

class TripEndedDetails {
  final String statusLabel;
  final String messageText;
  final String ratingPrompt;
  final String reviewPrompt;
  final String cancelLabel;
  final String submitLabel;

  const TripEndedDetails({
    required this.statusLabel,
    required this.messageText,
    required this.ratingPrompt,
    required this.reviewPrompt,
    required this.cancelLabel,
    required this.submitLabel,
  });
}

class TripEndedScreen extends StatefulWidget {
  const TripEndedScreen({super.key});

  @override
  State<TripEndedScreen> createState() => _TripEndedScreenState();
}

class _TripEndedScreenState extends State<TripEndedScreen> {
  TripEndedDetails? _details;
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
      _details = const TripEndedDetails(
        statusLabel: "انتهت الرحلة",
        messageText: "نتمنى أن تكون استمتعت برحلتك ونرجو منك تقييم هذه الرحلة من خلال تجربتك",
        ratingPrompt: "ما هو تقييمك للرحلة :",
        reviewPrompt: "اترك مراجعة عن رحلتك:",
        cancelLabel: "إلغاء",
        submitLabel: "تقديم",
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
              child: _DraggableTripEndedCard(
                details: _details!,
                onCancel: () {},
                onSubmit: (rating, review) {
                  print("Rating: $rating, Review: $review");
                },
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

class _DraggableTripEndedCard extends StatefulWidget {
  final TripEndedDetails details;
  final VoidCallback onCancel;
  final Function(int rating, String review) onSubmit;

  const _DraggableTripEndedCard({
    required this.details,
    required this.onCancel,
    required this.onSubmit,

  });

  @override
  State<_DraggableTripEndedCard> createState() => _DraggableTripEndedCardState();
}

class _DraggableTripEndedCardState extends State<_DraggableTripEndedCard>
    with SingleTickerProviderStateMixin {
  bool _expanded = false;
  late AnimationController _ctrl;
  late Animation<double> _anim;

  int _currentRating = 0;
  final TextEditingController _reviewController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _ctrl = AnimationController(vsync: this, duration: const Duration(milliseconds: 350));
    _anim = CurvedAnimation(parent: _ctrl, curve: Curves.easeInOut);
  }

  @override
  void dispose() {
    _ctrl.dispose();
    _reviewController.dispose();
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
      child: _TripEndedCardShell(
        isExpanded: _expanded,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              width: 72.w,
              height: 4.h,
              margin: EdgeInsets.only(top: 8.h, bottom: 12.h),
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
                        image: AssetImage('assets/icons/endRectangle.png'),
                        fit: BoxFit.fill,
                      ),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      textDirection: TextDirection.rtl,
                      children: [
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
              axisAlignment: -3,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 24.w),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    GestureDetector(
                      onTap: _toggleCard,
                      child: Padding(
                        padding: EdgeInsets.only(bottom: 12.h),
                        child: SvgPicture.asset(
                          'assets/icons/arrow down.svg',
                          width: 60.w,
                          height: 65.h,
                        ),
                      ),
                    ),
                    SizedBox(height: 5.h),

                Text(
                      widget.details.statusLabel,
                      textDirection: TextDirection.rtl,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 36.sp,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                      ),
                    ),
                    SizedBox(height: 10.h),

                    SizedBox(
                      width: 340.w,
                      child: Text(
                        widget.details.messageText,
                        textDirection: TextDirection.rtl,
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 20.sp,
                          fontWeight: FontWeight.w400,
                          color: Colors.black.withOpacity(0.8),
                          height: 1.3,
                        ),
                      ),
                  ) ,
                    SizedBox(height: 10.h),

                    Text(
                      widget.details.ratingPrompt,
                      textDirection: TextDirection.rtl,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 18.sp,
                        fontWeight: FontWeight.w500,
                        color: Colors.black,
                      ),
                    ),
                    SizedBox(height: 7.h),

                    SizedBox(
                      width: 240.w,
                      height: 45.h,
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        textDirection: TextDirection.rtl,
                        children: List.generate(5, (index) {
                          int starValue = index + 1;

                          bool isSelected = starValue <= _currentRating;

                          return GestureDetector(
                            onTap: () {
                              setState(() {
                                if (_currentRating == starValue) {
                                  _currentRating = starValue - 1;
                                } else {
                                  _currentRating = starValue;
                                }
                              });
                            },
                            child: SvgPicture.asset(
                              isSelected ? 'assets/icons/star5.svg' : 'assets/icons/star4.svg',
                              width: 38.w,
                              height: 38.h,
                            ),
                          );
                        }),
                      ),


                ),
                    SizedBox(height: 12.h),

                    Text(
                      widget.details.reviewPrompt,
                      textDirection: TextDirection.rtl,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 20.sp,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
    ),
                    ),
                    SizedBox(height: 8.h),

                Container(
                      width: 299.w,
                      height: 86.h,
                      decoration: const BoxDecoration(
                        image: DecorationImage(
                          image: AssetImage('assets/icons/endRectangle.png'),
                          fit: BoxFit.fill,
                        ),
                      ),
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(25.r),
                        child: BackdropFilter(
                          filter: ImageFilter.blur(sigmaX: 10, sigmaY: 10),
                          child: TextField(
                            controller: _reviewController,
                            maxLines: null,
                            expands: true,
                            textAlign: TextAlign.right,
                            textDirection: TextDirection.rtl,
                            style: TextStyle(fontFamily: 'Tajawal', fontSize: 15.sp),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(horizontal: 14.w, vertical: 12.h),
                            ),
                          ),
                        ),
                       ),
                    ),
                    SizedBox(height: 15.h),

                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      textDirection: TextDirection.rtl,
                      children: [
                        SizedBox(
                          width: 125.w,
                          height: 44.h,
                          child: ElevatedButton(
                            onPressed: () => widget.onSubmit(_currentRating, _reviewController.text),
                            style: ElevatedButton.styleFrom(
                              backgroundColor: const Color(0xFFF4A261),
                              elevation: 0,
                              padding: EdgeInsets.zero,
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12.r)),
                            ),
                            child: Text(
                              widget.details.submitLabel,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 22.sp,
                                fontWeight: FontWeight.w500,
                                color: Colors.black,
                              ),
                            ),
                          ),
                        ),
                        SizedBox(width: 20.w),

                        SizedBox(
                          width: 125.w,
                          height: 44.h,
                          child: OutlinedButton(
                            onPressed: widget.onCancel,
                            style: OutlinedButton.styleFrom(
                              side: const BorderSide(color: Color(0xFFDB1518), width: 1),
                              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12.r)),
                            ),
                            child: Text(
                              widget.details.cancelLabel,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 22.sp,
                                fontWeight: FontWeight.w500,
                                color: const Color(0xFFDB1518),
                              ),
                            ),

                          ),
                        ),
                      ],
                       ),
                    SizedBox(height: 10.h),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _TripEndedCardShell extends StatelessWidget {
  final Widget child;
  final bool isExpanded;

  const _TripEndedCardShell({required this.child, required this.isExpanded});

  @override
  Widget build(BuildContext context) {
    const topRadius = Radius.circular(70);
    const borderRadius = BorderRadius.only(topLeft: topRadius, topRight: topRadius);

    return ClipRRect(
      borderRadius: borderRadius,
      child: BackdropFilter(
        filter: ImageFilter.blur(sigmaX: 20, sigmaY: 20),
        child: _TripEndedGradientBorderPainter(
          borderRadius: borderRadius,
          child: Container(
            width: double.infinity,
            constraints: BoxConstraints(
              minHeight: isExpanded ? MediaQuery.of(context).size.height * 0.75 : 0.0,
            ),
            decoration: const BoxDecoration(
              borderRadius: borderRadius,
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                stops: [0.0, 0.043, 0.173, 1.0],
                colors: [
                  Color(0xFFE7D37F),
                  Color(0x8CE7D37F),
                  Color(0x1AE7D37F),
                  Color(0x00E7D37F),
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

class _TripEndedGradientBorderPainter extends StatelessWidget {
  final Widget child;
  final BorderRadius borderRadius;

  const _TripEndedGradientBorderPainter({required this.child, required this.borderRadius});

  @override
  Widget build(BuildContext context) {
    return CustomPaint(
      painter: _TripEndedBorderPainter(borderRadius: borderRadius),
      child: child,
    );
  }
}

class _TripEndedBorderPainter extends CustomPainter {
  final BorderRadius borderRadius;
  const _TripEndedBorderPainter({required this.borderRadius});

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
  bool shouldRepaint(covariant _TripEndedBorderPainter old) => old.borderRadius != borderRadius;
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
              color: Color(0xFFE7D37F),
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