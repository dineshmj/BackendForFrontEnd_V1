// store.ts
import { configureStore } from '@reduxjs/toolkit';
import { combineReducers } from 'redux';
import {
  persistStore,
  persistReducer,
  FLUSH,
  REHYDRATE,
  PAUSE,
  PERSIST,
  PURGE,
  REGISTER,
  Persistor,
} from 'redux-persist';
import storage from 'redux-persist/lib/storage';

import productsReducer from './productsSlice';
import categoriesReducer from './categoriesSlice';

// Combine reducers
const rootReducer = combineReducers({
  products: productsReducer,
  categories: categoriesReducer,
});

const persistConfig = {
  key: 'root',
  storage,
  whitelist: ['products', 'categories'],
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

// Create store
export const store = configureStore({
  reducer: persistedReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
      },
    }),
});

// Create persistor only in browser
export const persistor: Persistor | undefined =
  typeof window !== 'undefined' ? persistStore(store) : undefined;

// Types
export type RootState = ReturnType<typeof rootReducer>;
export type AppDispatch = typeof store.dispatch;


















// import { configureStore } from '@reduxjs/toolkit';
// import {
//   persistStore,
//   persistReducer,
//   FLUSH,
//   REHYDRATE,
//   PAUSE,
//   PERSIST,
//   PURGE,
//   REGISTER,
// } from 'redux-persist';
// import storage from 'redux-persist/lib/storage';
// import productsReducer, { ProductsState } from './productsSlice';
// import categoriesReducer, { CategoriesState } from './categoriesSlice';

// interface AppState {
//   products: ProductsState;
//   categories: CategoriesState;
// }

// const combinedReducer = {
//   products: productsReducer,
//   categories: categoriesReducer,
// };

// const rootReducer = persistReducer({
//   key: 'root',
//   storage,
//   whitelist: ['products', 'categories'],
// }, combinedReducer);

// export const store = configureStore({
//   reducer: rootReducer,
//   middleware: (getDefaultMiddleware) =>
//     getDefaultMiddleware({
//       serializableCheck: {
//         ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
//       },
//     }),
// });

// export const persistor = persistStore(store);

// // Explicitly defining RootState based on AppState + _persist key (which store.getState() returns)
// export type RootState = AppState & { _persist?: any }; 
// export type AppDispatch = typeof store.dispatch;