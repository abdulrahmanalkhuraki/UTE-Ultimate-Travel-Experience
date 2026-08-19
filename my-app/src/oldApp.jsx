import { useState } from 'react';
import Home from './home';      // ملف الـ Dashboard الرئيسي القديم
import Users from './users';    // ملف الـ Users الجديد الذي أنشأناه الآن

function App() {
  // الحالة المسؤولة عن تحديد الصفحة المعروضة حالياً
  const [activePage, setActivePage] = useState('Home');

  return (
    <div className="flex min-h-screen bg-[#0f1117] text-white">
      
      {/* 1. البار الجانبي الثابت (Sidebar) */}
      <aside className="w-64 border-r border-[#2d303e] p-6 flex flex-col justify-between fixed h-full bg-[#0f1117]">
        <div className="space-y-10">
          <div>
            <h2 className="text-xl font-bold tracking-wider text-white">UTE Tourism</h2>
            <p className="text-[10px] text-gray-500 font-semibold tracking-widest mt-1 uppercase">Admin Dashboard</p>
          </div>
          
          <nav className="space-y-3">
            {/* زر Home */}
            <button 
              onClick={() => setActivePage('Home')}
              className={`w-full text-left p-3.5 rounded-xl font-medium transition duration-200 flex items-center gap-3 ${
                activePage === 'Home' 
                  ? 'bg-[#2d303e] text-[#91B3FA] shadow-lg border border-[#3e4257]' 
                  : 'text-gray-400 hover:bg-[#1a1c24] hover:text-white'
              }`}
            >
              <span>🏠</span> Home
            </button>

            {/* زر Users */}
            <button 
              onClick={() => setActivePage('Users')}
              className={`w-full text-left p-3.5 rounded-xl font-medium transition duration-200 flex items-center gap-3 ${
                activePage === 'Users' 
                  ? 'bg-[#2d303e] text-[#91B3FA] shadow-lg border border-[#3e4257]' 
                  : 'text-gray-400 hover:bg-[#1a1c24] hover:text-white'
              }`}
            >
              <span>👥</span> Users
            </button>

            {/* باقي عناصر القائمة الجانبية (تعمل كأزرار وهمية حالياً) */}
            {['Group Trips', 'Companies', 'Financials', 'Support'].map((item, index) => (
              <button 
                key={index}
                className="w-full text-left p-3.5 rounded-xl font-medium text-gray-400 hover:bg-[#1a1c24] hover:text-white transition duration-200"
              >
                {item}
              </button>
            ))}
          </nav>
        </div>

        {/* إعدادات وتنبيهات أسفل البار */}
        <div className="space-y-3 border-t border-[#2d303e] pt-4 text-sm text-gray-400">
          <div className="p-2 hover:text-white cursor-pointer transition">🔔 Notifications</div>
          <div className="p-2 hover:text-white cursor-pointer transition">⚙️ Settings</div>
        </div>
      </aside>

      {/* 2. منطقة المحتوى والبار العلوي المتغيرين (Main Content View Container) */}
      {/* أضفنا ml-64 لأن البار الجانبي أخذ حيزاً ثابتاً بمقدار 64 وحدة */}
      <div className="flex-1 ml-64 flex flex-col min-h-screen">
        
        {/* البار العلوي الثابت (Top Navbar) */}
        <header className="border-b border-[#2d303e] bg-[#10121a]/80 backdrop-blur-md px-8 py-4 flex justify-between items-center sticky top-0 z-40">
          <h1 className="text-lg font-bold text-white flex items-center gap-2">
            System Control Panel <span className="text-xs font-normal text-gray-500">/ {activePage}</span>
          </h1>
          <div className="flex items-center gap-6">
            <span className="text-xs text-gray-400 font-mono">Jun 13, 2026</span>
            <div className="w-9 h-9 rounded-full bg-gradient-to-tr from-[#f4a261] to-[#91B3FA] p-[2px] cursor-pointer">
              <div className="w-full h-full bg-[#1a1c24] rounded-full flex items-center justify-center text-sm">👨‍💼</div>
            </div>
          </div>
        </header>

        {/* عرض الصفحة المطلوبة بناءً على الـ State */}
        <main className="p-8 flex-1 bg-[#0f1117]">
          {activePage === 'Home' && <Home />}
          {activePage === 'Users' && <Users />}
        </main>

      </div>

    </div>
  );
}

export default App;





// import Dashboard from './home'
// import Login from './login'
// import Home from './home2'
// import './App.css'

// function App() {

 
//   return (
// //<Home/>
// //<Login />
//      <div className="App">
//      <Dashboard />
//      </div>
//  )
// }

// export default App
