import {
  ArrowLeft, Building2, User, Calendar, MapPin, Phone, Mail,
  FileText, CreditCard, Fingerprint, Image as ImageIcon, CheckCircle, XCircle
} from 'lucide-react';
import { useApiData } from '../hooks/useApiData';
import { getUserById } from '../services/dashboardApi';
import { formatDate } from '../utils/format';

// 1. مكون مساعد لعرض المعلومة سطر بسطر مع الأيقونة (حسب طلبك: الأيقونة، الاسم : المعلومة)
const InfoItem = ({ icon: Icon, label, value }) => (
  <div className="flex items-start gap-3 py-3 border-b border-[var(--color-border)] last:border-0 hover:bg-[var(--color-surface-alt)] rounded-lg px-2 transition-colors duration-200">
    <div className="mt-0.5">
      <Icon className="w-4 h-4 text-[var(--color-accent)]" />
    </div>
    <div className="flex-1 text-sm">
      <span className="text-[var(--color-text-muted)] font-medium">{label} : </span>
      <span className="text-[var(--color-text)] ml-1 font-semibold">{value}</span>
    </div>
  </div>
);

// 2. مكون مساعد للبطاقة "الطافية" (التي تجمع كل كم شغلة مع بعض)
const GlassCard = ({ title, icon: TitleIcon, children, borderColor = "border-[var(--color-border)]/20" }) => (
  <div className={`bg-[var(--color-surface-alt)]/80 backdrop-blur-md border ${borderColor} rounded-2xl p-5 shadow-[0_8px_30px_rgb(0,0,0,0.15)] hover:shadow-[0_8px_30px_rgb(0,0,0,0.25)] transition-all duration-300`}>
    <div className="flex items-center gap-3 mb-4 pb-3 border-b border-[var(--color-border)]/50">
      <div className="p-2 bg-[var(--color-surface-alt)] rounded-lg">
        <TitleIcon className="w-4 h-4 text-[var(--color-text)]" />
      </div>
      <h3 className="text-base font-bold text-[var(--color-text)]">{title}</h3>
    </div>
    <div className="flex flex-col">
      {children}
    </div>
  </div>
);

export default function PendingCompanyDetails({ company, onBack, onApprove, onReject }) {
  // GET /api/User/:id -> بيانات مالك الشركة (المتاحة عبر company.userId فقط)
  const { data: owner } = useApiData(
    () => (company?.userId ? getUserById(company.userId) : Promise.resolve(null)),
    [company?.userId]
  );

  return (
    <div className="p-8 space-y-6 font-sans animate-in fade-in zoom-in-95 duration-300">
      
      {/* الترويسة */}
      <div className="flex items-center justify-between border-b border-[var(--color-border)] pb-4">
        <div className="flex items-center gap-4">
          <button 
            onClick={onBack}
            className="p-2 bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-lg hover:bg-[var(--color-surface-alt)] transition group"
          >
            <ArrowLeft className="w-5 h-5 text-[var(--color-text-muted)] group-hover:text-[var(--color-text)]" />
          </button>
          <div className="w-12 h-12 rounded-xl overflow-hidden border-2 border-[var(--color-border)]/50 shadow-lg">
            <img src={company?.logo || "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=150&q=80"} alt="Logo" className="w-full h-full object-cover" />
          </div>
          <div>
            <h2 className="text-2xl font-bold text-[var(--color-text)]">{company?.name || "Alpine Adventures"}</h2>
            <p className="text-sm text-[var(--color-accent-2)] flex items-center gap-1.5 mt-0.5">
              <span className="relative flex h-2 w-2">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-[var(--color-accent-2)] opacity-75"></span>
                <span className="relative inline-flex rounded-full h-2 w-2 bg-[var(--color-accent-2)]"></span>
              </span>
              Pending Review
            </p>
          </div>
        </div>
      </div>

      {/* المحتوى المقسم لعمودين (يمين للشركة، يسار للمالك أو العكس) */}
      <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
        
        {/* === القسم الأول: معلومات الشركة === */}
        <div className="space-y-6">
          <h2 className="text-lg font-bold text-[var(--color-accent)] px-2 flex items-center gap-2">
            <Building2 className="w-5 h-5" /> Company Details
          </h2>
          
          <GlassCard title="Identity & Description" icon={Building2} borderColor="border-[var(--color-accent)]/30">
            <InfoItem icon={Building2} label="Trade Name" value={company?.name || '—'} />
            <InfoItem icon={FileText} label="Description" value={company?.description || company?.about || '—'} />
            <InfoItem icon={Calendar} label="Founded Date" value={company?.foundingDate ? formatDate(company.foundingDate) : '—'} />
          </GlassCard>

          <GlassCard title="Contact & Location" icon={MapPin} borderColor="border-[var(--color-accent)]/30">
            <InfoItem icon={MapPin} label="Location" value={company?.location || '—'} />
            <InfoItem icon={Phone} label="Phone Number" value={company?.phoneNumber || '—'} />
            <InfoItem icon={Mail} label="Email" value={company?.email || '—'} />
          </GlassCard>

          <GlassCard title="Legal & Financial" icon={FileText} borderColor="border-[var(--color-accent)]/30">
            <InfoItem icon={FileText} label="Tourism Record Number" value={company?.tourismLicenseNumber || '—'} />
            <InfoItem icon={CreditCard} label="Bank Account" value={company?.bankAccount || '—'} />

            {/* صورة السجل */}
            <div className="mt-4 p-3 bg-[var(--color-app-bg)] rounded-xl border border-[var(--color-border)]">
              <span className="text-xs text-[var(--color-text-muted)] mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> Tourism Record Image</span>
              {company?.tourismLicenseImage ? (
                <div className="w-full h-24 rounded-lg overflow-hidden border border-[var(--color-border)]">
                  <img src={company.tourismLicenseImage} alt="Tourism Record" className="w-full h-full object-cover" />
                </div>
              ) : (
                <div className="w-full h-24 border-2 border-dashed border-[var(--color-border)] rounded-lg flex items-center justify-center text-[var(--color-text-muted)] bg-[var(--color-surface-alt)]">
                   <ImageIcon className="w-6 h-6" />
                </div>
              )}
            </div>
          </GlassCard>
        </div>

        {/* === القسم الثاني: معلومات المالك === */}
        <div className="space-y-6">
          <h2 className="text-lg font-bold text-[var(--color-accent-2)] px-2 flex items-center gap-2">
            <User className="w-5 h-5" /> Owner Details
          </h2>

          <GlassCard title="Personal Information" icon={User} borderColor="border-[var(--color-accent-2)]/30">
            <InfoItem icon={User} label="First Name" value={owner?.firstName || '—'} />
            <InfoItem icon={User} label="Last Name" value={owner?.lastName || '—'} />
            <InfoItem icon={User} label="Gender" value={owner?.gender || '—'} />
            <InfoItem icon={Calendar} label="Birth Date" value={owner?.dateOfBirth ? formatDate(owner.dateOfBirth) : '—'} />
            <InfoItem icon={Fingerprint} label="National ID" value={owner?.nationalNumber || '—'} />
          </GlassCard>

          <GlassCard title="Contact & Financial" icon={Phone} borderColor="border-[var(--color-accent-2)]/30">
            <InfoItem icon={Phone} label="Phone Number" value={owner?.phone || '—'} />
            <InfoItem icon={MapPin} label="Residence" value={owner?.placeOfResidence || owner?.currentLocation || '—'} />
            <InfoItem icon={CreditCard} label="Bank Account" value={owner?.bankAccount || '—'} />
          </GlassCard>

          <GlassCard title="Identity Document" icon={Fingerprint} borderColor="border-[var(--color-accent-2)]/30">
             {/* صورة الهوية */}
             <div className="p-3 bg-[var(--color-app-bg)] rounded-xl border border-[var(--color-border)]">
              <span className="text-xs text-[var(--color-text-muted)] mb-2 flex items-center gap-1.5"><ImageIcon className="w-3.5 h-3.5" /> ID Image</span>
              {owner?.nationalIdImage ? (
                <div className="w-full h-24 rounded-lg overflow-hidden border border-[var(--color-border)]">
                  <img src={owner.nationalIdImage} alt="National ID" className="w-full h-full object-cover" />
                </div>
              ) : (
                <div className="w-full h-24 border-2 border-dashed border-[var(--color-border)] rounded-lg flex items-center justify-center text-[var(--color-text-muted)] bg-[var(--color-surface-alt)]">
                   <ImageIcon className="w-6 h-6" />
                </div>
              )}
            </div>
          </GlassCard>
        </div>
      </div>

      {/* أزرار الإجراءات (قبول / رفض) */}
      <div className="flex items-center gap-4 pt-6 border-t border-[var(--color-border)] mt-8">
        <button 
          onClick={onReject}
          className="flex-1 flex justify-center items-center gap-2 bg-[var(--color-surface-alt)] text-red-400 py-3.5 rounded-xl font-bold hover:bg-red-500/10 hover:border-red-500/30 border border-transparent transition duration-300"
        >
          <XCircle className="w-5 h-5" /> Reject Application
        </button>
        <button 
          onClick={onApprove}
          className="flex-1 flex justify-center items-center gap-2 bg-[var(--color-accent)] text-[var(--color-on-accent)] py-3.5 rounded-xl font-bold hover:bg-[var(--color-accent)] shadow-[0_0_20px_rgba(145,179,250,0.2)] transition duration-300"
        >
          <CheckCircle className="w-5 h-5" /> Approve Application
        </button>
      </div>

    </div>
  );
}