namespace NuosHelpBot.Models;

public class User
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public bool Notify { get; set; }

    public int? GroupId;
    public Group? Group { get; set; }
}
