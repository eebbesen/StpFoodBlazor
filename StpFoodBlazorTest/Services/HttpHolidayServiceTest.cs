using AngleSharp.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Services
{
    public class HttpHolidayServiceTests
    {
        private readonly ILogger<HttpHolidayService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpHolidayService _service;
        private readonly MockHttpMessageHandler _messageHandlerMock;
        private static readonly string URL_BASE = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL");
        private static readonly string URL_TODAY = URL_BASE + "/today/";
        private static readonly string URL_RANGE = URL_BASE + "/range/?startDate=10-01&endDate=10-02";
        private static readonly string HOLIDAY_DATA = @"
            {
                ""2023-10-01"": [
                    ""National Homemade Cookies Day""
                ],
                ""2023-10-02"": [
                    ""National Fried Scallops Day""
                ]
            }";

        public HttpHolidayServiceTests()
        {
            _logger = Substitute.For<ILogger<HttpHolidayService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _messageHandlerMock = new MockHttpMessageHandler();
            _service = new HttpHolidayService(_memoryCache, new HttpClient(_messageHandlerMock), _logger);
        }

        [Fact]
        public async Task GetTodaysHolidaysAsync_ShouldReturnHolidays_WhenApiReturnsData()
        {
            var expectedResult = GetFixtureContent(1);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResult)
            };

            _messageHandlerMock.SetResponse(URL_TODAY, response);

            var result = await _service.GetTodaysHolidaysAsync();

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetTodaysHolidaysAsync_ShouldReturnCachedHolidays_WhenCacheReturnsData()
        {
            var expectedResult = GetFixtureContent(1);

            Dictionary<string, string[]> cachedHolidays = expectedResult.First().Value.ToDictionary(k => k, v => new[] { v });
            _memoryCache.Set("holidays", cachedHolidays, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(400)
            });

            var result = await _service.GetTodaysHolidaysAsync();

            Assert.Single(result);
            Assert.Equal(expectedResult.First().Value, result.First().Value);
        }

        [Fact]
        public async Task GetTodaysHolidaysAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((string[])null)
            };

            _messageHandlerMock.SetResponse(URL_TODAY, response);

            var result = await _service.GetTodaysHolidaysAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTodaysHolidaysAsync_ShouldLogAndRethrow_WhenApiThrowsException()
        {
            _messageHandlerMock.SetResponse(URL_TODAY, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetTodaysHolidaysAsync());
        }

        [Fact]
        public async Task GetHolidaysRangeAsync_ShouldReturnHolidays_WhenApiReturnsData()
        {
            var expectedResult = GetFixtureContent();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResult)
            };

            _messageHandlerMock.SetResponse(URL_RANGE, response);

            var result = await _service.GetHolidaysRangeAsync("10-01", "10-02");

            Assert.NotNull(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetHolidaysRangeAsync_ShouldReturnCachedHolidays_WhenCacheReturnsData()
        {
            var expectedResult = GetFixtureContent(1);

            Dictionary<string, string[]> cachedHolidays = expectedResult;

            _memoryCache.Set("holidays", cachedHolidays, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(400)
            });

            var result = await _service.GetHolidaysRangeAsync("10-01", "10-02");

            Assert.Single(result);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetHolidaysRangeAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((string[])null)
            };

            _messageHandlerMock.SetResponse(URL_RANGE, response);

            var result = await _service.GetHolidaysRangeAsync("10-01", "10-02");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetHolidaysRangeAsync_ShouldLogAndRethrow_WhenApiThrowsException()
        {
            _messageHandlerMock.SetResponse(URL_RANGE, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetHolidaysRangeAsync("10-01", "10-02"));
        }

        private static Dictionary<string, string[]> GetFixtureContent(int limit = -1)
        {
            var holidays = JsonSerializer.Deserialize<Dictionary<string, string[]>>(HOLIDAY_DATA) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");

            if (limit > -1)
            {
                return holidays.Take(limit).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            return holidays;
        }
    }
}
