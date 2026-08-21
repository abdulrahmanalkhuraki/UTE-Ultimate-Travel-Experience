import  { useState } from 'react';
import { Users, Building2, Map, Wallet } from 'lucide-react';
import {
  LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid,
  Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';
import ProgramDetails from './programDetailes';
import PendingCompanyDetails from './PendingCompanyDetails';
import { useApiData } from './hooks/useApiData';
import { useSyncedState } from './hooks/useSyncedState';
import {
  getMainDashboard,
  getCompaniesDashboard,
  getTourPackagesByStatus,
  getPendingTourCompanies,
  approveTourPackage,
  rejectTourPackage,
  approveTourCompany,
  rejectTourCompany,
} from './services/dashboardApi';
import { mapApiCompany, mapTourPackage } from './utils/mappers';

export default function Home() {
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
          { title: 'TOTAL TOURISTS', value: dashboardData ? dashboardData.activeTourists.toLocaleString() : '—', icon: Users, color: '#91B3FA' },
          { title: 'ACTIVE COMPANIES', value: dashboardData ? dashboardData.activeCompanies.toLocaleString() : '—', icon: Building2, color: '#91B3FA' },
          { title: 'ACTIVE TOUR PACKAGES', value: dashboardData ? dashboardData.tourPackages.active.toLocaleString() : '—', icon: Map, color: '#91B3FA' },
          { title: 'TOTAL REVENUE', value: dashboardData ? `$${dashboardData.totalRevenue.toLocaleString()}` : '—', icon: Wallet, color: '#F4A261', highlight: true },
        ].map((stat, idx) => (
          <div key={idx} className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 relative overflow-hidden shadow-lg group cursor-pointer hover:border-[#D4AF37]/70 transition-all">
            <div className="flex justify-between items-start">
              <div>
                <p className="text-xs text-gray-400 font-semibold tracking-wider mb-2">{stat.title}</p>
                <h3 className={`text-4xl font-bold ${stat.highlight ? 'text-[#F4A261]' : 'text-white'}`}>{stat.value}</h3>
              </div>
              <div className="bg-[#2A2A2D] p-3 rounded-lg">
                <stat.icon className="w-6 h-6" style={{ color: stat.color }} />
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* قسم المخططات البيانية (Charts) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 h-[400px]">
        {/* المخطط الخطي */}
        <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 flex flex-col">
          <div className="text-center mb-4">
            <h3 className="text-lg font-semibold">Tourists Growth Over Time</h3>
            <p className="text-sm text-gray-400">(past 12 months)</p>
          </div>
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={touristsGrowthChart}>
              <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
              <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
              <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
              <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#91B3FA', border: '1px solid #91B3FA', borderRadius: '0.8rem'}} />
              <Legend iconType="plainline" />
              <Line type="monotone" dataKey="tourists" stroke="#91B3FA" strokeWidth={3} dot={false} name="Tourists" />
              {/* <Line type="monotone" dataKey="international" stroke="#F4A261" strokeWidth={3} dot={false} name="International" /> */}
            </LineChart>
          </ResponsiveContainer>
        </div>

        {/* المخطط الشريطي */}
        <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30 flex flex-col">
          <div className="text-center mb-4">
            <h3 className="text-lg font-semibold">Companies and Program Growth</h3>
          </div>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={companyProgramChart}>
              <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
              <XAxis dataKey="name" stroke="#666" tick={{fill: '#888', fontSize: 12}} />
              <YAxis stroke="#666" tick={{fill: '#888', fontSize: 12}} />
              <Tooltip contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#F4A261', border: '1px solid #F4A261', borderRadius: '0.8rem'}} cursor={{fill: 'rgba(255,255,255,0.05)'}} />
              <Legend iconType="rect" />
              <Bar dataKey="company" fill="#91B3FA" name="Company" radius={[5, 2, 0, 0]} />
              <Bar dataKey="program" fill="#F4A261" name="Program Growth" radius={[2, 5, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {actionError && (
        <div className="rounded-xl border border-red-500/30 bg-red-500/10 p-3 text-sm text-red-400">
          {actionError}
        </div>
      )}

      {/* الجداول السفلية (Tables) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* جدول الرحلات */}
        <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-semibold">Pending programs</h3>
            <div className="p-2 rounded-lg bg-[#2A2A2D] text-[#91B3FA]">
              <Map className="w-5 h-5" />
            </div>
          </div>
          <div className="min-h-[220px]">
            <table className="w-full text-sm text-left">
              <thead className="text-xs text-gray-400 uppercase border-b border-[#ffff]">
                <tr>
                  <th className="px-4 py-3">TRIP NAME</th>
                  <th className="px-4 py-3">COMPANY NAME</th>
                  <th className="px-20 py-3 text-right">ACTIONS</th>
                </tr>
              </thead>
              <tbody>
                {programRows.length > 0 ? (
                  programRows.map((row, idx) => (
                    <tr key={idx} onClick={() => handleProgramRowClick(row)} className="cursor-pointer border-b border-[#222] border-radius-[0.5rem] hover:bg-[#252528] transition-colors ">
                      <td className="px-4 py-4 font-medium text-white">{row.trip}</td>
                      <td className="px-4 py-4 text-gray-300">{row.comp}</td>
                      <td className="px-4 py-4 flex justify-end gap-2">
                        <button 
                        onClick={(e) => handleRejectClick(e, row, 'program')}
                        className="px-4 py-2 bg-[#2A2A2D] text-gray-300 rounded hover:bg-[#333] transition">Reject</button>
                        <button 
                          onClick={(e) => handleApproveClick(e, row, 'program')}
                          className="px-4 py-2 bg-[#91B3FA] text-black font-medium rounded hover:bg-[#7fa1e8] transition">Approve</button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="3" className="py-16 text-center text-gray-400">
                      No pending programs currently.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* جدول الشركات */}
        <div className="bg-[#1C1C1E] p-6 rounded-xl border border-[#D4AF37]/30">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-xl font-semibold">Pending Company Applications</h3>
            <div className="p-2 rounded-lg bg-[#2A2A2D] text-[#F4A261]">
              <Building2 className="w-5 h-5" />
            </div>
          </div>
          <div className="min-h-[220px]">
            <table className="w-full text-sm text-left">
              <thead className="text-xs text-gray-400 uppercase border-b border-[#ffff]">
                <tr>
                  <th className="px-4 py-3">COMPANY NAME</th>
                  <th className="px-4 py-3">SINCE</th>
                  <th className="px-4 py-3 text-right">ACTIONS</th>
                </tr>
              </thead>
              <tbody>
                {companyRows.length > 0 ? (
                  companyRows.map((row, idx) => (
                    <tr key={idx} onClick={() => handleCompanyRowClick(row)} className="cursor-pointer border-b border-[#222] hover:bg-[#252528] transition-colors">
                      <td className="px-4 py-4 font-medium text-white">{row.comp}</td>
                      <td className="px-4 py-4">
                        <span className="text-[#F4A261] text-xs font-semibold">{row.since}</span>
                      </td>
                      <td className="px-4 py-4 flex justify-end gap-2">
                        <button 
                          onClick={(e) => handleRejectClick(e, row, 'company')}
                          className="px-4 py-2 bg-[#2A2A2D] text-gray-300 rounded hover:bg-[#333] transition">Reject</button>
                        <button 
                          onClick={(e) => handleApproveClick(e, row, 'company')}
                          className="px-4 py-2 bg-[#91B3FA] text-black font-medium rounded hover:bg-[#7fa1e8] transition">Approve</button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="3" className="py-16 text-center text-gray-400">
                      No pending companies currently.
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