using StackExchange.Redis;

namespace FilmDragon_WebUI.Services;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService()
    {
        _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = { { "your-endpoint", 15072 } },
            User = "default",
            Password = "your-password"
        });

        _db = _redis.GetDatabase();
    }

    public async Task<List<string>> GetNewPostersAsync()
    {
        var posters = new List<string>();

        var length = await _db.ListLengthAsync("movie_posters");
        for (long i = 0; i < length; i++)
        {
            string? poster = await _db.ListLeftPopAsync("movie_posters");
            if (!string.IsNullOrEmpty(poster))
            {
                posters.Add(poster);
            }
        }

        return posters;
    }
}
