import { useState } from 'react';
import UserDetails from './UserDetails';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { ChevronDown, Calendar, MapPin, Map, TrendingUp, UserRound,UsersRound , UserRoundX} from 'lucide-react';

// Mock Data for the chart
const chartData = [
  { name: 'Jan', tourists: 30 },
  { name: 'Feb', tourists: 45 },
  { name: 'Mar', tourists: 40 },
  { name: 'Apr', tourists: 60 },
  { name: 'May', tourists: 75 },
  { name: 'Jun', tourists: 70 },
  { name: 'Jul', tourists: 85 },
  { name: 'Aug', tourists: 80 },
  { name: 'Sep', tourists: 95 },
  { name: 'Oct', tourists: 90 },
  { name: 'Nov', tourists: 110 },
  { name: 'Dec', tourists: 105 },
];

// Mock Data for Active Users
const activeUsers = [
  {
    id: 1,
    name: 'Sarah Ahmed',
    age: 25,
    gender: 'Female',
    email: 'sarah.ahmed@email.com',
    phone: '+971 50 111 1111',
    companions: 1,
    location: 'Atlantis Hotel, Palm Jumeirah, Dubai, UAE',
    programs: 3,
    companies: 2,
    joined: '3 days ago',
    avatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150',
    nationalId: '1234567890',
    passportNo: 'P12345678',
    joinDate: '15 / 03 / 2025',
    stats: { programs: 3, companies: 2, trips: 2, companions: 1, spent: 12500 },
    companionsList: []
  },
  {
    id: 2,
    name: 'John Smith',
    age: 32,
    gender: 'Male',
    email: 'john.smith@email.com',
    phone: '+971 50 222 2222',
    companions: 3,
    location: 'Burj Al Arab, Jumeirah, Dubai, UAE',
    programs: 5,
    companies: 3,
    joined: '5 days ago',
    avatar: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150',
    nationalId: '2234567890',
    passportNo: 'P22345678',
    joinDate: '10 / 02 / 2025',
    stats: { programs: 5, companies: 3, trips: 4, companions: 3, spent: 24000 },
    companionsList: []
  },
  {
    id: 3,
    name: 'Aisha Khan',
    age: 28,
    gender: 'Female',
    email: 'aisha.khan@email.com',
    phone: '+971 50 333 3333',
    companions: 0,
    location: 'Marina Heights, Dubai Marina, Dubai, UAE',
    programs: 2,
    companies: 1,
    joined: 'Joined: Mar 5, 2026',
    avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150',
    nationalId: '3234567890',
    passportNo: 'P32345678',
    joinDate: '05 / 03 / 2026',
    stats: { programs: 2, companies: 1, trips: 1, companions: 0, spent: 7800 },
    companionsList: []
  },
];

// Mock Data for Deleted Users
const deletedUsers = [
  {
    id: 4,
    name: 'Michael Brown',
    age: 40,
    gender: 'Male',
    email: 'michael.brown@email.com',
    phone: '+971 50 444 4444',
    companions: 2,
    location: 'Downtown Hotel, Rooftop Ave, Dubai, UAE',
    programs: 1,
    companies: 1,
    joined: 'Deleted: 2 days ago',
    avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150',
    nationalId: '4234567890',
    passportNo: 'P42345678',
    joinDate: '01 / 01 / 2024',
    stats: { programs: 1, companies: 1, trips: 1, companions: 2, spent: 5400 },
    companionsList: []
  },
  {
    id: 5,
    name: 'Elena Rostova',
    age: 22,
    gender: 'Female',
    email: 'elena.rostova@email.com',
    phone: '+971 50 555 5555',
    companions: 1,
    location: 'JBR Apartments, Cluster T, Dubai, UAE',
    programs: 4,
    companies: 2,
    joined: 'Deleted: May 12, 2026',
    avatar: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150',
    nationalId: '5234567890',
    passportNo: 'P52345678',
    joinDate: '12 / 05 / 2026',
    stats: { programs: 4, companies: 2, trips: 3, companions: 1, spent: 11000 },
    companionsList: []
  },
];

export default function Users() {
  const [isDeletedExpanded, setIsDeletedExpanded] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

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
                  <p className="text-xl font-bold text-white mt-0.5">12,450</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <UserRoundX className="w-8 h-8 text-red-400" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Deleted Accounts</p>
                  <p className="text-xl font-bold text-red-400 mt-0.5">3,120</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <TrendingUp className="w-8 h-8 text-[#7FBF8E]" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Peak Records</p>
                  <p className="text-xl font-bold text-[#7FBF8E] mt-0.5">15,570</p>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
            <button
              onClick={() => setIsDeletedExpanded(!isDeletedExpanded)}
              className="w-full p-5 flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition"
            >
              <ChevronDown className={`w-5 h-5 text-gray-400 transition-transform duration-300 ${isDeletedExpanded ? 'rotate-180' : ''}`} />
              <h3 className="text-base font-semibold text-white">Deleted Accounts</h3>
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
            <h3 className="text-lg font-semibold text-white mb-6 text-right border-b border-[#333] pb-4">
              Active Users
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