import 'dart:ui';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:flutter/material.dart';
import 'model/user_profile_model.dart';
import 'bottomNavigationBar.dart';

class ProfileScreen extends StatefulWidget {
  final UserProfileModel user;

  const ProfileScreen({super.key, required this.user});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
int _idCurrentPage = 0;
int _passportCurrentPage = 0;
final PageController _idPageController = PageController();
final PageController _passportPageController = PageController();

@override
void dispose() {
_idPageController.dispose();
_passportPageController.dispose();
super.dispose();
}

@override
Widget build(BuildContext context) {
final size = MediaQuery.of(context).size;

return Scaffold(
backgroundColor: const Color(0xFFFFFFFF),
body: Stack(
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

Positioned.fill(
child: SingleChildScrollView(
padding: const EdgeInsets.only(top: 45, bottom: 120),
child: Column(
children: [

  Padding(
padding: const EdgeInsets.symmetric(horizontal: 20),
child: Row(
mainAxisAlignment: MainAxisAlignment.spaceBetween,
children: [
const SizedBox(width: 35),
const Text(
'الملف الشخصي',
textAlign: TextAlign.center,
style: TextStyle(
fontFamily: 'Cairo',
fontWeight: FontWeight.w700,
fontSize: 36,
letterSpacing: 2,
height: 1.0,
color: Color(0xFF000000),
),
),
GestureDetector(
onTap: () => Navigator.pop(context),
child: const Icon(
Icons.keyboard_arrow_right,
size: 35,
color: Colors.black,
),
),
],
),
),
const SizedBox(height: 20),

ClipOval(
child: SvgPicture.asset(
'assets/icons/Profile_Circle.svg',
width: 200,
height: 200,
fit: BoxFit.cover,
),
),
const SizedBox(height: 16),
Padding(
padding: const EdgeInsets.symmetric(horizontal: 16),
child: Directionality(
textDirection: TextDirection.rtl,
child: Row(
children: [
Expanded(
child: SizedBox(
height: 128,
child: Stack(
children: [
SvgPicture.asset(
'assets/images/card.svg',
fit: BoxFit.fill,
width: double.infinity,
height: 128,
),
Center(
child: Column(
mainAxisAlignment: MainAxisAlignment.center,
children: [
SvgPicture.asset(
'assets/icons/Profile _1.svg',
width: 30,
height: 30,
),
const SizedBox(height: 8),
Padding(
padding: const EdgeInsets.symmetric(horizontal: 8),
child: FittedBox(
fit: BoxFit.scaleDown,
child: Text(
widget.user.name,
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 20,
fontWeight: FontWeight.w400,
),
),
),
),
const SizedBox(height: 4),
Row(
mainAxisAlignment: MainAxisAlignment.center,
crossAxisAlignment: CrossAxisAlignment.center,
children: [
Transform.translate(
offset: const Offset(0, -2),
child: SizedBox(
width: 35,
height: 35,
child: Image.asset(
widget.user.genderIconPath,
fit: BoxFit.contain,
),
),
),
const SizedBox(width: 10),
Flexible(
child: FittedBox(
fit: BoxFit.scaleDown,
child: Text(
'${widget.user.age} سنة',
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 20,
fontWeight: FontWeight.w400,
height: 1,
),
),
),
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
const SizedBox(width: 10),
Expanded(
child: SizedBox(
height: 128,
child: Stack(
children: [
SvgPicture.asset(
'assets/images/card.svg',
fit: BoxFit.fill,
width: double.infinity,
height: 128,
),
Center(
child: Column(
mainAxisAlignment: MainAxisAlignment.center,
children: [
SvgPicture.asset('assets/icons/Call.svg', width: 30, height: 30),
const SizedBox(height: 8),
Padding(
padding: const EdgeInsets.symmetric(horizontal: 8),
child: FittedBox(
fit: BoxFit.scaleDown,
child: Text(
widget.user.phone,
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 20,
fontWeight: FontWeight.w400,
),
),
),
),
const SizedBox(height: 4),
Padding(
padding: const EdgeInsets.symmetric(horizontal: 8),
child: FittedBox(
fit: BoxFit.scaleDown,
child: Text(
widget.user.email,
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 20,
fontWeight: FontWeight.w400,
),
),
),
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
),
const SizedBox(height: 10),
Padding(
padding: const EdgeInsets.symmetric(horizontal: 16),
child: SizedBox(
width: double.infinity,
height: 128,
child: Stack(
children: [
SvgPicture.asset(
'assets/icons/Rectangular card.svg',
fit: BoxFit.fill,
width: double.infinity,
),
Center(
child: Column(
mainAxisAlignment: MainAxisAlignment.center,
children: [
SvgPicture.asset(
'assets/icons/Location_Add.svg',
width: 30,
height: 30,
),
const SizedBox(height: 8),
Text(
'السكن الحالي:    ${widget.user.currentLocation}',
style: const TextStyle(fontFamily: 'Tajawal', fontSize: 16),
textAlign: TextAlign.center,
),
const SizedBox(height: 4),
Text(
'مكان الإقامة:    ${widget.user.residence}',
style: const TextStyle(fontFamily: 'Tajawal', fontSize: 16),
textAlign: TextAlign.center,
),
],
),
),
],
),
),
),
const SizedBox(height: 10),

Padding(
padding: const EdgeInsets.symmetric(horizontal: 16),
child: SizedBox(
width: 400,
height: 128,
child: Stack(
children: [
SvgPicture.asset(
'assets/icons/Rectangular card2.svg',
fit: BoxFit.fill,
width: 400,
height: 128,
),
Positioned(
right: 20,
top: 0,
bottom: 0,
width: 100,
child: Padding(
padding: const EdgeInsets.only(top: 10),
child: Column(
mainAxisAlignment: MainAxisAlignment.start,
children: [
SvgPicture.asset(
'assets/icons/NID.svg',
width: 30,
height: 30,
),
const SizedBox(height: 8),
FittedBox(
fit: BoxFit.scaleDown,
alignment: Alignment.center,
child: Text(
widget.user.nationalId,
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 14,
),
),
),
],
),
),
),
Positioned(
left: 20,
top: 0,
bottom: 0,
width: 100,
child: Padding(
padding: const EdgeInsets.only(top: 10),
child: Column(
children: [
Image.asset(
'assets/icons/passport.png',
width: 30,
height: 30,
),
const SizedBox(height: 8),
FittedBox(
fit: BoxFit.scaleDown,
child: Text(
widget.user.passportNumber,
style: const TextStyle(fontFamily: 'Tajawal', fontSize: 14),
),
),
],
),
),
),
Positioned(
left: 20,
right: 20,
bottom: 10,
height: 40,
child: Row(
mainAxisAlignment: MainAxisAlignment.center,
children: [
FittedBox(
fit: BoxFit.scaleDown,
child: Text(
widget.user.cardNumber,
style: const TextStyle(
fontFamily: 'Tajawal',
fontSize: 14,
),
),
),
const SizedBox(width: 12),
SvgPicture.asset(
'assets/icons/Card.svg',
width: 30,
height: 30,
),
],
),
),
],
),
),
),
const SizedBox(height: 10),
Padding(
padding: const EdgeInsets.all(16.0),
child: Stack(
children: [
Positioned.fill(
child: SvgPicture.asset(
'assets/icons/SquareCard.svg',
fit: BoxFit.fill,
),
),
Padding(
padding: const EdgeInsets.only(top: 80, right: 20, left: 20, bottom: 20),
child: Column(
mainAxisSize: MainAxisSize.min,
children: [
_buildDataRow(" تاريخ الانضمام :", widget.user.joinDate),
_buildDataRow(" عدد البرامج التي انضممت لها :", widget.user.programCount),
_buildDataRow(" عدد الشركات التي سافرت معها :", widget.user.companiesCount),
_buildDataRow(" عدد الرحلات التي قمت بها :", widget.user.tripsCount),
_buildDataRow(" عدد المرافقين :", widget.user.accompanierCount),
_buildDataRow(
" المبلغ المنفق خلال رحلاتك وبرامجك :",
widget.user.spentAmount,
isCurrency: true,
),
],
),
),
Positioned(
top: 20,
right: 40,
  left: 40,
  child: SvgPicture.asset('assets/icons/Chart.svg', width: 40, height: 40),
),
],
),
),
  _buildImageSlider(
    title: 'صورة الهوية الشخصية:',
    frontImage: widget.user.idImageFront,
    backImage: widget.user.idImageBack,
    controller: _idPageController,
    currentPage: _idCurrentPage,
    onPageChanged: (index) => setState(() => _idCurrentPage = index),
  ),
  const SizedBox(height: 10),
  _buildImageSlider(
    title: 'صورة جواز السفر:',
    frontImage: widget.user.passportImageFront,
    backImage: widget.user.passportImageBack,
    controller: _passportPageController,
    currentPage: _passportCurrentPage,
    onPageChanged: (index) => setState(() => _passportCurrentPage = index),
  ),
  const SizedBox(height: 16),
  GestureDetector(
    onTap: () {





      // تعديل
    },
    child: Container(
      width: 222,
      height: 60,
      decoration: BoxDecoration(
        color: const Color(0xFFF4A261),
        borderRadius: BorderRadius.circular(15),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const SizedBox(
            width: 113,
            height: 38,
            child: Text(
              'تعديل',
              textAlign: TextAlign.center,
              textDirection: TextDirection.rtl,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 32,
                fontWeight: FontWeight.w500,
                height: 1.0,
                color: Color(0xFFFFFFFF),
              ),
            ),
          ),
          const SizedBox(width: 8),
          Image.asset(
            'assets/icons/Edit.png',
            width: 35,
            height: 35,
          ),
        ],
      ),
    ),
  ),
  Container(height: 10)
],
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
);
}
}

Widget _buildDataRow(String label, String value, {bool isCurrency = false}) {
  if (isCurrency) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            label,
            textAlign: TextAlign.right,
            textDirection: TextDirection.rtl,
            style: const TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 18,
              fontWeight: FontWeight.w400,
            ),
          ),
          const SizedBox(height: 2),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                value,
                style: const TextStyle(
                  fontFamily: 'AgencyFB',
                  fontSize: 32,
                  fontWeight: FontWeight.w700,
                ),
              ),
              const SizedBox(width: 15),
              const Text(
                '\$',
                style: TextStyle(
                  fontFamily: 'AgencyFB',
                  fontSize: 32,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  return Padding(
    padding: const EdgeInsets.symmetric(vertical: 4),
    child: Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        Text(
          value,
          style: const TextStyle(
            fontFamily: 'Tajawal',
            fontSize: 18,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: Text(
            label,
            textAlign: TextAlign.right,
            textDirection: TextDirection.rtl,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: const TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 18,
              fontWeight: FontWeight.w400,
            ),
          ),
        ),
      ],
    ),
  );
}
Widget _buildImageSlider({
  required String title,
  required String? frontImage,
  required String? backImage,
  required PageController controller,
  required int currentPage,
  required Function(int) onPageChanged,
}) {
  return Padding(
    padding: const EdgeInsets.symmetric(horizontal: 16),
    child: Column(
      children: [
        SizedBox(
          width: 237,
          height: 29,
          child: Text(
            title,
            textAlign: TextAlign.center,
            textDirection: TextDirection.rtl,
            style: const TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 24,
              fontWeight: FontWeight.w500,
              height: 1.0,
            ),
          ),
        ),
        const SizedBox(height: 10),
        SizedBox(
          width: 396,
          height: 236,
          child: Stack(
            children: [
              PageView(
                controller: controller,
                onPageChanged: onPageChanged,
                children: [
                  _buildImageWidget(frontImage),
                  _buildImageWidget(backImage),
                ],
              ),
              Positioned(
                bottom: 12,
                left: 0,
                right: 0,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: List.generate(2, (index) {
                    return AnimatedContainer(
                      duration: const Duration(milliseconds: 300),
                      margin: const EdgeInsets.symmetric(horizontal: 4),
                      width: currentPage == index ? 30 : 20,
                      height: 15.89,
                      decoration: BoxDecoration(
                        color: currentPage == index
                            ? const Color(0xFFFFFFFF)
                            : const Color(0xFFB1B1B1),
                        borderRadius: BorderRadius.circular(15),
                      ),
                    );
                  }),
                ),
              ),
            ],
          ),
        ),
      ],
    ),
  );
}
Widget _buildImageWidget(String? imagePath) {
  const String defaultImage = 'assets/images/Eiffel.png';

  return ClipRRect(
    borderRadius: BorderRadius.circular(20),
    child: imagePath != null && imagePath.isNotEmpty
        ? Image.network(
      imagePath,
      width: 396,
      height: 236,
      fit: BoxFit.cover,
      errorBuilder: (context, error, stackTrace) {
        return Image.asset(
          defaultImage,
          width: 396,
          height: 236,
          fit: BoxFit.cover,
        );
      },
    )
        : Image.asset(
      defaultImage,
      width: 396,
      height: 236,
      fit: BoxFit.cover,
    ),
  );
}