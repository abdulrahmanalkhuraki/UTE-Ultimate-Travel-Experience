import { useState } from 'react';
import CompanyFinancialDetails from './CompanyFinancialDetails'; 

import {
  Wallet, Building2, MapPin, TrendingUp, BarChart3
} from 'lucide-react';
import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer
} from 'recharts';
import { useApiData } from '../hooks/useApiData';
import { getFinancialDashboard, getCompaniesFinancial } from '../services/dashboardApi';

export default function Financials() {

    const [selectedCompany, setSelectedCompany] = useState(null);
    const { data: financialData } = useApiData(getFinancialDashboard, []);
    // pageSize كبيرة لأن التصميم الحالي بيعرض القائمة كاملة بدون Pagination UI
    const { data: companiesFinancialData } = useApiData(() => getCompaniesFinancial(1, 100), []);

    // GET /api/Admin/dashboard/financial -> profitGrowth
    const profitData = (financialData?.profitGrowth ?? []).map((g) => ({
      month: g.month,
      profit: g.profit,
    }));

    // GET /api/Admin/dashboard/companies/financial -> items
    const companiesFinancials = (companiesFinancialData?.items ?? []).map((item) => ({
      id: item.companyId,
      name: item.companyName,
      logo: item.companyLogo,
      location: item.companyLocation,
      companyEarnings: item.companyEarnings,
      appProfit: item.ourProfit,
    }));

  // === العمليات الحسابية والترتيب ===
  // ترتيب الشركات من الأكبر ربحاً لنا إلى الأصغر
  const sortedCompanies = [...companiesFinancials].sort((a, b) => b.appProfit - a.appProfit);

  // إجمالي ربح التطبيق ياخد مباشرة من /financial (مصدر أدق من جمع صفحة الشركات)
  const totalAppProfit = financialData ? financialData.totalProfit : 0;
  const totalCompaniesEarnings = sortedCompanies.reduce((sum, company) => sum + company.companyEarnings, 0);

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
      <div className="flex items-center justify-between border-b border-[var(--color-border)] pb-4">
        <div>
          <h1 className="text-3xl font-bold text-[var(--color-text)] flex items-center gap-3">
            <Wallet className="w-8 h-8 text-[var(--color-accent-2)]" /> 
            Financial Overview
          </h1>
          <p className="text-sm text-[var(--color-text-muted)] mt-1">Track app revenue, total spending, and companies' earnings</p>
        </div>
      </div>

      {/* === القسم العلوي: المخطط البياني والإحصائيات في نفس المربع === */}
      <div className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-2xl shadow-lg overflow-hidden flex flex-col">
        
        {/* عنوان المخطط */}
        <div className="p-6 pb-2">
          <h3 className="text-lg font-bold text-[var(--color-text)] flex items-center gap-2">
            <TrendingUp className="w-5 h-5 text-[var(--color-accent)]" /> Profit Growth Over Time
          </h3>
        </div>

        {/* مساحة المخطط البياني */}
        <div className="w-full h-[350px] p-6 pt-0">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={profitData} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
              <defs>
                <linearGradient id="colorProfit" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="var(--color-accent-2)" stopOpacity={0.4}/>
                  <stop offset="95%" stopColor="var(--color-accent-2)" stopOpacity={0}/>
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
              <XAxis dataKey="month" stroke="var(--color-text-muted)" tick={{fill: 'var(--color-text-muted)', fontSize: 12}} />
              <YAxis stroke="var(--color-text-muted)" tick={{fill: 'var(--color-text-muted)', fontSize: 12}} tickFormatter={(value) => `$${value/1000}k`} />
              <Tooltip 
                contentStyle={{backgroundColor: 'var(--color-surface-alt)', borderColor: 'var(--color-accent-2)', borderRadius: '8px'}} 
                itemStyle={{color: 'var(--color-accent-2)', fontWeight: 'bold'}}
              />
              <Area type="monotone" dataKey="profit" stroke="var(--color-accent-2)" strokeWidth={3} fillOpacity={1} fill="url(#colorProfit)" />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        {/* الإحصائيات (مربعين فقط بعد التعديل) */}
        <div className="grid grid-cols-1 md:grid-cols-2 divide-y md:divide-y-0 md:divide-x divide-[var(--color-border)] border-t border-[var(--color-border)] bg-[var(--color-surface-alt)]/50">
          
          {/* 1. الربح الكلي للتطبيق */}
          <div className="p-6 flex flex-col justify-center hover:bg-[var(--color-surface-alt)] transition-colors">
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-2 flex items-center gap-2">
              <BarChart3 className="w-4 h-4 text-[var(--color-success)]" /> Total App Profit
            </p>
            <h4 className="text-3xl font-bold text-[var(--color-success)]">
              ${totalAppProfit.toLocaleString()}
            </h4>
          </div>

          {/* 2. ما حصلت عليه الشركات */}
          <div className="p-6 flex flex-col justify-center hover:bg-[var(--color-surface-alt)] transition-colors">
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-2 flex items-center gap-2">
              <Building2 className="w-4 h-4 text-[var(--color-accent)]" /> Total Companies Earnings
            </p>
            <h4 className="text-3xl font-bold text-[var(--color-text)]">
              ${totalCompaniesEarnings.toLocaleString()}
            </h4>
          </div>

        </div>
      </div>

      {/* === القسم السفلي: قائمة الشركات مرتبة حسب الربح === */}
      <div className="space-y-4">
        <h3 className="text-xl font-bold text-[var(--color-text)] mb-6 border-b border-[var(--color-border)] pb-2">
          Companies Financial Breakdown <span className="text-sm font-normal text-[var(--color-text-muted)] ml-2">(Sorted by Highest Profit)</span>
        </h3>

        <div className="flex flex-col gap-4">
          {sortedCompanies.map((company) => (
            <div 
              key={company.id} 
              onClick={() => setSelectedCompany(company)}
              className="bg-[var(--color-surface-alt)] border border-[var(--color-border)] rounded-2xl p-5 flex flex-col md:flex-row md:items-center justify-between gap-6 shadow-lg hover:border-[var(--color-accent)]/40 transition-all duration-300 relative overflow-hidden cursor-pointer"
            >
              {/* شريط رفيع على اليسار يميز أعلى شركة */}
              { <div className="absolute left-0 top-0 w-1 h-full bg-[var(--color-accent)]"></div>}

              {/* 1. اللوغو، الاسم والموقع (الجهة اليسرى) */}
              <div className="flex items-center gap-4 flex-1">
                <div className="w-14 h-14 rounded-xl overflow-hidden border border-[var(--color-border)]/30 shrink-0">
                  <img src={company.logo || "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80"} alt={company.name} className="w-full h-full object-cover" />
                </div>
                <div>
                  <h4 className="text-lg font-bold text-[var(--color-text)]">{company.name}</h4>
                  <p className="text-sm text-[var(--color-text-muted)] flex items-center gap-1 mt-1">
                    <MapPin className="w-3.5 h-3.5" /> {company.location}
                  </p>
                </div>
              </div>

              {/* الأرقام المادية (الجهة اليمنى) */}
              <div className="flex items-center gap-8 md:gap-12 flex-wrap md:flex-nowrap bg-[var(--color-app-bg)] py-3 px-6 rounded-xl border border-[var(--color-border)]">
                
                {/* 2. ربح الشركة */}
                <div className="flex flex-col text-left">
                  <span className="text-[11px] text-[var(--color-text-muted)] uppercase tracking-wide mb-1">Company Earnings</span>
                  <span className="text-lg font-semibold text-[var(--color-text)]">
                    ${company.companyEarnings.toLocaleString()}
                  </span>
                </div>

                {/* فاصل عمودي */}
                <div className="w-[1px] h-10 bg-[var(--color-border)] hidden md:block"></div>

                {/* 3. ربح التطبيق (مكتوب بخط أكبر ولون مختلف للفت الانتباه) */}
                <div className="flex flex-col text-left">
                  <span className="text-[11px] text-[var(--color-accent-2)]/70 uppercase tracking-wide mb-1">Our Profit</span>
                  <span className="text-2xl font-bold text-[var(--color-accent-2)]">
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