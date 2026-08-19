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
  ChevronDown,
  //ChevronUp,
  MapPin,
  Building2,
  PlaneTakeoff,
  CircleOff,
  CalendarDays,
  Hourglass,
} from 'lucide-react';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';
import ProgramDetails from './programDetailes';
import PendingProgramReview from './PendingProgramReview';
import { useApiData } from './hooks/useApiData';
import { useSyncedState } from './hooks/useSyncedState';
import { getTourPackagesDashboard, getTourPackages } from './services/dashboardApi';
import { mapTourPackage } from './utils/mappers';

export default function GroupTrip() {
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isTotalOpen, setIsTotalOpen] = useState(true);
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [selectedPendingProgram, setSelectedPendingProgram] = useState(null);
  const [selectedProgramDetails, setSelectedProgramDetails] = useState(null);
  const [selectedPendingReviewProgram, setSelectedPendingReviewProgram] = useState(null);
  const { data: tourPackagesData } = useApiData(getTourPackagesDashboard, []);
  // pageSize كبيرة لأن التصميم الحالي بيعرض القوائم كاملة بدون Pagination UI
  const { data: allPackagesData } = useApiData(() => getTourPackages(1, 200), []);

  // GET /api/Admin/dashboard/tour-packages -> tourPackageGrowth
  const chartData = (tourPackagesData?.tourPackageGrowth ?? []).map((g) => ({
    name: g.month,
    Programs: g.count,
  }));

  // GET /api/TourPackage -> نفلترها حسب publishLabel لتصير Cancelled/All/Pending
  // ملاحظة: القيم الدقيقة لـ status/publishLabel مو موثقة رسمياً، فهاد أفضل تخمين ممكن بالاعتماد على النص
  const allPackages = (allPackagesData?.items ?? []).map(mapTourPackage);
  const sampleTrips = allPackages.filter(
    (p) => !/reject|cancel|pending/i.test(p.publishLabel)
  );
  const cancelledTrips = allPackages.filter((p) => /reject|cancel/i.test(p.publishLabel));
  const [pendingProgramsList, setPendingProgramsList] = useSyncedState(
    allPackagesData,
    (d) => (d?.items ?? []).map(mapTourPackage).filter((p) => /pending/i.test(p.publishLabel))
  );

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

  const handlePendingReviewDecision = (programId) => {
    setPendingProgramsList((prev) => prev.filter((item) => item.id !== programId));
    setSelectedPendingReviewProgram(null);
  };

  const handleRejectSubmit = (reason) => {
    console.log(`Rejected ${selectedPendingProgram?.title} for reason: ${reason}`);
    setPendingProgramsList((prev) => prev.filter((item) => item.id !== selectedPendingProgram?.id));
    setIsRejectDialogOpen(false);
    setSelectedPendingProgram(null);
  };

  const handleApproveConfirm = () => {
    console.log(`Approved ${selectedPendingProgram?.title}`);
    setPendingProgramsList((prev) => prev.filter((item) => item.id !== selectedPendingProgram?.id));
    setIsApproveDialogOpen(false);
    setSelectedPendingProgram(null);
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
      <div className="grid grid-cols-1 gap-8 lg:grid-cols-12">
        <div className="space-y-6 lg:col-span-7">
          <div className="rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] p-6 shadow-lg">
            <h3 className="text-lg font-semibold text-white mb-4 text-left">Programs Growth Over Time</h3>
            <div className="mt-6 h-64 w-full mb-6">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData} 
                // margin={{ top: 5, right: 20, left: -20, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis
                    dataKey="name"
                    stroke="#666"
                    // axisLine={false}
                    // tickLine={false}
                    tick={{ fontSize: 12, fill: '#888' }}
                    // dy={10}
                  />
                  <YAxis
                    stroke="#666"
                    //axisLine={false}
                    //tickLine={false}
                    tick={{ fontSize: 12, fill: '#888' }}
                  />
                  <Tooltip
                    contentStyle={{backgroundColor: '#1C1C1E', borderColor: '#F4A261', border: '1px solid #F4A261', borderRadius: '0.8rem'}}
                    //itemStyle={{ color: '#fff' }}
                  />
                  <Legend iconType="plainline" 
                  //wrapperStyle={{ fontSize: '13px', paddingTop: '10px' }}
                   />
                  <Line type="monotone" dataKey="Programs" stroke="#F4A261" strokeWidth={3} dot={false} name="Programs" />
                  {/* <Line type="monotone" dataKey="Companies" stroke="#91B3FA" strokeWidth={3} dot={false} name="Companies" /> */}
                </LineChart>
              </ResponsiveContainer>
            </div>

            <div className=" grid grid-cols-3 gap-4 border-t border-[#333] pt-6">
              <div className="flex items-center gap-4 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4">
                <div className="flex h-8 w-8 items-center justify-center text-[#91B3FA]">
                  <PlaneTakeoff className="w-8 h-8 " />
                </div>
                <div>
                  <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">Total Programs</p>
                  <p className="mt-0.5 text-xl font-bold text-[#91B3FA]">{tourPackagesData ? tourPackagesData.totalTourPackages.toLocaleString() : '—'}</p>
                </div>
              </div>

              <div className="flex items-center gap-4 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4">
                <div className="flex h-8 w-8   items-center justify-center text-red-400">
                  <CircleOff className="w-8 h-8 " />
                </div>
                <div>
                  <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">Cancelled Programs</p>
                  <p className="mt-0.5 text-xl font-bold text-red-400">{tourPackagesData ? tourPackagesData.rejectedTourPackages.toLocaleString() : '—'}</p>
                </div>
              </div>

              <div className="flex items-center gap-4 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4">
                <div className="flex h-10 w-10 items-center justify-center text-[#F4A261]">
                  <Hourglass className="w-8 h-8 "  />
                </div>
                <div>
                  <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">Pending Programs</p>
                  <p className="mt-0.5 text-xl font-bold text-[#F4A261]">{tourPackagesData ? tourPackagesData.pendingTourPackages.toLocaleString() : '—'}</p>
                </div>
              </div>
            </div>
          </div>
          <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
                      <button 
                        onClick={() => setIsRejectedOpen(!isRejectedOpen)}
                        className="w-full p-5 flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition"
                      >
                        <div className="flex items-center gap-4">
                          <CircleOff className="w-8 h-8  text-red-400 ml-5" />
                          <h3 className="text-base font-semibold text-white ">Cancelled Programs</h3>
                        </div>
                        <ChevronDown className={`w-5 h-5 text-gray-400 transition-transform mr-10 duration-300 ${isRejectedOpen ? 'rotate-180' : ''}`} />
                        
                        
                      </button>
                      {isRejectedOpen && (
                        <div className="p-5 space-y-4 max-h-[350px] overflow-y-auto custom-scrollbar border-t border-[#333]">
                          {cancelledTrips.map(trip => (
                            <TripCard key={trip.id} trip={trip} isDeleted={true} />
                          ))}
                        </div>
                      )}
                    </div>
          
                   
                    <div className="bg-[#1C1C1E] border border-[#D4AF37]/30 rounded-2xl overflow-hidden shadow-lg transition-all duration-300">
                      <button 
                        onClick={() => setIsTotalOpen(!isTotalOpen)}
                        className="w-full p-5 flex justify-between items-center bg-[#202022] hover:bg-[#252528] transition"
                      >
                        <div className="flex items-center gap-4">
                          <PlaneTakeoff className="w-8 h-8   ml-5 text-[#91B3FA]" />
                          <h3 className="text-base text-[#91B3FA] font-semibold  ml-10">All Programs</h3>
                        </div>
                        <ChevronDown className={`w-5 h-5 text-gray-400 mr-10 transition-transform duration-300 ${isTotalOpen ? 'rotate-180' : ''}`} />
                        
                      </button>
                      {isTotalOpen && (
                        <div className="p-5 space-y-4 max-h-[400px] overflow-y-auto custom-scrollbar border-t border-[#333]">
                          {sampleTrips.map(trip => (
                            <TripCard
                              key={trip.id}
                              trip={trip}
                              isDeleted={false}
                              onSelect={() => handleSelectProgram(trip)}
                            />
                          ))}
                        </div>
                      )}
                    </div>
        </div>

        <div className="lg:col-span-5">
          <div className="flex h-full flex-col rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] p-6 shadow-[0_10px_25px_rgba(0,0,0,0.25)]">
            <h3 className="border-b border-[#333] pb-4 text-left text-lg ml-5 font-semibold text-white">Pending Programs</h3>
            <div className="mt-5 flex flex-1 flex-col gap-4 overflow-y-auto pr-2">
              {pendingProgramsList.map((program) => (
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
              ))}
            </div>
          </div>
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

// function AccordionPanel({ title, titleColor = 'text-white', isOpen, onToggle, children }) {
//   return (
//     <div className="overflow-hidden rounded-2xl border border-[#D4AF37]/30 bg-[#1C1C1E] shadow-[0_10px_25px_rgba(0,0,0,0.2)]">
//       <button
//         onClick={onToggle}
//         className="flex w-full items-center justify-between bg-[#202022] px-5 py-4 transition hover:bg-[#252528]"
//       >
//         <div className="flex items-center">
//           {isOpen ? <ChevronUp className="h-5 w-5 text-gray-400" /> : <ChevronDown className="h-5 w-5 text-gray-400" />}
//         </div>
//         <span className={`text-sm font-semibold ${titleColor}`}>{title}</span>
//       </button>

//       {isOpen && (
//         <div className="grid gap-4 border-t border-[#333] bg-[#1C1C1E] p-5 md:grid-cols-2">
//           {children}
//         </div>
//       )}
//     </div>
//   );
// }