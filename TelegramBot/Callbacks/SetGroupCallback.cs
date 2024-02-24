using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Callbacks;

class SetGroupCallback : Callback
{
    public override string Name => "/setGroup";  // arg[0]: groupId

    public override bool Contains(CallbackQuery query)
    {
        return base.Contains(query);
    }

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        var groupId = int.Parse(args[0]);
        var chosenGroupCode = "";

        using (var context = new BotContext())
        {
            chosenGroupCode = context.SetStudentGroup(query.From.Id, groupId);
        }

        var keyboard = Keyboards.MainMenu;
        var text = $"Групу обрано: {chosenGroupCode}";

        await bot.Client.SendTextMessageAsync(
            query.Message.Chat.Id,
            text,
            replyMarkup: keyboard);
    }
}
