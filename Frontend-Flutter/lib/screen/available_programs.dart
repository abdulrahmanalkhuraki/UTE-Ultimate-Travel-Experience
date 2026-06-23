import 'package:ute_app/screen/search_screen.dart';
import 'package:ute_app/utils/bottomNavigationBar.dart';
import 'package:ute_app/utils/constants.dart';
import '../model/wishlist_data.dart';
import 'dart:ui';
import 'package:flutter/material.dart' hide BottomNavigationBar;

class AvailableProgramsPage extends StatefulWidget {
  const AvailableProgramsPage({super.key});

  @override
  State<AvailableProgramsPage> createState() => _AvailableProgramsPageState();
}

class _AvailableProgramsPageState extends State<AvailableProgramsPage> {

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

  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Container(
            decoration: AppColors.backgroundGradient,
            child: SafeArea(
              child: SingleChildScrollView(
                physics: const BouncingScrollPhysics(),
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
                              child: CustomHeaderTitle(title: 'البرامج المتاحة'),

                            ),
                          ),
                          const SizedBox(width: 4),
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

                    ListView.builder(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: programsData.length,
                      itemBuilder: (context, index) {
                        final currentProgram = programsData[index];

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
                                          child: Image.asset(
                                            'assets/icons/addToWishList.png',
                                            fit: BoxFit.contain,
                                            color: WishlistData.instance.isWishlisted(currentProgram['id'])
                                                ? Colors.orange
                                                : null,
                                            colorBlendMode: WishlistData.instance.isWishlisted(currentProgram['id'])
                                                ? BlendMode.srcIn
                                                : null,
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
                                                  decoration: BoxDecoration(
                                                    borderRadius: const BorderRadius.only(
                                                      topLeft: Radius.circular(20),
                                                      topRight: Radius.circular(20),
                                                    ),
                                                    gradient: const LinearGradient(
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
                                                        spreadRadius: 0,
                                                      ),
                                                    ],
                                                  ),
                                                ),
                                              ),
                                              Padding(
                                                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
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
                                                            style: const TextStyle(fontFamily: 'Tajawal', fontSize: 16, color: Colors.white),
                                                            overflow: TextOverflow.ellipsis,
                                                            maxLines: 1,
                                                            textAlign: TextAlign.right,
                                                          ),
                                                        ),
                                                        const SizedBox(width: 8),
                                                        Image.asset('assets/icons/Location.png', width: 20, height: 20),
                                                      ],
                                                    ),
                                                    const SizedBox(height: 6),
                                                    Row(
                                                      mainAxisAlignment: MainAxisAlignment.end,
                                                      children: [
                                                        Text(
                                                          currentProgram['date'],
                                                          style: const TextStyle(fontFamily: 'Tajawal', fontSize: 14, color: Colors.white70),
                                                        ),
                                                        const SizedBox(width: 8),
                                                        Image.asset('assets/icons/Calender 2.png', width: 20, height: 20),
                                                      ],
                                                    ),
                                                    const SizedBox(height: 6),

                                                    Row(
                                                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                                      children: [
                                                        Row(
                                                          children: List.generate(5, (starIndex) {
                                                            return GestureDetector(
                                                              onTap: () {
                                                                setState(() {
                                                                  WishlistData.instance.setRating(currentProgram['id'], starIndex + 1);
                                                                });
                                                              },
                                                              child: Image.asset(
                                                                WishlistData.instance.getRating(currentProgram['id']) > starIndex
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
                                                              style: const TextStyle(fontFamily: 'Tajawal', fontSize: 16, color: Colors.white, fontWeight: FontWeight.bold),
                                                            ),
                                                            const SizedBox(width: 4),
                                                            Image.asset('assets/icons/Wallet.png', width: 20, height: 20),
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
                    ),
                    const SizedBox(height: 100),
                  ],
                ),
              ),
            ),
          ),
          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: AppBottomNavBar(
              selectedIndex: 0,
            ),
          ),

        ],
      ),
    );
  }
}
