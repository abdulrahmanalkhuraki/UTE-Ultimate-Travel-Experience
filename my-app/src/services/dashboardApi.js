import { apiGet, apiPost } from './apiClient';

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
export const getTourPackagesFinancial = (companyId, page = 1, pageSize = 20) =>
  apiGet(`/api/Admin/dashboard/companies/${companyId}/tour-packages/financial`, { page, pageSize });

// GET /api/TourCompany/pending
export const getPendingTourCompanies = () => apiGet('/api/TourCompany/pending');

// GET /api/TourCompany (Admin only — يحتاج بروفايل مكتمل + دور Admin)
export const getTourCompanies = () => apiGet('/api/TourCompany');

// POST /api/TourCompany/:id/approve
export const approveTourCompany = (id) => apiPost(`/api/TourCompany/${id}/approve`);

// POST /api/TourCompany/:id/reject  body: { reason }
export const rejectTourCompany = (id, reason) => apiPost(`/api/TourCompany/${id}/reject`, { reason });

// GET /api/TourPackage?page=&pageSize= (Admin only، كل البرامج بكل الحالات)
export const getTourPackages = (page = 1, pageSize = 20) =>
  apiGet('/api/TourPackage', { page, pageSize });

// GET /api/TourPackage/unApproved?page=&pageSize= (Admin only — البرامج قيد المراجعة فقط)
export const getUnapprovedTourPackages = (page = 1, pageSize = 100) =>
  apiGet('/api/TourPackage/unApproved', { page, pageSize });

// POST /api/TourPackage/:id/approve
export const approveTourPackage = (id) => apiPost(`/api/TourPackage/${id}/approve`);

// POST /api/TourPackage/:id/reject  body: { reason }
export const rejectTourPackage = (id, reason) => apiPost(`/api/TourPackage/${id}/reject`, { reason });

// GET /api/User?page=&pageSize= -> PaginatedResponse<UserResponse>
export const getAllUsers = (page = 1, pageSize = 12) =>
  apiGet('/api/User', { page, pageSize });

// GET /api/User/filter?roleName=&page=&pageSize= -> PaginatedResponse<UserResponse>
export const getUsersByRole = (roleName, page = 1, pageSize = 100) =>
  apiGet('/api/User/filter', { roleName, page, pageSize });

// GET /api/User/deleted -> { totalCount, users }
export const getDeletedUsers = () => apiGet('/api/User/deleted');

// GET /api/User/:id
export const getUserById = (id) => apiGet(`/api/User/${id}`);

// GET /api/Booking/UserBookings/:touristId?page=&pageSize= -> PaginatedUserBookingsResponse
export const getUserBookings = (touristId, page = 1, pageSize = 10) =>
  apiGet(`/api/Booking/UserBookings/${touristId}`, { page, pageSize });

// GET /api/Companion/UserCompanions/:userId?page=&pageSize= -> PaginatedResponse<CompanionResponse>
export const getUserCompanions = (userId, page = 1, pageSize = 20) =>
  apiGet(`/api/Companion/UserCompanions/${userId}`, { page, pageSize });
