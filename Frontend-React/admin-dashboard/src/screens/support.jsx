import  { useEffect, useRef, useState } from 'react';
import { 
  Search, 
  Send, 
  Image as ImageIcon, 
  CheckCircle2, 
  Clock, 
  AlertCircle
} from 'lucide-react';
import SendConfirmDialog from './components/SendConfirmDialog';

// بيانات وهمية للطلبات القادمة من تطبيق الموبايل مع محادثات
const initialTickets = [
  {
    id: 'TCK-1042',
    user: 'أحمد محمود',
    title: 'مشكلة في حجز رحلة اسطنبول',
    status: 'pending', // pending: admin can send exactly one reply
    date: '10:30 AM',
    messages: [
      { from: 'user', text: 'مرحباً، عند محاولة الدفع يظهر لي خطأ ولم يتم إتمام الحجز.', time: '10:30 AM', image: 'https://images.unsplash.com/photo-1612810806563-4cb8265db55b?w=400&auto=format&fit=crop&q=60' }
    ],
    errors: [
      'Payment gateway timeout',
      'Invalid session token on checkout'
    ],
    sentByAdmin: false,
  },
  {
    id: 'TCK-1041',
    user: 'سارة خالد',
    title: 'تعديل موعد الرحلة',
    status: 'pending',
    date: 'Yesterday',
    messages: [
      { from: 'user', text: 'هل يمكنني تغيير موعد انطلاق الرحلة إلى الأسبوع القادم؟', time: 'Yesterday' }
    ],
    errors: [
      'User could not find edit option',
    ],
    sentByAdmin: false,
  },
  {
    id: 'TCK-1040',
    user: 'شركة الأفق للسياحة',
    title: 'تأخير في ظهور البرنامج الجديد',
    status: 'resolved',
    date: 'Jun 12',
    messages: [
      { from: 'user', text: 'تم رفع برنامج جديد ولكنه غير ظاهر للمستخدمين.', time: 'Jun 11' },
      { from: 'admin', text: 'تمت معالجة المشكلة وسيظهر خلال 24 ساعة. تم إرسال بريد تأكيد للمسؤول.', time: 'Jun 12' }
    ],
    errors: [],
    sentByAdmin: true,
  }
];

export default function Support() {
  const [tickets, setTickets] = useState(initialTickets);
  const [activeTicketId, setActiveTicketId] = useState(initialTickets[0].id);
  const [activeTab, setActiveTab] = useState('pending'); // 'pending' or 'resolved'
  const [searchTerm, setSearchTerm] = useState('');
  const [replyText, setReplyText] = useState('');
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [sendingTicketId, setSendingTicketId] = useState(null);
  const replyTextareaRef = useRef(null);

  const adjustReplyHeight = () => {
    const textarea = replyTextareaRef.current;
    if (!textarea) return;

    textarea.style.height = 'auto';
    textarea.style.height = `${Math.min(textarea.scrollHeight, 140)}px`;
  };

  useEffect(() => {
    adjustReplyHeight();
  }, [replyText]);

  const getStatusConfig = (status) => {
    switch(status) {
      case 'open': return { color: '#ef4444', icon: <AlertCircle size={14} />, text: 'New' };
      case 'pending': return { color: '#f59e0b', icon: <Clock size={14} />, text: 'Pending' };
      case 'resolved': return { color: '#10b981', icon: <CheckCircle2 size={14} />, text: 'Resolved' };
      default: return { color: '#9ca3af', icon: null, text: 'Unknown' };
    }
  };

  const getTicketById = (id) => tickets.find(t => t.id === id) || tickets[0];
  const activeTicket = getTicketById(activeTicketId);

  const filteredTickets = tickets.filter(t => {
    if (activeTab === 'resolved') return t.status === 'resolved';
    return t.status === 'pending';
  }).filter(t => t.title.toLowerCase().includes(searchTerm.toLowerCase()));

  const handleSendClick = (ticketId) => {
    setSendingTicketId(ticketId);
    setConfirmOpen(true);
  };

  const handleReplyChange = (e) => {
    setReplyText(e.target.value);
    requestAnimationFrame(adjustReplyHeight);
  };

  const handleConfirmSend = () => {
    // append admin message and mark sent
    setTickets(prev => prev.map(t => {
      if (t.id !== sendingTicketId) return t;
      const newMsg = { from: 'admin', text: replyText || 'تم إرسال الحل عبر البريد الإلكتروني.', time: 'Now' };
      return { ...t, messages: [...(t.messages||[]), newMsg], sentByAdmin: true };
    }));
    setReplyText('');
    setConfirmOpen(false);
    setSendingTicketId(null);
  };

  return (
    <div style={styles.container}>
      <div style={styles.layout}>

        {/* اللوحة اليسرى: تابات + قائمة التذاكر */}
        <div style={styles.ticketsList}>
          <div style={styles.tabsRow}>
            <button onClick={() => setActiveTab('pending')} style={{...styles.tabBtn, ...(activeTab==='pending'?styles.tabActive:{})}}>Pending</button>
            <button onClick={() => setActiveTab('resolved')} style={{...styles.tabBtn, ...(activeTab==='resolved'?styles.tabActive:{})}}>Resolved</button>
          </div>

          <div style={{padding: '12px'}}>
            <div style={styles.searchBox}>
              <Search size={18} color="#6b7280" />
              <input 
                type="text" 
                placeholder="Search by title..." 
                style={styles.searchInput}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              {/* <Filter size={18} color="#6b7280" style={{cursor: 'pointer'}} /> */}
            </div>
          </div>

          <div style={styles.ticketsScroll}>
            {filteredTickets.map(ticket => {
              const statusConf = getStatusConfig(ticket.status);
              const isActive = activeTicketId === ticket.id;
              return (
                <div 
                  key={ticket.id} 
                  style={{...styles.ticketCard, backgroundColor: isActive ? '#262626' : '#1a1a1a'}}
                  onClick={() => setActiveTicketId(ticket.id)}
                >
                  <div style={styles.ticketCardHeader}>
                    <span style={styles.ticketUser}>{ticket.user}</span>
                    <span style={styles.ticketDate}>{ticket.date}</span>
                  </div>
                  <h4 style={styles.ticketTitle}>{ticket.title}</h4>
                  <div style={styles.ticketFooter}>
                    <span style={{...styles.statusBadge, color: statusConf.color, backgroundColor: `${statusConf.color}15`}}>
                      {statusConf.icon}
                      <span style={{marginLeft: '4px'}}>{statusConf.text}</span>
                    </span>
                    {ticket.messages && ticket.messages.some(m => m.image) && <ImageIcon size={14} color="#6b7280" />}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* اللوحة اليمنى: تفاصيل التذكرة ومربع المحادثة */}
        <div style={styles.ticketDetail}>
          <div style={styles.detailHeader}>
            <div>
              <h2 style={styles.detailTitle}>{activeTicket.title}</h2>
              <p style={styles.detailSub}>Ticket {activeTicket.id} • From: <span style={{color: '#93c5fd'}}>{activeTicket.user}</span></p>
            </div>
            {/* <button style={styles.btnAction}><MoreVertical size={20} color="#9ca3af" /></button> */}
          </div>

          <div style={styles.detailBody}>
            {/* عرض الأخطاء إذا كانت في تبويب pending */}
            {activeTab === 'pending' && activeTicket.errors && activeTicket.errors.length > 0 && (
              <div style={styles.errorList}>
                <h4 style={{margin: '0 0 8px 0', color: '#f87171'}}>Error Messages</h4>
                <ul style={{margin:0, paddingLeft: '18px'}}>
                  {activeTicket.errors.map((err, i) => <li key={i} style={{color: '#fca5a5', marginBottom: '6px'}}>{err}</li>)}
                </ul>
              </div>
            )}

            {/* المحادثة */}
            <div style={styles.chatArea}>
              {(activeTicket.messages||[]).map((m, idx) => (
                <div key={idx} style={{display: 'flex', justifyContent: m.from === 'admin' ? 'flex-end' : 'flex-start', marginBottom: '12px'}}>
                  <div style={{...styles.chatBubble, ...(m.from === 'admin' ? styles.adminBubble : styles.userBubble)}}>
                    <p style={{...styles.chatText, color: m.from === 'admin' ? '#032e6b' : '#d1d5db'}}>{m.text}</p>
                    {m.image && <img src={m.image} alt="attach" style={styles.chatImage} />}
                    <div style={{fontSize: '11px', color: '#6b7280', marginTop: '6px'}}>{m.time}</div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* مربع الإدخال: قابل للإرسال فقط في تبويب pending ولم ترسل بعد */}
          <div style={styles.replySection}>
            {activeTab === 'resolved' ? (
              <div style={{padding: '16px', color: '#9ca3af'}}>المحادثة للعرض فقط. لا يمكنك إرسال رد هنا.</div>
            ) : (
              <div>
                {activeTicket.sentByAdmin ? (
                  <div style={{padding: '12px', color: '#9ca3af'}}>لقد تم إرسال الحل عبر البريد الإلكتروني. لا يمكنك إرسال رسائل إضافية.</div>
                ) : (
                  <div style={styles.replyBox}>
                    {/* <button style={styles.iconBtn}><Paperclip size={20} color="#9ca3af" /></button> */}
                    <textarea
                      ref={replyTextareaRef}
                      rows={1}
                      placeholder="Write your reply..."
                      style={styles.replyInput}
                      value={replyText}
                      onChange={handleReplyChange}
                    />
                    <button style={styles.sendBtn} onClick={() => handleSendClick(activeTicket.id)}>
                      <span>Send</span>
                      <Send size={16} />
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>

      </div>

      <SendConfirmDialog isOpen={confirmOpen} onClose={() => setConfirmOpen(false)} onConfirm={handleConfirmSend} targetName={activeTicket.user} />
    </div>
  );
}

// تصميم متوافق 100% مع الواجهات السابقة
const styles = {
  container: {
    padding: '30px',
    backgroundColor: '#121212',
    minHeight: '100vh',
    color: '#f8fafc',
    fontFamily: '"Inter", "Segoe UI", sans-serif',
    direction: 'ltr',
  },
  layout: {
    display: 'flex',
    gap: '24px',
    height: 'calc(100vh - 100px)',
    maxWidth: '1200px',
  },
  ticketsList: {
    width: '350px',
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden',
  },
  listHeader: {
    padding: '20px',
    borderBottom: '1px solid #2d2d2d',
  },
  headerTitle: {
    fontSize: '18px',
    fontWeight: '600',
    margin: '0 0 16px 0',
  },
  searchBox: {
    display: 'flex',
    alignItems: 'center',
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '8px 12px',
    gap: '8px',
  },
  searchInput: {
    flex: 1,
    backgroundColor: 'transparent',
    border: 'none',
    color: '#f8fafc',
    outline: 'none',
    fontSize: '14px',
  },
  ticketsScroll: {
    flex: 1,
    overflowY: 'auto',
    padding: '12px',
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
  },
  ticketCard: {
    padding: '16px',
    borderRadius: '8px',
    cursor: 'pointer',
    border: '1px solid transparent',
    transition: 'all 0.2s',
  },
  ticketCardHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    marginBottom: '8px',
  },
  ticketUser: {
    fontSize: '13px',
    fontWeight: '600',
    color: '#d1d5db',
  },
  ticketDate: {
    fontSize: '12px',
    color: '#6b7280',
  },
  ticketTitle: {
    fontSize: '14px',
    fontWeight: '500',
    margin: '0 0 12px 0',
    color: '#f8fafc',
  },
  ticketFooter: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  statusBadge: {
    display: 'flex',
    alignItems: 'center',
    padding: '4px 8px',
    borderRadius: '20px',
    fontSize: '12px',
    fontWeight: '500',
  },
  ticketDetail: {
    flex: 1,
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    display: 'flex',
    flexDirection: 'column',
  },
  detailHeader: {
    padding: '24px',
    borderBottom: '1px solid #2d2d2d',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  detailTitle: {
    fontSize: '20px',
    fontWeight: '600',
    margin: '0 0 8px 0',
  },
  detailSub: {
    fontSize: '13px',
    color: '#9ca3af',
    margin: 0,
  },
  btnAction: {
    background: 'none',
    border: 'none',
    cursor: 'pointer',
    padding: '8px',
  },
  detailBody: {
    flex: 1,
    padding: '24px',
    overflowY: 'auto',
    backgroundColor: '#121212', // خلفية أغمق لمنطقة الرسائل
  },
  messageBubble: {
    backgroundColor: '#1a1a1a',
    border: '1px solid #2d2d2d',
    borderRadius: '12px',
    padding: '20px',
    maxWidth: '80%',
  },
  messageText: {
    fontSize: '15px',
    lineHeight: '1.6',
    color: '#d1d5db',
    margin: '0 0 16px 0',
  },
  attachmentBox: {
    marginTop: '16px',
    paddingTop: '16px',
    borderTop: '1px solid #2d2d2d',
  },
  attachmentLabel: {
    fontSize: '13px',
    color: '#9ca3af',
    marginBottom: '8px',
  },
  attachedImg: {
    maxWidth: '300px',
    borderRadius: '8px',
    border: '1px solid #2d2d2d',
  },
  replySection: {
    padding: '20px',
    borderTop: '1px solid #2d2d2d',
    backgroundColor: '#1a1a1a',
  },
  replyBox: {
    display: 'flex',
    alignItems: 'flex-end',
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '8px 16px',
    gap: '12px',
  },
  tabsRow: {
    display: 'flex',
    gap: '8px',
    padding: '12px',
  },
  tabBtn: {
    flex: 1,
    padding: '10px 12px',
    background: 'transparent',
    border: '1px solid transparent',
    color: '#cbd5e1',
    borderRadius: '8px',
    cursor: 'pointer',
    fontWeight: 600,
  },
  tabActive: {
    backgroundColor: '#93c5fd',
    border: '1px solid #000000',
    color: '#000000',
  },
  errorList: {
    marginBottom: '12px',
    padding: '12px',
    borderRadius: '8px',
    backgroundColor: '#2a1a1a'
  },
  chatArea: {
    display: 'flex',
    flexDirection: 'column',
    gap: '12px'
  },
  chatBubble: {
    maxWidth: '70%',
    padding: '12px 16px',
    borderRadius: '12px',
    overflowWrap: 'anywhere',
  },
  chatText: {
    margin: 0,
    lineHeight: 1.6,
    whiteSpace: 'pre-wrap',
    wordBreak: 'break-word',
  },
  adminBubble: {
    backgroundColor: '#dbeafe',
    border: '1px solid #93c5fd'
  },
  userBubble: {
    backgroundColor: '#111827',
    border: '1px solid #2d2d2d'
  },
  chatImage: {
    marginTop: '8px',
    maxWidth: '280px',
    borderRadius: '8px',
    display: 'block'
  },
  iconBtn: {
    background: 'none',
    border: 'none',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  replyInput: {
    flex: 1,
    backgroundColor: 'transparent',
    border: 'none',
    color: '#f8fafc',
    outline: 'none',
    fontSize: '15px',
    padding: '8px 0',
    resize: 'none',
    minHeight: '24px',
    maxHeight: '140px',
    lineHeight: 1.5,
    overflowY: 'auto',
    whiteSpace: 'pre-wrap',
    wordBreak: 'break-word',
  },
  sendBtn: {
    backgroundColor: '#93c5fd',
    color: '#000000',
    border: 'none',
    borderRadius: '6px',
    padding: '8px 16px',
    fontSize: '14px',
    fontWeight: '600',
    cursor: 'pointer',
    display: 'flex',
    alignItems: 'center',
    gap: '8px',
    transition: 'opacity 0.2s',
  }
};