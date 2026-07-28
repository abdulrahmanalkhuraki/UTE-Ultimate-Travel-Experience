import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:image_picker/image_picker.dart';

class CompanyInfoScreen extends StatefulWidget {
  const CompanyInfoScreen({super.key});

  @override
  State<CompanyInfoScreen> createState() => _CompanyInfoScreenState();
}

class _CompanyInfoScreenState extends State<CompanyInfoScreen> {
  final TextEditingController companyNameController = TextEditingController();
  final TextEditingController shortBioController = TextEditingController();
  final TextEditingController locationController = TextEditingController();
  final TextEditingController phoneController = TextEditingController();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController registrationNumberController = TextEditingController();
  final TextEditingController bankAccountController = TextEditingController();
  final TextEditingController aboutCompanyController = TextEditingController();

  DateTime? selectedFoundingDate;

  File? companyLogoImage;
  File? registrationImageFile;

  final ImagePicker _picker = ImagePicker();

  @override
  void dispose() {
    companyNameController.dispose();
    shortBioController.dispose();
    locationController.dispose();
    phoneController.dispose();
    emailController.dispose();
    registrationNumberController.dispose();
    bankAccountController.dispose();
    aboutCompanyController.dispose();
    super.dispose();
  }

  Future<void> _pickImage(String type) async {
    final XFile? pickedFile = await _picker.pickImage(
      source: ImageSource.gallery,
    );
    if (pickedFile != null) {
      setState(() {
        if (type == 'logo') {
          companyLogoImage = File(pickedFile.path);
        } else if (type == 'registration') {
          registrationImageFile = File(pickedFile.path);
        }
      });
    }
  }

  Future<void> _selectFoundingDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime(2010),
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
    if (picked != null && picked != selectedFoundingDate) {
      setState(() => selectedFoundingDate = picked);
    }
  }

  void _submitCompany() {
    if (companyNameController.text.isEmpty || phoneController.text.isEmpty) {
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
          'تم حفظ معلومات الشركة بنجاح!',
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

    const double fieldSpacing = 85.0;
    const double firstFieldTop = 415.0;

    final double companyNameTop = firstFieldTop; // 415
    final double shortBioTop = companyNameTop + fieldSpacing; // 500
    final double locationTop = shortBioTop + fieldSpacing; // 585
    final double phoneTop = locationTop + fieldSpacing; // 670
    final double emailTop = phoneTop + fieldSpacing; // 755

    final double foundingLabelTop = emailTop + fieldSpacing; // 840
    final double foundingBoxTop = 895.0;

    final double registrationNumberTop = foundingBoxTop + 95; // 990
    final double registrationImageTitleTop = registrationNumberTop + 95; // 1085
    final double bankAccountTop = registrationImageTitleTop + 370; // 1455

    // تم تعديل الإحداثيات هنا لتعتمد على الحقل الذي يسبقها لمنع التداخل
    final double aboutCompanyTop = bankAccountTop + 95.0;
    const double aboutCompanyHeight = 300.0;

    final double submitButtonTop = aboutCompanyTop + aboutCompanyHeight + 40;
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
                    top: 99 * scale,
                    left: 41 * scale,
                    width: 357 * scale,
                    height: 45 * scale,
                    child: Center(
                      child: Text(
                        'معلومات الشركة',
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontFamily: 'Cairo',
                          fontWeight: FontWeight.w600,
                          fontSize: 24 * scale,
                          height: 1.0,
                          letterSpacing: 0,
                          color: const Color(0xFF000000),
                        ),
                      ),
                    ),
                  ),
                  Positioned(
                    top: 160 * scale,
                    left: 120 * scale,
                    width: 200 * scale,
                    height: 200 * scale,
                    child: GestureDetector(
                      onTap: () => _pickImage('logo'),
                      child: companyLogoImage != null
                          ? ClipOval(
                        child: Image.file(
                          companyLogoImage!,
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
                          'اللوغو الخاص بالشركة',
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
                    top: companyNameTop,
                    hintText: 'الاسم التجاري:',
                    assetPath: 'assets/icons/Profile_1.svg',
                    isSvg: true,
                    controller: companyNameController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: shortBioTop,
                    hintText: 'نبذة قصيرة عن الشركة:',
                    assetPath: 'assets/icons/Message.svg',
                    isSvg: true,
                    controller: shortBioController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: locationTop,
                    hintText: 'موقع الشركة:',
                    assetPath: 'assets/icons/Location_Add.svg',
                    isSvg: true,
                    controller: locationController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: phoneTop,
                    hintText: 'رقم الهاتف:',
                    assetPath: 'assets/icons/Call.svg',
                    isSvg: true,
                    controller: phoneController,
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: emailTop,
                    hintText: 'البريد الالكتروني:',
                    assetPath: 'assets/icons/mailC.svg',
                    isSvg: true,
                    controller: emailController,
                  ),
                  Positioned(
                    top: foundingLabelTop * scale,
                    left: 0,
                    width: 440 * scale,
                    child: Directionality(
                      textDirection: TextDirection.rtl,
                      child: Text(
                        'تاريخ التأسيس:',
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
                    top: foundingBoxTop,
                    left: 245,
                    text: selectedFoundingDate != null
                        ? '${selectedFoundingDate!.month}'
                        : 'شهر',
                    onTap: () => _selectFoundingDate(context),
                  ),
                  _buildDateBox(
                    scale: scale,
                    top: foundingBoxTop,
                    left: 95,
                    text: selectedFoundingDate != null
                        ? '${selectedFoundingDate!.year}'
                        : 'سنة',
                    onTap: () => _selectFoundingDate(context),
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: registrationNumberTop,
                    hintText: 'رقم السجل السياحي:',
                    assetPath: 'assets/icons/tourism_Card_ID.svg',
                    isSvg: true,
                    controller: registrationNumberController,
                  ),
                  ..._buildImageUploadFrame(
                    scale: scale,
                    top: registrationImageTitleTop,
                    title: 'صورة السجل السياحي:',
                    titleLeft: 130,
                    titleWidth: 250,
                    imageFile: registrationImageFile,
                    onTap: () => _pickImage('registration'),
                  ),
                  ..._buildTextInputField(
                    scale: scale,
                    top: bankAccountTop,
                    hintText: 'حسابك البنكي:',
                    assetPath: 'assets/icons/Card.svg',
                    isSvg: true,
                    controller: bankAccountController,
                  ),
                  _buildAboutCompanyField(
                    scale: scale,
                    top: aboutCompanyTop,
                    height: aboutCompanyHeight,
                    controller: aboutCompanyController,
                  ),
                  Positioned(
                    top: submitButtonTop * scale,
                    left: 109 * scale,
                    width: 222 * scale,
                    height: 60 * scale,
                    child: GestureDetector(
                      onTap: _submitCompany,
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
                  color: selectedFoundingDate != null
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

  Widget _buildAboutCompanyField({
    required double scale,
    required double top,
    required double height,
    required TextEditingController controller,
  }) {
    return Positioned(
      top: top * scale,
      left: 50 * scale,
      width: 340 * scale,
      height: height * scale,
      child: Stack(
        children: [
          Positioned.fill(
            child: SvgPicture.asset(
              'assets/icons/about the company.svg',
              fit: BoxFit.fill,
            ),
          ),
          Positioned(
            top: 40 * scale,
            bottom: 15 * scale,
            left: 15 * scale,
            right: 15 * scale,
            child: Directionality(
              textDirection: TextDirection.rtl,
              child: TextField(
                controller: controller,
                maxLines: null,
                expands: true,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 16 * scale,
                  color: Colors.black,
                ),
                decoration: InputDecoration(
                  hintText:
                  'اكتب التفاصيل المهمة عن الشركة من شأنها أن تجذب '
                      'السائحين والتي سوف تظهر في معلومات الشركة التي '
                      'يستطيع السائحين الوصول إليها.',
                  hintStyle: TextStyle(
                    fontFamily: 'Tajawal',
                    fontSize: 16 * scale,
                    color: Colors.grey.shade500,
                    height: 1.4,
                  ),
                  border: InputBorder.none,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}