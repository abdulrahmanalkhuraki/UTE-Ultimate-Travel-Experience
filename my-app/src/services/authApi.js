import { apiPostForm } from './apiClient';

// POST /api/Auth/login (multipart/form-data)
export const login = (email, password) => apiPostForm('/api/Auth/login', { email, password });
