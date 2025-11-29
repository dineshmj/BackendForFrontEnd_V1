import { ExecutionContext } from '@nestjs/common';
import { AuthService } from '../auth.service';
declare const SilentAuthGuard_base: import("@nestjs/passport").Type<import("@nestjs/passport").IAuthGuard>;
export declare class SilentAuthGuard extends SilentAuthGuard_base {
    private readonly authService;
    constructor(authService: AuthService);
    canActivate(context: ExecutionContext): Promise<boolean>;
    getAuthenticateOptions(context: ExecutionContext): {
        params: {
            prompt: string;
        };
        state: string;
    };
}
export {};
