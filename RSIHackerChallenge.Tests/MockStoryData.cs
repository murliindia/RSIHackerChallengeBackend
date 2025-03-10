using Newtonsoft.Json;
using RSIHackerChallenge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSIHackerChallenge.Tests
{
    public static class MockStoryData
    {
        public static ApiResponse GetStories()
        {
            IEnumerable<Task<Story>> tasks = Enumerable.Empty<Task<Story>>();
            ApiResponse apiResponse = new ApiResponse()
            {
                Stories = new List<Story>()
                {
                    new Story()
                    {
                        By="Google",
                        Title="Google News",
                        Url="www.google.com"
                    },
                    new Story()
                    {
                        By="Yahoo",
                        Title="Yahoo News",
                        Url="www.Yahoo.com"
                    },
                    new Story()
                    {
                        By="Rediff",
                        Title="Rediff News",
                        Url="www.Rediff.com"
                    }
                },
                TotalCount = 3
            };
            return apiResponse;
        }

        public static HttpResponseMessage GetStoryResponseMessage()
        {
            var data = GetStories();
            string json = JsonConvert.SerializeObject(data);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return response;
        }
    }
}
