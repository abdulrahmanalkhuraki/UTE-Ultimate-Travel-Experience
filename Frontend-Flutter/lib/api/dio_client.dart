import 'package:dio/dio.dart';
import 'package:ute_app/api/api_serv.dart';

class DioClient {
  DioClient._internal() {
    dio = Dio(
      BaseOptions(
        baseUrl: ApiServ.baseUrl,
        connectTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
        headers: {'Accept': 'application/json'},
      ),
    );

    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          print('🔵 REQUEST: ${options.method} ${options.uri}');
          print('🔵 DATA: ${options.data}');
          final token = ApiServ.authToken;
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onResponse: (response, handler) {
          print('🟢 RESPONSE: ${response.statusCode}');
          print('🟢 DATA: ${response.data}');
          handler.next(response);
        },
        onError: (error, handler) {
          print('🔴 ERROR TYPE: ${error.type}');
          print('🔴 MESSAGE: ${error.message}');
          print('🔴 URL: ${error.requestOptions.uri}');
          print('🔴 RESPONSE: ${error.response?.data}');
          handler.next(error);
        },
      ),
    );
  }

  static final DioClient instance = DioClient._internal();

  late final Dio dio;
}