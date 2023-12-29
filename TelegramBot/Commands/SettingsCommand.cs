using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Commands;

public class SettingsCommand : Command
{
    public override string Name => "Налаштування ⚙";

    public override async Task Execute(Bot bot, Message message)
    {
        var keyboard = bot.KeyboardController.SettingsMenu;

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Перехід до налаштувань...",
            replyMarkup: keyboard);
    }
}
