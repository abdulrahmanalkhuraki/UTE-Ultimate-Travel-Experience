import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get/get.dart';
import 'dart:math';

import 'package:ute_app/cubit/register_cubit/register_cubit.dart';
import 'package:ute_app/screen/home_screen.dart';
class RegisterScreen extends StatelessWidget {
  RegisterScreen({super.key});

  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _phoneController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => RegistrCubit(),
      child: BlocConsumer<RegistrCubit, RegistrState>(
        listener: (context, state) {
          if (state is RegistrSuccess) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: Colors.green,
              ),
            );
            Get.off(() => HomeScreen());
          } else if (state is RegistrFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: Colors.red,
              ),
            );
          }
        },
        builder: (context, state) {
          final isLoading = state is RegistrLoading;

          return Scaffold(
            backgroundColor: Colors.white,
            body: Directionality(
              textDirection: TextDirection.rtl,
              child: SingleChildScrollView(
                child: SizedBox(
                  height: 1000.h,
                  child: Stack(
                    children: [

                      /// Blob الخلفية
                      Positioned(
                        top: -50.h,
                        left: -120.w,
                        child: Transform.rotate(
                          angle: -14.65 * pi / 180,
                          child: CustomPaint(
                            size: Size(705.w, 678.h),
                            painter: _BlobPainter(),
                          ),
                        ),
                      ),

                      /// العنوان
                      Positioned(
                        top: 100.h,
                        right: 0,
                        left: -40.w,
                        child: Center(
                          child: Text(
                            'إنشاء حساب',
                            style: TextStyle(
                              fontSize: 32.sp,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ),

                      /// الصورة
                      Positioned(
                        top: 132.h,
                        left: 10.w,
                        child: Image.asset(
                          'assets/images/register_image.png',
                          width: 380.w,
                          height: 280.h,
                          fit: BoxFit.contain,
                          errorBuilder: (context, error, stackTrace) =>
                              Icon(Icons.image, size: 120.r, color: Colors.grey),
                        ),
                      ),

                      /// حقل البريد
                      _buildPositionedField(
                        top: 410.h,
                        hint: 'البريد الالكتروني',
                        icon: Icons.email_outlined,
                        controller: _emailController,
                        keyboardType: TextInputType.emailAddress,
                      ),

                      /// حقل الهاتف
                      _buildPositionedField(
                        top: 500.h,
                        hint: 'رقم الهاتف',
                        icon: Icons.phone_android_outlined,
                        controller: _phoneController,
                        keyboardType: TextInputType.phone,
                      ),

                      /// حقل كلمة المرور
                      _buildPositionedField(
                        top: 590.h,
                        hint: 'كلمة المرور',
                        icon: Icons.lock_outline,
                        controller: _passwordController,
                        isPass: true,
                      ),

                      /// حقل تأكيد كلمة المرور
                      _buildPositionedField(
                        top: 680.h,
                        hint: 'تأكيد كلمة المرور',
                        icon: Icons.lock_outline,
                        controller: _confirmPasswordController,
                        isPass: true,
                      ),

                      /// زر الإنشاء
                      Positioned(
                        top: 800.h,
                        left: 0,
                        right: 0,
                        child: Center(
                          child: SizedBox(
                            width: 280.w,
                            height: 55.h,
                            child: ElevatedButton(
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFFF7A56D),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(15.r),
                                ),
                              ),
                              onPressed: isLoading
                                  ? null
                                  : () {
                                      // تحقق من تطابق كلمة المرور أولاً
                                      if (_passwordController.text !=
                                          _confirmPasswordController.text) {
                                        ScaffoldMessenger.of(context).showSnackBar(
                                          const SnackBar(
                                            content: Text('كلمتا المرور غير متطابقتين'),
                                            backgroundColor: Colors.red,
                                          ),
                                        );
                                        return;
                                      }
                                      context.read<RegistrCubit>().register(
                                            email: _emailController.text.trim(),
                                            password: _passwordController.text.trim(),
                                          );
                                    },
                              child: isLoading
                                  ? const CircularProgressIndicator(color: Colors.white)
                                  : Text(
                                      'إنشاء',
                                      style: TextStyle(
                                        fontSize: 20.sp,
                                        color: Colors.white,
                                      ),
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
          );
        },
      ),
    );
  }

  Widget _buildPositionedField({
    required double top,
    required String hint,
    required IconData icon,
    required TextEditingController controller,
    bool isPass = false,
    TextInputType? keyboardType,
  }) {
    return Positioned(
      top: top,
      left: 25.w,
      child: Container(
        width: 340.w,
        height: 75.h,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(15.r),
          border: Border.all(color: Colors.black, width: 2),
        ),
        child: TextField(
          controller: controller,
          obscureText: isPass,
          keyboardType: keyboardType,
          textAlign: TextAlign.right,
          decoration: InputDecoration(
            hintText: hint,
            suffixIcon: Icon(icon, color: Colors.grey),
            border: InputBorder.none,
            contentPadding: EdgeInsets.all(20.r),
          ),
        ),
      ),
    );
  }
}

/// Blob Painter
class _BlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = const Color(0x8091B3FA)
      ..style = PaintingStyle.fill;

    final path = Path();
    path.moveTo(size.width * 0.4, size.height * 0.1);
    path.cubicTo(
      size.width * 0.9, 0,
      size.width * 1.1, size.height * 0.5,
      size.width * 0.7, size.height * 0.9,
    );
    path.cubicTo(
      size.width * 0.3, size.height * 1.1,
      -size.width * 0.2, size.height * 0.7,
      size.width * 0.1, size.height * 0.3,
    );
    path.close();

    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}