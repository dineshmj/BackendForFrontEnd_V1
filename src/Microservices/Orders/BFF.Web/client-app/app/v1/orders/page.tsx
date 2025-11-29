'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import axios from 'axios';

const BFF_URL = process.env.NEXT_PUBLIC_BFF_URL || 'http://localhost:3001';

interface Order {
  id: string;
  orderNumber: string;
  customerName: string;
  status: string;
  total: number;
  createdAt: string;
}

export default function OrdersPage() {
  const router = useRouter();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      // This would call your BFF endpoint which then calls the Orders API
      // For now, showing mock data
      const mockOrders: Order[] = [
        {
          id: '1',
          orderNumber: 'ORD-001',
          customerName: 'John Doe',
          status: 'Pending',
          total: 299.99,
          createdAt: new Date().toISOString(),
        },
        {
          id: '2',
          orderNumber: 'ORD-002',
          customerName: 'Jane Smith',
          status: 'Completed',
          total: 599.99,
          createdAt: new Date().toISOString(),
        },
      ];

      setOrders(mockOrders);
      setLoading(false);

      // Uncomment when you have the Orders API ready:
      // const response = await axios.get(`${BFF_URL}/api/orders`, {
      //   withCredentials: true,
      // });
      // setOrders(response.data);
    } catch (err: any) {
      setError(err.message);
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="container">
        <div className="card">
          <h1>Loading orders...</h1>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container">
        <div className="card error">
          <h1>Error</h1>
          <p>{error}</p>
          <button onClick={() => router.push('/')}>Go Home</button>
        </div>
      </div>
    );
  }

  return (
    <div className="container">
      <div className="card">
        <h1>Orders</h1>
        <div className="actions">
          <button onClick={() => router.push('/')}>← Back to Home</button>
          <button onClick={() => router.push('/orders/create')}>+ Create Order</button>
        </div>
        
        <div className="orders-list">
          {orders.length === 0 ? (
            <p>No orders found.</p>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Order Number</th>
                  <th>Customer</th>
                  <th>Status</th>
                  <th>Total</th>
                  <th>Created</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <tr key={order.id}>
                    <td>{order.orderNumber}</td>
                    <td>{order.customerName}</td>
                    <td>
                      <span className={`status ${order.status.toLowerCase()}`}>
                        {order.status}
                      </span>
                    </td>
                    <td>${order.total.toFixed(2)}</td>
                    <td>{new Date(order.createdAt).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}