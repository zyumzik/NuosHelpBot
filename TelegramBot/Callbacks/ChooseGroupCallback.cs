using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Callbacks;

public class ChooseGroupCallback : Callback
{
    public override string Name => "/chooseGroup";

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var keyboard = Keyboards.EducationFormKb;

        await bot.Client.SendTextMessageAsync(
            query.From.Id,
            "Оберіть форму та рівень освіти: ",
            replyMarkup: keyboard);
    }
}
