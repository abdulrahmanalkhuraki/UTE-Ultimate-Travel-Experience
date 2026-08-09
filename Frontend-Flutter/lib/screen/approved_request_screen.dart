import 'package:flutter/material.dart';
import 'app_constants.dart';
import 'dashboard_header_section.dart';

/// واجهة حالة "تمت الموافقة"
/// تستدعي الهيكل العام (DashboardHeaderSection) وتضيف تحته فقط
/// الفروقات الخاصة بهذه الحالة.
///
/// TODO: لم تصلنا بعد قياسات فيغما (Layout/Typography/Colors) الخاصة
/// بالمحتوى الذي يظهر أسفل الهيكل العام لهذه الحالة.
class ApprovedRequestScreen extends StatelessWidget {
  final DashboardStats stats;

  const ApprovedRequestScreen({super.key, required this.stats});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          children: [
            // ============ الهيكل العام (لا يُعدَّل هنا) ============
            DashboardHeaderSection(stats: stats),

            // النص "أكثر برامج طلباً" (بقياسات فيغما الدقيقة)
            const MostRequestedProgramsTitle(),

            // ============ TODO: محتوى خاص بحالة "تمت الموافقة" ============
          ],
        ),
      ),
    );
  }
}