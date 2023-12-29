using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Commands;

public class ScheduleWeekCommand : Command
{
    public override string Name => "Розклад на тиждень 📅";

    public override async Task Execute(Bot bot, Message message)
    {
        var msg = await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Завантаження...");

        int currentWeek = bot.TimeManager.CurrentWeek;
        int currentDay = bot.TimeManager.CurrentDay;

        var schedule = await bot.Context.GetSchedule(message.From.Id, currentWeek, currentDay);

        var text = bot.MessageFormatter.ScheduleToString(schedule, currentWeek, currentDay, true);
        var keyboard = bot.KeyboardController.ScheduleWeekKeyboard(msg.MessageId, currentWeek, currentDay);

        await bot.Client.EditMessageTextAsync(
            message.Chat,
            msg.MessageId,
            text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: keyboard);
    }
}
