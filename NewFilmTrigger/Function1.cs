using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NewFilmTrigger.Models;
using StackExchange.Redis;

namespace NewFilmTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly HttpClient _httpClient;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { { "your-endpoint", 15072 } },
                User = "default",
                Password = "your-password"
            });

            _db = _redis.GetDatabase();
        }

        [Function(nameof(Function1))]
        public async Task RunAsync([QueueTrigger("film-poster-links", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            try
            {
                string filmName = message.MessageText;
                _logger.LogInformation($"Processing film name: {filmName}");

                // Call OMDb API to get poster link
                string omdbApiKey = Environment.GetEnvironmentVariable("OMDbApiKey");
                string omdbUrl = $"http://www.omdbapi.com/?t={Uri.EscapeDataString(filmName)}&apikey={omdbApiKey}";

                var response = await _httpClient.GetAsync(omdbUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch data from OMDb API for film: {filmName}");
                    return;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var omdbData = System.Text.Json.JsonSerializer.Deserialize<OmdbResponse>(responseContent);

                if (omdbData == null || string.IsNullOrEmpty(omdbData.Poster))
                {
                    _logger.LogWarning($"No poster found for film: {filmName}");
                    return;
                }

                string posterLink = omdbData.Poster;

                await _db.ListRightPushAsync("movie_posters", posterLink);
                _logger.LogInformation($"Poster for '{filmName}' added to Redis: {posterLink}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing queue message: {ex.Message}");
            }
        }
    }
}
