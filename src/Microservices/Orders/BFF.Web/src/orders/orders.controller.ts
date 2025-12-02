import {
  Body,
  Controller,
  Get,
  HttpException,
  HttpStatus,
  Param,
  Post,
  Req,
  UseGuards,
} from '@nestjs/common';
import { Request } from 'express';
import { AuthService } from '../auth/auth.service';
import { SessionAuthGuard } from '../auth/guards/session-auth.guard';

@Controller('api/orders')
export class OrdersController {
  constructor(private readonly authService: AuthService) {}

  @Get('view-all')
  async getOrders(@Req() req: Request) {
      const accessToken = (req.session as any)?.AccessToken;

      // const user = req.user as any;
      // const accessToken = user?.tokens?.accessToken;

      console.log('Accessing getOrders with access token:', accessToken ? accessToken.substring(0, 10) + '...' : 'none');

      if (!accessToken) {
        throw new HttpException(
          'Access token not found',
          HttpStatus.UNAUTHORIZED,
        );
      }

    // For now, ignore access token and return mock data
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

  @Get(':id')
  async getOrderById(@Param('id') id: string, @Req() req: Request) {
    try {
      const user = req.user as any;
      const accessToken = user?.tokens?.accessToken;

      if (!accessToken) {
        throw new HttpException(
          'Access token not found',
          HttpStatus.UNAUTHORIZED,
        );
      }

      const ordersApiUrl = process.env.ORDERS_API_URL;
      const response = await this.authService.callDownstreamApi(
        accessToken,
        `${ordersApiUrl}/api/orders/${id}`,
        { method: 'GET' },
      );

      if (!response.ok) {
        throw new HttpException(
          'Failed to fetch order',
          response.status,
        );
      }

      return response.json();
    } catch (error) {
      throw new HttpException(
        error.message || 'Failed to fetch order',
        error.status || HttpStatus.INTERNAL_SERVER_ERROR,
      );
    }
  }

  @Post()
  async createOrder(@Body() orderData: any, @Req() req: Request) {
    try {
      const user = req.user as any;
      const accessToken = user?.tokens?.accessToken;

      if (!accessToken) {
        throw new HttpException(
          'Access token not found',
          HttpStatus.UNAUTHORIZED,
        );
      }

      const ordersApiUrl = process.env.ORDERS_API_URL;
      const response = await this.authService.callDownstreamApi(
        accessToken,
        `${ordersApiUrl}/api/orders`,
        {
          method: 'POST',
          body: JSON.stringify(orderData),
        },
      );

      if (!response.ok) {
        throw new HttpException(
          'Failed to create order',
          response.status,
        );
      }

      return response.json();
    } catch (error) {
      throw new HttpException(
        error.message || 'Failed to create order',
        error.status || HttpStatus.INTERNAL_SERVER_ERROR,
      );
    }
  }
}