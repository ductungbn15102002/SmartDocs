import { Routes, Route, Navigate, NavLink } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import DocumentsPage from './pages/DocumentsPage'
import ApprovalPage from './pages/ApprovalPage'
import { useAuthStore } from './store/authStore'
import { useNavigate } from 'react-router-dom'
import { LogOut } from 'lucide-react'

function Layout({ children }: { children: React.ReactNode }) {
  const { fullName, logout } = useAuthStore()
  const navigate = useNavigate()

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="bg-white shadow px-6 py-4 flex justify-between items-center">
        <div className="flex items-center gap-6">
          <h1 className="text-xl font-bold text-blue-600">SmartDocs</h1>
          <NavLink to="/" end className={({ isActive }) =>
            `text-sm ${isActive ? 'text-blue-600 font-semibold' : 'text-gray-600 hover:text-blue-500'}`}>
            Tài liệu
          </NavLink>
          <NavLink to="/approval" className={({ isActive }) =>
            `text-sm ${isActive ? 'text-blue-600 font-semibold' : 'text-gray-600 hover:text-blue-500'}`}>
            Approval
          </NavLink>
        </div>
        <div className="flex items-center gap-4">
          <span className="text-sm text-gray-600">Xin chào, {fullName}</span>
          <button onClick={() => { logout(); navigate('/login') }}
            className="flex items-center gap-1 text-sm text-red-500 hover:text-red-700">
            <LogOut size={16} /> Đăng xuất
          </button>
        </div>
      </div>
      {children}
    </div>
  )
}

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { token } = useAuthStore()
  return token ? <>{children}</> : <Navigate to="/login" />
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/" element={
        <ProtectedRoute>
          <Layout><DocumentsPage /></Layout>
        </ProtectedRoute>
      } />
      <Route path="/approval" element={
        <ProtectedRoute>
          <Layout><ApprovalPage /></Layout>
        </ProtectedRoute>
      } />
    </Routes>
  )
}