import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'app_constants.dart';

class SupportTeam extends StatefulWidget {
  const SupportTeam({super.key});

  @override
  State<SupportTeam> createState() => _SupportTeamState();
}

class _SupportTeamState extends State<SupportTeam> {
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
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
                      top: 42 * context.scale,
                      left: 20 * context.scale,
                      right: 20 * context.scale,
                    ),
                    child: Row(
                      children: [
                        SizedBox(width: 48 * context.scale),
                        Expanded(
                          child: FittedBox(
                            fit: BoxFit.scaleDown,
                            child: CustomHeaderTitle(title: 'فريق الدعم'),
                          ),
                        ),
                        SizedBox(width: 48 * context.scale, child: const CustomBackButton()),
                      ],
                    ),
                  ),
                  Padding(
                    padding: EdgeInsets.only(
                      top: 20 * context.scale,
                      left: 32 * context.scale,
                      right: 32 * context.scale,
                      bottom: 20 * context.scale,
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

                  SizedBox(height: 20 * context.scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * context.scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'عنوان المشكلة:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * context.scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),

                  SizedBox(height: 8 * context.scale),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * context.scale),
                    child: SizedBox(
                      height: 60 * context.scale,
                      child: Stack(
                        children: [
                          SvgPicture.asset(
                            'assets/icons/Rectangle1111.svg',
                            width: double.infinity,
                            height: 60 * context.scale,
                            fit: BoxFit.fill,
                          ),
                          TextField(
                            controller: _titleController,
                            textAlign: TextAlign.right,
                            textDirection: TextDirection.rtl,
                            textAlignVertical: TextAlignVertical.center,
                            style: TextStyle(
                              fontFamily: 'Tajawal',
                              fontSize: 16 * context.scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black87,
                            ),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(
                                horizontal: 16 * context.scale,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),

                  SizedBox(height: 16 * context.scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * context.scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'صف مشكلتك:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * context.scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),

                  SizedBox(height: 8 * context.scale),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * context.scale),
                    child: SizedBox(
                      height: 112 * context.scale,
                      child: Stack(
                        children: [
                          SvgPicture.asset(
                            'assets/icons/Rectangle11114.svg',
                            width: double.infinity,
                            height: 112 * context.scale,
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
                              fontSize: 16 * context.scale,
                              fontWeight: FontWeight.w400,
                              color: Colors.black87,
                            ),
                            decoration: InputDecoration(
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(
                                horizontal: 16 * context.scale,
                                vertical: 12 * context.scale,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),

                  SizedBox(height: 16 * context.scale),
                  Padding(
                    padding: EdgeInsets.only(right: 22 * context.scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'يمكنك أيضاً إرفاق صورة عن المشكلة:',
                        textDirection: TextDirection.rtl,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 32 * context.scale,
                          fontWeight: FontWeight.w400,
                          color: Colors.black,
                          height: 1.0,
                        ),
                      ),
                    ),
                  ),

                  SizedBox(height: 12 * context.scale),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 22 * context.scale),
                    child: Align(
                      alignment: Alignment.centerRight,
                      child: SizedBox(
                        width: 340 * context.scale,
                        height: 259 * context.scale,
                        child: Stack(
                          children: [
                            SvgPicture.asset(
                              'assets/icons/picFrame.svg',
                              width: 340 * context.scale,
                              height: 259 * context.scale,
                              fit: BoxFit.fill,
                            ),
                          /*  Center(
                              child: SvgPicture.asset(
                                'assets/icons/Add.svg',
                                width: 50 * context.scale,
                                height: 50 * context.scale,
                              ),
                            ),*/
                          ],
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 20 * context.scale),
                  Center(
                    child: GestureDetector(
                      onTap: () {

                        //  إرسال الطلب

                      },
                      child: Container(
                        width: 270 * context.scale,
                        height: 65 * context.scale,
                        decoration: BoxDecoration(
                          color: const Color(0xFFF4A261),
                          borderRadius: BorderRadius.circular(20 * context.scale),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Text(
                              'إرسال الطلب',
                              textDirection: TextDirection.rtl,
                              style: TextStyle(
                                fontFamily: 'Tajawal',
                                fontSize: 36 * context.scale,
                                fontWeight: FontWeight.w400,
                                color: Colors.black,
                                height: 1.0,
                              ),
                            ),

                            SizedBox(width: 8 * context.scale),
                            Image.asset(
                              'assets/icons/SendRequest.png',
                              width: 40 * context.scale,
                              height: 40 * context.scale,
                              fit: BoxFit.contain,
                            ),

                          ],
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: 30 * context.scale),

                ],
              ),
            ),

          ],
        ),
      ),
    );
  }
}