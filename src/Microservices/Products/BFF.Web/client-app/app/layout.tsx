import type { Metadata } from 'next';
import './globals.css';
import ErrorBoundary from './components/ErrorBoundary';

export const metadata: Metadata = {
  title: 'Products Microsrevice BFF',
  description: 'Next.js BFF for Products Microservice',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <ErrorBoundary>{children}</ErrorBoundary>
      </body>
    </html>
  );
}