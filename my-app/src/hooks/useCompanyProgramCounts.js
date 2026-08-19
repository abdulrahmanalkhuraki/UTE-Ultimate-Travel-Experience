import { useState, useEffect } from 'react';
import { getCompanyDetails } from '../services/dashboardApi';

// GET /api/Admin/dashboard/companies/:companyId -> totalTourPackages
// بيرجع {[companyId]: totalTourPackages} لقائمة شركات، مشان نعبي "Published Programs" بكروت القوائم
export function useCompanyProgramCounts(companyIds) {
  const [counts, setCounts] = useState({});
  const key = companyIds.join(',');

  useEffect(() => {
    if (!key) return undefined;
    let cancelled = false;

    Promise.all(
      key.split(',').map((id) =>
        getCompanyDetails(id)
          .then((d) => [id, d.totalTourPackages])
          .catch(() => [id, undefined])
      )
    ).then((pairs) => {
      if (!cancelled) {
        setCounts(Object.fromEntries(pairs.filter(([, v]) => v !== undefined)));
      }
    });

    return () => {
      cancelled = true;
    };
  }, [key]);

  return counts;
}
