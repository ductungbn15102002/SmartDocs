import { create } from 'zustand'

interface AuthState {
  token: string | null
  role: string | null
  fullName: string | null
  setAuth: (token: string, role: string, fullName: string) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>((set) => ({
  token: localStorage.getItem('accessToken'),
  role: localStorage.getItem('role'),
  fullName: localStorage.getItem('fullName'),

  setAuth: (token, role, fullName) => {
    localStorage.setItem('accessToken', token)
    localStorage.setItem('role', role)
    localStorage.setItem('fullName', fullName)
    set({ token, role, fullName })
  },

  logout: () => {
    localStorage.clear()
    set({ token: null, role: null, fullName: null })
  }
}))