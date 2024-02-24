using Telegram.Bot.Types.ReplyMarkups;
using NuosHelpBot.Models;
using NuosHelpBot.Extensions;

namespace NuosHelpBot;

public class Keyboards
{
    static public ReplyKeyboardMarkup MainMenu =>
        new ReplyKeyboardMarkup(new KeyboardButton[][]
        {
            new KeyboardButton[] { "Розклад на сьогодні ⏰" },
            new KeyboardButton[] { "Розклад на тиждень 📅", "Розклад групи 🎓" },
            new KeyboardButton[] { "Додатково ℹ", "Налаштування ⚙" }
        })
        { ResizeKeyboard = true };

    static public ReplyKeyboardMarkup SettingsMenu =>
        new ReplyKeyboardMarkup(new KeyboardButton[][]
        {
            new KeyboardButton[] { "Сповіщання 🔔" },
            new KeyboardButton[] { "Змінити групу 🎓" },
            new KeyboardButton[] { "◀ Головне меню" }
        })
        { ResizeKeyboard = true };

    static public InlineKeyboardMarkup ChooseGroupKb =>
        new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("Обрати групу", "/chooseGroup"));

    static public InlineKeyboardMarkup EducationFormKb =>
        new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Бакалавр денна форма", "/setEducationForm FT B"),
                InlineKeyboardButton.WithCallbackData("Магістр денна форма", "/setEducationForm FT M"),
            },
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData("Бакалавр заочна форма", "/setEducationForm PT B"),
                InlineKeyboardButton.WithCallbackData("Магістр заочна форма", "/setEducationForm PT M"),
            }
        });

    static public InlineKeyboardMarkup SetNotifyingKb =>
        new InlineKeyboardMarkup(new InlineKeyboardButton[] {
            InlineKeyboardButton.WithCallbackData("Ввімкнути сповіщання 🔔", "/setNotifications true"),
            InlineKeyboardButton.WithCallbackData("Вимкнути сповіщання 🔕", "/setNotifications false")});

    static public InlineKeyboardMarkup CoursesKb(string educationForm, string educationLevel, int semester)
    {
        if (educationForm == "FT")
        {
            if (educationLevel == "B")  // autumn semester
            {
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("1 курс", "/setCourse 1 FT B"),
                        InlineKeyboardButton.WithCallbackData("2 курс", "/setCourse 2 FT B"),
                        InlineKeyboardButton.WithCallbackData("3 курс", "/setCourse 3 FT B"),
                        InlineKeyboardButton.WithCallbackData("4 курс", "/setCourse 4 FT B")
                    });
            }
            if (educationLevel == "M")
            {
                if (semester == 1)
                {
                    return new InlineKeyboardMarkup(
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("5 курс", "/setCourse 5 FT M"),
                        InlineKeyboardButton.WithCallbackData("6 курс", "/setCourse 6 FT M")
                    });
                }
                if (semester == 2)
                {
                    return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("5 курс", "/setCourse 5 FT M"));
                }
            }
        }

        if (educationForm == "PT")
        {
            if (educationLevel == "B")  // autumn semester
            {
                return new InlineKeyboardMarkup(
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("1 курс", "/setCourse 1 PT B"),
                        InlineKeyboardButton.WithCallbackData("2 курс", "/setCourse 2 PT B"),
                        InlineKeyboardButton.WithCallbackData("3 курс", "/setCourse 3 PT B"),
                        InlineKeyboardButton.WithCallbackData("4 курс", "/setCourse 4 PT B"),
                        InlineKeyboardButton.WithCallbackData("5 курс", "/setCourse 5 PT B")
                    });
            }
            if (educationLevel == "M")
            {
                if (semester == 1)
                {
                    return new InlineKeyboardMarkup(
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("5 курс", "/setCourse 5 PT M"),
                        InlineKeyboardButton.WithCallbackData("6 курс", "/setCourse 6 PT M")
                    });
                }
                if (semester == 2)
                {
                    return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("5 курс", "/setCourse 5 PT M"));
                }
            }
        }

        return null;
    }

    static public InlineKeyboardMarkup GroupsKb(IEnumerable<Group> groups)
    {
        int width = 5;
        int counter = 0;
        List<List<InlineKeyboardButton>> buttons = new();
        while (counter != groups.Count())
        {
            List<InlineKeyboardButton> newRow = new();
            for (int i = 0; i < width; i++)
            {
                if (counter == groups.Count()) break;
                string groupCode = groups.ElementAt(counter).Code;
                string groupId = groups.ElementAt(counter).Id.ToString();
                newRow.Add(InlineKeyboardButton.WithCallbackData(
                    groupCode,
                    $"/setGroup {groupId}"));
                counter++;
            }
            buttons.Add(newRow);
        }
        return new InlineKeyboardMarkup(buttons);
    }

    static public InlineKeyboardMarkup ScheduleGroupKb(int messageId, int currentWeek, int currentDay)
    {
        int nextDay = currentDay + 1 > 5 ? 1 : currentDay + 1;
        int prevDay = currentDay - 1 <= 0 ? 5 : currentDay - 1;
        int week = currentWeek == 1 ? 2 : 1;
        return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"◀   { prevDay.ToDay() }",
                    $"/getScheduleGroup {messageId} {currentWeek} {prevDay}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{ nextDay.ToDay() }   ▶",
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

    static public InlineKeyboardMarkup ScheduleWeekKb(int messageId, int currentWeek, int currentDay)
    {
        int nextDay = currentDay + 1 > 5 ? 1 : currentDay + 1;
        int prevDay = currentDay - 1 <= 0 ? 5 : currentDay - 1;
        int week = currentWeek == 1 ? 2 : 1;
        return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                InlineKeyboardButton.WithCallbackData(
                    $"◀   { prevDay.ToDay() }",
                    $"/getScheduleWeek {messageId} {currentWeek} {prevDay}"),
                InlineKeyboardButton.WithCallbackData(
                    $"{nextDay.ToDay() }   ▶",
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
}
