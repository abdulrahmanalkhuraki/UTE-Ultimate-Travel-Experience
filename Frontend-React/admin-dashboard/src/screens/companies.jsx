import CompanyDetails from './companydetailes2';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';
import PendingCompanyDetails from './PendingCompanyDetails';
import  { useState } from 'react';
import { 
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer 
} from 'recharts';
import { 
  Building2, Calendar, MapPin, Map, Trash2, Hourglass, ChevronDown 
} from 'lucide-react';

// --- بيانات المخطط البياني (وهمية) ---
const chartData = [
  { name: 'Jan', International: 12, Local: 8 },
  { name: 'Feb', International: 15, Local: 10 },
  { name: 'Mar', International: 18, Local: 14 },
  { name: 'Apr', International: 22, Local: 16 },
  { name: 'May', International: 25, Local: 18 },
  { name: 'Jun', International: 30, Local: 22 },
  { name: 'Jul', International: 35, Local: 25 },
  { name: 'Aug', International: 32, Local: 28 },
  { name: 'Sep', International: 38, Local: 30 },
  { name: 'Oct', International: 42, Local: 35 },
  { name: 'Nov', International: 45, Local: 38 },
  { name: 'Dec', International: 50, Local: 40 },
];

// --- بيانات الشركات (وهمية) ---
const currentCompanies = [
  { id: 1, name: 'Global Trails Inc.', founded: '1998', location: 'Burj Al Arab, Jumeirah, Dubai, UAE', programs: 24, logo: 'https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80' },
  { id: 2, name: 'Urban Odyssey', founded: '2010', location: 'Marina Heights, Dubai Marina, UAE', programs: 15, logo: 'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=150&q=80' },
  { id: 3, name: 'Eco Escapes LLC', founded: '2015', location: 'Downtown Hotel, Dubai, UAE', programs: 8, logo: 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=150&q=80' },
];

const deletedCompanies = [
  { id: 4, name: 'Desert Navigators', founded: '2005', location: 'Deira City Center, Dubai, UAE', programs: 12, deletedAt: '5 days ago', logo: 'https://images.unsplash.com/photo-1435575653489-b0873ec954e2?w=150&q=80' },
  { id: 5, name: 'Summit Treks Ltd.', founded: '2012', location: 'Al Barsha, Dubai, UAE', programs: 5, deletedAt: '2 weeks ago', logo: 'https://images.unsplash.com/photo-1554469384-e58fac16e23a?w=150&q=80' },
];

const pendingCompaniesData = [
  { id: 6, name: 'Alpine Adventures', founded: '2021', location: 'Palm Jumeirah, Dubai, UAE', programs: 3, logo: 'https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=150&q=80' },
  { id: 7, name: 'Oceanic Ventures', founded: '2019', location: 'JBR, Dubai, UAE', programs: 7, logo: 'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=150&q=80' },
  { id: 8, name: 'Horizon Tours', founded: '2023', location: 'Business Bay, Dubai, UAE', programs: 1, logo: 'https://images.unsplash.com/photo-1497215842964-222b430dc094?w=150&q=80' },
];

export default function Companies() {
  const [isDeletedExpanded, setIsDeletedExpanded] = useState(false);
  const [isCurrentExpanded, setIsCurrentExpanded] = useState(true); // مفتوح كقيمة افتراضية
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [selectedPendingCompany, setSelectedPendingCompany] = useState(null);
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [itemToReject, setItemToReject] = useState(null);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [itemToApprove, setItemToApprove] = useState(null);
  const [pendingCompanies, setPendingCompanies] = useState(pendingCompaniesData);

  // دالة التعامل مع ضغطة زر الرفض
  const handleRejectClick = (e, company) => {
    e.stopPropagation(); // لمنع فتح تفاصيل الشركة عند الضغط على الزر
    setItemToReject(company);
    setIsRejectDialogOpen(true);
  };

  // دالة الإرسال الفعلية (بعد كتابة السبب)
  const handleConfirmRejection = (reason) => {
    console.log(`Rejecting ${itemToReject.name} for reason: ${reason}`);
    setPendingCompanies((prev) => prev.filter((company) => company.id !== itemToReject.id));
    if (selectedPendingCompany?.id === itemToReject.id) {
      setSelectedPendingCompany(null);
    }
    setIsRejectDialogOpen(false);
    setItemToReject(null);
  };

  const handleApproveClick = (e, company) => {
    e.stopPropagation();
    setItemToApprove(company);
    setIsApproveDialogOpen(true);
  };

  const handleConfirmApproval = () => {
    console.log(`Approving ${itemToApprove?.name}`);
    setPendingCompanies((prev) => prev.filter((company) => company.id !== itemToApprove.id));
    if (selectedPendingCompany?.id === itemToApprove.id) {
      setSelectedPendingCompany(null);
    }
    setIsApproveDialogOpen(false);
    setItemToApprove(null);
  };

  // تصميم موحد لكارت الشركة للـ Accordion (يمين اللوجو، يسار التاريخ)
  const CompanyCard = ({ company, isDeleted }) => (
    <div  onClick={() => !isDeleted && setSelectedCompany(company)}
    className="flex justify-between items-center bg-[#18181A] border border-[#333] p-5 rounded-2xl transition hover:border-[#D4AF37]/50 shadow-md">
      {/* يسار: تاريخ الحذف (إن وجد) */}
      <div className="text-xs text-gray-500 font-medium whitespace-nowrap w-24">
        {isDeleted ? `Deleted: ${company.deletedAt}` : ''}
      </div>

      {/* يمين: المعلومات واللوجو */}
      <div className="flex items-center gap-5 text-right flex-1 justify-end">
        <div className="space-y-2">
          <h4 className="text-base font-bold text-white">{company.name}</h4>
          
          <div className="flex items-center justify-end gap-4 text-xs text-gray-400">
            <span className="flex items-center gap-1.5"><Calendar className="w-3.5 h-3.5" /> Founded: {company.founded}</span>
            <span>•</span>
            <span className="flex items-center gap-1.5 text-[#91B3FA]"><MapPin className="w-3.5 h-3.5" /> {company.location}</span>
          </div>
          
          <div className="flex items-center justify-end gap-1.5 text-xs text-gray-400">
            <Map className="w-3.5 h-3.5" /> Published Programs: {company.programs}
          </div>
        </div>
        
        <div className="w-16 h-16 rounded-xl overflow-hidden border-2 border-[#2d303e] flex-shrink-0">
          <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
        </div>
      </div>
    </div>
  );

  if (selectedCompany) {
    return <CompanyDetails company={selectedCompany} onBack={() => setSelectedCompany(null)} />;
  }

  if (selectedPendingCompany) {
    return (
      <PendingCompanyDetails 
        company={selectedPendingCompany} 
        onBack={() => setSelectedPendingCompany(null)} 
        onApprove={() => {
          alert("تم القبول بنجاح!"); // لاحقاً تربطها بالـ API
          setSelectedPendingCompany(null);
        }}
        onReject={() => {
          alert("تم رفض الطلب.");
          setSelectedPendingCompany(null);
        }}
      />
    );
  }

  return (
    <div className="p-8 space-y-8 font-sans">
      
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        
        {/* === القسم الأيسر (المخطط والقوائم المتمددة) (يأخذ 7 من 12 من العرض) === */}
        <div className="lg:col-span-7 space-y-6">
          
          {/* المخطط والإحصائيات */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 p-6 rounded-2xl shadow-lg">
            <h3 className="text-lg font-semibold text-white mb-4 text-left">Company Growth Over Time</h3>
            <div className="w-full h-64 mb-6">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}}  />
                  <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
                  <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#F4A261', border: '1px solid #F4A261', borderRadius: '0.8rem'}} />
                  <Legend iconType="plainline" />
                  {/* <Line type="monotone" dataKey="Local" stroke="#91B3FA" strokeWidth={3} dot={false} /> */}
                  <Line type="monotone" dataKey="International" stroke="#F4A261" strokeWidth={3} dot={false} />
                </LineChart>
              </ResponsiveContainer>
            </div>

            {/* الإحصائيات أسفل المخطط */}
            <div className="grid grid-cols-3 gap-4 pt-6 border-t border-[#333]">
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <Building2 className="w-8 h-8 text-[#91B3FA]" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Active Companies</p>
                  <p className="text-xl font-bold text-white mt-0.5">1,250</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <Trash2 className="w-8 h-8 text-red-400" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Deleted Companies</p>
                  <p className="text-xl font-bold text-red-400 mt-0.5">150</p>
                </div>
              </div>
              <div className="flex items-center gap-4 bg-[#121212] p-4 rounded-xl border border-[#2a2a2a]">
                <Hourglass className="w-8 h-8 text-[#F4A261]" />
                <div>
                  <p className="text-[10px] text-gray-500 font-bold uppercase tracking-wider">Pending Companies</p>
                  <p className="text-xl font-bold text-[#F4A261] mt-0.5">{pendingCompanies.length}</p>
                </div>
              </div>
            </div>
          </div>

          {/* قائمة الشركات المحذوفة (المتمددة) */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
            <button 
              onClick={() => setIsDeletedExpanded(!isDeletedExpanded)}
              className="w-full p-5  flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition "
            >
              <div className="flex items-center gap-4">
              <Trash2 className="w-8 h-8 text-red-400 ml-5" />
              <h3 className="text-base text-lg font-semibold text-red-400 ">Users who deleted their accounts</h3>
              </div>
              <ChevronDown className={`w-5 h-5 text-gray-400 mr-10 transition-transform  duration-300 ${isDeletedExpanded ? 'rotate-180' : ''}`} />
              
            </button>
            {isDeletedExpanded && (
              <div className="p-5 space-y-4 max-h-[350px] overflow-y-auto custom-scrollbar border-t border-[#333]">
                {deletedCompanies.map(company => (
                  <CompanyCard key={company.id} company={company} isDeleted={true} />
                ))}
              </div>
            )}
          </div>

          {/* قائمة الشركات الحالية (المتمددة) */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
            <button 
              onClick={() => setIsCurrentExpanded(!isCurrentExpanded)}
              className="w-full p-5 flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition"
            >
              <div className="flex items-center gap-4">
              <Building2 className="w-8 h-8 text-[#F4A261] ml-5" />
              <h3 className="text-base text-lg font-semibold text-[#F4A261] ml-10">Current Companies</h3>
              </div>
              <ChevronDown className={`w-5 h-5 text-gray-400 mr-10 transition-transform duration-300 ${isCurrentExpanded ? 'rotate-180' : ''}`} />
              
            </button>
            {isCurrentExpanded && (
              <div className="p-5 space-y-4 max-h-[400px] overflow-y-auto custom-scrollbar border-t border-[#333]">
                {currentCompanies.map(company => (
                  <CompanyCard key={company.id} company={company} isDeleted={false} />
                ))}
              </div>
            )}
          </div>

        </div>

        {/* === القسم الأيمن (طلبات الشركات المعلقة) (يأخذ 5 من 12 من العرض) === */}
        <div className="lg:col-span-5">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl  shadow-lg p-6 h-full flex flex-col">
            <h3 className="text-lg font-semibold justify-content text-white mb-6 ml-5 text-left border-b border-[#333] pb-4">
              Pending Company Applications
            </h3>
            
            <div className="space-y-5 overflow-y-auto flex-1 pr-2 custom-scrollbar">
              {pendingCompanies.length === 0 ? (
                <p className="text-gray-400 text-center text-xl mt-10">No pending companies.</p>
              ) : (
              pendingCompanies.map(company => (
                <div key={company.id} 
                onClick={() => setSelectedPendingCompany(company)}
                className="bg-[#18181A] border border-[#333] p-5 rounded-2xl cursor-pointer hover:border-[#91B3FA]/50 transition-colors shadow-md"
                //className="bg-[#18181A] border border-[#333] p-5 rounded-2xl"
                >
                  {/* معلومات الشركة */}
                  <div className="flex items-center gap-4 text-right justify-end mb-5">
                    <div className="space-y-1">
                      <h4 className="text-base font-bold text-white">{company.name}</h4>
                      <p className="text-xs text-gray-400 flex items-center justify-end gap-2">
                        <Calendar className="w-3.5 h-3.5" /> Founded: {company.founded}
                      </p>
                      <p className="text-xs text-[#91B3FA] flex items-center justify-end gap-2">
                        <MapPin className="w-3.5 h-3.5" /> {company.location}
                      </p>
                      <p className="text-xs text-gray-400 flex items-center justify-end gap-2">
                        <Map className="w-3.5 h-3.5" /> Programs Prepared: {company.programs}
                      </p>
                    </div>
                    <div className="w-16 h-16 rounded-xl overflow-hidden border-2 border-[#2d303e]">
                      <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
                    </div>
                  </div>

                  {/* أزرار القبول والرفض */}
                  <div className="flex gap-3 mt-2">
                    <button 
                    onClick={(e) => handleRejectClick(e, company)} // <--- الربط هنا
                    className="flex-1 bg-[#2A2A2D] text-gray-300 py-2.5 rounded-xl font-medium hover:bg-[#333] transition duration-200">
                      Reject
                    </button>
                    <button
                    onClick={(e) => handleApproveClick(e, company)}
                    className="flex-1 bg-[#91B3FA] text-black py-2.5 rounded-xl font-medium hover:bg-[#7fa1e8] shadow-[0_0_15px_rgba(145,179,250,0.15)] transition duration-200">
                      Approve
                    </button>
                  </div>
                </div>
              )))}
            </div>
          </div>
        </div>

      </div>
      <RejectDialog 
    isOpen={isRejectDialogOpen}
    onClose={() => setIsRejectDialogOpen(false)}
    onSubmit={handleConfirmRejection}
    targetName={itemToReject?.name}
  />
  <ApproveDialog
    isOpen={isApproveDialogOpen}
    onClose={() => {
      setIsApproveDialogOpen(false);
      setItemToApprove(null);
    }}
    onConfirm={handleConfirmApproval}
    targetName={itemToApprove?.name}
  />
    </div>
   
  );
}