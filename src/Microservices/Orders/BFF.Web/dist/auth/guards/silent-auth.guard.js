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
Object.defineProperty(exports, "__esModule", { value: true });
exports.SilentAuthGuard = void 0;
const common_1 = require("@nestjs/common");
const passport_1 = require("@nestjs/passport");
const auth_service_1 = require("../auth.service");
let SilentAuthGuard = class SilentAuthGuard extends (0, passport_1.AuthGuard)('oidc') {
    constructor(authService) {
        super();
        this.authService = authService;
    }
    async canActivate(context) {
        const req = context.switchToHttp().getRequest();
        const returnUrl = req.query.returnUrl ?? '/';
        if (!this.authService.isValidReturnUrl(returnUrl)) {
            return true;
        }
        if (req.isAuthenticated && req.isAuthenticated()) {
            return true;
        }
        req.session.returnUrl = returnUrl;
        return (await super.canActivate(context));
    }
    getAuthenticateOptions(context) {
        const req = context.switchToHttp().getRequest();
        const returnUrl = req.query.returnUrl ?? '/';
        return {
            params: {
                prompt: 'none',
            },
            state: returnUrl,
        };
    }
};
exports.SilentAuthGuard = SilentAuthGuard;
exports.SilentAuthGuard = SilentAuthGuard = __decorate([
    (0, common_1.Injectable)(),
    __metadata("design:paramtypes", [auth_service_1.AuthService])
], SilentAuthGuard);
//# sourceMappingURL=silent-auth.guard.js.map