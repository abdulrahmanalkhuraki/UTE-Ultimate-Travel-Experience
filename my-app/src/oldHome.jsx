

export default function Home() {
  return (
    <div className="space-y-8">
      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        {[
          { title: 'TOTAL TOURISTS', value: '12,450' },
          { title: 'ACTIVE COMPANIES', value: '45' },
          { title: 'PUBLISHED PROGRAMS', value: '320' },
          { title: 'TOTAL REVENUE', value: '$1.2M' },
        ].map((stat, i) => (
          <div key={i} className="bg-[#1a1c24] p-6 rounded-xl border border-[#2d303e]">
            <p className="text-gray-400 text-xs font-bold mb-2">{stat.title}</p>
            <h3 className="text-2xl font-bold">{stat.value}</h3>
          </div>
        ))}
      </div>

      {/* Tables Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Pending Group Trips */}
        <div className="bg-[#1a1c24] p-6 rounded-xl border border-[#2d303e]">
          <h2 className="font-bold mb-6">Pending Group Trips</h2>
          {/* كود الجدول الخاص بك هنا... */}
        </div>

        {/* Pending Applications */}
        <div className="bg-[#1a1c24] p-6 rounded-xl border border-[#2d303e]">
          <h2 className="font-bold mb-6">Pending Company Applications</h2>
          {/* كود الجدول الخاص بك هنا... */}
        </div>
      </div>
    </div>
  );
}



// import  { useState } from 'react';
// import { 
//   Home, Users, Map, Building2, Wallet, HeadphonesIcon, 
//   Bell, Settings, Calendar, MessageSquare, HelpCircle,  
// } from 'lucide-react';
// import { 
//   LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, 
//   Tooltip, Legend, ResponsiveContainer 
// } from 'recharts';

// // --- البيانات الوهمية للمخططات ---
// const growthData = [
//   { name: 'Jan', local: 30, international: 15 },
//   { name: 'Feb', local: 50, international: 20 },
//   { name: 'Mar', local: 65, international: 25 },
//   { name: 'Apr', local: 55, international: 22 },
//   { name: 'May', local: 70, international: 30 },
//   { name: 'Jun', local: 95, international: 40 },
//   { name: 'Jul', local: 75, international: 35 },
//   { name: 'Aug', local: 90, international: 45 },
//   { name: 'Sep', local: 110, international: 55 },
//   { name: 'Oct', local: 105, international: 80 },
//   { name: 'Nov', local: 120, international: 70 },
//   { name: 'Dec', local: 135, international: 90 },
// ];

// const companyData = [
//   { name: 'Jan', company: 40, program: 20 },
//   { name: 'Feb', company: 48, program: 25 },
//   { name: 'Mar', company: 55, program: 30 },
//   { name: 'Apr', company: 65, program: 32 },
//   { name: 'May', company: 72, program: 40 },
//   { name: 'Jun', company: 80, program: 45 },
//   { name: 'Jul', company: 85, program: 50 },
//   { name: 'Aug', company: 90, program: 50 },
//   { name: 'Sep', company: 95, program: 62 },
//   { name: 'Oct', company: 105, program: 65 },
//   { name: 'Nov', company: 120, program: 70 },
//   { name: 'Dec', company: 120, program: 68 },
// ];

// export default function Dashboard() {
//   const [activeMenu, setActiveMenu] = useState('Home');

//   return (
//     // الخلفية الأساسية (الداكنة مع نمط خفيف يمكن إضافة رابط صورة له لاحقاً)
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
//               { id: 'Home', icon: Home },
//               { id: 'Users', icon: Users },
//               { id: 'Group Trips', icon: Map },
//               { id: 'Companies', icon: Building2 },
//               { id: 'Financials', icon: Wallet },
//               { id: 'Support', icon: HeadphonesIcon },
//             ].map((item) => (
//               <button
//                 key={item.id}
//                 onClick={() => setActiveMenu(item.id)}
//                 className={`w-full flex items-center px-4 py-3 rounded-lg transition-all duration-300 ${
//                   activeMenu === item.id 
//                   // تأثير التوهج الأزرق للعنصر النشط
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
//           <button className="w-full flex items-center px-4 py-3 text-gray-400 hover:text-white hover:bg-[#202020] rounded-lg">
//             <Bell className="w-5 h-5 mr-3" />
//             <span className="font-medium flex-1 text-left">Notifications</span>
//             <span className="bg-[#F4A261] text-black text-xs font-bold px-2 py-0.5 rounded-full">3</span>
//           </button>
//           <button className="w-full flex items-center px-4 py-3 text-gray-400 hover:text-white hover:bg-[#202020] rounded-lg">
//             <Settings className="w-5 h-5 mr-3" />
//             <span className="font-medium">Settings</span>
//           </button>
//         </div>
//       </aside>

//       {/* 2. منطقة المحتوى الرئيسية */}
//       <main className="flex-1 flex flex-col h-screen overflow-y-auto">
        
//         {/* الشريط العلوي (Topbar) */}
//         <header className="flex items-center justify-between px-8 py-5 bg-[#121212]/90 sticky top-0 z-10 backdrop-blur-md">
//           <div>
//             <h2 className="text-2xl font-semibold">Welcome, Admin <span className="text-sm font-normal text-gray-400 ml-3">Jun 13, 2026</span></h2>
//           </div>
//           <div className="flex items-center space-x-6">
//             <Calendar className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white" />
//             <MessageSquare className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white" />
//             <HelpCircle className="w-5 h-5 text-gray-400 cursor-pointer hover:text-white" />
//             <div className="w-10 h-10 rounded-full border-2 border-[#91B3FA] overflow-hidden cursor-pointer">
//               <img src="https://i.pravatar.cc/150?img=11" alt="Admin" className="w-full h-full object-cover" />
//             </div>
//           </div>
//         </header>

//         {/* محتوى لوحة التحكم */}
//         <div className="p-8 space-y-6">
          
//           {/* البطاقات العلوية (Stats Cards) */}
//           <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
//             {[
//               { title: 'TOTAL TOURISTS', value: '12,450', icon: Users, color: '#91B3FA' },
//               { title: 'ACTIVE COMPANIES', value: '45', icon: Building2, color: '#91B3FA' },
//               { title: 'PUBLISHED PROGRAMS', value: '320', icon: Map, color: '#91B3FA' },
//               { title: 'TOTAL REVENUE', value: '$1.2M', icon: Wallet, color: '#F4A261', highlight: true },
//             ].map((stat, idx) => (
//               <div key={idx} className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 relative overflow-hidden shadow-lg group cursor-pointer hover:border-[#D4AF37]/70 transition-all">
//                 <div className="flex justify-between items-start">
//                   <div>
//                     <p className="text-xs text-gray-400 font-semibold tracking-wider mb-2">{stat.title}</p>
//                     <h3 className={`text-4xl font-bold ${stat.highlight ? 'text-[#F4A261]' : 'text-white'}`}>{stat.value}</h3>
//                   </div>
//                   <div className="bg-[#2A2A2D] p-3 rounded-lg">
//                     <stat.icon className="w-6 h-6" style={{ color: stat.color }} />
//                   </div>
//                 </div>
//               </div>
//             ))}
//           </div>

//           {/* قسم المخططات البيانية (Charts) */}
//           <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 h-[400px]">
//             {/* المخطط الخطي */}
//             <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 flex flex-col">
//               <div className="text-center mb-4">
//                 <h3 className="text-lg font-semibold">Tourist Growth Over Time</h3>
//                 <p className="text-sm text-gray-400">(past 12 months)</p>
//               </div>
//               <ResponsiveContainer width="100%" height="100%">
//                 <LineChart data={growthData}>
//                   <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
//                   <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
//                   <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
//                   <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#333'}} />
//                   <Legend iconType="plainline" />
//                   <Line type="monotone" dataKey="local" stroke="#91B3FA" strokeWidth={3} dot={false} name="Local" />
//                   <Line type="monotone" dataKey="international" stroke="#F4A261" strokeWidth={3} dot={false} name="International" />
//                 </LineChart>
//               </ResponsiveContainer>
//             </div>

//             {/* المخطط الشريطي */}
//             <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 flex flex-col">
//               <div className="text-center mb-4">
//                 <h3 className="text-lg font-semibold">Company and Program Growth</h3>
//               </div>
//               <ResponsiveContainer width="100%" height="100%">
//                 <BarChart data={companyData}>
//                   <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
//                   <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
//                   <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
//                   <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#333'}} cursor={{fill: 'rgba(255,255,255,0.05)'}} />
//                   <Legend iconType="rect" />
//                   <Bar dataKey="company" fill="#91B3FA" name="Company" radius={[2, 2, 0, 0]} />
//                   <Bar dataKey="program" fill="#F4A261" name="Program Growth" radius={[2, 2, 0, 0]} />
//                 </BarChart>
//               </ResponsiveContainer>
//             </div>
//           </div>

//           {/* الجداول السفلية (Tables) */}
//           <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
//             {/* جدول الرحلات */}
//             <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30">
//               <h3 className="text-xl font-semibold mb-6">Pending Group Trips</h3>
//               <table className="w-full text-sm text-left">
//                 <thead className="text-xs text-gray-400 uppercase border-b border-[#333]">
//                   <tr>
//                     <th className="px-4 py-3">TRIP NAME</th>
//                     <th className="px-4 py-3">COMPANY NAME</th>
//                     <th className="px-4 py-3 text-right">ACTIONS</th>
//                   </tr>
//                 </thead>
//                 <tbody>
//                   {[
//                     { trip: 'Alpine Explorer', comp: 'Summit Treks Ltd.' },
//                     { trip: 'Desert Safari Ext.', comp: 'Dune Navigators' },
//                     { trip: 'Coastal Retreat', comp: 'Oceanic Ventures' }
//                   ].map((row, idx) => (
//                     <tr key={idx} className="border-b border-[#222] hover:bg-[#252528] transition-colors">
//                       <td className="px-4 py-4 font-medium text-white">{row.trip}</td>
//                       <td className="px-4 py-4 text-gray-300">{row.comp}</td>
//                       <td className="px-4 py-4 flex justify-end gap-2">
//                         <button className="px-4 py-2 bg-[#2A2A2D] text-gray-300 rounded hover:bg-[#333] transition">Reject</button>
//                         <button className="px-4 py-2 bg-[#91B3FA] text-black font-medium rounded hover:bg-[#7fa1e8] transition">Approve</button>
//                       </td>
//                     </tr>
//                   ))}
//                 </tbody>
//               </table>
//             </div>

//             {/* جدول الشركات */}
//             <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30">
//               <h3 className="text-xl font-semibold mb-6">Pending Company Applications</h3>
//               <table className="w-full text-sm text-left">
//                 <thead className="text-xs text-gray-400 uppercase border-b border-[#333]">
//                   <tr>
//                     <th className="px-4 py-3">COMPANY NAME</th>
//                     <th className="px-4 py-3">STATUS</th>
//                     <th className="px-4 py-3 text-right">ACTIONS</th>
//                   </tr>
//                 </thead>
//                 <tbody>
//                   {[
//                     { comp: 'Global Trails Inc.', status: 'Under Review' },
//                     { comp: 'Urban Odyssey', status: 'Docs Pending' },
//                     { comp: 'Eco Escapes LLC', status: 'New' }
//                   ].map((row, idx) => (
//                     <tr key={idx} className="border-b border-[#222] hover:bg-[#252528] transition-colors">
//                       <td className="px-4 py-4 font-medium text-white">{row.comp}</td>
//                       <td className="px-4 py-4">
//                         <span className="text-[#F4A261] text-xs font-semibold">{row.status}</span>
//                       </td>
//                       <td className="px-4 py-4 flex justify-end gap-2">
//                         <button className="px-4 py-2 bg-[#2A2A2D] text-gray-300 rounded hover:bg-[#333] transition">Reject</button>
//                         <button className="px-4 py-2 bg-[#91B3FA] text-black font-medium rounded hover:bg-[#7fa1e8] transition">Approve</button>
//                       </td>
//                     </tr>
//                   ))}
//                 </tbody>
//               </table>
//             </div>
//           </div>

//         </div>
//       </main>
//     </div>
//   );
// }