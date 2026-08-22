import { useTranslation } from 'react-i18next';
import LanguageSwitcher from '../LanguageSwitcher';
import ThemeToggle from '../ThemeToggle';

export default function Header({ currentUser, onProfileClick }) {
  const { t } = useTranslation();
  const displayName = currentUser ? `${currentUser.firstName} ${currentUser.lastName}` : 'Admin';
  const today = new Date().toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });

  return (
    <header className="flex items-center justify-between gap-4 px-6 md:px-8 py-4 bg-[var(--color-app-bg)]/90 sticky top-0 z-10 backdrop-blur-md border-b border-[var(--color-border)]">
      <div className="min-w-0">
        <h2 className="text-xl md:text-2xl font-semibold text-[var(--color-text)] truncate">
          {t('header.welcome', { name: displayName })}
        </h2>
        <p className="text-xs text-[var(--color-text-muted)] mt-0.5">{today}</p>
      </div>

      <div className="flex items-center gap-3 shrink-0">
        <LanguageSwitcher />
        <ThemeToggle />
        <button
          onClick={onProfileClick}
          title={t('header.profile')}
          className="w-11 h-11 rounded-full border-2 border-[var(--color-accent)] overflow-hidden cursor-pointer shrink-0 hover:opacity-90 transition-opacity"
        >
          <img
            src={currentUser?.image || 'https://i.pravatar.cc/150?img=11'}
            alt={displayName}
            className="w-full h-full object-cover"
          />
        </button>
      </div>
    </header>
  );
}
