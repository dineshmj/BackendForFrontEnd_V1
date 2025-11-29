"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const core_1 = require("@nestjs/core");
const app_module_1 = require("./app.module");
const express_session_1 = __importDefault(require("express-session"));
const passport_1 = __importDefault(require("passport"));
const fs_1 = __importDefault(require("fs"));
const path_1 = __importDefault(require("path"));
async function bootstrap() {
    const httpsOptions = {
        key: fs_1.default.readFileSync(path_1.default.join(__dirname, '..', 'certs', 'localhost.key')),
        cert: fs_1.default.readFileSync(path_1.default.join(__dirname, '..', 'certs', 'localhost.crt')),
    };
    const app = await core_1.NestFactory.create(app_module_1.AppModule, {
        httpsOptions,
    });
    app.enableCors({
        origin: process.env.NEXTJS_URL,
        credentials: true,
    });
    app.use((0, express_session_1.default)({
        secret: process.env.SESSION_SECRET,
        resave: false,
        saveUninitialized: false,
        proxy: true,
        cookie: {
            httpOnly: true,
            secure: true,
            sameSite: 'none',
            maxAge: 24 * 60 * 60 * 1000,
        },
    }));
    app.use(passport_1.default.initialize());
    app.use(passport_1.default.session());
    const port = Number(process.env.PORT);
    await app.listen(port);
    console.log(`🚀 Orders BFF is running on: https://localhost:${port}`);
}
bootstrap();
//# sourceMappingURL=main.js.map