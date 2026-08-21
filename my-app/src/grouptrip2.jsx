import { useState } from 'react';
import {
  Building2,
  Calendar,
  MapPin,
  Map,
  Trash2,
  Hourglass,
  ChevronDown,
  Check,
  X,
} from 'lucide-react';

export default function GroupTrip() {
  const [isRejectedExpanded, setIsRejectedExpanded] = useState(false);
  const [isCurrentExpanded, setIsCurrentExpanded] = useState(true);

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

  const programs = [];
  const rejectedPrograms = [];
  const pendingPrograms = [];

  const statistics = {
    total: null,
    rejected: null,
    pending: null,
  };

  // --------------------------------------------------
  // Program Card
  // --------------------------------------------------

  const ProgramCard = ({ program }) => {
    return (
      <div className={styles.programCard}>

        {/* Program information */}
        <div className={styles.programInfo}>

          <div className={styles.programDetails}>

            <h4 className={styles.programTitle}>
              {program.title}
            </h4>

            <div className={styles.detailRow}>
              <Building2 size={14} />
              <span>
                Publisher: {program.company}
              </span>
            </div>

            <div className={styles.detailRow}>
              <Calendar size={14} />
              <span>
                Starting Date: {program.startingDate}
              </span>
            </div>

            <div className={styles.detailRow}>
              <MapPin size={14} />
              <span>
                {program.country}
              </span>
            </div>

            <div className={styles.detailRow}>
              <Map size={14} />
              <span>
                {program.regions}
              </span>
            </div>

          </div>

        </div>

        {/* Actions */}
        <div className={styles.programActions}>

          <button
            type="button"
            className={styles.rejectButton}
            onClick={() => {
              // TODO:
              // Call your ASP.NET Core API
              // to reject this program.
            }}
          >
            <X size={16} />
            Reject
          </button>

          <button
            type="button"
            className={styles.approveButton}
            onClick={() => {
              // TODO:
              // Call your ASP.NET Core API
              // to approve this program.
            }}
          >
            <Check size={16} />
            Approve
          </button>

        </div>

      </div>
    );
  };

  // --------------------------------------------------
  // Empty State
  // --------------------------------------------------

  const EmptyState = ({ message }) => {
    return (
      <div style={styles.emptyState}>
        {message}
      </div>
    );
  };

  // --------------------------------------------------
  // Render
  // --------------------------------------------------

  return (
    <div style={styles.container}>

      <div style={styles.dashboardGrid}>

        {/* ==================================================
            LEFT SIDE
        ================================================== */}

        <div style={styles.leftColumn}>

          {/* Statistics */}
          <div style={styles.statisticsCard}>

            <h3 style={styles.sectionTitle}>
              Program Statistics
            </h3>

            <div style={styles.statisticsGrid}>

              {/* Total */}
              <div style={styles.statCard}>

                <Building2
                  size={32}
                  style={styles.blueIcon}
                />

                <div>
                  <p style={styles.statLabel}>
                    TOTAL PROGRAMS
                  </p>

                  <p style={styles.statValue}>
                    {statistics.total ?? '—'}
                  </p>
                </div>

              </div>

              {/* Rejected */}
              <div style={styles.statCard}>

                <Trash2
                  size={32}
                  style={styles.redIcon}
                />

                <div>
                  <p style={styles.statLabel}>
                    REJECTED PROGRAMS
                  </p>

                  <p
                    style={{
                      ...styles.statValue,
                      color: '#f87171',
                    }}
                  >
                    {statistics.rejected ?? '—'}
                  </p>
                </div>

              </div>

              {/* Pending */}
              <div style={styles.statCard}>

                <Hourglass
                  size={32}
                  style={styles.orangeIcon}
                />

                <div>
                  <p style={styles.statLabel}>
                    PENDING PROGRAMS
                  </p>

                  <p
                    style={{
                      ...styles.statValue,
                      color: '#F4A261',
                    }}
                  >
                    {statistics.pending ?? '—'}
                  </p>
                </div>

              </div>

            </div>

          </div>

          {/* ==================================================
              REJECTED PROGRAMS
          ================================================== */}

          <div style={styles.accordion}>

            <button
              type="button"
              onClick={() =>
                setIsRejectedExpanded(!isRejectedExpanded)
              }
              style={styles.accordionHeader}
            >

              <div style={styles.accordionHeaderContent}>

                <span style={styles.accordionTitle}>
                  Rejected Programs
                </span>

                <ChevronDown
                  size={20}
                  style={{
                    transform: isRejectedExpanded
                      ? 'rotate(180deg)'
                      : 'rotate(0deg)',
                    transition: 'transform 0.2s ease',
                  }}
                />

              </div>

            </button>

            {isRejectedExpanded && (
              <div style={styles.accordionContent}>

                {rejectedPrograms.length === 0 ? (
                  <EmptyState
                    message="No rejected programs found."
                  />
                ) : (
                  rejectedPrograms.map((program) => (
                    <ProgramCard
                      key={program.id}
                      program={program}
                    />
                  ))
                )}

              </div>
            )}

          </div>

          {/* ==================================================
              ALL PROGRAMS
          ================================================== */}

          <div style={styles.accordion}>

            <button
              type="button"
              onClick={() =>
                setIsCurrentExpanded(!isCurrentExpanded)
              }
              style={styles.accordionHeader}
            >

              <div style={styles.accordionHeaderContent}>

                <span
                  style={{
                    ...styles.accordionTitle,
                    color: '#F4A261',
                  }}
                >
                  All Programs
                </span>

                <ChevronDown
                  size={20}
                  style={{
                    transform: isCurrentExpanded
                      ? 'rotate(180deg)'
                      : 'rotate(0deg)',
                    transition: 'transform 0.2s ease',
                  }}
                />

              </div>

            </button>

            {isCurrentExpanded && (
              <div style={styles.accordionContent}>

                {programs.length === 0 ? (
                  <EmptyState
                    message="No programs found."
                  />
                ) : (
                  programs.map((program) => (
                    <ProgramCard
                      key={program.id}
                      program={program}
                    />
                  ))
                )}

              </div>
            )}

          </div>

        </div>

        {/* ==================================================
            RIGHT SIDE - PENDING PROGRAMS
        ================================================== */}

        <div style={styles.rightColumn}>

          <div style={styles.pendingCard}>

            <h3 style={styles.pendingTitle}>
              Pending Programs
            </h3>

            <div style={styles.pendingContent}>

              {pendingPrograms.length === 0 ? (
                <EmptyState
                  message="No pending programs found."
                />
              ) : (
                pendingPrograms.map((program) => (
                  <ProgramCard
                    key={program.id}
                    program={program}
                  />
                ))
              )}

            </div>

          </div>

        </div>

      </div>

    </div>
  );
}

// ======================================================
// Styles
// ======================================================

const styles = {

  container: {
    width: '100%',
    minHeight: '100vh',
    padding: '32px',
    backgroundColor: '#121212',
    color: '#ffffff',
    fontFamily:
      'Segoe UI, Tahoma, Geneva, Verdana, sans-serif',
    direction: 'ltr',
  },

  dashboardGrid: {
    display: 'grid',
    gridTemplateColumns:
      'minmax(0, 7fr) minmax(320px, 5fr)',
    gap: '32px',
    width: '100%',
  },

  leftColumn: {
    display: 'flex',
    flexDirection: 'column',
    gap: '24px',
    minWidth: 0,
  },

  rightColumn: {
    minWidth: 0,
  },

  // ====================================================
  // Statistics
  // ====================================================

  statisticsCard: {
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    padding: '24px',
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.2)',
  },

  sectionTitle: {
    margin: '0 0 20px 0',
    fontSize: '18px',
    fontWeight: '600',
    color: '#ffffff',
    textAlign: 'left',
  },

  statisticsGrid: {
    display: 'grid',
    gridTemplateColumns:
      'repeat(3, minmax(0, 1fr))',
    gap: '16px',
  },

  statCard: {
    display: 'flex',
    alignItems: 'center',
    gap: '16px',
    backgroundColor: '#121212',
    border: '1px solid #2a2a2a',
    borderRadius: '12px',
    padding: '16px',
  },

  blueIcon: {
    color: '#91B3FA',
    flexShrink: 0,
  },

  redIcon: {
    color: '#f87171',
    flexShrink: 0,
  },

  orangeIcon: {
    color: '#F4A261',
    flexShrink: 0,
  },

  statLabel: {
    margin: 0,
    fontSize: '10px',
    fontWeight: '700',
    letterSpacing: '0.08em',
    color: '#6b7280',
  },

  statValue: {
    margin: '4px 0 0 0',
    fontSize: '22px',
    fontWeight: '700',
    color: '#ffffff',
  },

  // ====================================================
  // Accordion
  // ====================================================

  accordion: {
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    overflow: 'hidden',
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.2)',
  },

  accordionHeader: {
    width: '100%',
    padding: '20px',
    border: 'none',
    backgroundColor: '#202022',
    color: '#ffffff',
    cursor: 'pointer',
    textAlign: 'left',
  },

  accordionHeaderContent: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    width: '100%',
    direction: 'ltr',
  },

  accordionTitle: {
    fontSize: '16px',
    fontWeight: '600',
    color: '#ffffff',
  },

  accordionContent: {
    padding: '20px',
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
    maxHeight: '450px',
    overflowY: 'auto',
    borderTop: '1px solid #333333',
    direction: 'ltr',
  },

  // ====================================================
  // Program Card
  // ====================================================

  programCard: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: '24px',
    padding: '20px',
    backgroundColor: '#18181A',
    border: '1px solid #333333',
    borderRadius: '16px',
    direction: 'ltr',
  },

  programInfo: {
    flex: 1,
    minWidth: 0,
  },

  programDetails: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    alignItems: 'flex-start',
  },

  programTitle: {
    margin: 0,
    fontSize: '16px',
    fontWeight: '700',
    color: '#ffffff',
  },

  detailRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    fontSize: '13px',
    color: '#9ca3af',
    textAlign: 'left',
  },

  programActions: {
    display: 'flex',
    gap: '10px',
    flexShrink: 0,
  },

  approveButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '8px',
    padding: '10px 18px',
    border: 'none',
    borderRadius: '10px',
    backgroundColor: '#91B3FA',
    color: '#000000',
    fontWeight: '600',
    cursor: 'pointer',
  },

  rejectButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '8px',
    padding: '10px 18px',
    border: '1px solid #333333',
    borderRadius: '10px',
    backgroundColor: '#2A2A2D',
    color: '#d1d5db',
    fontWeight: '600',
    cursor: 'pointer',
  },

  // ====================================================
  // Pending Programs
  // ====================================================

  pendingCard: {
    backgroundColor: '#1C1C1E',
    border: '1px solid rgba(212, 175, 55, 0.3)',
    borderRadius: '16px',
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.2)',
    padding: '24px',
    minHeight: '100%',
    display: 'flex',
    flexDirection: 'column',
  },

  pendingTitle: {
    margin: '0 0 20px 0',
    paddingBottom: '16px',
    borderBottom: '1px solid #333333',
    fontSize: '18px',
    fontWeight: '600',
    color: '#ffffff',
    textAlign: 'left',
  },

  pendingContent: {
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
    overflowY: 'auto',
    flex: 1,
    direction: 'ltr',
  },

  // ====================================================
  // Empty State
  // ====================================================

  emptyState: {
    padding: '40px 20px',
    textAlign: 'center',
    color: '#6b7280',
    fontSize: '14px',
    border: '1px dashed #333333',
    borderRadius: '12px',
    direction: 'ltr',
  },
};