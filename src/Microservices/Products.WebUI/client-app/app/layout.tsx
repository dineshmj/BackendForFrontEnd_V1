import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'BFF Application',
  description: 'Backend for Frontend with NextJS and ASP.NET Core',
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
