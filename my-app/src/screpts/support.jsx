import  { useState } from 'react';
import { 
  Search, 
  Filter, 
  MoreVertical, 
  Paperclip, 
  Send, 
  Image as ImageIcon, 
  CheckCircle2, 
  Clock, 
  AlertCircle
} from 'lucide-react';

// بيانات وهمية للطلبات القادمة من تطبيق الموبايل
const initialTickets = [
  {
    id: 'TCK-1042',
    user: 'أحمد محمود',
    title: 'مشكلة في حجز رحلة اسطنبول',
    description: 'مرحباً، قمت بمحاولة تأكيد الحجز للبرنامج السياحي ولكن التطبيق يظهر لي رسالة خطأ عند الوصول لصفحة الدفع. أرفقت لكم صورة للشاشة.',
    status: 'open', // open, pending, resolved
    date: '10:30 AM',
    hasImage: true,
    attachedImage: 'https://images.unsplash.com/photo-1612810806563-4cb8265db55b?w=400&auto=format&fit=crop&q=60',
  },
  {
    id: 'TCK-1041',
    user: 'سارة خالد',
    title: 'تعديل موعد الرحلة',
    description: 'هل يمكنني تغيير موعد انطلاق الرحلة إلى الأسبوع القادم؟ لم أجد خيار التعديل في إعدادات التطبيق.',
    status: 'pending',
    date: 'Yesterday',
    hasImage: false,
  },
  {
    id: 'TCK-1040',
    user: 'شركة الأفق للسياحة',
    title: 'تأخير في ظهور البرنامج الجدي',
    description: 'تم رفع برنامج سياحي جديد البارحة ولكنه لم يظهر للمستخدمين حتى الآن، يرجى المساعدة.',
    status: 'resolved',
    date: 'Jun 12',
    hasImage: false,
  }
];

export default function Support() {
  const [activeTicket, setActiveTicket] = useState(initialTickets[0]);
  const [replyText, setReplyText] = useState('');

  const getStatusConfig = (status) => {
    switch(status) {
      case 'open': return { color: '#ef4444', icon: <AlertCircle size={14} />, text: 'New' };
      case 'pending': return { color: '#f59e0b', icon: <Clock size={14} />, text: 'Pending' };
      case 'resolved': return { color: '#10b981', icon: <CheckCircle2 size={14} />, text: 'Resolved' };
      default: return { color: '#9ca3af', icon: null, text: 'Unknown' };
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.layout}>
        
        {/* اللوحة اليسرى: قائمة التذاكر */}
        <div style={styles.ticketsList}>
          <div style={styles.listHeader}>
            <h3 style={styles.headerTitle}>Support Tickets</h3>
            <div style={styles.searchBox}>
              <Search size={18} color="#6b7280" />
              <input 
                type="text" 
                placeholder="Search tickets..." 
                style={styles.searchInput}
              />
              <Filter size={18} color="#6b7280" style={{cursor: 'pointer'}} />
            </div>
          </div>

          <div style={styles.ticketsScroll}>
            {initialTickets.map(ticket => {
              const statusConf = getStatusConfig(ticket.status);
              const isActive = activeTicket.id === ticket.id;
              
              return (
                <div 
                  key={ticket.id} 
                  style={{...styles.ticketCard, backgroundColor: isActive ? '#262626' : '#1a1a1a'}}
                  onClick={() => setActiveTicket(ticket)}
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
                    {ticket.hasImage && <ImageIcon size={14} color="#6b7280" />}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        {/* اللوحة اليمنى: تفاصيل التذكرة ومربع الرد */}
        <div style={styles.ticketDetail}>
          {/* رأس التذكرة */}
          <div style={styles.detailHeader}>
            <div>
              <h2 style={styles.detailTitle}>{activeTicket.title}</h2>
              <p style={styles.detailSub}>Ticket {activeTicket.id} • From: <span style={{color: '#93c5fd'}}>{activeTicket.user}</span></p>
            </div>
            <button style={styles.btnAction}><MoreVertical size={20} color="#9ca3af" /></button>
          </div>

          {/* محتوى المشكلة (المستلم من الموبايل) */}
          <div style={styles.detailBody}>
            <div style={styles.messageBubble}>
              <p style={styles.messageText}>{activeTicket.description}</p>
              {activeTicket.hasImage && (
                <div style={styles.attachmentBox}>
                  <p style={styles.attachmentLabel}>Attached Image:</p>
                  <img src={activeTicket.attachedImage} alt="Issue Attachment" style={styles.attachedImg} />
                </div>
              )}
            </div>
          </div>

          {/* مربع كتابة الرد */}
          <div style={styles.replySection}>
            <div style={styles.replyBox}>
              <button style={styles.iconBtn}><Paperclip size={20} color="#9ca3af" /></button>
              <input 
                type="text" 
                placeholder="Write your reply to the user..." 
                style={styles.replyInput}
                value={replyText}
                onChange={(e) => setReplyText(e.target.value)}
              />
              <button style={styles.sendBtn}>
                <span>Send</span>
                <Send size={16} />
              </button>
            </div>
          </div>

        </div>
      </div>
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
    alignItems: 'center',
    backgroundColor: '#121212',
    border: '1px solid #2d2d2d',
    borderRadius: '8px',
    padding: '8px 16px',
    gap: '12px',
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