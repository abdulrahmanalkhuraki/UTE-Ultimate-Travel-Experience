import { useState } from 'react';
import UserDetails from './UserDetails';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { ChevronDown, Calendar, MapPin, Map, TrendingUp, UserRound,UsersRound , UserRoundX} from 'lucide-react';
import { useApiData } from './hooks/useApiData';
import { getTouristsDashboard, getUsersByRole, getDeletedUsers } from './services/dashboardApi';
import { timeAgo, formatDate, calcAge } from './utils/format';

const FALLBACK_AVATAR = 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150';

// GET /api/User/filter?roleName= أو GET /api/User/deleted -> نفس شكل عنصر اليوزر
function mapApiUser(u) {
  return {
    id: u.id,
    name: `${u.firstName ?? ''} ${u.lastName ?? ''}`.trim() || '—',
    age: calcAge(u.dateOfBirth) ?? '—',
    gender: u.gender || '—',
    email: u.email || '—',
    phone: u.phone || '—',
    // ما في API بيرجع عدد المرافقين/البرامج/الشركات لكل يوزر لحاله
    companions: '—',
    programs: '—',
    companies: '—',
    // currentLocation إحداثيات (lat/long) مو عنوان نصي — أقرب شي نصي هو المدينة/الجنسية
    location: u.residentialCityName || u.nationalityCountryName || '—',
    joined: timeAgo(u.createdAtUtc),
    avatar: u.image || FALLBACK_AVATAR,
    nationalId: u.nationalNumber || '—',
    passportNo: u.passportNumber || '—',
    nationalIdImage: u.nationalIdImage,
    passportImage: u.passportImage,
    joinDate: formatDate(u.createdAtUtc),
  };
}

export default function Users() {
  const [isDeletedExpanded, setIsDeletedExpanded] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const { data: touristsData } = useApiData(getTouristsDashboard, []);
  // pageSize كبيرة لأن التصميم الحالي بيعرض القائمة كاملة بدون Pagination UI
  const { data: touristUsersData } = useApiData(() => getUsersByRole('Tourist', 1, 100), []);
  const { data: deletedUsersData } = useApiData(getDeletedUsers, []);

  // GET /api/Admin/dashboard/tourists -> touristGrowth
  const chartData = (touristsData?.touristGrowth ?? []).map((g) => ({
    name: g.month,
    tourists: g.count,
  }));

  const activeUsers = (touristUsersData?.items ?? []).map(mapApiUser);
  const deletedUsers = (deletedUsersData?.users ?? []).map(mapApiUser);

  const UserCard = ({ user, isClickable = false }) => (
    <div
      onClick={() => isClickable && setSelectedUser(user)}
      className={`flex justify-between items-center bg-[#18181A] border border-[#333] p-5 rounded-2xl transition shadow-md ${isClickable ? 'cursor-pointer hover:border-[#D4AF37]/50' : ''}`}
    >
      <div className="text-xs text-gray-500 font-medium whitespace-nowrap w-24">
        {user.joined}
      </div>

      <div className="flex items-center gap-5 text-right flex-1 justify-end">
        <div className="space-y-2">
          <h4 className="text-base font-bold text-white">{user.name}</h4>

          <div className="flex items-center justify-end gap-4 text-xs text-gray-400">
            <span className="flex items-center gap-1.5"><Calendar className="w-3.5 h-3.5" /> Age: {user.age}</span>
            <span>•</span>
            <span className="flex items-center gap-1.5 text-[#91B3FA]"><UserRound className="w-3.5 h-3.5" /> Companions: {user.companions}</span>
          </div>

          <div className="flex items-center justify-end gap-1.5 text-xs text-[#91B3FA]">
            <MapPin className="w-3.5 h-3.5" /> {user.location}
          </div>

          <div className="flex items-center justify-end gap-1.5 text-xs text-gray-400">
            <Map className="w-3.5 h-3.5" /> Programs: {user.programs} • With {user.companies} Companies
          </div>
        </div>

        <div className="w-16 h-16 rounded-xl overflow-hidden border-2 border-[#2d303e] flex-shrink-0">
          <img src={user.avatar} alt={user.name} className="w-full h-full object-cover" />
        </div>
      </div>
    </div>
  );

  if (selectedUser) {
    return <UserDetails user={selectedUser} onBack={() => setSelectedUser(null)} />;
  }

  return (
    <div className="p-8 space-y-8 font-sans">
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <div className="lg:col-span-7 space-y-6">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 p-6 rounded-2xl shadow-lg">
            <h3 className="text-lg font-semibold text-white mb-4 text-left">Tourist Growth Analysis</h3>
            <div className="w-full h-64 mb-6">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis dataKey="name" stroke="#666" tick={{ fill: '#888', fontSize: 12 }} />
                  <YAxis stroke="#666" tick={{ fill: '#888', fontSize: 12 }} />
                  <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#91B3FA', border: '1px solid #91B3FA', borderRadius: '0.8rem'}} />
                  <Legend iconType="plainline" />
                  <Line type="monotone" dataKey="tourists" stroke="#91B3FA" strokeWidth={3} dot={true} />
                  {/* <Line type="monotone" dataKey="Local" stroke="#F4A261" strokeWidth={3} dot={false} /> */}
                </LineChart>
              </ResponsiveContainer>
            </div>

            <div className="grid grid-cols-3 gap-4 pt-6 border-t border-[#333]">
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <UsersRound className="w-8 h-8 text-[#91B3FA]" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Active Registered</p>
                  <p className="text-xl font-bold text-[#91B3FA] mt-0.5">{touristsData ? touristsData.activeTourists.toLocaleString() : '—'}</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <UserRoundX className="w-8 h-8 text-red-400" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Deleted Accounts</p>
                  <p className="text-xl font-bold text-red-400 mt-0.5">{touristsData ? touristsData.deletedTourists.toLocaleString() : '—'}</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <TrendingUp className="w-8 h-8 text-[#7FBF8E]" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Peak Records</p>
                  <p className="text-xl font-bold text-[#7FBF8E] mt-0.5">{touristsData ? touristsData.totalTourists.toLocaleString() : '—'}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
            <button
              onClick={() => setIsDeletedExpanded(!isDeletedExpanded)}
              className="w-full p-5 flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition"
            >
              <div className="flex items-center gap-4">
              <UserRoundX className="w-8 h-8 text-red-400 ml-5" />
              <h3 className="text-base text-lg font-semibold text-red-400">Deleted Accounts</h3>
              </div>
              <ChevronDown className={`w-5 h-5 text-gray-400 mr-10 transition-transform duration-300 ${isDeletedExpanded ? 'rotate-180' : ''}`} />
              
            </button>
            {isDeletedExpanded && (
              <div className="p-5 space-y-4 max-h-[350px] overflow-y-auto custom-scrollbar border-t border-[#333]">
                {deletedUsers.map((user) => (
                  <UserCard key={user.id} user={user} />
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="lg:col-span-5">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl shadow-lg p-6 h-full flex flex-col">
            <h3 className="text-lg font-semibold  mb-6 text-left ml-5 border-b border-[#333] pb-4 text-[#91B3FA]">
              <div className="flex items-center gap-4">
              <UsersRound className="w-8 h-8  ml-5" />
              ALL Users
              </div>
            </h3>
            

            <div className="space-y-5 overflow-y-auto flex-1 pr-2 custom-scrollbar">
              {activeUsers.map((user) => (
                <UserCard key={user.id} user={user} isClickable />
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}