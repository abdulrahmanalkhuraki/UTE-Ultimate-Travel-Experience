import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:get/get.dart';
import 'dart:math' as math;
import 'package:ute_app/cubit/forgot_cubit/forgot_password_cubit.dart';
import 'package:ute_app/screen/login_screen.dart';
import 'package:ute_app/utils/constants.dart';

class ForgotPasswordScreen extends StatefulWidget {
  final String email;
  const ForgotPasswordScreen({super.key, required this.email});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final List<TextEditingController> _controllers = List.generate(
    6,
    (_) => TextEditingController(),
  );
  final List<FocusNode> _focusNodes = List.generate(6, (_) => FocusNode());
  final TextEditingController _newPasswordController = TextEditingController();
  final TextEditingController _confirmPasswordController = TextEditingController();

  @override
  void dispose() {
    for (final c in _controllers) {
      c.dispose();
    }
    for (final f in _focusNodes) {
      f.dispose();
    }
    _newPasswordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  void _onChanged(String value, int index) {
    if (value.length == 1 && index < 5) {
      _focusNodes[index + 1].requestFocus();
    } else if (value.isEmpty && index > 0) {
      _focusNodes[index - 1].requestFocus();
    }
  }

  String get _code => _controllers.map((c) => c.text).join();

  void _resetPassword(BuildContext context) {
    if (_code.length < 4) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('أدخل رمز التحقق كاملاً'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }
    if (_newPasswordController.text.trim().isEmpty ||
        _confirmPasswordController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('أدخل كلمة المرور الجديدة'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }
    if (_newPasswordController.text != _confirmPasswordController.text) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('كلمتا المرور غير متطابقتين'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }
    context.read<ForgotPasswordCubit>().resetPassword(
          email: widget.email,
          code: _code,
          newPassword: _newPasswordController.text.trim(),
          confirmPassword: _confirmPasswordController.text.trim(),
        );
  }

  void _resend(BuildContext context) {
    for (final c in _controllers) {
      c.clear();
    }
    _focusNodes[0].requestFocus();
    context.read<ForgotPasswordCubit>().sendForgotPassword(email: widget.email);
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => ForgotPasswordCubit()..sendForgotPassword(email: widget.email),
      child: BlocConsumer<ForgotPasswordCubit, ForgotPasswordState>(
        listener: (context, state) {
          if (state is ForgotPasswordCodeSent) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.green),
            );
          } else if (state is ForgotPasswordResetSuccess) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('تم تغيير كلمة المرور بنجاح'),
                backgroundColor: Colors.green,
              ),
            );
            Get.offAll(() => LoginScreen());
          } else if (state is ForgotPasswordFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.errorMessage),
                backgroundColor: Colors.red,
              ),
            );
          }
        },
        builder: (context, state) {
          final isLoading = state is ForgotPasswordLoading;

          return Scaffold(
            backgroundColor: Colors.white,
            body: Directionality(
              textDirection: TextDirection.rtl,
              child: Stack(
                children: [
                  Positioned(
                    top: -120.h,
                    left: -20.w,
                    child: CustomPaint(
                      size: Size(705.w, 678.h),
                      painter: BlobPainter(),
                    ),
                  ),
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
                  SafeArea(
                    child: SingleChildScrollView(
                      child: Column(
                        children: [
                          Align(
                            alignment: Alignment.centerLeft,
                            child: IconButton(
                              icon: const Icon(Icons.chevron_right),
                              onPressed: () => Get.back(),
                            ),
                          ),
                          SizedBox(height: 10.h),
                          Text(
                            'إعادة تعيين كلمة المرور',
                            style: TextStyle(
                              fontSize: 28.sp,
                              fontWeight: FontWeight.bold,
                              color: Colors.black,
                            ),
                          ),
                          SizedBox(height: 16.h),
                          Image.asset(
                            'assets/images/passcode_image.png',
                            width: 260.w,
                            height: 220.h,
                            fit: BoxFit.contain,
                          ),
                          SizedBox(height: 24.h),
                          Padding(
                            padding: EdgeInsets.symmetric(horizontal: 30.w),
                            child: Column(
                              children: [
                                Text(
                                  'ادخل رمز التحقق الذي تم ارساله الى البريد',
                                  textAlign: TextAlign.center,
                                  style: TextStyle(
                                    fontSize: 14.sp,
                                    color: Colors.black87,
                                  ),
                                ),
                                SizedBox(height: 4.h),
                                Text(
                                  widget.email,
                                  style: TextStyle(
                                    fontSize: 14.sp,
                                    fontWeight: FontWeight.bold,
                                    color: Colors.black,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          SizedBox(height: 28.h),
                          Padding(
                            padding: EdgeInsets.symmetric(horizontal: 20.w),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                              children: List.generate(6, (i) => _buildOtpBox(i)),
                            ),
                          ),
                          SizedBox(height: 24.h),
                          Padding(
                            padding: EdgeInsets.symmetric(horizontal: 25.w),
                            child: Column(
                              children: [
                                AppTextField(
                                  controller: _newPasswordController,
                                  hint: 'كلمة المرور الجديدة',
                                  icon: Icons.lock_outline,
                                  isPassword: true,
                                  obscureText: true,
                                ),
                                SizedBox(height: 16.h),
                                AppTextField(
                                  controller: _confirmPasswordController,
                                  hint: 'تأكيد كلمة المرور',
                                  icon: Icons.lock_outline,
                                  isPassword: true,
                                  obscureText: true,
                                ),
                              ],
                            ),
                          ),
                          SizedBox(height: 32.h),
                          SizedBox(
                            width: 300.w,
                            height: 55.h,
                            child: ElevatedButton.icon(
                              onPressed: isLoading ? null : () => _resetPassword(context),
                              icon: const Icon(
                                Icons.verified_outlined,
                                color: Colors.white,
                              ),
                              label: isLoading
                                  ? const CircularProgressIndicator(color: Colors.white)
                                  : Text(
                                      'تأكيد',
                                      style: TextStyle(
                                        color: Colors.white,
                                        fontSize: 20.sp,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFFF4A261),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(15.r),
                                ),
                              ),
                            ),
                          ),
                          SizedBox(height: 30.h),
                          Text(
                            'لم تتلق اي رمز .؟',
                            style: TextStyle(fontSize: 14.sp, color: Colors.black87),
                          ),
                          SizedBox(height: 12.h),
                          Padding(
                            padding: EdgeInsets.symmetric(horizontal: 30.w),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  'او اطلب المساعدة من فريق\nالدعم',
                                  style: TextStyle(
                                    fontSize: 13.sp,
                                    color: Colors.black54,
                                  ),
                                ),
                                GestureDetector(
                                  onTap: isLoading ? null : () => _resend(context),
                                  child: Column(
                                    children: [
                                      Text(
                                        'إعادة ارسال الرمز',
                                        style: TextStyle(
                                          fontSize: 13.sp,
                                          color: const Color(0xFFF5A623),
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                      Icon(
                                        Icons.refresh,
                                        color: const Color(0xFFF5A623),
                                        size: 18.r,
                                      ),
                                    ],
                                  ),
                                ),
                              ],
                            ),
                          ),
                          SizedBox(height: 40.h),
                        ],
                      ),
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

  Widget _buildOtpBox(int index) {
    return SizedBox(
      width: 48.w,
      height: 56.h,
      child: TextFormField(
        controller: _controllers[index],
        focusNode: _focusNodes[index],
        textAlign: TextAlign.center,
        keyboardType: TextInputType.number,
        inputFormatters: [
          FilteringTextInputFormatter.digitsOnly,
          LengthLimitingTextInputFormatter(1),
        ],
        style: TextStyle(fontSize: 22.sp, fontWeight: FontWeight.bold),
        decoration: InputDecoration(
          contentPadding: EdgeInsets.zero,
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12.r),
            borderSide: BorderSide(color: Colors.grey.shade300, width: 1.5),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12.r),
            borderSide: const BorderSide(color: Color(0xFFF4A261), width: 2),
          ),
        ),
        onChanged: (v) => _onChanged(v, index),
      ),
    );
  }
}
