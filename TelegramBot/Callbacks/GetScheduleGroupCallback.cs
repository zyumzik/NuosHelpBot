using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Callbacks;

public class GetScheduleGroupCallback : Callback
{
    public override string Name => "/getScheduleGroup";  // [0] editMessageId, [1] - week, [2] - day

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        var messageId = int.Parse(args[0]);
        var week = int.Parse(args[1]);
        var day = int.Parse(args[2]);

        var schedule = await bot.Context.GetSchedule(query.From.Id, week, day, false);

        var text = bot.MessageFormatter.ScheduleToString(schedule, week, day);
        var keyboard = bot.KeyboardController.ScheduleGroupKeyboard(messageId, week, day);

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
        catch (Exception ex) { Console.WriteLine($">>Error: message {messageId} was not edited"); }
    }
}
