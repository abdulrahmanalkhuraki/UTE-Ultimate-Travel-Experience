import  { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Users, Building2, Map, Wallet } from 'lucide-react';
import {
  LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid,
  Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import RejectDialog from '../components/RejectDialog';
import ApproveDialog from '../components/ApproveDialog';
import ProgramDetails from './ProgramDetails';
import PendingCompanyDetails from './PendingCompanyDetails';
import { useApiData } from '../hooks/useApiData';
import { useSyncedState } from '../hooks/useSyncedState';
import {
  getMainDashboard,
  getCompaniesDashboard,
  getTourPackagesByStatus,
  getPendingTourCompanies,
  approveTourPackage,
  rejectTourPackage,
  approveTourCompany,
  rejectTourCompany,
} from '../services/dashboardApi';
import { mapApiCompany, mapTourPackage } from '../utils/mappers';

export default function Home() {
  const { t } = useTranslation();
  const { data: dashboardData } = useApiData(getMainDashboard, []);
  const { data: companiesData } = useApiData(getCompaniesDashboard, []);

  // GET /api/Admin/dashboard -> touristGrowth
  const touristsGrowthChart = (dashboardData?.touristGrowth ?? []).map((g) => ({
    name: g.month,
    tourists: g.count,
  }));

  // دمج tourPackageGrowth (من /dashboard) مع companyGrowth (من /dashboard/companies) حسب الشهر
  const growthByMonth = {};
  (dashboardData?.tourPackageGrowth ?? []).forEach((g) => {
    growthByMonth[g.month] = { ...(growthByMonth[g.month] || { name: g.month }), name: g.month, program: g.count };
  });
  (companiesData?.companyGrowth ?? []).forEach((g) => {
    growthByMonth[g.month] = { ...(growthByMonth[g.month] || { name: g.month }), name: g.month, company: g.count };
  });
  const companyProgramChart = Object.values(growthByMonth);

  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [itemToReject, setItemToReject] = useState(null);
  const [itemToApprove, setItemToApprove] = useState(null);
  const [selectedProgram, setSelectedProgram] = useState(null);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [rejectTargetType, setRejectTargetType] = useState('program');
  const [approveTargetType, setApproveTargetType] = useState('program');
  const [actionError, setActionError] = useState('');

  const { data: unapprovedData } = useApiData(() => getTourPackagesByStatus(0, 1, 20), []);
  const [programRows, setProgramRows] = useSyncedState(unapprovedData, (d) =>
    (d?.items ?? []).map(mapTourPackage).map((p) => ({ name: p.title, trip: p.title, comp: p.company, program: p }))
  );

  // GET /api/TourCompany/pending -> شركات قيد المراجعة
  const { data: pendingCompaniesData } = useApiData(getPendingTourCompanies, []);
  const [companyRows, setCompanyRows] = useSyncedState(pendingCompaniesData, (d) =>
    (d ?? []).map(mapApiCompany).map((c) => ({ name: c.name, comp: c.name, since: c.founded, company: c }))
  );

  const handleRejectClick = (e, item, type) => {
    e.stopPropagation();
    setActionError('');
    setItemToReject(item);
    setRejectTargetType(type);
    setIsRejectDialogOpen(true);
  };

  const handleApproveClick = (e, item, type) => {
    e.stopPropagation();
    setActionError('');
    setItemToApprove(item);
    setApproveTargetType(type);
    setIsApproveDialogOpen(true);
  };

  const handleProgramRowClick = (row) => {
    setSelectedProgram(row.program || { title: row.trip, company: row.comp });
    setSelectedCompany(null);
  };

  const handleCompanyRowClick = (row) => {
    setSelectedCompany(row.company || { name: row.comp });
    setSelectedProgram(null);
  };

  // POST /api/TourPackage/:id/reject أو /api/TourCompany/:id/reject  body: { reason }
  const handleConfirmRejection = async (reason) => {
    if (!itemToReject) return;
    setActionError('');
    try {
      if (rejectTargetType === 'program') {
        await rejectTourPackage(itemToReject.program.id, reason);
        setProgramRows((prev) => prev.filter((item) => item.name !== itemToReject.name));
      } else {
        await rejectTourCompany(itemToReject.company.id, reason);
        setCompanyRows((prev) => prev.filter((item) => item.name !== itemToReject.name));
        if (selectedCompany?.id === itemToReject.company.id) setSelectedCompany(null);
      }
      setIsRejectDialogOpen(false);
      setItemToReject(null);
      setRejectTargetType('program');
    } catch (err) {
      setIsRejectDialogOpen(false);
      setActionError(err.message || 'Failed to reject.');
    }
  };

  // POST /api/TourPackage/:id/approve أو /api/TourCompany/:id/approve
  const handleConfirmApproval = async () => {
    if (!itemToApprove) return;
    setActionError('');
    try {
      if (approveTargetType === 'program') {
        await approveTourPackage(itemToApprove.program.id);
        setProgramRows((prev) => prev.filter((item) => item.name !== itemToApprove.name));
      } else {
        await approveTourCompany(itemToApprove.company.id);
        setCompanyRows((prev) => prev.filter((item) => item.name !== itemToApprove.name));
        if (selectedCompany?.id === itemToApprove.company.id) setSelectedCompany(null);
      }
    } catch (err) {
      setIsApproveDialogOpen(false);
      setActionError(err.message || 'Failed to approve.');
      return;
    }
    setIsApproveDialogOpen(false);
    setItemToApprove(null);
    setApproveTargetType('program');
  };


  if (selectedProgram) {
    return <ProgramDetails program={selectedProgram} onBack={() => setSelectedProgram(null)} />;
  }

  if (selectedCompany) {
    return (
      <>
        <PendingCompanyDetails
          company={selectedCompany}
          onBack={() => setSelectedCompany(null)}
          onApprove={() => {
            setActionError('');
            setItemToApprove({ name: selectedCompany.name, company: selectedCompany });
            setApproveTargetType('company');
            setIsApproveDialogOpen(true);
          }}
          onReject={() => {
            setActionError('');
            setItemToReject({ name: selectedCompany.name, company: selectedCompany });
            setRejectTargetType('company');
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
    <div className="p-8 space-y-6">

      {/* البطاقات العلوية (Stats Cards) */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {[
          { title: t('home.totalTourists'), value: dashboardData ? dashboardData.activeTourists.toLocaleString() : '—', icon: Users },
          { title: t('home.activeCompanies'), value: dashboardData ? dashboardData.activeCompanies.toLocaleString() : '—', icon: Building2 },
          { title: t('home.activeTourPackages'), value: dashboardData ? dashboardData.tourPackages.active.toLocaleString() : '—', icon: Map },
          { title: t('home.totalRevenue'), value: dashboardData ? `$${dashboardData.totalRevenue.toLocaleString()}` : '—', icon: Wallet, highlight: true },
        ].map((stat, idx) => (
          <div key={idx} className="bg-[var(--color-surface)] p-6 rounded-xl border border-[var(--color-border)] relative overflow-hidden shadow-[var(--shadow-card)] group cursor-pointer hover:border-[var(--color-accent)]/60 transition-all">
            <div className="flex justify-between items-start">
              <div>
                <p className="text-xs text-[var(--color-text-muted)] font-semibold tracking-wider mb-2">{stat.title}</p>
                <h3 className={`text-4xl font-bold ${stat.highlight ? 'text-[var(--color-accent-2)]' : 'text-[var(--color-text)]'}`}>{stat.value}</h3>
              </div>
              <div className="bg-[var(--color-surface-alt)] p-3 rounded-lg">
                <stat.icon className="w-6 h-6 text-[var(--color-accent)]" />
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* قسم المخططات البيانية (Charts) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 h-[400px]">
        {/* المخطط الخطي */}
        <div className="bg-[var(--color-surface)] p-6 rounded-xl border border-[var(--color-border)] flex flex-col">
          <div className="text-center mb-4">
            <h3 className="text-lg font-semibold text-[var(--color-text)]">{t('home.touristsGrowth')}</h3>
            <p className="text-sm text-[var(--color-text-muted)]">{t('home.past12Months')}</p>
          </div>
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={touristsGrowthChart}>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
              <XAxis dataKey="name" stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
              <YAxis stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
              <Tooltip contentStyle={{ backgroundColor: 'var(--color-surface)', borderColor: 'var(--color-accent)', border: '1px solid var(--color-accent)', borderRadius: '0.8rem' }} />
              <Legend iconType="plainline" />
              <Line type="monotone" dataKey="tourists" stroke="var(--color-accent)" strokeWidth={3} dot={false} name={t('nav.tourists')} />
            </LineChart>
          </ResponsiveContainer>
        </div>

        {/* المخطط الشريطي */}
        <div className="bg-[var(--color-surface)] p-6 rounded-xl border border-[var(--color-border)] flex flex-col">
          <div className="text-center mb-4">
            <h3 className="text-lg font-semibold text-[var(--color-text)]">{t('home.companiesAndProgramGrowth')}</h3>
          </div>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={companyProgramChart}>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
              <XAxis dataKey="name" stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
              <YAxis stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
              <Tooltip contentStyle={{ backgroundColor: 'var(--color-surface)', borderColor: 'var(--color-accent-2)', border: '1px solid var(--color-accent-2)', borderRadius: '0.8rem' }} cursor={{ fill: 'var(--color-surface-alt)' }} />
              <Legend iconType="rect" />
              <Bar dataKey="company" fill="var(--color-accent)" name={t('nav.companies')} radius={[5, 2, 0, 0]} />
              <Bar dataKey="program" fill="var(--color-accent-2)" name={t('nav.tourPackages')} radius={[2, 5, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {actionError && (
        <div className="rounded-xl border border-[var(--color-danger)]/30 bg-[var(--color-danger-soft)] p-3 text-sm text-[var(--color-danger)]">
          {actionError}
        </div>
      )}

      {/* الجداول السفلية (Tables) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* جدول الرحلات */}
        <div className="bg-[var(--color-surface)] p-6 rounded-xl border border-[var(--color-border)]">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-semibold text-[var(--color-text)]">{t('home.pendingPrograms')}</h3>
            <div className="p-2 rounded-lg bg-[var(--color-surface-alt)] text-[var(--color-accent)]">
              <Map className="w-5 h-5" />
            </div>
          </div>
          <div className="min-h-[220px] overflow-x-auto">
            <table className="w-full text-sm text-start">
              <thead className="text-xs text-[var(--color-text-muted)] uppercase border-b border-[var(--color-border)]">
                <tr>
                  <th className="px-4 py-3 text-start">{t('home.tripName')}</th>
                  <th className="px-4 py-3 text-start">{t('home.companyName')}</th>
                  <th className="px-4 py-3 text-end">{t('common.actions')}</th>
                </tr>
              </thead>
              <tbody>
                {programRows.length > 0 ? (
                  programRows.map((row, idx) => (
                    <tr key={idx} onClick={() => handleProgramRowClick(row)} className="cursor-pointer border-b border-[var(--color-border)] hover:bg-[var(--color-surface-alt)] transition-colors">
                      <td className="px-4 py-4 font-medium text-[var(--color-text)]">{row.trip}</td>
                      <td className="px-4 py-4 text-[var(--color-text-muted)]">{row.comp}</td>
                      <td className="px-4 py-4 flex justify-end gap-2">
                        <button
                        onClick={(e) => handleRejectClick(e, row, 'program')}
                        className="px-4 py-2 bg-[var(--color-surface-alt)] text-[var(--color-text-muted)] rounded hover:opacity-80 transition">{t('common.reject')}</button>
                        <button
                          onClick={(e) => handleApproveClick(e, row, 'program')}
                          className="px-4 py-2 bg-[var(--color-accent)] text-white font-medium rounded hover:opacity-90 transition">{t('common.approve')}</button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="3" className="py-16 text-center text-[var(--color-text-muted)]">
                      {t('home.noPendingPrograms')}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* جدول الشركات */}
        <div className="bg-[var(--color-surface)] p-6 rounded-xl border border-[var(--color-border)]">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-semibold text-[var(--color-text)]">{t('home.pendingCompanies')}</h3>
            <div className="p-2 rounded-lg bg-[var(--color-surface-alt)] text-[var(--color-accent-2)]">
              <Building2 className="w-5 h-5" />
            </div>
          </div>
          <div className="min-h-[220px] overflow-x-auto">
            <table className="w-full text-sm text-start">
              <thead className="text-xs text-[var(--color-text-muted)] uppercase border-b border-[var(--color-border)]">
                <tr>
                  <th className="px-4 py-3 text-start">{t('home.companyName')}</th>
                  <th className="px-4 py-3 text-start">{t('home.since')}</th>
                  <th className="px-4 py-3 text-end">{t('common.actions')}</th>
                </tr>
              </thead>
              <tbody>
                {companyRows.length > 0 ? (
                  companyRows.map((row, idx) => (
                    <tr key={idx} onClick={() => handleCompanyRowClick(row)} className="cursor-pointer border-b border-[var(--color-border)] hover:bg-[var(--color-surface-alt)] transition-colors">
                      <td className="px-4 py-4 font-medium text-[var(--color-text)]">{row.comp}</td>
                      <td className="px-4 py-4">
                        <span className="text-[var(--color-accent-2)] text-xs font-semibold">{row.since}</span>
                      </td>
                      <td className="px-4 py-4 flex justify-end gap-2">
                        <button
                          onClick={(e) => handleRejectClick(e, row, 'company')}
                          className="px-4 py-2 bg-[var(--color-surface-alt)] text-[var(--color-text-muted)] rounded hover:opacity-80 transition">{t('common.reject')}</button>
                        <button
                          onClick={(e) => handleApproveClick(e, row, 'company')}
                          className="px-4 py-2 bg-[var(--color-accent)] text-white font-medium rounded hover:opacity-90 transition">{t('common.approve')}</button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="3" className="py-16 text-center text-[var(--color-text-muted)]">
                      {t('home.noPendingCompanies')}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <RejectDialog
        isOpen={isRejectDialogOpen}
        onClose={() => {
          setIsRejectDialogOpen(false);
          setItemToReject(null);
        }}
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