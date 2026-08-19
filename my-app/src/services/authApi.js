import { apiPostForm } from './apiClient';

// POST /api/Auth/login
export const login = (email, password) =>
  apiPostForm('/api/Auth/login', { Email: email, Password: password });
