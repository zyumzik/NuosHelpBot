using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Commands;

public class ScheduleGroupCommand : Command
{
    public override string Name => "Розклад групи 🎓";

    public override async Task Execute(Bot bot, Message message)
    {
        var msg = await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Завантаження...");

        var schedule = await bot.Context.GetSchedule(message.From.Id, 1, 1, false);
        var text = bot.MessageFormatter.ScheduleToString(schedule, 1, 1);
        var keyboard = bot.KeyboardController.ScheduleGroupKeyboard(msg.MessageId, 1, 1);

        await bot.Client.EditMessageTextAsync(
            message.Chat, 
            msg.MessageId,
            text,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true,
            replyMarkup: keyboard);
    }
}
