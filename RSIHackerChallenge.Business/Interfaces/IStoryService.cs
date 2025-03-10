using RSIHackerChallenge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSIHackerChallenge.Business.Interfaces
{
    public interface IStoryService
    {
        Task<ApiResponse> GetStories(string searchText, int pageNumber,int pageSize);

        Task<HttpResponseMessage> GetStoryByIdAsync(int id);

        Task<Story> GetStoryAsync(int storyId);

    }
}
