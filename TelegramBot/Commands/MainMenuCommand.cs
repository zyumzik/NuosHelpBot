using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Commands;

public class MainMenuCommand : Command
{
    public override string Name => "◀ Головне меню";

    public override async Task Execute(Bot bot, Message message)
    {
        var keyboard = Keyboards.MainMenu;

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Повернення до головного меню...",
            replyMarkup: keyboard);
    }
}
