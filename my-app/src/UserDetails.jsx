import  { useState } from 'react';
import { 
  ArrowLeft, User, MapPin, Phone, Mail, Calendar, 
   FileText, Fingerprint, Image as ImageIcon, 
  Map, Building2, Plane, Wallet, Users, ChevronDown, ChevronUp, Heart
} from 'lucide-react';

// --- مكونات مساعدة للتصميم ---

// 1. مكون سطر المعلومات
const InfoItem = ({ icon: Icon, label, value }) => (
  <div className="flex items-start gap-3 py-3 border-b border-[#ffffff0a] last:border-0 hover:bg-[#ffffff05] rounded-lg px-2 transition-colors duration-200">
    <div className="mt-0.5">
      <Icon className="w-4 h-4 text-[#91B3FA]" />
    </div>
    <div className="flex-1 text-sm">
      <span className="text-gray-400 font-medium">{label} : </span>
      <span className="text-white ml-1 font-semibold">{value}</span>
    </div>
  </div>
);

// 2. مكون المربع الإحصائي
const StatCard = ({ icon: Icon, label, value, colorClass }) => (
  <div className="bg-[#1C1C1E] border border-[#D4AF37]/20 rounded-2xl p-4 flex flex-col items-center justify-center text-center shadow-lg hover:border-[#D4AF37]/50 transition-colors">
    <Icon className={`w-6 h-6 mb-2 ${colorClass}`} />
    <p className="text-[11px] text-gray-400 uppercase tracking-wider mb-1">{label}</p>
    <p className="text-xl font-bold text-white">{value}</p>
  </div>
);

// 3. مكون المرافق القابل للتمدد (Accordion)
const CompanionCard = ({ companion, isExpanded, onToggle }) => {
  return (
    <div className="bg-[#1C1C1E] border border-[#D4AF37]/20 rounded-2xl overflow-hidden transition-all duration-300 shadow-md mb-4">
      {/* رأس المستطيل (قابل للنقر) */}
      <div 
        onClick={onToggle}
        className="flex items-center justify-between p-4 cursor-pointer hover:bg-[#252528] transition-colors"
      >
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full overflow-hidden border border-[#91B3FA]">
            <img src={companion.avatar} alt={companion.name} className="w-full h-full object-cover" />
          </div>
          <div>
            <h4 className="text-sm font-bold text-white">{companion.name}</h4>
            <p className="text-xs text-[#91B3FA] flex items-center gap-1">
              <Heart className="w-3 h-3" /> {companion.relation}
            </p>
          </div>
        </div>
        <div className="text-gray-400">
          {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
        </div>
      </div>

      {/* المحتوى الممتد */}
      {isExpanded && (
        <div className="p-4 border-t border-[#333] bg-[#18181A] animate-in slide-in-from-top-2 duration-200">
          <div className="space-y-1 mb-4">
            <InfoItem icon={User} label="Age" value={`${companion.age} Years`} />
            <InfoItem icon={User} label="Gender" value={companion.gender} />
            <InfoItem icon={MapPin} label="Location" value={companion.location} />
            <InfoItem icon={Phone} label="Phone" value={companion.phone} />
            <InfoItem icon={Fingerprint} label="National ID" value={companion.nationalId} />
            <InfoItem icon={Calendar} label="Date Joined" value={companion.joinDate} />
            <InfoItem icon={Plane} label="Last Trip" value={companion.lastTrip} />
            <InfoItem icon={Map} label="Programs Joined" value={companion.programsJoined} />
            <InfoItem icon={Wallet} label="Amount Spent" value={`$${companion.amountSpent.toLocaleString()}`} />
          </div>

          {/* صور وثائق المرافق */}
          <div className="grid grid-cols-2 gap-3 mt-4">
            <div className="p-2 bg-[#121212] rounded-xl border border-[#333]">
              <span className="text-[10px] text-gray-400 mb-1.5 flex items-center gap-1"><ImageIcon className="w-3 h-3" /> ID Image</span>
              <div className="w-full h-20 rounded-lg overflow-hidden border border-[#444]">
                <img src="https://images.unsplash.com/photo-1544644181-1484b3fdfc62?w=200&q=80" alt="ID" className="w-full h-full object-cover opacity-70" />
              </div>
            </div>
            <div className="p-2 bg-[#121212] rounded-xl border border-[#333]">
              <span className="text-[10px] text-gray-400 mb-1.5 flex items-center gap-1"><ImageIcon className="w-3 h-3" /> Passport Image</span>
              <div className="w-full h-20 rounded-lg overflow-hidden border border-[#444]">
                <img src="https://images.unsplash.com/photo-1544644181-1484b3fdfc62?w=200&q=80" alt="Passport" className="w-full h-full object-cover opacity-70" />
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default function UserDetails({ user, onBack }) {
  // حالة للتحكم في أي مرافق يتم عرضه حالياً (Accordion State)
  const [expandedCompanionId, setExpandedCompanionId] = useState(null);

  // === بيانات وهمية مستوحاة من الصور التي أرسلتها ===
  const mockUser = {
    name: "Abdulrahman Alkharqi",
    email: "abdulrahmanalkhuaqi@gmail.com",
    phone: "+963 944 993 829",
    avatar: "https://i.pravatar.cc/150?img=11",
    age: 20,
    gender: "Male",
    location: "Syria, Rif Dimashq, Jaramana",
    nationalId: "0333300884772917",
    passportNo: "0333300884772917",
    joinDate: "16 / 12 / 2025",
    stats: {
      programs: 10,
      companies: 10,
      trips: 10,
      companions: 2,
      spent: 200000
    },
    companionsList: [
      {
        id: 1,
        name: "Fatima Al-Zahraa",
        relation: "Wife",
        avatar: "https://i.pravatar.cc/150?img=5",
        age: 19,
        gender: "Female",
        location: "Syria, Rif Dimashq, Jaramana",
        phone: "+963 944 993 829",
        nationalId: "0333300884772917",
        joinDate: "16 / 12 / 2025",
        lastTrip: "Trip to Maldives (16/12/2025)",
        programsJoined: 10,
        amountSpent: 20000
      },
      {
        id: 2,
        name: "Omar Alkharqi",
        relation: "Son",
        avatar: "https://i.pravatar.cc/150?img=12",
        age: 5,
        gender: "Male",
        location: "Syria, Rif Dimashq, Jaramana",
        phone: "-",
        nationalId: "0333300884772918",
        joinDate: "10 / 01 / 2026",
        lastTrip: "Trip to Maldives (16/12/2025)",
        programsJoined: 4,
        amountSpent: 5000
      }
    ]
  };

  const currentUser = user || mockUser; // استخدام البيانات الممررة أو الوهمية
  // ما في API بيرجع حجوزات/مرافقين لكل يوزر لحاله، فبنستخدم قيم افتراضية بدل الكراش
  const stats = currentUser.stats || { programs: '—', companies: '—', trips: '—', companions: '—', spent: 0 };
  const companionsList = currentUser.companionsList || [];

  return (
    <div className="p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* 1. الترويسة */}
      <div className="flex items-center gap-4 border-b border-[#333] pb-4">
        <button 
          onClick={onBack}
          className="p-2 bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-lg hover:bg-[#252528] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
        </button>
        <div>
          <h2 className="text-2xl font-bold text-white">User Profile Details</h2>
          <p className="text-sm text-[#91B3FA]">Comprehensive overview & companions</p>
        </div>
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8">
        
        {/* === القسم الأيسر: معلومات المستخدم والإحصائيات (يأخذ 8 أعمدة) === */}
        <div className="xl:col-span-8 space-y-6">
          
          {/* بروفايل المستخدم الأساسي */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg flex items-center gap-6 relative overflow-hidden">
            <div className="absolute top-0 left-0 w-1 h-full bg-gradient-to-b from-[#91B3FA] to-[#F4A261]"></div>
            <div className="w-24 h-24 rounded-full border-2 border-[#D4AF37]/50 overflow-hidden shadow-[0_0_15px_rgba(212,175,55,0.2)] flex-shrink-0">
              <img src={currentUser.avatar} alt="Profile" className="w-full h-full object-cover" />
            </div>
            <div className="flex-1">
              <h1 className="text-2xl font-bold text-white mb-1">{currentUser.name}</h1>
              <div className="flex flex-wrap gap-4 text-sm text-gray-400">
                <span className="flex items-center gap-1.5"><Mail className="w-4 h-4 text-[#91B3FA]" /> {currentUser.email}</span>
                <span className="flex items-center gap-1.5"><Phone className="w-4 h-4 text-[#F4A261]" /> {currentUser.phone}</span>
              </div>
            </div>
          </div>

          {/* الإحصائيات (المربعات) */}
          <div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-5 gap-4">
            <StatCard icon={Calendar} label="Joined" value={currentUser.joinDate || '16 Dec 25'} colorClass="text-[#91B3FA]" />
            <StatCard icon={Map} label="Programs" value={stats.programs} colorClass="text-[#F4A261]" />
            <StatCard icon={Building2} label="Companies" value={stats.companies} colorClass="text-[#91B3FA]" />
            <StatCard icon={Plane} label="Trips" value={stats.trips} colorClass="text-[#D4AF37]" />
            <StatCard icon={Wallet} label="Total Spent" value={`$${(stats.spent / 1000) || 0}K`} colorClass="text-[#4ADE80]" />
          </div>

          {/* التفاصيل الكاملة (مقسمة لبطاقتين) */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="bg-[#18181A]/80 border border-[#D4AF37]/20 rounded-2xl p-5 shadow-lg">
               <h3 className="text-lg font-bold text-white mb-4 flex items-center gap-2 border-b border-[#333] pb-3">
                 <User className="w-5 h-5 text-[#91B3FA]" /> Personal Details
               </h3>
               <div className="space-y-1">
                 <InfoItem icon={User} label="Age" value={`${currentUser.age} Years`} />
                 <InfoItem icon={User} label="Gender" value={currentUser.gender} />
                 <InfoItem icon={MapPin} label="Current Residence" value={currentUser.location} />
                 <InfoItem icon={Fingerprint} label="National ID" value={currentUser.nationalId} />
                 <InfoItem icon={FileText} label="Passport No" value={currentUser.passportNo} />
               </div>
            </div>

            <div className="bg-[#18181A]/80 border border-[#D4AF37]/20 rounded-2xl p-5 shadow-lg">
               <h3 className="text-lg font-bold text-white mb-4 flex items-center gap-2 border-b border-[#333] pb-3">
                 <FileText className="w-5 h-5 text-[#F4A261]" /> Document Images
               </h3>
               <div className="space-y-4 mt-2">
                  <div>
                    <span className="text-xs text-gray-400 mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> ID Image (صورة الهوية)</span>
                    <div className="w-full h-24 rounded-xl overflow-hidden border-2 border-dashed border-[#444] relative group">
                       <img src={currentUser.nationalIdImage || "https://images.unsplash.com/photo-1544644181-1484b3fdfc62?w=400&q=80"} alt="ID" className="w-full h-full object-cover opacity-60 group-hover:opacity-100 transition" />
                    </div>
                  </div>
                  <div>
                    <span className="text-xs text-gray-400 mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> Passport Image (جواز السفر)</span>
                    <div className="w-full h-24 rounded-xl overflow-hidden border-2 border-dashed border-[#444] relative group">
                       <img src={currentUser.passportImage || "https://images.unsplash.com/photo-1544644181-1484b3fdfc62?w=400&q=80"} alt="Passport" className="w-full h-full object-cover opacity-60 group-hover:opacity-100 transition" />
                    </div>
                  </div>
               </div>
            </div>
          </div>

        </div>

        {/* === القسم الأيمن: المرافقون (يأخذ 4 أعمدة) === */}
        <div className="xl:col-span-4 flex flex-col">
          <div className="bg-[#18181A] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg flex-1">
            
            {/* عنوان قسم المرافقين */}
            <div className="flex items-center justify-between mb-6 pb-4 border-b border-[#333]">
              <h3 className="text-lg font-bold text-white flex items-center gap-2">
                <Users className="w-5 h-5 text-[#F4A261]" /> Companions
              </h3>
              <span className="bg-[#D4AF37]/20 text-[#D4AF37] py-1 px-3 rounded-full text-xs font-bold">
                {companionsList.length} Total
              </span>
            </div>

            {/* قائمة المرافقين القابلة للتمدد */}
            <div className="space-y-4">
              {companionsList.map(companion => (
                <CompanionCard
                  key={companion.id}
                  companion={companion}
                  isExpanded={expandedCompanionId === companion.id}
                  onToggle={() => setExpandedCompanionId(
                    expandedCompanionId === companion.id ? null : companion.id
                  )}
                />
              ))}

              {/* رسالة في حال عدم وجود مرافقين */}
              {companionsList.length === 0 && (
                <div className="text-center py-10 text-gray-500 flex flex-col items-center">
                   <Users className="w-10 h-10 mb-2 opacity-20" />
                   <p>No companions found for this user.</p>
                </div>
              )}
            </div>

          </div>
        </div>

      </div>
    </div>
  );
}