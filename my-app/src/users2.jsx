import UserDetails from './UserDetails';
import  { useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';

// Mock Data for the chart
const chartData = [
  { name: 'Jan', International: 30, Local: 15 },
  { name: 'Feb', International: 45, Local: 22 },
  { name: 'Mar', International: 40, Local: 35 },
  { name: 'Apr', International: 60, Local: 30 },
  { name: 'May', International: 75, Local: 45 },
  { name: 'Jun', International: 70, Local: 40 },
  { name: 'Jul', International: 85, Local: 55 },
  { name: 'Aug', International: 80, Local: 50 },
  { name: 'Sep', International: 95, Local: 65 },
  { name: 'Oct', International: 90, Local: 60 },
  { name: 'Nov', International: 110, Local: 75 },
  { name: 'Dec', International: 105, Local: 70 },
];

// Mock Data for Active Users
const activeUsers = [
  { id: 1, name: 'Sarah Ahmed', age: 25, companions: 1, location: 'Atlantis Hotel, Palm Jumeirah, Dubai, UAE', programs: 3, companies: 2, joined: '3 days ago', avatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150' },
  { id: 2, name: 'John Smith', age: 32, companions: 3, location: 'Burj Al Arab, Jumeirah, Dubai, UAE', programs: 5, companies: 3, joined: '5 days ago', avatar: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150' },
  { id: 3, name: 'Aisha Khan', age: 28, companions: 0, location: 'Marina Heights, Dubai Marina, Dubai, UAE', programs: 2, companies: 1, joined: 'Joined: Mar 5, 2026', avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150' },
];

// Mock Data for Deleted Users
const deletedUsers = [
  { id: 4, name: 'Michael Brown', age: 40, companions: 2, location: 'Downtown Hotel, Rooftop Ave, Dubai, UAE', programs: 1, companies: 1, joined: 'Deleted: 2 days ago', avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150' },
  { id: 5, name: 'Elena Rostova', age: 22, companions: 1, location: 'JBR Apartments, Cluster T, Dubai, UAE', programs: 4, companies: 2, joined: 'Deleted: May 12, 2026', avatar: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150' },
];

export default function Users() {
  const [isDeletedExpanded, setIsDeletedExpanded] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

  // Reusable Component for User Card
  const UserCard = ({ user }) => (
    <div   onClick={() => setSelectedUser(user)}
    className="flex justify-between items-center bg-[#1a1c24] border border-[#2d303e] p-5 rounded-2xl transition hover:border-[#91B3FA]/40 shadow-md cursor-pointer"
    >
      {/* Left: Registration Time */}
      <div className="text-xs text-gray-500 font-medium whitespace-nowrap">
        {user.joined}
      </div>

      {/* Right: Info + Profile Image */}
      <div className="flex items-center gap-4 text-right">
        <div className="space-y-1.5">
          <h4 className="text-base font-bold text-white">{user.name}</h4>
          {/* Row 1: Age & Companions */}
          <p className="text-xs text-gray-400 flex items-center justify-end gap-3">
            <span>🎂 Age: {user.age}</span>
            <span>•</span>
            <span>👥 Companions: {user.companions}</span>
          </p>
          {/* Row 2: Full Location */}
          <p className="text-xs text-[#91B3FA] flex items-center justify-end gap-1">
            <span>📍 {user.location}</span>
          </p>
          {/* Row 3: Programs & Companies */}
          <p className="text-xs text-gray-400 flex items-center justify-end gap-3">
            <span>🗺️ Programs: {user.programs}</span>
            <span>•</span>
            <span>🏢 With {user.companies} Companies</span>
          </p>
        </div>
        {/* Profile Avatar Container */}
        <img 
          src={user.avatar} 
          alt={user.name} 
          className="w-16 h-16 rounded-full object-cover border-2 border-[#2d303e]"
        />
      </div>
    </div>
  );

  if (selectedUser) {
  return <UserDetails user={selectedUser} onBack={() => setSelectedUser(null)} />;
}
  
  return (
    <div className="space-y-8 animate-fadeIn">
      {/* Layout Split: Left (Chart & Actions) | Right (Active Users List) */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        
        {/* LEFT COLUMN: Main Chart & Deleted Accordion (7/12 Width) */}
        <div className="lg:col-span-7 space-y-6">
          
          {/* Large Wide Line Chart Container */}
          <div className="bg-[#1a1c24] border border-[#2d303e] p-6 rounded-3xl shadow-xl">
            <h3 className="text-lg font-bold text-gray-300 mb-4 text-left">Tourist Growth Analysis</h3>
            <div className="w-full h-72">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#2d303e" />
                  <XAxis dataKey="name" stroke="#64748b" />
                  <YAxis stroke="#64748b" />
                  <Tooltip contentStyle={{ backgroundColor: '#1a1c24', borderColor: '#2d303e', color: '#fff' }} />
                  <Legend />
                  <Line type="monotone" dataKey="International" stroke="#60a5fa" strokeWidth={3} dot={{ r: 4 }} />
                  <Line type="monotone" dataKey="Local" stroke="#f4a261" strokeWidth={3} dot={{ r: 4 }} />
                </LineChart>
              </ResponsiveContainer>
            </div>

            {/* Stats Row Directly Under Chart */}
            <div className="grid grid-cols-3 gap-4 mt-6 pt-6 border-t border-[#2d303e]">
              <div className="text-center p-3 bg-[#0f1117] rounded-xl border border-[#2d303e]">
                <p className="text-[10px] font-bold text-gray-500 uppercase tracking-wider">Active Registered</p>
                <p className="text-xl font-extrabold text-white mt-1">12,450</p>
              </div>
              <div className="text-center p-3 bg-[#0f1117] rounded-xl border border-[#2d303e]">
                <p className="text-[10px] font-bold text-gray-500 uppercase tracking-wider">Deleted Accounts</p>
                <p className="text-xl font-extrabold text-red-400 mt-1">3,120</p>
              </div>
              <div className="text-center p-3 bg-[#0f1117] rounded-xl border border-[#2d303e]">
                <p className="text-[10px] font-bold text-gray-500 uppercase tracking-wider">Peak Records</p>
                <p className="text-xl font-extrabold text-green-400 mt-1">15,570</p>
              </div>
            </div>
          </div>

          {/* Expandable Box (Accordion): Users Who Deleted Accounts */}
          <div className="bg-[#1a1c24] border border-[#2d303e] rounded-3xl overflow-hidden shadow-xl transition-all duration-300">
            <button 
              onClick={() => setIsDeletedExpanded(!isDeletedExpanded)}
              className="w-full p-6 flex justify-between items-center bg-[#1e202b] hover:bg-[#252836] transition"
            >
              <span className={`transform transition-transform text-lg ${isDeletedExpanded ? 'rotate-180' : ''}`}>▼</span>
              <h3 className="text-base font-bold text-gray-300">Users who deleted their accounts</h3>
            </button>
            
            {isDeletedExpanded && (
              <div className="p-6 space-y-4 max-h-[400px] overflow-y-auto bg-[#1a1c24] border-t border-[#2d303e]">
                {deletedUsers.map(user => (
                  <UserCard key={user.id} user={user} />
                ))}
              </div>
            )}
          </div>

        </div>

        {/* RIGHT COLUMN: Fixed Height Container for Tourists Using App (5/12 Width) */}
        <div className="lg:col-span-5">
          <div className="bg-[#1a1c24] border border-[#2d303e] rounded-3xl shadow-xl p-6 h-full flex flex-col">
            <h3 className="text-lg font-bold text-gray-300 mb-6 text-right border-b border-[#2d303e] pb-4">
              Tourists who are using the app
            </h3>
            
            {/* Scrollable list zone */}
            <div className="space-y-4 overflow-y-auto flex-1 pr-1 max-h-[600px] custom-scrollbar">
              {activeUsers.map(user => (
                <UserCard key={user.id} user={user} />
              ))}
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}




