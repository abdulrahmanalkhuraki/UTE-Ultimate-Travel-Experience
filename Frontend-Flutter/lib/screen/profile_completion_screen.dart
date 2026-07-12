import 'dart:math' as math;
import 'dart:ui';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class ProfileCompletionScreen extends StatefulWidget {
  const ProfileCompletionScreen({Key? key}) : super(key: key);

  @override
  State<ProfileCompletionScreen> createState() =>
      _ProfileCompletionScreenState();
}

class _ProfileCompletionScreenState extends State<ProfileCompletionScreen> {
  // متحكمات النصوص (Controllers) لكتابة كل شيء يدوياً لتتطابق مع التصميم
  TextEditingController firstNameController = TextEditingController();
  TextEditingController lastNameController = TextEditingController();
  TextEditingController phoneController = TextEditingController();
  TextEditingController nationalityController = TextEditingController();
  TextEditingController residenceController = TextEditingController();
  TextEditingController locationController = TextEditingController();
  TextEditingController nidController = TextEditingController();
  TextEditingController passportController = TextEditingController();
  TextEditingController bankAccountController = TextEditingController();

  String? selectedGender;
  DateTime? selectedDob;

  bool isGenderDropdownOpen = false;
  final List<String> genderCategories = ['ذكر', 'أنثى'];

  @override
  void initState() {
    super.initState();
    // مراقبة حقل الجنسية ومكان الإقامة لإظهار "صورة الإقامة" عند الاختلاف
    nationalityController.addListener(_checkResidenceLogic);
    residenceController.addListener(_checkResidenceLogic);
  }

  void _checkResidenceLogic() {
    setState(() {}); // تحديث الشاشة فوراً عند الكتابة
  }

  @override
  void dispose() {
    firstNameController.dispose();
    lastNameController.dispose();
    phoneController.dispose();
    nationalityController.dispose();
    residenceController.dispose();
    locationController.dispose();
    nidController.dispose();
    passportController.dispose();
    bankAccountController.dispose();
    super.dispose();
  }

  Future<void> _selectDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime(2000),
      firstDate: DateTime(1920),
      lastDate: DateTime.now(),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: Color(0xFFF4A261),
              onPrimary: Colors.white,
              onSurface: Colors.black,
            ),
          ),
          child: child!,
        );
      },
    );
    if (picked != null && picked != selectedDob) {
      setState(() {
        selectedDob = picked;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;

    // الشرط الذكي: إظهار صورة الإقامة فقط إذا اختلفت الإقامة المكتوبة عن الجنسية
    bool showResidenceImage = false;
    String nat = nationalityController.text.trim();
    String res = residenceController.text.trim();
    if (nat.isNotEmpty && res.isNotEmpty && nat != res) {
      showResidenceImage = true;
    }

    double currentTop = 450.0;
    List<Widget> dynamicFields = [];

    // 1. الاسم الأول
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'الاسم الأول:',
        assetPath: 'assets/icons/Profile_1.svg',
        isSvg: true,
        controller: firstNameController,
      ),
    );
    currentTop += 85.0;

    // 2. الاسم الأخير
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'الاسم الأخير:',
        assetPath: 'assets/icons/Profile_1.svg',
        isSvg: true,
        controller: lastNameController,
      ),
    );
    currentTop += 85.0;

    // 3. رقم الهاتف
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'رقم الهاتف:',
        assetPath: 'assets/icons/Call.svg',
        isSvg: true,
        controller: phoneController,
      ),
    );
    currentTop += 85.0;

    // 4. الجنسية (إدخال يدوي بدون سهم)
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'الجنسية:',
        assetPath: 'assets/icons/nationality.png',
        isSvg: false,
        controller: nationalityController,
      ),
    );
    currentTop += 85.0;

    // 5. مكان الإقامة (إدخال يدوي بدون سهم)
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'مكان الإقامة:',
        assetPath: 'assets/icons/Location_Add.svg',
        isSvg: true,
        controller: residenceController,
      ),
    );
    currentTop += 85.0;

    // 6. الموقع الحالي
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'الموقع الحالي:',
        assetPath: 'assets/icons/Location_Add.svg',
        isSvg: true,
        controller: locationController,
      ),
    );
    currentTop += 85.0;

    // 7. الجنس (يحتوي على سهم)
    dynamicFields.add(
      Positioned(
        top: currentTop * scale,
        left: 280 * scale,
        width: 85 * scale,
        height: 80 * scale,
        child: SvgPicture.asset('assets/icons/fORm.svg', fit: BoxFit.contain),
      ),
    );
    dynamicFields.addAll(
      _buildDropdownField(
        scale: scale,
        top: currentTop,
        text: selectedGender ?? 'الجنس:',
        iconPath: '',
        isSvg: true,
        width: 130,
        left: 75,
        isOpen: isGenderDropdownOpen,
        options: genderCategories,
        onToggle: () =>
            setState(() => isGenderDropdownOpen = !isGenderDropdownOpen),
        onSelect: (val) {
          setState(() {
            selectedGender = val;
            isGenderDropdownOpen = false;
          });
        },
      ),
    );
    currentTop += isGenderDropdownOpen ? 175.0 : 95.0;

    // 8. تاريخ الميلاد
    dynamicFields.add(
      Positioned(
        top: currentTop * scale,
        left: 0,
        width: 440 * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            'أدخل تاريخ ميلادك:',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w500,
              fontSize: 24 * scale,
              color: Colors.black,
            ),
          ),
        ),
      ),
    );
    dynamicFields.add(
      _buildDateBox(
        scale: scale,
        top: currentTop + 40,
        left: 290,
        text: selectedDob != null ? '${selectedDob!.day}' : 'يوم',
        onTap: () => _selectDate(context),
      ),
    );
    dynamicFields.add(
      _buildDateBox(
        scale: scale,
        top: currentTop + 40,
        left: 170,
        text: selectedDob != null ? '${selectedDob!.month}' : 'شهر',
        onTap: () => _selectDate(context),
      ),
    );
    dynamicFields.add(
      _buildDateBox(
        scale: scale,
        top: currentTop + 40,
        left: 50,
        text: selectedDob != null ? '${selectedDob!.year}' : 'سنة',
        onTap: () => _selectDate(context),
      ),
    );
    currentTop += 130.0;

    // 9. الرقم الوطني
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'الرقم الوطني:',
        assetPath: 'assets/icons/NID.svg',
        isSvg: true,
        controller: nidController,
      ),
    );
    currentTop += 95.0;

    // 10. صورة الهوية
    dynamicFields.addAll(
      _buildImageUploadFrame(
        scale: scale,
        top: currentTop,
        title: 'صورة الهوية الشخصية:',
        titleLeft: 143,
        titleWidth: 237,
      ),
    );
    currentTop += 370.0;

    // 11. رقم جواز السفر
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'رقم جواز السفر:',
        assetPath: 'assets/icons/passport.png',
        isSvg: false,
        controller: passportController,
      ),
    );
    currentTop += 95.0;

    // 12. صورة جواز السفر
    dynamicFields.addAll(
      _buildImageUploadFrame(
        scale: scale,
        top: currentTop,
        title: 'صورة جواز السفر:',
        titleLeft: 204,
        titleWidth: 176,
      ),
    );
    currentTop += 370.0;

    // 13. صورة الإقامة الدولية (تظهر فقط إذا تحقق الشرط)
    if (showResidenceImage) {
      dynamicFields.addAll(
        _buildImageUploadFrame(
          scale: scale,
          top: currentTop,
          title: 'صورة الإقامة الدولية:',
          titleLeft: 171,
          titleWidth: 209,
        ),
      );
      currentTop += 370.0;
    }

    // 14. حسابك البنكي
    dynamicFields.addAll(
      _buildTextInputField(
        scale: scale,
        top: currentTop,
        hintText: 'حسابك البنكي:',
        assetPath: 'assets/icons/Card.svg',
        isSvg: true,
        controller: bankAccountController,
      ),
    );
    currentTop += 95.0;

    double submitButtonTop = currentTop + 20.0;

    // مساحة السكرول الآمنة لاحتواء الـ Vector
    double stackHeight = submitButtonTop + 150.0;
    if (stackHeight < 2750) stackHeight = 2750;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
      // إغلاق لوحة المفاتيح عند النقر في أي مكان فارغ
      body: GestureDetector(
        onTap: () => FocusScope.of(context).unfocus(),
        child: Directionality(
          textDirection: TextDirection.ltr,
          child: SingleChildScrollView(
            child: SizedBox(
              width: 440 * scale,
              height: stackHeight * scale,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  // 1. الخلفية العلوية (Vector)
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

                  // (تم حذف الخلفية السفلية من هنا بناءً على طلبك)

                  // 3. سهم الرجوع
                  Positioned(
                    top: 42 * scale,
                    left: 370 * scale,
                    width: 50 * scale,
                    height: 50 * scale,
                    child: SvgPicture.asset(
                      'assets/icons/Right.svg',
                      fit: BoxFit.contain,
                    ),
                  ),

                  // 4. العنوان المعدل (الرجاء استكمال معلومات ملفك / الشخصي) بخط Cairo
                  Positioned(
                    top: 92 * scale,
                    left: 41 * scale,
                    width: 357 * scale,
                    height: 90 * scale,
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        Text(
                          'الرجاء استكمال معلومات ملفك',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Cairo',
                            fontWeight: FontWeight.w600,
                            fontSize: 24 * scale,
                            height: 1.1,
                            color: const Color(0xFF000000),
                          ),
                        ),
                        SizedBox(height: 2 * scale), // مسافة المباعدة الدقيقة
                        Text(
                          'الشخصي',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Cairo',
                            fontWeight: FontWeight.w600,
                            fontSize: 24 * scale,
                            height: 1.1,
                            color: const Color(0xFF000000),
                          ),
                        ),
                      ],
                    ),
                  ),

                  // 5. أيقونة الملف الشخصي
                  Positioned(
                    top: 190 * scale,
                    left: 120 * scale,
                    width: 200 * scale,
                    height: 200 * scale,
                    child: SvgPicture.asset(
                      'assets/icons/Profile_Circle.svg',
                      fit: BoxFit.contain,
                    ),
                  ),

                  // 6. نص إضافة صورة
                  Positioned(
                    top: 400 * scale,
                    left: 79 * scale,
                    width: 282 * scale,
                    height: 28 * scale,
                    child: Directionality(
                      textDirection: TextDirection.rtl,
                      child: Center(
                        child: Text(
                          'اضف صورة لملفك الشخصي',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Tajawal',
                            fontWeight: FontWeight.w400,
                            fontSize: 20 * scale,
                            height: 1.0,
                            color: Colors.black,
                          ),
                        ),
                      ),
                    ),
                  ),

                  // الحقول الديناميكية والثابتة
                  ...dynamicFields,

                  // 7. زر التقديم
                  Positioned(
                    top: submitButtonTop * scale,
                    left: 109 * scale,
                    width: 222 * scale,
                    height: 60 * scale,
                    child: Container(
                      decoration: BoxDecoration(
                        color: const Color(0xFFF4A261),
                        borderRadius: BorderRadius.circular(15 * scale),
                      ),
                      child: Stack(
                        alignment: Alignment.center,
                        children: [
                          Text(
                            'تقديم',
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontWeight: FontWeight.w500,
                              fontSize: 24 * scale,
                              color: Colors.white,
                            ),
                          ),
                          Positioned(
                            left: 179 * scale,
                            width: 37 * scale,
                            height: 37 * scale,
                            child: SvgPicture.asset(
                              'assets/icons/present.svg',
                              fit: BoxFit.contain,
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
        ),
      ),
    );
  }

  // --- دالة حقول الكتابة (TextField) لجميع الحقول ---
  List<Widget> _buildTextInputField({
    required double scale,
    required double top,
    required String hintText,
    required String assetPath,
    required bool isSvg,
    TextEditingController? controller,
  }) {
    return [
      Positioned(
        top: top * scale,
        left: 50 * scale,
        width: 340 * scale,
        height: 75 * scale,
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            border: Border.all(color: Colors.black, width: 2 * scale),
            borderRadius: BorderRadius.circular(15 * scale),
          ),
          child: Stack(
            children: [
              Positioned(
                top: 0,
                bottom: 0,
                right: 60 * scale,
                left: 15 * scale,
                child: Directionality(
                  textDirection: TextDirection.rtl,
                  child: Center(
                    child: TextField(
                      controller: controller,
                      style: TextStyle(
                        fontFamily: 'Tajawal',
                        fontWeight: FontWeight.w500,
                        fontSize: 24 * scale,
                        color: Colors.black,
                      ),
                      decoration: InputDecoration(
                        hintText: hintText,
                        hintStyle: TextStyle(
                          fontFamily: 'Tajawal',
                          fontWeight: FontWeight.w500,
                          fontSize: 24 * scale,
                          color: Colors.black.withOpacity(0.5),
                        ),
                        border: InputBorder.none,
                        isDense: true,
                      ),
                    ),
                  ),
                ),
              ),
              Positioned(
                top: 22 * scale,
                right: 15 * scale,
                width: 30 * scale,
                height: 30 * scale,
                child: isSvg
                    ? SvgPicture.asset(assetPath, fit: BoxFit.contain)
                    : Image.asset(assetPath, fit: BoxFit.contain),
              ),
            ],
          ),
        ),
      ),
    ];
  }

  // --- دالة القائمة المنسدلة للـ (الجنس) ---
  List<Widget> _buildDropdownField({
    required double scale,
    required double top,
    required String text,
    required String iconPath,
    required bool isSvg,
    required bool isOpen,
    required List<String> options,
    required VoidCallback onToggle,
    required Function(String) onSelect,
    double width = 340,
    double left = 50,
  }) {
    return [
      Positioned(
        top: top * scale,
        left: left * scale,
        width: width * scale,
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 250),
          height: (isOpen ? 250.0 : 75.0) * scale,
          decoration: BoxDecoration(
            color: Colors.white,
            border: Border.all(color: Colors.black, width: 2 * scale),
            borderRadius: BorderRadius.circular(15 * scale),
          ),
          child: Column(
            children: [
              GestureDetector(
                onTap: onToggle,
                child: SizedBox(
                  height: 71 * scale,
                  child: Stack(
                    children: [
                      Positioned(
                        top: 22 * scale,
                        right: (iconPath.isNotEmpty ? 50 : 20) * scale,
                        child: Directionality(
                          textDirection: TextDirection.rtl,
                          child: Text(
                            text,
                            textAlign: TextAlign.start,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontWeight: FontWeight.w500,
                              fontSize: 24 * scale,
                              color: text.endsWith(':')
                                  ? Colors.black.withOpacity(0.5)
                                  : Colors.black,
                            ),
                          ),
                        ),
                      ),
                      if (iconPath.isNotEmpty)
                        Positioned(
                          top: 22 * scale,
                          right: 15 * scale,
                          width: 30 * scale,
                          height: 30 * scale,
                          child: isSvg
                              ? SvgPicture.asset(iconPath, fit: BoxFit.contain)
                              : Image.asset(iconPath, fit: BoxFit.contain),
                        ),
                      Positioned(
                        top: 22 * scale,
                        left: 15 * scale,
                        width: 24 * scale,
                        height: 24 * scale,
                        child: Container(
                          decoration: BoxDecoration(
                            border: Border.all(
                              color: Colors.black,
                              width: 1.2 * scale,
                            ),
                            borderRadius: BorderRadius.circular(5 * scale),
                          ),
                          child: Icon(
                            isOpen
                                ? Icons.keyboard_arrow_up
                                : Icons.keyboard_arrow_down,
                            size: 20 * scale,
                            color: Colors.black.withOpacity(0.5),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              if (isOpen)
                Expanded(
                  child: ListView.builder(
                    padding: EdgeInsets.zero,
                    itemCount: options.length,
                    itemBuilder: (context, index) {
                      return GestureDetector(
                        onTap: () => onSelect(options[index]),
                        child: Container(
                          width: double.infinity,
                          padding: EdgeInsets.symmetric(vertical: 8 * scale),
                          decoration: BoxDecoration(
                            border: Border(
                              top: BorderSide(color: Colors.grey.shade300),
                            ),
                          ),
                          child: Text(
                            options[index],
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 20 * scale,
                              fontWeight: FontWeight.w500,
                              color: Colors.black,
                            ),
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
    ];
  }

  Widget _buildDateBox({
    required double scale,
    required double top,
    required double left,
    required String text,
    required VoidCallback onTap,
  }) {
    return Positioned(
      top: top * scale,
      left: left * scale,
      width: 100 * scale,
      height: 75 * scale,
      child: GestureDetector(
        onTap: onTap,
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            border: Border.all(color: Colors.black, width: 2 * scale),
            borderRadius: BorderRadius.circular(15 * scale),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Container(
                width: 24 * scale,
                height: 24 * scale,
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.black, width: 1.2 * scale),
                  borderRadius: BorderRadius.circular(5 * scale),
                ),
                child: Icon(
                  Icons.keyboard_arrow_down,
                  size: 20 * scale,
                  color: Colors.black.withOpacity(0.5),
                ),
              ),
              SizedBox(width: 5 * scale),
              Text(
                text,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontWeight: FontWeight.w500,
                  fontSize: 20 * scale,
                  color: selectedDob != null
                      ? Colors.black
                      : Colors.black.withOpacity(0.5),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  List<Widget> _buildImageUploadFrame({
    required double scale,
    required double top,
    required String title,
    required double titleLeft,
    required double titleWidth,
  }) {
    return [
      Positioned(
        top: top * scale,
        left: titleLeft * scale,
        width: titleWidth * scale,
        child: Directionality(
          textDirection: TextDirection.rtl,
          child: Text(
            title,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontWeight: FontWeight.w500,
              fontSize: 24 * scale,
              color: Colors.black,
            ),
          ),
        ),
      ),
      Positioned(
        top: (top + 40) * scale,
        left: 50 * scale,
        width: 340 * scale,
        height: 319.6 * scale,
        child: Stack(
          children: [
            SvgPicture.asset(
              'assets/icons/picFrame.svg',
              width: 340 * scale,
              height: 319.6 * scale,
              fit: BoxFit.fill,
            ),
            Center(
              child: SvgPicture.asset(
                'assets/icons/Image.svg',
                width: 100 * scale,
                height: 100 * scale,
              ),
            ),
          ],
        ),
      ),
    ];
  }
}