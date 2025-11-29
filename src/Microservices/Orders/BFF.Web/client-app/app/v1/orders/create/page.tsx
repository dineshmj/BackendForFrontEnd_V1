'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

const BFF_URL = process.env.NEXT_PUBLIC_BFF_URL || 'http://localhost:3001';

export default function CreateOrderPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    customerName: '',
    productName: '',
    quantity: 1,
    price: 0,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'quantity' || name === 'price' ? parseFloat(value) || 0 : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // This would call your BFF endpoint which then calls the Orders API
      // For now, just simulating success
      
      // Uncomment when you have the Orders API ready:
      // await axios.post(`${BFF_URL}/api/orders`, formData, {
      //   withCredentials: true,
      // });

      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      alert('Order created successfully!');
      router.push('/orders');
    } catch (err: any) {
      setError(err.message || 'Failed to create order');
      setLoading(false);
    }
  };

  const total = formData.quantity * formData.price;

  return (
    <div className="container">
      <div className="card">
        <h1>Create New Order</h1>
        <button onClick={() => router.push('/orders')}>← Back to Orders</button>

        <form onSubmit={handleSubmit} style={{ marginTop: '20px' }}>
          <div className="form-group">
            <label htmlFor="customerName">Customer Name *</label>
            <input
              type="text"
              id="customerName"
              name="customerName"
              value={formData.customerName}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="productName">Product Name *</label>
            <input
              type="text"
              id="productName"
              name="productName"
              value={formData.productName}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="quantity">Quantity *</label>
            <input
              type="number"
              id="quantity"
              name="quantity"
              value={formData.quantity}
              onChange={handleChange}
              min="1"
              required
              disabled={loading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="price">Price (per unit) *</label>
            <input
              type="number"
              id="price"
              name="price"
              value={formData.price}
              onChange={handleChange}
              min="0"
              step="0.01"
              required
              disabled={loading}
            />
          </div>

          <div className="user-info" style={{ marginBottom: '20px' }}>
            <p><strong>Total: ${total.toFixed(2)}</strong></p>
          </div>

          {error && <div className="error-message">{error}</div>}

          <div className="actions">
            <button type="submit" disabled={loading}>
              {loading ? 'Creating...' : 'Create Order'}
            </button>
            <button 
              type="button" 
              onClick={() => router.push('/orders')}
              disabled={loading}
              style={{ background: '#6c757d' }}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}