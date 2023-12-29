using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Commands;

public class InfoCommand : Command
{
    public override string Name => "Додатково ℹ";

    public override async Task Execute(Bot bot, Message message)
    {
        await bot.Client.SendTextMessageAsync(
            message.Chat,
            bot.Configuration.Get("additionalInfoText"));
    }
}
