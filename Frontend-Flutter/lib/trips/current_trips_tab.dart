import 'package:flutter/material.dart';
import '../app_constants.dart';
import 'trip_models.dart';
import 'trip_shared_widgets.dart';

// ════════════════════════════════════════════════════════
// تبويب "الحالية" – يستقبل قائمة رحلات من الباك إند
// ════════════════════════════════════════════════════════
class CurrentTripsTab extends StatelessWidget {
  final List<CurrentTripModel> trips;

  const CurrentTripsTab({super.key, required this.trips});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Column(
        children: [
          ...trips.map((trip) => _CurrentTripCard(trip: trip)),
          SizedBox(height: 20 * context.scale),
        ],
      ),
    );
  }
}

class _CurrentTripCard extends StatelessWidget {
  final CurrentTripModel trip;
  const _CurrentTripCard({required this.trip});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(
        top: 50 * context.scale,
        left: 15 * context.scale,
        right: 15 * context.scale,
      ),
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Container(
            margin: EdgeInsets.only(top: 20 * context.scale),
            child: Image.asset(
              'assets/icons/tripsRectangle.png',
              width: 400 * context.scale,
              height: 440 * context.scale,
              fit: BoxFit.fill,
            ),
          ),

          Positioned(
            top: -35 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Container(
                width: 123 * context.scale,
                height: 79 * context.scale,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(40 * context.scale),
                  border: Border.all(color: Colors.white, width: 2 * context.scale),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.2),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ],
                  image: trip.tripImagePath != null
                      ? DecorationImage(
                    image: AssetImage(trip.tripImagePath!),
                    fit: BoxFit.cover,
                  )
                      : null,
                ),
              ),
            ),
          ),

          Positioned(
            top: 25 * context.scale,
            right: 25 * context.scale,
            child: Text(
              trip.tripDaysAgo,
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 16 * context.scale,
              ),
            ),
          ),

          Positioned(
            top: 45 * context.scale,
            left: 0,
            right: 0,
            child: Center(
              child: Text(
                trip.countryName,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 24 * context.scale,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
          ),

          Positioned(
            top: 95 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.end,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    trip.tripRoute,
                    textAlign: TextAlign.center,
                    softWrap: true,
                    overflow: TextOverflow.visible,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 14 * context.scale,
                    ),
                  ),
                ),
                SizedBox(width: 5 * context.scale),
                Image.asset(
                  'assets/icons/track.png',
                  width: 43 * context.scale,
                  height: 35 * context.scale,
                ),
              ],
            ),
          ),

          Positioned(
            top: 160 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.end,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    trip.passengerNames,
                    textAlign: TextAlign.center,
                    softWrap: true,
                    overflow: TextOverflow.visible,
                    style: TextStyle(
                      fontFamily: 'Tajawal',
                      fontSize: 14 * context.scale,
                    ),
                  ),
                ),
                SizedBox(width: 5 * context.scale),
                Image.asset(
                  'assets/icons/persons.png',
                  width: 40 * context.scale,
                  height: 40 * context.scale,
                ),
              ],
            ),
          ),

          Positioned(
            top: 225 * context.scale,
            right: 25 * context.scale,
            child: Text(
              "رقم الحجز:            ${trip.bookingNumber}",
              style: TextStyle(
                fontFamily: 'Tajawal',
                fontSize: 16 * context.scale,
              ),
            ),
          ),

          Positioned(
            top: 285 * context.scale,
            left: 15 * context.scale,
            right: 15 * context.scale,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Column(
                  children: [
                    CalendarDaysWidget(days: trip.daysToRegistrationEnd),
                    SizedBox(height: 6 * context.scale),
                    SizedBox(
                      width: 80 * context.scale,
                      child: Text(
                        "باقي لانتهاء التسجيل",
                        textAlign: TextAlign.center,
                        softWrap: true,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 12 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                    ),
                  ],
                ),

                Column(
                  mainAxisAlignment: MainAxisAlignment.start,
                  children: [
                    TouristsArcWidget(
                      current: trip.currentTourists,
                      max: trip.maxTourists,
                    ),
                    Transform.translate(
                      offset: Offset(0, -25 * context.scale),
                      child: Text(
                        "السائحين المنضمين",
                        textAlign: TextAlign.center,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 12 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                    ),
                  ],
                ),

                Column(
                  children: [
                    CalendarDaysWidget(days: trip.daysToStart),
                    SizedBox(height: 6 * context.scale),
                    SizedBox(
                      width: 80 * context.scale,
                      child: Text(
                        "باقي لبدأ الرحلة",
                        textAlign: TextAlign.center,
                        softWrap: true,
                        style: TextStyle(
                          fontFamily: 'Tajawal',
                          fontSize: 12 * context.scale,
                          color: Colors.black,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),

          Positioned(
            bottom: 1 * context.scale,
            left: 25 * context.scale,
            child: Row(
              children: List.generate(
                trip.starCount,
                    (i) => Padding(
                  padding: EdgeInsets.only(right: 4 * context.scale),
                  child: Image.asset(
                    'assets/icons/Star3.png',
                    width: 20 * context.scale,
                    height: 20 * context.scale,
                    fit: BoxFit.contain,
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
