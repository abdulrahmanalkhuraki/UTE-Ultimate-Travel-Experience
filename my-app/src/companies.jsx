// src/companies.jsx
import  { useState } from 'react';
import {
  Building2,
  Trash2,
  Hourglass,
  MapPin,
  Grid,
  //Calendar,
  CheckCircle,
  XCircle,
} from 'lucide-react';

function CompanyProfile({ company, showActions }) {
  return (
    <div className="flex items-center justify-between bg-[#141414] border border-[#262626] rounded-lg p-3">
      <div className="flex items-center space-x-3">
        {/* logo */}
        <div className="w-12 h-12 bg-[#1f2937] rounded-md flex items-center justify-center overflow-hidden">
          <img src={company.logo} alt={company.name} className="w-full h-full object-contain" />
        </div>

        <div className="min-w-0">
          <div className="flex items-center space-x-2">
            <h4 className="text-sm font-semibold text-white truncate">{company.name}</h4>
            <span className="text-xs text-gray-400">•</span>
            <span className="text-xs text-gray-400">{company.founded}</span>
          </div>

          <div className="text-xs text-gray-400 mt-1 flex items-center space-x-3">
            <span className="flex items-center space-x-1">
              <MapPin className="w-3 h-3 text-gray-400" />
              <span>{company.location}</span>
            </span>

            <span className="flex items-center space-x-1">
              <Grid className="w-3 h-3 text-gray-400" />
              <span>{company.programs} programs</span>
            </span>
          </div>
        </div>
      </div>

      {showActions ? (
        <div className="flex items-center space-x-2">
          <button className="px-3 py-1 rounded-md bg-[#2b2b2b] text-sm text-red-400 hover:bg-[#3a2f2f] transition">
            <XCircle className="w-4 h-4 inline-block mr-1" />
            Reject
          </button>
          <button className="px-3 py-1 rounded-md bg-[#0f5132] text-sm text-white hover:bg-[#0b3f27] transition">
            <CheckCircle className="w-4 h-4 inline-block mr-1" />
            Accept
          </button>
        </div>
      ) : null}
    </div>
  );
}

export default function Companies() {
  // sample data (replace with real data / props)
  const [deletedExpanded, setDeletedExpanded] = useState(true);
  const [currentExpanded, setCurrentExpanded] = useState(false);

  const deletedCompanies = [
    { id: 1, name: 'TravelCo', logo: 'https://via.placeholder.com/64?text=T', founded: 'Founded: 2015', location: 'Dubai, UAE', programs: 9 },
    { id: 2, name: 'Wanderlust', logo: 'https://via.placeholder.com/64?text=W', founded: 'Founded: 2017', location: 'London, UK', programs: 4 },
    { id: 3, name: 'ExploreNow', logo: 'https://via.placeholder.com/64?text=E', founded: 'Founded: 2014', location: 'New York, USA', programs: 7 },
  ];

  const currentCompanies = [
    { id: 11, name: 'GlobeTrips', logo: 'https://via.placeholder.com/64?text=G', founded: 'Founded: 2012', location: 'Berlin, Germany', programs: 12 },
    { id: 12, name: 'Sunset Tours', logo: 'https://via.placeholder.com/64?text=S', founded: 'Founded: 2018', location: 'Lisbon, Portugal', programs: 5 },
  ];

  const joinRequests = [
    { id: 21, name: 'AdventureWorks', logo: 'https://via.placeholder.com/64?text=A', founded: 'Founded: 2019', location: 'Sydney, Australia', programs: 5 },
    { id: 22, name: 'OceanicTours', logo: 'https://via.placeholder.com/64?text=O', founded: 'Founded: 2020', location: 'Rome, Italy', programs: 2 },
    { id: 23, name: 'SummitTravels', logo: 'https://via.placeholder.com/64?text=S', founded: 'Founded: 2022', location: 'Paris, France', programs: 6 },
  ];

  return (
    <div className="p-8">
      {/* Chart header */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 bg-transparent">
          <h3 className="text-lg font-semibold text-white mb-3">Tourist Growth Over Time (last 12 months)</h3>
          {/* Placeholder for chart area - keep same proportions as Users */}
          <div className="w-full h-56 bg-gradient-to-b from-[#0b1220] to-[#0f1724] rounded-lg border border-[#262626] flex items-center justify-center text-gray-500">
            {/* Replace with real chart component */}
            <span>Chart Component Here</span>
          </div>

          {/* stats */}
          <div className="flex items-center space-x-4 mt-4">
            <div className="flex-1 bg-[#141414] border border-[#262626] rounded-lg p-4 flex items-center space-x-3">
              <Building2 className="w-6 h-6 text-[#91B3FA]" />
              <div>
                <div className="text-xs text-gray-400">Currently registered companies</div>
                <div className="text-xl font-semibold">12,450</div>
              </div>
            </div>

            <div className="flex-1 bg-[#141414] border border-[#262626] rounded-lg p-4 flex items-center space-x-3">
              <Trash2 className="w-6 h-6 text-[#F87171]" />
              <div>
                <div className="text-xs text-gray-400">Companies deleted</div>
                <div className="text-xl font-semibold">3,120</div>
              </div>
            </div>

            <div className="flex-1 bg-[#141414] border border-[#262626] rounded-lg p-4 flex items-center space-x-3">
              <Hourglass className="w-6 h-6 text-[#F4A261]" />
              <div>
                <div className="text-xs text-gray-400">Companies pending approval</div>
                <div className="text-xl font-semibold">1,240</div>
              </div>
            </div>
          </div>
        </div>

        {/* Right column top can be empty or summary */}
        <div className="bg-transparent"></div>
      </div>

      {/* Below chart: left column cards and right requests */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
        {/* Left and center area (col-span-2) */}
        <div className="lg:col-span-2 space-y-4">
          {/* Deleted companies card */}
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg">
            <button
              onClick={() => setDeletedExpanded((s) => !s)}
              className="w-full text-left px-4 py-3 flex items-center justify-between"
            >
              <div className="flex items-center space-x-3">
                <h4 className="text-sm font-semibold text-white">companies who deleted their accounts</h4>
              </div>
              <div className="text-xs text-gray-400">{deletedExpanded ? 'Collapse' : 'Expand'}</div>
            </button>

            {deletedExpanded && (
              <div className="px-4 pb-4 space-y-3">
                {deletedCompanies.map((c) => (
                  <CompanyProfile key={c.id} company={c} showActions={false} />
                ))}
              </div>
            )}
          </div>

          {/* Current companies card */}
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg">
            <button
              onClick={() => setCurrentExpanded((s) => !s)}
              className="w-full text-left px-4 py-3 flex items-center justify-between"
            >
              <div className="flex items-center space-x-3">
                <h4 className="text-sm font-semibold text-white">current companies</h4>
              </div>
              <div className="text-xs text-gray-400">{currentExpanded ? 'Collapse' : 'Expand'}</div>
            </button>

            {currentExpanded && (
              <div className="px-4 pb-4 space-y-3">
                {currentCompanies.map((c) => (
                  <CompanyProfile key={c.id} company={c} showActions={false} />
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Right column: join requests */}
        <aside className="space-y-4">
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4">
            <h4 className="text-sm font-semibold text-white mb-3">Company join requests</h4>

            <div className="space-y-3">
              {joinRequests.map((r) => (
                <div key={r.id} className="bg-[#141414] border border-[#262626] rounded-lg p-3">
                  <CompanyProfile company={r} showActions={true} />
                </div>
              ))}
            </div>
          </div>

          {/* large extended card placeholder to show vertical balance */}
          <div className="bg-[#0f1113] border border-[#262626] rounded-lg p-4 h-40 flex items-center justify-center text-gray-500">
            Extended requests panel
          </div>
        </aside>
      </div>
    </div>
  );
}
