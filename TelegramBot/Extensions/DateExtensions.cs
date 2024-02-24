namespace NuosHelpBot.Extensions;

public static class DateExtensions
{
    public static string ToDay(this int day)
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
            default: return "undefined";
        }
    }

    public static string ToMonth(this int month)
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
            default: return "undefined";
        }
    }
}
