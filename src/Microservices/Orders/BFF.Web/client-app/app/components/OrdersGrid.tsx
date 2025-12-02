'use client';

import React from 'react';
import { Order } from '../types';

interface OrdersGridProps {
  orders: Order[];
}

const OrdersGrid: React.FC<OrdersGridProps> = ({ orders }) => {
  if (!orders || orders.length === 0) {
    return <p>No items found.</p>;
  }

  return (
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))', gap: '1.5rem', marginTop: '2rem' }}>
      {orders.map((p) => (
        <div key={p.id} style={{ border: '1px solid #ddd', borderRadius: '8px', padding: '1rem', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h3 style={{ fontSize: '1.25rem', marginBottom: '0.5rem' }}>Order #: {p.orderNumber}</h3>
		      <p style={{ margin: '0.25rem 0' }}><strong>Customer Name:</strong> ${p.customerName}</p>
          <p style={{ margin: '0.25rem 0' }}><strong>Status:</strong> ${p.status}</p>
          <p style={{ margin: '0.25rem 0' }}><strong>Total # of Items:</strong> {p.total}</p>
          <p style={{ margin: '0.25rem 0' }}><strong>Order Created At:</strong> {p.createdAt}</p>
        </div>
      ))}
    </div>
  );
};

export default OrdersGrid;