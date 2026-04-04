import React, { createContext, useContext, useState, useCallback } from 'react';

interface AuthContextType {
  userId: string | null;
  email: string | null;
  login: (email: string) => void;
  logout: () => void;
  isLoggedIn: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const STORAGE_KEY = 'ipl_merch_user_email';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [email, setEmail] = useState<string | null>(() => localStorage.getItem(STORAGE_KEY));

  const login = useCallback((newEmail: string) => {
    const trimmed = newEmail.trim().toLowerCase();
    localStorage.setItem(STORAGE_KEY, trimmed);
    setEmail(trimmed);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(STORAGE_KEY);
    setEmail(null);
  }, []);

  return (
    <AuthContext.Provider value={{ userId: email, email, login, logout, isLoggedIn: !!email }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};
