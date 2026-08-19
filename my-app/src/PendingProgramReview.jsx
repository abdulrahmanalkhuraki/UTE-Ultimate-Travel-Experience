import  { useState } from 'react';
import { 
  ArrowLeft, Users, CalendarDays, Timer, MapPin, 
  ChevronDown, ChevronUp, Clock,  
  UserCheck, CheckCircle2, MapPinned, Hourglass, 
  Check, X, AlertTriangle
} from 'lucide-react';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';

// --- مكون القائمة المنسدلة للأيام ---
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
              <p className="text-xs text-gray-400">{activity.desc}</p>
            </div>
          </div>
        ))}
      </div>
    )}
  </div>
);

export default function PendingProgramReview({ program, onBack, onDecision }) {
  const [expandedDay, setExpandedDay] = useState(0);
  
  // حالات الـ Dialog للرفض والقبول
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);

  // === بيانات وهمية لبرنامج قيد الانتظار (مستمدة من واجهة الإضافة) ===
  const mockPendingProgram = {
    id: "PRG-8892",
    title: "Istanbul Cultural Escape",
    company: {
      name: "Show Travel & Tourism",
      logo: "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80"
    },
    coverImage: "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=800&q=80",
    description: "A comprehensive 6-day tour exploring the historical wonders of Istanbul, including the Blue Mosque, Hagia Sophia, and a Bosphorus cruise. Perfect for culture enthusiasts and food lovers.",
    submissionDate: "2 Hours ago",
    registrationDeadline: "20 Jul 2026",
    startDate: "25 Jul 2026",
    duration: "6 Days & 5 Nights",
    meetingPoint: "Istanbul Airport - Gate 4",
    locations: ["Istanbul", "Bosphorus", "Sultanahmet", "Taksim"],
    price: 350000,
    totalSpots: 20,
    guide: {
      name: "Ahmed Yilmaz",
      experience: "8 Years Experience",
      avatar: "https://i.pravatar.cc/150?img=15"
    },
    itinerary: [
      {
        id: 0,
        date: "25/7",
        title: "Day One",
        subtitle: "Arrival & Welcome Dinner",
        activities: [
          { time: "2:00 PM", name: "Hotel Check-in", desc: "Settle into your 5-star accommodation." },
          { time: "7:30 PM", name: "Welcome Dinner", desc: "Traditional Turkish kebab and authentic sweets." }
        ]
      },
      {
        id: 1,
        date: "26/7",
        title: "Day Two",
        subtitle: "Historical Peninsula Tour",
        activities: [
          { time: "9:00 AM", name: "Hagia Sophia", desc: "Guided tour of the grand mosque." },
          { time: "1:00 PM", name: "Grand Bazaar", desc: "Free time for shopping and exploration." }
        ]
      }
    ]
  };

  const selectedProgram = {
    id: program?.id || mockPendingProgram.id,
    title: program?.title || mockPendingProgram.title,
    company: {
      name: program?.company || mockPendingProgram.company.name,
      logo: program?.companyLogo || mockPendingProgram.company.logo,
    },
    coverImage: program?.coverImage || mockPendingProgram.coverImage,
    description: program?.description || mockPendingProgram.description,
    submissionDate: program?.submissionDate || mockPendingProgram.submissionDate,
    registrationDeadline: program?.registrationDeadline || mockPendingProgram.registrationDeadline,
    startDate: program?.startDate || mockPendingProgram.startDate,
    duration: program?.duration || mockPendingProgram.duration,
    meetingPoint: program?.meetingPoint || mockPendingProgram.meetingPoint,
    locations: program?.locations || mockPendingProgram.locations,
    price: program?.price || mockPendingProgram.price,
    totalSpots: program?.totalSpots || mockPendingProgram.totalSpots,
    guide: program?.guide || mockPendingProgram.guide,
    itinerary: program?.itinerary || mockPendingProgram.itinerary,
  };

  // دوال الأزرار
  const handleApprove = () => {
    setIsApproveDialogOpen(true);
  };

  const handleApproveConfirm = () => {
    console.log("Program Approved:", selectedProgram.id);
    setIsApproveDialogOpen(false);
    onDecision?.(selectedProgram.id);
  };

  const handleRejectConfirm = (reason) => {
    // هنا تضع كود إرسال سبب الرفض (API Call)
    console.log(`Program Rejected: ${selectedProgram.id}. Reason: ${reason}`);
    setIsRejectDialogOpen(false);
    onDecision?.(selectedProgram.id);
  };

  return (
    <div className="p-6 md:p-8 space-y-6 font-sans animate-in fade-in duration-300 pb-32 relative min-h-screen">
      
      {/* --- شريط التنبيه (Review Mode) --- */}
      <div className="bg-[#EB996E]/10 border border-[#EB996E]/30 rounded-xl p-4 flex items-center gap-3">
        <AlertTriangle className="w-5 h-5 text-[#EB996E]" />
        <div>
          <h4 className="text-[#EB996E] font-bold text-sm">Action Required: Pending Approval</h4>
          <p className="text-xs text-[#EB996E]/80 mt-0.5">Please review the program details below before accepting or rejecting the company's application.</p>
        </div>
      </div>

      {/* --- 1. الترويسة العلوية --- */}
      <div className="space-y-6">
        <div className="flex flex-col md:flex-row md:items-start justify-between gap-4 border-b border-[#333] pb-4">
          <div className="flex items-start gap-4">
            <button onClick={onBack} className="p-2 bg-[#1C1C1E] border border-[#333] rounded-lg hover:bg-[#252528] transition group mt-1">
              <ArrowLeft className="w-5 h-5 text-gray-400 group-hover:text-white" />
            </button>
            
            <div className="flex flex-col gap-2">
              <div className="flex items-center gap-2 w-fit bg-[#18181A] border border-[#444] pr-4 pl-1 py-1 rounded-full">
                <div className="w-6 h-6 rounded-full overflow-hidden border border-[#91B3FA]">
                  <img src={selectedProgram.company.logo} alt="Company Logo" className="w-full h-full object-cover" />
                </div>
                <span className="text-xs font-semibold text-gray-300">Submitted by {selectedProgram.company.name}</span>
                <span className="text-[10px] text-gray-500 ml-2 border-l border-[#444] pl-2">{selectedProgram.submissionDate}</span>
              </div>
              <h2 className="text-3xl font-bold text-white">{selectedProgram.title}</h2>
            </div>
          </div>
        </div>

        {/* --- 2. المربعات الأربعة السريعة --- */}
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4">
          <div className="bg-[#1C1C1E] border border-[#91B3FA]/40 rounded-2xl p-5 flex items-center gap-4 shadow-lg relative overflow-hidden">
             <div className="absolute top-0 right-0 w-12 h-12 bg-[#91B3FA]/10 rounded-bl-full"></div>
             <div className="p-3 bg-[#91B3FA]/10 rounded-xl text-[#91B3FA]"><Hourglass className="w-6 h-6" /></div>
             <div>
               <p className="text-[11px] text-[#91B3FA] uppercase tracking-wider mb-1 font-semibold">Duration</p>
               <p className="text-lg font-bold text-white">{selectedProgram.duration}</p>
             </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#EB996E]/40 rounded-2xl p-5 flex items-center gap-4 shadow-lg">
             <div className="p-3 bg-[#EB996E]/10 rounded-xl text-[#EB996E]"><MapPinned className="w-6 h-6" /></div>
             <div>
               <p className="text-[11px] text-gray-400 uppercase tracking-wider mb-1">Meeting Point</p>
               <p className="text-sm font-bold text-gray-200 leading-tight">{selectedProgram.meetingPoint}</p>
             </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5 flex items-center gap-4 shadow-lg">
             <div className="p-3 bg-[#2A2A2D] rounded-xl text-gray-300"><Users className="w-6 h-6" /></div>
             <div>
               <p className="text-[11px] text-gray-400 uppercase tracking-wider mb-1">Total Capacity</p>
               <p className="text-xl font-bold text-white">{selectedProgram.totalSpots} <span className="text-xs font-normal text-gray-500">PAX</span></p>
             </div>
          </div>
          
          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-4 flex flex-col justify-center gap-2 shadow-lg">
             <div className="flex justify-between items-center">
               <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1"><Timer className="w-3 h-3 text-[#EB996E]"/> Reg. Closes:</span>
               <span className="text-xs font-bold text-white">{selectedProgram.registrationDeadline}</span>
             </div>
             <div className="w-full h-[1px] bg-[#333]"></div>
             <div className="flex justify-between items-center">
               <span className="text-[10px] text-gray-500 uppercase flex items-center gap-1"><CalendarDays className="w-3 h-3 text-[#91B3FA]"/> Starts On:</span>
               <span className="text-xs font-bold text-[#91B3FA]">{selectedProgram.startDate}</span>
             </div>
          </div>
        </div>
      </div>

      {/* --- 3. المحتوى الرئيسي --- */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8 pb-32">
        
        {/* العمود الأيسر */}
        <div className="xl:col-span-8 space-y-8">
          <div className="bg-[#1C1C1E] rounded-2xl overflow-hidden border border-[#333] shadow-lg">
            <div className="w-full h-64 md:h-80 relative">
              <img src={selectedProgram.coverImage} alt="Cover" className="w-full h-full object-cover" />
              <div className="absolute bottom-0 left-0 w-full h-1/2 bg-gradient-to-t from-[#1C1C1E] to-transparent"></div>
            </div>
            <div className="p-6">
              <h3 className="text-lg font-bold text-white mb-3">Program Description</h3>
              <p className="text-gray-300 leading-relaxed text-sm">{selectedProgram.description}</p>
            </div>
          </div>

          <div>
            <h3 className="text-xl font-bold text-white mb-4 border-b border-[#333] pb-2">Proposed Itinerary</h3>
            <div>
              {selectedProgram.itinerary.map((day) => (
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

        {/* العمود الأيمن */}
        <div className="xl:col-span-4 space-y-6">
          <div className="bg-[#1C1C1E] rounded-2xl border border-[#333] p-6 shadow-xl text-center">
             <p className="text-xs text-gray-400 uppercase tracking-wider mb-2">Proposed Ticket Price</p>
             <h2 className="text-4xl font-bold text-[#EB996E] mb-1">${selectedProgram.price.toLocaleString()}</h2>
             <p className="text-xs text-gray-500">Per Tourist</p>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <UserCheck className="w-4 h-4 text-[#91B3FA]" /> Assigned Guide
            </h4>
            <div className="flex items-center gap-4">
              <img src={selectedProgram.guide.avatar} alt="Guide" className="w-12 h-12 rounded-full border-2 border-[#91B3FA]/50" />
              <div>
                <p className="text-white font-bold text-sm">{selectedProgram.guide.name}</p>
                <p className="text-xs text-gray-400">{selectedProgram.guide.experience}</p>
              </div>
            </div>
          </div>

          <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl p-5">
            <h4 className="text-sm font-bold text-gray-400 uppercase mb-4 flex items-center gap-2">
              <MapPin className="w-4 h-4 text-[#91B3FA]" /> Destinations
            </h4>
            <div className="flex flex-wrap gap-2">
              {selectedProgram.locations.map((loc, i) => (
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
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Tickets</span> <span className="text-[#91B3FA]">Included</span></li>
              <li className="flex justify-between border-b border-[#333] pb-2"><span>Economy</span> <span className="text-gray-400">Yes</span></li>
              <li className="flex justify-between"><span>Business</span> <span className="text-gray-400">Yes</span></li>
            </ul>
          </div>
        </div>
      </div>

      {/* --- 4. شريط الإجراءات (الرفض والقبول) في أسفل الشاشة --- */}
      <div className="fixed bottom-0 right-0 left-0 md:left-64 bg-[#18181A]/90 backdrop-blur-md border-t border-[#333] p-4 px-6 md:px-12 flex justify-end items-center gap-4 z-40 shadow-[0_-10px_30px_rgba(0,0,0,0.5)]">
        <button 
          onClick={() => setIsRejectDialogOpen(true)}
          className="flex items-center justify-center gap-2 px-8 py-3 bg-red-500/10 text-red-400 hover:bg-red-500/20 border border-red-500/20 rounded-xl font-bold transition-all duration-200 w-full md:w-auto"
        >
          <X className="w-5 h-5" /> Reject Program
        </button>
        
        <button 
          onClick={handleApprove}
          className="flex items-center justify-center gap-2 px-8 py-3 bg-[#91B3FA] hover:bg-[#7fa1e8] text-black rounded-xl font-bold transition-all duration-200 shadow-[0_0_15px_rgba(145,179,250,0.2)] w-full md:w-auto"
        >
          <Check className="w-5 h-5" /> Approve & Publish
        </button>
      </div>

      {/* --- 5. نافذة الرفض المنبثقة --- */}
      <RejectDialog 
        isOpen={isRejectDialogOpen}
        onClose={() => setIsRejectDialogOpen(false)}
        onSubmit={handleRejectConfirm}
        targetName={selectedProgram.title}
      />

      <ApproveDialog
        isOpen={isApproveDialogOpen}
        onClose={() => setIsApproveDialogOpen(false)}
        onConfirm={handleApproveConfirm}
        targetName={selectedProgram.title}
      />

    </div>
  );
}