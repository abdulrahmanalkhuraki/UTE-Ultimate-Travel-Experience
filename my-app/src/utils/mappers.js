import { formatDate, timeAgo } from './format';

const FALLBACK_LOGO = 'https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80';
const FALLBACK_TRIP_IMAGE = 'https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=150&auto=format&fit=crop&q=60';

// Domain.Enums.TourPackageStatus بالباك — الحقل status برد /api/TourPackage رقم (مو نص)
export const TOUR_PACKAGE_STATUS = {
  PENDING: 0,
  ACTIVE: 1,
  COMPLETED: 2,
  CANCELLED: 3,
  REJECTED: 4,
};

// GET /api/TourCompany أو /api/TourCompany/pending -> عنصر شركة
export function mapApiCompany(c) {
  return {
    id: c.id,
    name: c.name,
    founded: formatDate(c.foundingDate),
    location: c.location || '—',
    // ما في API بيرجع عدد البرامج المنشورة لكل شركة بهالقائمة
    programs: '—',
    logo: c.logo || FALLBACK_LOGO,
    description: c.description,
    phoneNumber: c.phoneNumber,
    email: c.email,
    foundingDate: c.foundingDate,
    tourismLicenseNumber: c.tourismLicenseNumber,
    tourismLicenseImage: c.tourismLicenseImage,
    bankAccount: c.bankAccount,
    about: c.about,
    status: c.status,
    userId: c.userId,
    createdAtUtc: c.createdAtUtc,
  };
}

// GET /api/TourPackage -> عنصر برنامج سياحي
export function mapTourPackage(pkg) {
  const cityNames = (pkg.cities ?? []).map((c) => c.cityName);
  const cover = pkg.media?.[0]?.mediaUrl || FALLBACK_TRIP_IMAGE;
  const guide = pkg.guides?.[0];

  return {
    id: pkg.id,
    title: pkg.packageName,
    country: pkg.countryName || '—',
    regions: cityNames.join(', ') || '—',
    company: pkg.companyName || '—',
    companyLogo: null, // ما في لوجو الشركة بهاد الـ API
    startingDate: formatDate(pkg.startDate),
    image: cover,
    coverImage: cover,
    description: pkg.description,
    submissionDate: timeAgo(pkg.createdAtUtc),
    registrationDeadline: formatDate(pkg.registrationDeadline),
    startDate: formatDate(pkg.startDate),
    duration: pkg.durationInDays ? `${pkg.durationInDays} Days` : '—',
    meetingPoint: pkg.meetingPoint,
    locations: cityNames,
    price: pkg.pricePerPerson,
    totalSpots: pkg.totalCapacity,
    spotsLeft: pkg.availableSeats,
    rating: pkg.rate,
    guide: guide
      ? { name: guide.fullName, avatar: guide.profileImageUrl, experience: '—' }
      : undefined,
    itinerary: (pkg.days ?? []).map((d) => ({
      id: d.id,
      date: `Day ${d.dayNumber}`,
      title: d.dayTitle,
      subtitle: d.dayDescription,
      activities: (d.activities ?? []).map((a) => ({
        time: a.startTime,
        name: a.title,
        desc: a.description,
      })),
    })),
    // status: رقم (TourPackageStatus enum بالباك) — 0=Pending 1=Active 2=Completed 3=Cancelled 4=Rejected
    status: pkg.status,
    statusLabel: pkg.statusLabel || '',
  };
}
