import { API_BASE_URL } from '../config/constants';

async function request(path, { params, ...options } = {}) {
  const url = new URL(`${API_BASE_URL}${path}`);
  if (params) {
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        url.searchParams.set(key, value);
      }
    });
  }

  const token = localStorage.getItem('authToken');

  const res = await fetch(url.toString(), {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });

  if (!res.ok) {
    throw new Error(`API ${res.status}: ${res.statusText} (${path})`);
  }

  return res.json();
}

export function apiGet(path, params) {
  return request(path, { method: 'GET', params });
}
