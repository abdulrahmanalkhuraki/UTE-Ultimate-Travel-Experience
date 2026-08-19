import  { useState } from 'react';
import { 
  Bell, 
  Send, 
  Building2, 
  Map, 
  Users, 
  Megaphone,
  Check,
  X,
  Clock
} from 'lucide-react';

// بيانات وهمية للإشعارات الواردة (طلبات الشركات والبرامج)
const initialAlerts = [
  {
    id: 1,
    type: 'company_join',
    title: 'New Company Registration',
    message: 'Oceanic Ventures has requested to join as a tourism company.',
    time: '10 mins ago',
    icon: <Building2 size={20} color="#93c5fd" />
  },
  {
    id: 2,
    type: 'new_program',
    title: 'New Trip Program Submitted',
    message: 'Elite Journeys submitted "Magic of the East" program for approval.',
    time: '2 hours ago',
    icon: <Map size={20} color="#f59e0b" />
  },
  {
    id: 3,
    type: 'company_join',
    title: 'New Company Registration',
    message: 'Alpine Adventures wants to create a publisher account.',
    time: 'Yesterday',
    icon: <Building2 size={20} color="#93c5fd" />
  }
];

export default function Notifications() {
  const [alerts, setAlerts] = useState(initialAlerts);
  
  // حالات نموذج إرسال الإشعارات
  const [targetAudience, setTargetAudience] = useState('all'); // all, tourists, companies
  const [notifyTitle, setNotifyTitle] = useState('');
  const [notifyMessage, setNotifyMessage] = useState('');

  const handleSendNotification = (e) => {
    e.preventDefault();
    // هنا يتم ربط الـ API لإرسال الإشعار
    console.log("Sending to:", targetAudience);
    console.log("Title:", notifyTitle, "Message:", notifyMessage);
    
    // تفريغ الحقول بعد الإرسال
    setNotifyTitle('');
    setNotifyMessage('');
    alert('Notification sent successfully!');
  };

  const removeAlert = (id) => {
    setAlerts(alerts.filter(alert => alert.id !== id));
  };

  return (
    <div style={styles.container}>
      <div style={styles.pageHeader}>
        <Bell size={24} color="#f8fafc" />
        <h2 style={styles.pageTitle}>Notifications Center</h2>
      </div>

      <div style={styles.layout}>
        
        {/* القسم الأول: الإشعارات الواردة والطلبات (Left Column) */}
        <div style={styles.column}>
          <div style={styles.card}>
            <div style={styles.cardHeader}>
              <h3 style={styles.cardTitle}>Incoming Requests & Alerts</h3>
              <span style={styles.badge}>{alerts.length} New</span>
            </div>

            <div style={styles.alertsList}>
              {alerts.length > 0 ? (
                alerts.map((alert) => (
                  <div key={alert.id} style={styles.alertItem}>
                    <div style={styles.alertIconBox}>
                      {alert.icon}
                    </div>
                    <div style={styles.alertContent}>
                      <div style={styles.alertTopRow}>
                        <h4 style={styles.alertItemTitle}>{alert.title}</h4>
                        <span style={styles.alertTime}>
                          <Clock size={12} style={{marginRight: '4px'}} />
                          {alert.time}
                        </span>
                      </div>
                      <p style={styles.alertMessage}>{alert.message}</p>
                      {/* <div style={styles.alertActions}>
                        <button style={styles.btnApprove} onClick={() => removeAlert(alert.id)}>
                          <Check size={16} style={{marginRight: '6px'}} /> Approve
                        </button>
                        <button style={styles.btnReject} onClick={() => removeAlert(alert.id)}>
                          <X size={16} style={{marginRight: '6px'}} /> Reject
                        </button>
                      </div> */}
                    </div>
                  </div>
                ))
              ) : (
                <div style={styles.emptyState}>
                  <Bell size={32} color="#6b7280" style={{marginBottom: '12px'}} />
                  <p>No new alerts at the moment.</p>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* القسم الثاني: إرسال إشعار للمستخدمين (Right Column) */}
        <div style={styles.column}>
          <div style={styles.card}>
            <div style={styles.cardHeader}>
              <h3 style={styles.cardTitle}>Broadcast Notification</h3>
              <Megaphone size={20} color="#9ca3af" />
            </div>

            <form style={styles.formContainer} onSubmit={handleSendNotification}>
              
              {/* اختيار الفئة المستهدفة */}
              <div style={styles.formGroup}>
                <label style={styles.inputLabel}>Target Audience</label>
                <div style={styles.radioGroup}>
                  <label style={{...styles.radioOption, borderColor: targetAudience === 'all' ? '#93c5fd' : '#2d2d2d'}}>
                    <input 
                      type="radio" 
                      name="audience" 
                      value="all" 
                      checked={targetAudience === 'all'}
                      onChange={(e) => setTargetAudience(e.target.value)}
                      style={{display: 'none'}}
                    />
                    <Users size={18} color={targetAudience === 'all' ? '#93c5fd' : '#9ca3af'} />
                    <span style={{color: targetAudience === 'all' ? '#93c5fd' : '#d1d5db'}}>All Users</span>
                  </label>

                  <label style={{...styles.radioOption, borderColor: targetAudience === 'tourists' ? '#93c5fd' : '#2d2d2d'}}>
                    <input 
                      type="radio" 
                      name="audience" 
                      value="tourists"
                      checked={targetAudience === 'tourists'}
                      onChange={(e) => setTargetAudience(e.target.value)}
                      style={{display: 'none'}}
                    />
                    <Map size={18} color={targetAudience === 'tourists' ? '#93c5fd' : '#9ca3af'} />
                    <span style={{color: targetAudience === 'tourists' ? '#93c5fd' : '#d1d5db'}}>Tourists Only</span>
                  </label>

                  <label style={{...styles.radioOption, borderColor: targetAudience === 'companies' ? '#93c5fd' : '#2d2d2d'}}>
                    <input 
                      type="radio" 
                      name="audience" 
                      value="companies"
                      checked={targetAudience === 'companies'}
                      onChange={(e) => setTargetAudience(e.target.value)}
                      style={{display: 'none'}}
                    />
                    <Building2 size={18} color={targetAudience === 'companies' ? '#93c5fd' : '#9ca3af'} />
                    <span style={{color: targetAudience === 'companies' ? '#93c5fd' : '#d1d5db'}}>Companies Only</span>
                  </label>
                </div>
              </div>

              {/* عنوان الإشعار */}
              <div style={styles.formGroup}>
                <label style={styles.inputLabel}>Notification Title</label>
                <input 
                  type="text" 
                  placeholder="e.g., System Maintenance Update" 
                  style={styles.textInput}
                  value={notifyTitle}
                  onChange={(e) => setNotifyTitle(e.target.value)}
                  required
                />
              </div>

              {/* محتوى الإشعار */}
              <div style={styles.formGroup}>
                <label style={styles.inputLabel}>Notification Message</label>
                <textarea 
                  placeholder="Type the message you want to broadcast..." 
                  style={styles.textArea}
                  rows={5}
                  value={notifyMessage}
                  onChange={(e) => setNotifyMessage(e.target.value)}
                  required
                />
              </div>

              {/* زر الإرسال */}
              <button type="submit" style={styles.btnSend}>
                <Send size={18} />
                <span>Send Notification</span>
              </button>

            </form>
          </div>
        </div>

      </div>
    </div>
  );
}

// الأنماط المتناسقة مع الـ Dark Theme المعتمد مسبقاً
const styles = {
  container: {
    padding: '30px',
    backgroundColor: '#121212',
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: '"Inter", "Segoe UI", sans-serif',
    direction: 'ltr',
  },
  pageHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    marginBottom: '24px',
  },
  pageTitle: {
    fontSize: '22px',
    fontWeight: '600',
    margin: 0,
  },
  layout: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(400px, 1fr))',
    gap: '24px',
    maxWidth: '1200px',
  },
  column: {
    display: 'flex',
    flexDirection: 'column',
  },
  card: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    display: 'flex',
    flexDirection: 'column',
    height: '100%',
  },
  cardHeader: {
    padding: '20px 24px',
    borderBottom: '1px solid #2d2d2d',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  cardTitle: {
    fontSize: '16px',
    fontWeight: '600',
    color: '#f8fafc',
    margin: 0,
  },
  badge: {
    backgroundColor: 'rgba(239, 68, 68, 0.15)',
    color: '#ef4444',
    padding: '4px 10px',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '600',
  },
  alertsList: {
    padding: '20px',
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
    overflowY: 'auto',
    maxHeight: '600px',
  },
  alertItem: {
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '10px',
    padding: '16px',
    display: 'flex',
    gap: '16px',
  },
  alertIconBox: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    width: '40px',
    height: '40px',
    borderRadius: '8px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    flexShrink: 0,
  },
  alertContent: {
    flex: 1,
  },
  alertTopRow: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: '6px',
  },
  alertItemTitle: {
    fontSize: '14px',
    fontWeight: '600',
    margin: 0,
    color: '#f8fafc',
  },
  alertTime: {
    fontSize: '11px',
    color: '#6b7280',
    display: 'flex',
    alignItems: 'center',
  },
  alertMessage: {
    fontSize: '13px',
    color: '#9ca3af',
    margin: '0 0 16px 0',
    lineHeight: '1.4',
  },
  alertActions: {
    display: 'flex',
    gap: '12px',
  },
  btnApprove: {
    backgroundColor: '#93c5fd',
    color: '#000000',
    border: 'none',
    padding: '8px 16px',
    borderRadius: '6px',
    fontSize: '13px',
    fontWeight: '600',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    transition: 'opacity 0.2s',
  },
  btnReject: {
    backgroundColor: '#2d2d2d',
    color: '#d1d5db',
    border: 'none',
    padding: '8px 16px',
    borderRadius: '6px',
    fontSize: '13px',
    fontWeight: '500',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    transition: 'background 0.2s',
  },
  emptyState: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '40px 0',
    color: '#6b7280',
    fontSize: '14px',
  },
  formContainer: {
    padding: '24px',
    display: 'flex',
    flexDirection: 'column',
    gap: '20px',
  },
  formGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
  },
  inputLabel: {
    fontSize: '13px',
    fontWeight: '500',
    color: '#9ca3af',
  },
  radioGroup: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(120px, 1fr))',
    gap: '12px',
  },
  radioOption: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '8px',
    padding: '12px',
    backgroundColor: '#121212',
    border: '1px solid',
    borderRadius: '8px',
    cursor: 'pointer',
    fontSize: '13px',
    fontWeight: '500',
    transition: 'all 0.2s',
  },
  textInput: {
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '12px',
    color: '#f8fafc',
    fontSize: '14px',
    outline: 'none',
  },
  textArea: {
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '12px',
    color: '#f8fafc',
    fontSize: '14px',
    outline: 'none',
    resize: 'vertical',
    minHeight: '100px',
  },
  btnSend: {
    backgroundColor: '#93c5fd',
    color: '#000000',
    border: 'none',
    padding: '12px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '8px',
    marginTop: '10px',
    transition: 'opacity 0.2s',
  }
};