import { useEffect, useMemo, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Search, Send, Image as ImageIcon, CheckCircle2, Clock, AlertCircle, HeadphonesIcon, User,
} from 'lucide-react';
import SendConfirmDialog from '../components/SendConfirmDialog';
import { useApiData } from '../hooks/useApiData';
import { useSyncedState } from '../hooks/useSyncedState';
import { getTickets, getSupportReplies, sendSupportReply, TICKET_STATUS } from '../services/supportApi';
import { API_BASE_URL } from '../config/constants';
import { formatDate } from '../utils/format';

function resolveImageUrl(url) {
  if (!url) return null;
  return /^https?:\/\//i.test(url) ? url : `${API_BASE_URL}${url.startsWith('/') ? '' : '/'}${url}`;
}

function ticketUserLabel(user) {
  if (!user) return '—';
  const name = [user.firstName, user.lastName].filter(Boolean).join(' ').trim();
  return name || user.email || (user.id ? `User #${user.id}` : '—');
}

export default function Support() {
  const { t } = useTranslation();
  const { data: ticketsData, loading, error } = useApiData(getTickets, []);
  const [tickets, setTickets] = useSyncedState(ticketsData, (data) => data || []);
  const [activeTicketId, setActiveTicketId] = useState(null);
  const [statusFilter, setStatusFilter] = useState('all');
  const [searchTerm, setSearchTerm] = useState('');

  // replies[ticketId] = { data: SupportReplyResponse[], error?: string } once fetched, undefined while loading
  const [replies, setReplies] = useState({});

  const [replyText, setReplyText] = useState('');
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [sending, setSending] = useState(false);
  const [sendError, setSendError] = useState('');
  const replyTextareaRef = useRef(null);

  const activeTicket = useMemo(() => {
    if (activeTicketId) return tickets.find((ticket) => ticket.id === activeTicketId) || null;
    return tickets[0] || null;
  }, [tickets, activeTicketId]);

  useEffect(() => {
    const id = activeTicket?.id;
    if (!id || replies[id] !== undefined) return;
    let cancelled = false;
    getSupportReplies(id)
      .then((data) => {
        if (!cancelled) setReplies((prev) => ({ ...prev, [id]: { data: data || [] } }));
      })
      .catch((err) => {
        if (!cancelled) setReplies((prev) => ({ ...prev, [id]: { data: [], error: err.message || t('support.replyLoadError') } }));
      });
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTicket?.id]);

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
    switch (status) {
      case TICKET_STATUS.OPEN:
        return { color: 'var(--color-warning)', bg: 'var(--color-warning-soft)', icon: <Clock size={14} />, text: t('support.open') };
      case TICKET_STATUS.CLOSED:
        return { color: 'var(--color-success)', bg: 'var(--color-success-soft)', icon: <CheckCircle2 size={14} />, text: t('support.resolved') };
      default:
        return { color: 'var(--color-text-muted)', bg: 'var(--color-surface-alt)', icon: <AlertCircle size={14} />, text: '—' };
    }
  };

  const filteredTickets = tickets
    .filter((ticket) => (statusFilter === 'all' ? true : ticket.status === statusFilter))
    .filter((ticket) => (ticket.subject || '').toLowerCase().includes(searchTerm.toLowerCase()));

  const activeReplyEntry = activeTicket ? replies[activeTicket.id] : undefined;
  const repliesLoading = !!activeTicket && activeReplyEntry === undefined;
  const repliesError = activeReplyEntry?.error || '';
  const activeReplies = activeReplyEntry?.data || [];
  const hasReply = activeReplies.length > 0;
  const isClosed = activeTicket?.status === TICKET_STATUS.CLOSED || hasReply;

  const handleReplyChange = (e) => {
    setReplyText(e.target.value);
    requestAnimationFrame(adjustReplyHeight);
  };

  const handleSendClick = () => {
    if (!replyText.trim()) return;
    setSendError('');
    setConfirmOpen(true);
  };

  const handleConfirmSend = async () => {
    if (!activeTicket) return;
    setSending(true);
    setSendError('');
    try {
      const reply = await sendSupportReply(activeTicket.id, replyText.trim());
      setReplies((prev) => ({
        ...prev,
        [activeTicket.id]: { data: [...(prev[activeTicket.id]?.data || []), reply] },
      }));
      setTickets((prev) =>
        prev.map((tk) => (tk.id === activeTicket.id ? { ...tk, status: TICKET_STATUS.CLOSED } : tk))
      );
      setReplyText('');
      setConfirmOpen(false);
    } catch (err) {
      setSendError(err.message || t('support.replySendError'));
      setConfirmOpen(false);
    } finally {
      setSending(false);
    }
  };

  return (
    <div className="p-6 md:p-8 space-y-6">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-[var(--color-accent-soft)] flex items-center justify-center">
          <HeadphonesIcon className="w-5 h-5 text-[var(--color-accent)]" />
        </div>
        <div>
          <h2 className="text-xl font-semibold text-[var(--color-text)]">{t('support.title')}</h2>
          <p className="text-sm text-[var(--color-text-muted)]">{t('support.subtitle')}</p>
        </div>
      </div>

      {error && (
        <div className="rounded-xl border border-[var(--color-danger)]/30 bg-[var(--color-danger-soft)] p-3 text-sm text-[var(--color-danger)]">
          {t('support.loadError')}
        </div>
      )}

      <div className="flex flex-col lg:flex-row gap-6 h-[calc(100vh-220px)] min-h-[420px]">
        {/* Ticket list */}
        <div className="w-full lg:w-96 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl flex flex-col overflow-hidden shrink-0 shadow-[var(--shadow-card)]">
          <div className="flex gap-2 p-3 border-b border-[var(--color-border)]">
            {[
              { id: 'all', label: t('support.allStatuses') },
              { id: TICKET_STATUS.OPEN, label: t('support.open') },
              { id: TICKET_STATUS.CLOSED, label: t('support.resolved') },
            ].map((tab) => (
              <button
                key={tab.id}
                onClick={() => setStatusFilter(tab.id)}
                className={`flex-1 px-2 py-2 rounded-lg text-xs font-semibold transition-colors ${
                  statusFilter === tab.id
                    ? 'bg-[var(--color-accent)] text-white'
                    : 'text-[var(--color-text-muted)] hover:bg-[var(--color-surface-alt)]'
                }`}
              >
                {tab.label}
              </button>
            ))}
          </div>

          <div className="p-3">
            <div className="flex items-center gap-2 bg-[var(--color-app-bg)] border border-[var(--color-border)] rounded-lg px-3 py-2">
              <Search size={16} className="text-[var(--color-text-muted)] shrink-0" />
              <input
                type="text"
                placeholder={t('support.searchPlaceholder')}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="flex-1 bg-transparent border-none outline-none text-sm text-[var(--color-text)] placeholder:text-[var(--color-text-muted)]"
              />
            </div>
          </div>

          <div className="flex-1 overflow-y-auto p-3 pt-0 space-y-2">
            {loading && <p className="text-sm text-[var(--color-text-muted)] text-center py-6">{t('common.loading')}</p>}
            {!loading && filteredTickets.length === 0 && (
              <p className="text-sm text-[var(--color-text-muted)] text-center py-6">{t('support.noTickets')}</p>
            )}
            {filteredTickets.map((ticket) => {
              const statusConf = getStatusConfig(ticket.status);
              const isActive = activeTicketId === ticket.id;
              return (
                <button
                  key={ticket.id}
                  onClick={() => setActiveTicketId(ticket.id)}
                  className={`w-full text-start p-4 rounded-xl border transition-colors ${
                    isActive
                      ? 'bg-[var(--color-accent-soft)] border-[var(--color-accent)]/40'
                      : 'bg-[var(--color-app-bg)] border-transparent hover:border-[var(--color-border)]'
                  }`}
                >
                  <div className="flex items-center justify-between mb-1.5">
                    <span className="text-xs font-semibold text-[var(--color-text-muted)] flex items-center gap-1">
                      <User size={12} /> {ticketUserLabel(ticket.user)}
                    </span>
                    <span className="text-[11px] text-[var(--color-text-muted)]">{formatDate(ticket.createdAt)}</span>
                  </div>
                  <h4 className="text-sm font-medium text-[var(--color-text)] mb-2 truncate">{ticket.subject}</h4>
                  <div className="flex items-center justify-between">
                    <span
                      className="flex items-center gap-1 px-2 py-0.5 rounded-full text-[11px] font-semibold"
                      style={{ color: statusConf.color, backgroundColor: statusConf.bg }}
                    >
                      {statusConf.icon} {statusConf.text}
                    </span>
                    {ticket.imageUrl && <ImageIcon size={13} className="text-[var(--color-text-muted)]" />}
                  </div>
                </button>
              );
            })}
          </div>
        </div>

        {/* Ticket detail */}
        <div className="flex-1 bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl flex flex-col overflow-hidden shadow-[var(--shadow-card)]">
          {!activeTicket ? (
            <div className="flex-1 flex items-center justify-center text-sm text-[var(--color-text-muted)]">
              {t('support.selectTicket')}
            </div>
          ) : (
            <>
              <div className="p-6 border-b border-[var(--color-border)]">
                <h2 className="text-lg font-semibold text-[var(--color-text)] mb-1">{activeTicket.subject}</h2>
                <p className="text-xs text-[var(--color-text-muted)]">
                  {t('support.ticketFrom', { id: activeTicket.id, user: ticketUserLabel(activeTicket.user) })}
                </p>
              </div>

              <div className="flex-1 overflow-y-auto p-6 space-y-4 bg-[var(--color-app-bg)]">
                <div className="bg-[var(--color-surface)] border border-[var(--color-border)] rounded-xl p-4">
                  <p className="text-sm text-[var(--color-text)] leading-relaxed whitespace-pre-wrap">{activeTicket.description}</p>
                  {activeTicket.imageUrl && (
                    <img
                      src={resolveImageUrl(activeTicket.imageUrl)}
                      alt="attachment"
                      className="mt-3 max-w-xs rounded-lg border border-[var(--color-border)]"
                    />
                  )}
                </div>

                {repliesLoading && <p className="text-sm text-[var(--color-text-muted)]">{t('common.loading')}</p>}
                {repliesError && (
                  <div className="rounded-xl border border-[var(--color-danger)]/30 bg-[var(--color-danger-soft)] p-3 text-sm text-[var(--color-danger)]">
                    {repliesError}
                  </div>
                )}

                {hasReply && (
                  <div className="space-y-2">
                    <h4 className="text-xs font-bold uppercase text-[var(--color-text-muted)]">{t('support.replyHistory')}</h4>
                    {activeReplies.map((reply) => (
                      <div key={reply.id} className="bg-[var(--color-accent-soft)] border border-[var(--color-accent)]/30 rounded-xl p-4 ms-auto max-w-[80%]">
                        <p className="text-sm text-[var(--color-text)] whitespace-pre-wrap">{reply.replyContent}</p>
                        <p className="text-[11px] text-[var(--color-text-muted)] mt-2">
                          {t('support.replySentAt', { date: formatDate(reply.createdAt) })}
                        </p>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div className="p-5 border-t border-[var(--color-border)] bg-[var(--color-surface)]">
                {sendError && (
                  <div className="mb-3 rounded-xl border border-[var(--color-danger)]/30 bg-[var(--color-danger-soft)] p-3 text-sm text-[var(--color-danger)]">
                    {sendError}
                  </div>
                )}
                {isClosed ? (
                  <p className="text-sm text-[var(--color-text-muted)]">{t('support.resolvedNotice')}</p>
                ) : (
                  <div className="flex items-end gap-3 bg-[var(--color-app-bg)] border border-[var(--color-border)] rounded-xl px-4 py-2">
                    <textarea
                      ref={replyTextareaRef}
                      rows={1}
                      placeholder={t('support.replyPlaceholder')}
                      value={replyText}
                      onChange={handleReplyChange}
                      className="flex-1 bg-transparent border-none outline-none text-sm text-[var(--color-text)] placeholder:text-[var(--color-text-muted)] resize-none py-2 max-h-[140px]"
                    />
                    <button
                      onClick={handleSendClick}
                      disabled={!replyText.trim() || sending}
                      className="flex items-center gap-2 px-4 py-2 bg-[var(--color-accent)] hover:opacity-90 text-white font-semibold text-sm rounded-lg transition-opacity disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      <span>{sending ? t('support.sending') : t('support.sendReply')}</span>
                      <Send size={16} />
                    </button>
                  </div>
                )}
              </div>
            </>
          )}
        </div>
      </div>

      <SendConfirmDialog
        isOpen={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        onConfirm={handleConfirmSend}
        targetName={activeTicket ? ticketUserLabel(activeTicket.user) : ''}
      />
    </div>
  );
}
