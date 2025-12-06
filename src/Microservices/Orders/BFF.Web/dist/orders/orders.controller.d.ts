import { Request } from 'express';
import { AuthService } from '../auth/auth.service';
export declare class OrdersController {
    private readonly authService;
    constructor(authService: AuthService);
    getOrders(req: Request): Promise<any>;
    getOrderById(id: string, req: Request): Promise<any>;
    createOrder(orderData: any, req: Request): Promise<any>;
}
