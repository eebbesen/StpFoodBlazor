using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Endpoints;
using StpFoodBlazor.Services;

namespace StpFoodBlazorTest.Endpoints
{
    public class CacheEndpointsTest
    {
        private const string ValidKey = "test-secret";

        private readonly IMemoryCache _cache;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _config;
        private readonly HttpContext _httpContext;
        private readonly ICacheRefreshService _refreshService;

        public CacheEndpointsTest()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["APPCONFIG:CACHEINVALIDATIONKEY"] = ValidKey
                })
                .Build();

            _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["X-Cache-Invalidation-Key"] = ValidKey;

            _refreshService = Substitute.For<ICacheRefreshService>();
            _refreshService.RefreshAsync(Arg.Any<string[]>()).Returns(Task.CompletedTask);
        }

        [Fact]
        public void Handle_ReturnsOk_WhenNoKeySpecified()
        {
            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            Assert.IsType<Ok<InvalidatedKeysResponse>>(result);
        }

        [Theory]
        [InlineData(CacheKeys.Deals)]
        [InlineData(CacheKeys.GiftCards)]
        public void Handle_ReturnsOk_WhenValidCacheKeySpecified(string cacheKey)
        {
            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService, cacheKey);

            Assert.IsType<Ok<InvalidatedKeysResponse>>(result);
        }

        [Fact]
        public void Handle_ReturnsBadRequest_WhenUnknownCacheKeySpecified()
        {
            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService, "unknown");

            Assert.IsType<BadRequest<string>>(result);
        }

        [Fact]
        public void Handle_ReturnsUnauthorized_WhenAuthKeyIsWrong()
        {
            _httpContext.Request.Headers["X-Cache-Invalidation-Key"] = "wrong-key";

            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            Assert.IsType<UnauthorizedHttpResult>(result);
        }

        [Fact]
        public void Handle_ReturnsUnauthorized_WhenAuthKeyIsMissing()
        {
            _httpContext.Request.Headers.Remove("X-Cache-Invalidation-Key");

            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            Assert.IsType<UnauthorizedHttpResult>(result);
        }

        [Fact]
        public void Handle_Returns503_WhenAuthKeyIsNotConfigured()
        {
            var config = new ConfigurationBuilder().Build();

            var result = CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, config, _refreshService);

            Assert.IsType<StatusCodeHttpResult>(result);
            Assert.Equal(503, ((StatusCodeHttpResult)result).StatusCode);
        }

        [Fact]
        public void Handle_RemovesAllSheetKeys_WhenNoCacheKeySpecified()
        {
            foreach (var key in CacheKeys.SheetKeys)
                _cache.Set(key, new object());

            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            foreach (var key in CacheKeys.SheetKeys)
                Assert.False(_cache.TryGetValue(key, out _), $"Expected cache key '{key}' to be removed.");
        }

        [Theory]
        [InlineData(CacheKeys.Deals)]
        [InlineData(CacheKeys.GiftCards)]
        public void Handle_RemovesOnlySpecifiedKey_WhenCacheKeyProvided(string targetKey)
        {
            foreach (var key in CacheKeys.SheetKeys)
                _cache.Set(key, new object());

            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService, targetKey);

            Assert.False(_cache.TryGetValue(targetKey, out _), $"Expected '{targetKey}' to be removed.");
            foreach (var key in CacheKeys.SheetKeys.Where(k => k != targetKey))
                Assert.True(_cache.TryGetValue(key, out _), $"Expected '{key}' to remain cached.");
        }

        [Fact]
        public void Handle_DoesNotRemoveCacheKeys_WhenAuthKeyIsWrong()
        {
            foreach (var key in CacheKeys.SheetKeys)
                _cache.Set(key, new object());

            _httpContext.Request.Headers["X-Cache-Invalidation-Key"] = "wrong-key";
            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            foreach (var key in CacheKeys.SheetKeys)
                Assert.True(_cache.TryGetValue(key, out _), $"Expected cache key '{key}' to still be present.");
        }

        [Fact]
        public void Handle_TriggersRefresh_WhenCacheIsInvalidated()
        {
            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            _refreshService.Received().RefreshAsync(CacheKeys.SheetKeys);
        }

        [Theory]
        [InlineData(CacheKeys.Deals)]
        [InlineData(CacheKeys.GiftCards)]
        public void Handle_TriggersRefreshForSpecificKey_WhenKeyIsProvided(string targetKey)
        {
            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService, targetKey);

            _refreshService.Received().RefreshAsync(Arg.Is<string[]>(keys => keys.Length == 1 && keys[0] == targetKey));
        }

        [Fact]
        public void Handle_DoesNotTriggerRefresh_WhenAuthFails()
        {
            _httpContext.Request.Headers["X-Cache-Invalidation-Key"] = "wrong-key";

            CacheEndpoints.Handle(_httpContext, _cache, _loggerFactory, _config, _refreshService);

            _refreshService.DidNotReceive().RefreshAsync(Arg.Any<string[]>());
        }

        [Fact]
        public void HandleStatus_ReturnsUnauthorized_WhenAuthKeyIsWrong()
        {
            _httpContext.Request.Headers["X-Cache-Invalidation-Key"] = "wrong-key";

            var result = CacheEndpoints.HandleStatus(_httpContext, _cache, _loggerFactory, _config);

            Assert.IsType<UnauthorizedHttpResult>(result);
        }

        [Fact]
        public void HandleStatus_Returns503_WhenAuthKeyIsNotConfigured()
        {
            var config = new ConfigurationBuilder().Build();

            var result = CacheEndpoints.HandleStatus(_httpContext, _cache, _loggerFactory, config);

            Assert.IsType<StatusCodeHttpResult>(result);
            Assert.Equal(503, ((StatusCodeHttpResult)result).StatusCode);
        }

        [Fact]
        public void HandleStatus_ReturnsAllKeysFalse_WhenCacheIsEmpty()
        {
            var result = CacheEndpoints.HandleStatus(_httpContext, _cache, _loggerFactory, _config);

            var response = Assert.IsType<Ok<CacheStatusResponse>>(result).Value!;
            Assert.All(response.Keys, kvp => Assert.False(kvp.Value));
        }

        [Fact]
        public void HandleStatus_ReflectsWarmAndColdKeys()
        {
            _cache.Set(CacheKeys.Deals, new object());

            var result = CacheEndpoints.HandleStatus(_httpContext, _cache, _loggerFactory, _config);

            var response = Assert.IsType<Ok<CacheStatusResponse>>(result).Value!;
            Assert.True(response.Keys[CacheKeys.Deals]);
            Assert.False(response.Keys[CacheKeys.GiftCards]);
            Assert.False(response.Keys[CacheKeys.Holidays]);
        }
    }
}
