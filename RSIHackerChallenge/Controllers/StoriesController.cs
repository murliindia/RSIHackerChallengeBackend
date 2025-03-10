using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Business.Services;

namespace RSIHackerChallenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;
        public StoriesController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        /// <summary>
        /// Api to get top 200 stories based on the parameters passed in HttpContext
        /// </summary>
        /// <returns>List of Stories</returns>
        [HttpGet("NewStories")]
        public async Task<IActionResult> GetNewStories()
        {
            var pageNumber = HttpContext.Request.Query["pageNumber"];
            var pageSize = HttpContext.Request.Query["pageSize"];
            var searchString = HttpContext.Request.Query["searchString"];
            var data = await _storyService.GetStories(Convert.ToString(searchString),Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize));
            return Ok(data);
        }
    }
}
