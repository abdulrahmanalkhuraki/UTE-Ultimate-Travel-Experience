import { apiGet } from './apiClient';

// GET /api/Admin/dashboard
export const getMainDashboard = () => apiGet('/api/Admin/dashboard');

// GET /api/Admin/dashboard/tourists
export const getTouristsDashboard = () => apiGet('/api/Admin/dashboard/tourists');

// GET /api/Admin/dashboard/companies
export const getCompaniesDashboard = () => apiGet('/api/Admin/dashboard/companies');

// GET /api/Admin/dashboard/companies/:companyId
export const getCompanyDetails = (companyId) =>
  apiGet(`/api/Admin/dashboard/companies/${companyId}`);

// GET /api/Admin/dashboard/tour-packages
export const getTourPackagesDashboard = () => apiGet('/api/Admin/dashboard/tour-packages');

// GET /api/Admin/dashboard/financial
export const getFinancialDashboard = () => apiGet('/api/Admin/dashboard/financial');

// GET /api/Admin/dashboard/companies/financial?page=&pageSize=
export const getCompaniesFinancial = (page = 1, pageSize = 20) =>
  apiGet('/api/Admin/dashboard/companies/financial', { page, pageSize });

// GET /api/Admin/dashboard/companies/:companyId/tour-packages/financial?page=&pageSize=
// ملاحظة: المسار الدقيق مو مكتوب صراحة بملف الـ API (بس البارامترات موصوفة) — خمّنته حسب نمط بقية الـ endpoints، تأكد منه معي.
export const getTourPackagesFinancial = (companyId, page = 1, pageSize = 20) =>
  apiGet(`/api/Admin/dashboard/companies/${companyId}/tour-packages/financial`, { page, pageSize });

// GET /api/TourCompany/pending
export const getPendingTourCompanies = () => apiGet('/api/TourCompany/pending');

// GET /api/TourCompany
export const getTourCompanies = () => apiGet('/api/TourCompany');

// GET /api/TourPackage?page=&pageSize=
export const getTourPackages = (page = 1, pageSize = 20) =>
  apiGet('/api/TourPackage', { page, pageSize });

// GET /api/User/filter?roleName=
export const getUsersByRole = (roleName) => apiGet('/api/User/filter', { roleName });

// GET /api/User/deleted
export const getDeletedUsers = () => apiGet('/api/User/deleted');

// GET /api/User/:id
export const getUserById = (id) => apiGet(`/api/User/${id}`);
