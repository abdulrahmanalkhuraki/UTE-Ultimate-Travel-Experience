import 'bottomNavigationBar.dart';
import 'package:flutter/material.dart' hide BottomNavigationBar;
import 'package:tourism_app/search_screen.dart';
import 'app_constants.dart';


class WishlistScreen extends StatefulWidget {
  const WishlistScreen({super.key});

  @override
  State<WishlistScreen> createState() => _WishlistScreenState();
}

class _WishlistScreenState extends State<WishlistScreen> {

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
        child: SafeArea(
          child: Column(
            children: [
              Padding(
                padding: const EdgeInsets.only(top: 20, left: 20, right: 20),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    GestureDetector(
                      onTap: () => Navigator.push(
                        context,
                        PageRouteBuilder(
                          pageBuilder: (_, __, ___) => const SearchScreen(),
                        ),
                      ),
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
                      child: FittedBox(
                        fit: BoxFit.scaleDown,
                        alignment: Alignment.center,
                        child: Text(
                          'أتمنى زيارتها',
                          style: TextStyle(
                            fontFamily: 'Cairo',
                            fontSize: 40,
                            fontWeight: FontWeight.w700,
                            color: Color(0xFF000000),
                          ),
                        ),
                      ),
                    ),
                    SizedBox(
                      width: 50,
                      height: 50,
                      child: IconButton(
                        padding: EdgeInsets.zero,
                        onPressed: () => Navigator.pop(context),
                        icon: const Icon(
                          Icons.arrow_forward_ios,
                          size: 20,
                          color: Color(0xFF000000),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 32),
                child: Container(
                  width: double.infinity,
                  height: 1.2,
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.centerLeft,
                      end: Alignment.centerRight,
                      colors: [
                        const Color(0xFF666666).withOpacity(0.5),
                        const Color(0xFF000000),
                        const Color(0xFF666666).withOpacity(0.5),
                      ],
                    ),
                  ),
                ),
              ),
              const Padding(
                padding: EdgeInsets.only(top: 54),
                child: Center(
                  child: SizedBox(
                    width: 337,
                    child: Text(
                      'لم يتم إضافة أي برنامج',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 36,
                        fontWeight: FontWeight.w500,
                        color: Color(0xFF000000),
                        height: 1.0,
                      ),
                    ),
                  ),
                ),
              ),
              Padding(
                padding: const EdgeInsets.only(top: 27),
                child: Center(
                  child: Image.asset(
                    'assets/images/wishList.gif',
                    width: 300,
                    height: 300,
                    fit: BoxFit.contain,
                    //repeat: true,
                  ),
                ),
              ),
              const Padding(
                padding: EdgeInsets.symmetric(horizontal: 21),
                child: Text(
                  'قم بإضافة البرامج التي ترغب بالالتحاق بها ليكون الوصول اليها اكثر سهولة وسلاسة',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontWeight: FontWeight.w500,
                    fontSize: 24,
                    height: 1.0,
                    color: Color(0xFF455A64),
                  ),
                ),
              ),
              const SizedBox(height: 10),
            ],
          ),
        ),
      ),
            Positioned(
              bottom: 0,
              left: 0,
              right: 0,
              child: AppBottomNavBar(
                selectedIndex: 1,
              ),
            ),
   ] ));
  }

}