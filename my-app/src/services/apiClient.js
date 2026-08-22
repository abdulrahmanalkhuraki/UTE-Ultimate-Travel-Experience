import { API_BASE_URL } from '../config/constants';

function getCurrentLang() {
  const stored = localStorage.getItem('lang');
  return stored === 'ar' || stored === 'en' ? stored : 'en';
}

async function request(path, { params, isFormData, ...options } = {}) {
  const url = new URL(`${API_BASE_URL}${path}`);
  if (params) {
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        url.searchParams.set(key, value);
      }
    });
  }
  url.searchParams.set('lang', getCurrentLang());

  const token = localStorage.getItem('authToken');

  const res = await fetch(url.toString(), {
    ...options,
    headers: {
      // لما يكون الجسم FormData ما منحط Content-Type يدوياً — المتصفح
      // بيضيفه لحاله مع الـ boundary الصح (multipart/form-data; boundary=...)
      ...(isFormData ? {} : { 'Content-Type': 'application/json' }),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  });

  if (!res.ok) {
    let message = `API ${res.status}: ${res.statusText} (${path})`;
    try {
      const data = await res.clone().json();
      if (typeof data === 'string' && data) {
        message = data;
      } else if (data?.errors) {
        // شكل ValidationProblemDetails تبع ASP.NET: { errors: { Email: ["Email is required."] } }
        message = Object.values(data.errors).flat().join(' ');
      } else if (data?.detail) {
        message = data.detail;
      } else if (data?.message) {
        message = data.message;
      } else if (data?.title) {
        message = data.title;
      }
    } catch {
      // ما في جسم JSON بالرد، منستخدم الرسالة الافتراضية
    }
    throw new Error(message);
  }

  return res.json();
}

export function apiGet(path, params) {
  return request(path, { method: 'GET', params });
}

export function apiPost(path, body) {
  return request(path, { method: 'POST', body: JSON.stringify(body) });
}

// بيبعت الجسم كـ multipart/form-data بدل JSON — لأي endpoint بالباك محتاج [FromForm]
export function apiPostForm(path, fields) {
  const formData = new FormData();
  Object.entries(fields).forEach(([key, value]) => {
    if (value !== undefined && value !== null) formData.append(key, value);
  });
  return request(path, { method: 'POST', body: formData, isFormData: true });
}
