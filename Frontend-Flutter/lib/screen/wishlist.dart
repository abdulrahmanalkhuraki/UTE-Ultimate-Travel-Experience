import 'dart:ui';
import 'wishlist_data.dart';

import 'bottomNavigationBar.dart';
import 'package:flutter/material.dart' hide BottomNavigationBar;
import 'search_screen.dart';
import 'app_constants.dart';

class WishlistScreen extends StatefulWidget {
  const WishlistScreen({super.key});

  @override
  State<WishlistScreen> createState() => _WishlistScreenState();
}

class _WishlistScreenState extends State<WishlistScreen> {
  int _selectedTab = 0;

  final List<Map<String, dynamic>> programsData = [
    {
      'id': '1',
      'location': 'دبي _ ابو ظبي _ عجمان',
      'date': 'من 20-4 الى 20-5',
      'price': '4500\$',
      'image': 'assets/images/Eiffel.png',
    },
    {
      'id': '2',
      'location': 'باريس _ ديزني لاند _ ليون',
      'date': 'من 01-7 الى 15-7',
      'price': '6200\$',
      'image': 'assets/images/Eiffel.png',
    },
    {
      'id': '3',
      'location': 'إسطنبول _ بورصة _ سبانجا',
      'date': 'من 10-8 الى 25-8',
      'price': '3100\$',
      'image': 'assets/images/Eiffel.png',
    },
    {
      'id': '4',
      'location': 'طوكيو _ كيوتو _ أوساكا',
      'date': 'من 05-9 الى 20-9',
      'price': '5800\$',
      'image': 'assets/images/Eiffel.png',
    },
  ];

  List<Map<String, dynamic>> get wishlistedPrograms {
    return WishlistData.instance.getWishlistedPrograms(programsData);
  }

  @override
  Widget build(BuildContext context) {
    final bool isEmpty = wishlistedPrograms.isEmpty;

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
                            child: CustomHeaderTitle(title: 'أتمنى زيارتها'),
                          ),
                        ),
                        const CustomBackButton(),
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

                  if (isEmpty) ...[
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
                  ] else ...[
                    Padding(
                      padding: const EdgeInsets.only(top: 16, left: 16, right: 16),
                      child: Container(
                        width: 250,
                        height: 65,
                        decoration: BoxDecoration(
                          color: Colors.white.withOpacity(0.3),
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Row(
                          children: [
                            Expanded(
                              child: GestureDetector(
                                onTap: () => setState(() => _selectedTab = 1),
                                child: AnimatedContainer(
                                  duration: const Duration(milliseconds: 200),
                                  height: 65,
                                  decoration: BoxDecoration(
                                    color: _selectedTab == 1
                                        ? const Color(0xFFF4A261)
                                        : Colors.transparent,
                                    borderRadius: BorderRadius.circular(20),
                                  ),
                                  child: Center(
                                    child: Text(
                                      'الشركات\nالمفضلة',
                                      textAlign: TextAlign.center,
                                      style: TextStyle(
                                        fontFamily: 'Tajawal',
                                        fontSize: 20,
                                        fontWeight: FontWeight.w400,
                                        color: _selectedTab == 1
                                            ? Colors.black
                                            : const Color(0xFF8E8E93),
                                      ),
                                    ),
                                  ),
                                ),
                              ),
                            ),
                            Expanded(
                              child: GestureDetector(
                                onTap: () => setState(() => _selectedTab = 0),
                                child: AnimatedContainer(
                                  duration: const Duration(milliseconds: 200),
                                  height: 65,
                                  decoration: BoxDecoration(
                                    color: _selectedTab == 0
                                        ? const Color(0xFFF4A261)
                                        : Colors.transparent,
                                    borderRadius: BorderRadius.circular(20),
                                  ),
                                  child: Center(
                                    child: Text(
                                      'البرامج\nالجماعية',
                                      textAlign: TextAlign.center,
                                      style: TextStyle(
                                        fontFamily: 'Tajawal',
                                        fontSize: 20,
                                        fontWeight: FontWeight.w400,
                                        color: _selectedTab == 0
                                            ? Colors.black
                                            : const Color(0xFF8E8E93),
                                      ),
                                    ),
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    Expanded(
                      child: SingleChildScrollView(
                        padding: const EdgeInsets.only(bottom: 100),
                        child: _selectedTab == 0
                            ? _buildProgramsList()
                            : const Center(
                          child: Padding(
                            padding: EdgeInsets.only(top: 50),
                            child: Text(
                              'لا توجد شركات مفضلة',
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 24,
                                color: Color(0xFF455A64),
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ),
          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: AppBottomNavBar(selectedIndex: 1),
          ),
        ],
      ),
    );
  }

  Widget _buildProgramsList() {
    return ListView.builder(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: wishlistedPrograms.length,
      itemBuilder: (context, index) {
        final currentProgram = wishlistedPrograms[index];

        return Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
          child: LayoutBuilder(
            builder: (context, constraints) {
              final cardWidth = constraints.maxWidth;
              final cardHeight = cardWidth * (241 / 396);
              final infoHeight = cardHeight * (114.34 / 241);

              return Container(
                width: cardWidth,
                height: cardHeight,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(20),
                  boxShadow: [
                    BoxShadow(
                      color: const Color(0x00000000).withOpacity(0.25),
                      offset: const Offset(0, 4),
                      blurRadius: 4,
                      spreadRadius: 5,
                    ),
                  ],
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(20),
                  child: Stack(
                    children: [
                      Positioned.fill(
                        child: Image.asset(
                          currentProgram['image'],
                          fit: BoxFit.cover,
                        ),
                      ),
                      Positioned(
                        top: cardHeight * (20 / 241),
                        right: cardWidth * (17 / 396),
                        child: GestureDetector(
                          onTap: () {
                            setState(() {
                              WishlistData.instance.toggleWishlist(currentProgram['id']);
                            });
                          },
                          child: SizedBox(
                            width: 41,
                            height: 31.12,
                            child: Image.asset(
                              'assets/icons/addToWishList.png',
                              fit: BoxFit.contain,
                              color: Colors.orange,
                              colorBlendMode: BlendMode.srcIn,
                            ),
                          ),
                        ),
                      ),
                      Positioned(
                        bottom: 0,
                        left: 0,
                        right: 0,
                        height: infoHeight,
                        child: ClipRRect(
                          borderRadius: const BorderRadius.only(
                            topLeft: Radius.circular(20),
                            topRight: Radius.circular(20),
                          ),
                          child: Stack(
                            children: [
                              Positioned.fill(
                                child: BackdropFilter(
                                  filter: ImageFilter.blur(sigmaX: 1, sigmaY: 1),
                                  child: const SizedBox.expand(),
                                ),
                              ),
                              Positioned.fill(
                                child: Container(
                                  decoration: const BoxDecoration(
                                    borderRadius: BorderRadius.only(
                                      topLeft: Radius.circular(20),
                                      topRight: Radius.circular(20),
                                    ),
                                    gradient: LinearGradient(
                                      begin: Alignment.topCenter,
                                      end: Alignment.bottomCenter,
                                      colors: [
                                        Color(0x1A91B3FA),
                                        Color(0x1A000000),
                                      ],
                                    ),
                                    boxShadow: [
                                      BoxShadow(
                                        color: Color(0x40000000),
                                        offset: Offset(0, -10),
                                        blurRadius: 4,
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                              Padding(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 16, vertical: 8),
                                child: Column(
                                  mainAxisSize: MainAxisSize.min,
                                  crossAxisAlignment: CrossAxisAlignment.end,
                                  children: [
                                    Row(
                                      mainAxisAlignment: MainAxisAlignment.end,
                                      children: [
                                        Flexible(
                                          child: Text(
                                            currentProgram['location'],
                                            style: const TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: 16,
                                                color: Colors.white),
                                            overflow: TextOverflow.ellipsis,
                                            maxLines: 1,
                                            textAlign: TextAlign.right,
                                          ),
                                        ),
                                        const SizedBox(width: 8),
                                        Image.asset('assets/icons/Location.png',
                                            width: 20, height: 20),
                                      ],
                                    ),
                                    const SizedBox(height: 6),
                                    Row(
                                      mainAxisAlignment: MainAxisAlignment.end,
                                      children: [
                                        Text(
                                          currentProgram['date'],
                                          style: const TextStyle(
                                              fontFamily: 'Tajawal',
                                              fontSize: 14,
                                              color: Colors.white70),
                                        ),
                                        const SizedBox(width: 8),
                                        Image.asset('assets/icons/Calender 2.png',
                                            width: 20, height: 20),
                                      ],
                                    ),
                                    const SizedBox(height: 6),
                                    Row(
                                      mainAxisAlignment:
                                      MainAxisAlignment.spaceBetween,
                                      children: [
                                        Row(
                                          children: List.generate(5, (starIndex) {
                                            return GestureDetector(
                                              onTap: () {
                                                setState(() {
                                                  WishlistData.instance.setRating(
                                                      currentProgram['id'],
                                                      starIndex + 1);
                                                });
                                              },
                                              child: Image.asset(
                                                WishlistData.instance.getRating(
                                                    currentProgram['id']) >
                                                    starIndex
                                                    ? 'assets/icons/star1.png'
                                                    : 'assets/icons/star2.png',
                                                width: 18,
                                                height: 18,
                                              ),
                                            );
                                          }),
                                        ),
                                        Row(
                                          children: [
                                            Text(
                                              currentProgram['price'],
                                              style: const TextStyle(
                                                  fontFamily: 'Tajawal',
                                                  fontSize: 16,
                                                  color: Colors.white,
                                                  fontWeight: FontWeight.bold),
                                            ),
                                            const SizedBox(width: 4),
                                            Image.asset('assets/icons/Wallet.png',
                                                width: 20, height: 20),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          ),
        );
      },
    );
  }
}