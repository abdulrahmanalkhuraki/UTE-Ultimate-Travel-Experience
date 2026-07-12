import 'dart:io';
import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:image_picker/image_picker.dart';

import 'bottomNavigationBar.dart';
import 'profile_screen.dart';
import 'companions_screen.dart'; // قد لا تحتاج هذا إذا لم يكن للشركة مرافقون
import 'model/user_profile_model.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class CompanySettingsScreen extends StatefulWidget {
  final String companyName;
  final String companyEmail;
  final String companyLocation;
  final String establishmentYears;
  final String registrationNumber;

  const CompanySettingsScreen({
    Key? key,
    required this.companyName,
    required this.companyEmail,
    required this.companyLocation,
    required this.establishmentYears,
    required this.registrationNumber,
  }) : super(key: key);

  @override
  State<CompanySettingsScreen> createState() => _CompanySettingsScreenState();
}

class _CompanySettingsScreenState extends State<CompanySettingsScreen> {
  // يمكن استبدال هذا بنموذج بيانات خاص بالشركة لاحقاً
  final UserProfileModel dummyUser = UserProfileModel(
    name: 'شركة مدري شو',
    age: 20,
    gender: 'female',
    phone: '0988389494',
    email: 'kea******@gmail.com',
    currentLocation: 'سوريا_دمشق_الصناعة_كلية الهندسة المعلوماتية',
    residence: 'سوريا_دمشق_الصناعة_كلية الهندسة المعلوماتية',
    nationalId: '08784927594', // استخدمنا هذا الحقل مؤقتاً لرقم السجل
    passportNumber: '5567778',
    cardNumber: '87659876543234567',
    joinDate: '27/3/2025',
    programCount: '3',
    companiesCount: '5',
    tripsCount: '7',
    accompanierCount: '2',
    spentAmount: '500',
  );

  bool isLanguageExpanded = false;
  String selectedLanguage = 'العربية';
  bool isNotificationOn = true;

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;
    final double lOffset = isLanguageExpanded ? 90.0 : 0.0;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
      body: Directionality(
        textDirection: TextDirection.ltr,
        child: SingleChildScrollView(
          child: SizedBox(
            width: 440 * scale,
            height: (1680 + lOffset) * scale,
            child: Stack(
              clipBehavior: Clip.none,
              children: [
                Positioned(
                  top: 0,
                  left: 0,
                  right: 0,
                  child: SvgPicture.asset(
                    'assets/images/Vector.svg',
                    width: size.width,
                    fit: BoxFit.cover,
                  ),
                ),

                Positioned(
                  top: 25 * scale,
                  left: 131 * scale,
                  width: 178 * scale,
                  height: 75 * scale,
                  child: Center(
                    child: Text(
                      'الاعدادات',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Cairo',
                        fontWeight: FontWeight.w700,
                        fontSize: 40 * scale,
                        letterSpacing: 2 * scale,
                        height: 1.0,
                        color: const Color(0xFF000000),
                      ),
                    ),
                  ),
                ),

                // --- كرت الملف الشخصي للشركة ---
                Positioned(
                  top: 100 * scale,
                  left: 10 * scale,
                  width: 420 * scale,
                  height: 317 * scale,
                  child: SvgPicture.asset(
                    'assets/images/profileCard.svg',
                    fit: BoxFit.fill,
                  ),
                ),
                Positioned(
                  top: 100 * scale,
                  left: -2 * scale,
                  width: 130 * scale,
                  height: 130 * scale,
                  child: SvgPicture.asset(
                    'assets/icons/Union.svg',
                    fit: BoxFit.fill,
                  ),
                ),
                Positioned(
                  top: 188 * scale,
                  left: 2 * scale,
                  width: 49 * scale,
                  height: 35 * scale,
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(20 * scale),
                    child: BackdropFilter(
                      filter: ImageFilter.blur(sigmaX: 10.0, sigmaY: 10.0),
                      child: Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(20 * scale),
                          gradient: const LinearGradient(
                            begin: Alignment.topCenter,
                            end: Alignment.bottomCenter,
                            stops: [0.0, 0.1875, 0.5817],
                            colors: [
                              Color(0xFFF4A261),
                              Color(0x8CF4A261),
                              Color(0x1AF4A261),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 193 * scale,
                  left: 14 * scale,
                  width: 24 * scale,
                  height: 24 * scale,
                  child: SvgPicture.asset(
                    'assets/icons/Sun.svg',
                    fit: BoxFit.contain,
                  ),
                ),
                Positioned(
                  top: 121 * scale,
                  left: 309 * scale,
                  width: 100 * scale,
                  height: 100 * scale,
                  child: Container(
                    decoration: const BoxDecoration(shape: BoxShape.circle),
                    child: ClipOval(
                      child: SvgPicture.asset(
                        'assets/icons/Profile_Circle.svg',
                        fit: BoxFit.cover,
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 149 * scale,
                  left: 139 * scale,
                  width: 164 * scale,
                  height: 26 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      widget.companyName,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 22 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 170 * scale,
                  left: 151 * scale,
                  width: 140 * scale,
                  height: 40 * scale,
                  child: Text(
                    widget.companyEmail,
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 16 * scale,
                      fontWeight: FontWeight.w400,
                      color: Colors.black,
                      height: 1.2,
                    ),
                  ),
                ),
                Positioned(
                  top: 224 * scale,
                  left: 71 * scale,
                  width: 299 * scale,
                  height: 112 * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(25 * scale),
                      border: Border.all(color: Colors.black, width: 1 * scale),
                    ),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(25 * scale),
                      child: BackdropFilter(
                        filter: ImageFilter.blur(sigmaX: 10.0, sigmaY: 10.0),
                        child: Container(color: Colors.transparent),
                      ),
                    ),
                  ),
                ),
                _buildSquareArrow(scale: scale, top: 267, left: 86),

                // التأسيس
                Positioned(
                  top: 235 * scale,
                  left: 303 * scale,
                  width: 60 * scale,
                  height: 19 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'التأسيس:',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 14 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 255 * scale,
                  left: 303 * scale,
                  width: 60 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'منذ ${widget.establishmentYears}\nسنة',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 10 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.2,
                      ),
                    ),
                  ),
                ),

                // الموقع الحالي
                Positioned(
                  top: 235 * scale,
                  left: 168 * scale,
                  width: 93 * scale,
                  height: 19 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'الموقع الحالي:',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 14 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.0,
                      ),
                    ),
                  ),
                ),
                Positioned(
                  top: 255 * scale,
                  left: 140 * scale,
                  width: 150 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      widget.companyLocation,
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 10 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                        height: 1.2,
                      ),
                    ),
                  ),
                ),

                // رقم السجل
                Positioned(
                  top: 295 * scale,
                  left: 130 * scale,
                  width: 180 * scale,
                  child: Directionality(
                    textDirection: TextDirection.rtl,
                    child: Text(
                      'رقم السجل: ${widget.registrationNumber}',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontSize: 14 * scale,
                        fontWeight: FontWeight.w400,
                        color: Colors.black,
                      ),
                    ),
                  ),
                ),

                // أزرار الحذف وتسجيل الخروج
                Positioned(
                  top: 350 * scale,
                  left: 146.5 * scale,
                  width: 32 * scale,
                  height: 32 * scale,
                  child: SvgPicture.asset('assets/icons/Delete.svg'),
                ),
                Positioned(
                  top: 383 * scale,
                  left: 122 * scale,
                  width: 81 * scale,
                  height: 30 * scale,
                  child: Text(
                    'حذف حسابي',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w400,
                      fontSize: 16 * scale,
                      color: const Color(0xFFDB1518),
                    ),
                  ),
                ),
                Positioned(
                  top: 350 * scale,
                  left: 264.5 * scale,
                  width: 32 * scale,
                  height: 32 * scale,
                  child: SvgPicture.asset('assets/icons/Logout.svg'),
                ),
                Positioned(
                  top: 383 * scale,
                  left: 232 * scale,
                  width: 97 * scale,
                  height: 30 * scale,
                  child: Text(
                    'تسجيل الخروج',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontWeight: FontWeight.w400,
                      fontSize: 16 * scale,
                      color: const Color(0xFFDB1518),
                    ),
                  ),
                ),

                Positioned(
                  top: 340 * scale,
                  left: 110 * scale,
                  width: 100 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () => _showDeleteAccountDialog(context, scale),
                    child: Container(color: Colors.transparent),
                  ),
                ),
                Positioned(
                  top: 340 * scale,
                  left: 230 * scale,
                  width: 100 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () => _showLogoutDialog(context, scale),
                    child: Container(color: Colors.transparent),
                  ),
                ),

                Positioned(
                  top: 100 * scale,
                  left: 10 * scale,
                  width: 420 * scale,
                  height: 230 * scale,
                  child: GestureDetector(
                    onTap: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => ProfileScreen(user: dummyUser),
                        ),
                      );
                    },
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- قسم اللغة ---
                AnimatedPositioned(
                  duration: const Duration(milliseconds: 300),
                  curve: Curves.easeInOut,
                  top: 447 * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: (isLanguageExpanded ? 170 : 80) * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(25 * scale),
                      border: Border.all(color: Colors.black, width: 1 * scale),
                    ),
                    child: Stack(
                      children: [
                        Positioned(
                          top: 20 * scale,
                          left: 282 * scale,
                          width: 40 * scale,
                          height: 40 * scale,
                          child: SvgPicture.asset(
                            'assets/icons/laguage.svg',
                            fit: BoxFit.contain,
                          ),
                        ),
                        Positioned(
                          top: 27 * scale,
                          left: 188 * scale,
                          width: 46 * scale,
                          height: 26 * scale,
                          child: Directionality(
                            textDirection: TextDirection.rtl,
                            child: Center(
                              child: Text(
                                'اللغة',
                                textAlign: TextAlign.center,
                                style: TextStyle(
                                  fontFamily: 'Tajawal',
                                  fontWeight: FontWeight.w400,
                                  fontSize: 22 * scale,
                                  height: 1.0,
                                  color: Colors.black,
                                ),
                              ),
                            ),
                          ),
                        ),
                        Positioned(
                          top: 27 * scale,
                          left: 15 * scale,
                          width: 26 * scale,
                          height: 26 * scale,
                          child: Container(
                            decoration: BoxDecoration(
                              border: Border.all(
                                color: Colors.black,
                                width: 1.2 * scale,
                              ),
                              borderRadius: BorderRadius.circular(6 * scale),
                            ),
                            child: Center(
                              child: Icon(
                                isLanguageExpanded
                                    ? Icons.keyboard_arrow_up
                                    : Icons.keyboard_arrow_down,
                                size: 20 * scale,
                                color: Colors.black,
                              ),
                            ),
                          ),
                        ),
                        if (isLanguageExpanded) ...[
                          Positioned(
                            top: 78 * scale,
                            left: 0,
                            right: 0,
                            height: 40 * scale,
                            child: GestureDetector(
                              onTap: () => setState(() {
                                selectedLanguage = 'العربية';
                                isLanguageExpanded = false;
                              }),
                              child: Container(
                                color: Colors.transparent,
                                child: Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  children: [
                                    SizedBox(
                                      width: 90 * scale,
                                      child: Text(
                                        'العربية',
                                        textAlign: TextAlign.right,
                                        style: TextStyle(
                                          fontFamily: 'Tajawal',
                                          fontSize: 22 * scale,
                                          fontWeight: FontWeight.w400,
                                          color: Colors.black,
                                        ),
                                      ),
                                    ),
                                    SizedBox(width: 26 * scale),
                                    Container(
                                      width: 20 * scale,
                                      height: 20 * scale,
                                      decoration: BoxDecoration(
                                        shape: BoxShape.circle,
                                        color: selectedLanguage == 'العربية'
                                            ? const Color(0xFFF4A261)
                                            : Colors.transparent,
                                        border: Border.all(color: Colors.black12),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                          Positioned(
                            top: 124 * scale,
                            left: 0,
                            right: 0,
                            height: 40 * scale,
                            child: GestureDetector(
                              onTap: () => setState(() {
                                selectedLanguage = 'الإنكليزية';
                                isLanguageExpanded = false;
                              }),
                              child: Container(
                                color: Colors.transparent,
                                child: Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  children: [
                                    SizedBox(
                                      width: 90 * scale,
                                      child: Text(
                                        'الإنكليزية',
                                        textAlign: TextAlign.right,
                                        style: TextStyle(
                                          fontFamily: 'Tajawal',
                                          fontSize: 22 * scale,
                                          fontWeight: FontWeight.w400,
                                          color: Colors.black,
                                        ),
                                      ),
                                    ),
                                    SizedBox(width: 26 * scale),
                                    Container(
                                      width: 20 * scale,
                                      height: 20 * scale,
                                      decoration: BoxDecoration(
                                        shape: BoxShape.circle,
                                        color: selectedLanguage == 'الإنكليزية'
                                            ? const Color(0xFFF4A261)
                                            : Colors.transparent,
                                        border: Border.all(color: Colors.black12),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                        ],
                        Positioned(
                          top: 0,
                          left: 0,
                          width: 362 * scale,
                          height: 80 * scale,
                          child: GestureDetector(
                            onTap: () => setState(
                                  () => isLanguageExpanded = !isLanguageExpanded,
                            ),
                            child: Container(color: Colors.transparent),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),

                // --- العملة ---
                _buildListContainer(
                  scale: scale,
                  top: 542,
                  offset: lOffset,
                  left: 241,
                  width: 160,
                  height: 80,
                  hasShadow: true,
                ),
                _buildListContainer(
                  scale: scale,
                  top: 542,
                  offset: lOffset,
                  left: 39,
                  width: 160,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 562,
                  offset: lOffset,
                  left: 341,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/coin.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 572,
                  offset: lOffset,
                  left: 271,
                  width: 60,
                  height: 26,
                  text: 'العملة',
                  fontSize: 22,
                ),
                _buildText(
                  scale: scale,
                  top: 569,
                  offset: lOffset,
                  left: 104,
                  width: 70,
                  height: 26,
                  text: 'ل.س.ج',
                  fontSize: 22,
                ),
                _buildSquareArrow(
                  scale: scale,
                  top: 569,
                  offset: lOffset,
                  left: 54,
                ),
                Positioned(
                  top: (542 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () {},
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- تفعيل اشعارات التطبيق ---
                _buildListContainer(
                  scale: scale,
                  top: 637,
                  offset: lOffset,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildText(
                  scale: scale,
                  top: 663,
                  offset: lOffset,
                  left: 131,
                  width: 254,
                  height: 26,
                  text: 'تفعيل اشعارات التطبيق:',
                  fontSize: 22,
                ),
                AnimatedPositioned(
                  duration: const Duration(milliseconds: 200),
                  top: (659 + lOffset) * scale,
                  left: 59 * scale,
                  width: 68 * scale,
                  height: 35 * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      color: isNotificationOn
                          ? const Color(0xFF91B3FA)
                          : Colors.grey[400],
                      borderRadius: BorderRadius.circular(17.5 * scale),
                    ),
                  ),
                ),
                AnimatedPositioned(
                  duration: const Duration(milliseconds: 200),
                  curve: Curves.easeInOut,
                  top: (662.5 + lOffset) * scale,
                  left: (isNotificationOn ? 95 : 63) * scale,
                  width: 28 * scale,
                  height: 28 * scale,
                  child: Container(
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      shape: BoxShape.circle,
                    ),
                  ),
                ),
                AnimatedPositioned(
                  duration: const Duration(milliseconds: 200),
                  curve: Curves.easeInOut,
                  top: (664 + lOffset) * scale,
                  left: (isNotificationOn ? 97 : 65) * scale,
                  width: 24 * scale,
                  height: 24 * scale,
                  child: SvgPicture.asset('assets/icons/SMS.svg'),
                ),
                Positioned(
                  top: (637 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () =>
                        setState(() => isNotificationOn = !isNotificationOn),
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- المرشدون السياحيون ---
                _buildListContainer(
                  scale: scale,
                  top: 732,
                  offset: lOffset,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 752,
                  offset: lOffset,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/tour_guide.png', // 🌟 تم تعديل المسار ليعمل بشكل سليم
                ),
                _buildText(
                  scale: scale,
                  top: 759,
                  offset: lOffset,
                  left: 141,
                  width: 170,
                  height: 26,
                  text: 'المرشدون السياحيون',
                  fontSize: 22,
                ),
                _buildSquareArrow(
                  scale: scale,
                  top: 759,
                  offset: lOffset,
                  left: 54,
                ),
                Positioned(
                  top: (732 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () {
                      // Navigate to tour guides screen
                    },
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- الإبلاغ عن سائح ---
                _buildListContainer(
                  scale: scale,
                  top: 827,
                  offset: lOffset,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 847,
                  offset: lOffset,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/Profile_1.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 854,
                  offset: lOffset,
                  left: 162,
                  width: 140,
                  height: 26,
                  text: 'الإبلاغ عن سائح',
                  fontSize: 22,
                ),
                _buildSquareArrow(
                  scale: scale,
                  top: 854,
                  offset: lOffset,
                  left: 54,
                ),
                Positioned(
                  top: (827 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () => _showReportTouristBottomSheet(context, scale),
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- تقييم UTE ---
                _buildListContainer(
                  scale: scale,
                  top: 922,
                  offset: lOffset,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                _buildIcon(
                  scale: scale,
                  top: 942,
                  offset: lOffset,
                  left: 321,
                  width: 40,
                  height: 40,
                  assetPath: 'assets/icons/Like.svg',
                ),
                _buildText(
                  scale: scale,
                  top: 949,
                  offset: lOffset,
                  left: 190,
                  width: 120,
                  height: 26,
                  text: 'تقييم UTE',
                  fontSize: 22,
                ),
                _buildSquareArrow(
                  scale: scale,
                  top: 949,
                  offset: lOffset,
                  left: 54,
                ),
                Positioned(
                  top: (922 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () {},
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // --- تواصل مع فريق الدعم ---
                _buildListContainer(
                  scale: scale,
                  top: 1017,
                  offset: lOffset,
                  left: 39,
                  width: 362,
                  height: 80,
                ),
                Positioned(
                  top: (1037 + lOffset) * scale,
                  left: 321 * scale,
                  width: 40 * scale,
                  height: 40 * scale,
                  child: Image.asset(
                    'assets/icons/support.png',
                    fit: BoxFit.contain,
                  ),
                ),
                _buildText(
                  scale: scale,
                  top: 1044,
                  offset: lOffset,
                  left: 117,
                  width: 197,
                  height: 26,
                  text: 'تواصل مع فريق الدعم',
                  fontSize: 22,
                ),
                _buildSquareArrow(
                  scale: scale,
                  top: 1044,
                  offset: lOffset,
                  left: 54,
                ),
                Positioned(
                  top: (1017 + lOffset) * scale,
                  left: 39 * scale,
                  width: 362 * scale,
                  height: 80 * scale,
                  child: GestureDetector(
                    onTap: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const SupportTeam(),
                        ),
                      );
                    },
                    child: Container(color: Colors.transparent),
                  ),
                ),

                // زر الرجوع في الأعلى
                Positioned(
                  top: 46 * scale,
                  left: 373 * scale,
                  width: 35 * scale,
                  height: 35 * scale,
                  child: GestureDetector(
                    onTap: () => Navigator.pop(context),
                    child: Container(
                      color: Colors.transparent,
                      child: Icon(
                        Icons.keyboard_arrow_right,
                        size: 35 * scale,
                        color: Colors.black,
                      ),
                    ),
                  ),
                ),
                Positioned(
                  bottom: 0,
                  left: 0,
                  right: 0,
                  child: const AppBottomNavBar(selectedIndex: 4),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  // --- النوافذ المنبثقة للتحذيرات المخصصة ---

  void _showLogoutDialog(BuildContext context, double scale) {
    _showActionDialog(
      context: context,
      scale: scale,
      title: 'تسجيل الخروج',
      content: 'هل أنت متأكد أنك تريد تسجيل الخروج من حسابك؟',
      confirmText: 'تسجيل الخروج',
      confirmColor: const Color(0xFFDB6262),
      iconData: Icons.logout_rounded,
      onConfirm: () {
        print("تم تسجيل الخروج");
      },
    );
  }

  void _showDeleteAccountDialog(BuildContext context, double scale) {
    _showActionDialog(
      context: context,
      scale: scale,
      title: 'حذف الحساب',
      content: 'هل أنت متأكد أنك تريد حذف حسابك نهائياً؟\nسيتم مسح جميع بياناتك ولا يمكن التراجع عن هذا الإجراء.',
      confirmText: 'حذف الحساب',
      confirmColor: const Color(0xFFDB6262),
      iconData: Icons.delete_outline_rounded,
      onConfirm: () {
        print("تم حذف الحساب");
      },
    );
  }

  void _showActionDialog({
    required BuildContext context,
    required double scale,
    required String title,
    required String content,
    required String confirmText,
    required Color confirmColor,
    required IconData iconData,
    required VoidCallback onConfirm,
  }) {
    showDialog(
      context: context,
      builder: (context) {
        return Dialog(
          backgroundColor: Colors.transparent,
          elevation: 0,
          child: Container(
            width: 380 * scale,
            padding: EdgeInsets.all(24 * scale),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(25 * scale),
              border: Border.all(color: Colors.black, width: 1.5 * scale),
              boxShadow: const [
                BoxShadow(
                  color: Color(0x33000000),
                  blurRadius: 20,
                  offset: Offset(0, 10),
                ),
              ],
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 70 * scale,
                  height: 70 * scale,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    gradient: LinearGradient(
                      begin: Alignment.topCenter,
                      end: Alignment.bottomCenter,
                      stops: const [0.0, 0.0433, 0.1731, 1.0],
                      colors: [
                        confirmColor,
                        confirmColor.withOpacity(0.55),
                        confirmColor.withOpacity(0.10),
                        confirmColor.withOpacity(0.10),
                      ],
                    ),
                  ),
                  child: Center(
                    child: Icon(
                      iconData,
                      color: confirmColor,
                      size: 35 * scale,
                    ),
                  ),
                ),
                SizedBox(height: 16 * scale),
                Text(
                  title,
                  style: TextStyle(
                    fontFamily: 'Cairo',
                    fontSize: 26 * scale,
                    fontWeight: FontWeight.w700,
                    color: Colors.black,
                  ),
                ),
                SizedBox(height: 12 * scale),
                Text(
                  content,
                  textAlign: TextAlign.center,
                  textDirection: TextDirection.rtl,
                  style: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 18 * scale,
                    fontWeight: FontWeight.w400,
                    color: Colors.black87,
                    height: 1.4,
                  ),
                ),
                SizedBox(height: 24 * scale),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    GestureDetector(
                      onTap: () => Navigator.pop(context),
                      child: Container(
                        width: 120 * scale,
                        height: 50 * scale,
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(15 * scale),
                          border: Border.all(color: Colors.black, width: 1.2 * scale),
                        ),
                        child: Center(
                          child: Text(
                            'إلغاء',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20 * scale,
                              fontWeight: FontWeight.w500,
                              color: Colors.black,
                            ),
                          ),
                        ),
                      ),
                    ),
                    GestureDetector(
                      onTap: () {
                        onConfirm();
                        Navigator.pop(context);
                      },
                      child: Container(
                        width: 130 * scale,
                        height: 50 * scale,
                        decoration: BoxDecoration(
                          color: confirmColor,
                          borderRadius: BorderRadius.circular(15 * scale),
                        ),
                        child: Center(
                          child: Text(
                            confirmText,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 18 * scale,
                              fontWeight: FontWeight.w500,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  // --- نافذة الإبلاغ عن سائح ---
  void _showReportTouristBottomSheet(BuildContext context, double scale) {
    final TextEditingController nameController = TextEditingController();
    final TextEditingController reasonController = TextEditingController();

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) {
        return StatefulBuilder(
          builder: (BuildContext context, StateSetter setState) {
            return Padding(
              padding: EdgeInsets.only(
                bottom: MediaQuery.of(context).viewInsets.bottom,
              ),
              child: Container(
                width: 440 * scale,
                height: 536 * scale,
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.only(
                    topLeft: Radius.circular(50 * scale),
                    topRight: Radius.circular(50 * scale),
                  ),
                  boxShadow: const [
                    BoxShadow(
                      color: Color(0x40000000),
                      offset: Offset(0, -4),
                      blurRadius: 50,
                      spreadRadius: 10,
                    ),
                  ],
                ),
                child: Stack(
                  clipBehavior: Clip.none,
                  children: [
                    Positioned(
                      top: 8 * scale,
                      left: 153 * scale,
                      width: 134 * scale,
                      height: 5.3 * scale,
                      child: Container(
                        decoration: BoxDecoration(
                          color: const Color(0xFFF4A261),
                          borderRadius: BorderRadius.circular(100),
                        ),
                      ),
                    ),
                    Positioned(
                      top: 28 * scale,
                      left: 0,
                      right: 0,
                      child: Text(
                        'الإبلاغ عن سائح',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                        ),
                      ),
                    ),

                    Positioned(
                      top: 91 * scale,
                      right: 50 * scale,
                      child: Text(
                        'اسم السائح:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 24 * scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                        ),
                      ),
                    ),

                    Positioned(
                      top: 130 * scale,
                      left: 50 * scale,
                      width: 340 * scale,
                      height: 75 * scale,
                      child: Container(
                        decoration: BoxDecoration(
                          border: Border.all(
                            color: Colors.black,
                            width: 2 * scale,
                          ),
                          borderRadius: BorderRadius.circular(15 * scale),
                        ),
                        child: Stack(
                          children: [
                            Positioned(
                              right: 15 * scale,
                              top: 22 * scale,
                              child: SvgPicture.asset(
                                'assets/icons/Profile_1.svg',
                                width: 30 * scale,
                                height: 30 * scale,
                              ),
                            ),
                            Positioned(
                              left: 15 * scale,
                              right: 60 * scale,
                              top: 0,
                              bottom: 0,
                              child: Center(
                                child: TextField(
                                  controller: nameController,
                                  textAlign: TextAlign.right,
                                  textDirection: TextDirection.rtl,
                                  decoration: const InputDecoration(
                                    border: InputBorder.none,
                                  ),
                                  style: TextStyle(
                                    fontFamily: 'Tajawal',
                                    fontSize: 20 * scale,
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),

                    Positioned(
                      top: 220 * scale,
                      left: 50 * scale,
                      width: 340 * scale,
                      height: 180 * scale,
                      child: Stack(
                        children: [
                          Positioned.fill(
                            child: Image.asset(
                              'assets/icons/reason square.png',
                              fit: BoxFit.fill,
                            ),
                          ),
                          Positioned(
                            top: 35 * scale,
                            bottom: 10 * scale,
                            left: 15 * scale,
                            right: 15 * scale,
                            child: TextField(
                              controller: reasonController,
                              maxLines: null,
                              expands: true,
                              textAlign: TextAlign.right,
                              textDirection: TextDirection.rtl,
                              decoration: InputDecoration(
                                hintText:
                                'اشرح سبب الإبلاغ وشو ساوالك المخلوق واذا كنت عم تتغالظ عليه رح نحظرك الك فلا توجعلنا راسنا بغلاظتك الله يرضى عليك',
                                hintMaxLines: 4,
                                hintStyle: TextStyle(
                                  fontFamily: 'Tajawal',
                                  fontSize: 16 * scale,
                                  color: const Color(0x80000000),
                                ),
                                border: InputBorder.none,
                              ),
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 16 * scale,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),

                    Positioned(
                      top: 420 * scale,
                      left: 85 * scale,
                      width: 270 * scale,
                      height: 65 * scale,
                      child: GestureDetector(
                        onTap: () {
                          print(
                            "Reporting Tourist: ${nameController.text}, Reason: ${reasonController.text}",
                          );
                          Navigator.pop(context);
                        },
                        child: Container(
                          decoration: BoxDecoration(
                            color: const Color(0xFFF4A261),
                            borderRadius: BorderRadius.circular(20 * scale),
                          ),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Text(
                                'إرسال',
                                textDirection: TextDirection.rtl,
                                style: TextStyle(
                                  fontFamily: 'Tajawal',
                                  fontSize: 36 * scale,
                                  fontWeight: FontWeight.w400,
                                  color: Colors.black,
                                ),
                              ),
                              SizedBox(width: 8 * scale),
                              Image.asset(
                                'assets/icons/SendRequest.png',
                                width: 40 * scale,
                                height: 40 * scale,
                                fit: BoxFit.contain,
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  // --- نافذة إعادة تعيين كلمة المرور (احتفظنا بها فقط تحسباً لو احتجتها لاحقاً، رغم إزالتها من القائمة) ---
  void _showResetPasswordBottomSheet(BuildContext context, double scale) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) {
        return Padding(
          padding: EdgeInsets.only(
            bottom: MediaQuery.of(context).viewInsets.bottom,
          ),
          child: Container(
            width: 440 * scale,
            height: 473 * scale,
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.only(
                topLeft: Radius.circular(50 * scale),
                topRight: Radius.circular(50 * scale),
              ),
            ),
            child: Stack(
              children: [
                Positioned(
                  top: 8 * scale,
                  left: 153 * scale,
                  width: 134 * scale,
                  height: 5.3 * scale,
                  child: Container(
                    decoration: BoxDecoration(
                      color: const Color(0xFFF4A261),
                      borderRadius: BorderRadius.circular(100),
                    ),
                  ),
                ),
                Positioned(
                  top: 28 * scale,
                  left: 0,
                  right: 0,
                  child: Text(
                    'إعادة تعيين كلمة المرور',
                    textAlign: TextAlign.center,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 32 * scale,
                      fontWeight: FontWeight.w400,
                    ),
                  ),
                ),
                _buildPassField(scale, 91, 'كلمة المرور الحالية'),
                _buildPassField(scale, 186, 'كلمة المرور الجديدة'),
                _buildPassField(scale, 281, 'تأكيد كلمة المرور'),
                Positioned(
                  top: 376 * scale,
                  left: 85 * scale,
                  width: 270 * scale,
                  height: 65 * scale,
                  child: GestureDetector(
                    onTap: () => Navigator.pop(context),
                    child: Container(
                      decoration: BoxDecoration(
                        color: const Color(0xFFF4A261),
                        borderRadius: BorderRadius.circular(20 * scale),
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Text(
                            'إعادة تعيين',
                            textDirection: TextDirection.rtl,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 36 * scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black,
                            ),
                          ),
                          SizedBox(width: 10 * scale),
                          SvgPicture.asset(
                            'assets/icons/Refresh.svg',
                            width: 33 * scale,
                            height: 36 * scale,
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildPassField(double scale, double top, String hint) {
    return Positioned(
      top: top * scale,
      left: 50 * scale,
      width: 340 * scale,
      height: 75 * scale,
      child: Container(
        decoration: BoxDecoration(
          border: Border.all(color: Colors.black, width: 1 * scale),
          borderRadius: BorderRadius.circular(15 * scale),
        ),
        child: Stack(
          children: [
            Positioned(
              right: 15 * scale,
              top: 25 * scale,
              child: SvgPicture.asset(
                'assets/icons/Lock.svg',
                width: 22.5 * scale,
                height: 25 * scale,
              ),
            ),
            Positioned(
              left: 15 * scale,
              top: 22 * scale,
              child: SvgPicture.asset(
                'assets/icons/Hide.svg',
                width: 30 * scale,
                height: 30 * scale,
              ),
            ),
            Positioned(
              left: 60 * scale,
              right: 50 * scale,
              top: 0,
              bottom: 0,
              child: Center(
                child: TextField(
                  textAlign: TextAlign.right,
                  textDirection: TextDirection.rtl,
                  obscureText: true,
                  decoration: InputDecoration(
                    hintText: hint,
                    hintStyle: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 24 * scale,
                      color: Colors.black,
                    ),
                    border: InputBorder.none,
                  ),
                  style: TextStyle(fontFamily: 'Tajawal', fontSize: 24 * scale),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildListContainer({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    double offset = 0.0,
    bool hasShadow = false,
  }) {
    return AnimatedPositioned(
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
      top: (top + offset) * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(25 * scale),
          border: Border.all(color: Colors.black, width: 1 * scale),
          boxShadow: hasShadow
              ? [
            BoxShadow(
              color: const Color(0x40000000),
              offset: Offset(0, 4 * scale),
              blurRadius: 25 * scale,
              spreadRadius: 4 * scale,
            ),
          ]
              : null,
        ),
      ),
    );
  }

  Widget _buildText({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    double offset = 0.0,
    required String text,
    required double fontSize,
  }) {
    return AnimatedPositioned(
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
      top: (top + offset) * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: Directionality(
        textDirection: TextDirection.rtl,
        child: Center(
          child: Text(
            text,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w400,
              fontSize: fontSize * scale,
              height: 1.0,
              color: Colors.black,
            ),
          ),
        ),
      ),
    );
  }

  // 🌟 التعديل هنا: الدالة الآن تقبل الصور بصيغة png أو svg
  Widget _buildIcon({
    required double scale,
    required double top,
    required double left,
    required double width,
    required double height,
    double offset = 0.0,
    required String assetPath,
  }) {
    bool isSvg = assetPath.toLowerCase().endsWith('.svg');

    return AnimatedPositioned(
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
      top: (top + offset) * scale,
      left: left * scale,
      width: width * scale,
      height: height * scale,
      child: isSvg
          ? SvgPicture.asset(assetPath, fit: BoxFit.contain)
          : Image.asset(assetPath, fit: BoxFit.contain),
    );
  }

  Widget _buildSquareArrow({
    required double scale,
    required double top,
    required double left,
    double offset = 0.0,
  }) {
    return AnimatedPositioned(
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
      top: (top + offset) * scale,
      left: left * scale,
      width: 26 * scale,
      height: 26 * scale,
      child: Container(
        decoration: BoxDecoration(
          border: Border.all(color: Colors.black, width: 1.2 * scale),
          borderRadius: BorderRadius.circular(6 * scale),
        ),
        child: Center(
          child: Icon(
            Icons.keyboard_arrow_left,
            size: 20 * scale,
            color: Colors.black,
          ),
        ),
      ),
    );
  }
}

// -------------------------------------------------------------------------
// كود فريق الدعم (SupportTeam)
// -------------------------------------------------------------------------

class SupportTeam extends StatefulWidget {
  const SupportTeam({super.key});

  @override
  State<SupportTeam> createState() => _SupportTeamState();
}

class _SupportTeamState extends State<SupportTeam> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();

  File? _supportImage;
  final ImagePicker _picker = ImagePicker();

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final XFile? pickedFile = await _picker.pickImage(
      source: ImageSource.gallery,
    );
    if (pickedFile != null) {
      setState(() {
        _supportImage = File(pickedFile.path);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final double scale = MediaQuery.of(context).size.width / 440;

    return Scaffold(
      body: SafeArea(
        child: Stack(
          children: [
            Positioned(
              top: 0,
              left: 0,
              right: 0,
              child: SvgPicture.asset(
                'assets/images/Vector.svg',
                fit: BoxFit.cover,
              ),
            ),
            SingleChildScrollView(
              child: Column(
                children: [
                  Padding(
                    padding: EdgeInsets.only(
                      top: 42 * scale,
                      left: 20 * scale,
                      right: 20 * scale,
                    ),
                    child: Row(
                      children: [
                        SizedBox(width: 48 * scale),
                        Expanded(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: Text(
                              'فريق الدعم',
                              style: TextStyle(
                                fontFamily: 'Cairo',
                                fontSize: 36 * scale,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ),
                        SizedBox(
                          width: 48 * scale,
                          child: GestureDetector(
                            onTap: () => Navigator.pop(context),
                            child: Icon(
                              Icons.keyboard_arrow_right,
                              size: 35 * scale,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  Padding(
                    padding: EdgeInsets.only(
                      top: 20 * scale,
                      left: 32 * scale,
                      right: 32 * scale,
                      bottom: 20 * scale,
                    ),
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
                  SizedBox(height: 20 * scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'عنوان المشكلة:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 8 * scale),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * scale),
                    child: SizedBox(
                      height: 60 * scale,
                      child: Stack(
                        children: [
                          SvgPicture.asset(
                            'assets/icons/Rectangle1111.svg',
                            width: double.infinity,
                            height: 60 * scale,
                            fit: BoxFit.fill,
                          ),
                          TextField(
                            controller: _titleController,
                            textAlign: TextAlign.right,
                            textDirection: TextDirection.rtl,
                            textAlignVertical: TextAlignVertical.center,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 16 * scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black87,
                            ),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(
                                horizontal: 16 * scale,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  SizedBox(height: 16 * scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'صف مشكلتك:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 8 * scale),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * scale),
                    child: SizedBox(
                      height: 112 * scale,
                      child: Stack(
                        children: [
                          SvgPicture.asset(
                            'assets/icons/Rectangle11114.svg',
                            width: double.infinity,
                            height: 112 * scale,
                            fit: BoxFit.fill,
                          ),
                          TextField(
                            controller: _descriptionController,
                            textAlign: TextAlign.right,
                            textDirection: TextDirection.rtl,
                            maxLines: null,
                            expands: true,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 16 * scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black87,
                            ),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(
                                horizontal: 16 * scale,
                                vertical: 12 * scale,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  SizedBox(height: 16 * scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'يمكنك أيضاً إرفاق صورة عن المشكلة:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 12 * scale),

                  // 🌟 إطار الصورة التفاعلي
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: GestureDetector(
                        onTap: _pickImage,
                        child: SizedBox(
                          width: 340 * scale,
                          height: 259 * scale,
                          child: Stack(
                            alignment: Alignment.center,
                            children: [
                              SvgPicture.asset(
                                'assets/icons/picFrame.svg',
                                width: 340 * scale,
                                height: 259 * scale,
                                fit: BoxFit.fill,
                              ),
                              _supportImage != null
                                  ? ClipRRect(
                                borderRadius: BorderRadius.circular(
                                  15 * scale,
                                ),
                                child: Image.file(
                                  _supportImage!,
                                  width: 320 * scale,
                                  height: 240 * scale,
                                  fit: BoxFit.cover,
                                ),
                              )
                                  : SvgPicture.asset(
                                'assets/icons/addpic.svg',
                                width: 104 * scale,
                                height: 103 * scale,
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 20 * scale),
                  Center(
                    child: GestureDetector(
                      onTap: () {
                        print("Team Support Ticket: ${_titleController.text}");
                      },
                      child: Container(
                        width: 270 * scale,
                        height: 65 * scale,
                        decoration: BoxDecoration(
                          color: const Color(0xFFF4A261),
                          borderRadius: BorderRadius.circular(20 * scale),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Text(
                              'إرسال الطلب',
                              textDirection: TextDirection.rtl,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 36 * scale,
                                fontWeight: FontWeight.w400,
                                color: Colors.black,
                                height: 1.0,
                              ),
                            ),
                            SizedBox(width: 8 * scale),
                            Image.asset(
                              'assets/icons/SendRequest.png',
                              width: 40 * scale,
                              height: 40 * scale,
                              fit: BoxFit.contain,
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 30 * scale),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}