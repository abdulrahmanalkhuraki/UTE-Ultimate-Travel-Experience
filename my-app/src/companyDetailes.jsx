
import { 
  ArrowLeft, Building2, User, Calendar, MapPin, Phone, Mail, 
  FileText, CreditCard, Star, MessageSquare, Wallet, Users, Map, Image as ImageIcon, Fingerprint
} from 'lucide-react';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer 
} from 'recharts';

// --- بيانات وهمية للمخطط البياني للشركة ---
const companyPerformanceData = [
  { name: 'Jan', Tourists: 120, Programs: 5 },
  { name: 'Feb', Tourists: 150, Programs: 7 },
  { name: 'Mar', Tourists: 180, Programs: 8 },
  { name: 'Apr', Tourists: 220, Programs: 12 },
  { name: 'May', Tourists: 250, Programs: 15 },
  { name: 'Jun', Tourists: 310, Programs: 18 },
  { name: 'Jul', Tourists: 350, Programs: 20 },
  { name: 'Aug', Tourists: 320, Programs: 19 },
  { name: 'Sep', Tourists: 380, Programs: 22 },
  { name: 'Oct', Tourists: 420, Programs: 25 },
  { name: 'Nov', Tourists: 450, Programs: 28 },
  { name: 'Dec', Tourists: 500, Programs: 30 },
];

export default function CompanyDetails({ company, onBack }) {
  // عنصر مساعد لعرض سطور المعلومات بشكل أنيق
  const InfoRow = ({ icon: Icon, label, value }) => (
    <div className="flex flex-col mb-4">
      <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-1 flex items-center gap-1.5">
        <Icon className="w-3.5 h-3.5" /> {label}
      </span>
      <div className="bg-[#121212] border border-[#2a2a2a] rounded-lg p-3 text-sm text-gray-200">
        {value}
      </div>
    </div>
  );

  return (
    <div className="p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* 1. الترويسة وزر العودة */}
      <div className="flex items-center gap-4 border-b border-[#333] pb-4">
        <button 
          onClick={onBack}
          className="p-2 bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-lg hover:bg-[#252528] transition group"
        >
          <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
        </button>
        <div className="w-12 h-12 rounded-xl overflow-hidden border-2 border-[#D4AF37]/50">
          <img src={company?.logo || "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80"} alt="Logo" className="w-full h-full object-cover" />
        </div>
        <div>
          <h2 className="text-2xl font-bold text-white">{company?.name || "Global Trails Inc."}</h2>
          <p className="text-sm text-[#91B3FA]">Detailed Performance & Information</p>
        </div>
      </div>

      {/* 2. القسم العلوي (المخطط البياني + المربعات الأربعة) */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-6">
        
        {/* المخطط البياني والمستطيلات المدمجة (يأخذ 8 أعمدة) */}
        <div className="xl:col-span-8 flex flex-col">
          {/* المخطط */}
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-t-2xl p-6 flex-1 shadow-lg relative">
            <h3 className="text-base font-semibold text-white mb-6">Tourists & Programs Growth</h3>
            <div className="w-full h-64">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={companyPerformanceData} margin={{ top: 0, right: 0, left: -20, bottom: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
                  <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
                  <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#D4AF37/50', borderRadius: '8px'}} cursor={{fill: 'rgba(255,255,255,0.05)'}} />
                  <Legend iconType="circle" />
                  <Bar dataKey="Tourists" fill="#91B3FA" name="Total Tourists" radius={[4, 4, 0, 0]} barSize={20} />
                  <Bar dataKey="Programs" fill="#F4A261" name="Published Programs" radius={[4, 4, 0, 0]} barSize={20} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
          {/* المستطيلات المدمجة السفلية */}
          <div className="grid grid-cols-2 bg-[#18181A] border-x border-b border-[#D4AF37]/30 rounded-b-2xl divide-x divide-[#333] shadow-lg">
            <div className="p-4 flex items-center justify-center gap-4 hover:bg-[#202022] transition rounded-bl-2xl">
              <div className="p-3 bg-[#91B3FA]/10 rounded-xl"><Users className="w-6 h-6 text-[#91B3FA]" /></div>
              <div>
                <p className="text-xs text-gray-400 font-medium">Total Tourists Over Time</p>
                <p className="text-2xl font-bold text-white">3,650</p>
              </div>
            </div>
            <div className="p-4 flex items-center justify-center gap-4 hover:bg-[#202022] transition rounded-br-2xl">
              <div className="p-3 bg-[#F4A261]/10 rounded-xl"><Map className="w-6 h-6 text-[#F4A261]" /></div>
              <div>
                <p className="text-xs text-gray-400 font-medium">Total Published Programs</p>
                <p className="text-2xl font-bold text-[#F4A261]">209</p>
              </div>
            </div>
          </div>
        </div>

        {/* المربعات الأربعة (تأخذ 4 أعمدة) */}
        <div className="xl:col-span-4 grid grid-cols-2 gap-4">
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Calendar className="w-8 h-8 text-[#91B3FA] mb-3" />
            <p className="text-xs text-gray-400 mb-1">Joined Date</p>
            <p className="text-lg font-bold text-white">15 Mar 2024</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Wallet className="w-8 h-8 text-[#F4A261] mb-3" />
            <p className="text-xs text-gray-400 mb-1">Total Revenue</p>
            <p className="text-lg font-bold text-white">$145,200</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <MessageSquare className="w-8 h-8 text-[#91B3FA] mb-3" />
            <p className="text-xs text-gray-400 mb-1">User Reviews</p>
            <p className="text-lg font-bold text-white">1,204</p>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-5 flex flex-col justify-center items-center text-center shadow-lg hover:border-[#D4AF37]/70 transition">
            <Star className="w-8 h-8 text-[#D4AF37] mb-3 fill-[#D4AF37]" />
            <p className="text-xs text-gray-400 mb-1">Average Rating</p>
            <p className="text-lg font-bold text-white">4.8 / 5.0</p>
          </div>
        </div>
      </div>

      {/* 3. القسم السفلي (معلومات الشركة + معلومات المالك) بناءً على الصور المرفقة */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        
        {/* معلومات الشركة (Company Information) - الجهة اليسرى */}
        <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg relative overflow-hidden">
          <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#91B3FA] to-transparent opacity-50"></div>
          <div className="flex items-center gap-3 mb-6">
            <Building2 className="w-6 h-6 text-[#91B3FA]" />
            <h3 className="text-lg font-semibold text-white">Company Information</h3>
          </div>
          
          <div className="grid grid-cols-2 gap-x-4">
            <div className="col-span-2"><InfoRow icon={Building2} label="Trade Name (الاسم التجاري)" value="Global Trails Tourism Inc." /></div>
            <div className="col-span-2"><InfoRow icon={FileText} label="Short Description (نبذة عن الشركة)" value="We provide the best luxury desert and mountain trip experiences across the UAE." /></div>
            <div className="col-span-2"><InfoRow icon={MapPin} label="Location (موقع الشركة)" value="Burj Al Arab, Jumeirah, Dubai, UAE" /></div>
            <div className="col-span-1"><InfoRow icon={Phone} label="Phone Number (رقم الهاتف)" value="+971 50 123 4567" /></div>
            <div className="col-span-1"><InfoRow icon={Mail} label="Email (البريد الالكتروني)" value="contact@globaltrails.ae" /></div>
            <div className="col-span-1"><InfoRow icon={Calendar} label="Founded Date (تاريخ التأسيس)" value="10 / 1998" /></div>
            <div className="col-span-1"><InfoRow icon={FileText} label="Tourism Record Number (رقم السجل)" value="TR-98234-DXB" /></div>
            <div className="col-span-2"><InfoRow icon={CreditCard} label="Bank Account (حسابك البنكي)" value="AE12 3456 7890 1234 5678 90" /></div>
            
            {/* مكان صورة السجل السياحي */}
            <div className="col-span-2 mt-2">
              <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> Tourism Record Image (صورة السجل)</span>
              <div className="w-full h-32 border-2 border-dashed border-[#333] rounded-xl flex items-center justify-center bg-[#121212] text-gray-500">
                <ImageIcon className="w-8 h-8 opacity-50" />
              </div>
            </div>
          </div>
        </div>

        {/* معلومات مالك الشركة (Owner Information) - الجهة اليمنى */}
        <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl p-6 shadow-lg relative overflow-hidden">
           <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#F4A261] to-transparent opacity-50"></div>
          <div className="flex items-center gap-3 mb-6 border-b border-[#333] pb-4">
            <div className="w-14 h-14 rounded-full border-2 border-[#F4A261] overflow-hidden">
              {/* صورة صاحب الشركة */}
              <img src="https://i.pravatar.cc/150?img=11" alt="Owner" className="w-full h-full object-cover" />
            </div>
            <div>
              <h3 className="text-lg font-semibold text-white">Owner Information</h3>
              <p className="text-xs text-gray-400">Personal Details</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-x-4">
            <div className="col-span-1"><InfoRow icon={User} label="First Name (الاسم الأول)" value="Ahmad" /></div>
            <div className="col-span-1"><InfoRow icon={User} label="Last Name (الاسم الأخير)" value="Al-Maktoum" /></div>
            <div className="col-span-1"><InfoRow icon={Phone} label="Phone Number (رقم الهاتف)" value="+971 55 987 6543" /></div>
            <div className="col-span-1"><InfoRow icon={MapPin} label="Residence (مكان الإقامة)" value="Downtown Dubai" /></div>
            <div className="col-span-1"><InfoRow icon={User} label="Gender (الجنس)" value="Male (ذكر)" /></div>
            <div className="col-span-1"><InfoRow icon={Calendar} label="Birth Date (تاريخ الميلاد)" value="15 / 08 / 1980" /></div>
            <div className="col-span-2"><InfoRow icon={Fingerprint} label="National ID (الرقم الوطني)" value="784-1980-1234567-1" /></div>
            <div className="col-span-2"><InfoRow icon={CreditCard} label="Bank Account (حسابك البنكي)" value="AE98 7654 3210 9876 5432 10" /></div>
            
            {/* مكان صورة الهوية الشخصية */}
            <div className="col-span-2 mt-2">
              <span className="text-[11px] text-gray-500 uppercase tracking-wider mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> ID Image (صورة الهوية)</span>
              <div className="w-full h-32 border-2 border-dashed border-[#333] rounded-xl flex items-center justify-center bg-[#121212] text-gray-500">
                <ImageIcon className="w-8 h-8 opacity-50" />
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}