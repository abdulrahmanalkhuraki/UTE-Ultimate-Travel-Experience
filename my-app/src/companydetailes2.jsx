import {
  ArrowLeft, Building2, User, Calendar, MapPin, Phone, Mail,
  FileText, CreditCard, Star, MessageSquare, Wallet, Users, Map, Image as ImageIcon, Fingerprint
} from 'lucide-react';
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import { useApiData } from './hooks/useApiData';
import { getCompanyDetails, getUserById } from './services/dashboardApi';
import { formatDate } from './utils/format';

// === تم نقل InfoRow إلى هنا (خارج المكون الأساسي) لحل الخطأ ===
const InfoRow = ({ icon: Icon, label, value }) => (
  <div className="flex flex-col mb-4">
    <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-1 flex items-center gap-1.5">
      <Icon className="w-3.5 h-3.5" /> {label}
    </span>
    <div className="bg-[#121212] border border-[#2a2a2a] rounded-lg p-3 text-sm text-gray-200">
      {value}
    </div>
  </div>
);

export default function CompanyDetails({ company, onBack }) {
  const companyId = company?.id ?? company?.companyId;
  const { data: details } = useApiData(() => getCompanyDetails(companyId), [companyId]);
  const info = details?.company;

  // GET /api/Admin/dashboard/companies/:companyId -> bookingGrowth + tourPackageGrowth
  const growthByMonth = {};
  (details?.bookingGrowth ?? []).forEach((g) => {
    growthByMonth[g.month] = { ...(growthByMonth[g.month] || { name: g.month }), name: g.month, Bookings: g.count };
  });
  (details?.tourPackageGrowth ?? []).forEach((g) => {
    growthByMonth[g.month] = { ...(growthByMonth[g.month] || { name: g.month }), name: g.month, Programs: g.count };
  });
  const companyPerformanceData = Object.values(growthByMonth);

  // GET /api/User/:id -> بيانات مالك الشركة (المتاحة عبر company.userId فقط)
  const { data: owner } = useApiData(
    () => (info?.userId ? getUserById(info.userId) : Promise.resolve(null)),
    [info?.userId]
  );

  return (
    <div className="p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* 1. الترويسة وزر العودة */}
      <div className="flex items-center gap-4 border-b border-[#333] pb-4">
        <button 
          onClick={onBack}
          className="p-2 bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-lg hover:bg-[#252528] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
        </button>
        <div className="w-12 h-12 rounded-xl overflow-hidden border-2 border-[#D4AF37]/50">
          <img src={info?.logo || company?.logo || "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80"} alt="Logo" className="w-full h-full object-cover" />
        </div>
        <div>
          <h2 className="text-2xl font-bold text-white">{info?.name || company?.name || "Global Trails Inc."}</h2>
          <p className="text-sm text-[#91B3FA]">Detailed Performance & Information</p>
        </div>
      </div>

      {/* 2. القسم العلوي (المخطط البياني + المربعات الأربعة) */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-6">
        
        {/* المخطط البياني والمستطيلات المدمجة */}
        <div className="xl:col-span-8 flex flex-col">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-t-2xl p-6 flex-1 shadow-lg relative">
            <h3 className="text-base font-semibold text-white mb-6">Bookings & Programs Growth</h3>
            <div className="w-full h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={companyPerformanceData} margin={{ top: 0, right: 0, left: -20, bottom: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
                  <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
                  <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#D4AF37', borderRadius: '8px'}} cursor={{fill: 'rgba(255,255,255,0.05)'}} />
                  <Legend iconType="circle" />
                  <Bar dataKey="Bookings" fill="#91B3FA" name="Total Bookings" radius={[4, 4, 0, 0]} barSize={20} />
                  <Bar dataKey="Programs" fill="#F4A261" name="Published Programs" radius={[4, 4, 0, 0]} barSize={20} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
          {/* المستطيلات المدمجة السفلية */}
          <div className="grid grid-cols-2 bg-[#18181A] border-x border-b border-[#D4AF37]/30 rounded-b-2xl divide-x divide-[#333] shadow-lg">
            <div className="p-4 flex items-center justify-center gap-4 hover:bg-[#202022] transition rounded-bl-2xl">
              <div className="p-3 bg-[#91B3FA]/10 rounded-xl"><Users className="w-6 h-6 text-[#91B3FA]" /></div>
              <div>
                <p className="text-xs text-gray-400 font-medium">Total Bookings</p>
                <p className="text-2xl font-bold text-white">{details ? details.bookingsCount.toLocaleString() : '—'}</p>
              </div>
            </div>
            <div className="p-4 flex items-center justify-center gap-4 hover:bg-[#202022] transition rounded-br-2xl">
              <div className="p-3 bg-[#F4A261]/10 rounded-xl"><Map className="w-6 h-6 text-[#F4A261]" /></div>
              <div>
                <p className="text-xs text-gray-400 font-medium">Total Published Programs</p>
                <p className="text-2xl font-bold text-[#F4A261]">{details ? details.totalTourPackages.toLocaleString() : '—'}</p>
              </div>
            </div>
          </div>
        </div>

        {/* المربعات الأربعة */}
        <div className="xl:col-span-4 grid grid-cols-2 gap-4">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Calendar className="w-8 h-8 text-[#91B3FA] mb-3" />
            <p className="text-xs text-gray-400 mb-1">Joined Date</p>
            <p className="text-lg font-bold text-white">{info?.createdAtUtc ? formatDate(info.createdAtUtc) : '—'}</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Wallet className="w-8 h-8 text-[#F4A261] mb-3" />
            <p className="text-xs text-gray-400 mb-1">Total Revenue</p>
            <p className="text-lg font-bold text-white">{details ? `$${details.totalRevenue.toLocaleString()}` : '—'}</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <MessageSquare className="w-8 h-8 text-[#91B3FA] mb-3" />
            <p className="text-xs text-gray-400 mb-1">User Reviews</p>
            <p className="text-lg font-bold text-white">{details ? details.reviewsCount.toLocaleString() : '—'}</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Star className="w-8 h-8 text-[#D4AF37] mb-3 fill-[#D4AF37]" />
            <p className="text-xs text-gray-400 mb-1">Average Rating</p>
            <p className="text-lg font-bold text-white">{details ? `${details.averageRating} / 5.0` : '—'}</p>
          </div>
        </div>
      </div>

      {/* 3. القسم السفلي (معلومات الشركة + معلومات المالك) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        
        {/* معلومات الشركة - الجهة اليسرى */}
        <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg relative overflow-hidden">
          <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#91B3FA] to-transparent opacity-50"></div>
          <div className="flex items-center gap-3 mb-6">
            <Building2 className="w-6 h-6 text-[#91B3FA]" />
            <h3 className="text-lg font-semibold text-white">Company Information</h3>
          </div>
          
          <div className="grid grid-cols-2 gap-x-4">
            <div className="col-span-2"><InfoRow icon={Building2} label="Trade Name (الاسم التجاري)" value={info?.name || '—'} /></div>
            <div className="col-span-2"><InfoRow icon={FileText} label="Short Description (نبذة عن الشركة)" value={info?.description || info?.about || '—'} /></div>
            <div className="col-span-2"><InfoRow icon={MapPin} label="Location (موقع الشركة)" value={info?.location || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={Phone} label="Phone Number (رقم الهاتف)" value={info?.phoneNumber || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={Mail} label="Email (البريد الالكتروني)" value={info?.email || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={Calendar} label="Founded Date (تاريخ التأسيس)" value={info?.foundingDate ? formatDate(info.foundingDate) : '—'} /></div>
            <div className="col-span-1"><InfoRow icon={FileText} label="Tourism Record Number (رقم السجل)" value={info?.tourismLicenseNumber || '—'} /></div>
            <div className="col-span-2"><InfoRow icon={CreditCard} label="Bank Account (حسابك البنكي)" value={info?.bankAccount || '—'} /></div>

            <div className="col-span-2 mt-2">
              <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> Tourism Record Image (صورة السجل)</span>
              {info?.tourismLicenseImage ? (
                <div className="w-full h-32 rounded-xl overflow-hidden border border-[#333]">
                  <img src={info.tourismLicenseImage} alt="Tourism Record" className="w-full h-full object-cover" />
                </div>
              ) : (
                <div className="w-full h-32 border-2 border-dashed border-[#333] rounded-xl flex items-center justify-center bg-[#121212] text-gray-500">
                  <ImageIcon className="w-8 h-8 opacity-50" />
                </div>
              )}
            </div>
          </div>
        </div>

        {/* معلومات مالك الشركة - الجهة اليمنى */}
        <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg relative overflow-hidden">
           <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#F4A261] to-transparent opacity-50"></div>
          <div className="flex items-center gap-3 mb-6 border-b border-[#333] pb-4">
            <div className="w-14 h-14 rounded-full border-2 border-[#F4A261] overflow-hidden">
              <img src={owner?.image || "https://i.pravatar.cc/150?img=11"} alt="Owner" className="w-full h-full object-cover" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-white">Owner Information</h3>
              <p className="text-xs text-gray-400">Personal Details</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-x-4">
            <div className="col-span-1"><InfoRow icon={User} label="First Name (الاسم الأول)" value={owner?.firstName || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={User} label="Last Name (الاسم الأخير)" value={owner?.lastName || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={Phone} label="Phone Number (رقم الهاتف)" value={owner?.phone || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={MapPin} label="Residence (مكان الإقامة)" value={owner?.placeOfResidence || owner?.currentLocation || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={User} label="Gender (الجنس)" value={owner?.gender || '—'} /></div>
            <div className="col-span-1"><InfoRow icon={Calendar} label="Birth Date (تاريخ الميلاد)" value={owner?.dateOfBirth ? formatDate(owner.dateOfBirth) : '—'} /></div>
            <div className="col-span-2"><InfoRow icon={Fingerprint} label="National ID (الرقم الوطني)" value={owner?.nationalNumber || '—'} /></div>
            <div className="col-span-2"><InfoRow icon={CreditCard} label="Bank Account (حسابك البنكي)" value={owner?.bankAccount || '—'} /></div>

            <div className="col-span-2 mt-2">
              <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> ID Image (صورة الهوية)</span>
              {owner?.nationalIdImage ? (
                <div className="w-full h-32 rounded-xl overflow-hidden border border-[#333]">
                  <img src={owner.nationalIdImage} alt="National ID" className="w-full h-full object-cover" />
                </div>
              ) : (
                <div className="w-full h-32 border-2 border-dashed border-[#333] rounded-xl flex items-center justify-center bg-[#121212] text-gray-500">
                  <ImageIcon className="w-8 h-8 opacity-50" />
                </div>
              )}
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}