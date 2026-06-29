// ════════════════════════════════════════════════════════
// نماذج البيانات – ستُملأ من الباك إند
// ════════════════════════════════════════════════════════

/// بيانات رحلة "الحالية"
class CurrentTripModel {
  final String tripDaysAgo;
  final String countryName;
  final String tripRoute;
  final String passengerNames;
  final String bookingNumber;
  final int daysToRegistrationEnd;
  final int daysToStart;
  final int currentTourists;
  final int maxTourists;
  final int starCount;
  final String? tripImagePath;

  const CurrentTripModel({
    required this.tripDaysAgo,
    required this.countryName,
    required this.tripRoute,
    required this.passengerNames,
    required this.bookingNumber,
    required this.daysToRegistrationEnd,
    required this.daysToStart,
    required this.currentTourists,
    required this.maxTourists,
    this.starCount = 5,
    this.tripImagePath,
  });
}

/// بيانات رحلة "السابقة"
class PastTripModel {
  final String timeAgo;
  final String countryName;
  final String tripRoute;
  final String passengerNames;
  final String joinDate;
  final String duration;
  final String costLabel;
  final String costAmount;
  final int starCount;
  final String? tripImagePath;

  const PastTripModel({
    required this.timeAgo,
    required this.countryName,
    required this.tripRoute,
    required this.passengerNames,
    required this.joinDate,
    required this.duration,
    required this.costLabel,
    required this.costAmount,
    this.starCount = 5,
    this.tripImagePath,
  });
}
