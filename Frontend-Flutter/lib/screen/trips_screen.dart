import 'package:flutter/material.dart' hide BottomNavigationBar;
import 'bottomNavigationBar.dart';
import 'app_constants.dart';
import 'available_programs.dart';
import 'search_screen.dart';

class TripsScreen extends StatefulWidget {
  const TripsScreen({super.key});

  @override
  State<TripsScreen> createState() => _TripsScreenState();
}
class _TripsScreenState extends State<TripsScreen> {
  int selectedTab = 1;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Container(
            width: double.infinity,
            height: double.infinity,
            decoration: AppColors.backgroundGradient,
          ),
          SafeArea(
            child: Column(
              children: [
                Padding(
                  padding: EdgeInsets.only(
                      top: 10 * context.scale,
                      left: 20 * context.scale,
                      right: 20 * context.scale
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      GestureDetector(
                        onTap: () => Navigator.push(context,
                            PageRouteBuilder(pageBuilder: (_, __, ___) => const SearchScreen()
                            )),
                        child: Hero(
                          tag: 'search_bar_transition',
                          child: Container(
                            width: 56 * context.scale,
                            height: 49 * context.scale,
                            decoration: BoxDecoration(
                              color: Colors.white.withOpacity(0.10),
                              borderRadius: BorderRadius.circular(20 * context.scale),
                              border: Border.all(color: Colors.black, width: 2),
                            ),
                            child: Center(
                              child: Image.asset(
                                'assets/icons/searchIcon.png',
                                width: 35 * context.scale,
                                height: 35 * context.scale,
                                fit: BoxFit.contain,
                              ),
                            ),
                          ),
                        ),
                      ),
                      Expanded(
                        child: Text('رحلاتي',
                            textAlign: TextAlign.center,
                            style: TextStyle(
                                fontFamily: 'Cairo',
                                fontSize: 40 * context.scale,
                                fontWeight: FontWeight.w700,
                                color: Colors.black,
                                height: 1.0
                            )),
                      ),
                      SizedBox(
                          width: 50 * context.scale,
                          height: 50 * context.scale,
                          child: IconButton(
                              padding: EdgeInsets.zero,
                              onPressed: () => Navigator.pop(context),
                              icon: Icon(
                                  Icons.arrow_forward_ios,
                                  size: 20 * context.scale,
                                  color: Colors.black
                              ))),
                    ],
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(
                      vertical: 8 * context.scale,
                      horizontal: 32 * context.scale
                  ),
                  child: Container(
                      width: double.infinity,
                      height: 1.2,
                      decoration: BoxDecoration(
                          gradient: LinearGradient(
                              colors: [const Color(0xFF666666).withOpacity(0.5),
                                Colors.black,
                                const Color(0xFF666666).withOpacity(0.5)
                              ]
                          )
                      )
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 15 * context.scale),
                  child: Container(
                    width: 370 * context.scale,
                    height: 65 * context.scale,
                    decoration: BoxDecoration(
                        color: const Color(0xFFF4A261).withOpacity(0.36),
                        borderRadius: BorderRadius.circular(20 * context.scale)),
                    child: Row(
                      children: [
                        Expanded(child: _buildTab('الملغاة', 2)),
                        Expanded(child: _buildTab('الحالية', 1)),
                        Expanded(child: _buildTab('السابقة', 0)),
                      ],
                    ),
                  ),
                ),
                Padding(
                  padding: EdgeInsets.only(top: 15 * context.scale),
                  child: Text('لم تقم بأي رحلة بعد',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 36 * context.scale,
                          fontWeight: FontWeight.w500,
                          color: Colors.black,
                          height: 1.0
                      )
                  ),
                ),
                Expanded(
                  child: Padding(
                    padding: EdgeInsets.symmetric(vertical: 8 * context.scale),
                    child: Image.asset('assets/images/myTrips.gif',
                        gaplessPlayback: true,
                        fit: BoxFit.contain
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
                          color: Colors.black
                      ),
                      children: [
                        const TextSpan(
                            text: 'ستظهر هنا جميع الرحلات التي قمت بخوضها مع '
                        ),
                        TextSpan(
                            text: 'UTE',
                            style: TextStyle(
                                fontFamily: 'ArslanWessam',
                                fontSize: 36 * context.scale
                            )
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
                      right: 24 * context.scale
                  ),
                  child: SizedBox(
                    width: 332 * context.scale,
                    height: 65 * context.scale,
                    child: ElevatedButton(
                      onPressed: () => Navigator.push(context,
                          MaterialPageRoute(builder: (_) => const AvailableProgramsPage())
                      ),
                      style: ElevatedButton.styleFrom(
                          backgroundColor: const Color(0xFFF4A261),
                          elevation: 0,
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(20 * context.scale))),
                      child: Text('ابدأ رحلتك الآن',
                          style: TextStyle(fontFamily: 'Tajawal',
                              fontSize: 36 * context.scale,
                              color: Colors.black
                          )),
                    ),
                  ),
                ),
                AppBottomNavBar(selectedIndex: 3),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTab(String title, int index) {
    final bool isActive = selectedTab == index;
    return GestureDetector(
      onTap: () => setState(() => selectedTab = index),
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        margin: EdgeInsets.all(4 * context.scale),
        alignment: Alignment.center,
        decoration: BoxDecoration(color: isActive ? const Color(0xFFF4A261) : Colors.transparent, borderRadius: BorderRadius.circular(20 * context.scale)),
        child: Text(title, style: TextStyle(
            fontFamily: 'Tajawal',
            fontSize: isActive ? 32 * context.scale : 28 * context.scale,
            fontWeight: isActive ? FontWeight.w500 : FontWeight.w400,
            color: isActive ? Colors.black : const Color(0xFF8E8E93))),
      ),
    );
  }
}