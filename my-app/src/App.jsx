import { useState } from 'react';
import Layout from './components/layout/Layout';
import Login from './pages/Login';
import Home from './pages/Home';
import Users from './pages/Users';
import Companies from './pages/Companies';
import TourPackages from './pages/TourPackages';
import Support from './pages/Support';
import Financials from './pages/Financials';
import AdminProfile from './pages/AdminProfile';
import { hasSession, getStoredUser } from './utils/auth';

export default function App() {
  const [activeMenu, setActiveMenu] = useState('Home');
  const [isAuthenticated, setIsAuthenticated] = useState(hasSession);
  const [currentUser, setCurrentUser] = useState(getStoredUser);

  if (!isAuthenticated) {
    return (
      <Login
        onLoginSuccess={(user) => {
          setCurrentUser(user);
          setIsAuthenticated(true);
        }}
      />
    );
  }

  return (
    <Layout
      activeMenu={activeMenu}
      onSelectMenu={setActiveMenu}
      currentUser={currentUser}
      onProfileClick={() => setActiveMenu('AdminProfile')}
    >
      {activeMenu === 'Home' && <Home />}
      {activeMenu === 'Users' && <Users />}
      {activeMenu === 'Companies' && <Companies />}
      {activeMenu === 'Group Trips' && <TourPackages />}
      {activeMenu === 'Support' && <Support />}
      {activeMenu === 'Financials' && <Financials />}
      {activeMenu === 'AdminProfile' && <AdminProfile />}
    </Layout>
  );
}
