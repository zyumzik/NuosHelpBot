using Telegram.Bot.Types;
using NuosHelpBot.Models;
using Telegram.Bot;

namespace NuosHelpBot.Commands;

public class ChangeGroupCommand : Command
{
    public override string Name => "Змінити групу 🎓";

    public override async Task Execute(Bot bot, Message message)
    {
        var groups = await bot.Context.GetRawTable<Group>("Groups");

        var keyboard = bot.KeyboardController.GroupsInlineKeyboard(groups);

        await bot.Client.SendTextMessageAsync(
            message.Chat,
            "Оберіть нову групу",
            replyMarkup: keyboard);
    }
}
