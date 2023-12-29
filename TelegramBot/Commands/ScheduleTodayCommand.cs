using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Commands;

public class ScheduleTodayCommand : Command
{
    public override string Name => "Розклад на сьогодні ⏰";

    public override async Task Execute(Bot bot, Message message)
    {
        var currentWeek = bot.TimeManager.CurrentWeek;
        var currentDay = bot.TimeManager.CurrentDay;

        var schedule = await bot.Context.GetSchedule(message.From.Id, currentWeek, currentDay);

        var text = bot.MessageFormatter.ScheduleToString(schedule, currentWeek, currentDay, true);

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            text,
            parseMode: ParseMode.Html, 
            disableWebPagePreview: true);
    }
}
