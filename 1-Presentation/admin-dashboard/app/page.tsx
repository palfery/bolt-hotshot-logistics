'use client';

import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  TruckIcon, 
  UserGroupIcon, 
  CurrencyDollarIcon,
  ClockIcon,
  MapPinIcon,
  ChartBarIcon
} from '@heroicons/react/24/outline';
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell
} from 'recharts';

// Mock data - would come from API
const dashboardData = {
  overview: {
    totalJobs: 1247,
    activeJobs: 89,
    totalDrivers: 156,
    activeDrivers: 134,
    totalRevenue: 487650,
    monthlyRevenue: 45230
  },
  recentJobs: [
    {
      id: 'job_1',
      title: 'Electronics Delivery',
      status: 'in_progress',
      driver: 'John Martinez',
      route: 'Austin → Dallas',
      value: 450
    },
    {
      id: 'job_2',
      title: 'Medical Supplies',
      status: 'completed',
      driver: 'Sarah Johnson',
      route: 'Houston → San Antonio',
      value: 520
    }
  ],
  revenueChart: [
    { month: 'Jan', revenue: 42000 },
    { month: 'Feb', revenue: 38500 },
    { month: 'Mar', revenue: 45200 },
    { month: 'Apr', revenue: 41800 },
    { month: 'May', revenue: 47300 },
    { month: 'Jun', revenue: 45230 }
  ],
  jobsByStatus: [
    { name: 'Available', value: 23, color: '#3b82f6' },
    { name: 'In Progress', value: 21, color: '#f59e0b' },
    { name: 'Completed', value: 1158, color: '#22c55e' },
    { name: 'Cancelled', value: 45, color: '#ef4444' }
  ]
};

const StatCard = ({ title, value, icon: Icon, change, changeType }: any) => (
  <motion.div
    initial={{ opacity: 0, y: 20 }}
    animate={{ opacity: 1, y: 0 }}
    className="card hover:shadow-lg transition-shadow duration-300"
  >
    <div className="flex items-center justify-between">
      <div>
        <p className="text-sm font-medium text-gray-600">{title}</p>
        <p className="text-2xl font-bold text-gray-900">{value}</p>
        {change && (
          <p className={`text-sm ${changeType === 'positive' ? 'text-success-600' : 'text-danger-600'}`}>
            {changeType === 'positive' ? '+' : ''}{change}% from last month
          </p>
        )}
      </div>
      <div className="p-3 bg-primary-100 rounded-lg">
        <Icon className="h-6 w-6 text-primary-600" />
      </div>
    </div>
  </motion.div>
);

export default function Dashboard() {
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Simulate loading
    const timer = setTimeout(() => setIsLoading(false), 1000);
    return () => clearTimeout(timer);
  }, []);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="spinner w-8 h-8 border-primary-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div>
              <h1 className="text-3xl font-bold text-gradient">HotShot Logistics</h1>
              <p className="text-gray-600 mt-1">Admin Dashboard</p>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <div className="w-2 h-2 bg-success-500 rounded-full animate-pulse"></div>
                <span>System Online</span>
              </div>
              <div className="w-8 h-8 bg-primary-600 rounded-full flex items-center justify-center">
                <span className="text-white text-sm font-medium">A</span>
              </div>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Overview Stats */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <StatCard
            title="Total Jobs"
            value={dashboardData.overview.totalJobs.toLocaleString()}
            icon={TruckIcon}
            change={12.5}
            changeType="positive"
          />
          <StatCard
            title="Active Drivers"
            value={`${dashboardData.overview.activeDrivers}/${dashboardData.overview.totalDrivers}`}
            icon={UserGroupIcon}
            change={8.2}
            changeType="positive"
          />
          <StatCard
            title="Monthly Revenue"
            value={`$${dashboardData.overview.monthlyRevenue.toLocaleString()}`}
            icon={CurrencyDollarIcon}
            change={15.3}
            changeType="positive"
          />
          <StatCard
            title="Active Jobs"
            value={dashboardData.overview.activeJobs}
            icon={ClockIcon}
            change={-2.1}
            changeType="negative"
          />
        </div>

        {/* Charts Section */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
          {/* Revenue Chart */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="card"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Revenue Trend</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={dashboardData.revenueChart}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                <XAxis dataKey="month" stroke="#6b7280" />
                <YAxis stroke="#6b7280" />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: '#fff', 
                    border: '1px solid #e5e7eb',
                    borderRadius: '8px'
                  }}
                  formatter={(value) => [`$${value.toLocaleString()}`, 'Revenue']}
                />
                <Line 
                  type="monotone" 
                  dataKey="revenue" 
                  stroke="#3b82f6" 
                  strokeWidth={3}
                  dot={{ fill: '#3b82f6', strokeWidth: 2, r: 4 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </motion.div>

          {/* Jobs by Status */}
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            className="card"
          >
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Jobs by Status</h3>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={dashboardData.jobsByStatus}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={120}
                  paddingAngle={5}
                  dataKey="value"
                >
                  {dashboardData.jobsByStatus.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip formatter={(value) => [value, 'Jobs']} />
              </PieChart>
            </ResponsiveContainer>
            <div className="flex flex-wrap justify-center gap-4 mt-4">
              {dashboardData.jobsByStatus.map((entry, index) => (
                <div key={index} className="flex items-center">
                  <div 
                    className="w-3 h-3 rounded-full mr-2"
                    style={{ backgroundColor: entry.color }}
                  ></div>
                  <span className="text-sm text-gray-600">{entry.name}</span>
                </div>
              ))}
            </div>
          </motion.div>
        </div>

        {/* Recent Jobs */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="card"
        >
          <div className="flex justify-between items-center mb-6">
            <h3 className="text-lg font-semibold text-gray-900">Recent Jobs</h3>
            <button className="btn-primary">View All Jobs</button>
          </div>
          
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="table-header">Job</th>
                  <th className="table-header">Driver</th>
                  <th className="table-header">Route</th>
                  <th className="table-header">Status</th>
                  <th className="table-header">Value</th>
                  <th className="table-header">Actions</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {dashboardData.recentJobs.map((job) => (
                  <tr key={job.id} className="hover:bg-gray-50">
                    <td className="table-cell font-medium">{job.title}</td>
                    <td className="table-cell">{job.driver}</td>
                    <td className="table-cell">
                      <div className="flex items-center">
                        <MapPinIcon className="h-4 w-4 text-gray-400 mr-1" />
                        {job.route}
                      </div>
                    </td>
                    <td className="table-cell">
                      <span className={`status-badge status-${job.status.replace('_', '-')}`}>
                        {job.status.replace('_', ' ')}
                      </span>
                    </td>
                    <td className="table-cell font-semibold">${job.value}</td>
                    <td className="table-cell">
                      <button className="text-primary-600 hover:text-primary-900 text-sm font-medium">
                        View Details
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </motion.div>

        {/* Quick Actions */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8"
        >
          <div className="card text-center hover:shadow-lg transition-shadow duration-300 cursor-pointer">
            <TruckIcon className="h-12 w-12 text-primary-600 mx-auto mb-4" />
            <h4 className="text-lg font-semibold text-gray-900 mb-2">Create New Job</h4>
            <p className="text-gray-600 text-sm">Add a new delivery job to the system</p>
          </div>
          
          <div className="card text-center hover:shadow-lg transition-shadow duration-300 cursor-pointer">
            <UserGroupIcon className="h-12 w-12 text-primary-600 mx-auto mb-4" />
            <h4 className="text-lg font-semibold text-gray-900 mb-2">Manage Drivers</h4>
            <p className="text-gray-600 text-sm">View and manage driver profiles</p>
          </div>
          
          <div className="card text-center hover:shadow-lg transition-shadow duration-300 cursor-pointer">
            <ChartBarIcon className="h-12 w-12 text-primary-600 mx-auto mb-4" />
            <h4 className="text-lg font-semibold text-gray-900 mb-2">View Analytics</h4>
            <p className="text-gray-600 text-sm">Detailed reports and analytics</p>
          </div>
        </motion.div>
      </main>
    </div>
  );
}