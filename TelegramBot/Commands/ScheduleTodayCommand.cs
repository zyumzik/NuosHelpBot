using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NuosHelpBot.Extensions;

namespace NuosHelpBot.Commands;

public class ScheduleTodayCommand : Command
{
    public override string Name => "Розклад на сьогодні ⏰";

    public override async Task Execute(Bot bot, Message message)
    {
        var week = BotTimeManager.CurrentWeek;
        var day = BotTimeManager.CurrentDay;
        var semester = BotTimeManager.CurrentSemester;

        var text = "";
        
        using(var context = new BotContext())
        {
            var classes = context.GetClasses(message.From.Id, week, day, semester);
            text = classes.ToString(week, day, true);
        }

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            text,
            parseMode: ParseMode.Html, 
            disableWebPagePreview: true);
    }
}
