namespace NuosHelpBot.Models;

public class Time
{
    public int Id { get; set; }
    public int Number { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
