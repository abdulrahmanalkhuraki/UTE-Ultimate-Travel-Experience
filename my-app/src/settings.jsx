import  { useState } from 'react';
import { 
  Settings as SettingsIcon, 
  Globe, 
  DollarSign, 
  Bell, 
  Shield, 
  Save,
  //User
} from 'lucide-react';

export default function Settings() {
  const [activeTab, setActiveTab] = useState('general');

  // حالات إعدادات المنصة (وهمية للتوضيح)
  const [settings, setSettings] = useState({
    appName: 'Rehlati',
    currency: 'USD',
    commission: '15',
    notifyNewCompany: true,
    notifyTripCancel: true,
    notifySupport: false,
    twoFactor: true,
  });

  const handleToggle = (key) => {
    setSettings({ ...settings, [key]: !settings[key] });
  };

  const handleSave = (e) => {
    e.preventDefault();
    alert('Settings saved successfully!');
  };

  return (
    <div style={styles.container}>
      <div style={styles.pageHeader}>
        <SettingsIcon size={24} color="#f8fafc" />
        <h2 style={styles.pageTitle}>Platform Settings</h2>
      </div>

      <div style={styles.layout}>
        {/* القائمة الجانبية للتبويبات */}
        <div style={styles.sidebar}>
          <div 
            style={{...styles.tabBtn, ...(activeTab === 'general' ? styles.activeTab : {})}}
            onClick={() => setActiveTab('general')}
          >
            <Globe size={18} color={activeTab === 'general' ? '#93c5fd' : '#9ca3af'} />
            <span style={{color: activeTab === 'general' ? '#f8fafc' : '#9ca3af'}}>General</span>
          </div>

          <div 
            style={{...styles.tabBtn, ...(activeTab === 'financials' ? styles.activeTab : {})}}
            onClick={() => setActiveTab('financials')}
          >
            <DollarSign size={18} color={activeTab === 'financials' ? '#93c5fd' : '#9ca3af'} />
            <span style={{color: activeTab === 'financials' ? '#f8fafc' : '#9ca3af'}}>Financials</span>
          </div>

          <div 
            style={{...styles.tabBtn, ...(activeTab === 'notifications' ? styles.activeTab : {})}}
            onClick={() => setActiveTab('notifications')}
          >
            <Bell size={18} color={activeTab === 'notifications' ? '#93c5fd' : '#9ca3af'} />
            <span style={{color: activeTab === 'notifications' ? '#f8fafc' : '#9ca3af'}}>Notifications</span>
          </div>

          <div 
            style={{...styles.tabBtn, ...(activeTab === 'security' ? styles.activeTab : {})}}
            onClick={() => setActiveTab('security')}
          >
            <Shield size={18} color={activeTab === 'security' ? '#93c5fd' : '#9ca3af'} />
            <span style={{color: activeTab === 'security' ? '#f8fafc' : '#9ca3af'}}>Security</span>
          </div>
        </div>

        {/* محتوى التبويب النشط */}
        <div style={styles.contentArea}>
          <form style={styles.card} onSubmit={handleSave}>
            
            {/* --- General Settings --- */}
            {activeTab === 'general' && (
              <div style={styles.section}>
                <h3 style={styles.sectionTitle}>General Preferences</h3>
                <p style={styles.sectionDesc}>Manage your platform's basic information and localization.</p>
                
                <div style={styles.formGroup}>
                  <label style={styles.label}>Platform Name</label>
                  <input 
                    type="text" 
                    value={settings.appName}
                    onChange={(e) => setSettings({...settings, appName: e.target.value})}
                    style={styles.input} 
                  />
                </div>

                <div style={styles.formGroup}>
                  <label style={styles.label}>Default Currency</label>
                  <select 
                    value={settings.currency}
                    onChange={(e) => setSettings({...settings, currency: e.target.value})}
                    style={styles.select}
                  >
                    <option value="USD">USD ($) - US Dollar</option>
                    <option value="TRY">TRY (₺) - Turkish Lira</option>
                    <option value="EUR">EUR (€) - Euro</option>
                    <option value="AED">AED - Emirati Dirham</option>
                  </select>
                </div>
              </div>
            )}

            {/* --- Financials Settings --- */}
            {activeTab === 'financials' && (
              <div style={styles.section}>
                <h3 style={styles.sectionTitle}>Commission & Billing</h3>
                <p style={styles.sectionDesc}>Set the platform's financial rules for tourism companies.</p>
                
                <div style={styles.formGroup}>
                  <label style={styles.label}>Platform Commission Rate (%)</label>
                  <div style={styles.inputWithIcon}>
                    <DollarSign size={16} color="#9ca3af" style={styles.inputIcon} />
                    <input 
                      type="number" 
                      value={settings.commission}
                      onChange={(e) => setSettings({...settings, commission: e.target.value})}
                      style={{...styles.input, paddingLeft: '36px'}} 
                    />
                  </div>
                  <span style={styles.hintText}>This percentage is deducted from every successful trip booking.</span>
                </div>
              </div>
            )}

            {/* --- Notifications Settings --- */}
            {activeTab === 'notifications' && (
              <div style={styles.section}>
                <h3 style={styles.sectionTitle}>Admin Email Alerts</h3>
                <p style={styles.sectionDesc}>Choose which events trigger an email to the admin team.</p>
                
                <div style={styles.toggleRow}>
                  <div>
                    <h4 style={styles.toggleTitle}>New Company Registration</h4>
                    <p style={styles.toggleDesc}>Get notified when a new tourism company applies.</p>
                  </div>
                  <ToggleSwitch 
                    isOn={settings.notifyNewCompany} 
                    onToggle={() => handleToggle('notifyNewCompany')} 
                  />
                </div>

                <div style={styles.toggleRow}>
                  <div>
                    <h4 style={styles.toggleTitle}>Trip Cancellations</h4>
                    <p style={styles.toggleDesc}>Get notified if a company cancels a group trip.</p>
                  </div>
                  <ToggleSwitch 
                    isOn={settings.notifyTripCancel} 
                    onToggle={() => handleToggle('notifyTripCancel')} 
                  />
                </div>

                <div style={styles.toggleRow}>
                  <div>
                    <h4 style={styles.toggleTitle}>New Support Tickets</h4>
                    <p style={styles.toggleDesc}>Get notified when a tourist opens a support ticket.</p>
                  </div>
                  <ToggleSwitch 
                    isOn={settings.notifySupport} 
                    onToggle={() => handleToggle('notifySupport')} 
                  />
                </div>
              </div>
            )}

            {/* --- Security Settings --- */}
            {activeTab === 'security' && (
              <div style={styles.section}>
                <h3 style={styles.sectionTitle}>Security & Access</h3>
                <p style={styles.sectionDesc}>Manage admin access and security policies.</p>
                
                <div style={styles.formGroup}>
                  <label style={styles.label}>Change Admin Password</label>
                  <input type="password" placeholder="Enter new password" style={styles.input} />
                </div>
                
                <div style={styles.formGroup}>
                  <input type="password" placeholder="Confirm new password" style={styles.input} />
                </div>

                <div style={{...styles.toggleRow, marginTop: '24px'}}>
                  <div>
                    <h4 style={styles.toggleTitle}>Two-Factor Authentication (2FA)</h4>
                    <p style={styles.toggleDesc}>Require an email code when logging into the dashboard.</p>
                  </div>
                  <ToggleSwitch 
                    isOn={settings.twoFactor} 
                    onToggle={() => handleToggle('twoFactor')} 
                  />
                </div>
              </div>
            )}

            {/* زر الحفظ يظهر في كل التبويبات */}
            <div style={styles.footer}>
              <button type="submit" style={styles.btnSave}>
                <Save size={16} />
                <span>Save Changes</span>
              </button>
            </div>

          </form>
        </div>
      </div>
    </div>
  );
}

// مكون فرعي صغير لزر التبديل (Toggle Switch)
function ToggleSwitch({ isOn, onToggle }) {
  return (
    <div 
      onClick={onToggle}
      style={{
        width: '44px',
        height: '24px',
        backgroundColor: isOn ? '#93c5fd' : '#2d2d2d',
        borderRadius: '20px',
        display: 'flex',
        alignItems: 'center',
        padding: '2px',
        cursor: 'pointer',
        transition: 'background-color 0.3s',
      }}
    >
      <div 
        style={{
          width: '20px',
          height: '20px',
          backgroundColor: '#ffffff',
          borderRadius: '50%',
          transform: isOn ? 'translateX(20px)' : 'translateX(0)',
          transition: 'transform 0.3s ease',
        }}
      />
    </div>
  );
}

// الأنماط التصميمية (Dark Theme)
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
    display: 'flex',
    gap: '24px',
    maxWidth: '1000px',
  },
  sidebar: {
    width: '240px',
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
  },
  tabBtn: {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    padding: '12px 16px',
    borderRadius: '8px',
    cursor: 'pointer',
    fontSize: '14px',
    fontWeight: '500',
    transition: 'all 0.2s',
    backgroundColor: 'transparent',
  },
  activeTab: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
  },
  contentArea: {
    flex: 1,
  },
  card: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '30px',
    display: 'flex',
    flexDirection: 'column',
    minHeight: '400px',
  },
  section: {
    flex: 1,
    display: 'flex',
    flexDirection: 'column',
    gap: '20px',
  },
  sectionTitle: {
    fontSize: '18px',
    fontWeight: '600',
    margin: '0',
  },
  sectionDesc: {
    fontSize: '14px',
    color: '#9ca3af',
    margin: '0 0 16px 0',
  },
  formGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    maxWidth: '400px',
  },
  label: {
    fontSize: '13px',
    fontWeight: '500',
    color: '#d1d5db',
  },
  input: {
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '10px 14px',
    color: '#f8fafc',
    fontSize: '14px',
    outline: 'none',
    width: '100%',
  },
  select: {
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '10px 14px',
    color: '#f8fafc',
    fontSize: '14px',
    outline: 'none',
    cursor: 'pointer',
  },
  inputWithIcon: {
    position: 'relative',
    display: 'flex',
    alignItems: 'center',
  },
  inputIcon: {
    position: 'absolute',
    left: '12px',
  },
  hintText: {
    fontSize: '12px',
    color: '#6b7280',
    marginTop: '4px',
  },
  toggleRow: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: '16px',
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
  },
  toggleTitle: {
    fontSize: '14px',
    fontWeight: '500',
    margin: '0 0 4px 0',
    color: '#f8fafc',
  },
  toggleDesc: {
    fontSize: '12px',
    color: '#9ca3af',
    margin: 0,
  },
  footer: {
    marginTop: '32px',
    paddingTop: '20px',
    borderTop: '1px solid #2d2d2d',
    display: 'flex',
    justifyContent: 'flex-end',
  },
  btnSave: {
    backgroundColor: '#93c5fd',
    color: '#000000',
    border: 'none',
    padding: '10px 20px',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    transition: 'opacity 0.2s',
  }
};