const TOKEN_KEY = 'authToken';
const USER_KEY = 'authUser';

// بيتخزن بعد نجاح POST /api/Auth/login
export function saveSession({ token, ...user }) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function getStoredUser() {
  try {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

export function hasSession() {
  return !!localStorage.getItem(TOKEN_KEY);
}

export function clearSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}
