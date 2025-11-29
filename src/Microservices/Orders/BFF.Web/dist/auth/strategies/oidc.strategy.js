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
exports.OidcStrategy = void 0;
const common_1 = require("@nestjs/common");
const passport_1 = require("@nestjs/passport");
const openid_client_1 = require("openid-client");
const config_1 = require("@nestjs/config");
let OidcStrategy = class OidcStrategy extends (0, passport_1.PassportStrategy)(openid_client_1.Strategy, 'oidc') {
    constructor(configService) {
        const authority = configService.get('IDP_AUTHORITY') ?? 'https://localhost:44392';
        const clientId = configService.get('IDP_CLIENT_ID') ??
            'Orders.Microservice.BFF.ClientID';
        const clientSecret = configService.get('IDP_CLIENT_SECRET') ?? 'change-me';
        const callbackUrl = configService.get('IDP_CALLBACK_URL') ??
            'https://localhost:33800/api/auth/callback';
        const scopes = configService.get('IDP_SCOPES') ??
            'openid profile email orders_api';
        const issuer = new openid_client_1.Issuer({
            issuer: authority,
            authorization_endpoint: `${authority}/connect/authorize`,
            token_endpoint: `${authority}/connect/token`,
            userinfo_endpoint: `${authority}/connect/userinfo`,
            jwks_uri: `${authority}/.well-known/openid-configuration/jwks`,
        });
        const client = new issuer.Client({
            client_id: clientId,
            client_secret: clientSecret,
            redirect_uris: [callbackUrl],
            response_types: ['code'],
        });
        super({
            client,
            passReqToCallback: false,
            params: {
                scope: scopes,
            },
        });
        this.configService = configService;
    }
    async validate(tokenset, userinfo) {
        const user = {
            id: userinfo.sub,
            email: userinfo.email,
            name: userinfo.name,
            claims: userinfo,
            tokens: {
                accessToken: tokenset.access_token,
                idToken: tokenset.id_token,
                refreshToken: tokenset.refresh_token,
                expiresAt: tokenset.expires_at,
            },
        };
        return user;
    }
};
exports.OidcStrategy = OidcStrategy;
exports.OidcStrategy = OidcStrategy = __decorate([
    (0, common_1.Injectable)(),
    __metadata("design:paramtypes", [config_1.ConfigService])
], OidcStrategy);
//# sourceMappingURL=oidc.strategy.js.map