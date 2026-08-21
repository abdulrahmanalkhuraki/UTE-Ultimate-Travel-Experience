import { useState } from 'react';
import {
  ChevronDown,
  ChevronUp,
  Globe,
  Trash2,
  Hourglass,
} from 'lucide-react';

export default function GroupTrip3() {
  const [isRejectedOpen, setIsRejectedOpen] = useState(false);
  const [isTotalOpen, setIsTotalOpen] = useState(true);

  /*
   * Real data should come from your ASP.NET Core API.
   *
   * Example:
   *
   * const [programs, setPrograms] = useState([]);
   *
   * useEffect(() => {
   *   fetch('/api/tour-programs')
   *     .then(response => response.json())
   *     .then(data => setPrograms(data));
   * }, []);
   */

  const statistics = {
    total: null,
    rejected: null,
    pending: null,
  };

  return (
    <div style={styles.container}>
      <div style={styles.mainContent}>

        {/* ============================================
            PROGRAM STATISTICS
        ============================================ */}

        <div style={styles.statsGrid}>

          {/* Total Programs */}
          <div style={styles.statCard}>
            <div
              style={{
                ...styles.iconBox,
                color: '#93c5fd',
              }}
            >
              <Globe size={24} />
            </div>

            <div style={styles.statInfo}>
              <span style={styles.statLabel}>
                TOTAL PROGRAMS
              </span>

              <span style={styles.statValue}>
                {statistics.total ?? '—'}
              </span>
            </div>
          </div>

          {/* Rejected Programs */}
          <div style={styles.statCard}>
            <div
              style={{
                ...styles.iconBox,
                color: '#ef4444',
              }}
            >
              <Trash2 size={24} />
            </div>

            <div style={styles.statInfo}>
              <span style={styles.statLabel}>
                REJECTED PROGRAMS
              </span>

              <span
                style={{
                  ...styles.statValue,
                  color: '#ef4444',
                }}
              >
                {statistics.rejected ?? '—'}
              </span>
            </div>
          </div>

          {/* Pending Programs */}
          <div style={styles.statCard}>
            <div
              style={{
                ...styles.iconBox,
                color: '#f59e0b',
              }}
            >
              <Hourglass size={24} />
            </div>

            <div style={styles.statInfo}>
              <span style={styles.statLabel}>
                PENDING PROGRAMS
              </span>

              <span
                style={{
                  ...styles.statValue,
                  color: '#f59e0b',
                }}
              >
                {statistics.pending ?? '—'}
              </span>
            </div>
          </div>

        </div>

        {/* ============================================
            ACCORDIONS
        ============================================ */}

        <div style={styles.accordionsWrapper}>

          {/* ==========================================
              REJECTED PROGRAMS
          ========================================== */}

          <div style={styles.accordionContainer}>

            <button
              type="button"
              style={styles.accordionHeader}
              onClick={() =>
                setIsRejectedOpen(!isRejectedOpen)
              }
            >
              <div style={styles.accordionHeaderContent}>

                <span style={styles.accordionTitle}>
                  Rejected Programs
                </span>

                {isRejectedOpen ? (
                  <ChevronUp
                    size={20}
                    color="#6b7280"
                  />
                ) : (
                  <ChevronDown
                    size={20}
                    color="#6b7280"
                  />
                )}

              </div>
            </button>

            {isRejectedOpen && (
              <div style={styles.accordionContent}>
                <div style={styles.emptyState}>
                  No rejected programs found.
                </div>
              </div>
            )}

          </div>

          {/* ==========================================
              TOTAL PROGRAMS
          ========================================== */}

          <div style={styles.accordionContainer}>

            <button
              type="button"
              style={styles.accordionHeader}
              onClick={() =>
                setIsTotalOpen(!isTotalOpen)
              }
            >
              <div style={styles.accordionHeaderContent}>

                <span
                  style={{
                    ...styles.accordionTitle,
                    color: '#f59e0b',
                  }}
                >
                  Total Programs
                </span>

                {isTotalOpen ? (
                  <ChevronUp
                    size={20}
                    color="#6b7280"
                  />
                ) : (
                  <ChevronDown
                    size={20}
                    color="#6b7280"
                  />
                )}

              </div>
            </button>

            {isTotalOpen && (
              <div style={styles.accordionContent}>
                <div style={styles.emptyState}>
                  No programs found.
                </div>
              </div>
            )}

          </div>

        </div>

      </div>
    </div>
  );
}

// ======================================================
// STYLES
// ======================================================

const styles = {
  container: {
    width: '100%',
    minHeight: '100vh',
    padding: '30px',
    backgroundColor: '#121212',
    color: '#f8fafc',
    fontFamily: '"Inter", "Segoe UI", sans-serif',
    direction: 'ltr',
    boxSizing: 'border-box',
  },

  mainContent: {
    width: '100%',
    maxWidth: '900px',
    display: 'flex',
    flexDirection: 'column',
    gap: '24px',
  },

  // ====================================================
  // STATISTICS
  // ====================================================

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
    flexShrink: 0,
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

  // ====================================================
  // ACCORDIONS
  // ====================================================

  accordionsWrapper: {
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
  },

  accordionContainer: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    direction: 'ltr',
  },

  accordionHeader: {
    width: '100%',
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '16px 20px',
    display: 'flex',
    alignItems: 'center',
    cursor: 'pointer',
    userSelect: 'none',
    direction: 'ltr',
    textAlign: 'left',
    color: '#ffffff',
  },

  accordionHeaderContent: {
    width: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    direction: 'ltr',
  },

  accordionTitle: {
    fontSize: '14px',
    fontWeight: '600',
    color: '#f8fafc',
  },

  accordionContent: {
    padding: '8px 0 16px 0',
    display: 'grid',
    gridTemplateColumns:
      'repeat(auto-fill, minmax(350px, 1fr))',
    gap: '16px',
    direction: 'ltr',
  },

  // ====================================================
  // EMPTY STATE
  // ====================================================

  emptyState: {
    width: '100%',
    boxSizing: 'border-box',
    padding: '32px 20px',
    textAlign: 'center',
    color: '#6b7280',
    fontSize: '13px',
    border: '1px dashed #333333',
    borderRadius: '10px',
    gridColumn: '1 / -1',
  },
};