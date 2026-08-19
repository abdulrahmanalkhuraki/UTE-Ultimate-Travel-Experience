import  { useState } from 'react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { ChevronDown, ChevronUp, MapPin, Building2, Eye, Check, X, ClipboardList, CalendarX, Clock } from 'lucide-react';

// بيانات وهمية للمخطط البياني (توضح تزايد السياح والشركات)
const chartData = [
  { name: 'Jan', Tourists: 400, Companies: 240 },
  { name: 'Feb', Tourists: 700, Companies: 360 },
  { name: 'Mar', Tourists: 1200, Companies: 500 },
  { name: 'Apr', Tourists: 1500, Companies: 750 },
  { name: 'May', Tourists: 2100, Companies: 900 },
  { name: 'Jun', Tourists: 2900, Companies: 1100 },
];

// بيانات وهمية لبطاقات البرامج الجماعية
const sampleTrips = [
  {
    id: 1,
    title: "سحر الشرق في اسطنبول",
    country: "تركيا",
    regions: "اسطنبول، بورصة، سبانجا",
    company: "شركة رحلتي المتميزة",
    requests: 14,
    image: "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?w=150&auto=format&fit=crop&q=60"
  },
  {
    id: 2,
    title: "أضواء باريس الكلاسيكية",
    country: "فرنسا",
    regions: "باريس، قصر فرساي",
    company: "أفخم الأسفار",
    requests: 8,
    image: "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=150&auto=format&fit=crop&q=60"
  }
];

export default function GroupTrip() {
  // حالات التحكم بتمدد المستطيلات (Accordion)
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isAllOpen, setIsAllOpen] = useState(true);

  return (
    <div style={styles.container}>
      
      {/* 1. قسم المخطط البياني */}
      <div style={styles.sectionCard}>
        <h3 style={styles.sectionTitle}>مخطط تزايد السياح والشركات المُنضمّة</h3>
        <div style={{ width: '100%', height: 300 }}>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={chartData} margin={{ top: 20, right: 30, left: 0, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
              <XAxis dataKey="name" stroke="#94a3b8" />
              <YAxis stroke="#94a3b8" />
              <Tooltip contentStyle={{ backgroundColor: '#1e293b', border: 'none', borderRadius: '8px', color: '#fff' }} />
              <Bar dataKey="Tourists" fill="#3b82f6" radius={[4, 4, 0, 0]} name="عدد السياح" />
              <Bar dataKey="Companies" fill="#10b981" radius={[4, 4, 0, 0]} name="الشركات" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* 2. قسم إحصائيات البرامج (تحت المخطط مباشرة) */}
      <div style={styles.statsGrid}>
        <div style={styles.statCard}>
          <div style={{ ...styles.iconWrapper, backgroundColor: 'rgba(59, 130, 246, 0.15)' }}>
            <ClipboardList color="#3b82f6" size={24} />
          </div>
          <div>
            <p style={styles.statLabel}>البرامج الكلية</p>
            <h4 style={styles.statValue}>148</h4>
          </div>
        </div>

        <div style={styles.statCard}>
          <div style={{ ...styles.iconWrapper, backgroundColor: 'rgba(239, 68, 68, 0.15)' }}>
            <CalendarX color="#ef4444" size={24} />
          </div>
          <div>
            <p style={styles.statLabel}>البرامج الملغاة</p>
            <h4 style={styles.statValue}>12</h4>
          </div>
        </div>

        <div style={styles.statCard}>
          <div style={{ ...styles.iconWrapper, backgroundColor: 'rgba(245, 158, 11, 0.15)' }}>
            <Clock color="#f59e0b" size={24} />
          </div>
          <div>
            <p style={styles.statLabel}>في حالة الانتظار</p>
            <h4 style={styles.statValue}>27</h4>
          </div>
        </div>
      </div>

      {/* 3. المستطيلات القابلة للتمدد (Accordions) */}
      <div style={styles.accordionContainer}>
        
        {/* مستطيل البرامج المرفوضة */}
        <div style={styles.accordionHeader} onClick={() => setIsRejectedOpen(!isRejectedOpen)}>
          <span style={styles.accordionTitle}>البرامج المرفوضة</span>
          {isRejectedOpen ? <ChevronUp color="#94a3b8" /> : <ChevronDown color="#94a3b8" />}
        </div>
        
        {isRejectedOpen && (
          <div style={styles.accordionContent}>
            {sampleTrips.map((trip) => (
              <div key={trip.id} style={styles.tripCard}>
                <div style={styles.tripInfoSide}>
                  <img src={trip.image} alt={trip.title} style={styles.tripImage} />
                  <div style={styles.tripDetails}>
                    <h5 style={styles.tripTitleText}>{trip.title}</h5>
                    <div style={styles.detailRow}>
                      <MapPin size={14} color="#94a3b8" style={{ marginLeft: 4 }} />
                      <span>{trip.country} ({trip.regions})</span>
                    </div>
                    <div style={styles.detailRow}>
                      <Building2 size={14} color="#94a3b8" style={{ marginLeft: 4 }} />
                      <span>الشركة: {trip.company}</span>
                    </div>
                  </div>
                </div>

                <div style={styles.actionSide}>
                  <div style={styles.requestBadge}>
                    <Eye size={14} style={{ marginLeft: 4 }} />
                    الطلبات: {trip.requests}
                  </div>
                  <div style={styles.btnGroup}>
                    <button style={styles.btnAccept} title="قبول"><Check size={16} /></button>
                    <button style={styles.btnReject} title="رفض"><X size={16} /></button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* مستطيل البرامج الكلية */}
        <div style={{ ...styles.accordionHeader, marginTop: '16px' }} onClick={() => setIsAllOpen(!isAllOpen)}>
          <span style={styles.accordionTitle}>البرامج الكلية</span>
          {isAllOpen ? <ChevronUp color="#94a3b8" /> : <ChevronDown color="#94a3b8" />}
        </div>

        {isAllOpen && (
          <div style={styles.accordionContent}>
            {sampleTrips.slice().reverse().map((trip) => (
              <div key={trip.id} style={styles.tripCard}>
                <div style={styles.tripInfoSide}>
                  <img src={trip.image} alt={trip.title} style={styles.tripImage} />
                  <div style={styles.tripDetails}>
                    <h5 style={styles.tripTitleText}>{trip.title}</h5>
                    <div style={styles.detailRow}>
                      <MapPin size={14} color="#94a3b8" style={{ marginLeft: 4 }} />
                      <span>{trip.country} ({trip.regions})</span>
                    </div>
                    <div style={styles.detailRow}>
                      <Building2 size={14} color="#94a3b8" style={{ marginLeft: 4 }} />
                      <span>الشركة: {trip.company}</span>
                    </div>
                  </div>
                </div>

                <div style={styles.actionSide}>
                  <div style={styles.requestBadge}>
                    <Eye size={14} style={{ marginLeft: 4 }} />
                    الطلبات: {trip.requests}
                  </div>
                  <div style={styles.btnGroup}>
                    <button style={styles.btnAccept} title="قبول"><Check size={16} /></button>
                    <button style={styles.btnReject} title="رفض"><X size={16} /></button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

      </div>
    </div>
  );
}

// تصميم الواجهة متناسق وبخلفية موحدة (Dark theme أنيق ومريح متناسق مع الداشبورد)
const styles = {
  container: {
    padding: '24px',
    backgroundColor: '#0f172a', // لون خلفية موحد كامل
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: 'Segoe UI, Tahoma, Geneva, Verdana, sans-serif',
    direction: 'rtl', // اتجاه عربي متناسق
  },
  sectionCard: {
    backgroundColor: '#1e293b',
    borderRadius: '12px',
    padding: '20px',
    marginBottom: '24px',
    boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
  },
  sectionTitle: {
    fontSize: '18px',
    fontWeight: '600',
    marginBottom: '16px',
    color: '#f1f5f9',
  },
  statsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))',
    gap: '16px',
    marginBottom: '24px',
  },
  statCard: {
    backgroundColor: '#1e293b',
    borderRadius: '12px',
    padding: '16px',
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
    boxShadow: '0 2px 4px rgba(0,0,0,0.05)',
  },
  iconWrapper: {
    padding: '12px',
    borderRadius: '10px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  statLabel: {
    fontSize: '14px',
    color: '#94a3b8',
    margin: 0,
  },
  statValue: {
    fontSize: '22px',
    fontWeight: '700',
    margin: '4px 0 0 0',
    color: '#ffffff',
  },
  accordionContainer: {
    marginTop: '24px',
  },
  accordionHeader: {
    backgroundColor: '#1e293b',
    padding: '16px 20px',
    borderRadius: '10px',
    display: 'flex',
    justifyContent: 'between',
    alignItems: 'center',
    cursor: 'pointer',
    userSelect: 'none',
    borderRight: '4px solid #3b82f6',
  },
  accordionTitle: {
    fontSize: '16px',
    fontWeight: '600',
    flexGrow: 1,
    textAlign: 'right',
  },
  accordionContent: {
    backgroundColor: '#111827',
    padding: '16px',
    borderRadius: '0 0 10px 10px',
    border: '1px solid #1e293b',
    borderTop: 'none',
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
  },
  tripCard: {
    backgroundColor: '#1e293b',
    borderRadius: '8px',
    padding: '12px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    flexWrap: 'wrap',
    gap: '16px',
  },
  tripInfoSide: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
  },
  tripImage: {
    width: '80px',
    height: '80px',
    borderRadius: '6px',
    objectFit: 'cover',
  },
  tripDetails: {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
  },
  tripTitleText: {
    fontSize: '15px',
    fontWeight: '600',
    margin: 0,
    color: '#f8fafc',
  },
  detailRow: {
    display: 'flex',
    alignItems: 'center',
    fontSize: '13px',
    color: '#94a3b8',
  },
  actionSide: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
  },
  requestBadge: {
    backgroundColor: 'rgba(59, 130, 246, 0.1)',
    color: '#3b82f6',
    padding: '6px 12px',
    borderRadius: '20px',
    fontSize: '13px',
    display: 'flex',
    alignItems: 'center',
  },
  btnGroup: {
    display: 'flex',
    gap: '8px',
  },
  btnAccept: {
    backgroundColor: '#10b981',
    border: 'none',
    color: 'white',
    padding: '8px',
    borderRadius: '6px',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    transition: 'background 0.2s',
  },
  btnReject: {
    backgroundColor: '#ef4444',
    border: 'none',
    color: 'white',
    padding: '8px',
    borderRadius: '6px',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    transition: 'background 0.2s',
  }
};