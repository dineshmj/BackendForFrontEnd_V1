export interface Claim {
  type: string;
  value: string;
}

export interface Order {
  id: number;
  product: string;
  amount: number;
  status: string;
  date: string;
}

export interface Product {
  id: number;
  name: string;
  price: number;
  stock: number;
  category: string;
}

export interface Payment {
  id: number;
  amount: number;
  status: string;
  method: string;
  date: string;
}

export interface OrdersResponse {
  message: string;
  userId: string;
  userName: string;
  orders: Order[];
}

export interface ProductsResponse {
  message: string;
  userId: string;
  userName: string;
  products: Product[];
}

export interface PaymentsResponse {
  message: string;
  userId: string;
  userName: string;
  payments: Payment[];
}
