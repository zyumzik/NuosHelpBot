namespace NuosHelpBot.Models;

public class Student
{
    public int Id { get; set; }
    public string TelegramName { get; set; }
    public long TelegramId { get; set; }
    public Group Group { get; set; }
    public Subgroup Subgroup { get; set; }
    public bool Notify { get; set; }
}
