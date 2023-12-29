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
        var studentExists = await bot.Context.StudentExists(message.From.Id);
        if (!studentExists)
        {
            await bot.Context.AddStudent(message.From.Username, message.From.Id);
            keyboard = bot.KeyboardController.ChooseGroup;
        }
        else keyboard = bot.KeyboardController.MainMenu;
        var text = bot.Configuration.Get("startText");

        await bot.Client.SendTextMessageAsync(
            message.Chat.Id, 
            text, 
            //parseMode: ParseMode.Html,
            replyMarkup: keyboard);
    }
}
