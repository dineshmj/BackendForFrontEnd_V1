'use client';

import { useState } from 'react';
import { AuthGuard } from './components/AuthGuard';
import { useAuth } from './hooks/useAuth';

interface ProductsResponse {
  message: string;
  products: Array<{
    id: number;
    name: string;
    price: number;
    stock: number;
    category: string;
  }>;
}

function ProductsContent() {
  const { user } = useAuth(false);
  const [products, setProducts] = useState<ProductsResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!user) return null;

  const fetchProducts = async () => {
    setError(null);
    setLoading(true);

    try {
      const response = await fetch('/bff/products', {
        credentials: 'include',
        headers: {
          'X-CSRF': '1',
        },
      });

      if (!response.ok) {
        setError("Failed to load products");
        return;
      }

      const data: ProductsResponse = await response.json();
      setProducts(data);

    } catch (err) {
      console.error(err);
      setError("Failed to load products");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '2rem', fontFamily: 'system-ui', maxWidth: '900px', margin: '0 auto' }}>
      <h1>Products Microservice</h1>

      <button
        onClick={fetchProducts}
        disabled={loading}
        style={{
          marginTop: '1rem',
          padding: '0.75rem 1.5rem',
          background: loading ? '#999' : '#2563eb',
          color: 'white',
          border: 'none',
          borderRadius: '6px',
          cursor: loading ? 'not-allowed' : 'pointer'
        }}
      >
        {loading ? 'Loading…' : 'Load Products'}
      </button>

      {error && (
        <div style={{ marginTop: '1rem', color: 'red' }}>
          {error}
        </div>
      )}

      {products && (
        <div style={{ marginTop: '2rem' }}>
          <h2>Products</h2>
          <div style={{ overflowX: 'auto' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
              <thead>
                <tr style={{ background: '#eee' }}>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Category</th>
                  <th>Price</th>
                  <th>Stock</th>
                </tr>
              </thead>
              <tbody>
                {products.products.map((p) => (
                  <tr key={p.id}>
                    <td>{p.id}</td>
                    <td>{p.name}</td>
                    <td>{p.category}</td>
                    <td>${p.price}</td>
                    <td>{p.stock}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}

export default function Home() {
  return (
    <AuthGuard>
      <ProductsContent />
    </AuthGuard>
  );
}