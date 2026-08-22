import { apiGet, apiPost } from './apiClient';

export const TICKET_STATUS = {
  OPEN: 0,
  CLOSED: 1,
};

// GET /api/Ticket?userId= (optional) -> TicketResponse[]
export const getTickets = (userId) => apiGet('/api/Ticket', userId ? { userId } : undefined);

// GET /api/SupportReply/:ticketId -> SupportReplyResponse[]
export const getSupportReplies = (ticketId) => apiGet(`/api/SupportReply/${ticketId}`);

// POST /api/SupportReply  body: { ticketId, replyContent }
export const sendSupportReply = (ticketId, replyContent) =>
  apiPost('/api/SupportReply', { ticketId, replyContent });
