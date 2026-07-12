import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'theme_cubit.dart';
class AddCompanionScreen extends StatefulWidget {
  const AddCompanionScreen({super.key});

  @override
  State<AddCompanionScreen> createState() => _AddCompanionScreenState();
}

class _AddCompanionScreenState extends State<AddCompanionScreen> {
  final TextEditingController firstNameController = TextEditingController();
  final TextEditingController lastNameController = TextEditingController();
  final TextEditingController relationController = TextEditingController();
  final TextEditingController phoneController = TextEditingController();
  final TextEditingController nationalityController = TextEditingController();
  final TextEditingController residenceController = TextEditingController();
  final TextEditingController nationalIdController = TextEditingController();
  final TextEditingController passportController = TextEditingController();

  String? selectedAge;
  String? selectedGender;
  DateTime? selectedDob;

  bool isAgeDropdownOpen = false;
  bool isGenderDropdownOpen = false;

  File? companionProfileImage;
  File? idImage;
  File? passportImageFile;
  File? residenceImageFile;

  final ImagePicker _picker = ImagePicker();

  final List<String> ageCategories = [
    'تحت 6 سنوات',
    'من 6 إلى 14 سنوات',
    'من 14 إلى 18 سنوات',
    'فوق 18 سنة',
  ];
  final List<String> genderCategories = ['ذكر', 'أنثى'];

  @override
  void dispose() {
    firstNameController.dispose();
    lastNameController.dispose();
    relationController.dispose();
    phoneController.dispose();
    nationalityController.dispose();
    residenceController.dispose();
    nationalIdController.dispose();
    passportController.dispose();
    super.dispose();
  }

  Future<void> _pickImage(String type) async {
    final XFile? pickedFile = await _picker.pickImage(
      source: ImageSource.gallery,
    );
    if (pickedFile != null) {
      setState(() {
        if (type == 'profile')
          companionProfileImage = File(pickedFile.path);
        else if (type == 'id')
          idImage = File(pickedFile.path);
        else if (type == 'passport')
          passportImageFile = File(pickedFile.path);
        else if (type == 'residence')
          residenceImageFile = File(pickedFile.path);
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
    if (picked != null && picked != selectedDob)
      setState(() => selectedDob = picked);
  }

  void _submitCompanion() {
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
          'تمت إضافة المرافق بنجاح!',
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

    double genderOffset = isGenderDropdownOpen ? 100.0 : 0.0;
    double ageTop = 965.0 + genderOffset;
    double ageOffset = isAgeDropdownOpen ? 197.0 : 0.0;
    double dobTop = 1065.0 + genderOffset + ageOffset;
    double currentTop = dobTop + 140.0;

    List<Widget> dynamicFields = [];

    if (selectedAge == 'من 14 إلى 18 سنوات' || selectedAge == 'فوق 18 سنة') {
      dynamicFields.addAll(
        _buildTextInputField(
          scale: scale,
          top: currentTop,
          hintText: 'الرقم الوطني:',
          assetPath: 'assets/icons/NID.svg',
          isSvg: true,
          controller: nationalIdController,
        ),
      );
      currentTop += 95.0;
      dynamicFields.addAll(
        _buildImageUploadFrame(
          scale: scale,
          top: currentTop,
          title: 'صورة الهوية الشخصية:',
          titleLeft: 143,
          titleWidth: 237,
          imageFile: idImage,
          onTap: () => _pickImage('id'),
        ),
      );
      currentTop += 370.0;
    }

    if (selectedAge == 'فوق 18 سنة') {
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
      dynamicFields.addAll(
        _buildImageUploadFrame(
          scale: scale,
          top: currentTop,
          title: 'صورة جواز السفر:',
          titleLeft: 204,
          titleWidth: 176,
          imageFile: passportImageFile,
          onTap: () => _pickImage('passport'),
        ),
      );
      currentTop += 370.0;
      dynamicFields.addAll(
        _buildImageUploadFrame(
          scale: scale,
          top: currentTop,
          title: 'صورة الإقامة الدولية:',
          titleLeft: 171,
          titleWidth: 209,
          imageFile: residenceImageFile,
          onTap: () => _pickImage('residence'),
        ),
      );
      currentTop += 370.0;
    }

    double submitButtonTop = currentTop + 20.0;
    double stackHeight = submitButtonTop + 150.0;
    if (stackHeight < 1346) stackHeight = 1346;

    return Scaffold(
      backgroundColor: const Color(0xFFFFFFFF),
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
                  Positioned(
                    top: 25 * scale,
                    left: 92 * scale,
                    width: 256 * scale,
                    height: 75 * scale,
                    child: Center(
                      child: Text(
                        'إضافة مرافق',
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
                  Positioned(
                    top: 100 * scale,
                    left: 120 * scale,
                    width: 200 * scale,
                    height: 200 * scale,
                    child: GestureDetector(
                      onTap: () => _pickImage('profile'),
                      child: companionProfileImage != null
                          ? ClipOval(
                        child: Image.file(
                          companionProfileImage!,
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
                    top: 304 * scale,
                    left: 79 * scale,
                    width: 282 * scale,
                    height: 28 * scale,
                    child: Directionality(
                      textDirection: TextDirection.rtl,
                      child: Center(
                        child: Text(
                          'اضف صورة شخصية لمرافقك',
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
                  ..._buildTextInputField(
                    scale: scale,
                    top: 365,
                    hintText: 'الاسم الأول:',
                    assetPath: 'assets/icons/Profile_1.svg',
                    isSvg: true,
                    controller: firstNameController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 450,
                    hintText: 'الاسم الأخير:',
                    assetPath: 'assets/icons/Profile_1.svg',
                    isSvg: true,
                    controller: lastNameController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 535,
                    hintText: 'صلة قرابته بك:',
                    assetPath: 'assets/icons/interactions.png',
                    isSvg: false,
                    controller: relationController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 620,
                    hintText: 'رقم الهاتف:',
                    assetPath: 'assets/icons/Call.svg',
                    isSvg: true,
                    controller: phoneController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 705,
                    hintText: 'الجنسية:',
                    assetPath: 'assets/icons/passport.png',
                    isSvg: false,
                    controller: nationalityController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: 790,
                    hintText: 'مكان الإقامة:',
                    assetPath: 'assets/icons/Location_Add.svg',
                    isSvg: true,
                    controller: residenceController,
                  ),
                  Positioned(
                    top: 875 * scale,
                    left: 280 * scale,
                    width: 85 * scale,
                    height: 80 * scale,
                    child: SvgPicture.asset(
                      'assets/icons/fORm.svg',
                      fit: BoxFit.contain,
                    ),
                  ),
                  Positioned(
                    top: 875 * scale,
                    left: 75 * scale,
                    width: 130 * scale,
                    child: AnimatedContainer(
                      duration: const Duration(milliseconds: 200),
                      height: (isGenderDropdownOpen ? 175 : 75) * scale,
                      decoration: BoxDecoration(
                        color: Colors.white,
                        border: Border.all(
                          color: Colors.black,
                          width: 2 * scale,
                        ),
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
                                  if (isGenderDropdownOpen)
                                    isAgeDropdownOpen = false;
                                });
                              },
                              child: SizedBox(
                                height: 71 * scale,
                                child: Row(
                                  mainAxisAlignment: MainAxisAlignment.center,
                                  children: [
                                    Container(
                                      decoration: BoxDecoration(
                                        border: Border.all(
                                          color: Colors.black,
                                          width: 1.2 * scale,
                                        ),
                                        borderRadius: BorderRadius.circular(
                                          5 * scale,
                                        ),
                                      ),
                                      width: 24 * scale,
                                      height: 24 * scale,
                                      child: Icon(
                                        isGenderDropdownOpen
                                            ? Icons.keyboard_arrow_up
                                            : Icons.keyboard_arrow_down,
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
                                          color: selectedGender == null
                                              ? Colors.black.withOpacity(0.5)
                                              : Colors.black,
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
                                    padding: EdgeInsets.symmetric(
                                      vertical: 10 * scale,
                                    ),
                                    decoration: BoxDecoration(
                                      border: Border(
                                        top: BorderSide(
                                          color: Colors.grey.shade300,
                                        ),
                                      ),
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
                  Positioned(
                    top: ageTop * scale,
                    left: 50 * scale,
                    width: 340 * scale,
                    child: AnimatedContainer(
                      duration: const Duration(milliseconds: 300),
                      curve: Curves.easeInOut,
                      height: (isAgeDropdownOpen ? 272 : 75) * scale,
                      decoration: BoxDecoration(
                        color: Colors.white,
                        border: Border.all(
                          color: Colors.black,
                          width: 2 * scale,
                        ),
                        borderRadius: BorderRadius.circular(15 * scale),
                      ),
                      child: ClipRRect(
                        borderRadius: BorderRadius.circular(15 * scale),
                        child: SingleChildScrollView(
                          physics: const NeverScrollableScrollPhysics(),
                          child: Stack(
                            children: [
                              if (isAgeDropdownOpen)
                                Positioned(
                                  right: 25 * scale,
                                  top: 58 * scale,
                                  height: 200 * scale,
                                  child: SvgPicture.asset(
                                    'assets/icons/lines.svg',
                                    fit: BoxFit.fill,
                                  ),
                                ),
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.stretch,
                                children: [
                                  GestureDetector(
                                    onTap: () {
                                      setState(() {
                                        isAgeDropdownOpen = !isAgeDropdownOpen;
                                        if (isAgeDropdownOpen)
                                          isGenderDropdownOpen = false;
                                      });
                                    },
                                    child: Container(
                                      height: 71 * scale,
                                      padding: EdgeInsets.symmetric(
                                        horizontal: 15 * scale,
                                      ),
                                      color: Colors.white,
                                      child: Row(
                                        mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                        children: [
                                          Container(
                                            decoration: BoxDecoration(
                                              border: Border.all(
                                                color: Colors.black,
                                                width: 1.5 * scale,
                                              ),
                                              borderRadius:
                                              BorderRadius.circular(
                                                6 * scale,
                                              ),
                                            ),
                                            width: 30 * scale,
                                            height: 30 * scale,
                                            child: Icon(
                                              isAgeDropdownOpen
                                                  ? Icons.keyboard_arrow_up
                                                  : Icons.keyboard_arrow_down,
                                              size: 24 * scale,
                                            ),
                                          ),
                                          Directionality(
                                            textDirection: TextDirection.rtl,
                                            child: Text(
                                              selectedAge ?? 'الفئة العمرية:',
                                              style: TextStyle(
                                                fontFamily: 'Tajawal',
                                                fontSize: 24 * scale,
                                                fontWeight: FontWeight.w500,
                                                color: selectedAge == null
                                                    ? Colors.black.withOpacity(
                                                  0.5,
                                                )
                                                    : Colors.black,
                                              ),
                                            ),
                                          ),
                                          Image.asset(
                                            'assets/icons/age.png',
                                            width: 30 * scale,
                                            height: 30 * scale,
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                  if (isAgeDropdownOpen)
                                    SizedBox(
                                      height: 197 * scale,
                                      child: Column(
                                        children: ageCategories
                                            .map(
                                              (category) => GestureDetector(
                                            onTap: () => setState(() {
                                              selectedAge = category;
                                              isAgeDropdownOpen = false;
                                            }),
                                            child: Container(
                                              height: 49.25 * scale,
                                              width: double.infinity,
                                              color: Colors.transparent,
                                              alignment:
                                              Alignment.bottomRight,
                                              padding: EdgeInsets.only(
                                                right: 65 * scale,
                                                bottom: 14 * scale,
                                              ),
                                              child: Directionality(
                                                textDirection:
                                                TextDirection.rtl,
                                                child: Text(
                                                  category,
                                                  style: TextStyle(
                                                    fontFamily: 'Tajawal',
                                                    fontSize: 20 * scale,
                                                    fontWeight:
                                                    FontWeight.w500,
                                                    color: Colors.black
                                                        .withOpacity(0.8),
                                                  ),
                                                ),
                                              ),
                                            ),
                                          ),
                                        )
                                            .toList(),
                                      ),
                                    ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    top: dobTop * scale,
                    left: 0,
                    width: 440 * scale,
                    child: Directionality(
                      textDirection: TextDirection.rtl,
                      child: Text(
                        'أدخل تاريخ ميلاده:',
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
                    top: dobTop + 46,
                    left: 290,
                    text: selectedDob != null ? '${selectedDob!.day}' : 'يوم',
                    onTap: () => _selectDate(context),
                  ),
                  _buildDateBox(
                    scale: scale,
                    top: dobTop + 46,
                    left: 170,
                    text: selectedDob != null ? '${selectedDob!.month}' : 'شهر',
                    onTap: () => _selectDate(context),
                  ),
                  _buildDateBox(
                    scale: scale,
                    top: dobTop + 46,
                    left: 50,
                    text: selectedDob != null ? '${selectedDob!.year}' : 'سنة',
                    onTap: () => _selectDate(context),
                  ),
                  ...dynamicFields,
                  Positioned(
                    top: submitButtonTop * scale,
                    left: 109 * scale,
                    width: 222 * scale,
                    height: 60 * scale,
                    child: GestureDetector(
                      onTap: _submitCompanion,
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