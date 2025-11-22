'use client';

import React from 'react';
import { Provider } from 'react-redux';
import { store } from '../store/store';

interface ReduxProviderWrapperProps {
  children: React.ReactNode;
}

export default function ReduxProviderWrapper({ children }: ReduxProviderWrapperProps) {
  return (
    <Provider store={store}>
      {children}
    </Provider>
  );
}