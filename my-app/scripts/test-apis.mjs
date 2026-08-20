// سكربت تشخيصي: بيسجل دخول وبيجرب كل الـ APIs المربوطة بالداشبورد وحدة وحدة،
// وبيطبع تقرير واحد قابل للنسخ واللصق (status code + مقتطف من الرد لكل واحد).
//
// مهم: شغّله من جهاز عنده وصول مباشر لرابط الباك اند (API_BASE_URL بملف
// src/config/constants.js) — يعني بدون VPN عم يحجب الشبكة المحلية. أسهل مكان
// تشغّله فيه هو نفس جهاز الباك اند نفسه (بيصير الاتصال عبر localhost).
//
// الاستخدام:
//   node scripts/test-apis.mjs <email> <password>

// تعطيل التحقق من شهادة HTTPS (self-signed) — مقبول هون لأنه سكربت تشخيص محلي بس
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

import { API_BASE_URL } from '../src/config/constants.js';

const [, , email, password] = process.argv;

if (!email || !password) {
  console.error('الاستخدام: node scripts/test-apis.mjs <email> <password>');
  process.exit(1);
}

let token = '';

function statusLabel(status) {
  if (status >= 200 && status < 300) return '✅ OK';
  if (status === 401) return '🔒 401 Unauthorized';
  if (status === 403) return '⛔ 403 Forbidden';
  if (status === 404) return '❌ 404 Not Found';
  if (status >= 500) return '💥 Server Error';
  return `⚠️ ${status}`;
}

async function call(method, path, { params, form, auth = true } = {}) {
  const url = new URL(`${API_BASE_URL}${path}`);
  if (params) {
    Object.entries(params).forEach(([k, v]) => {
      if (v !== undefined && v !== null) url.searchParams.set(k, v);
    });
  }

  const options = { method, headers: {} };
  if (auth && token) options.headers.Authorization = `Bearer ${token}`;

  if (form) {
    const fd = new FormData();
    Object.entries(form).forEach(([k, v]) => fd.append(k, v));
    options.body = fd;
  }

  console.log(`${method} ${path}${params ? ' ?' + new URLSearchParams(params).toString() : ''}`);

  // بحد أقصى 8 ثواني لكل طلب، حتى ما يعلّق السكربت كامل لو endpoint معين مش راد
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), 8000);

  let res;
  let bodyText = '';
  try {
    res = await fetch(url.toString(), { ...options, signal: controller.signal });
    bodyText = await res.text();
  } catch (err) {
    const reason = err.name === 'AbortError' ? 'انتهت المهلة (8 ثواني) بدون رد' : err.message;
    console.log(`  💥 تعذر الاتصال: ${reason}`);
    console.log('');
    return null;
  } finally {
    clearTimeout(timeoutId);
  }

  console.log(`  ${statusLabel(res.status)} (${res.status})`);
  const preview = bodyText.length > 300 ? bodyText.slice(0, 300) + '...' : bodyText;
  console.log(`  ${preview || '(no body)'}`);
  console.log('');

  try {
    return JSON.parse(bodyText);
  } catch {
    return null;
  }
}

async function main() {
  console.log(`== قاعدة الرابط: ${API_BASE_URL} ==\n`);

  console.log('--- 1) تسجيل الدخول ---');
  const loginResult = await call('POST', '/api/Auth/login', { form: { email, password }, auth: false });
  if (loginResult?.token) {
    token = loginResult.token;
    console.log('  🔑 توكن تم الحصول عليه بنجاح.\n');
  } else {
    console.log('  ⚠️ ما انجاب توكن — الطلبات الجاية رح تترسل بدون Authorization.\n');
  }

  console.log('--- 2) Admin Dashboard ---');
  await call('GET', '/api/Admin/dashboard');
  await call('GET', '/api/Admin/dashboard/tourists');
  await call('GET', '/api/Admin/dashboard/companies');
  await call('GET', '/api/Admin/dashboard/tour-packages');
  await call('GET', '/api/Admin/dashboard/financial');
  await call('GET', '/api/Admin/dashboard/companies/financial', { params: { page: 1, pageSize: 5 } });

  console.log('--- 3) Tour Companies ---');
  const tourCompanies = await call('GET', '/api/TourCompany');
  await call('GET', '/api/TourCompany/pending');

  const sampleCompanyId = tourCompanies?.[0]?.id;
  if (sampleCompanyId) {
    console.log(`  (استخدمت companyId = ${sampleCompanyId} من نتيجة /api/TourCompany)\n`);
    await call('GET', `/api/Admin/dashboard/companies/${sampleCompanyId}`);
    await call('GET', `/api/Admin/dashboard/companies/${sampleCompanyId}/tour-packages/financial`, {
      params: { page: 1, pageSize: 5 },
    });
  } else {
    console.log('  ⚠️ ما لقيت أي companyId من /api/TourCompany لتجربة endpoints تفاصيل الشركة.\n');
  }

  console.log('--- 4) Tour Packages ---');
  await call('GET', '/api/TourPackage', { params: { page: 1, pageSize: 5 } });

  console.log('--- 5) Users ---');
  const touristUsers = await call('GET', '/api/User/filter', { params: { roleName: 'Tourist' } });
  await call('GET', '/api/User/deleted');

  const sampleUserId = touristUsers?.[0]?.id;
  if (sampleUserId) {
    console.log(`  (استخدمت userId = ${sampleUserId} من نتيجة /api/User/filter)\n`);
    await call('GET', `/api/User/${sampleUserId}`);
  } else {
    console.log('  ⚠️ ما لقيت أي userId من /api/User/filter لتجربة /api/User/:id.\n');
  }

  console.log('== خلص الفحص. انسخ كل الـ output يلي فوق وابعتلي ياه. ==');
}

main();
