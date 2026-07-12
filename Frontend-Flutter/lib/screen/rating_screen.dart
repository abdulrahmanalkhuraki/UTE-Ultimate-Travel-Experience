 import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
import 'bottomNavigationBar.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class RatingScreen extends StatefulWidget {
const RatingScreen({super.key});

@override
State<RatingScreen> createState() => _RatingScreenState();
}

class _RatingScreenState extends State<RatingScreen> {
int _rating = 0;
final TextEditingController _reviewController = TextEditingController();

@override
void dispose() {
_reviewController.dispose();
super.dispose();
}

@override
Widget build(BuildContext context) {
final size = MediaQuery.of(context).size;
final double scale = size.width / 440;

// 🌟 المتغير السحري لمعرفة حالة الوضع الليلي
final bool isDarkMode = context.watch<ThemeCubit>().state == ThemeMode.dark;
final Color textColor = isDarkMode ? Colors.white : Colors.black;

return Scaffold(
backgroundColor: isDarkMode ? const Color(0xFF1E1E1E) : const Color(0xFFFFFFFF),
body: SafeArea(
child: Directionality(
textDirection: TextDirection.ltr,
child: SingleChildScrollView(
child: SizedBox(
width: 440 * scale,
height: 950 * scale,
child: Stack(
clipBehavior: Clip.none,
children: [
// --- 1. الفيكتور الخلفي (تم حل التغبيش وتطبيق نفس طريقتك بالدعم) ---
Positioned(
top: 0,
left: 0,
right: 0,
child: SvgPicture.asset(
'assets/images/Vector.svg',
fit: BoxFit.cover,
colorFilter: isDarkMode
? const ColorFilter.mode(Color(0xFF223A5E), BlendMode.srcIn)
    : null,
),
),

// --- 2. زر الرجوع ---
Positioned(
top: 42 * scale,
left: 370 * scale,
width: 50 * scale,
height: 50 * scale,
child: GestureDetector(
onTap: () => Navigator.pop(context),
child: SvgPicture.asset(
'assets/icons/Right.svg',
fit: BoxFit.contain,
colorFilter: isDarkMode
? const ColorFilter.mode(Colors.white, BlendMode.srcIn)
    : null,
),
),
),

// --- 3. العنوان الرئيسي ---
Positioned(
top: 25 * scale,
left: 92 * scale,
width: 256 * scale,
height: 75 * scale,
child: Center(
child: Directionality(
textDirection: TextDirection.rtl,
child: Text(
'تقييم UTE',
textAlign: TextAlign.center,
style: TextStyle(
fontFamily: 'Cairo',
fontWeight: FontWeight.w700,
fontSize: 40 * scale,
letterSpacing: 2 * scale,
height: 1.0,
color: textColor,
),
),
),
),
),
// --- 🌟 4. الخط المتدرج (الذي تم إضافته برمجياً) 🌟 ---
Positioned(
top: 105 * scale,
left: 32 * scale,
right: 32 * scale,
child: Container(
height: 1.2,
decoration: BoxDecoration(
gradient: LinearGradient(
colors: [
const Color(0xFF666666).withOpacity(0.5),
textColor, // يتغير للأسود بالنهاري وللأبيض بالليلي
const Color(0xFF666666).withOpacity(0.5),
],
),
),
),
),

// --- 5. النص الترحيبي ---
Positioned(
top: 129 * scale,
left: 39 * scale,
width: 362 * scale,
height: 114 * scale,
child: Directionality(
textDirection: TextDirection.rtl,
child: Text(
'شكرا لك لرغبتك في تقييم تطبيقنا, سنكون سعيدين بمشاركتنا رأيك في التطبيق.',
textAlign: TextAlign.center,
style: TextStyle(
fontFamily: 'Tajawal',
fontWeight: FontWeight.w400,
fontSize: 32 * scale,
height: 1.2,
color: textColor,
),
),
),
),

// --- 6. سؤال التقييم ---
Positioned(
top: 263 * scale,
left: 78 * scale,
width: 337 * scale,
height: 29 * scale,
child: Directionality(
textDirection: TextDirection.rtl,
child: Text(
'ما هو تقييمك للتطبيق بشكل عام :',
textAlign: TextAlign.center,
style: TextStyle(
fontFamily: 'Tajawal',
fontWeight: FontWeight.w400,
fontSize: 24 * scale,
height: 1.0,
color: textColor,
),
),
),
),

// --- 7. النجمات الخمسة التفاعلية ---
Positioned(
top: 307.45 * scale,
left: 82 * scale,
width: 266 * scale,
height: 52.75 * scale,
child: Row(
mainAxisAlignment: MainAxisAlignment.spaceBetween,
children: List.generate(5, (index) {
return GestureDetector(
onTap: () {
setState(() {
_rating = index + 1;
});
},
child: SvgPicture.asset(
'assets/icons/star4.svg',
width: 50 * scale,
height: 50 * scale,
colorFilter: ColorFilter.mode(
index < _rating
? const Color(0xFFF4A261)
    : textColor,
BlendMode.srcIn,
),
),
);
}),
),
),
 // --- 8. نص مربع المراجعة ---
Positioned(
top: 381 * scale,
left: 66 * scale,
width: 349 * scale,
height: 87 * scale,
child: Directionality(
textDirection: TextDirection.rtl,
child: Text(
'اترك مراجعة عن التطبيق أو أي ميزة ترغب إضافتها لتحسين التجربة في التطبيق:',
textAlign: TextAlign.center,
style: TextStyle(
fontFamily: 'Tajawal',
fontWeight: FontWeight.w400,
fontSize: 24 * scale,
height: 1.2,
color: textColor,
),
),
),
),

// --- 9. مربع الإدخال ---
Positioned(
top: 480 * scale,
left: 22 * scale,
width: 396 * scale,
height: 186 * scale,
child: ClipRRect(
borderRadius: BorderRadius.circular(24.5 * scale),
child: BackdropFilter(
filter: ImageFilter.blur(sigmaX: 5.0, sigmaY: 5.0),
child: Container(
decoration: BoxDecoration(
color: Colors.transparent,
borderRadius: BorderRadius.circular(24.5 * scale),
border: Border.all(
color: textColor.withOpacity(0.5),
width: 1.5 * scale,
),
),
padding: EdgeInsets.all(16 * scale),
child: TextField(
controller: _reviewController,
textAlign: TextAlign.right,
textDirection: TextDirection.rtl,
maxLines: null,
expands: true,
style: TextStyle(
fontFamily: 'Tajawal',
fontSize: 20 * scale,
color: textColor,
),
decoration: InputDecoration(
border: InputBorder.none,
hintText: 'اكتب مراجعتك هنا...',
hintStyle: TextStyle(
fontFamily: 'Tajawal',
fontSize: 18 * scale,
color: textColor.withOpacity(0.4),
),
),
),
),
),
),
),
 // --- 10. زر الإرسال ---
Positioned(
top: 698 * scale,
left: 130 * scale,
width: 180 * scale,
height: 65 * scale,
child: GestureDetector(
onTap: () {
print('Rating: $_rating, Review: ${_reviewController.text}');
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

// --- 11. شريط التنقل السفلي ---
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
),
);
}
}