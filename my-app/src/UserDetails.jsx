import { useState } from 'react';
import {
  ArrowLeft, User, MapPin, Phone, Mail, Calendar,
  FileText, Fingerprint, Image as ImageIcon,
  Map as MapIcon, Plane, Wallet, Users, ChevronDown, ChevronUp, ChevronLeft, ChevronRight, Heart, Receipt,
} from 'lucide-react';
import { useApiData } from './hooks/useApiData';
import { getUserBookings } from './services/dashboardApi';
import { formatDate } from './utils/format';

const DEFAULT_AVATAR =
  "data:image/svg+xml;utf8," +
  "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'>" +
  "<rect width='100' height='100' fill='%232a2a2a'/>" +
  "<g fill='%2391B3FA' opacity='0.7'>" +
  "<circle cx='50' cy='38' r='20'/>" +
  "<path d='M50 62c-21 0-36 12-38 32h76c-3-20-18-32-38-32z'/>" +
  "</g></svg>";

const LoadingBlock = () => (
  <div className="flex items-center justify-center py-10">
    <div className="w-8 h-8 border-4 border-[#2d303e] border-t-[#91B3FA] rounded-full animate-spin" />
  </div>
);

const ErrorBlock = ({ message }) => (
  <p className="text-sm text-red-400 bg-red-400/10 border border-red-400/20 rounded-xl px-4 py-3 text-center">
    Failed to load: {message}
  </p>
);

const InfoItem = ({ icon: Icon, label, value }) => (
  <div className="flex items-start gap-3 py-3 border-b border-[#ffffff0a] last:border-0 hover:bg-[#ffffff05] rounded-lg px-2 transition-colors duration-200">
    <div className="mt-0.5">
      <Icon className="w-4 h-4 text-[#91B3FA]" />
    </div>
    <div className="flex-1 text-sm">
      <span className="text-gray-400 font-medium">{label} : </span>
      <span className="text-white ml-1 font-semibold">{value}</span>
    </div>
  </div>
);

const CompanionCard = ({ companion, isExpanded, onToggle }) => {
  const hasDocs = companion.nationalIdImage || companion.passportImage;

  return (
    <div className="bg-[#1C1C1E] border border-[#D4AF37]/20 rounded-2xl overflow-hidden transition-all duration-300 shadow-md mb-4">
      <div
        onClick={onToggle}
        className="flex items-center justify-between p-4 cursor-pointer hover:bg-[#252528] transition-colors"
      >
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full overflow-hidden border border-[#91B3FA]">
            <img src={companion.avatar} alt={companion.name} className="w-full h-full object-cover" />
          </div>
          <div>
            <h4 className="text-sm font-bold text-white">{companion.name}</h4>
            <p className="text-xs text-[#91B3FA] flex items-center gap-1">
              <Heart className="w-3 h-3" /> {companion.relation}
            </p>
          </div>
        </div>
        <div className="text-gray-400">
          {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
        </div>
      </div>

      {isExpanded && (
        <div className="p-4 border-t border-[#333] bg-[#18181A] animate-in slide-in-from-top-2 duration-200">
          <div className="space-y-1 mb-4">
            <InfoItem icon={User} label="Age" value={`${companion.age} Years`} />
            <InfoItem icon={User} label="Gender" value={companion.gender} />
            <InfoItem icon={MapPin} label="Location" value={companion.location} />
            <InfoItem icon={Phone} label="Phone" value={companion.phone} />
            <InfoItem icon={Fingerprint} label="National ID" value={companion.nationalId} />
            <InfoItem icon={Calendar} label="Date Registered" value={companion.joinDate} />
            <InfoItem icon={Plane} label="Last Trip" value={companion.lastTrip} />
            <InfoItem icon={MapIcon} label="Programs Joined" value={companion.programsJoined} />
            <InfoItem icon={Wallet} label="Amount Spent" value={`$${companion.amountSpent.toLocaleString()}`} />
          </div>

          {hasDocs && (
            <div className="grid grid-cols-2 gap-3 mt-4">
              {companion.nationalIdImage && (
                <div className="p-2 bg-[#121212] rounded-xl border border-[#333]">
                  <span className="text-[10px] text-gray-400 mb-1.5 flex items-center gap-1"><ImageIcon className="w-3 h-3" /> ID Image</span>
                  <div className="w-full h-20 rounded-lg overflow-hidden border border-[#444]">
                    <img src={companion.nationalIdImage} alt="ID" className="w-full h-full object-cover opacity-80" />
                  </div>
                </div>
              )}
              {companion.passportImage && (
                <div className="p-2 bg-[#121212] rounded-xl border border-[#333]">
                  <span className="text-[10px] text-gray-400 mb-1.5 flex items-center gap-1"><ImageIcon className="w-3 h-3" /> Passport Image</span>
                  <div className="w-full h-20 rounded-lg overflow-hidden border border-[#444]">
                    <img src={companion.passportImage} alt="Passport" className="w-full h-full object-cover opacity-80" />
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

// CompanionResponse -> بيانات عرض البطاقة
function mapApiCompanion(c) {
  return {
    id: c.id,
    name: c.fullname || `${c.firstname ?? ''} ${c.lastname ?? ''}`.trim() || '—',
    relation: c.relationship || '—',
    avatar: c.profileImage || DEFAULT_AVATAR,
    age: c.age ?? '—',
    gender: c.gender || '—',
    location: c.residentialCityName || c.nationalityCountryName || '—',
    phone: c.phone || '—',
    nationalId: c.nationalNumber || '—',
    joinDate: formatDate(c.registrationDate),
    lastTrip: c.lastTourPackage?.packageName || '—',
    programsJoined: c.joinedPackagesCount ?? '—',
    amountSpent: c.totalAmountSpent ?? 0,
    nationalIdImage: c.nationalIdCard,
    passportImage: c.passportScan,
  };
}

const statusColor = (label) => {
  const s = (label || '').toLowerCase();
  if (s.includes('cancell') || s.includes('reject') || s.includes('declin')) return 'bg-red-400/10 text-red-400 border-red-400/20';
  if (s.includes('complet')) return 'bg-[#7FBF8E]/10 text-[#7FBF8E] border-[#7FBF8E]/20';
  if (s.includes('pending')) return 'bg-[#F4A261]/10 text-[#F4A261] border-[#F4A261]/20';
  return 'bg-[#91B3FA]/10 text-[#91B3FA] border-[#91B3FA]/20';
};

export default function UserDetails({ user, onBack }) {
  const [expandedCompanionId, setExpandedCompanionId] = useState(null);
  const [bookingsPage, setBookingsPage] = useState(1);
  const [bookingsPageSize] = useState(10);

  const currentUser = user || {};
  const { data: bookingsData, loading: bookingsLoading, error: bookingsError } = useApiData(
    () => getUserBookings(currentUser.id, bookingsPage, bookingsPageSize),
    [currentUser.id, bookingsPage, bookingsPageSize]
  );

  const pagination = bookingsData?.pagination ?? {};
  const bookings = bookingsData?.items ?? [];
  const totalSpent = bookingsData?.totalAmountSpent ?? 0;

  // المرافقون متجمعين من داخل الحجوزات (نفس واجهة UserBookings) وتفريغ التكرارات حسب المعرّف
  const companionsById = new Map();
  bookings.forEach((b) =>
    (b.companions ?? []).forEach((c) => companionsById.set(c.id, c))
  );
  const companionsList = [...companionsById.values()].map(mapApiCompanion);

  return (
    <div className="p-8 space-y-6 font-sans animate-in fade-in duration-300">

      {/* 1. الترويسة */}
      <div className="flex items-center gap-4 border-b border-[#333] pb-4">
        <button
          onClick={onBack}
          className="p-2 bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-lg hover:bg-[#252528] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
        </button>
        <div>
          <h2 className="text-2xl font-bold text-white">User Profile Details</h2>
          <p className="text-sm text-[#91B3FA]">Comprehensive overview, bookings & companions</p>
        </div>
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8">

        {/* === بطاقة البروفايل (كاملة العرض) === */}
        <div className="xl:col-span-12">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg flex flex-col md:flex-row md:items-center gap-6 relative overflow-hidden">
            <div className="absolute top-0 left-0 w-1 h-full bg-gradient-to-b from-[#91B3FA] to-[#F4A261]"></div>
            <div className="w-24 h-24 rounded-full border-2 border-[#D4AF37]/50 overflow-hidden shadow-[0_0_15px_rgba(212,175,55,0.2)] flex-shrink-0">
              <img src={currentUser.avatar || DEFAULT_AVATAR} alt="Profile" className="w-full h-full object-cover" />
            </div>
            <div className="flex-1 min-w-0">
              <h1 className="text-2xl font-bold text-white mb-1 truncate">{currentUser.name || '—'}</h1>
              <div className="flex flex-wrap gap-x-5 gap-y-1.5 text-sm text-gray-400">
                <span className="flex items-center gap-1.5"><Mail className="w-4 h-4 text-[#91B3FA]" /> {currentUser.email || '—'}</span>
                <span className="flex items-center gap-1.5"><Phone className="w-4 h-4 text-[#F4A261]" /> {currentUser.phone || '—'}</span>
                <span className="flex items-center gap-1.5"><Calendar className="w-4 h-4 text-[#7FBF8E]" /> Joined: {currentUser.joinDate || '—'}</span>
              </div>
            </div>
          </div>
        </div>

        {/* === العمود الأيسر: الحجوزات + التفاصيل + الوثائق === */}
        <div className="xl:col-span-8 space-y-6">

          {/* بطاقة الحجوزات */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg">
            <div className="flex items-center justify-between mb-5 border-b border-[#333] pb-4">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <Receipt className="w-5 h-5 text-[#91B3FA]" /> User Bookings
              </h3>
              {!bookingsLoading && !bookingsError && (
                <span className="bg-[#91B3FA]/10 text-[#91B3FA] text-xs font-bold px-3 py-1.5 rounded-full">
                  {pagination.totalItems ?? bookings.length} Total
                </span>
              )}
            </div>

            {bookingsError ? (
              <ErrorBlock message={bookingsError.message} />
            ) : bookingsLoading ? (
              <LoadingBlock />
            ) : (
              <>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
                  <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                    <Receipt className="w-8 h-8 text-[#91B3FA]" />
                    <div>
                      <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Total Bookings</p>
                      <p className="text-xl font-bold text-white mt-0.5">{pagination.totalItems ?? bookings.length}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                    <Wallet className="w-8 h-8 text-[#7FBF8E]" />
                    <div>
                      <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Amount Spent</p>
                      <p className="text-xl font-bold text-[#7FBF8E] mt-0.5">${totalSpent.toLocaleString()}</p>
                    </div>
                  </div>
                </div>

                {bookings.length === 0 ? (
                  <p className="text-sm text-gray-500 text-center py-8">No bookings found for this user.</p>
                ) : (
                  <div className="space-y-3 max-h-80 overflow-y-auto custom-scrollbar pr-1">
                    {bookings.map((b) => (
                      <div key={b.id} className="flex items-center justify-between gap-4 bg-[#121212] border border-[#2a2a2a] rounded-xl px-4 py-3">
                        <div className="min-w-0">
                          <p className="text-sm font-semibold text-white truncate">{b.tourPackage?.packageName || `Booking #${b.bookingNumber ?? b.id}`}</p>
                          <p className="text-xs text-gray-400 mt-0.5">{formatDate(b.bookingDate)}</p>
                        </div>
                        <div className="flex items-center gap-3 flex-shrink-0">
                          <span className={`text-[11px] font-bold px-2.5 py-1 rounded-full border ${statusColor(b.statusLabel)}`}>{b.statusLabel}</span>
                          <span className="text-sm font-bold text-white whitespace-nowrap">${(b.totalCost ?? 0).toLocaleString()}</span>
                        </div>
                      </div>
                    ))}
                  </div>
                )}

                {bookings.length > 0 && (
                  <div className="flex items-center justify-between mt-4 pt-4 border-t border-[#333]">
                    <p className="text-xs text-gray-400">
                      Page {pagination.page || '—'} of {pagination.totalPages || '—'}
                    </p>
                    <div className="flex items-center gap-3">
                      <button
                        onClick={() => setBookingsPage((p) => Math.max(1, p - 1))}
                        disabled={!pagination.hasPreviousPage}
                        className="flex items-center gap-1.5 px-4 py-2 rounded-xl border border-[#2d303e] bg-[#121212] text-sm font-medium text-gray-300 hover:border-[#91B3FA]/40 hover:text-white transition disabled:opacity-40 disabled:cursor-not-allowed"
                      >
                        <ChevronLeft className="w-4 h-4" /> Prev
                      </button>
                      <button
                        onClick={() => setBookingsPage((p) => p + 1)}
                        disabled={!pagination.hasNextPage}
                        className="flex items-center gap-1.5 px-4 py-2 rounded-xl border border-[#2d303e] bg-[#121212] text-sm font-medium text-gray-300 hover:border-[#91B3FA]/40 hover:text-white transition disabled:opacity-40 disabled:cursor-not-allowed"
                      >
                        Next <ChevronRight className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>

          {/* التفاصيل الكاملة (مقسمة لبطاقتين) */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-[#18181A]/80 border border-[#D4AF37]/20 rounded-2xl p-5 shadow-lg">
              <h3 className="text-lg font-bold text-white mb-4 flex items-center gap-2 border-b border-[#333] pb-3">
                <User className="w-5 h-5 text-[#91B3FA]" /> Personal Details
              </h3>
              <div className="space-y-1">
                <InfoItem icon={User} label="Age" value={`${currentUser.age} Years`} />
                <InfoItem icon={User} label="Gender" value={currentUser.gender} />
                <InfoItem icon={MapPin} label="Current Residence" value={currentUser.location} />
                <InfoItem icon={Fingerprint} label="National ID" value={currentUser.nationalId} />
                <InfoItem icon={FileText} label="Passport No" value={currentUser.passportNo} />
              </div>
            </div>

            <div className="bg-[#18181A]/80 border border-[#D4AF37]/20 rounded-2xl p-5 shadow-lg">
              <h3 className="text-lg font-bold text-white mb-4 flex items-center gap-2 border-b border-[#333] pb-3">
                <FileText className="w-5 h-5 text-[#F4A261]" /> Document Images
              </h3>
              <div className="space-y-4 mt-2">
                <div>
                  <span className="text-xs text-gray-400 mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> ID Image (صورة الهوية)</span>
                  <div className="w-full h-24 rounded-xl overflow-hidden border-2 border-dashed border-[#444] relative group">
                    {currentUser.nationalIdImage ? (
                      <img src={currentUser.nationalIdImage} alt="ID" className="w-full h-full object-cover opacity-60 group-hover:opacity-100 transition" />
                    ) : (
                      <p className="text-xs text-gray-500 flex items-center justify-center h-full">Not available</p>
                    )}
                  </div>
                </div>
                <div>
                  <span className="text-xs text-gray-400 mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> Passport Image (جواز السفر)</span>
                  <div className="w-full h-24 rounded-xl overflow-hidden border-2 border-dashed border-[#444] relative group">
                    {currentUser.passportImage ? (
                      <img src={currentUser.passportImage} alt="Passport" className="w-full h-full object-cover opacity-60 group-hover:opacity-100 transition" />
                    ) : (
                      <p className="text-xs text-gray-500 flex items-center justify-center h-full">Not available</p>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>

        </div>

        {/* === العمود الأيمن: المرافقون === */}
        <div className="xl:col-span-4 flex flex-col">
          <div className="bg-[#18181A] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg flex-1">

            <div className="flex items-center justify-between mb-6 pb-4 border-b border-[#333]">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <Users className="w-5 h-5 text-[#F4A261]" /> Companions
              </h3>
              {!bookingsLoading && !bookingsError && (
                <span className="bg-[#D4AF37]/20 text-[#D4AF37] py-1 px-3 rounded-full text-xs font-bold">
                  {companionsList.length} Total
                </span>
              )}
            </div>

            {bookingsError ? (
              <ErrorBlock message={bookingsError.message} />
            ) : bookingsLoading ? (
              <LoadingBlock />
            ) : (
              <div className="space-y-4">
                {companionsList.map((companion) => (
                  <CompanionCard
                    key={companion.id}
                    companion={companion}
                    isExpanded={expandedCompanionId === companion.id}
                    onToggle={() => setExpandedCompanionId(
                      expandedCompanionId === companion.id ? null : companion.id
                    )}
                  />
                ))}

                {companionsList.length === 0 && (
                  <div className="text-center py-10 text-gray-500 flex flex-col items-center">
                    <Users className="w-10 h-10 mb-2 opacity-20" />
                    <p>No companions found for this user.</p>
                  </div>
                )}
              </div>
            )}

          </div>
        </div>

      </div>
    </div>
  );
}