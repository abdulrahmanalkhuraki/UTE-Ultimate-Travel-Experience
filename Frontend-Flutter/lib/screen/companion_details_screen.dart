import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'bottomNavigationBar.dart';
import 'model/companion_model.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class CompanionDetailsScreen extends StatefulWidget {
  final CompanionModel companion;
  const CompanionDetailsScreen({super.key, required this.companion});

  @override
  State<CompanionDetailsScreen> createState() => _CompanionDetailsScreenState();
}

class _CompanionDetailsScreenState extends State<CompanionDetailsScreen> {
  @override
  Widget build(BuildContext context) {
    final double scale = MediaQuery.of(context).size.width / 440;
    final bool showIdSection = widget.companion.age > 14;

    return Scaffold(
      backgroundColor: Colors.white,
      body: Directionality(
        textDirection: TextDirection.rtl,
        child: Stack(
          children: [
            Positioned(top: 0, child: SvgPicture.asset('assets/images/Vector.svg')),
            SingleChildScrollView(
              padding: const EdgeInsets.only(top: 60, bottom: 120),
              child: Column(
                children: [
                  Text(widget.companion.relationshipName, style: const TextStyle(fontFamily: 'Cairo', fontSize: 36, fontWeight: FontWeight.bold)),
                  const SizedBox(height: 20),
                  SvgPicture.asset('assets/icons/Profile_Circle.svg', width: 200 * scale),
                  const SizedBox(height: 20),
                  Text("الاسم: ${widget.companion.name}", style: const TextStyle(fontSize: 20)),
                  Text("العمر: ${widget.companion.age}", style: const TextStyle(fontSize: 20)),
                  if (showIdSection) const Text("عرض الهوية"),
                  const Text("عرض جواز السفر"),
                ],
              ),
            ),
            const Positioned(bottom: 0, left: 0, right: 0, child: AppBottomNavBar(selectedIndex: 4)),
          ],
        ),
      ),
    );
  }
}