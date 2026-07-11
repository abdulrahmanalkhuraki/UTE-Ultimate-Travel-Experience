import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'bottomNavigationBar.dart';
import 'package:tourism_app/search_screen.dart';
import 'app_constants.dart';

class NoInternetScreen extends StatelessWidget {
  final String title;

  const NoInternetScreen({
    super.key,
    required this.title,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Container(
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
                            PageRouteBuilder(pageBuilder: (_, __, ___) => const SearchScreen()),
                          ),
                          child: Container(
                            width: 56 * context.scale,
                            height: 49 * context.scale,
                            decoration: BoxDecoration(
                              color: Colors.white.withOpacity(0.10),
                              borderRadius: BorderRadius.circular(20 * context.scale),
                              border: Border.all(color: Colors.black, width: 2),
                            ),
                            child: Center(
                              child: Image.asset('assets/icons/searchIcon.png', width: 35 * context.scale, height: 35 * context.scale),
                            ),
                          ),
                        ),
                        Expanded(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: CustomHeaderTitle(title: title),
                          ),
                        ),
                        const SizedBox(width: 4),
                        const CustomBackButton(),
                      ],
                    ),
                  ),

                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 10),
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
                    child: Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.start,
                        children: [
                          SvgPicture.asset(
                            'assets/icons/noInternet.svg',
                            width: 250,
                          ),
                          const SizedBox(height: 20),
                          const Padding(
                            padding: EdgeInsets.symmetric(horizontal: 30),
                            child: Text(
                              'ليس هناك اتصال بالإنترنت.. يرجى التأكد من اتصالكم والمحاولة من جديد',
                              textAlign: TextAlign.center,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 34,
                                fontWeight: FontWeight.w500,
                                color: Colors.black,
                                height: 1.2,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),

          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: AppBottomNavBar(selectedIndex: 0),
          ),
        ],
      ),
    );
  }
}