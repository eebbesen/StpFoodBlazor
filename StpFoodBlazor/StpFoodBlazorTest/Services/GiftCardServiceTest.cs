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
using System.Threading.Tasks;

namespace StpFoodBlazorTest.Services
{
    public class HttpGiftCardServiceTests
    {
        private readonly ILogger<HttpGiftCardService> _logger;
        private readonly MockHttpMessageHandler _messageHandlerMock;
        private readonly HttpGiftCardService _service;
        private readonly string _testUrl;
        private static readonly string GIFTCARD_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "giftcards.json");

        public HttpGiftCardServiceTests()
        {
            _testUrl = Helper.GetUrl("giftcards");
            _logger = Substitute.For<ILogger<HttpGiftCardService>>();
            _messageHandlerMock = new MockHttpMessageHandler();
            _service = new HttpGiftCardService(new HttpClient(_messageHandlerMock), _logger);
        }

        [Fact]
        public async Task GetGiftCardsAsync_ShouldReturnGiftCards_WhenApiReturnsData()
        {
            var expectedGiftCards = GetFixtureContent();

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(GetFixtureContent())
            };

            _messageHandlerMock.SetResponse(_testUrl, response);

            var result = await _service.GetGiftCardsAsync();

            Assert.NotNull(result);
            Assert.Equal(expectedGiftCards.Length, result.Length);
            Assert.Equal(expectedGiftCards[0].Deal, result[0].Deal);
            Assert.Equal(expectedGiftCards[0].Start, result[0].Start);
            Assert.Equal(expectedGiftCards[0].End, result[0].End);
            Assert.Equal(expectedGiftCards[1].Name, result[1].Name);
            Assert.Equal(expectedGiftCards[1].Terms, result[1].Terms);
            Assert.Equal(expectedGiftCards[1].URL, result[1].URL);
        }

        [Fact]
        public async Task GetGiftCardsAsync_ShouldReturnEmptyArray_WhenApiReturnsNull()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create((GiftCard[])null)
            };

            _messageHandlerMock.SetResponse(_testUrl, response);

            var result = await _service.GetGiftCardsAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGiftCardsAsync_ShouldLogAndRethrow_WhenApiThrowsException()
        {
            _messageHandlerMock.SetResponse(_testUrl, new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Test exception")
            });

            await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetGiftCardsAsync());
        }

        private static GiftCard[] GetFixtureContent()
        {
            var jsonContent = File.ReadAllText(GIFTCARD_FIXTURES_PATH);
            return JsonSerializer.Deserialize<GiftCard[]>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
        }
    }
}
