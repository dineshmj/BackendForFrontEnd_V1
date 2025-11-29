import { Strategy, TokenSet, UserinfoResponse } from 'openid-client';
import { ConfigService } from '@nestjs/config';
declare const OidcStrategy_base: new (...args: any[]) => Strategy<unknown, import("openid-client").BaseClient>;
export declare class OidcStrategy extends OidcStrategy_base {
    private readonly configService;
    constructor(configService: ConfigService);
    validate(tokenset: TokenSet, userinfo: UserinfoResponse): Promise<any>;
}
export {};
