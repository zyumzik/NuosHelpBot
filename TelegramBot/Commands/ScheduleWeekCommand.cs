using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NuosHelpBot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Commands;

public class ScheduleWeekCommand : Command
{
    public override string Name => "Розклад на тиждень 📅";

    public override async Task Execute(Bot bot, Message message)
    {
        var msg = await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Завантаження...");

        string text;
        InlineKeyboardMarkup keyboard;

        using (var context = new BotContext())
        {
            var week = BotTimeManager.CurrentWeek;
            var day = BotTimeManager.CurrentDay;

            if (day > 5)
            {
                day = 1;
                week = week == 1 ? 2 : 1;
            }

            var semester = BotTimeManager.CurrentSemester;
            var classes = context.GetClasses(message.From.Id, week, day, semester);
            text = classes.ToString(week, day, true);
            keyboard = Keyboards.ScheduleWeekKb(msg.MessageId, week, day);
        }

        await bot.Client.EditMessageTextAsync(
            message.Chat,
            msg.MessageId,
            text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: keyboard);
    }
}
