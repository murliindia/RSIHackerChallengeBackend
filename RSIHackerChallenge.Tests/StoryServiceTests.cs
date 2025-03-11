using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using RSIHackerChallenge.Business.Services;
using RSIHackerChallenge.Data;

namespace RSIHackerChallenge.Tests
{
    [TestFixture]
    public class StoryServiceTests
    {
        private StoryService _storyService;
        private Mock<IMemoryCache> _mockMemoryCache;
        private Mock<IOptions<ConfigurationSettings>> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ConfigurationSettings _configSettings;

        [SetUp]
        public void Setup()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();

            // Mocking IOptions<ConfigurationSettings>
            _mockConfig = new Mock<IOptions<ConfigurationSettings>>();
            _configSettings = new ConfigurationSettings
            {
                HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/"
            };
            _mockConfig.Setup(c => c.Value).Returns(_configSettings);

            // Mocking HttpMessageHandler for HttpClient
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            // ✅ Ensure _mockMemoryCache is set up properly
            _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>);

            // Initialize StoryService
            _storyService = new StoryService(_mockConfig.Object, _mockMemoryCache.Object);
        }


        [Test]
        public async Task GetStories_ReturnsStories_WhenApiReturnsValidResponse()
        {
            // Arrange: Mock API response for fetching best story IDs
            var storyIds = new List<int> { 101, 102, 103 };
            var storyJson = JsonConvert.SerializeObject(storyIds);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri != null && req.RequestUri.ToString().Contains("beststories.json")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(storyJson)
                });

            // Arrange: Mock API response for fetching individual stories
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri != null && req.RequestUri.ToString().Contains("item")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var storyResponse = JsonConvert.SerializeObject(new Story
                    {
                        Title = "Sample Story",
                        By = "TestUser",
                        Url = "https://example.com/story"
                    });

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(storyResponse)
                    };
                });

            // Ensure the HttpClient uses the mocked handler
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _storyService = new StoryService(_mockConfig.Object, _mockMemoryCache.Object);

            // Act: Call GetStories
            var response = await _storyService.GetStories(null, 1, 2);

            // Assert: Ensure the API returns valid stories
            Assert.NotNull(response);
            Assert.IsInstanceOf<ApiResponse>(response);
            Assert.That(response.Stories.Count, Is.GreaterThan(0));
            
        }

    }
}
