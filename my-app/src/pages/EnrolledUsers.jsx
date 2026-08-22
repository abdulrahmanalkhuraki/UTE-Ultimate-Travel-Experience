import { 
  ArrowLeft, Users, CalendarDays, Timer, 
  UserPlus, Wallet, BadgeCheck, Clock
} from 'lucide-react';

// === بيانات وهمية للمستخدمين المسجلين ===
const mockEnrolledUsers = [
  {
    id: 1,
    name: "Ahmad Al-Khaled",
    avatar: "https://i.pravatar.cc/150?img=11",
    enrollDate: "10 Jun 2026",
    companions: 2,
    amountPaid: 600000,
    status: "Confirmed"
  },
  {
    id: 2,
    name: "Sara Mohammad",
    avatar: "https://i.pravatar.cc/150?img=5",
    enrollDate: "11 Jun 2026",
    companions: 0,
    amountPaid: 200000,
    status: "Confirmed"
  },
  {
    id: 3,
    name: "Omar Yassin",
    avatar: "https://i.pravatar.cc/150?img=12",
    enrollDate: "12 Jun 2026",
    companions: 4,
    amountPaid: 1000000,
    status: "Confirmed"
  },
  {
    id: 4,
    name: "Laila Mahmoud",
    avatar: "https://i.pravatar.cc/150?img=9",
    enrollDate: "12 Jun 2026",
    companions: 1,
    amountPaid: 400000,
    status: "Pending" // كمثال لحالة مختلفة
  }
];

export default function EnrolledUsers({ program, onBack }) {
  // أخذ الإحصائيات من الـ program props الممررة
  const totalEnrolled = program?.touristsJoined || 120;
  const spotsLeft = program?.spotsLeft || 6;
  const deadline = program?.registrationDeadline || "12 Jun 2026";

  return (
    <div className="p-6 md:p-8 space-y-8 font-sans animate-in fade-in duration-300">
      
      {/* === 1. الترويسة العلوية === */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-[var(--color-border)] pb-6">
        
        {/* اليسار: زر الرجوع واسم البرنامج */}
        <div className="flex items-center gap-4">
          <button 
            onClick={onBack} 
            className="p-2 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-lg hover:bg-[var(--color-surface-alt)] transition group"
          >
            <ArrowLeft className="w-5 h-5 text-[var(--color-text-muted)] group-hover:text-[var(--color-text)]" />
          </button>
          <div>
            <h2 className="text-2xl font-bold text-[var(--color-text)] flex items-center gap-2">
              {program?.title || "Program Name"} 
            </h2>
            <p className="text-sm text-[var(--color-text-muted)] mt-0.5">Enrolled Tourists List</p>
          </div>
        </div>

        {/* اليمين: بروفايل الشركة واسمها */}
        <div className="flex items-center gap-3 bg-[var(--color-surface)] border border-[var(--color-border)] pr-4 pl-1.5 py-1.5 rounded-full shadow-md">
          <div className="w-8 h-8 rounded-full overflow-hidden border border-[var(--color-accent)]/50">
            <img 
              src={program?.company?.logo || "https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80"} 
              alt="Company Logo" 
              className="w-full h-full object-cover" 
            />
          </div>
          <span className="text-sm font-semibold text-[var(--color-text-muted)]">
            {program?.company?.name || "Company Name"}
          </span>
        </div>
      </div>

      {/* === 2. مربعات الإحصائيات العلوية === */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        
        {/* المربع الأول: عدد المسجلين الكلي */}
        <div className="bg-[var(--color-surface)] border border-[var(--color-accent)]/30 rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[var(--color-accent)]/10 rounded-xl">
            <Users className="w-7 h-7 text-[var(--color-accent)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Total Enrolled</p>
            <p className="text-2xl font-bold text-[var(--color-text)]">
              {totalEnrolled} <span className="text-sm text-[var(--color-text-muted)] font-normal">Tourists</span>
            </p>
          </div>
        </div>

        {/* المربع الثاني: الأماكن المتبقية */}
        <div className="bg-[var(--color-surface)] border border-[var(--color-accent-2)]/30 rounded-2xl p-6 shadow-lg flex items-center gap-4 relative overflow-hidden">
          <div className="absolute top-0 right-0 w-16 h-16 bg-[var(--color-accent-2)]/5 rounded-bl-full"></div>
          <div className="p-4 bg-[var(--color-accent-2)]/10 rounded-xl">
            <UserPlus className="w-7 h-7 text-[var(--color-accent-2)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-accent-2)]/80 uppercase tracking-wider mb-1 font-semibold">Spots Left</p>
            <p className="text-2xl font-bold text-[var(--color-accent-2)]">
              {spotsLeft} <span className="text-sm text-[var(--color-text-muted)] font-normal">Available</span>
            </p>
          </div>
        </div>

        {/* المربع الثالث: انتهاء التسجيل */}
        <div className="bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl p-6 shadow-lg flex items-center gap-4">
          <div className="p-4 bg-[var(--color-surface-alt)] rounded-xl">
            <Timer className="w-7 h-7 text-[var(--color-text-muted)]" />
          </div>
          <div>
            <p className="text-xs text-[var(--color-text-muted)] uppercase tracking-wider mb-1">Registration Closes</p>
            <p className="text-xl font-bold text-[var(--color-text)]">{deadline}</p>
          </div>
        </div>
      </div>

      {/* === 3. شبكة المستخدمين المسجلين === */}
      <div>
        <h3 className="text-lg font-bold text-[var(--color-text)] mb-6 border-b border-[var(--color-border)] pb-2">Enrolled Users Details</h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
          {mockEnrolledUsers.map(user => (
            <div 
              key={user.id} 
              className="bg-[var(--color-surface-alt)] border border-[var(--color-border)] hover:border-[var(--color-accent)]/40 transition-colors duration-300 rounded-2xl p-5 shadow-lg relative"
            >
              {/* حالة الدفع/التسجيل */}
              <div className="absolute top-4 right-4 flex items-center gap-1">
                {user.status === "Confirmed" ? (
                  <span className="flex items-center gap-1 text-[10px] text-green-400 bg-green-400/10 px-2 py-1 rounded-full border border-green-400/20">
                    <BadgeCheck className="w-3 h-3" /> Confirmed
                  </span>
                ) : (
                  <span className="flex items-center gap-1 text-[10px] text-yellow-400 bg-yellow-400/10 px-2 py-1 rounded-full border border-yellow-400/20">
                    <Clock className="w-3 h-3" /> Pending
                  </span>
                )}
              </div>

              {/* معلومات المستخدم الأساسية */}
              <div className="flex items-center gap-4 mb-5">
                <img 
                  src={user.avatar} 
                  alt={user.name} 
                  className="w-14 h-14 rounded-full border-2 border-[var(--color-surface-alt)] object-cover" 
                />
                <div>
                  <h4 className="text-[var(--color-text)] font-bold text-base">{user.name}</h4>
                  <p className="text-xs text-[var(--color-text-muted)] mt-0.5">Tourist ID: #{user.id}092</p>
                </div>
              </div>

              {/* تفاصيل التسجيل (خط فاصل خفيف بينهم) */}
              <div className="space-y-3 bg-[var(--color-surface)] p-4 rounded-xl border border-[var(--color-border)]">
                
                {/* تاريخ التسجيل */}
                <div className="flex justify-between items-center">
                  <span className="text-xs text-[var(--color-text-muted)] flex items-center gap-2">
                    <CalendarDays className="w-4 h-4 text-[var(--color-accent)]" /> Enroll Date
                  </span>
                  <span className="text-sm font-semibold text-[var(--color-text-muted)]">{user.enrollDate}</span>
                </div>

                <div className="w-full h-[1px] bg-[var(--color-surface-alt)]"></div>

                {/* عدد المرافقين */}
                <div className="flex justify-between items-center">
                  <span className="text-xs text-[var(--color-text-muted)] flex items-center gap-2">
                    <Users className="w-4 h-4 text-[var(--color-accent)]" /> Companions
                  </span>
                  <span className="text-sm font-semibold text-[var(--color-text-muted)]">
                    {user.companions > 0 ? `+${user.companions} Person` : "None"}
                  </span>
                </div>

                <div className="w-full h-[1px] bg-[var(--color-surface-alt)]"></div>

                {/* المبلغ المدفوع */}
                <div className="flex justify-between items-center">
                  <span className="text-xs text-[var(--color-text-muted)] flex items-center gap-2">
                    <Wallet className="w-4 h-4 text-[var(--color-accent-2)]" /> Amount Paid
                  </span>
                  <span className="text-base font-bold text-[var(--color-accent-2)]">
                    ${user.amountPaid.toLocaleString()}
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