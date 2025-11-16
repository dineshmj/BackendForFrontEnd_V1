import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Products Microservice - Frontend',
  description: 'Products Microservice Frontend - Backend for Frontend with NextJS and ASP.NET Core',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
