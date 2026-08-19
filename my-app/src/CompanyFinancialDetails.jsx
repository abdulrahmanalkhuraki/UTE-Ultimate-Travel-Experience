import {
  ArrowLeft, MapPin, Users, DollarSign, Map,
  Wallet,
  //TrendingUp,
   BarChart3, Building2
} from 'lucide-react';
import { useApiData } from './hooks/useApiData';
import { getTourPackagesFinancial } from './services/dashboardApi';

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
      <div className="flex items-center gap-4 border-b border-[#333] pb-4">
        <button 
          onClick={onBack}
          className="p-2 bg-[#1C1C1E] border border-[#333] rounded-lg hover:bg-[#252528] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
        </button>
        <div className="w-14 h-14 rounded-xl overflow-hidden border border-[#91B3FA]/50 shadow-[0_0_15px_rgba(145,179,250,0.15)]">
          <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
        </div>
        <div>
          <h2 className="text-2xl font-bold text-white">{company.name}</h2>
          <p className="text-sm text-gray-400 flex items-center gap-1 mt-0.5">
            <MapPin className="w-3.5 h-3.5 text-[#EB996E]" /> {company.location}
          </p>
        </div>
      </div>

      {/* === 2. مربعات الإحصائيات العلوية === */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-[#1C1C1E] border border-[#91B3FA]/20 rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[#91B3FA]/10 rounded-xl">
            <Map className="w-6 h-6 text-[#91B3FA]" />
          </div>
          <div>
            <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">Total Programs</p>
            <p className="text-2xl font-bold text-white">{totalPrograms} <span className="text-sm text-gray-500 font-normal">Published</span></p>
          </div>
        </div>

        <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[#2A2A2D] rounded-xl">
            <Building2 className="w-6 h-6 text-gray-300" />
          </div>
          <div>
            <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">Company Earnings</p>
            <p className="text-2xl font-bold text-white">${company.companyEarnings.toLocaleString()}</p>
          </div>
        </div>

        {/* مربع ربح التطبيق (مميز بلونك المفضل EB996E) */}
        <div className="bg-[#1C1C1E] border border-[#EB996E]/30 rounded-2xl p-6 shadow-[0_0_20px_rgba(235,153,110,0.1)] flex items-center gap-4 relative overflow-hidden">
          <div className="absolute top-0 right-0 w-16 h-16 bg-[#EB996E]/10 rounded-bl-full"></div>
          <div className="p-4 bg-[#EB996E]/10 rounded-xl">
            <BarChart3 className="w-6 h-6 text-[#EB996E]" />
          </div>
          <div>
            <p className="text-xs text-[#EB996E]/80 uppercase tracking-wider mb-1 font-semibold">Our Profit</p>
            <p className="text-2xl font-bold text-[#EB996E]">${company.appProfit.toLocaleString()}</p>
          </div>
        </div>
      </div>

      {/* === 3. قائمة البرامج === */}
      <div className="space-y-4">
        <h3 className="text-lg font-bold text-white mb-6 border-b border-[#333] pb-2 flex items-center gap-2">
          <Wallet className="w-5 h-5 text-[#91B3FA]" /> Programs Financial Breakdown
        </h3>

        <div className="flex flex-col gap-5">
          {companyPrograms.map(program => (
            <div 
              key={program.id} 
              className="bg-[#18181A] border border-[#333] rounded-2xl p-4 flex flex-col xl:flex-row items-center gap-6 shadow-md hover:border-[#91B3FA]/30 transition-colors"
            >
              {/* القسم الأيسر: الصورة ومعلومات البرنامج */}
              <div className="flex items-center gap-4 flex-1 w-full xl:w-auto border-b xl:border-b-0 border-[#333] pb-4 xl:pb-0">
                <div className="w-20 h-20 rounded-xl overflow-hidden shrink-0 border border-[#444]">
                  <img src={program.image || "https://images.unsplash.com/photo-1451337516015-6b6e9a44a8a3?w=200&q=80"} alt={program.name} className="w-full h-full object-cover" />
                </div>
                <div>
                  <h4 className="text-base font-bold text-white mb-1.5">{program.name}</h4>
                  <p className="text-xs text-gray-400 flex items-center gap-1.5 mb-1.5">
                    <MapPin className="w-3.5 h-3.5 text-[#91B3FA]" /> {program.locations}
                  </p>
                </div>
              </div>

              {/* القسم الأيمن: البطاقة الداخلية للتفاصيل المالية */}
              <div className="w-full xl:w-auto bg-[#121212] rounded-xl border border-[#222] p-3 flex flex-wrap md:flex-nowrap items-center justify-between gap-6 md:gap-8 px-6">
                
                {/* عدد الحجوزات المكتملة */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1 mb-1"><Users className="w-3 h-3" /> Bookings</span>
                  <span className="text-sm font-semibold text-gray-200">{program.touristsCount} <span className="text-xs font-normal text-gray-500">Completed</span></span>
                </div>

                {/* سعر الشخص */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1 mb-1"><DollarSign className="w-3 h-3" /> Price / Person</span>
                  <span className="text-sm font-semibold text-gray-200">${program.pricePerPerson}</span>
                </div>

                {/* فاصل للموبايل */}
                <div className="hidden md:block w-[1px] h-8 bg-[#333]"></div>

                {/* ربح الشركة */}
                <div className="flex flex-col text-left">
                  <span className="text-[10px] text-gray-500 uppercase mb-1">Company Profit</span>
                  <span className="text-base font-bold text-gray-300">${program.companyProfit.toLocaleString()}</span>
                </div>

                {/* فاصل للموبايل */}
                <div className="hidden md:block w-[1px] h-8 bg-[#333]"></div>

                {/* ربح التطبيق (بارز ولون مميز #EB996E) */}
                <div className="flex flex-col text-left bg-[#1C1C1E] p-2 px-4 rounded-lg border border-[#EB996E]/20">
                  <span className="text-[10px] text-[#EB996E]/70 uppercase mb-0.5 font-bold tracking-wider">Our Profit</span>
                  <span className="text-xl font-bold text-[#EB996E]">${program.appProfit.toLocaleString()}</span>
                </div>

              </div>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}