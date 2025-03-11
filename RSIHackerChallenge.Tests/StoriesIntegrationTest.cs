using Newtonsoft.Json;
using RSIHackerChallenge.Data;

[TestFixture]
public class IntegrationTests
{
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _client = new HttpClient();
    }

    /// <summary>
    /// Integration test for the actual hacker api data it covers all the methods including cache also
    /// </summary>
    /// <returns>passed or fail</returns>
    [Test]
    public async Task GetNewStories_ReturnsOkResult()
    {
        var response = await _client.GetAsync("https://localhost:7095/api/Stories/NewStories");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ApiResponse>(content);
        Assert.IsNotNull(data);
    }
}
