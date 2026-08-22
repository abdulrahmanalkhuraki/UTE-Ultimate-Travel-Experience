import { useState } from 'react';
import EnrolledUsers from './EnrolledUsers';
import { 
  ArrowLeft, Users, CalendarDays, Timer, MapPin, Star, 
  ChevronDown, ChevronUp, Clock, Coffee, 
  Info, UserCheck 
} from 'lucide-react';

const DayAccordion = ({ day, isExpanded, onToggle }) => (
  <div className="bg-[var(--color-surface)] border border-[var(--color-surface-alt)] rounded-2xl overflow-hidden mb-3 transition-all duration-200">
    <div 
      onClick={onToggle}
      className="flex items-center justify-between p-4 cursor-pointer hover:bg-[var(--color-surface-alt)] transition-colors"
    >
      <div className="flex items-center gap-4">
        <div className="w-12 h-12 rounded-xl bg-[var(--color-accent)]/10 border border-[var(--color-accent)]/20 flex flex-col items-center justify-center">
          <CalendarDays className="w-4 h-4 text-[var(--color-accent)] mb-0.5" />
          <span className="text-[10px] font-bold text-[var(--color-accent)] uppercase">{day.date}</span>
        </div>
        <div>
          <h4 className="text-[var(--color-text)] font-semibold text-sm">{day.title}</h4>
          <p className="text-xs text-[var(--color-text-muted)]">{day.subtitle}</p>
        </div>
      </div>
      <div className="text-[var(--color-text-muted)]">
        {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
      </div>
    </div>
    
    {isExpanded && (
      <div className="p-4 border-t border-[var(--color-surface-alt)] bg-[var(--color-surface-alt)] space-y-4">
        {day.activities?.map((activity, idx) => (
          <div key={idx} className="flex gap-4 relative">
            {idx !== day.activities.length - 1 && (
              <div className="absolute left-2.5 top-6 bottom-[-16px] w-[1px] bg-[var(--color-surface-alt)]"></div>
            )}
            <div className="w-5 h-5 rounded-full bg-[var(--color-surface-alt)] border-2 border-[var(--color-border)] flex items-center justify-center shrink-0 z-10 mt-0.5">
              <Clock className="w-3 h-3 text-[var(--color-text-muted)]" />
            </div>
            <div>
              <p className="text-sm font-semibold text-[var(--color-accent-2)] mb-1">
                {activity.time} <span className="text-[var(--color-text)] ml-2 font-normal">{activity.name}</span>
              </p>
              <p className="text-xs text-[var(--color-text-muted)] flex items-start gap-1.5 leading-relaxed">
                <Coffee className="w-3.5 h-3.5 mt-0.5 shrink-0 opacity-70" /> {activity.desc}
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

  if (!program) return null;

  if (showEnrolledUsers) {
    return (
      <EnrolledUsers 
        program={program} 
        onBack={() => setShowEnrolledUsers(false)} 
      />
    );
  }

  return (
    <div className="p-6 md:p-8 space-y-6 font-sans animate-in fade-in duration-300">
      
      {/* Header & Top Stats */}
      <div className="space-y-6">
        <div className="flex items-center gap-4 border-b border-[var(--color-surface-alt)] pb-5">
          <button 
            onClick={onBack} 
            className="p-2.5 bg-[var(--color-surface)] border border-[var(--color-surface-alt)] rounded-xl hover:bg-[var(--color-surface-alt)] transition group shadow-sm"
          >
            <ArrowLeft className="w-5 h-5 text-[var(--color-text-muted)] group-hover:text-[var(--color-text)]" />
          </button>
          <div>
            <h2 className="text-2xl font-bold text-[var(--color-text)] tracking-tight">{program.title}</h2>
            <p className="text-sm text-[var(--color-text-muted)] flex items-center gap-1.5 mt-1">
              By <span className="text-[var(--color-accent)] font-medium">{program.companyName}</span>
            </p>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-[var(--color-surface)] border border-[var(--color-accent)]/20 rounded-2xl p-5 flex items-center gap-4 shadow-xl backdrop-blur-md">
            <div className="p-3 bg-[var(--color-accent)]/10 rounded-xl text-[var(--color-accent)]">
              <Users className="w-6 h-6" />
            </div>
            <div>
              <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Tourists Joined</p>
              <p className="text-2xl font-bold text-[var(--color-text)]">
                {program.touristsJoined ?? '—'} <span className="text-sm font-normal text-[var(--color-text-muted)]">PAX</span>
              </p>
            </div>
          </div>
          
          <div className="bg-[var(--color-surface)] border border-[var(--color-accent-2)]/30 rounded-2xl p-5 flex items-center gap-4 shadow-xl relative overflow-hidden backdrop-blur-md">
            <div className="absolute right-0 top-0 w-1 h-full bg-[var(--color-accent-2)]"></div>
            <div className="p-3 bg-[var(--color-accent-2)]/10 rounded-xl text-[var(--color-accent-2)]">
              <Timer className="w-6 h-6" />
            </div>
            <div>
              <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Registration Deadline</p>
              <p className="text-lg font-bold text-[var(--color-text)]">{program.registrationDeadline}</p>
            </div>
          </div>

          <div className="bg-[var(--color-surface)] border border-[var(--color-surface-alt)] rounded-2xl p-5 flex items-center gap-4 shadow-xl backdrop-blur-md">
            <div className="p-3 bg-[var(--color-surface-alt)] rounded-xl text-[var(--color-text)]">
              <CalendarDays className="w-6 h-6" />
            </div>
            <div>
              <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Program Start Date</p>
              <p className="text-lg font-bold text-[var(--color-accent)]">{program.startDate}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Main Grid Content */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8">
        
        {/* Left Column (Main - 8 Columns) */}
        <div className="xl:col-span-8 space-y-8">
          
          {/* Cover & Description */}
          <div className="bg-[var(--color-surface)] rounded-2xl overflow-hidden border border-[var(--color-surface-alt)] shadow-xl">
            <div className="w-full h-72 md:h-80 relative">
              <img src={program.coverImage} alt="Cover" className="w-full h-full object-cover" />
              <div className="absolute bottom-0 left-0 w-full h-1/2 bg-gradient-to-t from-[var(--color-surface)] to-transparent"></div>
              <div className="absolute bottom-4 left-6 flex items-center gap-2 bg-black/60 backdrop-blur-md px-3.5 py-1.5 rounded-full border border-white/10 shadow-lg">
                <Star className="w-4 h-4 text-[var(--color-accent-2)] fill-[var(--color-accent-2)]" />
                <span className="text-[var(--color-text)] font-semibold text-sm">{program.rating} Rating</span>
              </div>
            </div>
            <div className="p-6">
              <h3 className="text-base font-semibold text-[var(--color-text)] mb-3 flex items-center gap-2">
                <Info className="w-5 h-5 text-[var(--color-accent)]" /> Program Description
              </h3>
              <p className="text-[var(--color-text-muted)] leading-relaxed text-sm">
                {program.description}
              </p>
            </div>
          </div>

          {/* Itinerary */}
          <div>
            <h3 className="text-lg font-bold text-[var(--color-text)] mb-4 border-b border-[var(--color-surface-alt)] pb-3">Full Trip Itinerary</h3>
            <div>
              {program.itinerary?.map((day) => (
                <DayAccordion 
                  key={day.id} 
                  day={day} 
                  isExpanded={expandedDay === day.id}
                  onToggle={() => setExpandedDay(expandedDay === day.id ? null : day.id)}
                />
              ))}
            </div>
          </div>

          {/* Reviews */}
          {program.reviews && program.reviews.length > 0 && (
            <div>
              <h3 className="text-lg font-bold text-[var(--color-text)] mb-4 border-b border-[var(--color-surface-alt)] pb-3 flex items-center gap-2">
                <Users className="w-5 h-5 text-[var(--color-accent-2)]" /> Tourist Reviews ({program.reviews.length})
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {program.reviews.map(review => (
                  <div key={review.id} className="bg-[var(--color-surface)] p-4.5 rounded-2xl border border-[var(--color-surface-alt)] shadow-sm">
                    <div className="flex items-center justify-between mb-3">
                      <div className="flex items-center gap-2.5">
                        <div className="w-8 h-8 rounded-full bg-[var(--color-accent-2)]/20 flex items-center justify-center text-[var(--color-accent-2)] font-bold text-sm">
                          {review.name?.charAt(0)}
                        </div>
                        <span className="text-sm font-semibold text-[var(--color-text)]">{review.name}</span>
                      </div>
                      <span className="text-[11px] text-[var(--color-text-muted)]">{review.time}</span>
                    </div>
                    <p className="text-xs text-[var(--color-text-muted)] italic leading-relaxed">"{review.text}"</p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Right Column (Sidebar - 4 Columns) */}
        <div className="xl:col-span-4 space-y-6">
          
          {/* Pricing & Actions */}
          <div className="bg-gradient-to-br from-[var(--color-surface)] to-[var(--color-surface-alt)] rounded-2xl border border-[var(--color-accent-2)]/30 p-6 shadow-2xl relative overflow-hidden">
             <div className="absolute top-[-20px] right-[-20px] w-24 h-24 bg-[var(--color-accent-2)]/10 rounded-full blur-2xl"></div>
             
             <div className="flex justify-between items-end mb-6 border-b border-[var(--color-surface-alt)] pb-4">
               <div>
                 <p className="text-xs text-[var(--color-accent-2)] uppercase font-bold tracking-wider mb-1">Cost Per Person</p>
                 <h2 className="text-3xl font-bold text-[var(--color-text)]">${program.price?.toLocaleString()}</h2>
               </div>
               <div className="text-right">
                 <p className="text-xs text-[var(--color-text-muted)] mb-1">Spots Left</p>
                 <p className="text-xl font-bold text-[var(--color-text)]">{program.spotsLeft}</p>
               </div>
             </div>

             <button
               onClick={() => setShowEnrolledUsers(true)}
               className="w-full py-3.5 bg-[var(--color-accent-2)] hover:opacity-90 text-[var(--color-on-accent-2)] font-bold text-sm rounded-xl transition-all duration-200 active:scale-[0.98]">
                View Enrolled Users
             </button>
          </div>

          {/* Tour Guide */}
          {program.guide && (
            <div className="bg-[var(--color-surface)] border border-[var(--color-surface-alt)] rounded-2xl p-5 shadow-lg">
              <h4 className="text-xs font-bold text-[var(--color-text-muted)] uppercase tracking-wider mb-4 flex items-center gap-2">
                <UserCheck className="w-4 h-4 text-[var(--color-accent)]" /> Tour Guide
              </h4>
              <div className="flex items-center gap-4">
                <img src={program.guide.avatar} alt="Guide" className="w-12 h-12 rounded-full object-cover border-2 border-[var(--color-accent)]/40 shadow-md" />
                <div>
                  <p className="text-[var(--color-text)] font-semibold text-sm">{program.guide.name}</p>
                  <p className="text-xs text-[var(--color-text-muted)] mt-0.5">{program.guide.experience}</p>
                </div>
              </div>
            </div>
          )}

          {/* Destinations */}
          {program.locations && program.locations.length > 0 && (
            <div className="bg-[var(--color-surface)] border border-[var(--color-surface-alt)] rounded-2xl p-5 shadow-lg">
              <h4 className="text-xs font-bold text-[var(--color-text-muted)] uppercase tracking-wider mb-4 flex items-center gap-2">
                <MapPin className="w-4 h-4 text-[var(--color-accent)]" /> Destinations
              </h4>
              <div className="flex flex-wrap gap-2">
                {program.locations.map((loc, i) => (
                  <span key={i} className="px-3 py-1.5 bg-[var(--color-surface-alt)] border border-[var(--color-border)] rounded-xl text-xs text-[var(--color-text-muted)] font-medium">
                    {loc}
                  </span>
                ))}
              </div>
            </div>
          )}

        </div>
      </div>
    </div>
  );
}