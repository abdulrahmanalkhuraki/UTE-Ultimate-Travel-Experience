class RegisterResponse {
  final int? userId;
  final String? email;
  final bool? isEmailVerified;
  final String? message;

  RegisterResponse({
    this.userId,
    this.email,
    this.isEmailVerified,
    this.message,
  });

  factory RegisterResponse.fromJson(Map<String, dynamic> json) {
    return RegisterResponse(
      userId: json['userId'] as int?,
      email: json['email'] as String?,
      isEmailVerified: json['isEmailVerified'] as bool?,
      message: json['message'] as String?,
    );
  }
}

class AuthResponse {
  final int? userId;
  final String? firstName;
  final String? lastName;
  final String? email;
  final String? image;
  final String? dateOfBirth;
  final String? role;
  final bool? isEmailVerified;
  final bool? isProfileCompleted;
  final String? token;
  final String? expiresAt;

  AuthResponse({
    this.userId,
    this.firstName,
    this.lastName,
    this.email,
    this.image,
    this.dateOfBirth,
    this.role,
    this.isEmailVerified,
    this.isProfileCompleted,
    this.token,
    this.expiresAt,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      userId: json['userId'] as int?,
      firstName: json['firstName'] as String?,
      lastName: json['lastName'] as String?,
      email: json['email'] as String?,
      image: json['image'] as String?,
      dateOfBirth: json['dateOfBirth'] as String?,
      role: json['role'] as String?,
      isEmailVerified: json['isEmailVerified'] as bool?,
      isProfileCompleted: json['isProfileCompleted'] as bool?,
      token: json['token'] as String?,
      expiresAt: (json['expiresAt'] ?? json['expiresAtUtc']) as String?,
    );
  }
}

class OtpResponse {
  final String? email;
  final String? expiresAtUtc;
  final String? message;

  OtpResponse({this.email, this.expiresAtUtc, this.message});

  factory OtpResponse.fromJson(Map<String, dynamic> json) {
    return OtpResponse(
      email: json['email'] as String?,
      expiresAtUtc: json['expiresAtUtc'] as String?,
      message: json['message'] as String?,
    );
  }
}