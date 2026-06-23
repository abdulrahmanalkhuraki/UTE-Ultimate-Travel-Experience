import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get/get.dart';
import 'dart:math' as math;

import 'package:ute_app/cubit/login_cubit/login_cubit.dart';
import 'package:ute_app/screen/forgot_password_scrren.dart';
import 'package:ute_app/screen/home_screen.dart';
import 'package:ute_app/screen/register_screen.dart';
import 'package:ute_app/utils/constants.dart';



class LoginScreen extends StatelessWidget {
  LoginScreen({super.key});

  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => LoginCubit(),
      child: BlocConsumer<LoginCubit, LoginState>(
        listener: (context, state) {
          if (state is LoginSuccess) {
            Get.off(() => const HomeScreenProvider());
          } else if (state is LoginFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: Colors.red,
              ),
            );
          }
        },
        builder: (context, state) {
          final isLoading = state is LoginLoading;
          final obscure = switch (state) {
            LoginInitial() => state.obscurePassword,
            LoginLoading() => state.obscurePassword,
            LoginFailure() => state.obscurePassword,
            _ => true,
          };

          return Scaffold(
            backgroundColor: Colors.white,
            body: Directionality(
              textDirection: TextDirection.rtl,
              child: Stack(
                children: [
                  // Blob أعلى
                  Positioned(
                    top: -120.h,
                    left: -20.w,
                    child: CustomPaint(
                      size: Size(705.w, 678.h),
                      painter: BlobPainter(),
                    ),
                  ),
                  // Blob أسفل
                  Positioned(
                    top: 574.h,
                    left: -100.w,
                    child: Transform.rotate(
                      angle: -14.64 * (math.pi / 180),
                      child: CustomPaint(
                        size: Size(705.w, 678.h),
                        painter: BlobPainter(),
                      ),
                    ),
                  ),

                  SingleChildScrollView(
                    child: Column(
                      children: [
                        _buildHeader(),
                        Padding(
                          padding: EdgeInsets.symmetric(
                            horizontal: 24.w,
                            vertical: 20.h,
                          ),
                          child: Column(
                            children: [
                              // ── البريد ──
                              AppTextField(
                                controller: _emailController,
                                hint: 'البريد الالكتروني',
                                icon: Icons.email_outlined,
                                keyboardType: TextInputType.emailAddress,
                              ),
                              SizedBox(height: 16.h),
                              // ── كلمة المرور ──
                              AppTextField(
                                controller: _passwordController,
                                hint: 'كلمة المرور',
                                icon: Icons.lock_outline,
                                isPassword: true,
                                obscureText: obscure,

                                onToggleObscure: () => context
                                    .read<LoginCubit>()
                                    .toggleObscurePassword(),
                              ),
                              SizedBox(height: 16.h),
                              Align(
                                alignment: Alignment.center,
                                child: GestureDetector(
                                  onTap: () {
                                    final email = _emailController.text.trim();
                                    if (email.isEmpty) {
                                      ScaffoldMessenger.of(context).showSnackBar(
                                        const SnackBar(
                                          content: Text('أدخل بريدك الإلكتروني أولاً'),
                                          backgroundColor: Colors.red,
                                        ),
                                      );
                                      return;
                                    }
                                    Get.to(() => ForgotPasswordScreen(email: email));
                                  },
                                  child: Text(
                                    'نسيت كلمة المرور؟',
                                    style: TextStyle(
                                      fontSize: 14.sp,
                                      color: Color(0xFFF5A623),
                                      fontWeight: FontWeight.bold,
                                    ),
                                  ),
                                ),
                              ),
                              SizedBox(height: 28.h),
                              _buildLoginButton(context, isLoading),
                            ],
                          ),
                        ),
                        _buildBottomSection(),
                        SizedBox(height: 50.h),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildHeader() {
    return SizedBox(
      height: 350.h,
      width: double.infinity,
      child: Column(
        children: [
          SizedBox(height: 80.h),
          Text(
            'تسجيل الدخول',
            style: TextStyle(
              fontSize: 32.sp,
              fontWeight: FontWeight.bold,
              color: Colors.black,
            ),
          ),
          SizedBox(height: 20.h),
          Expanded(
            child: Image.asset(
              'assets/images/login_image.png',
              width: 300.w,
              height: 300.h,
              fit: BoxFit.contain,
              errorBuilder: (context, error, stackTrace) =>
                  Icon(Icons.image, size: 120.r, color: Colors.grey),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLoginButton(BuildContext context, bool isLoading) {
    return SizedBox(
      width: 290.w,
      height: 55.h,
      child: ElevatedButton(
        onPressed: isLoading
            ? null
            : () => context.read<LoginCubit>().login(
                email: _emailController.text.trim(),
                password: _passwordController.text.trim(),
              ),
        style: ElevatedButton.styleFrom(
          backgroundColor: const Color(0xFFF4A261),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(15.r),
          ),
        ),
        child: isLoading
            ? const CircularProgressIndicator(color: Colors.white)
            : Text(
                'تسجيل الدخول',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 20.sp,
                  fontWeight: FontWeight.bold,
                ),
              ),
      ),
    );
  }

  Widget _buildBottomSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(height: 5.h),
        Padding(
          padding: EdgeInsets.only(right: 65.w),
          child: Text(
            'أو سجل الدخول عبر',
            style: TextStyle(fontSize: 16.sp, color: Colors.black54),
          ),
        ),
        SizedBox(height: 13.h),
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            SizedBox(width: 65.w),
            _socialIcon(Icons.g_mobiledata, Colors.red),
            SizedBox(width: 15.w),
            _socialIcon(Icons.facebook, Colors.blue),
            SizedBox(width: 15.w),
            _socialIcon(Icons.apple, Colors.black),
          ],
        ),
        SizedBox(height: 20.h),
        Row(
          mainAxisAlignment: MainAxisAlignment.start,
          children: [
            Padding(
              padding: EdgeInsets.only(right: 65.w),
              child: Text(
                'أليس لديك حساب؟ ',
                style: TextStyle(fontSize: 14.sp),
              ),
            ),
            GestureDetector(
              onTap: () => Get.to(() => RegisterScreen()),
              child: Text(
                'انشاء حساب',
                style: TextStyle(
                  fontSize: 14.sp,
                  color: const Color(0xFFF5A623),
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _socialIcon(IconData icon, Color color) {
    return Container(
      padding: EdgeInsets.all(8.r),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: Colors.grey.shade300),
        borderRadius: BorderRadius.circular(12.r),
      ),
      child: Icon(icon, size: 35.r, color: color),
    );
  }
}
