import { apiPost } from './apiClient';

// POST /api/Auth/login
export const login = (email, password) => apiPost('/api/Auth/login', { email, password });
