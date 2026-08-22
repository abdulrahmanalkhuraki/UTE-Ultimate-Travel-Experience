import { useTranslation } from 'react-i18next';
import { Languages } from 'lucide-react';

const LANGUAGES = [
  { code: 'en', label: 'EN' },
  { code: 'ar', label: 'AR' },
];

export default function LanguageSwitcher() {
  const { i18n, t } = useTranslation();

  return (
    <div
      className="flex items-center gap-1 rounded-lg border border-[var(--color-border)] bg-[var(--color-surface-alt)] p-1"
      role="group"
      aria-label={t('header.language')}
    >
      <Languages className="w-4 h-4 ms-1.5 text-[var(--color-text-muted)]" aria-hidden="true" />
      {LANGUAGES.map((lang) => (
        <button
          key={lang.code}
          type="button"
          onClick={() => i18n.changeLanguage(lang.code)}
          aria-pressed={i18n.resolvedLanguage === lang.code}
          className={`px-2.5 py-1 rounded-md text-xs font-semibold transition-colors ${
            i18n.resolvedLanguage === lang.code
              ? 'bg-[var(--color-accent)] text-white'
              : 'text-[var(--color-text-muted)] hover:text-[var(--color-text)]'
          }`}
        >
          {lang.label}
        </button>
      ))}
    </div>
  );
}
