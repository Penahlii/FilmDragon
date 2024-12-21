using Azure.Storage.Queues;

namespace FilmDragon_WebUI.Services;

public class QueueService : IQueueService
{
    private readonly QueueClient _queueClient;
    public QueueService(string connectionString, string queueName)
    {
        _queueClient = new QueueClient(connectionString, queueName);
        _queueClient.CreateIfNotExists();
    }

    public async Task<string> ReceiveMessageAsync()
    {
        var messageResponse = await _queueClient.ReceiveMessageAsync();
        if (messageResponse.Value != null)
        {
            string message = messageResponse.Value.Body.ToString();
            await _queueClient.DeleteMessageAsync(messageResponse.Value.MessageId, messageResponse.Value.PopReceipt);
            return message;
        }
        return null;
    }

    public async Task SendMessageAsync(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            await _queueClient.SendMessageAsync(message, TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(10));
        }
    }
}
