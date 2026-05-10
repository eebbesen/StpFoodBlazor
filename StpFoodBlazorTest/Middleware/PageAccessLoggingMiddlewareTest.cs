using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Middleware;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Middleware
{
    public class PageAccessLoggingMiddlewareTests
    {
        private readonly TestLogger<PageAccessLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly PageAccessLoggingMiddleware _middleware;

        public PageAccessLoggingMiddlewareTests()
        {
            _logger = new TestLogger<PageAccessLoggingMiddleware>();
            _next = Substitute.For<RequestDelegate>();
            _middleware = new PageAccessLoggingMiddleware(_next, _logger);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/deals")]
        [InlineData("/giftcards")]
        [InlineData("/about")]
        public async Task InvokeAsync_LogsAccess_ForPageRequests(string path)
        {
            await _middleware.InvokeAsync(MakeContext(path));

            Assert.Single(_logger.InfoLogs);
        }

        [Theory]
        [InlineData("/_blazor")]
        [InlineData("/_blazor/negotiate")]
        [InlineData("/_framework/blazor.web.js")]
        [InlineData("/api/cache")]
        [InlineData("/api/cache/invalidate")]
        [InlineData("/app.css")]
        [InlineData("/favicon.ico")]
        [InlineData("/images/logo.png")]
        public async Task InvokeAsync_DoesNotLog_ForNonPageRequests(string path)
        {
            await _middleware.InvokeAsync(MakeContext(path));

            Assert.Empty(_logger.InfoLogs);
        }

        [Fact]
        public async Task InvokeAsync_UsesFirstForwardedIp_WhenXForwardedForPresent()
        {
            var context = MakeContext("/deals");
            context.Request.Headers["X-Forwarded-For"] = "203.0.113.5, 10.0.0.1";

            await _middleware.InvokeAsync(context);

            Assert.Contains("203.0.113.5", _logger.InfoLogs[0]);
            Assert.DoesNotContain("10.0.0.1", _logger.InfoLogs[0]);
        }

        [Fact]
        public async Task InvokeAsync_UsesRemoteIp_WhenXForwardedForAbsent()
        {
            var context = MakeContext("/deals");
            context.Connection.RemoteIpAddress = IPAddress.Parse("198.51.100.42");

            await _middleware.InvokeAsync(context);

            Assert.Contains("198.51.100.42", _logger.InfoLogs[0]);
        }

        [Fact]
        public async Task InvokeAsync_LogsPath_InMessage()
        {
            await _middleware.InvokeAsync(MakeContext("/deals"));

            Assert.Contains("/deals", _logger.InfoLogs[0]);
        }

        [Fact]
        public async Task InvokeAsync_AlwaysCallsNext()
        {
            var context = MakeContext("/deals");

            await _middleware.InvokeAsync(context);

            await _next.Received(1).Invoke(context);
        }

        [Fact]
        public async Task InvokeAsync_CallsNext_EvenForNonPageRequests()
        {
            var context = MakeContext("/_blazor/negotiate");

            await _middleware.InvokeAsync(context);

            await _next.Received(1).Invoke(context);
        }

        private static DefaultHttpContext MakeContext(string path) => new()
        {
            Request = { Path = path }
        };

        private class TestLogger<T> : ILogger<T>
        {
            public List<string> InfoLogs { get; } = [];

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (logLevel == LogLevel.Information)
                    InfoLogs.Add(formatter(state, exception));
            }
        }
    }
}
