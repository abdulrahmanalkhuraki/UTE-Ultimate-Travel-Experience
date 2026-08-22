import CompanyDetails from './CompanyDetails';
import RejectDialog from '../components/RejectDialog';
import ApproveDialog from '../components/ApproveDialog';
import PendingCompanyDetails from './PendingCompanyDetails';
import  { useState } from 'react';
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import {
  Building2, Calendar, MapPin, Map, Hourglass, ChevronDown
} from 'lucide-react';
import { useApiData } from '../hooks/useApiData';
import { useSyncedState } from '../hooks/useSyncedState';
import { useCompanyProgramCounts } from '../hooks/useCompanyProgramCounts';
import {
  getCompaniesDashboard,
  getTourCompanies,
  getPendingTourCompanies,
  approveTourCompany,
  rejectTourCompany,
} from '../services/dashboardApi';
import { mapApiCompany } from '../utils/mappers';

export default function Companies() {
  const [isCurrentExpanded, setIsCurrentExpanded] = useState(true);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [selectedPendingCompany, setSelectedPendingCompany] = useState(null);
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [itemToReject, setItemToReject] = useState(null);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [itemToApprove, setItemToApprove] = useState(null);
  const [actionError, setActionError] = useState('');
  
  const { data: companiesData } = useApiData(getCompaniesDashboard, []);
  const { data: allCompaniesData } = useApiData(getTourCompanies, []);
  const { data: pendingCompaniesData } = useApiData(getPendingTourCompanies, []);

  // نسخة محلية قابلة للتعديل من قائمة الشركات المعلّقة
  const [pendingCompanies, setPendingCompanies] = useSyncedState(
    pendingCompaniesData,
    (d) => (d ?? []).map(mapApiCompany)
  );

  // GET /api/Admin/dashboard/companies -> companyGrowth
  const chartData = (companiesData?.companyGrowth ?? []).map((g) => ({
    name: g.month,
    count: g.count,
  }));

  // GET /api/TourCompany -> Status بالباك: Pending/Approved/Rejected
  const allCompanies = (allCompaniesData ?? []).map(mapApiCompany);
  const approvedCompanies = allCompanies.filter((c) => c.status === 'Approved');

  // GET /api/Admin/dashboard/companies/:companyId -> totalTourPackages
  const approvedProgramCounts = useCompanyProgramCounts(approvedCompanies.map((c) => c.id));
  const pendingProgramCounts = useCompanyProgramCounts(pendingCompanies.map((c) => c.id));
  
  const withProgramCount = (list, counts) =>
    list.map((c) => ({ ...c, programs: counts[c.id] ?? c.programs }));

  const currentCompanies = withProgramCount(approvedCompanies, approvedProgramCounts);
  const pendingCompaniesDisplay = withProgramCount(pendingCompanies, pendingProgramCounts);

  const handleRejectClick = (e, company) => {
    e.stopPropagation();
    setActionError('');
    setItemToReject(company);
    setIsRejectDialogOpen(true);
  };

  const handleConfirmRejection = async (reason) => {
    if (!itemToReject) return;
    setActionError('');
    try {
      await rejectTourCompany(itemToReject.id, reason);
      setPendingCompanies((prev) => prev.filter((company) => company.id !== itemToReject.id));
      if (selectedPendingCompany?.id === itemToReject.id) {
        setSelectedPendingCompany(null);
      }
      setIsRejectDialogOpen(false);
      setItemToReject(null);
    } catch (err) {
      setIsRejectDialogOpen(false);
      setActionError(err.message || 'Failed to reject the company.');
    }
  };

  const handleApproveClick = (e, company) => {
    e.stopPropagation();
    setActionError('');
    setItemToApprove(company);
    setIsApproveDialogOpen(true);
  };

  const handleConfirmApproval = async () => {
    if (!itemToApprove) return;
    setActionError('');
    try {
      await approveTourCompany(itemToApprove.id);
      setPendingCompanies((prev) => prev.filter((company) => company.id !== itemToApprove.id));
      if (selectedPendingCompany?.id === itemToApprove.id) {
        setSelectedPendingCompany(null);
      }
      setIsApproveDialogOpen(false);
      setItemToApprove(null);
    } catch (err) {
      setIsApproveDialogOpen(false);
      setActionError(err.message || 'Failed to approve the company.');
    }
  };

  const CompanyCard = ({ company }) => (
    <div onClick={() => setSelectedCompany(company)}
    className="flex justify-between items-center bg-[var(--color-surface-alt)] border border-[var(--color-border)] p-5 rounded-2xl transition hover:border-[var(--color-border)]/50 shadow-md cursor-pointer">
      
      <div className="flex items-center gap-5 text-right flex-1 justify-end">
        <div className="space-y-2">
          <h4 className="text-base font-bold text-[var(--color-text)]">{company.name}</h4>
          
          <div className="flex items-center justify-end gap-4 text-xs text-[var(--color-text-muted)]">
            <span className="flex items-center gap-1.5"><Calendar className="w-3.5 h-3.5" /> Founded: {company.founded}</span>
            <span>•</span>
            <span className="flex items-center gap-1.5 text-[var(--color-accent)]"><MapPin className="w-3.5 h-3.5" /> {company.location}</span>
          </div>
          
          <div className="flex items-center justify-end gap-1.5 text-xs text-[var(--color-text-muted)]">
            <Map className="w-3.5 h-3.5" /> Published Programs: {company.programs}
          </div>
        </div>
        
        <div className="w-16 h-16 rounded-xl overflow-hidden border-2 border-[var(--color-border)] flex-shrink-0">
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
      <>
        <PendingCompanyDetails
          company={selectedPendingCompany}
          onBack={() => setSelectedPendingCompany(null)}
          onApprove={() => {
            setActionError('');
            setItemToApprove(selectedPendingCompany);
            setIsApproveDialogOpen(true);
          }}
          onReject={() => {
            setActionError('');
            setItemToReject(selectedPendingCompany);
            setIsRejectDialogOpen(true);
          }}
        />
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
      </>
    );
  }

  return (
    <div className="p-8 space-y-8 font-sans">
      
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        
        {/* === القسم الأيسر (المخطط والقوائم المتمددة) (يأخذ 7 من 12 من العرض) === */}
        <div className="lg:col-span-7 space-y-6">
          
          {/* المخطط والإحصائيات */}
          <div className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 p-6 rounded-2xl shadow-lg">
            <h3 className="text-lg font-semibold text-[var(--color-text)] mb-4 text-left">Company Growth Over Time</h3>
            <div className="w-full h-64 mb-6">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
                  <XAxis dataKey="name" stroke="var(--color-text-muted)" tick={{fill: 'var(--color-text-muted)', fontSize: 12}}  />
                  <YAxis stroke="var(--color-text-muted)" tick={{fill: 'var(--color-text-muted)', fontSize: 12}} />
                  <Tooltip contentStyle={{backgroundColor: 'var(--color-surface)', borderColor: 'var(--color-accent-2)', border: '1px solid var(--color-accent-2)', borderRadius: '0.8rem'}} />
                  <Legend iconType="plainline" />
                  <Line type="monotone" dataKey="count" name="Company Growth" stroke="var(--color-accent-2)" strokeWidth={3} dot={false} />
                </LineChart>
              </ResponsiveContainer>
            </div>

            {/* الإحصائيات أسفل المخطط */}
            <div className="grid grid-cols-2 gap-4 pt-6 border-t border-[var(--color-border)]">
              <div className="flex items-center gap-4 bg-[var(--color-app-bg)] p-4 rounded-xl border border-[var(--color-border)]">
                <Building2 className="w-8 h-8 text-[var(--color-accent)]" />
                <div>
                  <p className="text-[10px] text-[var(--color-text-muted)] font-bold uppercase tracking-wider">Active Companies</p>
                  <p className="text-xl font-bold text-[var(--color-text)] mt-0.5">{companiesData ? companiesData.activeCompanies.toLocaleString() : '—'}</p>
                </div>
              </div>
              
              <div className="flex items-center gap-4 bg-[var(--color-app-bg)] p-4 rounded-xl border border-[var(--color-border)]">
                <Hourglass className="w-8 h-8 text-[var(--color-accent-2)]" />
                <div>
                  <p className="text-[10px] text-[var(--color-text-muted)] font-bold uppercase tracking-wider">Pending Companies</p>
                  <p className="text-xl font-bold text-[var(--color-accent-2)] mt-0.5">{companiesData ? companiesData.pendingCompanies.toLocaleString() : pendingCompanies.length}</p>
                </div>
              </div>
            </div>
          </div>

          {/* قائمة الشركات الحالية (المتمددة) */}
          <div className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
            <button 
              onClick={() => setIsCurrentExpanded(!isCurrentExpanded)}
              className="w-full p-5 flex justify-between items-center bg-[var(--color-surface-alt)] hover:bg-[var(--color-surface-alt)] transition"
            >
              <div className="flex items-center gap-4">
              <Building2 className="w-8 h-8 text-[var(--color-accent-2)] ml-5" />
              <h3 className="text-base text-lg font-semibold text-[var(--color-accent-2)] ml-10">Current Companies</h3>
              </div>
              <ChevronDown className={`w-5 h-5 text-[var(--color-text-muted)] mr-10 transition-transform duration-300 ${isCurrentExpanded ? 'rotate-180' : ''}`} />
              
            </button>
            {isCurrentExpanded && (
              <div className="p-5 space-y-4 max-h-[400px] overflow-y-auto custom-scrollbar border-t border-[var(--color-border)]">
                {currentCompanies.map(company => (
                  <CompanyCard key={company.id} company={company} />
                ))}
              </div>
            )}
          </div>

        </div>

        {/* === القسم الأيمن (طلبات الشركات المعلقة) (يأخذ 5 من 12 من العرض) === */}
        <div className="lg:col-span-5">
          <div className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-2xl  shadow-lg p-6 h-full flex flex-col">
            <h3 className="text-lg font-semibold justify-content text-[var(--color-text)] mb-6 ml-5 text-left border-b border-[var(--color-border)] pb-4">
              Pending Company Applications
            </h3>
            {actionError && (
              <div className="mb-4 rounded-xl border border-red-500/30 bg-red-500/10 p-3 text-sm text-red-400">
                {actionError}
              </div>
            )}

            <div className="space-y-5 overflow-y-auto flex-1 pr-2 custom-scrollbar">
              {pendingCompaniesDisplay.length === 0 ? (
                <p className="text-[var(--color-text-muted)] text-center text-xl mt-10">No pending companies.</p>
              ) : (
              pendingCompaniesDisplay.map(company => (
                <div key={company.id} 
                onClick={() => setSelectedPendingCompany(company)}
                className="bg-[var(--color-surface-alt)] border border-[var(--color-border)] p-5 rounded-2xl cursor-pointer hover:border-[var(--color-accent)]/50 transition-colors shadow-md"
                >
                  {/* معلومات الشركة */}
                  <div className="flex items-center gap-4 text-right justify-end mb-5">
                    <div className="space-y-1">
                      <h4 className="text-base font-bold text-[var(--color-text)]">{company.name}</h4>
                      <p className="text-xs text-[var(--color-text-muted)] flex items-center justify-end gap-2">
                        <Calendar className="w-3.5 h-3.5" /> Founded: {company.founded}
                      </p>
                      <p className="text-xs text-[var(--color-accent)] flex items-center justify-end gap-2">
                        <MapPin className="w-3.5 h-3.5" /> {company.location}
                      </p>
                      <p className="text-xs text-[var(--color-text-muted)] flex items-center justify-end gap-2">
                        <Map className="w-3.5 h-3.5" /> Programs Prepared: {company.programs}
                      </p>
                    </div>
                    <div className="w-16 h-16 rounded-xl overflow-hidden border-2 border-[var(--color-border)]">
                      <img src={company.logo} alt={company.name} className="w-full h-full object-cover" />
                    </div>
                  </div>

                  {/* أزرار القبول والرفض */}
                  <div className="flex gap-3 mt-2">
                    <button 
                    onClick={(e) => handleRejectClick(e, company)}
                    className="flex-1 bg-[var(--color-surface-alt)] text-[var(--color-text-muted)] py-2.5 rounded-xl font-medium hover:bg-[var(--color-border)] transition duration-200">
                      Reject
                    </button>
                    <button
                    onClick={(e) => handleApproveClick(e, company)}
                    className="flex-1 bg-[var(--color-accent)] text-[var(--color-on-accent)] py-2.5 rounded-xl font-medium hover:bg-[var(--color-accent)] shadow-[0_0_15px_rgba(145,179,250,0.15)] transition duration-200">
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