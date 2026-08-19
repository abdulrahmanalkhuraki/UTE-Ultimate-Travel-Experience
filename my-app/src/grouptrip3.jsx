import { useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { ChevronDown, ChevronUp, MapPin, Building2, Globe, CalendarDays, Trash2, Hourglass } from 'lucide-react';

const chartData = [
  { name: 'Jan', Tourists: 10, Programs: 15 },
  { name: 'Feb', Tourists: 15, Programs: 20 },
  { name: 'Mar', Tourists: 20, Programs: 25 },
  { name: 'Apr', Tourists: 28, Programs: 30 },
  { name: 'May', Tourists: 35, Programs: 32 },
  { name: 'Jun', Tourists: 42, Programs: 40 },
  { name: 'Jul', Tourists: 48, Programs: 38 },
  { name: 'Aug', Tourists: 50, Programs: 45 },
  { name: 'Sep', Tourists: 65, Programs: 55 },
  { name: 'Oct', Tourists: 75, Programs: 60 },
  { name: 'Nov', Tourists: 80, Programs: 68 },
  { name: 'Dec', Tourists: 90, Programs: 75 },
];

const samplePrograms = [
  {
    id: 1,
    title: 'Golden Desert Safari',
    country: 'UAE',
    regions: 'Dubai Desert Conservation Reserve',
    operator: 'Desert Trails',
    startingDate: '10/07/2026',
    image: 'https://images.unsplash.com/photo-1519821172141-bb4f64760f3f?w=480&auto=format&fit=crop&q=80'
  },
  {
    id: 2,
    title: 'Mediterranean Escape',
    country: 'Greece',
    regions: 'Santorini, Mykonos',
    operator: 'Aegean Wonders',
    startingDate: '22/08/2026',
    image: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=480&auto=format&fit=crop&q=80'
  },
  {
    id: 3,
    title: 'Nordic Light Adventure',
    country: 'Iceland',
    regions: 'Reykjavik, Golden Circle',
    operator: 'Aurora Expeditions',
    startingDate: '05/10/2026',
    image: 'https://images.unsplash.com/photo-1518684079-3c5fd2451f15?w=480&auto=format&fit=crop&q=80'
  },
  {
    id: 4,
    title: 'Safari & Sea Combo',
    country: 'Kenya',
    regions: 'Maasai Mara, Diani Beach',
    operator: 'Savanna Travel',
    startingDate: '18/11/2026',
    image: 'https://images.unsplash.com/photo-1526779259212-3f6b3044d7e8?w=480&auto=format&fit=crop&q=80'
  }
];

export default function GroupTrip3() {
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isTotalOpen, setIsTotalOpen] = useState(true);

  return (
    <div style={styles.container}>
      <div style={styles.mainContent}>

        <div style={styles.card}>
          <h3 style={styles.cardTitle}>Programs Growth Over Time</h3>
          <div style={{ width: '100%', height: 280, marginTop: '20px' }}>
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={chartData} margin={{ top: 5, right: 20, left: -20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#2d2d2d" vertical={false} />
                <XAxis dataKey="name" stroke="#6b7280" axisLine={false} tickLine={false} tick={{ fontSize: 12 }} dy={10} />
                <YAxis stroke="#6b7280" axisLine={false} tickLine={false} tick={{ fontSize: 12 }} />
                <Tooltip contentStyle={{ backgroundColor: '#1e1e1e', border: '1px solid #333', borderRadius: '8px' }} itemStyle={{ color: '#fff' }} />
                <Legend iconType="plainline" wrapperStyle={{ fontSize: '13px', paddingTop: '10px' }} />
                <Line type="monotone" dataKey="Tourists" stroke="#f59e0b" strokeWidth={2} dot={false} name="Tourists" />
                <Line type="monotone" dataKey="Programs" stroke="#93c5fd" strokeWidth={2} dot={false} name="Programs" />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div style={styles.statsGrid}>
          <div style={styles.statCard}>
            <div style={{ ...styles.iconBox, color: '#93c5fd' }}><Globe size={24} /></div>
            <div style={styles.statInfo}>
              <span style={styles.statLabel}>TOTAL PROGRAMS</span>
              <span style={styles.statValue}>1,250</span>
            </div>
          </div>
          <div style={styles.statCard}>
            <div style={{ ...styles.iconBox, color: '#ef4444' }}><Trash2 size={24} /></div>
            <div style={styles.statInfo}>
              <span style={styles.statLabel}>CANCELLED PROGRAMS</span>
              <span style={styles.statValue}>150</span>
            </div>
          </div>
          <div style={styles.statCard}>
            <div style={{ ...styles.iconBox, color: '#f59e0b' }}><Hourglass size={24} /></div>
            <div style={styles.statInfo}>
              <span style={styles.statLabel}>PENDING PROGRAMS</span>
              <span style={styles.statValue}>45</span>
            </div>
          </div>
        </div>

        <div style={styles.accordionsWrapper}>
          <div style={styles.accordionContainer}>
            <div style={styles.accordionHeader} onClick={() => setIsRejectedOpen(!isRejectedOpen)}>
              <div style={styles.accordionIcon}>{isRejectedOpen ? <ChevronUp size={20} color="#6b7280" /> : <ChevronDown size={20} color="#6b7280" />}</div>
              <span style={styles.accordionTitle}>Rejected Programs</span>
            </div>
            {isRejectedOpen && (
              <div style={styles.accordionContent}>
                {samplePrograms.map((program) => (
                  <TripCard key={`rejected-${program.id}`} trip={program} />
                ))}
              </div>
            )}
          </div>

          <div style={styles.accordionContainer}>
            <div style={styles.accordionHeader} onClick={() => setIsTotalOpen(!isTotalOpen)}>
              <div style={styles.accordionIcon}>{isTotalOpen ? <ChevronUp size={20} color="#6b7280" /> : <ChevronDown size={20} color="#6b7280" />}</div>
              <span style={{ ...styles.accordionTitle, color: '#f59e0b' }}>Total Programs</span>
            </div>
            {isTotalOpen && (
              <div style={styles.accordionContent}>
                {samplePrograms.map((program) => (
                  <TripCard key={`total-${program.id}`} trip={program} />
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

function TripCard({ trip }) {
  return (
    <div style={styles.tripCard}>
      <div style={styles.tripCardTop}>
        <div style={styles.tripCardInfo}>
          <h4 style={styles.tripTitle}>{trip.title}</h4>
          <div style={styles.tripMetaRow}>
            <MapPin size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>{trip.country} • {trip.regions}</span>
          </div>
          <div style={styles.tripMetaRow}>
            <Building2 size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>Operator: {trip.operator}</span>
          </div>
          <div style={styles.tripMetaRow}>
            <CalendarDays size={14} color="#9ca3af" />
            <span style={styles.tripMetaText}>Starting date: <strong style={{ color: '#f8fafc' }}>{trip.startingDate}</strong></span>
          </div>
        </div>
        <img src={trip.image} alt={trip.title} style={styles.tripImage} />
      </div>
      <div style={styles.tripCardActions}>
        <button style={styles.btnReject}>Reject</button>
        <button style={styles.btnApprove}>Approve</button>
      </div>
    </div>
  );
}

const styles = {
  container: {
    padding: '30px',
    backgroundColor: '#121212',
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: '"Inter", "Segoe UI", sans-serif',
    direction: 'ltr',
  },
  mainContent: {
    maxWidth: '900px',
    display: 'flex',
    flexDirection: 'column',
    gap: '24px',
  },
  card: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '24px',
  },
  cardTitle: {
    fontSize: '16px',
    fontWeight: '600',
    color: '#f8fafc',
    margin: 0,
  },
  statsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(3, 1fr)',
    gap: '16px',
  },
  statCard: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
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
    gap: '8px',
  },
  accordionHeader: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '16px 20px',
    display: 'flex',
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
    padding: '8px 0 16px 0',
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))',
    gap: '16px',
  },
  tripCard: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '20px',
    display: 'flex',
    flexDirection: 'column',
    gap: '20px',
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
    backgroundColor: '#2d2d2d',
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
    backgroundColor: '#93c5fd',
    color: '#000000',
    border: 'none',
    padding: '10px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    transition: 'opacity 0.2s',
  },
};
