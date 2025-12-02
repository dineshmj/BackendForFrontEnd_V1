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
exports.AuthController = void 0;
const common_1 = require("@nestjs/common");
const passport_1 = require("@nestjs/passport");
const auth_service_1 = require("./auth.service");
const silent_auth_guard_1 = require("./guards/silent-auth.guard");
let AuthController = class AuthController {
    constructor(authService) {
        this.authService = authService;
    }
    async silentLogin(returnUrl = '/', req, res) {
        if (!this.authService.isValidReturnUrl(returnUrl)) {
            console.log('Invalid returnUrl detected in silent-login:', returnUrl);
            const htmlContent = `
        <div style='padding:20px; border:1px solid #d0d8e8; background:#f7faff; color:#1a2c4e; border-radius:8px; font-family:Segoe UI,Roboto,sans-serif; max-width:500px;'>
          <div style='display:flex; align-items:center; font-size:18px; font-weight:600; margin-bottom:10px;'>
            <span style='font-size:22px; margin-right:8px;'>&#x2757;</span>
            Microservice Task Not Implemented
          </div>
          <div style='font-size:14px; line-height:1.6; color:#2d3e5e;'>
            This microservice task has not been implemented yet. Please check back later or contact the service owner.
          </div>
        </div>`;
            return res.status(common_1.HttpStatus.NOT_FOUND).send(htmlContent);
        }
        if (req.isAuthenticated && req.isAuthenticated()) {
            console.log('Silent login: user is already authenticated.');
            const spaUrl = `${process.env.NEXTJS_URL}${returnUrl}`;
            return res.redirect(spaUrl);
        }
        console.log('Silent login: presenting `Authentication Required` message.');
        const htmlContent = `
        <div style='padding:20px; border:1px solid #d0d8e8; background:#f7faff; color:#1a2c4e; border-radius:8px; font-family:Segoe UI,Roboto,sans-serif; max-width:500px;'>
          <div style='display:flex; align-items:center; font-size:18px; font-weight:600; margin-bottom:10px;'>
            <span style='font-size:22px; margin-right:8px;'>&#x2757;</span>
            Authentication Required
          </div>
          <div style='font-size:14px; line-height:1.6; color:#2d3e5e;'>
            Authentication is required to proceed with this operation.
          </div>
        </div>`;
        return res.status(common_1.HttpStatus.UNAUTHORIZED).send(htmlContent);
    }
    async oidcAuth(_req) {
    }
    async oidcCallback(req, res) {
        req.session.bffUser = req.user;
        const accessToken = req.user?.tokens?.accessToken;
        if (accessToken) {
            console.log('Storing access token in session for downstream API calls - ' + accessToken.substring(0, 10) + '...');
            req.session.AccessToken = accessToken;
        }
        const sessionReturnUrl = req.session?.returnUrl;
        const stateReturnUrl = req.query['state'] || undefined;
        const returnUrl = sessionReturnUrl || stateReturnUrl || '/';
        if (req.session) {
            delete req.session.returnUrl;
        }
        const base = (process.env.NEXTJS_URL ?? '').replace(/\/+$/, '');
        const spaUrl = `${base}${returnUrl}`;
        return res.redirect(spaUrl);
    }
    async silentLogoutPost(req, res) {
        req.session.AccessToken = null;
        return this.handleSilentLogout(req, res);
    }
    async silentLogoutGet(req, res) {
        req.session.AccessToken = null;
        return this.handleSilentLogout(req, res);
    }
    async handleSilentLogout(req, res) {
        try {
            if (!req.isAuthenticated || !req.isAuthenticated()) {
                return res.status(common_1.HttpStatus.OK).json({
                    success: true,
                    message: 'User was not authenticated',
                    alreadyLoggedOut: true,
                });
            }
            await new Promise((resolve, reject) => {
                req.logout((err) => {
                    if (err)
                        reject(err);
                    else
                        resolve();
                });
            });
            if (req.session) {
                req.session.destroy((err) => {
                    if (err) {
                        console.error('Session destroy error:', err);
                    }
                });
            }
            return res.status(common_1.HttpStatus.OK).json({
                success: true,
                message: 'Successfully logged out from Orders Microservice BFF',
                timestamp: new Date().toISOString(),
            });
        }
        catch (error) {
            return res.status(common_1.HttpStatus.INTERNAL_SERVER_ERROR).json({
                success: false,
                message: 'Logout failed',
                error: error?.message ?? 'Unknown error',
            });
        }
    }
    async getAuthStatus(req) {
        const user = req.session?.bffUser ?? req.user;
        return {
            isAuthenticated: !!user,
            userName: user?.name,
            email: user?.email,
            claims: user?.claims || {},
        };
    }
    async getAccessToken(req) {
        const user = req.session?.bffUser ?? req.user;
        if (!user?.tokens?.accessToken) {
            return {
                success: false,
                message: 'No access token available',
            };
        }
        return {
            success: true,
            accessToken: user.tokens.accessToken,
            expiresAt: user.tokens.expiresAt,
        };
    }
};
exports.AuthController = AuthController;
__decorate([
    (0, common_1.Get)('silent-login'),
    (0, common_1.UseGuards)(silent_auth_guard_1.SilentAuthGuard),
    __param(0, (0, common_1.Query)('returnUrl')),
    __param(1, (0, common_1.Req)()),
    __param(2, (0, common_1.Res)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [String, Object, Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "silentLogin", null);
__decorate([
    (0, common_1.Get)('oidc'),
    (0, common_1.UseGuards)((0, passport_1.AuthGuard)('oidc')),
    __param(0, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "oidcAuth", null);
__decorate([
    (0, common_1.Get)('callback'),
    (0, common_1.UseGuards)((0, passport_1.AuthGuard)('oidc')),
    __param(0, (0, common_1.Req)()),
    __param(1, (0, common_1.Res)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "oidcCallback", null);
__decorate([
    (0, common_1.Post)('silent-logout'),
    __param(0, (0, common_1.Req)()),
    __param(1, (0, common_1.Res)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "silentLogoutPost", null);
__decorate([
    (0, common_1.Get)('silent-logout'),
    __param(0, (0, common_1.Req)()),
    __param(1, (0, common_1.Res)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "silentLogoutGet", null);
__decorate([
    (0, common_1.Get)('status'),
    __param(0, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "getAuthStatus", null);
__decorate([
    (0, common_1.Get)('access-token'),
    __param(0, (0, common_1.Req)()),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", Promise)
], AuthController.prototype, "getAccessToken", null);
exports.AuthController = AuthController = __decorate([
    (0, common_1.Controller)('api/auth'),
    __metadata("design:paramtypes", [auth_service_1.AuthService])
], AuthController);
//# sourceMappingURL=auth.controller.js.map