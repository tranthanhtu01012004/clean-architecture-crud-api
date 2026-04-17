namespace CleanCrudDemo.Worker.Options;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string NewsQueue { get; set; } = "news-events";
    public string MenuQueue { get; set; } = "menu-events";
}