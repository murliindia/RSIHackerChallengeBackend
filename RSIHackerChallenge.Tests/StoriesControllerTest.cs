using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using RSIHackerChallenge.API.Controllers;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;
using RSIHackerChallenge.Data;
using System.Net;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace RSIHackerChallenge.Tests
{
    [TestFixture]
    public class StoriesControllerTests
    {
        private StoriesController _storyController;
        private StoryService _storyService;
        private Mock<IStoryService> repository;
        private Mock<IMemoryCache> _mockCache;
        private Mock<IOptions<ConfigurationSettings>> _mockConfig;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;

        [SetUp]
        public void Initialise()
        {
            _mockConfig = new Mock<IOptions<ConfigurationSettings>>();
            _mockCache = new Mock<IMemoryCache>();
            var configurationSettings = new ConfigurationSettings
            {
                HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/",
            };
            _mockConfig.Setup(c => c.Value).Returns(configurationSettings);

            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            var queryParameters = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["pageNumber"] = new StringValues("1"),
                ["pageSize"] = new StringValues("10"),
                ["searchString"] = new StringValues("")
            });
            var expectedStory = new Story { Title = "Test Story" };

            mockRequest.SetupGet(r => r.Query).Returns(queryParameters);
            mockHttpContext.SetupGet(c => c.Request).Returns(mockRequest.Object);

            repository = new Mock<IStoryService>();
            repository.Setup(repo => repo.GetStories("", 1, 10)).ReturnsAsync(MockStoryData.GetStories());
            repository.Setup(repo => repo.GetStoryByIdAsync(1)).ReturnsAsync(MockStoryData.GetStoryResponseMessage());
            repository.Setup(repo => repo.GetStoryAsync(1)).ReturnsAsync(MockStoryData.GetStories().Stories.Where(x => x.By == "Google").FirstOrDefault());

            _storyController = new StoriesController(repository.Object);
            _storyController.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };


            _storyService = new StoryService(_mockConfig.Object, _mockCache.Object);

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        }

        /// <summary>
        /// Test case for the successfull test of GetNewStories method from controller
        /// </summary>
        /// <returns>Test case passed or fail</returns>
        [Test]
        public async Task GetNewStories_ReturnsOkResult()
        {

            var result = await _storyController.GetNewStories() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var data = result.Value as ApiResponse;

            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.TotalCount);
            Assert.AreEqual("Google", data.Stories.Select(s => s.By).FirstOrDefault());
        }

        /// <summary>
        /// Get Story By ID method of Story service
        /// </summary>
        /// <returns>pass or fail</returns>
        [Test]
        public async Task GetStoryByIdAsync_WithValidResponse_ReturnsStory()
        {
            var expectedStory = new Story { By = "Google", Title = "Test Google Story" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponse.Content = new StringContent(JsonConvert.SerializeObject(expectedStory));
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var result = await repository.Object.GetStoryByIdAsync(1);


            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode.ToString(), "OK");
        }

        /// <summary>
        /// GetStoryAsync method of Story service
        /// </summary>
        /// <returns>pass or fail</returns>
        [Test]
        public async Task GetStoryAsync_WithValidResponse_ReturnsStory()
        {
            var expectedStory = new Story { By = "Google", Title = "Test Google Story" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponse.Content = new StringContent(JsonConvert.SerializeObject(expectedStory));
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponse);

            var result = await repository.Object.GetStoryAsync(1);


            Assert.IsNotNull(result);
            Assert.AreEqual(result.By, "Google");
        }

    }

}