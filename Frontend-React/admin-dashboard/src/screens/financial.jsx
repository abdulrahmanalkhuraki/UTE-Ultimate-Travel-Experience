
import  { useState } from 'react'; // تأكد من استيراد useState
import CompanyFinancialDetails from './CompanyFinancialDetails'; // <--- الإضافة هنا
//import { ... } from 'lucide-react';

import { 
  DollarSign, Wallet, Building2, MapPin, TrendingUp, BarChart3
} from 'lucide-react';
import { 
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer 
} from 'recharts';

// === 1. البيانات الوهمية للمخطط البياني (تزايد الربح) ===
const profitData = [
  { month: 'Jan', profit: 15000 },
  { month: 'Feb', profit: 22000 },
  { month: 'Mar', profit: 28000 },
  { month: 'Apr', profit: 35000 },
  { month: 'May', profit: 42000 },
  { month: 'Jun', profit: 48000 },
  { month: 'Jul', profit: 60000 },
  { month: 'Aug', profit: 75000 },
  { month: 'Sep', profit: 82000 },
  { month: 'Oct', profit: 95000 },
  { month: 'Nov', profit: 110000 },
  { month: 'Dec', profit: 125000 },
];

// === 2. البيانات الوهمية للشركات ===
const companiesFinancials = [
  {
    id: 1,
    name: "Global Trails Inc.",
    logo: "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80",
    location: "Dubai, UAE",
    companyEarnings: 850000,
    appProfit: 125000, // ربحنا من هذه الشركة
  },
  {
    id: 2,
    name: "Desert Navigators",
    logo: "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=150&q=80",
    location: "Abu Dhabi, UAE",
    companyEarnings: 420000,
    appProfit: 63000,
  },
  {
    id: 3,
    name: "Summit Treks Ltd.",
    logo: "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=150&q=80",
    location: "Sharjah, UAE",
    companyEarnings: 980000,
    appProfit: 147000,
  },
  {
    id: 4,
    name: "Oceanic Ventures",
    logo: "https://images.unsplash.com/photo-1497366216548-37526070297c?w=150&q=80",
    location: "Ajman, UAE",
    companyEarnings: 150000,
    appProfit: 22500,
  },
];

export default function Financials() {

    const [selectedCompany, setSelectedCompany] = useState(null);
  // === 3. العمليات الحسابية والترتيب ===
  // ترتيب الشركات من الأكبر ربحاً لنا إلى الأصغر
  const sortedCompanies = [...companiesFinancials].sort((a, b) => b.appProfit - a.appProfit);

  // حساب الإحصائيات الكلية من البيانات
  const totalAppProfit = sortedCompanies.reduce((sum, company) => sum + company.appProfit, 0);
  const totalCompaniesEarnings = sortedCompanies.reduce((sum, company) => sum + company.companyEarnings, 0);
  const totalVolume = totalAppProfit + totalCompaniesEarnings; // المبلغ الكلي المدفوع في التطبيق

  
  
  if (selectedCompany) {
    return (
      <CompanyFinancialDetails 
        company={selectedCompany} 
        onBack={() => setSelectedCompany(null)} 
      />
    );
  }
  
  
  return (
    <div className="p-8 space-y-8 font-sans animate-in fade-in duration-300">
      
      {/* الترويسة */}
      <div className="flex items-center justify-between border-b border-[#333] pb-4">
        <div>
          <h1 className="text-3xl font-bold text-white flex items-center gap-3">
            <Wallet className="w-8 h-8 text-[#F4A261]" /> 
            Financial Overview
          </h1>
          <p className="text-sm text-gray-400 mt-1">Track app revenue, total spending, and companies' earnings</p>
        </div>
      </div>

      {/* === القسم العلوي: المخطط البياني والإحصائيات في نفس المربع === */}
      <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl shadow-lg overflow-hidden flex flex-col">
        
        {/* عنوان المخطط */}
        <div className="p-6 pb-2">
          <h3 className="text-lg font-bold text-white flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-[#91B3FA]" /> Profit Growth Over Time
          </h3>
        </div>

        {/* مساحة المخطط البياني */}
        <div className="w-full h-[350px] p-6 pt-0">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={profitData} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
              <defs>
                <linearGradient id="colorProfit" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#F4A261" stopOpacity={0.4}/>
                  <stop offset="95%" stopColor="#F4A261" stopOpacity={0}/>
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
              <XAxis dataKey="month" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
              <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} tickFormatter={(value) => `$${value/1000}k`} />
              <Tooltip 
                contentStyle={{backgroundColor: '#18181A', borderColor: '#F4A261', borderRadius: '8px'}} 
                itemStyle={{color: '#F4A261', fontWeight: 'bold'}}
              />
              <Area type="monotone" dataKey="profit" stroke="#F4A261" strokeWidth={3} fillOpacity={1} fill="url(#colorProfit)" />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        {/* الإحصائيات (المربعات الثلاثة أسفل المخطط داخل نفس الحاوية) */}
        <div className="grid grid-cols-1 md:grid-cols-3 divide-y md:divide-y-0 md:divide-x divide-[#333] border-t border-[#333] bg-[#18181A]/50">
          
          {/* 1. الربح الكلي للتطبيق */}
          <div className="p-6 flex flex-col justify-center hover:bg-[#ffffff05] transition-colors">
            <p className="text-xs text-gray-400 uppercase tracking-wider mb-2 flex items-center gap-2">
              <BarChart3 className="w-4 h-4 text-[#4ADE80]" /> Total App Profit
            </p>
            <h4 className="text-3xl font-bold text-[#4ADE80]">
              ${totalAppProfit.toLocaleString()}
            </h4>
          </div>

          {/* 2. المبلغ الكلي المدفوع في التطبيق */}
          <div className="p-6 flex flex-col justify-center hover:bg-[#ffffff05] transition-colors">
            <p className="text-xs text-gray-400 uppercase tracking-wider mb-2 flex items-center gap-2">
              <DollarSign className="w-4 h-4 text-[#91B3FA]" /> Total Volume Spent
            </p>
            <h4 className="text-3xl font-bold text-white">
              ${totalVolume.toLocaleString()}
            </h4>
          </div>

          {/* 3. ما حصلت عليه الشركات */}
          <div className="p-6 flex flex-col justify-center hover:bg-[#ffffff05] transition-colors">
            <p className="text-xs text-gray-400 uppercase tracking-wider mb-2 flex items-center gap-2">
              <Building2 className="w-4 h-4 text-[#91B3FA]" /> Total Companies Earnings
            </p>
            <h4 className="text-3xl font-bold text-white">
              ${totalCompaniesEarnings.toLocaleString()}
            </h4>
          </div>

        </div>
      </div>

      {/* === القسم السفلي: قائمة الشركات مرتبة حسب الربح === */}
      <div className="space-y-4">
        <h3 className="text-xl font-bold text-white mb-6 border-b border-[#333] pb-2">
          Companies Financial Breakdown <span className="text-sm font-normal text-gray-500 ml-2">(Sorted by Highest Profit)</span>
        </h3>

        <div className="flex flex-col gap-4">
          {sortedCompanies.map((company, index) => (
            <div 
              key={company.id} 
              onClick={() => setSelectedCompany(company)}
              className="bg-[#18181A] border border-[#333] rounded-2xl p-5 flex flex-col md:flex-row md:items-center justify-between gap-6 shadow-lg hover:border-[#91B3FA]/40 transition-all duration-300 relative overflow-hidden cursor-pointer"
            >
              {/* شريط رفيع على اليسار يميز أعلى شركة */}
              { <div className="absolute left-0 top-0 w-1 h-full bg-[#91B3FA]"></div>}

              {/* 1. اللوغو، الاسم والموقع (الجهة اليسرى) */}
              <div className="flex items-center gap-4 flex-1">
                <div className="w-14 h-14 rounded-xl overflow-hidden border border-[#D4AF37]/30 shrink-0">
                  <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
                </div>
                <div>
                  <h4 className="text-lg font-bold text-white">{company.name}</h4>
                  <p className="text-sm text-gray-400 flex items-center gap-1 mt-1">
                    <MapPin className="w-3.5 h-3.5" /> {company.location}
                  </p>
                </div>
              </div>

              {/* الأرقام المادية (الجهة اليمنى) */}
              <div className="flex items-center gap-8 md:gap-12 flex-wrap md:flex-nowrap bg-[#121212] py-3 px-6 rounded-xl border border-[#222]">
                
                {/* 2. ربح الشركة */}
                <div className="flex flex-col text-left">
                  <span className="text-[11px] text-gray-500 uppercase tracking-wide mb-1">Company Earnings</span>
                  <span className="text-lg font-semibold text-white">
                    ${company.companyEarnings.toLocaleString()}
                  </span>
                </div>

                {/* فاصل عمودي */}
                <div className="w-[1px] h-10 bg-[#333] hidden md:block"></div>

                {/* 3. ربح التطبيق (مكتوب بخط أكبر ولون مختلف للفت الانتباه) */}
                <div className="flex flex-col text-left">
                  <span className="text-[11px] text-[#F4A261]/70 uppercase tracking-wide mb-1">Our Profit</span>
                  <span className="text-2xl font-bold text-[#F4A261]">
                    ${company.appProfit.toLocaleString()}
                  </span>
                </div>

              </div>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}