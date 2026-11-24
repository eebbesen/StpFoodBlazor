using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
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
    public class HttpSnelrandDealServiceTests
    {
        private readonly MockHttpMessageHandler _messageHandlerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpSnelrandDealService _service;
        private readonly string _testUrl;
        private static readonly string DEAL_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "deals.json");

        public HttpSnelrandDealServiceTests()
        {
            Environment.SetEnvironmentVariable("APPCONFIG__SHEETSURL", "http://test-sheets-url");
            Environment.SetEnvironmentVariable("APPCONFIG__SHEETID", "test-sheet-id");
            Environment.SetEnvironmentVariable("APPCONFIG__SNELRANDSHEETID", "test-snelrand-sheet-id");
            _testUrl = Helper.GetUrl("Deals", Helper.SNELRAND_SHEET_ID_VARIABLE);
            _messageHandlerMock = new MockHttpMessageHandler();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["CacheDuration:DealsSnelrandMinutes"] = "200"
                })
                .Build();
            _service = new HttpSnelrandDealService(_memoryCache, new HttpClient(_messageHandlerMock),
                Substitute.For<ILogger<HttpSnelrandDealService>>(), config);
        }

        [Fact]
        public void Service_ImplementsSnelrandDealService()
        {
            Assert.IsAssignableFrom<ISnelrandDealService>(_service);
        }

        [Fact]
        public void Url_UsesSnelrandSheetId()
        {
            Assert.Contains("sheet_id=test-snelrand-sheet-id", _testUrl);
            Assert.Contains("tab_name=Deals", _testUrl);
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnDealsAndUseOwnCacheKey_WhenApiReturnsData()
        {
            var expectedDeals = GetFixtureContent();
            _messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedDeals)
            });

            var result = await _service.GetDealsAsync();

            Assert.Equal(expectedDeals.Length, result.Length);
            Assert.Equal(expectedDeals[0].Name, result[0].Name);
            Assert.True(_memoryCache.TryGetValue(CacheKeys.DealsSnelrand, out _));
            Assert.False(_memoryCache.TryGetValue(CacheKeys.Deals, out _));
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnCachedSnelrandDeals_AndIgnoreMainDealsCache()
        {
            DealEvent[] snelrandDeals = GetFixtureContent()[0..2];
            DealEvent[] mainDeals = GetFixtureContent()[2..5];
            _memoryCache.Set(CacheKeys.DealsSnelrand, snelrandDeals);
            _memoryCache.Set(CacheKeys.Deals, mainDeals);

            var result = await _service.GetDealsAsync();

            Assert.Equal(2, result.Length);
            Assert.Equal(snelrandDeals[0].Name, result[0].Name);
            Assert.Equal(snelrandDeals[1].Deal, result[1].Deal);
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            _messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((DealEvent[]?)null)
            });

            var result = await _service.GetDealsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnEmptyArray_WhenApiReturnsErrorObject()
        {
            _messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """{"statusCode":400,"body":{"error":"google rejected sheet_id and/or tab_name"}}""",
                    System.Text.Encoding.UTF8,
                    "application/json")
            });

            var result = await _service.GetDealsAsync();

            Assert.Empty(result);
            Assert.False(_memoryCache.TryGetValue(CacheKeys.DealsSnelrand, out _));
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnEmptyArray_WhenApiThrowsException()
        {
            _messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            var result = await _service.GetDealsAsync();

            Assert.Empty(result);
        }

        private static DealEvent[] GetFixtureContent()
        {
            var jsonContent = File.ReadAllText(DEAL_FIXTURES_PATH);
            return JsonSerializer.Deserialize<DealEvent[]>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
        }
    }
}
