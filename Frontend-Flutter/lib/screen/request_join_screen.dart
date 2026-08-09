import 'package:flutter/material.dart';
import 'app_constants.dart';

class JoinRequest {
  final String id;
  final String applicantName;
  final String programName;
  final int peopleCount;
  final int previousProgramsCount;
  final String timeAgoLabel;
  final Color avatarColor;
  final String? avatarImageUrl;
  final String ticketClassLabel;
  final String accommodationPreferenceText;
  final String foodPreferenceText;
  final String businessClassCostLabel;
  final String additionalPreferenceText;

  JoinRequest({
    required this.id,
    required this.applicantName,
    required this.programName,
    required this.peopleCount,
    required this.previousProgramsCount,
    required this.timeAgoLabel,
    this.avatarColor = const Color(0xFFF4A261),
    this.avatarImageUrl,
    this.ticketClassLabel = 'درجة اقتصادية',
    this.accommodationPreferenceText =
        'هون رح نبعت النص يلي كاتبو الساءح متل ما هوو',
    this.foodPreferenceText = 'هون رح نبعت النص يلي كاتبو الساءح متل ما هوو',
    this.businessClassCostLabel = 'تكلفة درجة رجال الأعمال',
    this.additionalPreferenceText =
        'هون رح نبعت النص يلي كاتبو الساءح متل ما هوو',
  });
}

class RequestJoinScreen extends StatefulWidget {
  const RequestJoinScreen({super.key});

  @override
  State<RequestJoinScreen> createState() => _RequestJoinScreenState();
}

class _RequestJoinScreenState extends State<RequestJoinScreen> {
  final List<JoinRequest> _joinRequests = [
    JoinRequest(
      id: '1',
      applicantName: 'محمد المدري شو',
      programName: 'اسم البرنامج',
      peopleCount: 3,
      previousProgramsCount: 11,
      timeAgoLabel: 'منذ 10 دقائق',
    ),
  ];

  void _handleAccept(JoinRequest request) {
    setState(() {
      _joinRequests.removeWhere((r) => r.id == request.id);
    });
  }

  void _handleReject(JoinRequest request) {
    setState(() {
      _joinRequests.removeWhere((r) => r.id == request.id);
    });
  }

  void _handleSubmit(JoinRequest request) {}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: false,
      body: Container(
        width: double.infinity,
        height: double.infinity,
        decoration: AppColors.backgroundGradient,
        child: SafeArea(
          child: Stack(
            children: [
              Column(
                children: [
                  Padding(
                    padding: const EdgeInsets.only(
                      top: 20,
                      left: 20,
                      right: 20,
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const SizedBox(width: 48),

                        Expanded(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: Text(
                              'طلبات الانضمام',
                              textAlign: TextAlign.center,
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: const TextStyle(
                                fontFamily: 'Cairo',
                                fontWeight: FontWeight.w700,
                                fontSize: 40,
                                height: 1.0,
                                letterSpacing: 2,
                                color: Color(0xFF000000),
                              ),
                            ),
                          ),
                        ),
                        const CustomBackButton(),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(
                      top: 5,
                      left: 32,
                      right: 32,
                      bottom: 20,
                    ),
                    child: Container(
                      width: double.infinity,
                      height: 1.2,
                      decoration: BoxDecoration(
                        gradient: LinearGradient(
                          colors: [
                            const Color(0xFF666666).withOpacity(0.5),
                            const Color(0xFF000000),
                            const Color(0xFF666666).withOpacity(0.5),
                          ],
                        ),
                      ),
                    ),
                  ),
                  Expanded(
                    child: _joinRequests.isEmpty
                        ? const Center(
                            child: Text(
                              'لا توجد طلبات انضمام حاليًا',
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 16,
                                color: Colors.black54,
                              ),
                            ),
                          )
                        : SingleChildScrollView(
                            child: Column(
                              children: _joinRequests
                                  .map(
                                    (request) => Padding(
                                      padding: const EdgeInsets.only(
                                        left: 20,
                                        right: 20,
                                        bottom: 20,
                                      ),
                                      child: Column(
                                        children: [
                                          JoinRequestCard(
                                            request: request,
                                            onAccept: () =>
                                                _handleAccept(request),
                                            onReject: () =>
                                                _handleReject(request),
                                          ),
                                          const SizedBox(height: 40),
                                          JoinRequestDetailsSection(
                                            request: request,
                                            onReject: () =>
                                                _handleReject(request),
                                            onSubmit: () =>
                                                _handleSubmit(request),
                                          ),
                                        ],
                                      ),
                                    ),
                                  )
                                  .toList(),
                            ),
                          ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class JoinRequestCard extends StatelessWidget {
  final JoinRequest request;
  final VoidCallback onAccept;
  final VoidCallback onReject;

  const JoinRequestCard({
    super.key,
    required this.request,
    required this.onAccept,
    required this.onReject,
  });

  static const double _groupWidth = 400;
  static const double _groupHeight = 224.5;

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final double availableWidth = constraints.maxWidth;
        final double scale = availableWidth / _groupWidth;
        final double scaledHeight = _groupHeight * scale;

        return SizedBox(
          width: availableWidth,
          height: scaledHeight,
          child: FittedBox(
            fit: BoxFit.fill,
            alignment: Alignment.topCenter,
            child: SizedBox(
              width: _groupWidth,
              height: _groupHeight,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  Positioned(
                    left: 0,
                    top: 36,
                    width: 400,
                    height: 163,
                    child: Image.asset(
                      'assets/icons/Rectangle11.png',
                      fit: BoxFit.fill,
                    ),
                  ),

                  Positioned(
                    left: 20,
                    top: 10,
                    width: 146,
                    height: 32,
                    child: Image.asset(
                      'assets/icons/bracket.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 25,
                    top: 17,
                    width: 135,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        '${request.previousProgramsCount} برنامج سابق معك',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 234,
                    top: 11,
                    width: 146,
                    height: 32,
                    child: Image.asset(
                      'assets/icons/bracket.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  // نص "3 أشخاص" -> left:260 top:17 w:68 h:19
                  Positioned(
                    left: 260,
                    top: 17,
                    width: 68,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        '${request.peopleCount} أشخاص',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 334,
                    top: 9,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/persons.png',
                      fit: BoxFit.contain,
                    ),
                  ),

                  Positioned(
                    left: 170,
                    top: 0,
                    width: 60,
                    height: 60,
                    child: Container(
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        color: request.avatarColor,
                        image: request.avatarImageUrl != null
                            ? DecorationImage(
                                image: NetworkImage(request.avatarImageUrl!),
                                fit: BoxFit.cover,
                              )
                            : null,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.25),
                            blurRadius: 4,
                            offset: const Offset(0, 4),
                          ),
                        ],
                      ),
                    ),
                  ),

                  Positioned(
                    left: 361,
                    top: 61,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/profile.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 213,
                    top: 64,
                    width: 143,
                    height: 24,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.applicantName,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 20,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 132,
                    top: 58,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/airplane_ticket.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 11,
                    top: 64,
                    width: 116,
                    height: 24,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.ticketClassLabel,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 20,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 72,
                    top: 89,
                    width: 256,
                    height: 34,
                    child: Text(
                      'يريد ${request.applicantName} الانضمام الى برنامجك برنامج "${request.programName}"',
                      textAlign: TextAlign.center,
                      textDirection: TextDirection.rtl,
                      style: const TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w400,
                        fontSize: 14,
                        height: 1.0,
                        color: Color(0xFF000000),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 50,
                    top: 138,
                    width: 129,
                    height: 46,
                    child: Material(
                      color: Colors.transparent,
                      borderRadius: BorderRadius.circular(15),
                      child: InkWell(
                        borderRadius: BorderRadius.circular(15),
                        onTap: onReject,
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(15),
                            border: Border.all(
                              color: const Color(0xFFDB1518),
                              width: 1,
                            ),
                          ),
                          alignment: Alignment.center,
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: const Text(
                              'رفض',
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 24,
                                height: 1.0,
                                color: Color(0xFFDB1518),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 221,
                    top: 138,
                    width: 129,
                    height: 46,
                    child: Material(
                      color: Colors.transparent,
                      borderRadius: BorderRadius.circular(15),
                      child: InkWell(
                        borderRadius: BorderRadius.circular(15),
                        onTap: onAccept,
                        child: Container(
                          decoration: BoxDecoration(
                            color: const Color(0xFFF4A261),
                            borderRadius: BorderRadius.circular(15),
                          ),
                          alignment: Alignment.center,
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: const Text(
                              'قبول',
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 24,
                                height: 1.0,
                                color: Color(0xFF000000),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 20,
                    top: 190,
                    width: 113,
                    height: 35,
                    child: Image.asset(
                      'assets/icons/arc2.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 35,
                    top: 198,
                    width: 83,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.timeAgoLabel,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }
}

class JoinRequestDetailsSection extends StatelessWidget {
  final JoinRequest request;
  final VoidCallback? onReject;
  final VoidCallback? onSubmit;

  const JoinRequestDetailsSection({
    super.key,
    required this.request,
    this.onReject,
    this.onSubmit,
  });

  static const double _groupWidth = 400;
  static const double _groupHeight =684; 

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final double availableWidth = constraints.maxWidth;
        final double scale = availableWidth / _groupWidth;
        final double scaledHeight = _groupHeight * scale;

        return SizedBox(
          width: availableWidth,
          height: scaledHeight,
          child: FittedBox(
            fit: BoxFit.fill,
            alignment: Alignment.topCenter,
            child: SizedBox(
              width: _groupWidth,
              height: _groupHeight,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  Positioned(
                    left: 0,
                    top: 0,
                    width: 400,
                    height: 684,
                    child: Image.asset(
                      'assets/icons/bigRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),

                  Positioned(
                    left: 20,
                    top: -26.6,
                    width: 146,
                    height: 33,
                    child: Image.asset(
                      'assets/icons/bracket.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 23,
                    top: -20.2,
                    width: 135,
                    height: 20.26,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        '${request.previousProgramsCount} برنامج سابق معك',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 232,
                    top: -26.6,
                    width: 146,
                    height: 33,
                    child: Image.asset(
                      'assets/icons/bracket.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 258,
                    top: -20.2,
                    width: 68,
                    height: 20.26,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        '${request.peopleCount} أشخاص',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 331.51,
                    top: -28.23,
                    width: 30.98,
                    height: 30.98,
                    child: Image.asset(
                      'assets/icons/persons.png',
                      fit: BoxFit.contain,
                    ),
                  ),

                  Positioned(
                    left: 167.02,
                    top: -33,
                    width: 61.96,
                    height: 61.96,
                    child: Container(
                      decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        color: request.avatarColor,
                        image: request.avatarImageUrl != null
                            ? DecorationImage(
                                image: NetworkImage(request.avatarImageUrl!),
                                fit: BoxFit.cover,
                              )
                            : null,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.25),
                            blurRadius: 4,
                            offset: const Offset(0, 4),
                          ),
                        ],
                      ),
                    ),
                  ),

                  Positioned(
                    left: 126,
                    top: 32.05,
                    width: 143,
                    height: 25.59,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.applicantName,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 20,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 70,
                    top: 61.91,
                    width: 256,
                    height: 36.26,
                    child: Text(
                      'يريد ${request.applicantName} الانضمام الى برنامجك برنامج "${request.programName}" مع بعض التفضيلات',
                      textAlign: TextAlign.center,
                      textDirection: TextDirection.rtl,
                      style: const TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w400,
                        fontSize: 14,
                        height: 1.0,
                        color: Color(0xFF000000),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 240,
                    top: 107,
                    width: 90,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        'تذاكر الطيران',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 330,
                    top: 97,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/airplane_ticket.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 20,
                    top: 117,
                    width: 360,
                    height: 50,
                    child: Image.asset(
                      'assets/icons/ticketRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 198,
                    top: 134,
                    width: 135,
                    height: 24,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.ticketClassLabel,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w300,
                          fontSize: 20,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 219,
                    top: 183,
                    width: 111,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        'تفضيلات الإقامة',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 330,
                    top: 172,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/hotel.png',
                      fit: BoxFit.contain,
                    ),
                  ),

                  Positioned(
                    left: 20,
                    top: 192,
                    width: 360,
                    height: 83,
                    child: Image.asset(
                      'assets/icons/hotelRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 87,
                    top: 211,
                    width: 253,
                    height: 48,
                    child: Text(
                      request.accommodationPreferenceText,
                      textAlign: TextAlign.center,
                      textDirection: TextDirection.rtl,
                      style: const TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w300,
                        fontSize: 20,
                        height: 1.0,
                        color: Color(0xFF000000),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 200,
                    top: 293,
                    width: 123,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        'تفضيلات الأطعمة',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 327,
                    top: 287,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/food.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 20,
                    top: 302,
                    width: 360,
                    height: 83,
                    child: Image.asset(
                      'assets/icons/foodRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 87,
                    top: 320,
                    width: 253,
                    height: 48,
                    child: Text(
                      request.foodPreferenceText,
                      textAlign: TextAlign.center,
                      textDirection: TextDirection.rtl,
                      style: const TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w300,
                        fontSize: 20,
                        height: 1.0,
                        color: Color(0xFF000000),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 215,
                    top: 408,
                    width: 112,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        'تفضيلات إضافية',
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 326,
                    top: 400,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/file.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 19,
                    top: 417,
                    width: 360,
                    height: 83,
                    child: Image.asset(
                      'assets/icons/hotelRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 67,
                    top: 432,
                    width: 253,
                    height: 48,
                    child: Text(
                      request.additionalPreferenceText,
                      textAlign: TextAlign.center,
                      textDirection: TextDirection.rtl,
                      style: const TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w300,
                        fontSize: 20,
                        height: 1.0,
                        color: Color(0xFF000000),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 29,
                    top: 519,
                    width: 340,
                    height: 75,
                    child: Image.asset(
                      'assets/icons/priceRectangle.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 324,
                    top: 539,
                    width: 30,
                    height: 30,
                    child: Image.asset(
                      'assets/icons/price.png',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    left: 70,
                    top: 542,
                    width: 239,
                    height: 29,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.businessClassCostLabel,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 24,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  Positioned(
                    left: 48,
                    top: 619,
                    width: 129,
                    height: 49.05,
                    child: Material(
                      color: Colors.transparent,
                      borderRadius: BorderRadius.circular(15),
                      child: InkWell(
                        borderRadius: BorderRadius.circular(15),
                        onTap: onReject,
                        child: Container(
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(15),
                            border: Border.all(
                              color: const Color(0xFFDB1518),
                              width: 1,
                            ),
                          ),
                          alignment: Alignment.center,
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: const Text(
                              'رفض',
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 24,
                                height: 1.0,
                                color: Color(0xFFDB1518),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 219,
                    top: 619,
                    width: 129,
                    height: 49.05,
                    child: Material(
                      color: Colors.transparent,
                      borderRadius: BorderRadius.circular(15),
                      child: InkWell(
                        borderRadius: BorderRadius.circular(15),
                        onTap: onSubmit,
                        child: Container(
                          decoration: BoxDecoration(
                            color: const Color(0xFFF4A261),
                            borderRadius: BorderRadius.circular(15),
                          ),
                          alignment: Alignment.center,
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: const Text(
                              'إرسال',
                              textDirection: TextDirection.rtl,
                              maxLines: 1,
                              softWrap: false,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontWeight: FontWeight.w400,
                                fontSize: 24,
                                height: 1.0,
                                color: Color(0xFF000000),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    left: 20,
                    top: 680,
                    width: 113,
                    height: 35,
                    child: Image.asset(
                      'assets/icons/arc2.png',
                      fit: BoxFit.fill,
                    ),
                  ),
                  Positioned(
                    left: 35,
                    top: 680,
                    width: 83,
                    height: 19,
                    child: FittedBox(
                      fit: BoxFit.scaleDown,
                      child: Text(
                        request.timeAgoLabel,
                        textAlign: TextAlign.center,
                        textDirection: TextDirection.rtl,
                        maxLines: 1,
                        softWrap: false,
                        style: const TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w400,
                          fontSize: 16,
                          height: 1.0,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }
}
