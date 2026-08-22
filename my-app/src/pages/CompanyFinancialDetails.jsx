import {
  ArrowLeft, MapPin, Users, DollarSign, Map,
  Wallet,
  //TrendingUp,
   BarChart3, Building2
} from 'lucide-react';
import { useApiData } from '../hooks/useApiData';
import { getTourPackagesFinancial } from '../services/dashboardApi';

export default function CompanyFinancialDetails({ company, onBack }) {
  const companyId = company?.id ?? company?.companyId;
  // pageSize كبيرة لأن التصميم الحالي بيعرض القائمة كاملة بدون Pagination UI
  const { data: financialData } = useApiData(() => getTourPackagesFinancial(companyId, 1, 100), [companyId]);

  // GET /api/Admin/dashboard/companies/:companyId/tour-packages/financial -> items
  const companyPrograms = (financialData?.items ?? []).map((item) => ({
    id: item.tourPackageId,
    name: item.packageName,
    image: item.packageImage,
    locations: (item.packageCities ?? []).map((c) => c.cityName).join(', '),
    touristsCount: item.completedBookingsCount,
    pricePerPerson: item.averagePrice,
    companyProfit: item.companyEarnings,
    appProfit: item.ourProfit,
  }));

  const totalPrograms = financialData ? financialData.pagination.totalItems : companyPrograms.length;

  return (
    <div className="p-8 space-y-8 font-sans animate-in fade-in duration-300">
      
      {/* === 1. الترويسة === */}
      <div className="flex items-center gap-4 border-b border-[var(--color-border)] pb-4">
        <button 
          onClick={onBack}
          className="p-2 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-lg hover:bg-[var(--color-surface-alt)] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-[var(--color-text-muted)] group-hover:text-[var(--color-text)]" />
        </button>
        <div className="w-14 h-14 rounded-xl overflow-hidden border border-[var(--color-accent)]/50 shadow-[0_0_15px_rgba(145,179,250,0.15)]">
          <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
        </div>
        <div>
          <h2 className="text-2xl font-bold text-[var(--color-text)]">{company.name}</h2>
          <p className="text-sm text-[var(--color-text-muted)] flex items-center gap-1 mt-0.5">
            <MapPin className="w-3.5 h-3.5 text-[var(--color-accent-2)]" /> {company.location}
          </p>
        </div>
      </div>

      {/* === 2. مربعات الإحصائيات العلوية === */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-[var(--color-surface)] border border-[var(--color-accent)]/20 rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[var(--color-accent)]/10 rounded-xl">
            <Map className="w-6 h-6 text-[var(--color-accent)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Total Programs</p>
            <p className="text-2xl font-bold text-[var(--color-text)]">{totalPrograms} <span className="text-sm text-[var(--color-text-muted)] font-normal">Published</span></p>
          </div>
        </div>

        <div className="bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[var(--color-surface-alt)] rounded-xl">
            <Building2 className="w-6 h-6 text-[var(--color-text-muted)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Company Earnings</p>
            <p className="text-2xl font-bold text-[var(--color-text)]">${company.companyEarnings.toLocaleString()}</p>
          </div>
        </div>

        {/* مربع ربح التطبيق (مميز بلونك المفضل EB996E) */}
        <div className="bg-[var(--color-surface)] border border-[var(--color-accent-2)]/30 rounded-2xl p-6 shadow-[0_0_20px_rgba(235,153,110,0.1)] flex items-center gap-4 relative overflow-hidden">
          <div className="absolute top-0 right-0 w-16 h-16 bg-[var(--color-accent-2)]/10 rounded-bl-full"></div>
          <div className="p-4 bg-[var(--color-accent-2)]/10 rounded-xl">
            <BarChart3 className="w-6 h-6 text-[var(--color-accent-2)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-accent-2)]/80 uppercase tracking-wider mb-1 font-semibold">Our Profit</p>
            <p className="text-2xl font-bold text-[var(--color-accent-2)]">${company.appProfit.toLocaleString()}</p>
          </div>
        </div>
      </div>

      {/* === 3. قائمة البرامج === */}
      <div className="space-y-4">
        <h3 className="text-lg font-bold text-[var(--color-text)] mb-6 border-b border-[var(--color-border)] pb-2 flex items-center gap-2">
          <Wallet className="w-5 h-5 text-[var(--color-accent)]" /> Programs Financial Breakdown
        </h3>

        <div className="flex flex-col gap-5">
          {companyPrograms.map(program => (
            <div 
              key={program.id} 
              className="bg-[var(--color-surface-alt)] border border-[var(--color-border)] rounded-2xl p-4 flex flex-col xl:flex-row items-center gap-6 shadow-md hover:border-[var(--color-accent)]/30 transition-colors"
            >
              {/* القسم الأيسر: الصورة ومعلومات البرنامج */}
              <div className="flex items-center gap-4 flex-1 w-full xl:w-auto border-b xl:border-b-0 border-[var(--color-border)] pb-4 xl:pb-0">
                <div className="w-20 h-20 rounded-xl overflow-hidden shrink-0 border border-[var(--color-border)]">
                  <img src={program.image || "https://images.unsplash.com/photo-1451337516015-6b6e9a44a8a3?w=200&q=80"} alt={program.name} className="w-full h-full object-cover" />
                </div>
                <div>
                  <h4 className="text-base font-bold text-[var(--color-text)] mb-1.5">{program.name}</h4>
                  <p className="text-xs text-[var(--color-text-muted)] flex items-center gap-1.5 mb-1.5">
                    <MapPin className="w-3.5 h-3.5 text-[var(--color-accent)]" /> {program.locations}
                  </p>
                </div>
              </div>

              {/* القسم الأيمن: البطاقة الداخلية للتفاصيل المالية */}
              <div className="w-full xl:w-auto bg-[var(--color-app-bg)] rounded-xl border border-[var(--color-border)] p-3 flex flex-wrap md:flex-nowrap items-center justify-between gap-6 md:gap-8 px-6">
                
                {/* عدد الحجوزات المكتملة */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-[var(--color-text-muted)] uppercase flex items-center gap-1 mb-1"><Users className="w-3 h-3" /> Bookings</span>
                  <span className="text-sm font-semibold text-[var(--color-text-muted)]">{program.touristsCount} <span className="text-xs font-normal text-[var(--color-text-muted)]">Completed</span></span>
                </div>

                {/* سعر الشخص */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-[var(--color-text-muted)] uppercase flex items-center gap-1 mb-1"><DollarSign className="w-3 h-3" /> Price / Person</span>
                  <span className="text-sm font-semibold text-[var(--color-text-muted)]">${program.pricePerPerson}</span>
                </div>

                {/* فاصل للموبايل */}
                <div className="hidden md:block w-[1px] h-8 bg-[var(--color-border)]"></div>

                {/* ربح الشركة */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-[var(--color-text-muted)] uppercase mb-1">Company Profit</span>
                  <span className="text-base font-bold text-[var(--color-text-muted)]">${program.companyProfit.toLocaleString()}</span>
                </div>

                {/* فاصل للموبايل */}
                <div className="hidden md:block w-[1px] h-8 bg-[var(--color-border)]"></div>

                {/* ربح التطبيق (بارز ولون مميز var(--color-accent-2)) */}
                <div className="flex flex-col text-left bg-[var(--color-surface)] p-2 px-4 rounded-lg border border-[var(--color-accent-2)]/20">
                  <span className="text-[10px] text-[var(--color-accent-2)]/70 uppercase mb-0.5 font-bold tracking-wider">Our Profit</span>
                  <span className="text-xl font-bold text-[var(--color-accent-2)]">${program.appProfit.toLocaleString()}</span>
                </div>

              </div>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}