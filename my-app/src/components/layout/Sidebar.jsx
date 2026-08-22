import { Home as HomeIcon, Users as UsersIcon, Map, Building2, Wallet, HeadphonesIcon, Compass } from 'lucide-react';
import { useTranslation } from 'react-i18next';

const NAV_ITEMS = [
  { id: 'Home', labelKey: 'nav.home', icon: HomeIcon },
  { id: 'Users', labelKey: 'nav.tourists', icon: UsersIcon },
  { id: 'Group Trips', labelKey: 'nav.tourPackages', icon: Map },
  { id: 'Companies', labelKey: 'nav.companies', icon: Building2 },
  { id: 'Financials', labelKey: 'nav.financials', icon: Wallet },
  { id: 'Support', labelKey: 'nav.support', icon: HeadphonesIcon },
];

export default function Sidebar({ activeMenu, onSelect }) {
  const { t } = useTranslation();

  return (
    <aside className="w-64 bg-[var(--color-surface)] border-e border-[var(--color-border)] flex-col justify-between hidden md:flex shrink-0">
      <div>
        <div className="p-6 flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-[var(--color-accent-soft)] flex items-center justify-center shrink-0">
            <Compass className="w-5 h-5 text-[var(--color-accent)]" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-[var(--color-text)] tracking-wide leading-tight">
              {t('app.name')}
            </h1>
            <p className="text-[11px] text-[var(--color-text-muted)] tracking-widest uppercase">
              {t('app.subtitle')}
            </p>
          </div>
        </div>

        <nav className="px-4 space-y-1.5 mt-4">
          {NAV_ITEMS.map((item) => {
            const isActive = activeMenu === item.id;
            return (
              <button
                key={item.id}
                onClick={() => onSelect(item.id)}
                aria-current={isActive ? 'page' : undefined}
                className={`w-full flex items-center px-4 py-3 rounded-lg transition-colors duration-150 ${
                  isActive
                    ? 'bg-[var(--color-accent-soft)] text-[var(--color-accent)] border border-[var(--color-accent)]/40'
                    : 'text-[var(--color-text-muted)] hover:text-[var(--color-text)] hover:bg-[var(--color-surface-alt)] border border-transparent'
                }`}
              >
                <item.icon className="w-5 h-5 me-3 shrink-0" />
                <span className="font-medium text-sm">{t(item.labelKey)}</span>
              </button>
            );
          })}
        </nav>
      </div>
    </aside>
  );
}

export { NAV_ITEMS };
