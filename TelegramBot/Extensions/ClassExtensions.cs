using NuosHelpBot.Models;
using System.Configuration;
using System.Data;
using System.Text;

namespace NuosHelpBot.Extensions;

public static class ClassExtensions
{
    public static string ToString(this Class @class)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"<b>🕙 | {@class.Time.Number} | " +
            $"{@class.Time.StartTime.ToString(@"hh\.mm")} -" +
            $"{@class.Time.EndTime.ToString(@"hh\.mm")} | " +
            $"{@class.ClassType.Name}</b>");
        sb.AppendLine($"📚 | {@class.Discipline.Name}");
        sb.AppendLine($"👨‍🏫 | {@class.Teacher.Name}");

        if (!string.IsNullOrEmpty(@class.Audience))
            sb.AppendLine($"🏫 | Ауд: {@class.Audience}");

        return sb.ToString();
    }

    public static string ToString(this IEnumerable<Class> classes, 
        int week = 0, int day = 0, bool specifyDate = false)
    {
        var sb = new StringBuilder();

        sb.Append("<i>📆 | ");
        if (specifyDate)
        {
            var date = DateTime.Now;

            int difference = 0;

            if (week != BotTimeManager.CurrentWeek) difference += 7;

            if (day > (int)DateTime.Now.DayOfWeek) difference += day - (int)DateTime.Now.DayOfWeek;
            else if (day < (int)DateTime.Now.DayOfWeek) difference -= (int)DateTime.Now.DayOfWeek - day;

            date = date.AddDays(difference);

            sb.Append($"{date.Day} {date.Month.ToMonth()}, ");
        }

        if (day != 0 && week != 0)
            sb.Append($"{day.ToDay()}, {week} тиждень\n\n");
        sb.Append("</i>");

        if (classes.Count() == 0) sb.Append(ConfigurationManager.AppSettings["emptyDayText"]);

        foreach (var item in classes)
        {
            sb.Append(ToString(item) + "\n");
        }

        return sb.ToString();
    }

    public static string ToNotify(this IEnumerable<Class> classes)
    {
        if (classes.Count() <= 0) return string.Empty;

        var sb = new StringBuilder();
        var minutes = ConfigurationManager.AppSettings["minutesBeforeClassNotify"];

        sb.Append($"Наступна пара через {minutes} хвилин: ");
        foreach (var item in classes)
        {
            sb.Append(item.ToString());
        }

        return sb.ToString();
    }
}
