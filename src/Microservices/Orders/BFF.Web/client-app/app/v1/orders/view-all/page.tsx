'use client';

import { config } from '../../../../app.config.json';

import { HttpError } from '@/app/types';
import React, { useState, useEffect, useRef } from 'react';

// Define the expected Order structure based on mock data
interface Order {
  orderId: number;
  dateOfOrder: string; // ISO Date string
  totalAmount: number;
  paymentMethod: string;
  invoiceNumber: string;
  numberOfItems: number;
  dispatchStatus: string;
  customerName: string;
}

// Utility function for date formatting: dd-MMM-yyyy
const formatDate = (dateString: string): string => {
  try {
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'short',
      day: '2-digit',
    };
    const date = new Date(dateString);
    return date.toLocaleDateString('en-GB', options).replace(/ /g, '-'); // e.g. 20-Nov-2025
  } catch (e) {
    console.error('Date formatting failed:', e);
    return dateString;
  }
};

const BFF_BASE_URL =
  (process.env.NEXT_PUBLIC_ORDERS_BFF_URL ?? 'https://localhost:33800').replace(
    /\/+$/,
    '',
  );

const OrdersGetAllPage: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<HttpError | null>(null);

  const didFetchRef = useRef(false);

  useEffect(() => {
  const fetchOrders = async () => {
    try {
      const res = await fetch(`${BFF_BASE_URL}/api/orders/view-all`, {
        method: 'GET',
        credentials: 'include', // important to send BFF auth cookie
      });

      if (!res.ok) {
        const text = await res.text();
        console.error(
          'BFF /api/orders/view-all failed:',
          'status =', res.status,
          'statusText =', res.statusText,
          'body =', text,
        );
        throw new Error(
          `API error ${res.status} ${res.statusText}: ${text || 'No body'}`,
        );
      }

      const data = await res.json();

      if (Array.isArray(data)) {
        setOrders(data);
      } else {
        setError(new HttpError('Received unexpected data format from API.', 500));
      }
    } catch (err: any) {
      // API error 401 Unauthorized: {"statusCode":401,"message":"Access token not found"}

      // if err contains "401 Unauthorized", set error status to 401
      if (err.message && (err.message.includes('401 Unauthorized') || err.message.includes('"statusCode":401'))) {
        setError(new HttpError('User is not authenticated.', 401));
        return;
      }
      else {
        const errorMessage = err?.message ?? 'Failed to fetch orders.';
        const errorStatus = (err as any)?.status || 500;
        setError(new HttpError(errorMessage, errorStatus));
      }
    } finally {
      setLoading(false);
    }
  };

  if (!didFetchRef.current) {
    didFetchRef.current = true;
    fetchOrders();
  }
}, []);


  if (loading) {
    return (
      <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
        Loading Orders...
      </div>
    );
  }

  if (error) {
    if (error.statusCode === 401) {
      return (
        <div style={{ padding: '2rem', fontFamily: 'system-ui', textAlign: 'center' }}>
          <h1>Authentication Required</h1>
          
          <p>
            User is not authenticated. Please login from
            <a href={process.env.PMS_LOGIN_URL} style={{ marginLeft: '0.5rem' }}>Platform Management System</a>.
          </p>
        </div>
      );
    }

    return (
      <div
        style={{
          padding: '20px',
          color: 'red',
          fontFamily: 'sans-serif',
          whiteSpace: 'pre-wrap',
        }}
      >
        Error loading orders: {error?.message}
      </div>
    );
  }

  const columns = [
    { key: 'orderId', header: 'Order ID' },
    { key: 'dateOfOrder', header: 'Date' },
    { key: 'totalAmount', header: 'Amount' },
    { key: 'paymentMethod', header: 'Payment' },
    { key: 'invoiceNumber', header: 'Invoice #' },
    { key: 'numberOfItems', header: 'Items' },
    { key: 'dispatchStatus', header: 'Status' },
    { key: 'customerName', header: 'Customer' },
  ];

  return (
    <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
      <h1 style={{ marginBottom: '20px' }}>All Orders</h1>

      {/* Header row */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: `repeat(${columns.length}, minmax(100px, 1fr))`,
          gap: '10px',
          borderBottom: '2px solid #ccc',
          padding: '10px 0',
          fontWeight: 'bold',
        }}
      >
        {columns.map((col) => (
          <div key={col.key}>{col.header}</div>
        ))}
      </div>

      {/* Data rows */}
      {orders.map((order, index) => (
        <div
          key={order.orderId || index}
          style={{
            display: 'grid',
            gridTemplateColumns: `repeat(${columns.length}, minmax(100px, 1fr))`,
            gap: '10px',
            padding: '10px 0',
            borderBottom:
              index % 2 === 0 ? '1px solid #eee' : '1px solid #f7f7f7',
          }}
        >
          <div
            style={{
              whiteSpace: 'nowrap',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
            }}
          >
            {order.orderId}
          </div>
          <div>{formatDate(order.dateOfOrder)}</div>
          <div>$ {order.totalAmount.toFixed(2)}</div>
          <div
            style={{
              whiteSpace: 'nowrap',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
            }}
          >
            {order.paymentMethod}
          </div>
          <div
            style={{
              whiteSpace: 'nowrap',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
            }}
          >
            {order.invoiceNumber}
          </div>
          <div>{order.numberOfItems}</div>
          <div>{order.dispatchStatus}</div>
          <div
            style={{
              whiteSpace: 'nowrap',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
            }}
          >
            {order.customerName}
          </div>
        </div>
      ))}

      {!loading && orders.length === 0 && <p>No orders found.</p>}
    </div>
  );
};

export default OrdersGetAllPage;