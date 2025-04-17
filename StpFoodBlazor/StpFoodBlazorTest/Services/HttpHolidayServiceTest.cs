using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly string URL_BASE = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL");
        private static readonly string HOLIDAY_DATA = @"
            [{
                ""2023-10-01"": [
                    ""National Homemade Cookies Day"",
                ],
                ""2023-10-02"": [
                    ""National Fried Scallops Day"",
                ]
            }]";

        public HttpHolidayServiceTests()
        {
            _logger = Substitute.For<ILogger<HttpHolidayService>>();
        }

        [Fact]
        public async Task GetHolidaysAsync_ShouldReturnHolidays_WhenApiReturnsData()
        {
            var messageHandlerMock = new MockHttpMessageHandler();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(GetFixtureContent().First())
            };

            messageHandlerMock.SetResponse(URL_BASE + "/today/", response);

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpHolidayService(httpClient, _logger);

            var result = await service.GetTodaysHolidaysAsync();

            Assert.NotNull(result);
            // Assert.Equal(JsonContent.Create(GetFixtureContent(), result);
        }

        [Fact]
        public async Task GetHolidaysAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            var messageHandlerMock = new MockHttpMessageHandler();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((string[])null)
            };

            messageHandlerMock.SetResponse(URL_BASE + "/today/", response);

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpHolidayService(httpClient, _logger);

            var result = await service.GetTodaysHolidaysAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetHolidaysAsync_ShouldLogAndRethrow_WhenApiThrowsException()
        {
            var messageHandlerMock = new MockHttpMessageHandler();
            messageHandlerMock.SetResponse(URL_BASE + "/today/", new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpHolidayService(httpClient, _logger);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetTodaysHolidaysAsync());
        }

        private static Dictionary<string, string[]> GetFixtureContent()
        {
            var jsonContent = HOLIDAY_DATA;
            return JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
        }
    }
}
