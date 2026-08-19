// import { useState } from 'react';
// // قمنا بتغيير اسم أيقونة Home إلى HomeIcon لتجنب التعارض مع مكون واجهة Home
// import { 
//   Home as HomeIcon, Users as UserIcon , Map, Building2, Wallet, HeadphonesIcon,
//   Bell, Settings as SetIcon, Calendar, MessageSquare, HelpCircle,  
// } from 'lucide-react';

// // استدعاء واجهات النظام الداخلي
// import Home from './home';
// //import Users from './users';س
// //import Companies from './companies';
// import Companies from './companies2';
// import GroupTrip3 from './grouptrip1';
// import Support from './support';
// import Notifications from './Notifications';
// import Settings from './settings';
// import CompanyDetails from './companydetailes2';
// import UserDetails from './UserDetails';
// import AdminProfile from './profile';
// import Financials from './financial';
// import ProgramDetails from './programDetailes';
// import Users from './users'; // يمكنك إزالة التعليق لاحقاً عند إنشاء ملف المستخدمين

// export default function App() {
//   const [activeMenu, setActiveMenu] = useState('Home');

//   return (
//     <div className="min-h-screen bg-[#121212] text-white font-sans flex overflow-hidden">
      
//       {/* 1. القائمة الجانبية (Sidebar) */}
//       <aside className="w-64 bg-[#181818] border-r border-[#2a2a2a] flex flex-col justify-between hidden md:flex">
//         <div>
//           <div className="p-6">
//             <h1 className="text-2xl font-bold text-white tracking-wide">UTE Tourism</h1>
//             <p className="text-xs text-gray-400 tracking-widest mt-1">ADMIN DASHBOARD</p>
//           </div>
          
//           <nav className="px-4 space-y-2 mt-4">
//             {[
//               { id: 'Home', icon: HomeIcon },
//               { id: 'Users', icon: UserIcon },
//               { id: 'Group Trips', icon: Map },
//               { id: 'Companies', icon: Building2 },
//               { id: 'Financials', icon: Wallet },
//               { id: 'Support', icon: HeadphonesIcon },
//             ].map((item) => (
//               <button
//                 key={item.id}
//                 onClick={() => setActiveMenu(item.id)}
//                 className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
//                   activeMenu === item.id 
//                   ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]' 
//                   : 'text-gray-400 hover:text-white hover:bg-[#202020]'
//                 }`}
//               >
//                 <item.icon className="w-5 h-5 mr-3" />
//                 <span className="font-medium">{item.id}</span>
//               </button>
//             ))}
//           </nav>
//         </div>

//         <div className="p-4 space-y-2 mb-4">
//           <button /*className="w-full flex items-center px-4 py-3 text-gray-400 hover:text-white hover:bg-[#202020] rounded-lg"*/
//           key={Notifications}
//           onClick={() => setActiveMenu('Notifications')}
//                 className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
//                   activeMenu === Notifications 
//                   ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]' 
//                   : 'text-gray-400 hover:text-white hover:bg-[#202020]'
//                 }`}
//           >
//             <Bell className="w-5 h-5 mr-3" />
//             <span className="font-medium flex-1 text-left">Notifications</span>
//             <span className="bg-[#F4A261] text-black text-xs font-bold px-2 py-0.5 rounded-full">3</span>
            
//           </button>
//           <button /*className="w-full flex items-center px-4 py-3 text-gray-400 hover:text-white hover:bg-[#202020] rounded-lg"*/
//           key={Settings}
//           onClick={() => setActiveMenu('Settings')}
//                 className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
//                   activeMenu === Settings 
//                   ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]' 
//                   : 'text-gray-400 hover:text-white hover:bg-[#202020]'
//                 }`}
//           >
//             <SetIcon className="w-5 h-5 mr-3" />
//             <span className="font-medium">Settings</span>
//           </button>
//         </div>
//       </aside>

//       {/* 2. منطقة المحتوى الرئيسية */}
//       <main className="flex-1 flex flex-col h-screen overflow-y-auto">
        
//         {/* الشريط العلوي (Topbar) */}
//         <header className="flex items-center justify-between px-8 py-5 bg-[#121212]/90 sticky top-0 z-10 backdrop-blur-md border-b border-[#2a2a2a]/50">
//           <div>
//             <h2 className="text-2xl font-semibold">
//               Welcome, Admin <span className="text-sm font-normal text-gray-400 ml-3">Jun 13, 2026</span>
//             </h2>
//           </div>
//           <div className="flex items-center space-x-6 space-y-0">
//             {/* <Calendar className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */}
//             {/* <MessageSquare className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */}
//             {/* <HelpCircle className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */} 
//             <button className="w-15 h-15 rounded-full border-2 border-[#91B3FA] overflow-hidden cursor-pointer space-y-0 space-x-6"

//           onClick={() => setActiveMenu('AdminProfile')}
               
                  
                
//             >
//               <img src="https://i.pravatar.cc/150?img=11" alt="Admin" className="w-full h-full object-cover" />
//               <h2 className="text-2xl font-semibold">
//               admins name
//             </h2>
//             </button>
//           </div>
//         </header>

//         {/* عرض المحتوى الديناميكي هنا بناءً على القائمة الجانبية */}
//         {activeMenu === 'Home' && <Home />}
//         {activeMenu === 'Users' && <Users />}
//         {activeMenu === 'Companies' && <Companies />}
//         {activeMenu === 'Group Trips' && <GroupTrip3 />}
//         {activeMenu === 'Support' && <ProgramDetails />}
//         {activeMenu === 'Financials' && <ProgramDetails />}
//         {activeMenu === 'Notifications' && <Notifications />}
//         {activeMenu === 'Settings' && <Settings />}
//         {activeMenu === 'AdminProfile' && <AdminProfile />}

//         {/* {activeMenu === 'Home' && <Home />}
//         {activeMenu === 'Users' && <Users />} */}
//         {/* {activeMenu === 'Users' && <Users />} */}

//       </main>
//     </div>
//   );
// }





// // import Login from './login'; // تأكد من صحة مسار الاستيراد

// // function App() {
// //   return (
// //     <Login />
// //   );
// // }

// // export default App;
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






// 