import { useState } from 'react';
// قمنا بتغيير اسم أيقونة Home إلى HomeIcon لتجنب التعارض مع مكون واجهة Home
import { 
  Home as HomeIcon, Users as UserIcon , Map, Building2, Wallet, HeadphonesIcon,
  Bell, Settings as SetIcon, Calendar, MessageSquare, HelpCircle,  
} from 'lucide-react';

// استدعاء واجهات النظام الداخلي
 import Home from './screens/home';
// //import Users from './users';س
// //import Companies from './companies';
// import Companies from './companies2';
// import GroupTrip from './grouptrip';
// import Support from './support';
// import Notifications from './Notifications';
// import Settings from './settings';
// import CompanyDetails from './companydetailes2';
// import UserDetails from './UserDetails';
// import AdminProfile from './profile';
// import Financials from './financial';
// import ProgramDetails from './programDetailes';
// import Users from './users'; // يمكنك إزالة التعليق لاحقاً عند إنشاء ملف المستخدمين

export default function App() {
  const [activeMenu, setActiveMenu] = useState('Home');

  return (
    <div className="min-h-screen bg-[#121212] text-white font-sans flex overflow-hidden">
      
      {/* 1. القائمة الجانبية (Sidebar) */}
      <aside className="w-64 bg-[#181818] border-r border-[#2a2a2a] flex flex-col justify-between hidden md:flex">
        <div>
          <div className="p-6">
            <h1 className="text-2xl font-bold text-white tracking-wide">UTE Tourism</h1>
            <p className="text-xs text-gray-400 tracking-widest mt-1">ADMIN DASHBOARD</p>
          </div>
          
          <nav className="px-4 space-y-2 mt-4">
            {[
              { id: 'Home', icon: HomeIcon },
              { id: 'Users', icon: UserIcon },
              { id: 'Group Trips', icon: Map },
              { id: 'Companies', icon: Building2 },
              { id: 'Financials', icon: Wallet },
              { id: 'Support', icon: HeadphonesIcon },
            ].map((item) => (
              <button
                key={item.id}
                onClick={() => setActiveMenu(item.id)}
                className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
                  activeMenu === item.id 
                  ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]' 
                  : 'text-gray-400 hover:text-white hover:bg-[#202020]'
                }`}
              >
                <item.icon className="w-5 h-5 mr-3" />
                <span className="font-medium">{item.id}</span>
              </button>
            ))}
          </nav>
        </div>

        <div className="p-4 space-y-2 mb-4">
          <button
            onClick={() => setActiveMenu('Notifications')}
            className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
              activeMenu === 'Notifications'
                ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]'
                : 'text-gray-400 hover:text-white hover:bg-[#202020]'
            }`}
          >
            <Bell className="w-5 h-5 mr-3" />
            <span className="font-medium flex-1 text-left">Notifications</span>
            <span className="bg-[#F4A261] text-black text-xs font-bold px-2 py-0.5 rounded-full">3</span>
          </button>
          <button
            onClick={() => setActiveMenu('Settings')}
            className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-10 ${
              activeMenu === 'Settings'
                ? 'bg-[#1e2330] text-[#91B3FA] border border-[#91B3FA] shadow-[0_0_15px_rgba(145,179,250,0.2)]'
                : 'text-gray-400 hover:text-white hover:bg-[#202020]'
            }`}
          >
            <SetIcon className="w-5 h-5 mr-3" />
            <span className="font-medium">Settings</span>
          </button>
        </div>
      </aside>

      {/* 2. منطقة المحتوى الرئيسية */}
      <main className="flex-1 flex flex-col h-screen overflow-y-auto">
        
        {/* الشريط العلوي (Topbar) */}
        <header className="flex items-center justify-between px-8 py-5 bg-[#121212]/90 sticky top-0 z-10 backdrop-blur-md border-b border-[#2a2a2a]/50">
          <div>
            <h2 className="text-2xl font-semibold">
              Welcome, Admin <span className="text-sm font-normal text-gray-400 ml-3">Jun 13, 2026</span>
            </h2>
          </div>
          <div className="flex items-center space-x-6 space-y-0">
            {/* <Calendar className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */}
            {/* <MessageSquare className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */}
            {/* <HelpCircle className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white transition" /> */} 
            <button className="w-15 h-15 rounded-full border-2 border-[#91B3FA] overflow-hidden cursor-pointer space-y-0 space-x-6"

          onClick={() => setActiveMenu('AdminProfile')}

                  
                
            >
              <img src="https://i.pravatar.cc/150?img=11" alt="Admin" className="w-full h-full object-cover" />
              <h2 className="text-2xl font-semibold">
              admins name
            </h2>
            </button>
          </div>
        </header>

        {/* عرض المحتوى الديناميكي هنا بناءً على القائمة الجانبية */}
         {activeMenu === 'Home' && <Home />}
        {/*{activeMenu === 'Users' && <Users />}
        {activeMenu === 'Companies' && <Companies />}
        {activeMenu === 'Group Trips' && <GroupTrip />}
        {activeMenu === 'Support' && <UserDetails />}
        {activeMenu === 'Financials' && <ProgramDetails />}
        {activeMenu === 'Notifications' && <Notifications />}
        {activeMenu === 'Settings' && <Settings />}
        {activeMenu === 'AdminProfile' && <AdminProfile />} */}

        {/* {activeMenu === 'Home' && <Home />}
        {activeMenu === 'Users' && <Users />} */}
        {/* {activeMenu === 'Users' && <Users />} */}

      </main>
    </div>
  );
}





// import Login from './login'; // تأكد من صحة مسار الاستيراد

// function App() {
//   return (
//     <Login />
//   );
// }

// export default App;