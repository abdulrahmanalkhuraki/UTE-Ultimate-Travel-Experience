import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:get/get.dart';

import 'dart:math' as math;

import 'package:ute_app/cubit/otp_cubit/otp_cubit.dart';
import 'package:ute_app/screen/profile_completion_screen.dart';
import 'package:ute_app/utils/constants.dart';



class OTPScreen extends StatefulWidget {
  final String email;

  const OTPScreen({super.key, required this.email});

  @override
  State<OTPScreen> createState() => _OTPScreenState();
}

class _OTPScreenState extends State<OTPScreen> {
  final List<TextEditingController> _controllers =
      List.generate(6, (_) => TextEditingController());
  final List<FocusNode> _focusNodes = List.generate(6, (_) => FocusNode());

  @override
  void dispose() {
    for (final c in _controllers) {
      c.dispose();
    }
    for (final f in _focusNodes) {
      f.dispose();
    }
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

  void _verify(BuildContext context) {
    if (_code.length < 6) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('أدخل رمز التحقق كاملاً'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }
    context.read<OtpCubit>().verifyOtp(email: widget.email, code: _code);
  }

  void _resend(BuildContext context) {
    for (final c in _controllers) {
      c.clear();
    }
    _focusNodes[0].requestFocus();
    context.read<OtpCubit>().resendOtp(email: widget.email);
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => OtpCubit(),
      child: BlocConsumer<OtpCubit, OtpState>(
        listener: (context, state) {
          if (state is OtpSuccess) {
            Get.off(() => const ProfileCompletionScreen());
          } else if (state is OtpResent) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.green),
            );
          } else if (state is OtpFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          final isLoading = state is OtpLoading;

          return Directionality(
            textDirection: TextDirection.rtl,
            child: Scaffold(
              backgroundColor: Colors.white,
              body: Stack(
                children: [
                  Positioned(
                    top: -150.h,
                    left: -20.w,
                    child: CustomPaint(
                      size: Size(705.w, 678.h),
                      painter: BlobPainter(),
                    ),
                  ),
                  Positioned(
                    top: 600.h,
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
                      padding: EdgeInsets.symmetric(horizontal: 24.w),
                      child: Column(
                        children: [
                          Align(
                            alignment: Alignment.centerLeft,
                            child: IconButton(
                              icon: const Icon(
                                Icons.arrow_forward_ios,
                                color: Colors.black87,
                                size: 20,
                              ),
                              onPressed: () => Navigator.maybePop(context),
                            ),
                          ),
                          SizedBox(height: 8.h),
                          Text(
                            'رمز الأمان',
                            style: TextStyle(
                              fontSize: 30.sp,
                              fontWeight: FontWeight.bold,
                              color: Colors.black87,
                            ),
                          ),
                          SizedBox(height: 24.h),
                          SizedBox(
                            height: 220.h,
                            child: Image.asset(
                              'assets/images/passcode_image.png',
                              fit: BoxFit.contain,
                            ),
                          ),
                          SizedBox(height: 28.h),
                          Text(
                            'ادخل رمز التحقق الذي تم ارساله الى البريد',
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 15.sp,
                              color: Colors.black87,
                              height: 1.5,
                            ),
                          ),
                          SizedBox(height: 4.h),
                          Text(
                            widget.email,
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 15.sp,
                              fontWeight: FontWeight.w600,
                              color: Colors.black87,
                              letterSpacing: 0.5,
                            ),
                          ),
                          SizedBox(height: 32.h),
                          LayoutBuilder(
                            builder: (context, constraints) {
                              const count = 6;
                              const totalSpacing = (count - 1) * 8.0;
                              final boxSize =
                                  ((constraints.maxWidth - totalSpacing) / count)
                                      .clamp(0.0, 52.0);
                              return Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: List.generate(count, (i) {
                                  final index = (count - 1) - i;
                                  return Padding(
                                    padding: EdgeInsets.only(
                                      left: i < count - 1 ? 8.0 : 0,
                                    ),
                                    child: _OTPBox(
                                      size: boxSize,
                                      controller: _controllers[index],
                                      focusNode: _focusNodes[index],
                                      onChanged: (val) => _onChanged(val, index),
                                    ),
                                  );
                                }),
                              );
                            },
                          ),
                          SizedBox(height: 36.h),
                          SizedBox(
                            width: double.infinity,
                            height: 54.h,
                            child: ElevatedButton.icon(
                              onPressed: isLoading ? null : () => _verify(context),
                              icon: const Icon(
                                Icons.verified_user_outlined,
                                color: Colors.white,
                                size: 22,
                              ),
                              label: isLoading
                                  ? const CircularProgressIndicator(color: Colors.white)
                                  : Text(
                                      'تحقق',
                                      style: TextStyle(
                                        fontSize: 18.sp,
                                        fontWeight: FontWeight.bold,
                                        color: Colors.white,
                                      ),
                                    ),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: const Color(0xFFF5A623),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(30.r),
                                ),
                                elevation: 0,
                              ),
                            ),
                          ),
                          SizedBox(height: 32.h),
                          Text(
                            'لم تتلق اي رمز .؟',
                            style: TextStyle(
                              fontSize: 15.sp,
                              color: Colors.black87,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                          SizedBox(height: 20.h),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Flexible(
                                child: TextButton(
                                  onPressed: () {},
                                  child: Text(
                                    'او اطلب المساعدة من فريق الدعم',
                                    textAlign: TextAlign.center,
                                    style: TextStyle(
                                      fontSize: 13.sp,
                                      color: Colors.black54,
                                    ),
                                  ),
                                ),
                              ),
                              TextButton.icon(
                                onPressed: isLoading ? null : () => _resend(context),
                                icon: Icon(
                                  Icons.refresh,
                                  color: const Color(0xFFF5A623),
                                  size: 18.r,
                                ),
                                label: Text(
                                  'إعادة ارسال الرمز',
                                  style: TextStyle(
                                    fontSize: 13.sp,
                                    color: const Color(0xFFF5A623),
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          SizedBox(height: 16.h),
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
}

class _OTPBox extends StatelessWidget {
  final TextEditingController controller;
  final FocusNode focusNode;
  final ValueChanged<String> onChanged;
  final double size;

  const _OTPBox({
    required this.controller,
    required this.focusNode,
    required this.onChanged,
    required this.size,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFDDE8FF), width: 1.5),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Center(
        child: TextField(
          controller: controller,
          focusNode: focusNode,
          onChanged: onChanged,
          textAlign: TextAlign.center,
          keyboardType: TextInputType.number,
          maxLength: 1,
          style: TextStyle(
            fontSize: size * 0.4,
            fontWeight: FontWeight.bold,
            color: Colors.black87,
          ),
          inputFormatters: [FilteringTextInputFormatter.digitsOnly],
          decoration: const InputDecoration(
            border: InputBorder.none,
            counterText: '',
          ),
        ),
      ),
    );
  }
}
