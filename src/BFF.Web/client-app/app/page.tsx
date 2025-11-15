'use client';

import { useState } from 'react';
import { AuthGuard } from './components/AuthGuard';
import { UserProfile } from './components/UserProfile';
import { useAuth } from './hooks/useAuth';

interface OrdersResponse {
  message: string;
  userId: string;
  userName: string;
  orders: Array<{
    id: number;
    product: string;
    amount: number;
    status: string;
    date: string;
  }>;
}

interface ProductsResponse {
  message: string;
  userId: string;
  userName: string;
  products: Array<{
    id: number;
    name: string;
    price: number;
    stock: number;
    category: string;
  }>;
}

interface PaymentsResponse {
  message: string;
  userId: string;
  userName: string;
  payments: Array<{
    id: number;
    amount: number;
    status: string;
    method: string;
    date: string;
  }>;
}

function HomeContent() {
  const { user } = useAuth(false); // Don't auto-redirect since we're already authenticated
  const [orders, setOrders] = useState<OrdersResponse | null>(null);
  const [products, setProducts] = useState<ProductsResponse | null>(null);
  const [payments, setPayments] = useState<PaymentsResponse | null>(null);
  const [loading, setLoading] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const fetchOrders = async (): Promise<void> => {
    setError(null);
    setLoading('orders');
    try {
      const response = await fetch('/api/orders', {
        credentials: 'include',
        headers: {
          'X-CSRF': '1',
        },
      });

      if (response.ok) {
        const data: OrdersResponse = await response.json();
        setOrders(data);
      } else {
        setError('Failed to fetch orders');
      }
    } catch (err) {
      console.error('Failed to fetch orders:', err);
      setError('Failed to fetch orders');
    } finally {
      setLoading(null);
    }
  };

  const fetchProducts = async (): Promise<void> => {
    setError(null);
    setLoading('products');
    try {
      const response = await fetch('/api/products', {
        credentials: 'include',
        headers: {
          'X-CSRF': '1',
        },
      });

      if (response.ok) {
        const data: ProductsResponse = await response.json();
        setProducts(data);
      } else {
        setError('Failed to fetch products');
      }
    } catch (err) {
      console.error('Failed to fetch products:', err);
      setError('Failed to fetch products');
    } finally {
      setLoading(null);
    }
  };

  const fetchPayments = async (): Promise<void> => {
    setError(null);
    setLoading('payments');
    try {
      const response = await fetch('/api/payments', {
        credentials: 'include',
        headers: {
          'X-CSRF': '1',
        },
      });

      if (response.ok) {
        const data: PaymentsResponse = await response.json();
        setPayments(data);
      } else {
        setError('Failed to fetch payments');
      }
    } catch (err) {
      console.error('Failed to fetch payments:', err);
      setError('Failed to fetch payments');
    } finally {
      setLoading(null);
    }
  };

  if (!user) {
    return null;
  }

  return (
    <div style={{ padding: '2rem', fontFamily: 'system-ui, sans-serif', maxWidth: '1200px', margin: '0 auto' }}>
      <h1 style={{ marginBottom: '1.5rem' }}>BFF Application with NextJS</h1>

      <UserProfile claims={user} />

      {error && (
        <div
          style={{
            padding: '1rem',
            marginBottom: '1rem',
            backgroundColor: '#fee',
            color: '#c00',
            borderRadius: '5px',
            border: '1px solid #fcc',
          }}
        >
          <strong>Error:</strong> {error}
        </div>
      )}

      <div style={{ marginBottom: '2rem' }}>
        <h2 style={{ fontSize: '1.25rem', marginBottom: '1rem' }}>API Actions</h2>
        <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
          <button
            onClick={fetchOrders}
            disabled={loading === 'orders'}
            style={{
              padding: '0.75rem 1.5rem',
              backgroundColor: loading === 'orders' ? '#ccc' : '#059669',
              color: 'white',
              border: 'none',
              borderRadius: '5px',
              cursor: loading === 'orders' ? 'not-allowed' : 'pointer',
              fontSize: '0.95rem',
              fontWeight: '500',
            }}
          >
            {loading === 'orders' ? 'Loading...' : 'Fetch Orders'}
          </button>
          <button
            onClick={fetchProducts}
            disabled={loading === 'products'}
            style={{
              padding: '0.75rem 1.5rem',
              backgroundColor: loading === 'products' ? '#ccc' : '#7c3aed',
              color: 'white',
              border: 'none',
              borderRadius: '5px',
              cursor: loading === 'products' ? 'not-allowed' : 'pointer',
              fontSize: '0.95rem',
              fontWeight: '500',
            }}
          >
            {loading === 'products' ? 'Loading...' : 'Fetch Products'}
          </button>
          <button
            onClick={fetchPayments}
            disabled={loading === 'payments'}
            style={{
              padding: '0.75rem 1.5rem',
              backgroundColor: loading === 'payments' ? '#ccc' : '#ea580c',
              color: 'white',
              border: 'none',
              borderRadius: '5px',
              cursor: loading === 'payments' ? 'not-allowed' : 'pointer',
              fontSize: '0.95rem',
              fontWeight: '500',
            }}
          >
            {loading === 'payments' ? 'Loading...' : 'Fetch Payments'}
          </button>
        </div>
      </div>

      {orders && (
        <div style={{ marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '1.25rem', marginBottom: '1rem' }}>Orders</h2>
          <div style={{ 
            backgroundColor: '#f8f9fa', 
            padding: '1rem', 
            borderRadius: '5px',
            border: '1px solid #dee2e6'
          }}>
            <pre style={{ margin: 0, overflow: 'auto', fontSize: '0.9rem' }}>
              {JSON.stringify(orders, null, 2)}
            </pre>
          </div>
        </div>
      )}

      {products && (
        <div style={{ marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '1.25rem', marginBottom: '1rem' }}>Products</h2>
          <div style={{ 
            backgroundColor: '#f8f9fa', 
            padding: '1rem', 
            borderRadius: '5px',
            border: '1px solid #dee2e6'
          }}>
            <pre style={{ margin: 0, overflow: 'auto', fontSize: '0.9rem' }}>
              {JSON.stringify(products, null, 2)}
            </pre>
          </div>
        </div>
      )}

      {payments && (
        <div style={{ marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '1.25rem', marginBottom: '1rem' }}>Payments</h2>
          <div style={{ 
            backgroundColor: '#f8f9fa', 
            padding: '1rem', 
            borderRadius: '5px',
            border: '1px solid #dee2e6'
          }}>
            <pre style={{ margin: 0, overflow: 'auto', fontSize: '0.9rem' }}>
              {JSON.stringify(payments, null, 2)}
            </pre>
          </div>
        </div>
      )}

      <div style={{ 
        marginTop: '3rem',
        padding: '1.5rem',
        backgroundColor: '#e7f3ff',
        borderRadius: '8px',
        border: '1px solid #b3d9ff'
      }}>
        <h3 style={{ marginTop: 0, color: '#004085' }}>User Claims</h3>
        <ul style={{ margin: 0, paddingLeft: '1.5rem' }}>
          {user.map((claim, index) => (
            <li key={index} style={{ marginBottom: '0.5rem', fontSize: '0.9rem' }}>
              <strong>{claim.type}:</strong> <span style={{ color: '#495057' }}>{claim.value}</span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default function Home() {
  return (
    <AuthGuard>
      <HomeContent />
    </AuthGuard>
  );
}
