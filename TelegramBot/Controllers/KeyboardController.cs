using Telegram.Bot.Types.ReplyMarkups;
using NuosHelpBot.Models;

namespace NuosHelpBot.Controllers;

public class KeyboardController
{
    public ReplyKeyboardMarkup MainMenu =>
        new ReplyKeyboardMarkup(new KeyboardButton[][]
        {
            new KeyboardButton[] { "Розклад на сьогодні ⏰" },
            new KeyboardButton[] { "Розклад на тиждень 📅", "Розклад групи 🎓" },
            new KeyboardButton[] { "Додатково ℹ", "Налаштування ⚙" }
        })
        { ResizeKeyboard = true };

    public ReplyKeyboardMarkup SettingsMenu =>
        new ReplyKeyboardMarkup(new KeyboardButton[][]
        {
            new KeyboardButton[] { "Сповіщання 🔔" },
            new KeyboardButton[] { "Змінити групу 🎓", "Змінити підгрупу 🥼" },
            new KeyboardButton[] { "◀ Головне меню" }
        })
        { ResizeKeyboard = true };

    public InlineKeyboardMarkup ChooseGroup =>
        new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("Обрати групу", "/chooseGroup"));

    public InlineKeyboardMarkup SetNotifying =>
        new InlineKeyboardMarkup(new InlineKeyboardButton[] {
            InlineKeyboardButton.WithCallbackData("Ввімкнути сповіщання 🔔", "/setNotifications true"),
            InlineKeyboardButton.WithCallbackData("Вимкнути сповіщання 🔕", "/setNotifications false")});

    public InlineKeyboardMarkup ScheduleGroupKeyboard(int messageId, int currentWeek, int currentDay)
    {
        int nextDay = currentDay + 1 > 5 ? 1 : currentDay + 1;
        int prevDay = currentDay - 1 <= 0 ? 5 : currentDay - 1;
        int week = currentWeek == 1 ? 2 : 1;
        return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"◀   { BotMessageFormatter.DayToString(prevDay) }", 
                    $"/getScheduleGroup {messageId} {currentWeek} {prevDay}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{ BotMessageFormatter.DayToString(nextDay) }   ▶", 
                    $"/getScheduleGroup {messageId} {currentWeek} {nextDay}")
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"🔽     {week} тиждень     🔽", 
                    $"/getScheduleGroup {messageId} {week} 1")
            }
        });
    }

    public InlineKeyboardMarkup ScheduleWeekKeyboard(int messageId, int currentWeek, int currentDay)
    {
        int nextDay = currentDay + 1 > 5 ? 1 : currentDay + 1;
        int prevDay = currentDay - 1 <= 0 ? 5 : currentDay - 1;
        int week = currentWeek == 1 ? 2 : 1;
        return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"◀   { BotMessageFormatter.DayToString(prevDay) }", 
                    $"/getScheduleWeek {messageId} {currentWeek} {prevDay}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{BotMessageFormatter.DayToString(nextDay) }   ▶",
                    $"/getScheduleWeek {messageId} {currentWeek} {nextDay}")
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"🔽     {week} тиждень     🔽",
                    $"/getScheduleWeek {messageId} {week} 1")
            }
        });
    }

    public InlineKeyboardMarkup GroupsInlineKeyboard(IEnumerable<Group> groups, bool setSubroup = false)
    {
        var groupsList = groups.ToList();
        int width = 5;
        int counter = 0;
        List<List<InlineKeyboardButton>> buttons = new();
        while (counter != groups.Count())
        {
            List<InlineKeyboardButton> newRow = new();
            for (int i = 0; i < width; i++)
            {
                if (counter == groups.Count()) break;
                string code = groupsList[counter].Code.ToString();
                newRow.Add(InlineKeyboardButton.WithCallbackData(
                    code,
                    $"/setGroup {code} {setSubroup}"));
                counter++;
            }
            buttons.Add(newRow);
        }
        return new InlineKeyboardMarkup(buttons);
    }

    public InlineKeyboardMarkup SubgroupsInlineKeyboard(IEnumerable<Subgroup> subgroups, bool setNotifications = false)
    {
        var buttons = new List<InlineKeyboardButton>();
        foreach (var subgroup in subgroups)
        {
            if (subgroup.Type == 0) continue;
            buttons.Add(InlineKeyboardButton.WithCallbackData(
                $"Підгрупа {subgroup.Type}",
                $"/setSubgroup {subgroup.Type} {setNotifications}"));
        }
        return new InlineKeyboardMarkup(buttons);
    }
}
