import 'package:flutter/material.dart';
import 'package:ute_app/screen/available_programs.dart';
import 'package:ute_app/screen/trips_screen.dart';
import 'package:ute_app/screen/wishlist_screen.dart';


class AppBottomNavBar extends StatelessWidget {
  final int selectedIndex;

  const AppBottomNavBar({super.key, required this.selectedIndex});

  void _navigate(BuildContext context, int index) {
    if (index == selectedIndex) return;
    final Map<int, Widget> pages = {
      0: const AvailableProgramsPage(),
      1: const WishlistScreen(),
      3: const TripsScreen(),
    };
    final page = pages[index];
    if (page == null) return;
    Navigator.pushReplacement(
      context,
      PageRouteBuilder(
        pageBuilder: (_, __, ___) => page,
        transitionDuration: Duration.zero,
      ),
    );
  }

  static const _iconsNormal = ['home.png', 'addWishList.png', 'Heart.png', 'Calender.png', 'setting.png'];
  static const _iconsActive = ['home2.png', 'addWishList2.png', 'Heart2.png', 'calender2.png', 'setting2.png'];
  static const _labels = ['الرئيسية', 'أتمنى زيارتها', 'المفضلة', 'رحلاتي', 'الإعدادات'];

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 85,
      margin: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: Colors.black.withOpacity(0.50), width: 1),
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(19),
        child: Stack(
          children: [
            Positioned.fill(
              child: Image.asset(
                'assets/icons/nav_background.png',
                fit: BoxFit.fill,
              ),
            ),
            Row(
              children: List.generate(
                _iconsNormal.length,
                    (i) => Expanded(
                  child: _NavItem(
                    iconNormal: 'assets/icons/${_iconsNormal[i]}',
                    iconActive: 'assets/icons/${_iconsActive[i]}',
                    label: _labels[i],
                    isSelected: selectedIndex == i,
                    onTap: () => _navigate(context, i),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _NavItem extends StatelessWidget {
  final String iconNormal;
  final String iconActive;
  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  const _NavItem({
    required this.iconNormal,
    required this.iconActive,
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  static const _orange = Color(0xFFF4A261);

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      behavior: HitTestBehavior.opaque,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Image.asset(
            isSelected ? iconActive : iconNormal,
            width: isSelected ? 32 : 28,
            height: isSelected ? 32 : 28,
          ),
          if (isSelected) ...[
            const SizedBox(height: 3),
            Text(
              label,
              style: const TextStyle(
                fontFamily: 'Cairo',
                fontSize: 9,
                fontWeight: FontWeight.w700,
                color: _orange,
              ),
            ),
          ],
        ],
      ),
    );
  }
}