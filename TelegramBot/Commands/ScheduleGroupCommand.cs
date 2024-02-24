using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using NuosHelpBot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Commands;

public class ScheduleGroupCommand : Command
{
    public override string Name => "Розклад групи 🎓";

    public override async Task Execute(Bot bot, Message message)
    {
        var msg = await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Завантаження...");

        string text;
        InlineKeyboardMarkup keyboard;

        using (var context = new BotContext())
        {
            var semester = BotTimeManager.CurrentSemester;
            var classes = context.GetClasses(message.From.Id, 1, 1, semester);
            text = classes.ToString(1, 1);
            keyboard = Keyboards.ScheduleGroupKb(msg.MessageId, 1, 1);
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
