using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Commands;

public class StartCommand : Command
{
    public override string Name => "/start";

    public override async Task Execute(Bot bot, Message message)
    {
        IReplyMarkup keyboard;

        using (var context = new BotContext())
        {
            if (!context.UserExists(message.From.Id))
            {
                context.AddUser(new()
                {
                    TelegramId = message.From.Id
                });
                keyboard = Keyboards.ChooseGroupKb;
            }
            else 
                keyboard = Keyboards.MainMenu;
        }

        var text = ConfigurationManager.AppSettings["startText"];

        await bot.Client.SendTextMessageAsync(
            message.Chat.Id, 
            text,
            replyMarkup: keyboard);
    }
}
