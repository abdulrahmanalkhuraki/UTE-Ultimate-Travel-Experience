import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get/get.dart';
import 'dart:math' as math;

import 'package:ute_app/cubit/register_cubit/register_cubit.dart';
import 'package:ute_app/screen/otp_screen.dart';
import 'package:ute_app/utils/constants.dart';



class RegisterScreen extends StatelessWidget {
  RegisterScreen({super.key});

  final TextEditingController _emailController           = TextEditingController();
  final TextEditingController _passwordController        = TextEditingController();
  final TextEditingController _confirmPasswordController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => RegistrCubit(),
      child: BlocConsumer<RegistrCubit, RegistrState>(
        listener: (context, state) {
          if (state is RegistrSuccess) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.green),
            );
            Get.off(() => OTPScreen(email: state.email));
          } else if (state is RegistrFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
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
                child: Column(
                  children: [
                    // ── الجزء العلوي (blob + عنوان + صورة) ──
                    SizedBox(
                      height: 420.h,
                      child: Stack(
                        clipBehavior: Clip.none,
                        children: [
                          // ── Blob الخلفية ──
                          Positioned.fill(
                            child: Transform.rotate(
                              angle: -14.65 * math.pi / 180,
                              child: CustomPaint(
                                painter: BlobPainter(),
                              ),
                            ),
                          ),

                          // ── السهم العلوي (back button) ──
                          Positioned(
                            top: 50.h,
                            left: 20.w,
                            child: Icon(
                              Icons.chevron_right,
                              size: 32.r,
                              color: Colors.black87,
                            ),
                          ),

                          // ── العنوان ──
                          Positioned(
                            top: 80.h,
                            left: 0,
                            right: 0,
                            child: Center(
                              child: Text(
                                'إنشاء حساب',
                                style: TextStyle(
                                  fontSize: 32.sp,
                                  fontWeight: FontWeight.bold,
                                  color: Colors.black87,
                                ),
                              ),
                            ),
                          ),

                          // ── الصورة ──
                          Positioned(
                            top: 120.h,
                            left: 0,
                            right: 0,
                            child: Center(
                              child: Image.asset(
                                'assets/images/register_image.png',
                                width: 300.w,
                                height: 260.h,
                                fit: BoxFit.contain,
                                errorBuilder: (context, error, stackTrace) =>
                                    Icon(Icons.image, size: 120.r, color: Colors.grey),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),

                    // ── الجزء السفلي (الحقول والزر) ──
                    Padding(
                      padding: EdgeInsets.symmetric(horizontal: 25.w),
                      child: Column(
                        children: [
                          SizedBox(height: 24.h),

                          // ── حقل البريد ──
                          AppTextField(
                            controller: _emailController,
                            hint: 'البريد الالكتروني',
                            icon: Icons.email_outlined,
                            keyboardType: TextInputType.emailAddress,
                          ),

                          SizedBox(height: 16.h),

                          // ── حقل كلمة المرور ──
                          AppTextField(
                            controller: _passwordController,
                            hint: 'كلمة المرور',
                            icon: Icons.lock_outline,
                            isPassword: true,
                            obscureText: true,
                          ),

                          SizedBox(height: 16.h),

                          // ── حقل تأكيد كلمة المرور ──
                          AppTextField(
                            controller: _confirmPasswordController,
                            hint: 'تأكيد كلمة المرور',
                            icon: Icons.lock_outline,
                            isPassword: true,
                            obscureText: true,
                          ),

                          SizedBox(height: 36.h),

                          // ── زر الإنشاء ──
                          SizedBox(
                            width: 280.w,
                            height: 55.h,
                            child: ElevatedButton(
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFFF7A56D),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(15.r),
                                ),
                                elevation: 0,
                              ),
                              onPressed: isLoading
                                  ? null
                                  : () {
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
                                            confirmPassword:
                                                _confirmPasswordController.text.trim(),
                                          );
                                    },
                              child: isLoading
                                  ? const CircularProgressIndicator(color: Colors.white)
                                  : Row(
                                      mainAxisAlignment: MainAxisAlignment.center,
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        Text(
                                          'إنشاء',
                                          style: TextStyle(
                                            fontSize: 20.sp,
                                            color: Colors.white,
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                        SizedBox(width: 10.w),
                                        Container(
                                          width: 28.r,
                                          height: 28.r,
                                          decoration: BoxDecoration(
                                            color: Colors.white.withOpacity(0.3),
                                            borderRadius: BorderRadius.circular(8.r),
                                          ),
                                          child: Icon(
                                            Icons.add,
                                            color: Colors.white,
                                            size: 20.r,
                                          ),
                                        ),
                                      ],
                                    ),
                            ),
                          ),

                          SizedBox(height: 30.h),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}