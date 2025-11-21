'use client';

import { useEffect, useState } from 'react';
import { checkAuthentication, type AuthState } from '../lib/auth';

/*
 * Custom hook for authentication
 * Automatically checks auth status and redirects to login if not authenticated
 */
export function useAuth(autoRedirect: boolean = true) {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
    isLoading: true,
  });
  const [showLoginMessage, setShowLoginMessage] = useState(false);
  
  useEffect(() => {
    const loadConfigAndCheckAuth = async () => {
      const state = await checkAuthentication();
      setAuthState(state);

      if (autoRedirect && !state.isAuthenticated && !state.isLoading) {
        setShowLoginMessage(true);
      }
    };

    loadConfigAndCheckAuth();
  }, [autoRedirect]);

  return { ...authState, showLoginMessage };
}