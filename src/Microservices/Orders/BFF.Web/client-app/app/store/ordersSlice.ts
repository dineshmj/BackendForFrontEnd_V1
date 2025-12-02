import { config } from '../../app.config.json';
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { Order, OrdersResponse } from '../types';

export interface OrdersState {
  data: Order[] | null;
  loading: boolean;
  error: string | null;
  hasLoaded: boolean;
}

const initialState: OrdersState = {
  data: null,
  loading: false,
  error: null,
  hasLoaded: false,
};

// Async Thunk for API call
export const fetchOrders = createAsyncThunk<
  OrdersResponse,
  void,
  { rejectValue: string }
>('orders/fetchOrders', async (_, { rejectWithValue }) => {
  try {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 30000); // 30 second timeout

    // const response = await fetch(`${process.env.NEXT_PUBLIC_ORDERS_BFF_URL}/api/orders/view-all`, {

    // Make a BFF edge API call from the NextJS store.
    const response = await fetch(`${config.ordersBffUrl}/api/orders/view-all`, {
        method: 'GET',
        credentials: 'include', // important to send BFF auth cookie
      });

    clearTimeout(timeoutId);

    if (!response.ok) {
      return rejectWithValue(`Failed to load orders: Status ${response.status}`);
    }

    // The call to the BFF edge API was successful.
    const data: OrdersResponse = await response.json();
    return data;

  } catch (err) {
    if (err instanceof Error && err.name === 'AbortError') {
        return rejectWithValue("Orders fetch timed out");
    }

    console.error(err);
    return rejectWithValue("Network error fetching orders");
  }
});

const ordersSlice = createSlice({
  name: 'orders',
  initialState,
  reducers: {
    // Reducer for explicit 'Fetch Records Again' if needed, though async thunk handles update flow well.
    // We will rely on dispatching fetchOrders for reloads.
    clearOrdersError: (state) => {
        state.error = null;
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrders.fulfilled, (state, action: PayloadAction<OrdersResponse>) => {
        state.loading = false;
        state.data = action.payload.orders;
        state.hasLoaded = true;
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload || "An unknown error occurred during fetch.";
        state.data = null; // Clear data on failure? Let's keep existing data if it exists and hasLoaded is true, but the requirement implies fetching updates state entirely. Clearing seems safer for reloads unless specified otherwise.
      });
  },
});

export const { clearOrdersError } = ordersSlice.actions;
export default ordersSlice.reducer;