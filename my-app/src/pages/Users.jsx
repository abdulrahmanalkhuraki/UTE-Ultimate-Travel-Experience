import { useState } from 'react';
import UserDetails from './UserDetails';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { ChevronLeft, ChevronRight, UsersRound, UserRoundX, TrendingUp, UserRound, MapPin, Calendar } from 'lucide-react';
import { useApiData } from '../hooks/useApiData';
import { getTouristsDashboard, getUsersByRole, getDeletedUsers } from '../services/dashboardApi';
import { timeAgo, formatDate, calcAge } from '../utils/format';

const FALLBACK_AVATAR =
  "data:image/svg+xml;utf8," +
  "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 100 100'>" +
  "<rect width='100' height='100' fill='%232a2a2a'/>" +
  "<g fill='%2391B3FA' opacity='0.7'>" +
  "<circle cx='50' cy='38' r='20'/>" +
  "<path d='M50 62c-21 0-36 12-38 32h76c-3-20-18-32-38-32z'/>" +
  "</g></svg>";

// GET /api/User (أو /api/User/deleted) -> نفس شكل عنصر اليوزر
function mapApiUser(u) {
  return {
    id: u.id,
    name: `${u.firstName ?? ''} ${u.lastName ?? ''}`.trim() || '—',
    age: calcAge(u.dateOfBirth) ?? '—',
    gender: u.gender || '—',
    email: u.email || '—',
    phone: u.phone || '—',
    companions: '—',
    programs: '—',
    companies: '—',
    nationality: u.nationalityCountryName || '—',
    location: u.residentialCityName || u.nationalityCountryName || '—',
    joined: timeAgo(u.createdAtUtc),
    avatar: u.image || FALLBACK_AVATAR,
    nationalId: u.nationalNumber || '—',
    passportNo: u.passportNumber || '—',
    nationalIdImage: u.nationalIdImage,
    passportImage: u.passportImage,
    joinDate: formatDate(u.createdAtUtc),
  };
}

const LoadingBlock = () => (
  <div className="flex items-center justify-center py-12">
    <div className="w-8 h-8 border-4 border-[var(--color-border)] border-t-[var(--color-accent)] rounded-full animate-spin" />
  </div>
);

const ErrorBlock = ({ message }) => (
  <div className="flex items-center justify-center py-10 text-center">
    <p className="text-sm text-red-400 bg-red-400/10 border border-red-400/20 rounded-xl px-4 py-3">
      Failed to load: {message}
    </p>
  </div>
);

const EmptyBlock = ({ text }) => (
  <p className="text-sm text-[var(--color-text-muted)] text-center py-10">{text}</p>
);

export default function Users() {
  const [selectedUser, setSelectedUser] = useState(null);
  const [allUsersPage, setAllUsersPage] = useState(1);
  const [allUsersPageSize] = useState(12);

  const { data: touristsData, loading: chartLoading, error: chartError } = useApiData(getTouristsDashboard, []);
  const { data: allUsersData, loading: allUsersLoading, error: allUsersError } = useApiData(
    () => getUsersByRole('Tourist', allUsersPage, allUsersPageSize),
    [allUsersPage, allUsersPageSize]
  );
  const { data: deletedUsersData, loading: deletedLoading, error: deletedError } = useApiData(getDeletedUsers, []);

  // GET /api/Admin/dashboard/tourists -> touristGrowth
  const chartData = (touristsData?.touristGrowth ?? []).map((g) => ({
    name: g.month,
    tourists: g.count,
  }));

  const pagination = allUsersData?.pagination ?? {};
  const allUsers = (allUsersData?.items ?? []).map(mapApiUser);
  const deletedUsers = (deletedUsersData?.users ?? []).map(mapApiUser);

  const UserCard = ({ user, isClickable = false }) => (
    <div
      onClick={() => isClickable && setSelectedUser(user)}
      className={`flex items-center gap-5 bg-[var(--color-surface)] border border-[var(--color-border)] p-5 rounded-2xl transition shadow-lg hover:border-[var(--color-accent)]/40 ${isClickable ? 'cursor-pointer' : ''}`}
    >
      <div className="w-16 h-16 rounded-full overflow-hidden border-2 border-[var(--color-border)] flex-shrink-0">
        <img src={user.avatar} alt={user.name} className="w-full h-full object-cover" />
      </div>

      <div className="flex-1 min-w-0">
        <h4 className="text-base font-bold text-[var(--color-text)] truncate">{user.name}</h4>

        <div className="flex flex-wrap items-center gap-x-4 gap-y-1 mt-1.5 text-xs">
          <span className="flex items-center gap-1.5 text-[var(--color-text-muted)]">
            <UserRound className="w-3.5 h-3.5" /> Age: {user.age}
          </span>
          <span className="flex items-center gap-1.5 text-[var(--color-accent)]">
            <MapPin className="w-3.5 h-3.5" /> {user.nationality}
          </span>
          <span className="flex items-center gap-1.5 text-[var(--color-text-muted)]">
            <Calendar className="w-3.5 h-3.5" /> Joined: {user.joinDate}
          </span>
        </div>
      </div>
    </div>
  );

  if (selectedUser) {
    return <UserDetails user={selectedUser} onBack={() => setSelectedUser(null)} />;
  }

  return (
    <div className="p-8 space-y-8 font-sans">
      {/* 1. TOP SECTION: Tourist Growth Graph (full-width) */}
      <section className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 p-6 rounded-2xl shadow-lg">
        <h3 className="text-lg font-semibold text-[var(--color-text)] mb-4 text-left">Tourist Growth Analysis</h3>

        {chartError ? (
          <ErrorBlock message={chartError.message} />
        ) : (
          <div className="w-full h-64 mb-6">
            {chartLoading ? (
              <LoadingBlock />
            ) : (
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
                  <XAxis dataKey="name" stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
                  <YAxis stroke="var(--color-text-muted)" tick={{ fill: 'var(--color-text-muted)', fontSize: 12 }} />
                  <Tooltip contentStyle={{ backgroundColor: 'var(--color-surface)', borderColor: 'var(--color-accent)', border: '1px solid var(--color-accent)', borderRadius: '0.8rem' }} />
                  <Legend iconType="plainline" />
                  <Line type="monotone" dataKey="tourists" stroke="var(--color-accent)" strokeWidth={3} dot={true} />
                </LineChart>
              </ResponsiveContainer>
            )}
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 pt-6 border-t border-[var(--color-border)]">
          <div className="flex items-center gap-4 bg-[var(--color-app-bg)] p-4 rounded-xl border border-[var(--color-border)]">
            <UsersRound className="w-8 h-8 text-[var(--color-accent)]" />
            <div>
              <p className="text-[10px] text-[var(--color-text-muted)] font-bold uppercase tracking-wider">Active Registered</p>
              <p className="text-xl font-bold text-[var(--color-accent)] mt-0.5">{touristsData ? touristsData.activeTourists.toLocaleString() : '—'}</p>
            </div>
          </div>
          <div className="flex items-center gap-4 bg-[var(--color-app-bg)] p-4 rounded-xl border border-[var(--color-border)]">
            <UserRoundX className="w-8 h-8 text-red-400" />
            <div>
              <p className="text-[10px] text-[var(--color-text-muted)] font-bold uppercase tracking-wider">Deleted Accounts</p>
              <p className="text-xl font-bold text-red-400 mt-0.5">{touristsData ? touristsData.deletedTourists.toLocaleString() : '—'}</p>
            </div>
          </div>
          <div className="flex items-center gap-4 bg-[var(--color-app-bg)] p-4 rounded-xl border border-[var(--color-border)]">
            <TrendingUp className="w-8 h-8 text-[var(--color-success)]" />
            <div>
              <p className="text-[10px] text-[var(--color-text-muted)] font-bold uppercase tracking-wider">Peak Records</p>
              <p className="text-xl font-bold text-[var(--color-success)] mt-0.5">{touristsData ? touristsData.totalTourists.toLocaleString() : '—'}</p>
            </div>
          </div>
        </div>
      </section>

      {/* 2. MIDDLE SECTION: Deleted Users list */}
      <section className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-2xl p-6 shadow-lg">
        <div className="flex items-center justify-between mb-5 border-b border-[var(--color-border)] pb-4">
          <div className="flex items-center gap-4">
            <UserRoundX className="w-7 h-7 text-red-400" />
            <h3 className="text-lg font-semibold text-red-400">Deleted Accounts</h3>
          </div>
          <span className="bg-red-400/10 text-red-400 text-xs font-bold px-3 py-1.5 rounded-full">
            {deletedUsersData?.totalCount ?? (deletedLoading ? '…' : 0)} Deleted
          </span>
        </div>

        {deletedError ? (
          <ErrorBlock message={deletedError.message} />
        ) : deletedLoading ? (
          <LoadingBlock />
        ) : deletedUsers.length === 0 ? (
          <EmptyBlock text="No deleted users found." />
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
            {deletedUsers.map((user) => (
              <UserCard key={user.id} user={user} />
            ))}
          </div>
        )}
      </section>

      {/* 3. BOTTOM SECTION: All Users grid */}
      <section className="bg-[var(--color-surface)] border border-[var(--color-border)]/30 rounded-2xl p-6 shadow-lg">
        <div className="flex items-center justify-between mb-5 border-b border-[var(--color-border)] pb-4">
          <div className="flex items-center gap-4">
            <UsersRound className="w-7 h-7 text-[var(--color-accent)]" />
            <h3 className="text-lg font-semibold text-[var(--color-accent)]">ALL Users</h3>
          </div>
          <span className="bg-[var(--color-accent)]/10 text-[var(--color-accent)] text-xs font-bold px-3 py-1.5 rounded-full">
            {pagination.totalItems ?? (allUsersLoading ? '…' : 0)} Total
          </span>
        </div>

        {allUsersError ? (
          <ErrorBlock message={allUsersError.message} />
        ) : allUsersLoading ? (
          <LoadingBlock />
        ) : allUsers.length === 0 ? (
          <EmptyBlock text="No users found." />
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
              {allUsers.map((user) => (
                <UserCard key={user.id} user={user} isClickable />
              ))}
            </div>

            <div className="flex items-center justify-between mt-6 pt-5 border-t border-[var(--color-border)]">
              <p className="text-xs text-[var(--color-text-muted)]">
                Page {pagination.page || '—'} of {pagination.totalPages || '—'}
              </p>
              <div className="flex items-center gap-3">
                <button
                  onClick={() => setAllUsersPage((p) => Math.max(1, p - 1))}
                  disabled={!pagination.hasPreviousPage}
                  className="flex items-center gap-1.5 px-4 py-2 rounded-xl border border-[var(--color-border)] bg-[var(--color-app-bg)] text-sm font-medium text-[var(--color-text-muted)] hover:border-[var(--color-accent)]/40 hover:text-[var(--color-text)] transition disabled:opacity-40 disabled:cursor-not-allowed"
                >
                  <ChevronLeft className="w-4 h-4" /> Prev
                </button>
                <button
                  onClick={() => setAllUsersPage((p) => p + 1)}
                  disabled={!pagination.hasNextPage}
                  className="flex items-center gap-1.5 px-4 py-2 rounded-xl border border-[var(--color-border)] bg-[var(--color-app-bg)] text-sm font-medium text-[var(--color-text-muted)] hover:border-[var(--color-accent)]/40 hover:text-[var(--color-text)] transition disabled:opacity-40 disabled:cursor-not-allowed"
                >
                  Next <ChevronRight className="w-4 h-4" />
                </button>
              </div>
            </div>
          </>
        )}
      </section>
    </div>
  );
}