'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

const BFF_URL = process.env.NEXT_PUBLIC_BFF_URL || 'http://localhost:3001';

interface AuthStatus {
  isAuthenticated: boolean;
  userName?: string;
  email?: string;
}

export default function Home() {
  const router = useRouter();
  const [authStatus, setAuthStatus] = useState<AuthStatus | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    checkAuthStatus();
  }, []);

  const checkAuthStatus = async () => {
    try {
      const response = await axios.get(`${BFF_URL}/api/auth/status`, {
        withCredentials: true,
      });
      setAuthStatus(response.data);
      
      // If not authenticated and in iframe, trigger silent login
      if (!response.data.isAuthenticated && window.parent !== window) {
        // We're in an iframe, redirect to silent login
        window.location.href = `${BFF_URL}/api/auth/silent-login?returnUrl=${encodeURIComponent(window.location.pathname)}`;
      }
    } catch (error) {
      console.error('Failed to check auth status:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="container">
        <div className="card">
          <h1>Loading...</h1>
        </div>
      </div>
    );
  }

  if (!authStatus?.isAuthenticated) {
    return (
      <div className="container">
        <div className="card">
          <h1>Orders Microservice</h1>
          <p>Authenticating...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="card">
        <h1>Orders Microservice</h1>
        <div className="user-info">
          <p><strong>Welcome, {authStatus.userName || 'User'}!</strong></p>
          {authStatus.email && <p>Email: {authStatus.email}</p>}
        </div>
        
        <div className="actions">
          <button onClick={() => router.push('/orders')}>
            View Orders
          </button>
          <button onClick={() => router.push('/orders/create')}>
            Create Order
          </button>
        </div>
      </div>
    </div>
  );
}