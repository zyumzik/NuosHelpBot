using Telegram.Bot;
using Telegram.Bot.Types;
using NuosHelpBot.Models;

namespace NuosHelpBot.Commands;

public class ChangeSubgroupCommand : Command
{
    public override string Name => "Змінити підгрупу 🥼";

    public override async Task Execute(Bot bot, Message message)
    {
        var subgroups = await bot.Context.GetRawTable<Subgroup>("Subgroups");

        var keyboard = bot.KeyboardController.SubgroupsInlineKeyboard(subgroups);

        await bot.Client.SendTextMessageAsync(
            message.Chat,
            "Оберіть нову підгрупу",
            replyMarkup: keyboard);
    }
}
