import 'dart:io';

class UserProfileModel {
  final String name;
  final int age;
  final String gender;
  final String phone;
  final String email;
  final String currentLocation;
  final String residence;
  final String nationalId;
  final String passportNumber;
  final String cardNumber;
  final String? idImageFront;  // وجه الهوية الأمامي
  final String? idImageBack;   // وجه الهوية الخلفي
  final String? passportImageFront; // وجه الجواز الأمامي
  final String? passportImageBack;

  final String joinDate;
  final String programCount;
  final String companiesCount;
  final String tripsCount;
  final String accompanierCount;
  final String spentAmount;

  final String? registrationDate;
  final String? lastTrip;
  final String? lastTripDate;
  final String? wifePrograms;
  final String? wifeAccompanierCount;
  final String? wifeSpentAmount;

  UserProfileModel({
    required this.name,
    required this.age,
    required this.gender,
    required this.phone,
    required this.email,
    required this.currentLocation,
    required this.residence,
    required this.nationalId,
    required this.passportNumber,
    required this.cardNumber,
    required this.joinDate,
    required this.programCount,
    required this.companiesCount,
    required this.tripsCount,
    required this.accompanierCount,
    required this.spentAmount,
    this.idImageFront,
    this.idImageBack,
    this.passportImageFront,
    this.passportImageBack,
    this.registrationDate,
    this.lastTrip,
    this.lastTripDate,
    this.wifePrograms,
    this.wifeAccompanierCount,
    this.wifeSpentAmount,


  });

  String get genderIconPath {
    return gender.toLowerCase() == 'female'
        ? 'assets/icons/female.png'
        : 'assets/icons/male.png';
  }
}