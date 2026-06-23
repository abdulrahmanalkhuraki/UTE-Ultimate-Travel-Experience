import 'package:flutter/material.dart';
import 'package:flutter_screenutil/flutter_screenutil.dart';
import 'package:get/get.dart';
import 'package:ute_app/api/api_serv.dart';
import 'package:ute_app/screen/onboarding_screen.dart';
import 'package:ute_app/screen/home_screen.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await ApiServ.loadAuthToken();
  runApp(MyApp(isLoggedIn: ApiServ.isLoggedIn));
}

class MyApp extends StatelessWidget {
  final bool isLoggedIn;
  const MyApp({super.key, required this.isLoggedIn});

  @override
  Widget build(BuildContext context) {
    return ScreenUtilInit(
      designSize: const Size(384, 837),
      minTextAdapt: true,
      splitScreenMode: true,
      builder: (context, child) {
        return GetMaterialApp(
          title: 'UTE App',
          debugShowCheckedModeBanner: false,
          theme: ThemeData(
            fontFamily: 'Cairo',
            scaffoldBackgroundColor: Colors.white,
          ),
          home: isLoggedIn ? const HomeScreenProvider() : OnboardingScreen(),
        );
      },
    );
  }
}