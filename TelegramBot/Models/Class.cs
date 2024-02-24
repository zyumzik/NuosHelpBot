using System.Text;

namespace NuosHelpBot.Models;

public class Class
{
    public int Id { get; set; }
    public int Day { get; set; }
    public int Week { get; set; }
    public string? Audience { get; set; }
    public int Semester { get; set; }

    public int TimeId;
    public Time Time { get; set; } = null!;

    public int ClassTypeId;
    public ClassType ClassType { get; set; } = null!;

    public int DisciplineId;
    public Discipline Discipline { get; set; } = null!;

    public int TeacherId;
    public Teacher Teacher { get; set; } = null!;

    public int GroupId;
    public Group Group { get; set; } = null!;
}
