import 'package:flutter/material.dart';
import '../app_constants.dart';
import 'trip_models.dart';

// ════════════════════════════════════════════════════════
// تبويب "السابقة"
// ════════════════════════════════════════════════════════
class PastTripsTab extends StatelessWidget {
  final List<PastTripModel> trips;

  const PastTripsTab({super.key, required this.trips});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Column(
        children: [
          ...trips.map((trip) => _PastTripCard(trip: trip)),
          SizedBox(height: 20 * context.scale),
        ],
      ),
    );
  }
}

class _PastTripCard extends StatelessWidget {
  final PastTripModel trip;
  const _PastTripCard({required this.trip});

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
              height: 397 * context.scale,
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
              trip.timeAgo,
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
            child: _buildIconRow(
              context,
              iconPath: 'assets/icons/track.png',
              iconW: 43, iconH: 35,
              text: trip.tripRoute,
            ),
          ),

          Positioned(
            top: 155 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: _buildIconRow(
              context,
              iconPath: 'assets/icons/persons.png',
              iconW: 40, iconH: 40,
              text: trip.passengerNames,
            ),
          ),

          Positioned(
            top: 215 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: _buildIconRow(
              context,
              iconPath: 'assets/icons/tripsCalender.png',
              iconW: 40, iconH: 40,
              text: trip.joinDate,
            ),
          ),

          Positioned(
            top: 265 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: _buildIconRow(
              context,
              iconPath: 'assets/icons/tripsCalender.png',
              iconW: 40, iconH: 40,
              text: trip.duration,
            ),
          ),

          Positioned(
            top: 320 * context.scale,
            left: 20 * context.scale,
            right: 20 * context.scale,
            child: _buildCostRow(context),
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

  Widget _buildIconRow(
      BuildContext context, {
        required String iconPath,
        required double iconW,
        required double iconH,
        required String text,
      }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        Expanded(
          child: Text(
            text,
            textAlign: TextAlign.center,
            softWrap: true,
            overflow: TextOverflow.visible,
            style: TextStyle(
              fontFamily: 'Tajawal',
              fontSize: 14 * context.scale,
              color: Colors.black,
            ),
          ),
        ),
        SizedBox(width: 5 * context.scale),
        Image.asset(
          iconPath,
          width: iconW * context.scale,
          height: iconH * context.scale,
          fit: BoxFit.contain,
        ),
      ],
    );
  }

  Widget _buildCostRow(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.end,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Expanded(
              child: Text(
                trip.costLabel,
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 14 * context.scale,
                  color: Colors.black,
                ),
              ),
            ),
            SizedBox(width: 5 * context.scale),
            Image.asset(
              'assets/icons/tripPrice.png',
              width: 40 * context.scale,
              height: 40 * context.scale,
              fit: BoxFit.contain,
            ),
          ],
        ),
        SizedBox(height: 4 * context.scale),
        RichText(
          textAlign: TextAlign.center,
          text: TextSpan(
            children: [
              TextSpan(
                text: r'$ ',
                style: TextStyle(
                  fontFamily: 'Tajawal',
                  fontSize: 16 * context.scale,
                  color: Colors.black,
                ),
              ),
              TextSpan(
                text: trip.costAmount.replaceAll(r'$ ', '').replaceAll('\$', '').trim(),
                style: TextStyle(
                  fontFamily: 'AgencyFB',
                  fontSize: 20 * context.scale,
                  fontWeight: FontWeight.w400,
                  color: Colors.black,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
