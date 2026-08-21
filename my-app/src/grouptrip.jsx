import { useState } from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from 'recharts';
import {
  ChevronLeft,
  ChevronRight,
  MapPin,
  Building2,
  CalendarDays,
  Clock,
  CheckCircle2,
  XCircle,
  AlertTriangle,
} from 'lucide-react';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';
import ProgramDetails from './programDetailes';
import PendingProgramReview from './PendingProgramReview';
import { useApiData } from './hooks/useApiData';
import {
  getTourPackagesDashboard,
  getTourPackagesByStatus,
  getTourPackageStatusCounts,
  approveTourPackage,
  rejectTourPackage,
} from './services/dashboardApi';
import { mapTourPackage } from './utils/mappers';

const STATUS_TABS = [
  { key: 'Pending', label: 'Pending', icon: Clock, color: '#F4A261' },
  { key: 'Active', label: 'Active', icon: CheckCircle2, color: '#4ADE80' },
  { key: 'Completed', label: 'Completed', icon: CheckCircle2, color: '#91B3FA' },
  { key: 'Cancelled', label: 'Cancelled', icon: XCircle, color: '#F87171' },
  { key: 'Rejected', label: 'Rejected', icon: AlertTriangle, color: '#A78BFA' },
];

const STATUS_MAP = {
  Pending: 0,
  Active: 1,
  Completed: 2,
  Cancelled: 3,
  Rejected: 4,
};

export default function GroupTrip() {
  const [activeTab, setActiveTab] = useState('Pending');
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [selectedPendingProgram, setSelectedPendingProgram] = useState(null);
  const [selectedProgramDetails, setSelectedProgramDetails] = useState(null);
  const [selectedPendingReviewProgram, setSelectedPendingReviewProgram] = useState(null);
  const [actionError, setActionError] = useState('');

  const [tabPages, setTabPages] = useState({
    Pending: 1,
    Active: 1,
    Completed: 1,
    Cancelled: 1,
    Rejected: 1,
  });
  const [tabExpanded, setTabExpanded] = useState({
    Pending: true,
    Active: false,
    Completed: false,
    Cancelled: false,
    Rejected: false,
  });
  const pageSize = 10;

  const { data: tourPackagesData } = useApiData(getTourPackagesDashboard, []);
  const { data: statusCountsData } = useApiData(getTourPackageStatusCounts, []);

  const { data: tabData, loading: tabLoading, error: tabError } = useApiData(
    () => getTourPackagesByStatus(STATUS_MAP[activeTab], tabPages[activeTab], pageSize),
    [activeTab, tabPages[activeTab]]
  );

  const chartData = (tourPackagesData?.tourPackageGrowth ?? []).map((g) => ({
    name: g.month,
    TourPackages: g.count,
  }));

  const tabPagination = tabData?.pagination ?? {};
  const tabPackages = (tabData?.items ?? []).map(mapTourPackage);

  const openRejectDialog = (program) => {
    setSelectedPendingProgram(program);
    setIsRejectDialogOpen(true);
  };

  const openApproveDialog = (program) => {
    setSelectedPendingProgram(program);
    setIsApproveDialogOpen(true);
  };

  const handleSelectProgram = (program) => {
    setSelectedProgramDetails(program);
  };

  const handleOpenPendingReview = (program) => {
    setSelectedPendingReviewProgram(program);
  };

  const handlePendingReviewDecision = () => {
    setSelectedPendingReviewProgram(null);
  };

  const handleRejectSubmit = async (reason) => {
    if (!selectedPendingProgram) return;
    setActionError('');
    try {
      await rejectTourPackage(selectedPendingProgram.id, reason);
      setIsRejectDialogOpen(false);
      setSelectedPendingProgram(null);
      setTabPages((prev) => ({ ...prev, Pending: prev.Pending }));
    } catch (err) {
      setIsRejectDialogOpen(false);
      setActionError(err.message || 'Failed to reject the tour package.');
    }
  };

  const handleApproveConfirm = async () => {
    if (!selectedPendingProgram) return;
    setActionError('');
    try {
      await approveTourPackage(selectedPendingProgram.id);
      setIsApproveDialogOpen(false);
      setSelectedPendingProgram(null);
      setTabPages((prev) => ({ ...prev, Pending: prev.Pending }));
    } catch (err) {
      setIsApproveDialogOpen(false);
      setActionError(err.message || 'Failed to approve the tour package.');
    }
  };

  if (selectedPendingReviewProgram) {
    return (
      <PendingProgramReview
        program={selectedPendingReviewProgram}
        onBack={() => setSelectedPendingReviewProgram(null)}
        onDecision={handlePendingReviewDecision}
      />
    );
  }

  if (selectedProgramDetails) {
    return (
      <ProgramDetails
        program={selectedProgramDetails}
        onBack={() => setSelectedProgramDetails(null)}
      />
    );
  }

  return (
    <div className="p-8 space-y-8 font-sans">
      {/* Top: Growth Chart */}
      <div className="rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] p-6 shadow-lg">
        <h3 className="text-lg font-semibold text-white mb-4 text-left">Tour Package Growth Over Time</h3>
        <div className="mt-6 h-64 w-full">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
              <XAxis
                dataKey="name"
                stroke="#666"
                tick={{ fontSize: 12, fill: '#888' }}
              />
              <YAxis
                stroke="#666"
                tick={{ fontSize: 12, fill: '#888' }}
              />
              <Tooltip
                contentStyle={{ backgroundColor: '#1C1C1E', borderColor: '#F4A261', border: '1px solid #F4A261', borderRadius: '0.8rem' }}
              />
              <Legend iconType="plainline" />
              <Line type="monotone" dataKey="TourPackages" stroke="#F4A261" strokeWidth={3} dot={false} name="Tour Packages" />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Middle: Status Summary Box */}
      <div className="rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] p-6 shadow-lg">
        <h3 className="text-lg font-semibold text-white mb-4 text-left">Status Summary</h3>
        <div className="grid grid-cols-5 gap-4">
          {STATUS_TABS.map((tab) => {
            const Icon = tab.icon;
            const count = statusCountsData
              ? statusCountsData[tab.key.toLowerCase()] ?? 0
              : '—';
            return (
              <div
                key={tab.key}
                className="flex flex-col items-center gap-2 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4 text-center"
              >
                <Icon className="w-6 h-6" style={{ color: tab.color }} />
                <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">{tab.label}</p>
                <p className="text-xl font-bold" style={{ color: tab.color }}>
                  {typeof count === 'number' ? count.toLocaleString() : count}
                </p>
              </div>
            );
          })}
        </div>
      </div>

      {/* Bottom: Accordion Tabbed List */}
      <div className="rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] shadow-lg overflow-hidden">
        <div className="flex border-b border-[#333]">
          {STATUS_TABS.map((tab) => {
            const Icon = tab.icon;
            const isActive = activeTab === tab.key;
            const count = statusCountsData
              ? statusCountsData[tab.key.toLowerCase()] ?? 0
              : null;
            return (
              <button
                key={tab.key}
                onClick={() => setActiveTab(tab.key)}
                className={`flex-1 flex items-center justify-center gap-2 px-4 py-4 text-sm font-semibold transition-colors ${
                  isActive
                    ? 'bg-[#252528] text-white border-b-2'
                    : 'text-gray-400 hover:bg-[#202022] hover:text-gray-200'
                }`}
                style={isActive ? { borderBottomColor: tab.color } : undefined}
              >
                <Icon className="w-4 h-4" style={{ color: tab.color }} />
                {tab.label}
                {count !== null && (
                  <span
                    className="text-xs font-bold px-2 py-0.5 rounded-full"
                    style={{ backgroundColor: `${tab.color}20`, color: tab.color }}
                  >
                    {count}
                  </span>
                )}
              </button>
            );
          })}
        </div>

        {actionError && (
          <div className="m-4 rounded-xl border border-red-500/30 bg-red-500/10 p-3 text-sm text-red-400">
            {actionError}
          </div>
        )}

        <div className="p-5">
          {tabLoading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-8 h-8 border-4 border-[#2d303e] border-t-[#91B3FA] rounded-full animate-spin" />
            </div>
          ) : tabError ? (
            <div className="flex items-center justify-center py-10 text-center">
              <p className="text-sm text-red-400 bg-red-400/10 border border-red-400/20 rounded-xl px-4 py-3">
                Failed to load: {tabError.message}
              </p>
            </div>
          ) : tabPackages.length === 0 ? (
            <p className="text-sm text-gray-500 text-center py-10">No tour packages found for this status.</p>
          ) : (
            <>
              <div className="space-y-4 max-h-[500px] overflow-y-auto custom-scrollbar">
                {activeTab === 'Pending'
                  ? tabPackages.map((program) => (
                      <div
                        key={program.id}
                        className="cursor-pointer rounded-2xl border border-[#333] bg-[#18181A] p-5 shadow-md transition hover:border-[#91B3FA]/50"
                        onClick={() => handleOpenPendingReview(program)}
                      >
                        <div className="flex items-center justify-end gap-4 text-right">
                          <div className="space-y-2">
                            <h4 className="text-base font-bold text-white">{program.title}</h4>
                            <p className="flex items-center justify-end gap-2 text-xs text-gray-400">
                              <CalendarDays className="h-3.5 w-3.5" /> Starting: {program.startingDate}
                            </p>
                            <p className="flex items-center justify-end gap-2 text-xs text-[#91B3FA]">
                              <MapPin className="h-3.5 w-3.5" /> {program.country} - {program.regions}
                            </p>
                            <p className="flex items-center justify-end gap-2 text-xs text-gray-400">
                              <Building2 className="h-3.5 w-3.5" /> Publisher: {program.company}
                            </p>
                          </div>
                          <img src={program.image} alt={program.title} className="h-16 w-16 rounded-xl border border-[#2d303e] object-cover" />
                        </div>
                        <div className="mt-4 grid grid-cols-2 gap-3">
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              openRejectDialog(program);
                            }}
                            className="rounded-xl bg-[#2A2A2D] py-2.5 text-sm font-semibold text-gray-300 transition hover:bg-[#333]"
                          >
                            Reject
                          </button>
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              openApproveDialog(program);
                            }}
                            className="rounded-xl bg-[#91B3FA] py-2.5 text-sm font-semibold text-black shadow-[0_0_15px_rgba(145,179,250,0.15)] transition hover:bg-[#7fa1e8]"
                          >
                            Approve
                          </button>
                        </div>
                      </div>
                    ))
                  : tabPackages.map((trip) => (
                      <TripCard
                        key={trip.id}
                        trip={trip}
                        onSelect={() => handleSelectProgram(trip)}
                      />
                    ))}
              </div>
              {tabPackages.length > 0 && (
                <div className="flex items-center justify-between border-t border-[#333] px-2 py-4 mt-4">
                  <p className="text-xs text-gray-400">
                    Page {tabPagination.page || '—'} of {tabPagination.totalPages || '—'}
                  </p>
                  <div className="flex items-center gap-3">
                    <button
                      onClick={() => setTabPages((prev) => ({ ...prev, [activeTab]: Math.max(1, prev[activeTab] - 1) }))}
                      disabled={!tabPagination.hasPreviousPage}
                      className="flex items-center gap-1.5 rounded-xl border border-[#2d303e] bg-[#121212] px-4 py-2 text-sm font-medium text-gray-300 transition hover:border-[#91B3FA]/40 hover:text-white disabled:cursor-not-allowed disabled:opacity-40"
                    >
                      <ChevronLeft className="h-4 w-4" /> Prev
                    </button>
                    <button
                      onClick={() => setTabPages((prev) => ({ ...prev, [activeTab]: prev[activeTab] + 1 }))}
                      disabled={!tabPagination.hasNextPage}
                      className="flex items-center gap-1.5 rounded-xl border border-[#2d303e] bg-[#121212] px-4 py-2 text-sm font-medium text-gray-300 transition hover:border-[#91B3FA]/40 hover:text-white disabled:cursor-not-allowed disabled:opacity-40"
                    >
                      Next <ChevronRight className="h-4 w-4" />
                    </button>
                  </div>
                </div>
              )}
            </>
          )}
        </div>
      </div>

      <RejectDialog
        isOpen={isRejectDialogOpen}
        onClose={() => setIsRejectDialogOpen(false)}
        onSubmit={handleRejectSubmit}
        targetName={selectedPendingProgram?.title}
      />
      <ApproveDialog
        isOpen={isApproveDialogOpen}
        onClose={() => setIsApproveDialogOpen(false)}
        onConfirm={handleApproveConfirm}
        targetName={selectedPendingProgram?.title}
      />
    </div>
  );
}

function TripCard({ trip, onSelect, isDeleted = false }) {
  return (
    <div
      className={`rounded-2xl border border-[#333] bg-[#18181A] p-5 shadow-md transition ${!isDeleted ? 'cursor-pointer hover:border-[#91B3FA]/50' : 'cursor-default'}`}
      onClick={() => !isDeleted && onSelect?.(trip)}
    >
      <div className="flex items-start justify-between gap-4">
        <div className="text-xs font-medium whitespace-nowrap text-gray-500">
          Starting: {trip.startingDate}
        </div>

        <div className="flex flex-1 items-center justify-end gap-5 text-right">
          <div className="space-y-2">
            <h4 className="text-base font-semibold text-white">{trip.title}</h4>

            <div className="flex items-center justify-end gap-4 text-xs text-gray-400">
              <span className="flex items-center gap-1.5">
                <CalendarDays className="h-3.5 w-3.5" /> Starting: {trip.startingDate}
              </span>
              <span>•</span>
              <span className="flex items-center gap-1.5 text-[#91B3FA]">
                <MapPin className="h-3.5 w-3.5" /> {trip.country} - {trip.regions}
              </span>
            </div>

            <div className="flex items-center justify-end gap-1.5 text-xs text-gray-400">
              <Building2 className="h-3.5 w-3.5" /> Publisher: {trip.company}
            </div>
          </div>

          <img src={trip.image} alt={trip.title} className="h-16 w-16 rounded-xl border border-[#2d303e] object-cover" />
        </div>
      </div>
    </div>
  );
}
