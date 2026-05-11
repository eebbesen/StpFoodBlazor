using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;

namespace StpFoodBlazor.Middleware
{
    public class SecurityHeadersMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var nonceBytes = new byte[16];
            RandomNumberGenerator.Fill(nonceBytes);
            var nonce = WebEncoders.Base64UrlEncode(nonceBytes);

            context.Items["csp-nonce"] = nonce;

            var headers = context.Response.Headers;

            headers["Content-Security-Policy"] =
                $"default-src 'self'; " +
                $"script-src 'self'; " +
                $"script-src-elem 'self'; " +
                $"script-src-attr 'none'; " +
                $"style-src 'self' 'nonce-{nonce}'; " +
                $"style-src-elem 'self' 'nonce-{nonce}'; " +
                $"style-src-attr 'unsafe-hashes' " +
                $"'sha256-phSae2Ud+nJs666rsURzxXg7FV5Tg7c+iiSFDGd3tAw=' " +
                $"'sha256-oLiTjTy/4afiaW/t7b0OVz122l2am89Dh+080MmksZM='; " +
                $"img-src 'self' data:; " +
                $"font-src 'self'; " +
                $"connect-src 'self'; " +
                $"worker-src 'self' blob:; " +
                $"frame-ancestors 'none'; " +
                $"form-action 'self'; " +
                $"base-uri 'self'; " +
                $"object-src 'none'; " +
                $"frame-src 'self'; " +
                $"media-src 'self'; " +
                $"manifest-src 'self'; " +
                $"upgrade-insecure-requests";

            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
            headers["Cross-Origin-Opener-Policy"] = "same-origin";
            headers["Cross-Origin-Resource-Policy"] = "same-origin";

            await next(context);
        }
    }
}
