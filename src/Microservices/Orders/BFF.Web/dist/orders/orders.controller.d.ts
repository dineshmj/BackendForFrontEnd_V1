import { Request } from 'express';
import { AuthService } from '../auth/auth.service';
export declare class OrdersController {
    private readonly authService;
    constructor(authService: AuthService);
    getOrders(req: Request): Promise<{
        orderId: number;
        dateOfOrder: string;
        totalAmount: number;
        paymentMethod: string;
        invoiceNumber: string;
        numberOfItems: number;
        dispatchStatus: string;
        customerName: string;
    }[]>;
    getOrderById(id: string, req: Request): Promise<any>;
    createOrder(orderData: any, req: Request): Promise<any>;
}
