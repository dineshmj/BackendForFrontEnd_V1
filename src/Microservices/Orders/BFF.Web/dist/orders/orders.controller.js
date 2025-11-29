"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.OrdersController = void 0;
const common_1 = require("@nestjs/common");
const auth_service_1 = require("../auth/auth.service");
const session_auth_guard_1 = require("../auth/guards/session-auth.guard");
let OrdersController = class OrdersController {
    constructor(authService) {
        this.authService = authService;
    }
    async getOrders(req) {
        const orders = [
            {
                orderId: 1001,
                dateOfOrder: '2025-11-20T10:00:00Z',
                totalAmount: 150.75,
                paymentMethod: 'Credit Card',
                invoiceNumber: 'INV-1001',
                numberOfItems: 3,
                dispatchStatus: 'Shipped',
                customerName: 'John Doe',
            },
            {
                orderId: 1002,
                dateOfOrder: '2025-11-21T15:30:00Z',
                totalAmount: 89.99,
                paymentMethod: 'UPI',
                invoiceNumber: 'INV-1002',
                numberOfItems: 1,
                dispatchStatus: 'Processing',
                customerName: 'Jane Smith',
            },
        ];
        return orders;
    }
    async getOrderById(id, req) {
        try {
            const user = req.user;
            const accessToken = user?.tokens?.accessToken;
            if (!accessToken) {
                throw new common_1.HttpException('Access token not found', common_1.HttpStatus.UNAUTHORIZED);
            }
            const ordersApiUrl = process.env.ORDERS_API_URL;
            const response = await this.authService.callDownstreamApi(accessToken, `${ordersApiUrl}/api/orders/${id}`, { method: 'GET' });
            if (!response.ok) {
                throw new common_1.HttpException('Failed to fetch order', response.status);
            }
            return response.json();
        }
        catch (error) {
            throw new common_1.HttpException(error.message || 'Failed to fetch order', error.status || common_1.HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
    async createOrder(orderData, req) {
        try {
            const user = req.user;
            const accessToken = user?.tokens?.accessToken;
            if (!accessToken) {
                throw new common_1.HttpException('Access token not found', common_1.HttpStatus.UNAUTHORIZED);
            }
            const ordersApiUrl = process.env.ORDERS_API_URL;
            const response = await this.authService.callDownstreamApi(accessToken, `${ordersApiUrl}/api/orders`, {
                method: 'POST',
                body: JSON.stringify(orderData),
            });
            if (!response.ok) {
                throw new common_1.HttpException('Failed to create order', response.status);
            }
            return response.json();
        }
        catch (error) {
            throw new common_1.HttpException(error.message || 'Failed to create order', error.status || common_1.HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
};
exports.OrdersController = OrdersController;
__decorate([
    (0, common_1.Get)('view-all'),
    (0, common_1.UseGuards)(session_auth_guard_1.SessionAuthGuard),
    __param(0, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", Promise)
], OrdersController.prototype, "getOrders", null);
__decorate([
    (0, common_1.Get)(':id'),
    __param(0, (0, common_1.Param)('id')),
    __param(1, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [String, Object]),
    __metadata("design:returntype", Promise)
], OrdersController.prototype, "getOrderById", null);
__decorate([
    (0, common_1.Post)(),
    __param(0, (0, common_1.Body)()),
    __param(1, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", Promise)
], OrdersController.prototype, "createOrder", null);
exports.OrdersController = OrdersController = __decorate([
    (0, common_1.Controller)('api/orders'),
    __metadata("design:paramtypes", [auth_service_1.AuthService])
], OrdersController);
//# sourceMappingURL=orders.controller.js.map