import { useEffect, useState } from 'react';

// هوك بسيط لجلب بيانات الداشبورد: بيرجع {data, loading, error}
export function useApiData(fetcher, deps = []) {
  const [state, setState] = useState({ data: null, loading: true, error: null });

  useEffect(() => {
    let cancelled = false;

    fetcher()
      .then((result) => {
        if (!cancelled) setState({ data: result, loading: false, error: null });
      })
      .catch((err) => {
        if (!cancelled) setState({ data: null, loading: false, error: err });
      });

    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, deps);

  return state;
}
