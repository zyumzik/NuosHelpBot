using NuosHelpBot.Models;
using System.Configuration;
using System.Text;

namespace NuosHelpBot;

public class BotMessageFormatter
{
    private Bot _bot;

    public string EmptyDay =>
        _bot.Configuration.Get("emptyDayText");

    public BotMessageFormatter(Bot bot)
    {
        _bot = bot;
    }

    public string ScheduleToString(IEnumerable<Class> schedule, int week = 0, int day = 0, bool formatDate = false)
    {
        var sb = new StringBuilder();

        if (formatDate)
        {
            var date = DateTime.Now;

            int difference = 0;

            if (week != _bot.TimeManager.CurrentWeek) difference += 7;
                
            if (day > (int)DateTime.Now.DayOfWeek) difference += day - (int)DateTime.Now.DayOfWeek;
            else if (day < (int)DateTime.Now.DayOfWeek) difference -= (int)DateTime.Now.DayOfWeek - day;

            date = date.AddDays(difference);

            sb.Append($"{date.Day} {MonthToString(date.Month)}, ");
        }

        if (day != 0 && week != 0)
            sb.Append($"{DayToString(day)}, {week} тиждень\n\n");

        if (schedule.Count() == 0) sb.Append(EmptyDay);

        foreach (var item in schedule)
        {
            if (item.Subgroup.Type != 0) sb.Append($"<b>| {item.Subgroup.Type} підгрупа |</b>\n");
            sb.Append(item.ToString() + "\n\n");
        }

        return sb.ToString();
    }

    public string ScheduleToNotify(IEnumerable<Class> schedule)
    {
        var sb = new StringBuilder();
        var minutes = _bot.Configuration.Get("minutesBeforeClassNotify");
        sb.Append($"Наступна пара через {minutes} хвилин:");
        foreach (var item in schedule)
        {
            sb.Append("\n\n" + item.ToString());
        }
        return sb.ToString();
    }

    public static string DayToString(int day)
    {
        switch (day)
        {
            case 1: return "Понеділок";
            case 2: return "Вівторок";
            case 3: return "Середа";
            case 4: return "Четверг";
            case 5: return "П'ятниця";
            case 6: return "Субота";
            case 7: return "Неділя";
        }
        return "undefined";
    }

    public static string MonthToString(int month)
    {
        switch (month)
        {
            case 1: return "Січня";
            case 2: return "Лютого";
            case 3: return "Березня";
            case 4: return "Квітня";
            case 5: return "Травня";
            case 6: return "Червня";
            case 7: return "Липня";
            case 8: return "Серпня";
            case 9: return "Вересня";
            case 10: return "Жовтня";
            case 11: return "Листопада";
            case 12: return "Грудня";
        }
        return "undefined";
    }
}
