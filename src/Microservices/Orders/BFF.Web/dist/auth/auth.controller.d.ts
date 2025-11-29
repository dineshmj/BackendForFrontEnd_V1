import { Request, Response } from 'express';
import { AuthService } from './auth.service';
export declare class AuthController {
    private authService;
    constructor(authService: AuthService);
    silentLogin(returnUrl: string, req: Request, res: Response): Promise<void | Response<any, Record<string, any>>>;
    oidcAuth(_req: Request): Promise<void>;
    oidcCallback(req: Request, res: Response): Promise<void>;
    silentLogoutPost(req: Request, res: Response): Promise<Response<any, Record<string, any>>>;
    silentLogoutGet(req: Request, res: Response): Promise<Response<any, Record<string, any>>>;
    private handleSilentLogout;
    getAuthStatus(req: Request): Promise<{
        isAuthenticated: boolean;
        userName: any;
        email: any;
        claims: any;
    }>;
    getAccessToken(req: Request): Promise<{
        success: boolean;
        message: string;
        accessToken?: undefined;
        expiresAt?: undefined;
    } | {
        success: boolean;
        accessToken: any;
        expiresAt: any;
        message?: undefined;
    }>;
}
