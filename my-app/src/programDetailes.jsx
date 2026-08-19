import  { useState } from 'react';
import EnrolledUsers from './EnrolledUsers';
import { 
  ArrowLeft, Users, CalendarDays, Timer, MapPin, Star, 
  ChevronDown, ChevronUp, Clock, Coffee, ShieldCheck, 
  Info, /*Wallet,*/ UserCheck, CheckCircle2
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
    
    {/* تفاصيل اليوم */}
    {isExpanded && (
      <div className="p-4 border-t border-[#333] bg-[#18181A] space-y-4">
        {day.activities.map((activity, idx) => (
          <div key={idx} className="flex gap-4 relative">
            {/* خط التايم لاين */}
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
  const [showEnrolledUsers, setShowEnrolledUsers] = useState(false);

  // === بيانات وهمية احتياطية (تُستخدم فقط للحقول يلي مالها API) ===
  const mockProgramData = {
    title: program?.title || program?.name || "Magical France Tour",
    companyName: program?.company || program?.companyName || "Show Travel & Tourism",
    coverImage: program?.coverImage || "https://images.unsplash.com/photo-1499856871958-5b9627545d1a?w=800&q=80",
    description: program?.description || "A magical journey with lots of excitement and thrill, visiting the most beautiful tourist places in France, including flights and accommodation in luxury hotels with meals in the finest restaurants. Pack your bags and join this interesting adventure.",
    rating: program?.rating ?? 4.8,
    // ما في API بيرجع عدد السياح المنضمين فعلياً للبرنامج
    touristsJoined: program?.touristsJoined ?? '—',
    registrationDeadline: program?.registrationDeadline || "12 Jun 2026",
    startDate: program?.startDate || "18 Jun 2026",
    duration: program?.duration || "5 Days",
    locations: program?.locations?.length ? program.locations : ["Dubai", "Burj Khalifa", "Ajman", "Future Museum", "Abu Dhabi", "Sharjah"],
    price: program?.price ?? 200000,
    spotsLeft: program?.spotsLeft ?? 6,
    guide: program?.guide || {
      name: "Mohammad Al-Abdullah",
      experience: "10 Years Experience in Tourism",
      avatar: "https://i.pravatar.cc/150?img=11"
    },
    itinerary: program?.itinerary?.length ? program.itinerary : [
      {
        id: 0,
        date: "18/6",
        title: "Day One",
        subtitle: "Arrival and Hotel Check-in",
        activities: [
          { time: "8:30 AM", name: "Traditional French Breakfast", desc: "Start the day with a breakfast at a warm Parisian cafe (Fresh Coffee & Croissant)" },
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
    // ما في API بيرجع مراجعات/تقييمات كتابية للبرنامج
    reviews: program?.reviews || [
      { id: 1, name: "Ahmad Mohammad", time: "5 days ago", text: "A magical trip with lots of thrill, visiting most beautiful places..." },
      { id: 2, name: "Sara Ali", time: "1 week ago", text: "Excellent organization and the tour guide was very helpful." }
    ]
  };

  if (showEnrolledUsers) {
    return (
      <EnrolledUsers 
        program={mockProgramData} 
        onBack={() => setShowEnrolledUsers(false)} 
      />
    );
  }

  return (
    <div className="p-6 md:p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* 1. الترويسة العلوية (Header & Top Stats) */}
      <div className="space-y-6">
        <div className="flex items-center gap-4 border-b border-[#333] pb-4">
          <button onClick={onBack} className="p-2 bg-[#1C1C1E] border border-[#333] rounded-lg hover:bg-[#252528] transition group">
            <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
          </button>
          <div>
            <h2 className="text-2xl font-bold text-white">{mockProgramData.title}</h2>
            <p className="text-sm text-gray-400 flex items-center gap-1.5 mt-1">
              By <span className="text-[#91B3FA] font-semibold">{mockProgramData.companyName}</span>
            </p>
          </div>
        </div>

        {/* المربعات الثلاثة المطلوبة (الإحصائيات العلوية) */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-[#1C1C1E] border border-[#91B3FA]/20 rounded-2xl p-5 flex items-center gap-4 shadow-lg">
            <div className="p-3 bg-[#91B3FA]/10 rounded-xl text-[#91B3FA]"><Users className="w-6 h-6" /></div>
            <div>
              <p className="text-xs text-gray-400 uppercase mb-1">Tourists Joined</p>
              <p className="text-2xl font-bold text-white">{mockProgramData.touristsJoined} <span className="text-sm font-normal text-gray-500">PAX</span></p>
            </div>
          </div>
          
          <div className="bg-[#1C1C1E] border border-[#EB996E]/30 rounded-2xl p-5 flex items-center gap-4 shadow-lg relative overflow-hidden">
            <div className="absolute right-0 top-0 w-1 h-full bg-[#EB996E]"></div>
            <div className="p-3 bg-[#EB996E]/10 rounded-xl text-[#EB996E]"><Timer className="w-6 h-6" /></div>
            <div>
              <p className="text-xs text-gray-400 uppercase mb-1">Registration Deadline</p>
              <p className="text-xl font-bold text-white">{mockProgramData.registrationDeadline}</p>
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5 flex items-center gap-4 shadow-lg">
            <div className="p-3 bg-[#333] rounded-xl text-white"><CalendarDays className="w-6 h-6" /></div>
            <div>
              <p className="text-xs text-gray-400 uppercase mb-1">Program Start Date</p>
              <p className="text-xl font-bold text-[#91B3FA]">{mockProgramData.startDate}</p>
            </div>
          </div>
        </div>
      </div>

      {/* 2. المحتوى الرئيسي المقسم لعمودين */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8">
        
        {/* === العمود الأيسر (الرئيسي - 8 أعمدة) === */}
        <div className="xl:col-span-8 space-y-8">
          
          {/* صورة الغلاف والوصف */}
          <div className="bg-[#1C1C1E] rounded-2xl overflow-hidden border border-[#333] shadow-lg">
            <div className="w-full h-64 md:h-80 relative">
              <img src={mockProgramData.coverImage} alt="Cover" className="w-full h-full object-cover" />
              <div className="absolute bottom-0 left-0 w-full h-1/2 bg-gradient-to-t from-[#1C1C1E] to-transparent"></div>
              <div className="absolute bottom-4 left-6 flex items-center gap-2 bg-black/50 backdrop-blur-md px-3 py-1.5 rounded-full border border-white/10">
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

          {/* الجدول الزمني (Itinerary) */}
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

          {/* المراجعات */}
          <div>
            <h3 className="text-xl font-bold text-white mb-4 border-b border-[#333] pb-2 flex items-center gap-2">
              <Users className="w-5 h-5 text-[#EB996E]" /> Tourist Reviews ({mockProgramData.reviews.length})
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {mockProgramData.reviews.map(review => (
                <div key={review.id} className="bg-[#1C1C1E] p-4 rounded-xl border border-[#333]">
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-2">
                      <div className="w-8 h-8 rounded-full bg-[#EB996E]/20 flex items-center justify-center text-[#EB996E] font-bold">
                        {review.name.charAt(0)}
                      </div>
                      <span className="text-sm font-bold text-white">{review.name}</span>
                    </div>
                    <span className="text-[10px] text-gray-500">{review.time}</span>
                  </div>
                  <p className="text-xs text-gray-400 italic">"{review.text}"</p>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* === العمود الأيمن (الجانبي - 4 أعمدة) === */}
        <div className="xl:col-span-4 space-y-6">
          
          {/* التسعير والتسجيل */}
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

             <button
             onClick={() => setShowEnrolledUsers(true)}
             className="w-full py-3 bg-[#EB996E] hover:bg-[#d8875c] text-black font-bold rounded-xl transition-colors duration-200 shadow-[0_0_15px_rgba(235,153,110,0.3)]">
               View Enrolled Users
             </button>
          </div>

          {/* الدليل السياحي */}
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

          {/* المناطق المزارة */}
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

          {/* تفضيلات الحجز المتاحة (Booking Configurations) */}
          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <CheckCircle2 className="w-4 h-4 text-[#91B3FA]" /> Configured Preferences
            </h4>
            <ul className="space-y-3 text-sm text-gray-300">
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Add Companions</span> <span className="text-[#91B3FA]">Enabled</span></li>
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Flight Tickets</span> <span className="text-[#91B3FA]">Optional</span></li>
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Accommodation</span> <span className="text-[#91B3FA]">Required</span></li>
              <li className="flex justify-between"><span>Food Preferences</span> <span className="text-[#91B3FA]">Enabled</span></li>
            </ul>
          </div>

          {/* الملاحظات */}
          <div className="bg-[#2a1f1a] border border-[#EB996E]/20 rounded-2xl p-5">
            <h4 className="text-sm font-bold text-[#EB996E] uppercase mb-3 flex items-center gap-2">
              <ShieldCheck className="w-4 h-4" /> Important Notes
            </h4>
            <ul className="space-y-3 text-xs text-gray-300 list-disc list-inside">
              <li>Price per person will change based on preferences chosen during booking.</li>
              <li>Group trips are carefully studied to suit most people.</li>
              <li>Immediate booking automatically deducts the amount from the wallet.</li>
            </ul>
          </div>

        </div>
      </div>
    </div>
  );
}