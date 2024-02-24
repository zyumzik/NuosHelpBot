using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Commands;

public class NotificationsSettingsCommand : Command
{
    public override string Name => "Сповіщання 🔔";

    public override async Task Execute(Bot bot, Message message)
    {
        var keyboard = Keyboards.SetNotifyingKb;

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Чи бажаєте ви отримувати повідомлення про початок пар?",
            replyMarkup: keyboard);
    }
}
