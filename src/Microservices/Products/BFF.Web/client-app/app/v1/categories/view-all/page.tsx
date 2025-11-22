'use client';

import { useState, useEffect } from 'react';
import { AuthGuard } from '../../../components/AuthGuard';
import { useAuth } from '../../../hooks/useAuth';
import CategoriesGrid from '../../../components/CategoriesGrid';

interface CategoriesResponse {
  message: string;
  categories: Array<{
    id: number;
    name: string;
    price: number;
    categoryName: string;
    stockQuantity: number;
  }>;
}

function CategoriesContent() {
  const { user } = useAuth(false);
  const [categories, setCategories] = useState<CategoriesResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCategories = async () => {
    setError(null);
    setLoading(true);

    try {
      const response = await fetch('/bff/categories', {
        credentials: 'include',
        headers: {
          'X-CSRF': '1',
        },
      });

      if (!response.ok) {
        setError("Failed to load categories");
        return;
      }

      const data: CategoriesResponse = await response.json();
      setCategories(data);

    } catch (err) {
      console.error(err);
      setError("Failed to load categories");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user) {
      fetchCategories();
    }
  }, [user]);

  if (!user) return null;

  return (
    <div style={{ padding: '2rem', fontFamily: 'system-ui' }}>
      <h1>View All Categories</h1>

      {loading && <p>Loading categories...</p>}
      {error && (
        <div style={{ marginTop: '1rem', color: 'red' }}>
          {error}
        </div>
      )}
      {categories && <CategoriesGrid categories={categories.categories} />}
    </div>
  );
}

export default function Home() {
  return (
    <AuthGuard>
      <CategoriesContent />
    </AuthGuard>
  );
}