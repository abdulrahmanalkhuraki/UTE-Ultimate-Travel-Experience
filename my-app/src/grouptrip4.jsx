import { useState } from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend
} from 'recharts';
import { 
  ChevronDown, 
  ChevronUp, 
  MapPin, 
  Building2, 
  Globe, 
  CalendarDays, 
  Trash2, 
  Hourglass
} from 'lucide-react';
import RejectDialog from './components/RejectDialog';
import ApproveDialog from './components/ApproveDialog';
import ProgramDetails from './programDetailes';

// Mock data for the chart
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

// Mock data for the trips
const sampleTrips = [
  {
    id: 1,
    title: "Magic of the East",
    country: "Turkey",
    regions: "Istanbul, Bursa, Sapanca",
    company: "Elite Journeys",
    startingDate: "12/4/2026",
    image: "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=150&auto=format&fit=crop&q=60"
  },
  {
    id: 2,
    title: "Classic Paris Lights",
    country: "France",
    regions: "Paris, Versailles",
    company: "Oceanic Ventures",
    startingDate: "12/4/2026",
    image: "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=150&auto=format&fit=crop&q=60"
  },
  {
    id: 3,
    title: "Desert Serenity",
    country: "UAE",
    regions: "Abu Dhabi, Al Ain",
    company: "Golden Routes",
    startingDate: "20/4/2026",
    image: "https://images.unsplash.com/photo-1516483638261-f4dbaf036963?w=150&auto=format&fit=crop&q=60"
  }
];

const pendingPrograms = [
  {
    id: 4,
    title: "Nile Heritage Tour",
    country: "Egypt",
    regions: "Cairo, Luxor, Aswan",
    company: "Ancient Trails",
    startingDate: "15/6/2026",
    image: "https://images.unsplash.com/photo-1548013146-72479768bada?w=150&auto=format&fit=crop&q=60"
  },
  {
    id: 5,
    title: "Scenic Balkan Adventure",
    country: "Bosnia",
    regions: "Sarajevo, Mostar",
    company: "Atlas Travel",
    startingDate: "22/6/2026",
    image: "https://images.unsplash.com/photo-1517760444937-f6397edcbbcd?w=150&auto=format&fit=crop&q=60"
  },
  {
    id: 6,
    title: "Coastal Morocco Escape",
    country: "Morocco",
    regions: "Marrakech, Essaouira",
    company: "Sahara Voyages",
    startingDate: "01/7/2026",
    image: "https://images.unsplash.com/photo-1548013146-72479768bada?w=150&auto=format&fit=crop&q=60"
  }
];

export default function GroupTrip() {
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isTotalOpen, setIsTotalOpen] = useState(false);
  const [isRejectDialogOpen, setIsRejectDialogOpen] = useState(false);
  const [isApproveDialogOpen, setIsApproveDialogOpen] = useState(false);
  const [selectedPendingProgram, setSelectedPendingProgram] = useState(null);
  const [selectedProgramDetails, setSelectedProgramDetails] = useState(null);
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
    <div style={styles.container}>
      <div style={styles.pageGrid}>
        <div style={styles.mainContent}>
          <div style={styles.card}>
            <h3 style={styles.cardTitle}>Programs Growth Over Time</h3>
            <div style={{ width: '100%', height: 280, marginTop: '20px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData} margin={{ top: 5, right: 20, left: -20, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#333" vertical={false} />
                  <XAxis 
                    dataKey="name" 
                    stroke="#666" 
                    axisLine={false} 
                    tickLine={false} 
                    tick={{ fontSize: 12, fill: '#888' }} 
                    dy={10}
                  />
                  <YAxis 
                    stroke="#666" 
                    axisLine={false} 
                    tickLine={false} 
                    tick={{ fontSize: 12, fill: '#888' }}
                  />
                  <Tooltip 
                    contentStyle={{ backgroundColor: '#1C1C1E', border: '1px solid #333', borderRadius: '8px' }}
                    itemStyle={{ color: '#fff' }}
                  />
                  <Legend 
                    iconType="plainline" 
                    wrapperStyle={{ fontSize: '13px', paddingTop: '10px' }} 
                  />
                  <Line type="monotone" dataKey="Tourists" stroke="#F4A261" strokeWidth={3} dot={false} name="Tourists" />
                  <Line type="monotone" dataKey="Companies" stroke="#91B3FA" strokeWidth={3} dot={false} name="Companies" />
                </LineChart>
              </ResponsiveContainer>
            </div>

            <div style={styles.statsGrid}>
              <div style={styles.statCard}>
                <div style={{ ...styles.iconBox, color: '#91B3FA' }}>
                  <Globe size={24} />
                </div>
                <div style={styles.statInfo}>
                  <span style={styles.statLabel}>TOTAL PROGRAMS</span>
                  <span style={styles.statValue}>1,250</span>
                </div>
              </div>

              <div style={styles.statCard}>
                <div style={{ ...styles.iconBox, color: '#ef4444' }}>
                  <Trash2 size={24} />
                </div>
                <div style={styles.statInfo}>
                  <span style={styles.statLabel}>CANCELLED PROGRAMS</span>
                  <span style={styles.statValue}>150</span>
                </div>
              </div>

              <div style={styles.statCard}>
                <div style={{ ...styles.iconBox, color: '#F4A261' }}>
                  <Hourglass size={24} />
                </div>
                <div style={styles.statInfo}>
                  <span style={styles.statLabel}>PENDING PROGRAMS</span>
                  <span style={styles.statValue}>45</span>
                </div>
              </div>
            </div>
          </div>

          <div style={styles.accordionsWrapper}>
            <div style={styles.accordionContainer}>
              <div 
                style={styles.accordionHeader} 
                onClick={() => setIsRejectedOpen(!isRejectedOpen)}
              >
                <div style={styles.accordionIcon}>
                  {isRejectedOpen ? <ChevronUp size={20} color="#6b7280" /> : <ChevronDown size={20} color="#6b7280" />}
                </div>
                <span style={styles.accordionTitle}>Rejected Programs</span>
              </div>
              
              {isRejectedOpen && (
                <div style={styles.accordionContent}>
                  {sampleTrips.slice(0, 2).map((trip) => (
                    <TripCard key={`rejected-${trip.id}`} trip={trip} showActions={false} onSelect={handleSelectProgram} />
                  ))}
                </div>
              )}
            </div>

            <div style={styles.accordionContainer}>
              <div 
                style={styles.accordionHeader} 
                onClick={() => setIsTotalOpen(!isTotalOpen)}
              >
                <div style={styles.accordionIcon}>
                  {isTotalOpen ? <ChevronUp size={20} color="#6b7280" /> : <ChevronDown size={20} color="#6b7280" />}
                </div>
                <span style={{ ...styles.accordionTitle, color: '#f59e0b' }}>Total Programs</span>
              </div>
              
              {isTotalOpen && (
                <div style={styles.accordionContent}>
                  {sampleTrips.slice(0, 2).map((trip) => (
                    <TripCard key={`total-${trip.id}`} trip={trip} showActions={false} onSelect={handleSelectProgram} />
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        <div style={styles.sidePanel}>
          <div style={styles.sidePanelHeader}>Pending Programs</div>
          <div style={styles.sidePanelList}>
            {pendingProgramsList.map((program) => (
              <div key={program.id} style={{ ...styles.pendingCard, cursor: 'pointer' }} onClick={() => handleSelectProgram(program)}>
                <div style={styles.pendingInfoBox}>
                  <div style={styles.pendingTextBlock}>
                    <h4 style={styles.pendingTitle}>{program.title}</h4>
                    <p style={styles.pendingMetaRow}><CalendarDays size={14} color="#9ca3af" /> Starting: {program.startingDate}</p>
                    <p style={styles.pendingMetaRow}><MapPin size={14} color="#91B3FA" /> {program.country} - {program.regions}</p>
                    <p style={styles.pendingMetaRow}><Building2 size={14} color="#9ca3af" /> Publisher: {program.company}</p>
                  </div>
                  <img src={program.image} alt={program.title} style={styles.pendingImage} />
                </div>
                <div style={styles.pendingActions}>
                  <button onClick={(e) => { e.stopPropagation(); openRejectDialog(program); }} style={styles.pendingRejectBtn}>Reject</button>
                  <button onClick={(e) => { e.stopPropagation(); openApproveDialog(program); }} style={styles.pendingApproveBtn}>Approve</button>
                </div>
              </div>
            ))}
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

// Sub-component for the Trip Card to keep code clean and match the image's right-side cards
function TripCard({ trip, showActions = true, onSelect }) {
  return (
    <div style={{ ...styles.tripCard, cursor: 'pointer' }} onClick={() => onSelect?.(trip)}>
      <div style={styles.tripCardTop}>
        <div style={styles.tripCardInfo}>
          <h4 style={styles.tripTitle}>{trip.title}</h4>
          
          <div style={styles.tripMetaRow}>
            <MapPin size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>{trip.country} - {trip.regions}</span>
          </div>
          
          <div style={styles.tripMetaRow}>
            <Building2 size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>Publisher: {trip.company}</span>
          </div>
          
          <div style={styles.tripMetaRow}>
            <CalendarDays size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>Starting date: <strong style={{color: '#f8fafc'}}>{trip.startingDate}</strong></span>
          </div>
        </div>
        
        <img src={trip.image} alt={trip.title} style={styles.tripImage} />
      </div>

      {showActions && (
        <div style={styles.tripCardActions}>
          <button style={styles.btnReject}>Reject</button>
          <button style={styles.btnApprove}>Approve</button>
        </div>
      )}
    </div>
  );
}

// Styles meticulously crafted to match the provided dark theme image
const styles = {
  container: {
    padding: '30px',
    backgroundColor: '#121212', // Dark background matching the image
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: '"Inter", "Segoe UI", sans-serif',
    direction: 'ltr',
  },
  pageGrid: {
    display: 'grid',
    gridTemplateColumns: '1.4fr 0.8fr',
    gap: '24px',
    alignItems: 'start',
  },
  mainContent: {
    display: 'flex',
    flexDirection: 'column',
    gap: '24px',
  },
  card: {
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    padding: '100px',
    boxShadow: '0 10px 25px rgba(0,0,0,0.25)',
  },
  cardTitle: {
    fontSize: '16px',
    fontWeight: '600',
    color: '#f8fafc',
    margin: 0,
  },
  statsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(3, minmax(0, 1fr))',
    gap: '16px',
    marginTop: '24px',
    paddingTop: '20px',
    borderTop: '1px solid #333',
  },
  statCard: {
    backgroundColor: '#121212',
    border: '1px solid #2a2a2a',
    borderRadius: '12px',
    padding: '20px',
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
  },
  iconBox: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  statInfo: {
    display: 'flex',
    flexDirection: 'column',
  },
  statLabel: {
    fontSize: '10px',
    fontWeight: '600',
    color: '#6b7280',
    letterSpacing: '0.5px',
    marginBottom: '4px',
  },
  statValue: {
    fontSize: '24px',
    fontWeight: '700',
    color: '#ffffff',
    lineHeight: '1',
  },
  accordionsWrapper: {
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
  },
  accordionContainer: {
    display: 'flex',
    flexDirection: 'column',
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    overflow: 'hidden',
    boxShadow: '0 10px 25px rgba(0,0,0,0.2)',
  },
  accordionHeader: {
    backgroundColor: '#202022',
    padding: '16px 20px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    cursor: 'pointer',
    userSelect: 'none',
  },
  accordionIcon: {
    marginRight: '16px',
    display: 'flex',
    alignItems: 'center',
  },
  accordionTitle: {
    fontSize: '14px',
    fontWeight: '600',
    color: '#f8fafc',
  },
  accordionContent: {
    padding: '20px',
    display: 'grid',
    gridTemplateColumns: 'repeat(2, minmax(0, 1fr))',
    gap: '16px',
    borderTop: '1px solid #333',
    backgroundColor: '#1C1C1E',
  },
  // Trip Card styles matching the "Pending Company Applications" cards in the image
  tripCard: {
    backgroundColor: '#18181A',
    border: '1px solid #333',
    borderRadius: '14px',
    padding: '20px',
    display: 'flex',
    flexDirection: 'column',
    gap: '20px',
    boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
  },
  tripCardTop: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    gap: '16px',
  },
  tripCardInfo: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    flex: 1,
  },
  tripTitle: {
    fontSize: '15px',
    fontWeight: '600',
    color: '#ffffff',
    margin: '0 0 4px 0',
  },
  tripMetaRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
  },
  tripMetaText: {
    fontSize: '12px',
    color: '#9ca3af',
  },
  tripImage: {
    width: '70px',
    height: '70px',
    borderRadius: '8px',
    objectFit: 'cover',
  },
  tripCardActions: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '12px',
  },
  btnReject: {
    backgroundColor: '#2d2d2d', // Dark grey button matching image
    color: '#d1d5db',
    border: 'none',
    padding: '10px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '500',
    cursor: 'pointer',
    transition: 'background 0.2s',
  },
  btnApprove: {
    backgroundColor: '#93c5fd', // Light blue button matching image
    color: '#000000',
    border: 'none',
    padding: '10px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    transition: 'opacity 0.2s',
  },
  sidePanel: {
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    boxShadow: '0 10px 25px rgba(0,0,0,0.25)',
    padding: '20px',
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
    minHeight: '100%',
  },
  sidePanelHeader: {
    fontSize: '18px',
    fontWeight: '700',
    color: '#ffffff',
    paddingBottom: '12px',
    borderBottom: '1px solid #333',
    textAlign: 'right',
  },
  sidePanelList: {
    display: 'flex',
    flexDirection: 'column',
    gap: '14px',
    overflowY: 'auto',
    paddingRight: '4px',
  },
  pendingCard: {
    backgroundColor: '#18181A',
    border: '1px solid #333',
    borderRadius: '14px',
    padding: '14px',
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
    boxShadow: '0 4px 12px rgba(0,0,0,0.2)',
  },
  pendingInfoBox: {
    display: 'flex',
    alignItems: 'flex-start',
    justifyContent: 'space-between',
    gap: '12px',
  },
  pendingTextBlock: {
    flex: 1,
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
    textAlign: 'right',
  },
  pendingTitle: {
    fontSize: '15px',
    fontWeight: '700',
    color: '#ffffff',
    margin: 0,
  },
  pendingMetaRow: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    gap: '6px',
    fontSize: '12px',
    color: '#9ca3af',
    margin: 0,
  },
  pendingImage: {
    width: '60px',
    height: '60px',
    borderRadius: '10px',
    objectFit: 'cover',
    border: '1px solid #2d303e',
  },
  pendingActions: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '10px',
  },
  pendingRejectBtn: {
    backgroundColor: '#2A2A2D',
    color: '#d1d5db',
    border: 'none',
    padding: '10px',
    borderRadius: '10px',
    fontSize: '13px',
    fontWeight: '600',
    cursor: 'pointer',
  },
  pendingApproveBtn: {
    backgroundColor: '#91B3FA',
    color: '#000000',
    border: 'none',
    padding: '10px',
    borderRadius: '10px',
    fontSize: '13px',
    fontWeight: '700',
    cursor: 'pointer',
    boxShadow: '0 0 15px rgba(145,179,250,0.15)',
  }
};