import axios from 'axios';

const BFF_URL = process.env.NEXT_PUBLIC_BFF_URL || 'http://localhost:3001';

// Create axios instance with default config
const api = axios.create({
  baseURL: BFF_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Redirect to silent login if unauthorized
      window.location.href = `${BFF_URL}/api/auth/silent-login?returnUrl=${encodeURIComponent(window.location.pathname)}`;
    }
    return Promise.reject(error);
  },
);

export default api;