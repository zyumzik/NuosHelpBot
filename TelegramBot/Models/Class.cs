using System.Text;

namespace NuosHelpBot.Models;

public class Class
{
    public int Id { get; set; }
    public Time Time { get; set; }
    public ClassType ClassType { get; set; }
    public Discipline Discipline { get; set; }
    public Teacher Teacher { get; set; }
    public Group Group { get; set; }
    public Subgroup Subgroup { get; set; }
    public int Day { get; set; }
    public int Week { get; set; }
    public string Audience { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"[ {Time.Number} ] " +
            $"{Time.StartTime.ToString(@"hh\:mm")} - " +
            $"{Time.EndTime.ToString(@"hh\:mm")}\n");
        sb.Append($"<b>{ClassType.Name}</b> {Discipline.Name}\n");
        sb.Append($"{Teacher.Position.Name} <strong>{Teacher.Name}</strong>");
        if (!string.IsNullOrEmpty(Teacher.Contacts)) sb.Append(Teacher.Contacts);
        if (!string.IsNullOrEmpty(Audience)) sb.Append($"\nАудиторія: {Audience}");
        if (!string.IsNullOrEmpty(Discipline.ClassLink)) sb.Append($"\nКлас: [ {Discipline.ClassLink} ]");
        if (!string.IsNullOrEmpty(Discipline.MeetLink)) sb.Append($"\nМіт: [ {Discipline.MeetLink} ]");
        return sb.ToString();
    }
}
