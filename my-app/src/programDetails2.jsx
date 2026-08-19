import  { useState } from 'react';
import { 
  ArrowLeft, Users, CalendarDays, Timer, MapPin, Star, 
  ChevronDown, ChevronUp, Clock, Coffee, /*ShieldCheck,*/ 
  Info, /*Wallet,*/ UserCheck, CheckCircle2, MapPinned, Hourglass
} from 'lucide-react';

// --- مكون القائمة المنسدلة لليوم (Accordion) ---
const DayAccordion = ({ day, isExpanded, onToggle }) => (
  <div className="bg-[#1C1C1E] border border-[#333] rounded-xl overflow-hidden mb-3 transition-all">
    <div 
      onClick={onToggle}
      className="flex items-center justify-between p-4 cursor-pointer hover:bg-[#252528] transition-colors"
    >
      <div className="flex items-center gap-4">
        <div className="w-12 h-12 rounded-lg bg-[#91B3FA]/10 border border-[#91B3FA]/20 flex flex-col items-center justify-center">
          <CalendarDays className="w-4 h-4 text-[#91B3FA] mb-0.5" />
          <span className="text-[10px] font-bold text-[#91B3FA] uppercase">{day.date}</span>
        </div>
        <div>
          <h4 className="text-white font-bold">{day.title}</h4>
          <p className="text-xs text-gray-400">{day.subtitle}</p>
        </div>
      </div>
      <div className="text-gray-400">
        {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
      </div>
    </div>
    
    {isExpanded && (
      <div className="p-4 border-t border-[#333] bg-[#18181A] space-y-4">
        {day.activities.map((activity, idx) => (
          <div key={idx} className="flex gap-4 relative">
            {idx !== day.activities.length - 1 && (
              <div className="absolute left-2.5 top-6 bottom-[-16px] w-[1px] bg-[#333]"></div>
            )}
            <div className="w-5 h-5 rounded-full bg-[#2A2A2D] border-2 border-[#555] flex items-center justify-center shrink-0 z-10 mt-0.5">
              <Clock className="w-3 h-3 text-gray-400" />
            </div>
            <div>
              <p className="text-sm font-bold text-[#EB996E] mb-1">{activity.time} <span className="text-white ml-2">{activity.name}</span></p>
              <p className="text-xs text-gray-400 flex items-start gap-1">
                <Coffee className="w-3.5 h-3.5 mt-0.5 shrink-0" /> {activity.desc}
              </p>
            </div>
          </div>
        ))}
      </div>
    )}
  </div>
);

export default function ProgramDetails({ program, onBack }) {
  const [expandedDay, setExpandedDay] = useState(0);

  // === البيانات المحدثة بناءً على واجهة إضافة البرنامج ===
  const mockProgramData = {
    title: program?.name || "Magical France Tour",
    company: {
      name: program?.companyName || "Show Travel & Tourism",
      logo: "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80" // صورة افتراضية للشركة
    },
    coverImage: "https://images.unsplash.com/photo-1499856871958-5b9627545d1a?w=800&q=80",
    description: "A magical journey with lots of excitement and thrill, visiting the most beautiful tourist places in France, including flights and accommodation in luxury hotels with meals in the finest restaurants. Pack your bags and join this interesting adventure.",
    rating: 4.8,
    touristsJoined: 120,
    registrationDeadline: "12 Jun 2026",
    startDate: "18 Jun 2026",
    duration: "5 Days & 4 Nights", // <--- تم التحديث
    meetingPoint: "Dubai International Airport - T3", // <--- تم التحديث
    locations: ["Dubai", "Burj Khalifa", "Ajman", "Future Museum", "Abu Dhabi", "Sharjah"],
    price: 200000,
    spotsLeft: 6,
    guide: {
      name: "Mohammad Al-Abdullah",
      experience: "10 Years Experience in Tourism",
      avatar: "https://i.pravatar.cc/150?img=11"
    },
    itinerary: [
      {
        id: 0,
        date: "18/6",
        title: "Day One",
        subtitle: "Arrival and Hotel Check-in",
        activities: [
          { time: "8:30 AM", name: "Traditional French Breakfast", desc: "Start the day with a breakfast at a warm Parisian cafe" },
          { time: "11:00 AM", name: "Eiffel Tower Tour", desc: "Guided tour with skip-the-line access to the summit." }
        ]
      },
      {
        id: 1,
        date: "19/6",
        title: "Day Two",
        subtitle: "Louvre Museum & Seine Cruise",
        activities: [
          { time: "9:00 AM", name: "Louvre Museum", desc: "Explore the world's largest art museum." },
          { time: "6:00 PM", name: "Seine River Cruise", desc: "Enjoy a sunset cruise with dinner." }
        ]
      }
    ],
    reviews: [
      { id: 1, name: "Ahmad Mohammad", time: "5 days ago", text: "A magical trip with lots of thrill, visiting most beautiful places..." }
    ]
  };

  return (
    <div className="p-6 md:p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* 1. الترويسة العلوية */}
      <div className="space-y-6">
        <div className="flex items-start gap-4 border-b border-[#333] pb-4">
          <button onClick={onBack} className="p-2 bg-[#1C1C1E] border border-[#333] rounded-lg hover:bg-[#252528] transition group mt-1">
            <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
          </button>
          
          <div className="flex flex-col gap-2">
            {/* بروفايل واسم الشركة فوق اسم البرنامج */}
            <div className="flex items-center gap-2 w-fit bg-[#18181A] border border-[#444] pr-4 pl-1 py-1 rounded-full">
              <div className="w-6 h-6 rounded-full overflow-hidden border border-[#91B3FA]">
                <img src={mockProgramData.company.logo} alt="Company Logo" className="w-full h-full object-cover" />
              </div>
              <span className="text-xs font-semibold text-gray-300">By {mockProgramData.company.name}</span>
            </div>
            
            {/* اسم البرنامج */}
            <h2 className="text-3xl font-bold text-white">{mockProgramData.title}</h2>
          </div>
        </div>

        {/* === المربعات الأربعة الجديدة (تتضمن مدة الرحلة ونقطة الالتقاء) === */}
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">
          
          {/* 1. مدة الرحلة (واضحة ومميزة بلون أزرق فاتح) */}
          <div className="bg-[#1C1C1E] border border-[#91B3FA]/40 rounded-2xl p-5 flex items-center gap-4 shadow-[0_0_15px_rgba(145,179,250,0.1)] relative overflow-hidden">
            <div className="absolute top-0 right-0 w-12 h-12 bg-[#91B3FA]/10 rounded-bl-full"></div>
            <div className="p-3 bg-[#91B3FA]/10 rounded-xl text-[#91B3FA]"><Hourglass className="w-6 h-6" /></div>
            <div>
              <p className="text-[11px] text-[#91B3FA] uppercase tracking-wider mb-1 font-semibold">Trip Duration</p>
              <p className="text-xl font-bold text-white">{mockProgramData.duration}</p>
            </div>
          </div>

          {/* 2. مكان الالتقاء */}
          <div className="bg-[#1C1C1E] border border-[#EB996E]/40 rounded-2xl p-5 flex items-center gap-4 shadow-[0_0_15px_rgba(235,153,110,0.05)]">
            <div className="p-3 bg-[#EB996E]/10 rounded-xl text-[#EB996E]"><MapPinned className="w-6 h-6" /></div>
            <div>
              <p className="text-[11px] text-gray-400 uppercase tracking-wider mb-1">Meeting Point</p>
              <p className="text-sm font-bold text-gray-200 leading-tight">{mockProgramData.meetingPoint}</p>
            </div>
          </div>

          {/* 3. السياح المنضمين */}
          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5 flex items-center gap-4 shadow-lg">
            <div className="p-3 bg-[#2A2A2D] rounded-xl text-gray-300"><Users className="w-6 h-6" /></div>
            <div>
              <p className="text-[11px] text-gray-400 uppercase tracking-wider mb-1">Tourists Joined</p>
              <p className="text-xl font-bold text-white">{mockProgramData.touristsJoined} <span className="text-xs font-normal text-gray-500">/ {mockProgramData.touristsJoined + mockProgramData.spotsLeft} PAX</span></p>
            </div>
          </div>
          
          {/* 4. تواريخ التسجيل والبدء */}
          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-4 flex flex-col justify-center gap-2 shadow-lg">
            <div className="flex justify-between items-center">
              <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1"><Timer className="w-3 h-3 text-[#EB996E]"/> Reg. Closes:</span>
              <span className="text-xs font-bold text-white">{mockProgramData.registrationDeadline}</span>
            </div>
            <div className="w-full h-[1px] bg-[#333]"></div>
            <div className="flex justify-between items-center">
              <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1"><CalendarDays className="w-3 h-3 text-[#91B3FA]"/> Starts On:</span>
              <span className="text-xs font-bold text-[#91B3FA]">{mockProgramData.startDate}</span>
            </div>
          </div>

        </div>
      </div>

      {/* 2. المحتوى الرئيسي المقسم لعمودين (نفس التنسيق السابق مع بعض التحسينات البصرية) */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8">
        
        {/* === العمود الأيسر (الرئيسي - 8 أعمدة) === */}
        <div className="xl:col-span-8 space-y-8">
          
          <div className="bg-[#1C1C1E] rounded-2xl overflow-hidden border border-[#333] shadow-lg">
            <div className="w-full h-64 md:h-80 relative">
              <img src={mockProgramData.coverImage} alt="Cover" className="w-full h-full object-cover" />
              <div className="absolute bottom-0 left-0 w-full h-1/2 bg-gradient-to-t from-[#1C1C1E] to-transparent"></div>
              <div className="absolute bottom-4 left-6 flex items-center gap-2 bg-black/60 backdrop-blur-md px-3 py-1.5 rounded-full border border-white/10">
                <Star className="w-4 h-4 text-[#EB996E] fill-[#EB996E]" />
                <span className="text-white font-bold text-sm">{mockProgramData.rating} Rating</span>
              </div>
            </div>
            <div className="p-6">
              <h3 className="text-lg font-bold text-white mb-3 flex items-center gap-2">
                <Info className="w-5 h-5 text-[#91B3FA]" /> Program Description
              </h3>
              <p className="text-gray-300 leading-relaxed text-sm">
                {mockProgramData.description}
              </p>
            </div>
          </div>

          <div>
            <h3 className="text-xl font-bold text-white mb-4 border-b border-[#333] pb-2">Full Trip Itinerary</h3>
            <div>
              {mockProgramData.itinerary.map((day) => (
                <DayAccordion 
                  key={day.id} 
                  day={day} 
                  isExpanded={expandedDay === day.id}
                  onToggle={() => setExpandedDay(expandedDay === day.id ? null : day.id)}
                />
              ))}
            </div>
          </div>
        </div>

        {/* === العمود الأيمن (الجانبي - 4 أعمدة) === */}
        <div className="xl:col-span-4 space-y-6">
          
          <div className="bg-gradient-to-br from-[#1C1C1E] to-[#252528] rounded-2xl border border-[#EB996E]/30 p-6 shadow-xl relative overflow-hidden">
             <div className="absolute top-[-20px] right-[-20px] w-24 h-24 bg-[#EB996E]/10 rounded-full blur-2xl"></div>
             <div className="flex justify-between items-end mb-6 border-b border-[#333] pb-4">
               <div>
                 <p className="text-xs text-[#EB996E] uppercase font-bold tracking-wider mb-1">Cost Per Person</p>
                 <h2 className="text-3xl font-bold text-white">${mockProgramData.price.toLocaleString()}</h2>
               </div>
               <div className="text-right">
                 <p className="text-xs text-gray-400 mb-1">Spots Left</p>
                 <p className="text-xl font-bold text-white">{mockProgramData.spotsLeft}</p>
               </div>
             </div>
             <button className="w-full py-3 bg-[#EB996E] hover:bg-[#d8875c] text-black font-bold rounded-xl transition-colors duration-200 shadow-[0_0_15px_rgba(235,153,110,0.3)]">
               View Enrolled Users
             </button>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <UserCheck className="w-4 h-4 text-[#91B3FA]" /> Tour Guide
            </h4>
            <div className="flex items-center gap-4">
              <img src={mockProgramData.guide.avatar} alt="Guide" className="w-12 h-12 rounded-full border-2 border-[#91B3FA]/50" />
              <div>
                <p className="text-white font-bold text-sm">{mockProgramData.guide.name}</p>
                <p className="text-xs text-gray-400">{mockProgramData.guide.experience}</p>
              </div>
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <MapPin className="w-4 h-4 text-[#91B3FA]" /> Destinations
            </h4>
            <div className="flex flex-wrap gap-2">
              {mockProgramData.locations.map((loc, i) => (
                <span key={i} className="px-3 py-1.5 bg-[#252528] border border-[#444] rounded-lg text-xs text-gray-300">
                  {loc}
                </span>
              ))}
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <CheckCircle2 className="w-4 h-4 text-[#91B3FA]" /> Flight Config
            </h4>
            <ul className="space-y-3 text-sm text-gray-300">
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Flight Tickets</span> <span className="text-[#91B3FA]">Included</span></li>
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Economy Class</span> <span className="text-gray-400">Available</span></li>
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Premium Class</span> <span className="text-gray-400">Available</span></li>
              <li className="flex justify-between"><span>Business Class</span> <span className="text-gray-400">Not Available</span></li>
            </ul>
          </div>

        </div>
      </div>
    </div>
  );
}