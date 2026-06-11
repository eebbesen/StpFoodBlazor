using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Services
{
    public class CacheRefreshServiceTests
    {
        private readonly IMemoryCache _cache;
        private readonly MockHttpMessageHandler _dealHandler;
        private readonly MockHttpMessageHandler _giftCardHandler;
        private readonly MockHttpMessageHandler _holidayHandler;
        private readonly HttpDealService _dealService;
        private readonly HttpGiftCardService _giftCardService;
        private readonly HttpHolidayService _holidayService;
        private readonly ILogger<CacheRefreshService> _logger;
        private readonly CacheRefreshService _service;
        private readonly string _dealUrl;
        private readonly string _giftCardUrl;
        private readonly string _holidayUrl;

        public CacheRefreshServiceTests()
        {
            Environment.SetEnvironmentVariable("APPCONFIG__SHEETSURL", "http://test-sheets-url");
            Environment.SetEnvironmentVariable("APPCONFIG__SHEETID", "test-sheet-id");

            _cache = new MemoryCache(new MemoryCacheOptions());
            _logger = Substitute.For<ILogger<CacheRefreshService>>();
            _dealHandler = new MockHttpMessageHandler();
            _giftCardHandler = new MockHttpMessageHandler();
            _holidayHandler = new MockHttpMessageHandler();

            var dealConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["CacheDuration:DealsMinutes"] = "200" })
                .Build();
            var giftCardConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["CacheDuration:GiftCardsMinutes"] = "400" })
                .Build();
            var holidayConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["APPCONFIG:HOLIDAYURL"] = "http://test-holiday-url",
                    ["CacheDuration:HolidaysMinutes"] = "400"
                })
                .Build();

            _dealService = new HttpDealService(_cache, new HttpClient(_dealHandler), Substitute.For<ILogger<HttpDealService>>(), dealConfig);
            _giftCardService = new HttpGiftCardService(_cache, new HttpClient(_giftCardHandler), Substitute.For<ILogger<HttpGiftCardService>>(), giftCardConfig);
            _holidayService = new HttpHolidayService(_cache, new HttpClient(_holidayHandler), Substitute.For<ILogger<HttpHolidayService>>(), holidayConfig);
            _service = new CacheRefreshService(_dealService, _giftCardService, _holidayService, _logger);

            _dealUrl = StpFoodBlazor.Helpers.Helper.GetUrl("Deals");
            _giftCardUrl = StpFoodBlazor.Helpers.Helper.GetUrl("giftcards");
            _holidayUrl = "http://test-holiday-url/today/";
        }

        [Fact]
        public async Task RefreshAsync_PopulatesDealsCache_WhenDealsKeyProvided()
        {
            var deals = JsonSerializer.Deserialize<StpFoodBlazor.Models.DealEvent[]>(
                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "deals.json")))!;
            _dealHandler.SetResponse(_dealUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(deals)
            });

            await _service.RefreshAsync([CacheKeys.Deals]);

            Assert.True(_cache.TryGetValue(CacheKeys.Deals, out _));
        }

        [Fact]
        public async Task RefreshAsync_PopulatesGiftCardsCache_WhenGiftCardsKeyProvided()
        {
            var giftCards = JsonSerializer.Deserialize<StpFoodBlazor.Models.GiftCard[]>(
                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "giftcards.json")))!;
            _giftCardHandler.SetResponse(_giftCardUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(giftCards)
            });

            await _service.RefreshAsync([CacheKeys.GiftCards]);

            Assert.True(_cache.TryGetValue(CacheKeys.GiftCards, out _));
        }

        [Fact]
        public async Task RefreshAsync_PopulatesHolidaysCache_WhenHolidaysKeyProvided()
        {
            var holidays = new Dictionary<string, string[]>
            {
                [DateTime.Now.ToString("MM-dd")] = ["Sample Holiday"]
            };
            _holidayHandler.SetResponse(_holidayUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(holidays)
            });

            await _service.RefreshAsync([CacheKeys.Holidays]);

            Assert.True(_cache.TryGetValue(CacheKeys.Holidays, out _));
        }

        [Fact]
        public async Task RefreshAsync_ContinuesWhenOneKeyFails()
        {
            var giftCards = JsonSerializer.Deserialize<StpFoodBlazor.Models.GiftCard[]>(
                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "giftcards.json")))!;
            _dealHandler.SetResponse(_dealUrl, new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });
            _giftCardHandler.SetResponse(_giftCardUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(giftCards)
            });

            await _service.RefreshAsync([CacheKeys.Deals, CacheKeys.GiftCards]);

            Assert.True(_cache.TryGetValue(CacheKeys.GiftCards, out _));
        }

        [Fact]
        public async Task RefreshAsync_DoesNotThrow_WhenUnknownKeyProvided()
        {
            var exception = await Record.ExceptionAsync(() => _service.RefreshAsync(["unknown-key"]));

            Assert.Null(exception);
            Assert.False(_cache.TryGetValue(CacheKeys.Deals, out _));
            Assert.False(_cache.TryGetValue(CacheKeys.GiftCards, out _));
            Assert.False(_cache.TryGetValue(CacheKeys.Holidays, out _));
        }

        [Fact]
        public async Task RefreshAsync_DoesNotThrow_WhenApiReturnsError()
        {
            _dealHandler.SetResponse(_dealUrl, new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError });

            var exception = await Record.ExceptionAsync(() => _service.RefreshAsync([CacheKeys.Deals]));

            Assert.Null(exception);
        }
    }
}
