import { useState } from 'react';
import {
  ChevronDown,
  ChevronUp,
  MapPin,
  Building2,
  Eye,
  Check,
  X,
  ClipboardList,
  CalendarX,
  Clock
} from 'lucide-react';

export default function GroupTrip() {
  // Accordion states
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isAllOpen, setIsAllOpen] = useState(true);

  return (
    <div style={styles.container}>

      {/* 1. Program statistics */}
      <div style={styles.statsGrid}>

        <div style={styles.statCard}>
          <div
            style={{
              ...styles.iconWrapper,
              backgroundColor: 'rgba(59, 130, 246, 0.15)'
            }}
          >
            <ClipboardList color="#3b82f6" size={24} />
          </div>

          <div>
            <p style={styles.statLabel}>البرامج الكلية</p>
            <h4 style={styles.statValue}>—</h4>
          </div>
        </div>

        <div style={styles.statCard}>
          <div
            style={{
              ...styles.iconWrapper,
              backgroundColor: 'rgba(239, 68, 68, 0.15)'
            }}
          >
            <CalendarX color="#ef4444" size={24} />
          </div>

          <div>
            <p style={styles.statLabel}>البرامج الملغاة</p>
            <h4 style={styles.statValue}>—</h4>
          </div>
        </div>

        <div style={styles.statCard}>
          <div
            style={{
              ...styles.iconWrapper,
              backgroundColor: 'rgba(245, 158, 11, 0.15)'
            }}
          >
            <Clock color="#f59e0b" size={24} />
          </div>

          <div>
            <p style={styles.statLabel}>في حالة الانتظار</p>
            <h4 style={styles.statValue}>—</h4>
          </div>
        </div>

      </div>

      {/* 2. Tour package accordions */}
      <div style={styles.accordionContainer}>

        {/* Rejected tour packages */}
        <div
          style={styles.accordionHeader}
          onClick={() => setIsRejectedOpen(!isRejectedOpen)}
        >
          <span style={styles.accordionTitle}>
            البرامج المرفوضة
          </span>

          {isRejectedOpen ? (
            <ChevronUp color="#94a3b8" />
          ) : (
            <ChevronDown color="#94a3b8" />
          )}
        </div>

        {isRejectedOpen && (
          <div style={styles.accordionContent}>
            <div style={styles.emptyState}>
              لا توجد بيانات برامج مرفوضة.
            </div>
          </div>
        )}

        {/* All tour packages */}
        <div
          style={{
            ...styles.accordionHeader,
            marginTop: '16px'
          }}
          onClick={() => setIsAllOpen(!isAllOpen)}
        >
          <span style={styles.accordionTitle}>
            البرامج الكلية
          </span>

          {isAllOpen ? (
            <ChevronUp color="#94a3b8" />
          ) : (
            <ChevronDown color="#94a3b8" />
          )}
        </div>

        {isAllOpen && (
          <div style={styles.accordionContent}>
            <div style={styles.emptyState}>
              لا توجد بيانات برامج سياحية.
            </div>
          </div>
        )}

      </div>
    </div>
  );
}

const styles = {
  container: {
    padding: '24px',
    backgroundColor: '#0f172a',
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: 'Segoe UI, Tahoma, Geneva, Verdana, sans-serif',

    // LTR for the tour package section
    direction: 'ltr',
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

    // Force the accordion itself to LTR
    direction: 'ltr',
  },

  accordionHeader: {
    backgroundColor: '#1e293b',
    padding: '16px 20px',
    borderRadius: '10px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    cursor: 'pointer',
    userSelect: 'none',
    borderLeft: '4px solid #3b82f6',

    // LTR
    direction: 'ltr',
  },

  accordionTitle: {
    fontSize: '16px',
    fontWeight: '600',
    flexGrow: 1,
    textAlign: 'left',

    // LTR
    direction: 'ltr',
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

    // LTR
    direction: 'ltr',
  },

  emptyState: {
    padding: '24px',
    textAlign: 'center',
    color: '#94a3b8',
    direction: 'ltr',
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
    direction: 'ltr',
  },

  tripInfoSide: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
    direction: 'ltr',
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
    direction: 'ltr',
  },

  tripTitleText: {
    fontSize: '15px',
    fontWeight: '600',
    margin: 0,
    color: '#f8fafc',
    textAlign: 'left',
  },

  detailRow: {
    display: 'flex',
    alignItems: 'center',
    fontSize: '13px',
    color: '#94a3b8',
    direction: 'ltr',
  },

  actionSide: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
    direction: 'ltr',
  },

  requestBadge: {
    backgroundColor: 'rgba(59, 130, 246, 0.1)',
    color: '#3b82f6',
    padding: '6px 12px',
    borderRadius: '20px',
    fontSize: '13px',
    display: 'flex',
    alignItems: 'center',
    direction: 'ltr',
  },

  btnGroup: {
    display: 'flex',
    gap: '8px',
    direction: 'ltr',
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
  },
};