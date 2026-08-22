import Sidebar from './Sidebar';
import Header from './Header';

export default function Layout({ activeMenu, onSelectMenu, currentUser, onProfileClick, children }) {
  return (
    <div className="min-h-screen bg-[var(--color-app-bg)] text-[var(--color-text)] font-sans flex overflow-hidden">
      <Sidebar activeMenu={activeMenu} onSelect={onSelectMenu} />

      <main className="flex-1 flex flex-col h-screen overflow-y-auto">
        <Header currentUser={currentUser} onProfileClick={onProfileClick} />
        {children}
      </main>
    </div>
  );
}
