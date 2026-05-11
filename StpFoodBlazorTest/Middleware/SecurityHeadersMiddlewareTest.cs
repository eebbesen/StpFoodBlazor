using Microsoft.AspNetCore.Http;
using NSubstitute;
using StpFoodBlazor.Middleware;
using System.Linq;
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Middleware
{
    public class SecurityHeadersMiddlewareTests
    {
        private readonly RequestDelegate _next;
        private readonly SecurityHeadersMiddleware _middleware;

        public SecurityHeadersMiddlewareTests()
        {
            _next = Substitute.For<RequestDelegate>();
            _middleware = new SecurityHeadersMiddleware(_next);
        }

        [Fact]
        public async Task InvokeAsync_AlwaysCallsNext()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            await _next.Received(1).Invoke(context);
        }

        [Fact]
        public async Task InvokeAsync_SetsNonceInContextItems()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.True(context.Items.ContainsKey("csp-nonce"));
            var nonce = Assert.IsType<string>(context.Items["csp-nonce"]);
            Assert.NotEmpty(nonce);
        }

        [Fact]
        public async Task InvokeAsync_NonceAppearsInCspHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            var nonce = (string)context.Items["csp-nonce"]!;
            var csp = context.Response.Headers["Content-Security-Policy"].ToString();
            Assert.Contains($"'nonce-{nonce}'", csp);
        }

        [Fact]
        public async Task InvokeAsync_GeneratesUniqueNoncePerRequest()
        {
            var context1 = MakeContext();
            var context2 = MakeContext();

            await _middleware.InvokeAsync(context1);
            await _middleware.InvokeAsync(context2);

            var nonce1 = (string)context1.Items["csp-nonce"]!;
            var nonce2 = (string)context2.Items["csp-nonce"]!;
            Assert.NotEqual(nonce1, nonce2);
        }

        [Fact]
        public async Task InvokeAsync_CspHeaderIsNotDuplicated()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Single(context.Response.Headers["Content-Security-Policy"]);
        }

        [Fact]
        public async Task InvokeAsync_SetsCspHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            var csp = context.Response.Headers["Content-Security-Policy"].ToString();
            Assert.Contains("default-src 'self'", csp);
            Assert.Contains("script-src 'self'", csp);
            Assert.Contains("script-src-elem 'self'", csp);
            Assert.Contains("script-src-attr 'none'", csp);
            Assert.Contains("frame-ancestors 'none'", csp);
            Assert.Contains("object-src 'none'", csp);
            Assert.Contains("base-uri 'self'", csp);
            Assert.Contains("form-action 'self'", csp);
            Assert.Contains("upgrade-insecure-requests", csp);
            Assert.Contains("worker-src 'self' blob:", csp);
        }

        [Fact]
        public async Task InvokeAsync_CspDoesNotContainFontSrcData()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            var csp = context.Response.Headers["Content-Security-Policy"].ToString();
            Assert.DoesNotContain("font-src data:", csp);
        }

        [Fact]
        public async Task InvokeAsync_StyleSrcAttrContainsUnsafeHashesAndHashes()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            var csp = context.Response.Headers["Content-Security-Policy"].ToString();
            Assert.Contains("style-src-attr 'unsafe-hashes'", csp);
            Assert.Contains("'sha256-phSae2Ud+nJs666rsURzxXg7FV5Tg7c+iiSFDGd3tAw='", csp);
            Assert.Contains("'sha256-oLiTjTy/4afiaW/t7b0OVz122l2am89Dh+080MmksZM='", csp);
        }

        [Fact]
        public async Task InvokeAsync_StyleSrcDoesNotContainUnsafeHashes()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            // 'unsafe-hashes' must only appear in style-src-attr (narrowest scope), not in style-src
            var csp = context.Response.Headers["Content-Security-Policy"].ToString();
            var styleSrc = csp.Split("; ").First(d => d.StartsWith("style-src ") && !d.StartsWith("style-src-"));
            Assert.DoesNotContain("'unsafe-hashes'", styleSrc);
        }

        [Fact]
        public async Task InvokeAsync_SetsXFrameOptionsHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Equal("DENY", context.Response.Headers["X-Frame-Options"].ToString());
        }

        [Fact]
        public async Task InvokeAsync_SetsXContentTypeOptionsHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Equal("nosniff", context.Response.Headers["X-Content-Type-Options"].ToString());
        }

        [Fact]
        public async Task InvokeAsync_SetsReferrerPolicyHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Equal("strict-origin-when-cross-origin", context.Response.Headers["Referrer-Policy"].ToString());
        }

        [Fact]
        public async Task InvokeAsync_SetsPermissionsPolicyHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            var policy = context.Response.Headers["Permissions-Policy"].ToString();
            Assert.Contains("camera=()", policy);
            Assert.Contains("geolocation=()", policy);
            Assert.Contains("microphone=()", policy);
            Assert.Contains("payment=()", policy);
        }

        [Fact]
        public async Task InvokeAsync_SetsCrossOriginOpenerPolicyHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Equal("same-origin", context.Response.Headers["Cross-Origin-Opener-Policy"].ToString());
        }

        [Fact]
        public async Task InvokeAsync_SetsCrossOriginResourcePolicyHeader()
        {
            var context = MakeContext();

            await _middleware.InvokeAsync(context);

            Assert.Equal("same-origin", context.Response.Headers["Cross-Origin-Resource-Policy"].ToString());
        }

        private static DefaultHttpContext MakeContext() => new();
    }
}
