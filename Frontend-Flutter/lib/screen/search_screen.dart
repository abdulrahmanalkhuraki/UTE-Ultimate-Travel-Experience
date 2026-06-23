import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:ute_app/utils/constants.dart';

class SearchScreen extends StatefulWidget {
  const SearchScreen({super.key});

  @override
  State<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends State<SearchScreen> {
  bool isDropdownOpen = false;
  bool hasNoResults = false;
  final TextEditingController _searchController = TextEditingController();

  final List<String> countries = [
    'سوريا',
    'فرنسا',
    'الامارات',
    'ماليزيا',
    'لبنان',
    'الاردن',
    'مصر',
    'السعودية',
    'الكويت',
    'قطر',
    'تركيا',
    'عمان',
    'لندن',
    'ابو ظبي',
  ];

  List<String> filteredCountries = [];

  @override
  void initState() {
    super.initState();
    filteredCountries = countries;
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: false,
      body: Container(
        width: double.infinity,
        height: double.infinity,
        decoration: AppColors.backgroundGradient,
        child: SafeArea(
          child: Stack(
            children: [
              Column(
                children: [
                  Padding(
                    padding: const EdgeInsets.only(top: 20, left: 20, right: 20),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const SizedBox(width: 48),
                        Expanded(
                          child: CustomHeaderTitle(title: 'البحث'),
                        ),
                        const CustomBackButton(),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(top: 5, left: 32, right: 32, bottom: 20),
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
                  Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    child: Hero(
                      tag: 'search_bar_transition',
                      child: Material(
                        color: Colors.transparent,
                        child: Container(
                          width: 345,
                          height: 60,
                          decoration: BoxDecoration(
                            color: const Color(0xFFFFFFFF),
                            borderRadius: BorderRadius.circular(20),
                            border: hasNoResults
                                ? Border.all(color: const Color(0xFFE53935), width: 2)
                                : null,
                            boxShadow: [
                              BoxShadow(
                                color: Colors.black.withOpacity(0.05),
                                blurRadius: 10,
                                offset: const Offset(0, 4),
                              ),
                            ],
                          ),
                          child: TextField(
                            controller: _searchController,
                            textAlign: TextAlign.right,
                            readOnly: false,
                            textInputAction: TextInputAction.search,
                            onSubmitted: (value) {
                              FocusScope.of(context).unfocus();
                            },
                            onChanged: (value) {
                              if (value.isEmpty) {
                                setState(() {
                                  filteredCountries = countries;
                                  isDropdownOpen = false;
                                  hasNoResults = false;
                                });
                                return;
                              }

                              final filtered = countries
                                  .where((country) => country.contains(value))
                                  .toList();

                              setState(() {
                                filteredCountries = filtered;
                                isDropdownOpen = filtered.isNotEmpty;
                                hasNoResults = filtered.isEmpty;
                              });
                                                       },
                            onTap: () {
                              if (_searchController.text.isEmpty) {
                                setState(() {
                                  isDropdownOpen = !isDropdownOpen;
                                });
                              }
                            },
                            style: const TextStyle(
                              fontFamily: 'Cairo',
                              fontSize: 18,
                              color: Colors.black,
                            ),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 20, vertical: 15),
                              suffixIcon: Padding(
                                padding: const EdgeInsets.all(12.5),
                                child: Image.asset(
                                  'assets/icons/searchIcon.png',
                                  width: 35,
                                  height: 35,
                                  fit: BoxFit.contain,
                                ),
                              ),
                              prefixIcon: _searchController.text.isNotEmpty || isDropdownOpen
                                  ? IconButton(
                                icon: const Icon(Icons.close,
                                    color: Colors.black, size: 22),
                                onPressed: () {
                                  setState(() {
                                    _searchController.clear();
                                    filteredCountries = countries;
                                    isDropdownOpen = false;
                                    hasNoResults = false;
                                  });
                                },
                              )
                                  : null,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),

                  if (hasNoResults) ...[
                    Padding(
                      padding: const EdgeInsets.only(top: 27),
                      child: SvgPicture.asset(
                        'assets/images/errorSearch.svg',
                        width: 300,
                        height: 300,
                        fit: BoxFit.contain,
                      ),
                    ),
                    const Padding(
                      padding: EdgeInsets.only(top: 40),
                      child: SizedBox(
                        width: 331,
                        child: Text(
                          'لم يتم العثور على نتائج',
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
                  ] else ...[
                    Padding(
                      padding: const EdgeInsets.only(top: 27),
                      child: Image.asset(
                        'assets/images/search.gif',
                        width: 300,
                        height: 300,
                        fit: BoxFit.contain,
                      ),
                    ),
                  ],
                ],
              ),
              if (isDropdownOpen)
                Positioned(
                  top: 145,
                  right: 35,
                  child: Container(
                    width: 136,
                    height: 328,
                    decoration: BoxDecoration(
                      color: const Color(0xFFFFFFFF),
                      borderRadius: BorderRadius.circular(20),
                      border: Border.all(
                        color: const Color(0xFFF4A261),
                        width: 2,
                      ),
                    ),
                    child: Column(
                      children: [
                        const Padding(
                          padding: EdgeInsets.only(top: 10, bottom: 10),
                          child: Text(
                            'حدد الدولة',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20,
                              fontWeight: FontWeight.w500,
                              color: Color(0xFF000000),
                            ),
                          ),
                        ),
                        Expanded(
                          child: ListView.builder(
                            padding: const EdgeInsets.only(bottom: 10),
                            itemCount: filteredCountries.length,
                            itemBuilder: (context, index) {
                              return InkWell(
                                onTap: () {
                                  setState(() {
                                    _searchController.text = filteredCountries[index];
                                    isDropdownOpen = false;
                                    hasNoResults = false;
                                    filteredCountries = countries;
                                  });
                                },
                                child: Padding(
                                  padding: const EdgeInsets.symmetric(
                                      vertical: 10, horizontal: 12),
                                  child: Row(
                                    mainAxisAlignment: MainAxisAlignment.end,
                                    children: [
                                      Text(
                                        filteredCountries[index],
                                        style: const TextStyle(
                                          fontFamily: 'Tajawal',
                                          fontSize: 16,
                                          fontWeight: FontWeight.w400,
                                          color: Color(0xFF000000),
                                          decoration: TextDecoration.underline,
                                        ),
                                      ),
                                      const SizedBox(width: 8),
                                      Container(
                                        width: 10,
                                        height: 10,
                                        decoration: const BoxDecoration(
                                          color: Color(0xFF000000),
                                          shape: BoxShape.circle,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            },
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
    );
  }
}