import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class OwnerInfoScreen extends StatefulWidget {
  const OwnerInfoScreen({super.key});

  @override
  State<OwnerInfoScreen> createState() => _OwnerInfoScreenState();
}

class _OwnerInfoScreenState extends State<OwnerInfoScreen> {
  final TextEditingController firstNameController = TextEditingController();
  final TextEditingController lastNameController = TextEditingController();
  final TextEditingController phoneController = TextEditingController();
  final TextEditingController residenceController = TextEditingController();
  final TextEditingController nationalIdController = TextEditingController();
  final TextEditingController bankAccountController = TextEditingController();

  String? selectedGender;
  DateTime? selectedDob;
  bool isGenderDropdownOpen = false;

  File? profileImage;
  File? idImage;

  final ImagePicker _picker = ImagePicker();
  final List<String> genderCategories = ['ذكر', 'أنثى'];

  @override
  void dispose() {
    firstNameController.dispose();
    lastNameController.dispose();
    phoneController.dispose();
    residenceController.dispose();
    nationalIdController.dispose();
    bankAccountController.dispose();
    super.dispose();
  }

  Future<void> _pickImage(String type) async {
    final XFile? pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      setState(() {
        if (type == 'profile') profileImage = File(pickedFile.path);
        else if (type == 'id') idImage = File(pickedFile.path);
      });
    }
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
      setState(() => selectedDob = picked);
    }
  }

  void _submitOwnerInfo() {
    if (firstNameController.text.isEmpty || lastNameController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'الرجاء تعبئة جميع الحقول الإلزامية',
            style: TextStyle(fontFamily: 'Tajawal', fontSize: 16),
            textAlign: TextAlign.center,
          ),
          backgroundColor: Colors.red,
          duration: Duration(seconds: 3),
        ),
      );
      return;
    }

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text(
          'تم حفظ معلومات صاحب الشركة بنجاح!',
          style: TextStyle(fontFamily: 'Tajawal', fontSize: 16),
          textAlign: TextAlign.center,
        ),
        backgroundColor: Colors.green,
      ),
    );

    Future.delayed(const Duration(seconds: 1), () {
      Navigator.pop(context);
    });
  }

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.of(context).size;
    final double scale = size.width / 440;

    // حساب المسافات الديناميكية بناءً على حالة القائمة المنسدلة
    double genderOffset = isGenderDropdownOpen ? 100.0 : 0.0;
    // تم زيادة currentTop ليناسب الإزاحة الجديدة للعنوان
    double currentTop = 855.0 + genderOffset;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
      body: GestureDetector(
        onTap: () => FocusScope.of(context).unfocus(),
        child: Directionality(
          textDirection: TextDirection.ltr,
          child: SingleChildScrollView(
            child: SizedBox(
              width: 440 * scale,
              height: (currentTop + 850) * scale,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  // 🌟 قياسات وموقع الفيكتور مطابقة لفيغما تمامًا + اللون الصحيح
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

                  // سهم الرجوع
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
                      ),
                    ),
                  ),

                  // 🌟 تعديل قياسات وخصائص جملة (معلومات صاحب الشركة) كما طلبتِ
                  Positioned(
                    top: 99 * scale,
                    left: 41 * scale,
                    width: 357 * scale,
                    height: 45 * scale,
                    child: Center(
                      child: Text(
                        'معلومات صاحب الشركة',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontFamily: 'Cairo',
                          fontWeight: FontWeight.w600, // SemiBold
                          fontSize: 24 * scale,
                          height: 1.0, // line-height 100%
                          letterSpacing: 0,
                          color: const Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),

                  // صورة البروفايل (تمت إزاحتها قليلاً للأسفل لتناسب العنوان الجديد)
                  Positioned(
                    top: 160 * scale,
                    left: 120 * scale,
                    width: 200 * scale,
                    height: 200 * scale,
                    child: GestureDetector(
                      onTap: () => _pickImage('profile'),
                      child: profileImage != null
                          ? ClipOval(
                        child: Image.file(
                          profileImage!,
                          width: 200 * scale,
                          height: 200 * scale,
                          fit: BoxFit.cover,
                        ),
                      )
                          : SvgPicture.asset(
                        'assets/icons/Profile_Circle.svg',
                        fit: BoxFit.contain,
                      ),
                    ),
                  ),

                  Positioned(
                    top: 370 * scale,
                    left: 50 * scale,
                    width: 340 * scale,
                    height: 28 * scale,
                    child: Directionality(
                      textDirection: TextDirection.rtl,
                      child: Center(
                        child: Text(
                          'الصورة الشخصية لصاحب الشركة',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Tajawal',
                            fontWeight: FontWeight.w400,
                            fontSize: 20 * scale,
                            color: Colors.black,
                          ),
                        ),
                      ),
                    ),
                  ),

                  // الحقول النصية الأساسية
                  ..._buildTextInputField(
                    scale: scale,
                    top: 415,
                    hintText: 'الاسم الأول:',
                    assetPath: 'assets/icons/Profile_1.svg',
                    isSvg: true,
                    controller: firstNameController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 500,
                    hintText: 'الاسم الأخير:',
                    assetPath: 'assets/icons/Profile_1.svg',
                    isSvg: true,
                    controller: lastNameController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 585,
                    hintText: 'رقم الهاتف:',
                    assetPath: 'assets/icons/Call.svg',
                    isSvg: true,
                    controller: phoneController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 670,
                    hintText: 'مكان الإقامة:',
                    assetPath: 'assets/icons/Location_Add.svg',
                    isSvg: true,
                    controller: residenceController,
                  ),

                  // حقل الجنس (Dropdown)
                  Positioned(
                    top: 755 * scale,
                    left: 280 * scale,
                    width: 85 * scale,
                    height: 80 * scale,
                    child: SvgPicture.asset(
                      'assets/icons/fORm.svg',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    top: 755 * scale,
                    left: 75 * scale,
                    width: 130 * scale,
                    child: AnimatedContainer(
                      duration: const Duration(milliseconds: 200),
                      height: (isGenderDropdownOpen ? 175 : 75) * scale,
                      decoration: BoxDecoration(
                        color: Colors.white,
                        border: Border.all(color: Colors.black, width: 2 * scale),
                        borderRadius: BorderRadius.circular(15 * scale),
                      ),
                      child: SingleChildScrollView(
                        physics: const NeverScrollableScrollPhysics(),
                        child: Column(
                          children: [
                            GestureDetector(
                              onTap: () {
                                setState(() {
                                  isGenderDropdownOpen = !isGenderDropdownOpen;
                                });
                              },
                              child: SizedBox(
                                height: 71 * scale,
                                child: Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  children: [
                                    Container(
                                      decoration: BoxDecoration(
                                        border: Border.all(color: Colors.black, width: 1.2 * scale),
                                        borderRadius: BorderRadius.circular(5 * scale),
                                      ),
                                      width: 24 * scale,
                                      height: 24 * scale,
                                      child: Icon(
                                        isGenderDropdownOpen ? Icons.keyboard_arrow_up : Icons.keyboard_arrow_down,
                                        size: 20 * scale,
                                      ),
                                    ),
                                    SizedBox(width: 10 * scale),
                                    Directionality(
                                      textDirection: TextDirection.rtl,
                                      child: Text(
                                        selectedGender ?? 'الجنس:',
                                        style: TextStyle(
                                          fontFamily: 'Tajawal',
                                          fontSize: 24 * scale,
                                          fontWeight: FontWeight.w500,
                                          color: selectedGender == null ? Colors.black.withOpacity(0.5) : Colors.black,
                                        ),
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                            if (isGenderDropdownOpen)
                              ...genderCategories.map(
                                    (gender) => GestureDetector(
                                  onTap: () => setState(() {
                                    selectedGender = gender;
                                    isGenderDropdownOpen = false;
                                  }),
                                  child: Container(
                                    width: double.infinity,
                                    padding: EdgeInsets.symmetric(vertical: 10 * scale),
                                    decoration: BoxDecoration(
                                      border: Border(top: BorderSide(color: Colors.grey.shade300)),
                                    ),
                                    child: Text(
                                      gender,
                                      textAlign: TextAlign.center,
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
                          ],
                        ),
                      ),
                    ),
                  ),

                  // تاريخ الميلاد
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
                  _buildDateBox(
                    scale: scale,
                    top: currentTop + 46,
                    left: 290,
                    text: selectedDob != null ? '${selectedDob!.day}' : 'يوم',
                    onTap: () => _selectDate(context),
                  ),
                  _buildDateBox(
                    scale: scale,
                    top: currentTop + 46,
                    left: 170,
                    text: selectedDob != null ? '${selectedDob!.month}' : 'شهر',
                    onTap: () => _selectDate(context),
                  ),
                  _buildDateBox(
                    scale: scale,
                    top: currentTop + 46,
                    left: 50,
                    text: selectedDob != null ? '${selectedDob!.year}' : 'سنة',
                    onTap: () => _selectDate(context),
                  ),

                  // الرقم الوطني
                  ..._buildTextInputField(
                    scale: scale,
                    top: currentTop + 135,
                    hintText: 'الرقم الوطني:',
                    assetPath: 'assets/icons/NID.svg',
                    isSvg: true,
                    controller: nationalIdController,
                  ),

                  // صورة الهوية
                  ..._buildImageUploadFrame(
                    scale: scale,
                    top: currentTop + 230,
                    title: 'صورة الهوية الشخصية:',
                    titleLeft: 143,
                    titleWidth: 237,
                    imageFile: idImage,
                    onTap: () => _pickImage('id'),
                  ),

                  // الحساب البنكي
                  ..._buildTextInputField(
                    scale: scale,
                    top: currentTop + 620,
                    hintText: 'حسابك البنكي:',
                    assetPath: 'assets/icons/Card.svg',
                    isSvg: true,
                    controller: bankAccountController,
                  ),

                  // زر التقديم
                  Positioned(
                    top: (currentTop + 715) * scale,
                    left: 109 * scale,
                    width: 222 * scale,
                    height: 60 * scale,
                    child: GestureDetector(
                      onTap: _submitOwnerInfo,
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
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

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
                  color: selectedDob != null ? Colors.black : Colors.black.withOpacity(0.5),
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
    File? imageFile,
    required VoidCallback onTap,
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
        child: GestureDetector(
          onTap: onTap,
          child: Stack(
            children: [
              SvgPicture.asset(
                'assets/icons/picFrame.svg',
                width: 340 * scale,
                height: 319.6 * scale,
                fit: BoxFit.fill,
              ),
              Center(
                child: imageFile != null
                    ? ClipRRect(
                  borderRadius: BorderRadius.circular(15 * scale),
                  child: Image.file(
                    imageFile,
                    width: 320 * scale,
                    height: 300 * scale,
                    fit: BoxFit.cover,
                  ),
                )
                    : SvgPicture.asset(
                  'assets/icons/Image.svg',
                  width: 100 * scale,
                  height: 100 * scale,
                ),
              ),
            ],
          ),
        ),
      ),
    ];
  }
}