import 'package:flutter/material.dart';
import '../app_constants.dart';
import '../available_programs.dart';

// ════════════════════════════════════════════════════════
// تبويب "الملغاة" – واجهة فارغة
// ════════════════════════════════════════════════════════
class CancelledTripsTab extends StatelessWidget {
  const CancelledTripsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Padding(
          padding: EdgeInsets.only(top: 15 * context.scale),
          child: Text(
            'لم تقم بأي رحلة بعد',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 36 * context.scale,
              fontWeight: FontWeight.w500,
              color: Colors.black,
              height: 1.0,
            ),
          ),
        ),
        Expanded(
          child: Padding(
            padding: EdgeInsets.symmetric(vertical: 8 * context.scale),
            child: Image.asset(
              'assets/images/myTrips.gif',
              gaplessPlayback: true,
              fit: BoxFit.contain,
            ),
          ),
        ),
        Padding(
          padding: EdgeInsets.symmetric(horizontal: 21 * context.scale),
          child: RichText(
            textAlign: TextAlign.center,
            textDirection: TextDirection.rtl,
            text: TextSpan(
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 24 * context.scale,
                color: Colors.black,
              ),
              children: [
                const TextSpan(text: 'ستظهر هنا جميع الرحلات التي قمت بخوضها مع '),
                TextSpan(
                  text: 'UTE',
                  style: TextStyle(
                    fontFamily: 'ArslanWessam',
                    fontSize: 36 * context.scale,
                  ),
                ),
              ],
            ),
          ),
        ),
        SizedBox(height: 10 * context.scale),
        Padding(
          padding: EdgeInsets.only(
            bottom: 8 * context.scale,
            left: 24 * context.scale,
            right: 24 * context.scale,
          ),
          child: SizedBox(
            width: 332 * context.scale,
            height: 65 * context.scale,
            child: ElevatedButton(
              onPressed: (context as Element).findAncestorStateOfType<NavigatorState>() != null
                  ? () => Navigator.push(
                context,
                MaterialPageRoute(builder: (_) => const AvailableProgramsPage()),
              )
                  : null,
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color(0xFFF4A261),
                elevation: 0,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(20 * context.scale),
                ),
              ),
              child: Text(
                'ابدأ رحلتك الآن',
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 36 * context.scale,
                  color: Colors.black,
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
