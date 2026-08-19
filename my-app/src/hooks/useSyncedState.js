import { useState } from 'react';

// حالة محلية (قابلة للتعديل مباشرة، متل الحذف بعد قبول/رفض) بس بتنعمل sync تلقائي
// كل ما تتغير مصدر البيانات (مثلاً نتيجة جديدة من useApiData). بيعتمد نمط
// "Adjusting state when a prop changes" الموصى فيه من React بدل useEffect+setState.
export function useSyncedState(source, mapFn) {
  const [prevSource, setPrevSource] = useState(source);
  const [state, setState] = useState(() => mapFn(source));

  if (source !== prevSource) {
    setPrevSource(source);
    setState(mapFn(source));
  }

  return [state, setState];
}
