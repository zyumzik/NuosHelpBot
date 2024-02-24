using NuosHelpBot.Extensions;
using NuosHelpBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Callbacks;

public class GetScheduleWeekCallback : Callback
{
    public override string Name => "/getScheduleWeek";  // [0] editMessageId, [1] - week, [2] - day

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        var messageId = int.Parse(args[0]);
        var week = int.Parse(args[1]);
        var day = int.Parse(args[2]);

        string text;
        InlineKeyboardMarkup keyboard;

        using (var context = new BotContext())
        {
            var semester = BotTimeManager.CurrentSemester;
            var classes = context.GetClasses(query.From.Id, week, day, semester);
            text = classes.ToString(week, day, true);
            keyboard = Keyboards.ScheduleWeekKb(messageId, week, day);
        }

        try
        {
            await bot.Client.EditMessageTextAsync(
                query.From.Id,
                messageId,
                text,
                parseMode: ParseMode.Html,
                disableWebPagePreview: true,
                replyMarkup: keyboard);
        }
        catch (Exception e) { Console.WriteLine($">>Error: message {messageId} was not edited. {e}"); }
    }
}
