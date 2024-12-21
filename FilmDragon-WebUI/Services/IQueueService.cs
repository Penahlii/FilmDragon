namespace FilmDragon_WebUI.Services;

public interface IQueueService
{
    Task SendMessageAsync(string message);
    Task<string> ReceiveMessageAsync();
}
