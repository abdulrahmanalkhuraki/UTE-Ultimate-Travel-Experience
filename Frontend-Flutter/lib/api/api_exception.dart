import 'package:dio/dio.dart';

class ApiException implements Exception {
  final String message;
  final int? statusCode;

  ApiException({required this.message, this.statusCode});

  @override
  String toString() => message;

  static ApiException fromDioException(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = _extractMessage(e.response?.data) ?? _defaultMessage(statusCode);

    return ApiException(message: message, statusCode: statusCode);
  }

  static String? _extractMessage(dynamic data) {
    if (data == null) return null;
    if (data is String && data.isNotEmpty) return data;

    if (data is Map) {
      for (final key in ['message', 'Message', 'title', 'Title']) {
        final value = data[key];
        if (value != null && value.toString().isNotEmpty) {
          return value.toString();
        }
      }

      final errors = data['errors'];
      if (errors is Map) {
        for (final entry in errors.entries) {
          final value = entry.value;
          if (value is List && value.isNotEmpty) {
            return value.first.toString();
          }
          if (value != null && value.toString().isNotEmpty) {
            return value.toString();
          }
        }
      }
    }

    return null;
  }

  static String _defaultMessage(int? statusCode) {
    return switch (statusCode) {
      400 => 'طلب غير صالح. تحقق من البيانات المدخلة.',
      401 => 'غير مصرح. تحقق من بيانات الدخول.',
      403 => 'البريد الإلكتروني غير مُفعَّل أو غير مصرح بالوصول.',
      404 => 'البيانات المطلوبة غير موجودة.',
      409 => 'هذه العملية غير متاحة أو تم تنفيذها مسبقاً.',
      500 => 'خطأ في الخادم. حاول لاحقاً.',
      _ => 'حدث خطأ في الاتصال. حاول مرة أخرى.',
    };
  }
}
