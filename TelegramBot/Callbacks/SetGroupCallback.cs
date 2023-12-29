using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using NuosHelpBot.Models;

namespace NuosHelpBot.Callbacks;

class SetGroupCallback : Callback
{
    public override string Name => "/setGroup";  // arg[0] - groupCode, arg[1] - setSubgroup

    public override bool Contains(CallbackQuery query)
    {
        return base.Contains(query);
    }

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        var groupCode = args[0];
        bool setSubgroup = bool.Parse(args[1]);

        await bot.Context.SetStudentGroup(query.From.Id, groupCode);

        var subgroups = await bot.Context.GetRawTable<Subgroup>("Subgroups");
        IReplyMarkup keyboard = setSubgroup
            ? bot.KeyboardController.SubgroupsInlineKeyboard(subgroups, true)
            : bot.KeyboardController.MainMenu;
        var text = $"Обрано групу {groupCode}";
        text += setSubgroup ? ", тепер оберіть підгрупу" : "";

        await bot.Client.SendTextMessageAsync(
            query.Message.Chat.Id,
            text,
            replyMarkup: keyboard);
    }
}
