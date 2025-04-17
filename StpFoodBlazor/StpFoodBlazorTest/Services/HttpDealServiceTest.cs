using Microsoft.Extensions.Logging;
using NSubstitute;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Services
{
    public class HttpDealServiceTests
    {
        // private readonly HttpClient _httpClient;
        private readonly ILogger<HttpDealService> _logger;
        private readonly HttpMessageHandler _messageHandler;
        private readonly string _testUrl;
        private static readonly string DEAL_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "deals.json");

        public HttpDealServiceTests()
        {
            // Setup test URL and mock handler
            _testUrl = Helper.GetUrl("Deals");
            _messageHandler = Substitute.For<HttpMessageHandler>();
            // _httpClient = new HttpClient(_messageHandler);
            _logger = Substitute.For<ILogger<HttpDealService>>();
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnDeals_WhenApiReturnsData()
        {
            var expectedDeals = GetFixtureContent();
            var messageHandlerMock = new MockHttpMessageHandler();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(GetFixtureContent())
            };

            messageHandlerMock.SetResponse(_testUrl, response);

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpDealService(httpClient, _logger);


            var result = await service.GetDealsAsync();

            Assert.NotNull(result);
            Assert.Equal(expectedDeals.Length, result.Length);
            Assert.Equal(expectedDeals[0].Name, result[0].Name);
            Assert.Equal(expectedDeals[0].Deal, result[0].Deal);
            Assert.Equal(expectedDeals[1].Name, result[1].Name);
            Assert.Equal(expectedDeals[1].Deal, result[1].Deal);
        }

        [Fact]
        public async Task GetDealsAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            var messageHandlerMock = new MockHttpMessageHandler();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((DealEvent[])null)
            };

            messageHandlerMock.SetResponse(_testUrl, response);

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpDealService(httpClient, _logger);

            var result = await service.GetDealsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        private static DealEvent[] GetFixtureContent()
        {
            var jsonContent = File.ReadAllText(DEAL_FIXTURES_PATH);
            return JsonSerializer.Deserialize<DealEvent[]>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
        }

        [Fact]
        public async Task GetDealsAsync_ShouldLogAndRethrow_WhenApiThrowsException()
        {
            var messageHandlerMock = new MockHttpMessageHandler();
            messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            var httpClient = new HttpClient(messageHandlerMock);
            var service = new HttpDealService(httpClient, _logger);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetDealsAsync());
        }
    }

    // Custom HttpMessageHandler for testing
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> _responses = new();

        public void SetResponse(string url, HttpResponseMessage response)
        {
            _responses[url] = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && _responses.TryGetValue(request.RequestUri.ToString(), out var response))
            {
                return Task.FromResult(response);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
