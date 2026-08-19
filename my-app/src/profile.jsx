// src/adminProfile.jsx
import  { useState } from 'react';
import {
  User,
  ShieldCheck,
  Key,
  LogOut,
  Bell,
  Settings,
  Users,
  PieChart,
  BarChart2,
  //Clock,
  FileText,
  //CheckCircle,
  //XCircle,
  Download,
} from 'lucide-react';

/**
 * AdminProfile component
 * - متوافق مع Tailwind classes في مشروعك
 * - استبدل بيانات الـ sample بالـ props أو استدعاءات API عند الحاجة
 */

function KPI({ icon: Icon, label, value, delta }) {
  return (
    <div className="bg-[#141414] border border-[#262626] rounded-lg p-4 flex items-center justify-between">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-md bg-[#0f1724] flex items-center justify-center">
          <Icon className="w-5 h-5 text-[#91B3FA]" />
        </div>
        <div>
          <div className="text-xs text-gray-400">{label}</div>
          <div className="text-lg font-semibold text-white">{value}</div>
        </div>
      </div>
      <div className={`text-sm ${delta >= 0 ? 'text-green-400' : 'text-red-400'}`}>
        {delta >= 0 ? `+${delta}%` : `${delta}%`}
      </div>
    </div>
  );
}

function QuickAction({ icon: Icon, label, onClick, color = 'text-white' }) {
  return (
    <button
      onClick={onClick}
      className="flex items-center gap-2 px-3 py-2 bg-[#0f1113] border border-[#262626] rounded-md hover:shadow-[0_6px_18px_rgba(0,0,0,0.6)] transition"
    >
      <Icon className={`w-4 h-4 ${color}`} />
      <span className="text-sm text-gray-200">{label}</span>
    </button>
  );
}

function ActivityItem({ item }) {
  return (
    <div className="flex items-start gap-3 bg-[#141414] border border-[#262626] rounded-lg p-3">
      <div className="w-9 h-9 rounded-md bg-[#0b1220] flex items-center justify-center">
        <FileText className="w-4 h-4 text-[#F4A261]" />
      </div>
      <div className="flex-1">
        <div className="text-sm text-white">{item.title}</div>
        <div className="text-xs text-gray-400 mt-1">{item.time}</div>
      </div>
      <div className="text-xs text-gray-400">{item.meta}</div>
    </div>
  );
}

export default function AdminProfile() {
  // sample state/data - replace with real data or props
  const [notificationsEnabled, setNotificationsEnabled] = useState(true);
  const admin = {
    name: 'Kenan Al-Hassan',
    role: 'Super Admin',
    avatar: 'https://i.pravatar.cc/150?img=12',
    lastLogin: 'Jun 16, 2026 — 22:14',
    email: 'kenan@example.com',
  };

  const kpis = [
    { id: 1, icon: PieChart, label: 'Active Users', value: '12,450', delta: 4.2 },
    { id: 2, icon: Users, label: 'Companies', value: '1,240', delta: -1.1 },
    { id: 3, icon: BarChart2, label: 'Pending Approvals', value: '32', delta: 12.5 },
  ];

  const activities = [
    { id: 1, title: 'Approved company: GlobeTrips', time: '2 hours ago', meta: 'by you' },
    { id: 2, title: 'User reported issue #452', time: '6 hours ago', meta: 'support' },
    { id: 3, title: 'Exported monthly report', time: 'Yesterday', meta: 'CSV' },
  ];

  // action handlers (placeholders)
  const handleImpersonate = () => alert('Impersonate action (implement API)');
  const handleForceLogout = () => alert('Force logout all sessions (implement API)');
  const handleToggleNotifications = () => setNotificationsEnabled((s) => !s);
  const handleExportLogs = () => alert('Export logs (implement API)');

  return (
    <div className="p-8">
      {/* Header */}
      <div className="flex items-center justify-between gap-6">
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full overflow-hidden border-2 border-[#91B3FA]">
            <img src={admin.avatar} alt={admin.name} className="w-full h-full object-cover" />
          </div>
          <div>
            <h2 className="text-2xl font-semibold text-white">{admin.name}</h2>
            <div className="text-sm text-gray-400 flex items-center gap-3">
              <span className="flex items-center gap-2">
                <ShieldCheck className="w-4 h-4 text-[#91B3FA]" />
                <span>{admin.role}</span>
              </span>
              <span>•</span>
              <span className="text-xs">Last login: {admin.lastLogin}</span>
            </div>
            <div className="mt-2 text-xs text-gray-400">{admin.email}</div>
          </div>
        </div>

        {/* Quick actions */}
        <div className="flex items-center gap-3">
          <QuickAction icon={User} label="Edit Profile" onClick={() => alert('Edit profile')} />
          <QuickAction icon={Key} label="Reset Password" onClick={() => alert('Reset password')} />
          <QuickAction icon={LogOut} label="Sign Out" onClick={() => alert('Sign out')} />
        </div>
      </div>

      {/* Quick action bar + toggles */}
      <div className="mt-6 flex items-center justify-between gap-6">
        <div className="flex items-center gap-3">
          <QuickAction icon={Bell} label={notificationsEnabled ? 'Notifications On' : 'Notifications Off'} onClick={handleToggleNotifications} color={notificationsEnabled ? 'text-[#F4A261]' : 'text-gray-400'} />
          <QuickAction icon={Settings} label="Settings" onClick={() => alert('Open settings')} />
          <QuickAction icon={Users} label="Impersonate" onClick={handleImpersonate} />
          <QuickAction icon={LogOut} label="Force Logout All" onClick={handleForceLogout} />
        </div>

        <div className="flex items-center gap-3">
          <button onClick={handleExportLogs} className="flex items-center gap-2 px-3 py-2 bg-[#0f1113] border border-[#262626] rounded-md text-sm text-gray-200 hover:bg-[#161616] transition">
            <Download className="w-4 h-4 text-[#91B3FA]" /> Export Logs
          </button>
        </div>
      </div>

      {/* KPI cards */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mt-6">
        {kpis.map((k) => (
          <KPI key={k.id} icon={k.icon} label={k.label} value={k.value} delta={k.delta} />
        ))}
      </div>

      {/* Main content: activity feed + sessions & RBAC */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
        {/* Activity feed */}
        <div className="lg:col-span-2 space-y-4">
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4">
            <div className="flex items-center justify-between mb-3">
              <h4 className="text-sm font-semibold text-white">Recent activity</h4>
              <div className="text-xs text-gray-400">Showing last 30 events</div>
            </div>

            <div className="space-y-3">
              {activities.map((a) => (
                <ActivityItem key={a.id} item={a} />
              ))}
            </div>
          </div>

          {/* Audit / logs quick view */}
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4">
            <div className="flex items-center justify-between mb-3">
              <h4 className="text-sm font-semibold text-white">Audit & logs</h4>
              <div className="flex items-center gap-2">
                <button className="text-xs text-gray-400">Filter</button>
                <button className="text-xs text-gray-400">Export</button>
              </div>
            </div>

            <div className="w-full h-40 rounded-md bg-gradient-to-b from-[#0b1220] to-[#0f1724] flex items-center justify-center text-gray-500">
              Mini timeline / chart placeholder
            </div>
          </div>
        </div>

        {/* Right column: sessions, roles */}
        <aside className="space-y-4">
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4">
            <div className="flex items-center justify-between mb-3">
              <h4 className="text-sm font-semibold text-white">Active sessions</h4>
              <div className="text-xs text-gray-400">3 devices</div>
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-md p-2">
                <div className="text-sm text-white">Chrome — Warsaw</div>
                <div className="text-xs text-gray-400">Last active: 1h</div>
              </div>
              <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-md p-2">
                <div className="text-sm text-white">Mobile App — iPhone</div>
                <div className="text-xs text-gray-400">Last active: 2d</div>
              </div>
              <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-md p-2">
                <div className="text-sm text-white">API Token — Service</div>
                <div className="text-xs text-gray-400">Last active: 7d</div>
              </div>
            </div>

            <div className="mt-3 flex gap-2">
              <button className="flex-1 px-3 py-2 rounded-md bg-[#2b2b2b] text-sm text-gray-200">Revoke all</button>
              <button className="px-3 py-2 rounded-md bg-[#0f5132] text-sm text-white">Keep sessions</button>
            </div>
          </div>

          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4">
            <div className="flex items-center justify-between mb-3">
              <h4 className="text-sm font-semibold text-white">Roles & permissions</h4>
              <div className="text-xs text-gray-400">Manage RBAC</div>
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-md p-2">
                <div className="text-sm text-white">Super Admin</div>
                <div className="text-xs text-gray-400">Full access</div>
              </div>
              <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-md p-2">
                <div className="text-sm text-white">Content Manager</div>
                <div className="text-xs text-gray-400">Manage programs</div>
              </div>
            </div>

            <div className="mt-3">
              <button className="w-full px-3 py-2 rounded-md bg-[#0f5132] text-sm text-white">Create role</button>
            </div>
          </div>
        </aside>
      </div>
    </div>
  );
}
