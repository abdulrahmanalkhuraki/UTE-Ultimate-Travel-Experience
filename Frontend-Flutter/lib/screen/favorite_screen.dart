import 'package:flutter_svg/flutter_svg.dart';
import 'bottomNavigationBar.dart';
import 'package:flutter/material.dart' hide BottomNavigationBar;
import 'package:tourism_app/search_screen.dart';
import 'app_constants.dart';

class FavoriteScreen extends StatelessWidget {
  const FavoriteScreen({super.key});

  static const List<Map<String, dynamic>> _favoritePrograms = [
    {
      'companyName': 'شركة مدري شو للسياحة والسفر',
      'description':
      'رحلة ساحرة مع الكثير من الاثارة التشويق وزيارة معظم الاماكن السياحية الجميلة في فرنسا متضمنة',
      'programsCount': 11,
      'touristsCount': 120,
      'rating': 5,
    },
    {
      'companyName': 'شركة مدري شو للسياحة والسفر',
      'description':
      'رحلة ساحرة مع الكثير من الاثارة التشويق وزيارة معظم الاماكن السياحية الجميلة في فرنسا متضمنة',
      'programsCount': 11,
      'touristsCount': 120,
      'rating': 3,
    },
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.transparent,
      body: Stack(
        children: [
          Positioned.fill(
            child: Container(
              decoration: AppColors.backgroundGradient,
              child: SafeArea(
                child: LayoutBuilder(
                  builder: (context, constraints) {
                    return SingleChildScrollView(
                      physics: const BouncingScrollPhysics(),
                      child: ConstrainedBox(
                        constraints:
                        BoxConstraints(minHeight: constraints.maxHeight),
                        child: IntrinsicHeight(
                          child: Column(
                            children: [
                              Padding(
                                padding: const EdgeInsets.only(
                                    top: 20, left: 20, right: 20),
                                child: Row(
                                  mainAxisAlignment:
                                  MainAxisAlignment.spaceBetween,
                                  children: [
                                    GestureDetector(
                                      onTap: () => Navigator.push(
                                        context,
                                        PageRouteBuilder(
                                          pageBuilder: (_, __, ___) =>
                                          const SearchScreen(),
                                        ),
                                      ),
                                      child: Hero(
                                        tag: 'search_bar_transition',
                                        child: Container(
                                          width: 56 * context.scale,
                                          height: 49 * context.scale,
                                          decoration: BoxDecoration(
                                            color: Colors.white
                                                .withOpacity(0.10),
                                            borderRadius:
                                            BorderRadius.circular(
                                                20 * context.scale),
                                            border: Border.all(
                                                color: Colors.black,
                                                width: 2),
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
                                        child: CustomHeaderTitle(
                                            title: 'المفضلة'),
                                      ),
                                    ),
                                    const SizedBox(width: 4),
                                    const CustomBackButton(),
                                  ],
                                ),
                              ),
                              Padding(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 32),
                                child: Container(
                                  width: double.infinity,
                                  height: 1.2,
                                  decoration: BoxDecoration(
                                    gradient: LinearGradient(
                                      begin: Alignment.centerLeft,
                                      end: Alignment.centerRight,
                                      colors: [
                                        const Color(0xFF666666)
                                            .withOpacity(0.5),
                                        const Color(0xFF000000),
                                        const Color(0xFF666666)
                                            .withOpacity(0.5),
                                      ],
                                    ),
                                  ),
                                ),
                              ),
                              SizedBox(height: 10 * context.scale),
                              for (final program in _favoritePrograms)
                                _FavoriteProgramCard(
                                  companyName:
                                  program['companyName'] as String,
                                  description:
                                  program['description'] as String,
                                  programsCount:
                                  program['programsCount'] as int,
                                  touristsCount:
                                  program['touristsCount'] as int,
                                  rating: program['rating'] as int,
                                ),
                              SizedBox(height: 90 * context.scale),
                              const Spacer(),
                            ],
                          ),
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
          ),
          const Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: AppBottomNavBar(selectedIndex: 2),
          ),
        ],
      ),
    );
  }
}

class _FavoriteProgramCard extends StatelessWidget {
  final String companyName;
  final String description;
  final int programsCount;
  final int touristsCount;
  final int rating;

  const _FavoriteProgramCard({
    required this.companyName,
    required this.description,
    required this.programsCount,
    required this.touristsCount,
    required this.rating,
  });

  static const double _avatarSize = 60;
  static const double _avatarSinkIntoCard = 24;
  static const double _topRowHeight = 30;
  static const double _starSize = 20.37;
  static const double _starSinkIntoCard = 15;
  static const double _topGroupsInwardPadding = 34;

  @override
  Widget build(BuildContext context) {
    final double avatarTopOverlap =
        (_avatarSize - _avatarSinkIntoCard) * context.scale;
    final double starBottomOverlap =
        (_starSize - _starSinkIntoCard) * context.scale;

    return Padding(
      padding: EdgeInsets.only(
        left: 20 * context.scale,
        right: 20 * context.scale,
        top: avatarTopOverlap,
        bottom: starBottomOverlap + 12 * context.scale,
      ),
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Stack(
            children: [
              Positioned.fill(
                child: SvgPicture.asset(
                  'assets/icons/Rectangular card.svg',
                  fit: BoxFit.fill,
                ),
              ),
              Padding(
                padding: EdgeInsets.only(
                  top: (_topRowHeight / 2 + 14) * context.scale,
                  left: 20 * context.scale,
                  right: 20 * context.scale,
                  bottom: (_starSize / 2 + 6) * context.scale,
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      companyName,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w400,
                        fontSize: 20 * context.scale,
                        color: Colors.black,
                      ),
                    ),
                    SizedBox(height: 6 * context.scale),
                    Text(
                      description,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w400,
                        fontSize: 14 * context.scale,
                        color: Colors.black,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),

          Positioned(
            top: -avatarTopOverlap,
            left: 0,
            right: 0,
            height: _avatarSize * context.scale,
            child: Padding(
              padding: EdgeInsets.symmetric(
                  horizontal: _topGroupsInwardPadding * context.scale),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  _CountItem(
                    icon: 'assets/icons/tourists.png',
                    number: '$programsCount',
                    label: 'برنامج',
                    iconFirst: true,
                  ),
                  Container(
                    width: _avatarSize * context.scale,
                    height: _avatarSize * context.scale,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: const Color(0xFFF4A261),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.25),
                          offset: const Offset(0, 4),
                          blurRadius: 4,
                        ),
                      ],
                    ),
                    // TODO: استبدالها بصورة شعار الشركة الفعلية من الباك إند
                  ),
                  _CountItem(
                    icon: 'assets/icons/tourists.png',
                    number: '$touristsCount',
                    label: 'سائح',
                    iconFirst: false,
                  ),
                ],
              ),
            ),
          ),

          Positioned(
            bottom: -starBottomOverlap,
            left: 12 * context.scale,
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: List.generate(
                rating,
                    (index) => Padding(
                  padding:
                  EdgeInsets.symmetric(horizontal: 1 * context.scale),
                  child: SvgPicture.asset(
                    'assets/icons/star5.svg',
                    width: _starSize * context.scale,
                    height: _starSize * context.scale,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _CountItem extends StatelessWidget {
  final String icon;
  final String number;
  final String label;
  final bool iconFirst;

  const _CountItem({
    required this.icon,
    required this.number,
    required this.label,
    required this.iconFirst,
  });

  @override
  Widget build(BuildContext context) {
    final iconWidget = Image.asset(
      icon,
      width: 24 * context.scale,
      height: 24 * context.scale,
      fit: BoxFit.contain,
    );

    final numberLabelWidget = Directionality(
      textDirection: TextDirection.rtl,
      child:Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(
          number,
          style: TextStyle(
            fontFamily: 'Tajawal',
            fontWeight: FontWeight.w700,
            fontSize: 15 * context.scale,
            color: Colors.black,
          ),
        ),
        SizedBox(width: 3 * context.scale),
        Text(
          label,
          style: TextStyle(
            fontFamily: 'Tajawal',
            fontWeight: FontWeight.w400,
            fontSize: 15 * context.scale,
            color: Colors.black,
          ),
        ),
      ],
         ) );

    return Row(
      mainAxisSize: MainAxisSize.min,
      children: iconFirst
          ? [iconWidget, SizedBox(width: 4 * context.scale), numberLabelWidget]
          : [numberLabelWidget, SizedBox(width: 4 * context.scale), iconWidget],
    );
  }
}