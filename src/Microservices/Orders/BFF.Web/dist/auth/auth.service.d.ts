import { ConfigService } from '@nestjs/config';
export declare class AuthService {
    private configService;
    constructor(configService: ConfigService);
    isValidReturnUrl(returnUrl: string): boolean;
    callDownstreamApi(accessToken: string, apiUrl: string, options?: RequestInit): Promise<Response>;
    getOrders(accessToken: string): Promise<any>;
}
