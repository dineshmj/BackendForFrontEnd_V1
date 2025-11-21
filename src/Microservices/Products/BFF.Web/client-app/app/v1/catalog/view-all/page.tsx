'use client';

import { useState, useEffect } from 'react';
import { AuthGuard } from '../../../components/AuthGuard';
import { useAuth } from '../../../hooks/useAuth';
import ProductsGrid from '../../../components/ProductsGrid';

interface ProductsResponse {
  message: string;
  products: Array<{
    id: number;
    name: string;
    price: number;
    categoryName: string;
    stockQuantity: number;
  }>;
}

function ProductsContent() {
  const { user } = useAuth(false);
  const [products, setProducts] = useState<ProductsResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

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

  useEffect(() => {
    if (user) {
      fetchProducts();
    }
  }, [user]);

  if (!user) return null;

  return (
    <div style={{ padding: '2rem', fontFamily: 'system-ui' }}>
      <h1>View All Products</h1>

      {loading && <p>Loading products...</p>}
      {error && (
        <div style={{ marginTop: '1rem', color: 'red' }}>
          {error}
        </div>
      )}
      {products && <ProductsGrid products={products.products} />}
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