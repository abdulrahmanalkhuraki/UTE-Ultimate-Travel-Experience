import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get/get.dart';
import 'dart:math' as math;

import 'package:ute_app/cubit/login_cubit/login_cubit.dart';
import 'package:ute_app/screen/home_screen.dart';
import 'package:ute_app/screen/register_screen.dart';



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
            Get.off(() => HomeScreen());
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
                  // Blob أعلى اليسار
                  Positioned(
                    top: -120.h,
                    left: -20.w,
                    child: CustomPaint(
                      size: Size(705.w, 678.h),
                      painter: _BlobPainter(),
                    ),
                  ),
                  // Blob أسفل اليسار (مدوّر)
                  Positioned(
                    top: 574.h,
                    left: -100.w,
                    child: Transform.rotate(
                      angle: -14.64 * (math.pi / 180),
                      child: CustomPaint(
                        size: Size(705.w, 678.h),
                        painter: _BlobPainter(),
                      ),
                    ),
                  ),
                  SingleChildScrollView(
                    child: Column(
                      children: [
                        _buildHeader(),
                        Padding(
                          padding: EdgeInsets.symmetric(
                              horizontal: 24.w, vertical: 20.h),
                          child: Column(
                            children: [
                              _buildTextField(
                                controller: _emailController,
                                hint: 'البريد الالكتروني',
                                icon: Icons.email_outlined,
                                keyboardType: TextInputType.emailAddress,
                              ),
                              SizedBox(height: 16.h),
                              _buildPasswordField(context, obscure),
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

  // ── Header ──
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

  // ── Email Field ──
  Widget _buildTextField({
    required TextEditingController controller,
    required String hint,
    required IconData icon,
    TextInputType? keyboardType,
  }) {
    return Container(
      width: 340.w,
      height: 65.h,
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.9),
        borderRadius: BorderRadius.circular(15.r),
        border: Border.all(color: Colors.black, width: 1.5),
      ),
      child: TextField(
        controller: controller,
        keyboardType: keyboardType,
        decoration: InputDecoration(
          hintText: hint,
          prefixIcon: Icon(icon, color: Colors.grey),
          border: InputBorder.none,
          contentPadding:
              EdgeInsets.symmetric(vertical: 18.h, horizontal: 16.w),
        ),
      ),
    );
  }

  // ── Password Field ──
  Widget _buildPasswordField(BuildContext context, bool obscure) {
    return Container(
      width: 340.w,
      height: 65.h,
      decoration: BoxDecoration(
        color: Colors.white.withOpacity(0.9),
        borderRadius: BorderRadius.circular(15.r),
        border: Border.all(color: Colors.black, width: 1.5),
      ),
      child: TextField(
        controller: _passwordController,
        obscureText: obscure,
        decoration: InputDecoration(
          hintText: 'كلمة المرور',
          prefixIcon: const Icon(Icons.lock_outline, color: Colors.grey),
          suffixIcon: IconButton(
            icon: Icon(obscure ? Icons.visibility_off : Icons.visibility),
            onPressed: () =>
                context.read<LoginCubit>().toggleObscurePassword(),
          ),
          border: InputBorder.none,
          contentPadding:
              EdgeInsets.symmetric(vertical: 18.h, horizontal: 16.w),
        ),
      ),
    );
  }

  // ── Login Button ──
  Widget _buildLoginButton(BuildContext context, bool isLoading) {
    return SizedBox(
      width: 300.w,
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

  // ── Bottom Section ──
  Widget _buildBottomSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(height: 20.h),
        Padding(
          padding: EdgeInsets.only(right: 65.w),
          child: Text(
            'أو سجل الدخول عبر',
            style: TextStyle(fontSize: 16.sp, color: Colors.black54),
          ),
        ),
        SizedBox(height: 15.h),
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
        SizedBox(height: 30.h),
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

  // ── Social Icon ──
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

// ── Blob Painter (محدّث حسب SVG الأصلي) ──
class _BlobPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = const Color(0x8091B3FA)
      ..style = PaintingStyle.fill;

    // الـ bounding box للـ SVG الأصلي:
    // x: من -68.11 إلى 623.96  → عرض ≈ 692.07
    // y: من -22.71 إلى 701.10  → ارتفاع ≈ 723.81
    const double originX = 68.1119;
    const double originY = 22.7061;
    const double bboxW   = 692.07;
    const double bboxH   = 723.81;

    final double sx = size.width  / bboxW;
    final double sy = size.height / bboxH;

    // تحويل نقطة من إحداثيات SVG إلى إحداثيات Canvas
    Offset p(double x, double y) =>
        Offset((x + originX) * sx, (y + originY) * sy);

    final path = Path();

    path.moveTo(p(361.121, 20.1921).dx, p(361.121, 20.1921).dy);

    path.cubicTo(
      p(439.838,  60.0738).dx, p(439.838,  60.0738).dy,
      p(424.442, 173.450 ).dx, p(424.442, 173.450 ).dy,
      p(468.624, 249.307 ).dx, p(468.624, 249.307 ).dy,
    );

    path.cubicTo(
      p(512.056, 323.874).dx, p(512.056, 323.874).dy,
      p(623.965, 367.438).dx, p(623.965, 367.438).dy,
      p(612.950, 452.555).dx, p(612.950, 452.555).dy,
    );

    path.cubicTo(
      p(601.791, 538.790).dx, p(601.791, 538.790).dy,
      p(498.866, 575.522).dx, p(498.866, 575.522).dy,
      p(421.201, 615.782).dx, p(421.201, 615.782).dy,
    );

    path.cubicTo(
      p(348.502, 653.468).dx, p(348.502, 653.468).dy,
      p(267.619, 701.103).dx, p(267.619, 701.103).dy,
      p(190.895, 671.343).dx, p(190.895, 671.343).dy,
    );

    path.cubicTo(
      p(117.311, 642.801).dx, p(117.311, 642.801).dy,
      p(112.674, 545.621).dx, p(112.674, 545.621).dy,
      p( 70.139, 479.610).dx, p( 70.139, 479.610).dy,
    );

    path.cubicTo(
      p( 24.294, 408.462).dx, p( 24.294, 408.462).dy,
      p(-68.112, 359.618).dx, p(-68.112, 359.618).dy,
      p(-64.140, 275.548).dx, p(-64.140, 275.548).dy,
    );

    path.cubicTo(
      p(-59.777, 183.193).dx, p(-59.777, 183.193).dy,
      p( 10.417, 105.355).dx, p( 10.417, 105.355).dy,
      p( 90.081,  57.519).dx, p( 90.081,  57.519).dy,
    );

    path.cubicTo(
      p(171.025,   8.9147).dx, p(171.025,   8.9147).dy,
      p(276.450, -22.7061).dx, p(276.450, -22.7061).dy,
      p(361.121,  20.1921).dx, p(361.121,  20.1921).dy,
    );

    path.close();
    canvas.drawPath(path, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}