import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import en from './locales/en/translation.json';
import ar from './locales/ar/translation.json';

export const RTL_LANGUAGES = ['ar'];
const LANG_KEY = 'lang';

export function getStoredLanguage() {
  const stored = localStorage.getItem(LANG_KEY);
  return stored === 'ar' || stored === 'en' ? stored : 'en';
}

export function applyDocumentDirection(lang) {
  document.documentElement.lang = lang;
  document.documentElement.dir = RTL_LANGUAGES.includes(lang) ? 'rtl' : 'ltr';
}

const initialLanguage = getStoredLanguage();
applyDocumentDirection(initialLanguage);

i18n.use(initReactI18next).init({
  resources: {
    en: { translation: en },
    ar: { translation: ar },
  },
  lng: initialLanguage,
  fallbackLng: 'en',
  interpolation: { escapeValue: false },
});

i18n.on('languageChanged', (lang) => {
  localStorage.setItem(LANG_KEY, lang);
  applyDocumentDirection(lang);
});

export default i18n;
