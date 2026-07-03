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

const chartData = [
  { name: 'Jan', Tourists: 10, Companies: 15 },
  { name: 'Feb', Tourists: 15, Companies: 20 },
  { name: 'Mar', Tourists: 20, Companies: 25 },
  { name: 'Apr', Tourists: 28, Companies: 30 },
  { name: 'May', Tourists: 35, Companies: 32 },
  { name: 'Jun', Tourists: 42, Companies: 40 },
  { name: 'Jul', Tourists: 48, Companies: 38 },
  { name: 'Aug', Tourists: 50, Companies: 45 },
  { name: 'Sep', Tourists: 65, Companies: 55 },
  { name: 'Oct', Tourists: 75, Companies: 60 },
  { name: 'Nov', Tourists: 80, Companies: 68 },
  { name: 'Dec', Tourists: 90, Companies: 75 },
];

const sampleTrips = [
  {
    id: 1,
    title: 'Magic of the East',
    country: 'Turkey',
    regions: 'Istanbul, Bursa, Sapanca',
    company: 'Elite Journeys',
    startingDate: '12/4/2026',
    image: 'https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=150&auto=format&fit=crop&q=60',
  },
  {
    id: 2,
    title: 'Classic Paris Lights',
    country: 'France',
    regions: 'Paris, Versailles',
    company: 'Oceanic Ventures',
    startingDate: '12/4/2026',
    image: 'https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=150&auto=format&fit=crop&q=60',
  },
  {
    id: 3,
    title: 'Desert Serenity',
    country: 'UAE',
    regions: 'Abu Dhabi, Al Ain',
    company: 'Golden Routes',
    startingDate: '20/4/2026',
    image: 'https://images.unsplash.com/photo-1516483638261-f4dbaf036963?w=150&auto=format&fit=crop&q=60',
  },
];

const pendingPrograms = [
  {
    id: 4,
    title: 'Nile Heritage Tour',
    country: 'Egypt',
    regions: 'Cairo, Luxor, Aswan',
    company: 'Ancient Trails',
    companyLogo: 'https://images.unsplash.com/photo-1560179707-f14e90ef3623?w=150&q=80',
    startingDate: '15/6/2026',
    image: 'https://images.unsplash.com/photo-1548013146-72479768bada?w=150&auto=format&fit=crop&q=60',
    coverImage: 'https://images.unsplash.com/photo-1548013146-72479768bada?w=800&q=80',
    description: 'A heritage-rich journey through Egypt with visits to ancient landmarks, Nile cruises, and local culinary experiences.',
    submissionDate: '3 Hours ago',
    registrationDeadline: '10 Jul 2026',
    startDate: '15 Jul 2026',
    duration: '6 Days & 5 Nights',
    meetingPoint: 'Cairo International Airport',
    locations: ['Cairo', 'Luxor', 'Aswan', 'Nile Cruise'],
    price: 280000,
    totalSpots: 18,
    guide: {
      name: 'Omar Hassan',
      experience: '7 Years Experience',
      avatar: 'https://i.pravatar.cc/150?img=16',
    },
    itinerary: [
      { id: 0, date: '15/7', title: 'Day One', subtitle: 'Arrival & Pyramids', activities: [{ time: '9:00 AM', name: 'Giza Pyramids', desc: 'Explore the ancient wonders with a certified guide.' }] },
      { id: 1, date: '16/7', title: 'Day Two', subtitle: 'Nile Cruise', activities: [{ time: '8:00 AM', name: 'River Cruise', desc: 'Enjoy a relaxing cruise along the Nile.' }] },
    ],
  },
  {
    id: 5,
    title: 'Scenic Balkan Adventure',
    country: 'Bosnia',
    regions: 'Sarajevo, Mostar',
    company: 'Atlas Travel',
    companyLogo: 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=150&q=80',
    startingDate: '22/6/2026',
    image: 'https://images.unsplash.com/photo-1517760444937-f6397edcbbcd?w=150&auto=format&fit=crop&q=60',
    coverImage: 'https://images.unsplash.com/photo-1517760444937-f6397edcbbcd?w=800&q=80',
    description: 'A scenic adventure through the Balkans with mountain views, historic towns, and unforgettable cultural stops.',
    submissionDate: '1 Day ago',
    registrationDeadline: '18 Jul 2026',
    startDate: '22 Jul 2026',
    duration: '7 Days & 6 Nights',
    meetingPoint: 'Sarajevo Airport',
    locations: ['Sarajevo', 'Mostar', 'Trebinje', 'Jajce'],
    price: 320000,
    totalSpots: 22,
    guide: {
      name: 'Lejla Mujic',
      experience: '9 Years Experience',
      avatar: 'https://i.pravatar.cc/150?img=18',
    },
    itinerary: [
      { id: 0, date: '22/7', title: 'Day One', subtitle: 'Arrival & Old Town', activities: [{ time: '10:00 AM', name: 'Old Town Walk', desc: 'Discover Sarajevo’s heritage streets.' }] },
      { id: 1, date: '23/7', title: 'Day Two', subtitle: 'Mostar Visit', activities: [{ time: '9:30 AM', name: 'Bridge Tour', desc: 'Visit the legendary old bridge and riverside cafes.' }] },
    ],
  },
  {
    id: 6,
    title: 'Coastal Morocco Escape',
    country: 'Morocco',
    regions: 'Marrakech, Essaouira',
    company: 'Sahara Voyages',
    companyLogo: 'https://images.unsplash.com/photo-1497215842964-222b430dc094?w=150&q=80',
    startingDate: '01/7/2026',
    image: 'https://images.unsplash.com/photo-1548013146-72479768bada?w=150&auto=format&fit=crop&q=60',
    coverImage: 'https://images.unsplash.com/photo-1548013146-72479768bada?w=800&q=80',
    description: 'Enjoy coastal charm and desert vibes in Morocco with food tours, markets, and beachfront evenings.',
    submissionDate: '2 Days ago',
    registrationDeadline: '20 Jul 2026',
    startDate: '01 Aug 2026',
    duration: '5 Days & 4 Nights',
    meetingPoint: 'Marrakech Airport',
    locations: ['Marrakech', 'Essaouira', 'Agadir', 'Sidi Ifni'],
    price: 260000,
    totalSpots: 16,
    guide: {
      name: 'Youssef Benali',
      experience: '6 Years Experience',
      avatar: 'https://i.pravatar.cc/150?img=17',
    },
    itinerary: [
      { id: 0, date: '1/8', title: 'Day One', subtitle: 'Arrival & Medina', activities: [{ time: '3:00 PM', name: 'Medina Walk', desc: 'Stroll through the colorful old city.' }] },
      { id: 1, date: '2/8', title: 'Day Two', subtitle: 'Coastal Escape', activities: [{ time: '11:00 AM', name: 'Essaouira Tour', desc: 'Enjoy the coastal breeze and artisan markets.' }] },
    ],
  },
];

export default function GroupTrip() {
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isTotalOpen, setIsTotalOpen] = useState(true);
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [selectedPendingProgram, setSelectedPendingProgram] = useState(null);
  const [selectedProgramDetails, setSelectedProgramDetails] = useState(null);
  const [selectedPendingReviewProgram, setSelectedPendingReviewProgram] = useState(null);
  const [pendingProgramsList, setPendingProgramsList] = useState(pendingPrograms);

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
        program={{
          name: selectedProgramDetails.title,
          companyName: selectedProgramDetails.company,
        }}
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
                  <Line type="monotone" dataKey="Tourists" stroke="#F4A261" strokeWidth={3} dot={false} name="Tourists" />
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
                  <p className="mt-0.5 text-xl font-bold text-[#91B3FA]">1,250</p>
                </div>
              </div>

              <div className="flex items-center gap-4 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4">
                <div className="flex h-8 w-8   items-center justify-center text-red-400">
                  <CircleOff className="w-8 h-8 " />
                </div>
                <div>
                  <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">Cancelled Programs</p>
                  <p className="mt-0.5 text-xl font-bold text-red-400">150</p>
                </div>
              </div>

              <div className="flex items-center gap-4 rounded-xl border border-[#2a2a2a] bg-[#121212] p-4">
                <div className="flex h-10 w-10 items-center justify-center text-[#F4A261]">
                  <Hourglass className="w-8 h-8 "  />
                </div>
                <div>
                  <p className="text-[10px] font-bold uppercase tracking-wider text-gray-500">Pending Programs</p>
                  <p className="mt-0.5 text-xl font-bold text-[#F4A261]">45</p>
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
                          {sampleTrips.map(trip => (
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
