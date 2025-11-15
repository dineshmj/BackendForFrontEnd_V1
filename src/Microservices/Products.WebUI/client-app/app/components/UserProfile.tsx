'use client';

import { redirectToLogout, getUserDisplayName, type BffUser } from '../lib/auth';

interface UserProfileProps {
  claims: BffUser[];
}

export function UserProfile({ claims }: UserProfileProps) {
  const displayName = getUserDisplayName(claims);

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      padding: '1rem',
      backgroundColor: '#f8f9fa',
      borderRadius: '8px',
      marginBottom: '2rem'
    }}>
      <div>
        <span style={{ fontSize: '0.9rem', color: '#6c757d' }}>Logged in as:</span>
        <strong style={{ marginLeft: '0.5rem', fontSize: '1.1rem' }}>{displayName}</strong>
      </div>
      <button
        onClick={() => redirectToLogout(claims)}
        style={{
          padding: '0.5rem 1rem',
          backgroundColor: '#dc3545',
          color: 'white',
          border: 'none',
          borderRadius: '5px',
          cursor: 'pointer',
          fontSize: '0.95rem',
          fontWeight: '500'
        }}
      >
        Logout
      </button>
    </div>
  );
}
