using Telegram.Bot;
using Telegram.Bot.Types;
using NuosHelpBot.Models;

namespace NuosHelpBot.Callbacks;

public class ChooseGroupCallback : Callback
{
    public override string Name => "/chooseGroup";

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var groups = await bot.Context.GetRawTable<Group>("Groups");
        var keyboard = bot.KeyboardController.GroupsInlineKeyboard(groups, true);

        await bot.Client.SendTextMessageAsync(
            query.Message.Chat.Id,
            $"Оберіть вашу групу",
            replyMarkup: keyboard);
    }
}
