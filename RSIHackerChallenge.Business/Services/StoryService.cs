using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSIHackerChallenge.Business.Interfaces;
using RSIHackerChallenge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSIHackerChallenge.Business.Services
{
    public class StoryService : IStoryService
    {
        private IMemoryCache _cache;
        private static HttpClient client = new HttpClient();
        private readonly ConfigurationSettings _config;

        public StoryService(IOptions<ConfigurationSettings> config, IMemoryCache cache)
        {
            _config = config.Value;
            _cache = cache;
        }
        /// <summary>
        /// Get the top 200 stories or stories based on the searched string
        /// </summary>
        /// <param name="searchText">Searched text from UI</param>
        /// <param name="pageNumber">page number like 1,2 or 10</param>
        /// <param name="pageSize">page size, for ex: 10,20 or 50</param>
        /// <returns>Return the list of stories based on parameters</returns>
        public async Task<ApiResponse> GetStories(string searchText, int pageNumber, int pageSize)
        {
            IEnumerable<Task<Story>> tasks = Enumerable.Empty<Task<Story>>();
            ApiResponse apiResponse = new ApiResponse()
            {
                Stories = new List<Story>()
            };
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 200;

            List<Story> stories = new List<Story>();
            string url = string.Format("{0}{1}", _config.HackerNewsBaseUrl, "beststories.json");
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                if (bestIds != null)
                {
                    int skip = (pageNumber - 1) * pageSize;
                    apiResponse.TotalCount = bestIds.Count;
                    if (String.IsNullOrEmpty(searchText))
                        tasks = bestIds.Skip(skip).Take(pageSize).Select(GetStoryAsync);
                    else
                        tasks = bestIds.Select(GetStoryAsync);
                    stories = (await Task.WhenAll(tasks)).ToList();

                    if (!String.IsNullOrEmpty(searchText))
                    {
                        var search = searchText.ToLower();
                        stories = stories.Where(s =>
                                           s.Title.ToLower().IndexOf(search) > -1 || s.By.ToLower().IndexOf(search) > -1)
                                           .Skip(skip).Take(pageSize).ToList();
                        apiResponse.TotalCount = stories.Count;
                    }
                }

            }

            apiResponse.Stories.AddRange(stories);
            return apiResponse;
        }

        /// <summary>
        /// pass Story id to GetStoryByIdAsync method to get the response and convert that to story object
        /// </summary>
        /// <param name="storyId">story id got from hacker api</param>
        /// <returns>Returns the Story based on id</returns>
        public async Task<Story> GetStoryAsync(int storyId)
        {
#pragma warning disable CS8603
            return await _cache.GetOrCreateAsync<Story>(storyId, async cacheEntry =>
            {
                var response = await GetStoryByIdAsync(storyId);
                if (response.IsSuccessStatusCode)
                {
                    var storyResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Story>(storyResponse);
                }

                return new Story();
            });
#pragma warning restore CS8603
        }

        /// <summary>
        /// Get story from hacker api based on story id
        /// </summary>
        /// <param name="id">story id from hacker api</param>
        /// <returns>Returns the http response from hacker api</returns>
        public async Task<HttpResponseMessage> GetStoryByIdAsync(int id)
        {
            return await client.GetAsync(string.Format("{0}{1}", _config.HackerNewsBaseUrl, string.Format("item/{0}.json", id)));
        }
    }
}
